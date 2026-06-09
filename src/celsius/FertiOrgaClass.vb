Option Compare Text
Imports System.Data
Imports System.Data.Sqlite
Public Class FertiOrgaClass

    Dim Norg(0 To 731) As Double
    Dim Porg(0 To 731) As Double
    Dim Korg(0 To 731) As Double
    Dim Qorga(0 To 731) As Double

    Dim Norganique As Double
    Dim Porganique As Double
    Dim Korganique As Double
    Dim QorgaTot As Double
    Dim TypeMorga As String
    Dim CsurNRes As Double

    Dim dataFertiOrgAdp As SqliteDataAdapter
    Dim rstDataFertiOrg As DataSet
    'classe créée CP 14/09/13 ajout de la classe FertOrga pour lecture des données
    'd'apports de fertilisants organiques (sur le même principe que l'irrigation)
    ' relu FA 15/11/13: reste ds choses à vérifier voir commentaires insérés
    'VERSION 3 du 8-10 nov 2017'

    Sub LisFertiOrgD(DataBase_Cnn As SqliteConnection, SimUnit As SimulationUnitClass, GestionTechnique As GestionTechniqueClass)
        'commentaires décrivant les variables à insérer
        Dim AnFertiOrg As Integer
        Dim jourFertiOrg As Integer
        'Dim Norganique As Double
        'Dim Porganique As Double
        'Dim Korganique As Double
        Dim Joursim, I As Integer
        Dim ErrFa As Boolean
        Dim Ndyear1 As Integer
        Dim msg As String
        On Error GoTo Err_LisFertiOrgD

        If SimUnit.bSY_Bissextile Then Ndyear1 = 366 Else Ndyear1 = 365
        dataFertiOrgAdp = New SqliteDataAdapter("SELECT * FROM FertiOrga_List where IdTech_Com='" & SimUnit.sIdTec & "' Order by DateFertiOrg", DataBase_Cnn)
        dataFertiOrgAdp.Fill(rstDataFertiOrg)        ' *** à vérifier : redondant avec ouverture requête, voir si critère = idsim ou idtec
        Dim Trouve As Boolean
        Trouve = False
        I = 0
        Do While I < rstDataFertiOrg.Tables(0).Rows.Count And Not Trouve 'Not rstDataFertiOrg.EOF And Not Trouve
            'attention idDclim si Escape, codeStat si mada
            If rstDataFertiOrg.Tables(0).Rows(I)("IdTech_Com") = SimUnit.sIdSim Then Trouve = True
            I += 1 'rstDataFertiOrg.MoveNext
        Loop
        If Not Trouve Then
            msg = "error in organic fertilisation data table"
            GoTo ErrFA_LisFertiOrgD
        End If
        ' fin bloc redondant avec ouverture requête, voir si critère = idsim ou idtec
        'rstDataFertiOrg.MoveFirst
        I = 0
        Do While I < rstDataFertiOrg.Tables(0).Rows.Count 'Not rstDataFertiOrg.EOF

            AnFertiOrg = rstDataFertiOrg.Tables(0).Rows(I)("YearFertiOrg")
            jourFertiOrg = rstDataFertiOrg.Tables(0).Rows(I)("JourYrFertiOrg")
            Norganique = rstDataFertiOrg.Tables(0).Rows(I)("Norganique") 'vérifier unités
            Porganique = rstDataFertiOrg.Tables(0).Rows(I)("Porganique") 'vérifier unités
            Korganique = rstDataFertiOrg.Tables(0).Rows(I)("Korganique") 'vérifier unités
            QorgaTot = rstDataFertiOrg.Tables(0).Rows(I)("QorgaTot")
            TypeMorga = rstDataFertiOrg.Tables(0).Rows(I)("TypeMorga")
            CsurNRes = rstDataFertiOrg.Tables(0).Rows(I)("CsurNRes")

            If AnFertiOrg = SimUnit.nStartYear Then
                Joursim = jourFertiOrg - SimUnit.nStartDay + 1
            Else
                Joursim = jourFertiOrg + Ndyear1 - SimUnit.nStartDay + 1
            End If
            If Joursim < 0 Then
                msg = "Attention, calendrier des apports organiques incohérent avec calendrier de simulation"
                GoTo ErrFA_LisFertiOrgD
            Else
                Norg(Joursim) = Norganique
                Porg(Joursim) = Porganique
                Korg(Joursim) = Korganique
                Qorga(Joursim) = QorgaTot

            End If
            I += 1
        Loop



Exit_LisFertiOrgD:
        Exit Sub
ErrFA_LisFertiOrgD:
        ErrFa = True
        Console.WriteLine("ERREUR ! " & msg)
        Resume Exit_LisFertiOrgD
Err_LisFertiOrgD:
        Console.WriteLine(Err.Description)
        Resume Exit_LisFertiOrgD
    End Sub



    Function dNorg(J As Integer) As Double
        Return Norg(J)
    End Function
    'Property Get dNorg(J As Integer) As Double
    'dNorg = Norg(J)
    'End Property

    Function dPorg(J As Integer) As Double
        Return Porg(J)
    End Function
    'Property Get dPorg(J As Integer) As Double
    'dPorg = Porg(J)
    'End Property

    Function dKorg(J As Integer) As Double
        Return Korg(J)
    End Function
    'Property Get dKorg(J As Integer) As Double
    'dKorg = Korg(J)
    'End Property
    Function dQorga(J As Integer) As Double
        dQorga = Qorga(J)
    End Function
    Function sTypeMorga() As Double
        sTypeMorga = TypeMorga
    End Function
    Function dCsurNRes() As Double
        dCsurNRes = CsurNRes
    End Function



End Class
