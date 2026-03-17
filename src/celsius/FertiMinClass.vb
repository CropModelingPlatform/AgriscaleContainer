Option Compare Text
Imports System.Data
Imports System.Data.Sqlite
Public Class FertiMinClass

    Dim Nmin(0 To 731) As Double
    Dim Pmin(0 To 731) As Double
    Dim Kmin(0 To 731) As Double

    Dim dataFertiMinAdp As SqliteDataAdapter
    Dim rstDataFertiMin As DataSet
    'classe créée par CP 14/09/13 pour lecture des données
    'd'apports de fertilisants minéraux (sur le même principe que l'irrigation)
    ' relu FA le 15/11, reste des choses à vérifier (commentaires insérés)
    'VERSION 3 du 8-10 nov 2017'
    Sub LisFertiMinD(DataBase_Cnn As SqliteConnection, SimUnit As SimulationUnitClass, GestionTechnique As GestionTechniqueClass)
        ' vérifier: manque les commentaires décrivant les variables
        Dim AnFertiMin As Integer
        Dim jourFertiMin As Integer
        Dim Nmineral As Double
        Dim Pmineral As Double
        Dim Kmineral As Double
        Dim Joursim, I As Integer
        Dim ErrFa As Boolean
        Dim Ndyear1 As Integer
        Dim msg As String
        On Error GoTo Err_LisFertiMinD

        If SimUnit.bSY_Bissextile Then Ndyear1 = 366 Else Ndyear1 = 365
        dataFertiMinAdp = New SqliteDataAdapter("SELECT * FROM FertiMin_List where IdTech_Com='" & SimUnit.sIdTec & "' Order by DateFertiMin", DataBase_Cnn)
        dataFertiMinAdp.Fill(rstDataFertiMin)
        'rstDataFertiMin = New ADODB.Recordset
        'rstDataFertiMin.Open("SELECT * FROM FertiMin_List where IdTech_Com='" & SimUnit.sIdTec & "' Order by DateFertiMin", DataBase_Cnn, ADODB.CursorTypeEnum.adOpenDynamic)
        Dim Trouve As Boolean = False
        Trouve = False
        '**** REDONDANT ! A verifier si critère = idsim ou idtec !!!
        I = 0
        Do While I < rstDataFertiMin.Tables(0).Rows.Count And Not Trouve 'Not rstDataFertiMin.EOF
            'attention idDclim si Escape, codeStat si mada
            If rstDataFertiMin.Tables(0).Rows(I)("IdTech_Com") = SimUnit.sIdSim Then Trouve = True
            I += 1 'rstDataFertiMin.MoveNext
        Loop
        If Not Trouve Then
            msg = "error in mineral fertilisation data table"
            GoTo ErrFA_LisFertiMinD
        End If
        'fin partie redondante a verifier
        'rstDataFertiMin.MoveFirst

        I = 0
        Do While I < rstDataFertiMin.Tables(0).Rows.Count 'While Not rstDataFertiMin.EOF

            AnFertiMin = rstDataFertiMin.Tables(0).Rows(I)("YearFertiMin")
            jourFertiMin = rstDataFertiMin.Tables(0).Rows(I)("JourYrFertiMin")
            Nmineral = rstDataFertiMin.Tables(0).Rows(I)("Nmineral") 'vérifier unités utilisées
            Pmineral = rstDataFertiMin.Tables(0).Rows(I)("Pmineral") 'vérifier unités utilisées
            Kmineral = rstDataFertiMin.Tables(0).Rows(I)("Kmineral") 'vérifier unités utilisées

            If AnFertiMin = SimUnit.nStartYear Then
                Joursim = jourFertiMin - SimUnit.nStartDay + 1
            Else
                Joursim = jourFertiMin + Ndyear1 - SimUnit.nStartDay + 1
            End If
            If Joursim < 0 Then
                msg = "Attention, calendrier des apports minéraux incohérent avec calendrier de simulation"
                GoTo ErrFA_LisFertiMinD
            Else
                Nmin(Joursim) = Nmineral
                Pmin(Joursim) = Pmineral
                Kmin(Joursim) = Kmineral

            End If
            I += 1 'rstDataFertiMin.MoveNext
        Loop



Exit_LisFertiMinD:
        Exit Sub
ErrFA_LisFertiMinD:
        ErrFa = True
        Console.WriteLine("ERREUR ! " & msg)
        Resume Exit_LisFertiMinD
Err_LisFertiMinD:
        Console.WriteLine(Err.Description)
        Resume Exit_LisFertiMinD
    End Sub
    Property dNmin(J As Integer) As Double
        Get
            Return Nmin(J)
        End Get
        Set()
        End Set
    End Property
    Property dPmin(J As Integer) As Double
        Get
            Return Pmin(J)
        End Get
        Set()
        End Set
    End Property
    Property dKmin(J As Integer) As Double
        Get
            Return Kmin(J)
        End Get
        Set()
        End Set
    End Property
    'Function dNmin(J As Integer) As Double
    '    Return Nmin(J)
    'End Function
    ''Property Get dNmin(J As Integer) As Double
    ''dNmin = Nmin(J)
    ''End Property

    'Function dPmin(J As Integer) As Double
    '    Return Pmin(J)
    'End Function
    ''Property Get dPmin(J As Integer) As Double
    ''dPmin = Pmin(J)
    ''End Property

    'Function dKmin(J As Integer) As Double
    '    Return Kmin(J)
    'End Function
    ''Property Get dKmin(J As Integer) As Double
    ''dKmin = Kmin(J)
    ''End Property

End Class
