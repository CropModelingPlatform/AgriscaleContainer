Option Compare Text
Option Explicit On
Imports System.Math
Imports System.Data
Imports System.Data.Sqlite
Public Class MulchClass
    Dim Qpaillis(0 To 731) As Double 'VE quantité de paillis présente au sol chaque jour (Mg/ha)
    'inutile
    'Dim Qpaillis0 As Double 'VX Quantité de paillis au 1er jour de simulation (Mg/ha)
    Dim Eomulch As Double 'VE, évaporation potentielle du mulch (mm)
    Dim Emulch(0 To 731) As Double 'VE, évaporation du mulch (mm)
    Dim Smulch As Double 'VE, stock en eau du mulch (mm)
    Dim EauVersSol As Double 'VE, quantité d'eau disponible sous le mulch pour infiltration dans le sol (mm)
    Dim Eau_vers_Mulch As Double 'VE quantité d'eau disponible après ruissellement pour infiltration dans le sol et le mulch(mm)
    Dim dataMulchAdp As SqliteDataAdapter
    Dim rstMulchData As DataSet
    Dim gamma_mulch As Double 'VX, Coefficient d'extinction de l'évapotranspiration potentielle par le mulch
    Dim CapaciteWMulch As Double 'VX, capacité de stockage de l'eau par le mulch en mm/T/ha de mulch
    Dim Alpha_pail As Double 'VX, exp(-Alph_pail) est le taux de disparition quotidienne du paillis
    Dim FracSoilCover As Double 'VE fraction du sol couverte par le paillis (sd)
    Dim Beta_pail As Double 'VX, pouvoir couvrant du paillis (ha/T DM)
    'Dim a_ruis As Double 'VX, coefficient de ruissellement en absence de paillis
    Dim b_ruis As Double    'VX, coefficient d'augmentation du ruissellement par le paillis (est en général négatif car le paillis réduit le ruissellement)
    'Dim seuil_ruis As Double 'VX, seuil de précipitations en-dessous duquel il n'y a pas de ruissellement
    Dim Ruis(0 To 731) As Double 'VE, ruissellement journalier (mm)
    Dim SigmaSimEmulch As Double 'VS cumul sur la simulation de Emulch (mm)
    Dim SigmaSimRuis As Double 'VS cumul sur la simulation de Ruis (mm)
    Dim SigmaSimPluM As Double 'VS, cumul sur la simulation de la pluie parvenant à la couche de mulch (avant ruissellement en surface) (mm)
    Dim IKJ As Double 'VE indice d'antériorité des pluies (Albergel et al.) pour le calcul du ruissellement au sahel
    Dim RuisEtrange As Boolean
    'Dernière modif 10/05/11
    ' dernière modif 28/04/13 ruissellement selon albergel et al combiné avec Scopel et al pour prise en compte paillis...
    'VERSION 3 du 8-10 nov 2017'
    Sub LisMulch(DataBase_Cnn As SqliteConnection, CodParamMulch As Integer, Qpaillisinit As Double)
        Dim Trouve As Boolean
        Dim msg As String
        rstMulchData = New DataSet
        dataMulchAdp = New SqliteDataAdapter("SELECT * FROM Mulch where idMulch=" & CodParamMulch, DataBase_Cnn)
        dataMulchAdp.Fill(rstMulchData)

        Trouve = False

        ' rstMulchData.Open("SELECT * FROM Mulch where idMulch=" & CodParamMulch, DataBase_Cnn)
        ' rstMulchData.MoveFirst()
        If rstMulchData.Tables(0).Rows.Count > 0 Then
            'While Not rstMulchData.EOF And Not Trouve
            'If rstMulchData!idMulch = CodParamMulch Then
            Trouve = True
            gamma_mulch = rstMulchData.Tables(0).Rows(0)("gamma_mulch")
            CapaciteWMulch = rstMulchData.Tables(0).Rows(0)("CapaciteWMulch")
            Alpha_pail = rstMulchData.Tables(0).Rows(0)("Alpha_pail")
            Beta_pail = rstMulchData.Tables(0).Rows(0)("Beta_pail")
            ' a_ruis = rstMulchData!a_ruis
            b_ruis = rstMulchData.Tables(0).Rows(0)("b_ruis")
            'seuil_ruis = rstMulchData!seuil_ruis
        End If
        'rstMulchData.MoveNext()
        'End While
        'rstMulchData.Close()
        'rstMulchData = Nothing
        Qpaillis(0) = Qpaillisinit
        If Qpaillisinit > 0 And Alpha_pail = 0 Then
            msg = "Attention! Paillis initial non nul dans ParamIni et pas de décomposition du paillage (alpha_pail=0 dans Mulch) ! Le paillis sera initialisé à 0"
            Console.WriteLine(msg)
            Qpaillis(0) = 0
        End If
    End Sub
    Sub LisRuiObs(DataBase_Cnn As SqliteConnection, SimUnit As SimulationUnitClass)

        Dim AnRuiObs As Integer
        Dim JourRuiObs As Integer
        Dim RuiObs As Double
        Dim joursim As Integer
        Dim ErrFa As Boolean
        Dim jourEnPlus As Integer
        Dim Ndyear1 As Integer
        Dim msg As String
        Dim NbreRuiObs As Integer
        Dim dataRuiObsAdp As SqliteDataAdapter
        Dim rstDataRuiObs As DataSet

        On Error GoTo Err_LisRuiObs

        If SimUnit.bSY_Bissextile Then Ndyear1 = 366 Else Ndyear1 = 365
        rstDataRuiObs = New DataSet
        dataRuiObsAdp = New SqliteDataAdapter("SELECT * FROM RuissellementObs where idTechCom='" & SimUnit.sIdTec & "' Order by YearRuiObs, JourRuiObs", DataBase_Cnn)
        dataMulchAdp.Fill(rstDataRuiObs)
        'dataRuiObsAdp = New ADODB.Recordset
        'Lecture table "RuissellementObs" à modifier si nécessaire
        'rstDataRuiObs.Open "SELECT * FROM RuissellementObs where idTechCom='" & SimUnit.sIdTec & "' Order by YearRuiObs, JourRuiObs", DataBase_Cnn, adOpenDynamic
        'rstDataRuiObs.MoveFirst


        'While Not rstDataRuiObs.EOF

        For i = 0 To rstDataRuiObs.Tables(0).Rows.Count - 1
            AnRuiObs = rstDataRuiObs.Tables(0).Rows(i)("YearRuiObs")
            JourRuiObs = rstDataRuiObs.Tables(0).Rows(i)("JourRuiObs")
            RuiObs = rstDataRuiObs.Tables(0).Rows(i)("RuiObs")

            If AnRuiObs = SimUnit.nStartYear Then
                    joursim = JourRuiObs - SimUnit.nStartDay + 1
                Else
                    joursim = JourRuiObs + Ndyear1 - SimUnit.nStartDay + 1
                End If
                If joursim < 0 Then
                    msg = "Attention, calendrier des irrigations incohérent avec calendrier de simulation"
                    GoTo ErrFA_LisRuiObs
                Else
                    Ruis(joursim) = RuiObs
                    NbreRuiObs = NbreRuiObs + 1
                End If
            Next
        'rstDataRuiObs.MoveNext
        'Wend
        If NbreRuiObs = 0 Then
            msg = "error in observed runoff table, no runoff found during simulation period"
            GoTo ErrFA_LisRuiObs
        End If

        '    rstDataRuiObs.Close
        'Set rstDataRuiObs = Nothing



