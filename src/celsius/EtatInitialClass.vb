Option Compare Text
Option Explicit On
Imports System.Data
Imports System.Data.Sqlite
Public Class EtatInitialClass
    Dim Stockinit As Double 'VX, Valeur du stock hydrique utile au premier jour de la simulation (mm)
    Dim Qpaillisinit As Double
    Dim IniSolhautON As Boolean 'VX, Choix d'initialiser le stock par le haut (Oui) ou par le bas du profil (Non)
    Dim rstDataIni As DataSet
    Dim dataIniAdp As SqliteDataAdapter   '  modif 10/05/2011
    ' dernière modif le 30/11/11 introduction option remplissage par le bas du sol
    'VERSION 3 du 8-10 nov 2017'
    Sub LisInitialData(DataBase_Cnn As SqliteConnection, idIni As String)


        Dim Trouve As Boolean
        rstDataIni = New DataSet
        Trouve = False
        dataIniAdp = New SqliteDataAdapter("Select * From ParamIni where idIni='" & idIni & "'", DataBase_Cnn)
        dataIniAdp.Fill(rstDataIni)
        'rstDataIni.Open "Select * From ParamIni where idIni=" & idIni, DataBase_Cnn
        'rstDataIni.MoveFirst
        If rstDataIni.Tables(0).Rows.Count > 0 Then
            'While Not rstDataIni.EOF And Not Trouve
            'If rstDataIni.Tables(0).Rows(0)("idIni") = idIni Then
            'Trouve = True
            Stockinit = rstDataIni.Tables(0).Rows(0)("Stockinit")
            Qpaillisinit = rstDataIni.Tables(0).Rows(0)("Qpaillisinit")
            IniSolhautON = rstDataIni.Tables(0).Rows(0)("IniSolhautON")
        End If
        'fermeture table et libération mémoire de l'objet


    End Sub
    Sub recursive(Etatfinal As EtatFinalClass)
        Stockinit = Etatfinal.Stockfinal
        Qpaillisinit = Etatfinal.QpaillisFinal
        IniSolhautON = Etatfinal.FinalSolhautON
    End Sub
    Property dStockinit() As Double
        Get
            Return Stockinit
        End Get
        Set()
        End Set
    End Property
    Property dQpaillisinit() As Double
        Get
            Return Qpaillisinit
        End Get
        Set()
        End Set
    End Property
    Property bIniSolhautON() As Boolean
        Get
            Return IniSolhautON
        End Get
        Set()
        End Set
    End Property
    'Function dStockinit() As Double
    '    Return Stockinit
    'End Function
    ''Property Get dStockinit() As Double
    ''dStockinit = Stockinit
    ''End Property

    'Function dQpaillisinit() As Double
    '    Return Qpaillisinit
    'End Function
    ''Property Get dQpaillisinit() As Double
    ''dQpaillisinit = Qpaillisinit
    ''End Property

    'Function bIniSolhautON() As Boolean
    '    Return IniSolhautON
    'End Function
    ''Property Get bIniSolhautON() As Boolean
    ''bIniSolhautON = IniSolhautON
    ''End Property

End Class
