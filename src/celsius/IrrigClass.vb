Option Compare Text
Imports System.Data
Imports System.Data.Sqlite
Public Class IrrigClass


    Dim Irr(0 To 731) As Double 'VX irrigations journalières en mm chaque jour depuis le 1er jour de simulation (indice 1)
    'Dim dateIrr(0 To 731)  'VX date des irrigations en jour depuis le 1er jour de simul
    Dim NbreIrrig As Integer
    Dim dataIrrigAdp As SqliteDataAdapter
    Dim rstDataIrr As DataSet
    ' classe créée le 08/03/2012
    ' des modifs ont été introduites par CP,
    ' relu FA le 15/11, reste des choses à vérifier, cf commentaires

    Sub LisIrrD(DataBase_Cnn As SqliteConnection, SimUnit As SimulationUnitClass, GestionTechnique As GestionTechniqueClass)

        Dim AnIrrig As Integer
        Dim jourIrrig As Integer
        Dim irrig As Double
        Dim Joursim, I As Integer
        Dim ErrFa As Boolean
        Dim jourEnPlus As Integer
        Dim Ndyear1 As Integer
        Dim msg As String
        On Error GoTo Err_LisIrrD

        If SimUnit.bSY_Bissextile Then
            Ndyear1 = 366
        Else : Ndyear1 = 365
        End If

        dataIrrigAdp = New SqliteDataAdapter("SELECT * FROM Irrigation_List where IdTech_Com='" & SimUnit.sIdTec & "' Order by DateIrrig", DataBase_Cnn)
        dataIrrigAdp.Fill(rstDataIrr)
        'Lecture table "Irrigation" à modifier si nécessaire
        'rstDataIrr.Open "SELECT * FROM Irrigation where id_parcelle='" & Gestiontechnique.sIdparcelle & "'AND year(date)>=" & SimUnit.nStartYear & " Order by annee, Jda", DataBase_Cnn, adOpenDynamic

        'rstDataIrr.Open("SELECT * FROM Irrigation_List where IdTech_Com='" & SimUnit.sIdTec & "' Order by DateIrrig", DataBase_Cnn, ADODB.CursorTypeEnum.adOpenDynamic)
        'rstDataIrr.Open "Irrigation", DataBase_Cnn, adOpenDynamic
        'Trouve = False
        ' *** ATTENTION ajouts CP bizarre car redondant avec la requête ouverte ci-dessus
        'While Not rstDataIrr.EOF And Not Trouve
        'attention idDclim si Escape, codeStat si mada
        '    If rstDataIrr!IdTech_Com = SimUnit.sIdSim Then Trouve = True
        '    rstDataIrr.MoveNext
        'Wend
        'If Not Trouve Then
        '    msg = "error in irrigation data table"
        '   GoTo ErrFA_LisIrrD
        'End If
        ' *** fin ajout bizarre
        I = 0 'rstDataIrr.MoveFirst
        For I = 0 To rstDataIrr.Tables(0).Rows.Count - 1
            'Do While I < rstDataIrr.Tables(0).Rows.Count 'Not rstDataIrr.EOF
            '    AnIrrig = Year(rstDataIrr!Dateirrig)
            ' activer ligne précédente si irrig= tableau de dates calendaires
            AnIrrig = rstDataIrr.Tables(0).Rows(I)("YearIrrig")
            If AnIrrig < 1900 Then AnIrrig = SimUnit.nStartYear + AnIrrig
            '    jourIrrig = Julian(rstDataIrr!Dateirrig)
            ' activer ligne précédente si irrig= tableau de dates calendaires
            jourIrrig = rstDataIrr.Tables(0).Rows(I)("JourYrIrrig")
            irrig = rstDataIrr.Tables(0).Rows(I)("Irrigation")
            If AnIrrig = SimUnit.nStartYear Then
                Joursim = jourIrrig - SimUnit.nStartDay + 1
            Else
                Joursim = jourIrrig + Ndyear1 - SimUnit.nStartDay + 1
            End If
            If Joursim < 0 Then
                msg = "Attention, calendrier des irrigations incohérent avec calendrier de simulation"
                GoTo ErrFA_LisIrrD
            Else
                Irr(Joursim) = irrig
                NbreIrrig = NbreIrrig + 1
            End If
                ' I += 1 'rstDataIrr.MoveNext
            'Loop
        Next


        If NbreIrrig = 0 Then
            msg = "error in irrigation table, no irrigation found during simulation period"
            GoTo ErrFA_LisIrrD
        End If


Exit_LisIrrD:
        Exit Sub
ErrFA_LisIrrD:
        ErrFa = True
        Console.WriteLine("ERREUR ! " & msg)
        Resume Exit_LisIrrD
Err_LisIrrD:
        Console.WriteLine(Err.Description)
        Resume Exit_LisIrrD
    End Sub
    Property dIrr(J As Integer) As Double
        Get
            Return Irr(J)
        End Get
        Set()
        End Set
    End Property

End Class