Exit_LisRuiObs:
        Exit Sub
ErrFA_LisRuiObs:
        ErrFa = True
        Console.WriteLine("ERREUR ! " & msg)
        Resume Exit_LisRuiObs
Err_LisRuiObs:
        Console.WriteLine(Err.Description)
        Resume Exit_LisRuiObs

    End Sub
    Sub BiomasseMulch(Joursim As Integer, jourpaillage As Boolean, QpaillisApport As Double)
        Qpaillis(Joursim) = Qpaillis(Joursim - 1) * Exp(-Alpha_pail)
        If jourpaillage Then Qpaillis(Joursim) = Qpaillis(Joursim) + QpaillisApport
        FracSoilCover = 1 - Exp(-Beta_pail * Qpaillis(Joursim))
    End Sub
    Sub BilanMulch(Joursim As Integer, EoSM As Double, precip As Double)
        'calcul du bilan hydrique du mulch équivalent à la routine introduite dans Stics 3.0 par A. Findeling
        ' selon Arreola 1996
        'precip est l'eau de pluie moins le ruissellement

        Dim epail1 As Double
        Dim epail2 As Double
        Dim intercep As Double
        Dim msg As String

        Eomulch = EoSM * (1 - Exp(-gamma_mulch * Qpaillis(Joursim)))
        If Qpaillis(Joursim) > 0 And Qpaillis(Joursim - 1) > 0 Then

            Smulch = Smulch * Qpaillis(Joursim) / Qpaillis(Joursim - 1)
            If Smulch > 0 Then
                '  calcul du premier terme de l'évaporation du paillis, epail1, dû à la
                '  disparition d'une quantité (Qpaillis(joursim-1) - Qpaillis(joursim) et donc de l'eau
                ' qu 'elle contenait
                epail1 = Smulch * ((Qpaillis(Joursim - 1) - Qpaillis(Joursim)) / Qpaillis(Joursim))

                If epail1 < 0 Then epail1 = 0
                If epail1 < Eomulch Then
                    '  calcul du deuxième terme d'évaporation calculé comme le complément de ep1
                    '  à l'évap potentielle eopaillis en respectant la contrainte de stoc : respail>0
                    epail2 = Eomulch - epail1
                    If epail2 > Smulch Then epail2 = Smulch
                End If
            Else
                Smulch = 0
            End If

        End If

        Emulch(Joursim) = epail1 + epail2
        If epail1 + epail2 > Smulch Then
            msg = "yabug"
        End If
        ' on passe par FracSoilCover pour l'interception de l'eau car le parametre capaciteWmulch
        ' peut ainsi être mesuré par gravimétrie sur un échantillon de pailles et qu'on peut empiriquement
        ' déduire aussi la relation entre quantité de mulch et taux de couverture du sol
        ' mais attention, dans stics6 il y a un contresens, FracSoilCover n'est pas
        ' utilisé pour le stockage de l'eau mais il l'est pour l'évaporation (selon le bouquin en tout cas)!!

        intercep = precip * FracSoilCover
        'Smulch = Max(0, Smulch - Emulch(joursim)) + intercep
        Smulch = Smulch - epail2 + intercep
        If Smulch > CapaciteWMulch * Qpaillis(Joursim) Then

            EauVersSol = precip - intercep + Smulch - CapaciteWMulch * Qpaillis(Joursim)
            Smulch = CapaciteWMulch * Qpaillis(Joursim)

        Else
            EauVersSol = precip - intercep
            'à ce stade Smulch contient encore toute l'eau de pluie du jour et on lui enlève l'excès ligne suivante
            'si pas de mulch Smulch est égal à précip, Eauversol aussi et Smulch devient nul
        End If

        SigmaSimEmulch = SigmaSimEmulch + Emulch(Joursim)

    End Sub
    Sub Ruissellement(Joursim As Integer, precip As Double, TypeSurf As TypeSurfClass, LAI As Double)
        'Dim propor_ruis As Double
        Dim seuil As Double

        SigmaSimPluM = SigmaSimPluM + precip
        If Ruis(Joursim) = 0 And (precip > 0) Then
            ' si pas de ruissellement observé

            If TypeSurf.dAp2 = 0 And TypeSurf.dAp3 = 0 And TypeSurf.dAp4 = 0 Then
                seuil = TypeSurf.dseuil_ruis
            Else
                If TypeSurf.dAp1 + TypeSurf.dAp3 * IKJ = 0 Then
                    seuil = 0
                Else
                    seuil = (TypeSurf.dAp4 - TypeSurf.dAp2 * IKJ) / (TypeSurf.dAp1 + TypeSurf.dAp3 * IKJ)
                End If
            End If

            Ruis(Joursim) = (TypeSurf.dAp1 + TypeSurf.dAp3 * IKJ + b_ruis * Qpaillis(Joursim)) * (precip - seuil)
            Ruis(Joursim) = Max(0, Ruis(Joursim))
            If TypeSurf.bEffetLAI Then Ruis(Joursim) = Ruis(Joursim) * Exp(-0.5 * LAI)
        Else
            'cas d'un ruissellement observé non nul alors que pluie+irrig=0, on force à 0 le ruissellement mais on le signale
            If Ruis(Joursim) <> 0 And (precip = 0) Then
                Ruis(Joursim) = 0
                RuisEtrange = True
            End If
            ' dans le cas d'un ruissellement observé, c'est sa valeur qui est retenue et utilisée plus loin
        End If
        ' mise à jour indice antériorité des pluies IKJ pour la prochaine itération: (IKJ(n+1)=(IKJ(n)+P(n))*exp(-0.5)
        IKJ = (IKJ + precip) * Exp(-0.5)



        '
        'propor_ruis = (a_ruis + b_ruis * Qpaillis(Joursim))

        'Ruis(Joursim) = Max(0, propor_ruis * (precip - seuil_ruis))
        'début bloc fction ruis albergel et al.
        '        If blocalbergel Then
        '            If (coderui = 4) Then
        '                Ap1 = 0.2
        '               Ap2 = 0.03
        '                Ap3 = 0.004
        '                Ap4 = 3
        ''             End If
        '                Ap1 = 0.35
        '               Ap2 = 0.04
        '                Ap3 = 0.004
        '              Ap4 = 3
        '            End If
        '             If (coderui = 6) Then
        '                Ap1 = 0.77
        ''                Ap2 = -0.62
        '               Ap3 = 0.01
        '               Ap4 = 0.56
        '            End If
        '          Ruis(Joursim) = (Ap1 * precip) + (Ap2 * IKJ) + (Ap3 * precip * IKJ) - Ap4


        ' modele mixte:
        ' Ruis(Joursim) = (Ap1 + Ap3 * IKJ + b_ruis * Qpaillis(Joursim)) * (precip - (Ap4 - Ap2 * IKJ) / (Ap1 + Ap3 * IKJ))
        ' fin bloc fonction ruis Albergel et al..


        SigmaSimRuis = SigmaSimRuis + Ruis(Joursim)
        Eau_vers_Mulch = precip - Ruis(Joursim)
    End Sub
    Property dEomulch() As Double
        Get
            Return Eomulch
        End Get
        Set()
        End Set
    End Property
    Property dEmulch(Joursim As Integer) As Double
        Get
            Return Emulch(Joursim)
        End Get
        Set()
        End Set
    End Property
    Property dEauVersSol() As Double
        Get
            Return EauVersSol
        End Get
        Set()
        End Set
    End Property
    Property dEau_vers_Mulch() As Double
        Get
            Return Eau_vers_Mulch
        End Get
        Set()
        End Set
    End Property
    Property dSigmaSimEmulch() As Double
        Get
            Return SigmaSimEmulch
        End Get
        Set()
        End Set
    End Property
    Property dSigmaSimRuis() As Double
        Get
            Return SigmaSimRuis
        End Get
        Set()
        End Set
    End Property
    Property dRuis(Joursim As Integer) As Double
        Get
            Return Ruis(Joursim)
        End Get
        Set()
        End Set
    End Property
    Property dQpaillis(Joursim As Integer) As Double
        Get
            Return Qpaillis(Joursim)
        End Get
        Set()
        End Set
    End Property
    Property dSigmaSimPluM() As Double
        Get
            Return SigmaSimPluM
        End Get
        Set()
        End Set
    End Property
    Property bRuisEtrange() As Boolean
        Get
            Return RuisEtrange
        End Get
        Set()

        End Set
    End Property

End Class
