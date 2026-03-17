Option Compare Text
Option Explicit On
Imports System.Data
Imports System.Data.Sqlite
Public Class GenParamClass

    Dim Codgenclim As Boolean 'si utilisation d'un générateur de climat --a déplacer vers optionsmodel
    Dim Vlaimax As Double 'PX constante d'influence du taux de développement sur le LAI
    Dim ParSurRg As Double 'PX

    Dim dataGenParamAdp As SqliteDataAdapter
    Dim rstGenParam As New DataSet
    'lecture des paramètres généraux de simulation
    'table "general_paramaters"
    'dernière modif 08/04/2011
    'VERSION 3 du 8-10 nov 2017'
    Sub LisGenParam(DataBase_Cnn As SqliteConnection, SimUnit As SimulationUnitClass)


        Dim Trouve As Boolean
        rstGenParam = New DataSet
        dataGenParamAdp = New SqliteDataAdapter("SELECT * FROM General_Parameters where idGenParam='" & SimUnit.sidGenParam & "'", DataBase_Cnn)
        dataGenParamAdp.Fill(rstGenParam)
        Trouve = False

        ' rstGenParam.Open("SELECT * FROM General_Parameters where idGenParam='" & SimUnit.sidGenParam & "'", DataBase_Cnn)
        'rstGenParam.MoveFirst()
        'While Not rstGenParam.EOF And Not Trouve
        'If rstGenParam("idGenParam").Value = SimUnit.sidGenParam Then
        If rstGenParam.Tables(0).Rows.Count > 0 Then
            Trouve = True
            Vlaimax = rstGenParam.Tables(0).Rows(0)("Vlaimax")
            Codgenclim = rstGenParam.Tables(0).Rows(0)("Codgenclim")
            ParSurRg = rstGenParam.Tables(0).Rows(0)("ParSurRg")
            ' Steplait, complete ici lecture de tous les champs de general parameters

            ' General_Parameters.iniprofil, General_Parameters.aclim, General_Parameters.beta, General_Parameters.lvopt, General_Parameters.rayon, General_Parameters.Ra, General_Parameters.bdilC3, General_Parameters.adilC3, General_Parameters.adilC4, General_Parameters.bdilC4, General_Parameters.adilC3max, General_Parameters.bdilC3max, General_Parameters.adilC4max, General_Parameters.bdilC4max, General_Parameters.Vmax1, General_Parameters.Kmabs1, General_Parameters.Vmax2, General_Parameters.Kmabs2, General_Parameters.difN, General_Parameters.concrr, General_Parameters.plNmin, General_Parameters.FTEM, General_Parameters.TREF, General_Parameters.FHUM, General_Parameters.FMIN1, General_Parameters.FMIN2, General_Parameters.FMIN3, General_Parameters.Wh, General_Parameters.kbio, General_Parameters.yres,  General_Parameters.coefb, General_Parameters.dessecplt, General_Parameters.h2ogrmat, General_Parameters.prop
            'ac , General_Parameters.jvcmini, General_Parameters.codanox, General_Parameters.codlaist, General_Parameters.codengraut, General_Parameters.deltaadvmax, General_Parameters.codevsol, General_Parameters.codgenclim

        End If
        'rstGenParam.MoveNext()
        'End While
        'fermeture table et libération mémoire de l'objet

        'rstGenParam.Close()
        'rstGenParam = Nothing

    End Sub
    Property nCodgenclim() As Integer
        Get
            Return Codgenclim
        End Get
        Set()
        End Set
    End Property
    Property dVlaimax() As Double
        Get
            Return Vlaimax
        End Get
        Set()
        End Set
    End Property
    Property dParSurRg() As Double
        Get
            Return ParSurRg
        End Get
        Set()
        End Set
    End Property

End Class
