Option Compare Text
Option Explicit On
Imports System.Data
Imports System.Data.Sqlite
Public Class TypeSurfClass
    Dim dataSolAdp As SqliteDataAdapter
    Dim rstSolData As DataSet 'les tables de variables sol lues
    Dim CdeRui As Integer 'copie locale de Typerui, champ de laison avec la table sol
    'Dim TypeRui As Integer 'champ de laison avec la table sol
    Dim Ap1 As Double 'Vx parametre Ap1 des fonctions de ruissellement Albergel et al, et coef de ruissellement (si Ap2..Ap4=0, Rui=Ap1*(P-Seuil))
    Dim Ap2 As Double, Ap3 As Double, Ap4 As Double 'Vx parametre Ap2 des fonctions de ruissellement Albergel et al combinée avec effet paillis (Ruis(Joursim) = (Ap1 + Ap3 * IKJ + b_ruis * Qpaillis(Joursim)) * (precip - (Ap4 - Ap2 * IKJ) / (Ap1 + Ap3 * IKJ))
    Dim seuil_ruis As Double 'VX seuil de précipitations en-dessous duquel il n'y a pas de ruissellement (mm). Non utilisé si Ap2 ou Ap3 ou Ap4 <> 0
    Dim EffetLAI As Boolean 'VX: vrai: effet du LAI sur le ruissellement
    '  nouvelle classe introduite le 28/04/13 pour simulation ruissellement selon  albergel et al.

    Sub readSurf(DataBase_Cnn As SqliteConnection, TypeRui As Integer)
        rstSolData = New DataSet
        dataSolAdp = New SqliteDataAdapter("SELECT * FROM TypeSurfSol WHERE TypeRui = " & TypeRui, DataBase_Cnn)
        dataSolAdp.Fill(rstSolData)

        'rstSolData.Open "SELECT * FROM TypeSurfSol WHERE TypeRui = " & TypeRui, DataBase_Cnn
        Ap1 = rstSolData.Tables(0).Rows(0)("Ap1")
        Ap2 = rstSolData.Tables(0).Rows(0)("Ap2")
        Ap3 = rstSolData.Tables(0).Rows(0)("Ap3")
        Ap4 = rstSolData.Tables(0).Rows(0)("Ap4")
        seuil_ruis = rstSolData.Tables(0).Rows(0)("seuil_ruis")
        EffetLAI = rstSolData.Tables(0).Rows(0)("EffetLAI")
    End Sub
    Property dAp1() As Double
        Get
            Return Ap1
        End Get
        Set()
        End Set
    End Property
    Property dAp2() As Double
        Get
            Return Ap2
        End Get
        Set()
        End Set
    End Property
    Property dAp3() As Double
        Get
            Return Ap3
        End Get
        Set()
        End Set
    End Property
    Property dAp4() As Double
        Get
            Return Ap4
        End Get
        Set()
        End Set
    End Property
    Property dseuil_ruis() As Double
        Get
            Return seuil_ruis
        End Get
        Set()
        End Set
    End Property
    Property bEffetLAI() As Boolean
        Get
            Return EffetLAI
        End Get
        Set()
        End Set
    End Property
    Property nCdeRui() As Boolean
        Get
            Return CdeRui
        End Get
        Set()
        End Set
    End Property


End Class
