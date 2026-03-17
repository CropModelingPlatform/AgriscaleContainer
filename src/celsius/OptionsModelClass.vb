Option Compare Text
Option Explicit On
Imports System.Data
Imports System.Data.Sqlite
Public Class OptionsModelClass
    Dim ActivestressH As Boolean 'vrai: stress hydrique actif sur variables  plante (après levée)
    Dim ActivestressN As Boolean 'vrai: stress hydrique actif sur variables  plante (après levée)
    Dim simlevee As Boolean 'vrai: simulation germination et levee ; faux levee forcée à date indiquée dans tecperCrop
    Dim EcritDResus As Boolean 'vrai écriture des résultats journaliers dans "OutputD"
    Dim Trouve As Boolean
    Dim CorAlti As Boolean 'si vrai correction des températures en fonction de la différence d'altitue dentre station clim et unité simulée
    Dim CyberST As Boolean 'si vrai activation de la gestion technique automatique (Cyber Système Technique...)
    Dim TypeNPKstress As Integer 'choix de la méthode de calcul des stress nutritionnels 1: bilan saisonnier; 2: bilan journalier
    Dim CodeDevelop As String 'option de calcul des stades phenos
    Dim CCYNo As Boolean 'si vrai utilisation de scénarios de changement clim par méthode des deltas et lecture du scénario lors de lecture dataclim selon
    ' code lu dans simUnitList (si faux le code n'est pas lu)


    Dim OptionsAdp As SqliteDataAdapter
    Dim rstOptionsModel As DataSet
    'module lisant les paramètres de choix des options de simulation
    'modif 08/04/2011
    'modif 13/11/2011 CorAlti
    ' 7/12/12 introduction stress N et donc ici ActiveStressN
    ' dernière modif 18/04/13 inytroduction gestion technique auto
    ' modif le 24/05/16 pour option de routine de calcul du developpement
    'VERSION 3 du 8-10 nov 2017'
    ' modif FA du 30/08/18 pour scenarios CC: lecture nouveau champ CCYNo

    Sub LisOptionsModel(DataBase_Cnn As SqliteConnection, idOptionsModel As Integer)
        rstOptionsModel = New DataSet
        OptionsAdp = New SqliteDataAdapter("SELECT * FROM OptionsModel where idCodModel=" & idOptionsModel, DataBase_Cnn)
        OptionsAdp.Fill(rstOptionsModel)

        'rstOptionsModel.Open "OptionsModel", DataBase_Cnn
        'rstOptionsModel.MoveFirst
        'While Not rstOptionsModel.EOF And Not Trouve
        If rstOptionsModel.Tables(0).Rows.Count > 0 Then

            Trouve = True
            ActivestressH = rstOptionsModel.Tables(0).Rows(0)("ActiveWstress")
            ActivestressN = rstOptionsModel.Tables(0).Rows(0)("ActiveNstress")
            simlevee = rstOptionsModel.Tables(0).Rows(0)("simlevee")
            EcritDResus = rstOptionsModel.Tables(0).Rows(0)("EcritDResus")
            CorAlti = rstOptionsModel.Tables(0).Rows(0)("CorrigAlti")
            CyberST = rstOptionsModel.Tables(0).Rows(0)("CyberST")
            'introduire ici lectures options P, K
            TypeNPKstress = rstOptionsModel.Tables(0).Rows(0)("TypeNPKstress")
            CodeDevelop = rstOptionsModel.Tables(0).Rows(0)("CodeDevelop")
            CCYNo = rstOptionsModel.Tables(0).Rows(0)("CCYNo")
            'introduire ici option mauvaises herbes
        End If
        'rstOptionsModel.MoveNext()
        'End While
        'fermeture table et libération mémoire de l'objet
        If CyberST And simlevee = False Then
            Console.WriteLine("CyberST vrai et Simlevee faux ! Simlevee sera reglé sur True")
            simlevee = True
        End If

        'rstOptionsModel.Close()
        'rstOptionsModel = Nothing
    End Sub
    Property bActiveStressH() As Boolean
        Get
            Return ActivestressH
        End Get
        Set()
        End Set
    End Property
    Property bActiveStressN() As Boolean
        Get
            Return ActivestressN
        End Get
        Set()
        End Set
    End Property
    Property bSimLevee() As Boolean
        Get
            Return simlevee
        End Get
        Set()
        End Set
    End Property
    Property bEcritDresus() As Boolean
        Get
            Return EcritDResus
        End Get
        Set()
        End Set
    End Property
    Property bCorAlti() As Boolean
        Get
            Return CorAlti
        End Get
        Set()
        End Set
    End Property
    Property bCyberST() As Boolean
        Get
            Return CyberST
        End Get
        Set()
        End Set
    End Property
    Property nTypeNPKstress() As Integer
        Get
            Return TypeNPKstress
        End Get
        Set()
        End Set
    End Property
    Property sCodeDevelop() As String
        Get
            Return CodeDevelop
        End Get
        Set()

        End Set
    End Property
    Property bCCYNo() As String
        Get
            Return CCYNo
        End Get
        Set()

        End Set
    End Property
    'Function bActiveStressH() As Boolean
    '    Return ActivestressH
    'End Function
    ''Property Get bActiveStressH() As Boolean
    ''bActiveStressH = ActivestressH
    ''End Property

    'Function bActiveStressN() As Boolean
    '    Return ActivestressN
    'End Function
    ''Property Get bActiveStressN() As Boolean
    ''bActiveStressN = ActivestressN
    ''End Property

    'Function bSimLevee() As Boolean
    '    Return simlevee
    'End Function
    ''Property Get bSimLevee() As Boolean
    ''bSimLevee = simlevee
    ''End Property

    'Function bEcritDresus() As Boolean
    '    Return simlevee
    'End Function
    ''Property Get bEcritDresus() As Boolean
    ''bEcritDresus = EcritDResus
    ''End Property

    'Function bCorAlti() As Boolean
    '    Return CorAlti
    'End Function
    ''Property Get bCorAlti() As Boolean
    ''bCorAlti = CorAlti
    ''End Property

    'Function bCyberST() As Boolean
    '    Return CyberST
    'End Function
    ''Property Get bCyberST() As Boolean
    ''bCyberST = CyberST
    ''End Property

    'Function nTypeNPKstress() As Integer
    '    Return TypeNPKstress
    'End Function
    ''Property Get nTypeNPKstress() As Integer
    ''nTypeNPKstress = TypeNPKstress
    ''End Property

End Class
