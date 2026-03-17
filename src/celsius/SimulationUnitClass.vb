Option Explicit On
Option Compare Text
Imports System.Data

Public Class SimulationUnitClass

    ' Classe originale FA
    Dim SY_Bissextile As Boolean
    Dim IdSim As String
    Dim IdTec As String
    Dim IdWeather As String
    Dim idSoil As String
    Dim idWeedCom As String
    Dim idbiotic_Alea As String
    Dim idIni As String
    Dim idGenParam As String
    Dim idCodModel As Integer
    Dim StartYear As Integer
    Dim StartDay As Integer
    Dim EndYear As Integer
    Dim EndDay As Integer
    Dim EndDOY As Integer 'Jour de fin de simulation comptés à partir du 1er janvier de l'année de début
    Dim NbJourSimul As Integer
    Dim CodCC As String
    'VERSION 3 du 8-10 nov 2017
    'Modif FA du 30/08/18 pour scenarios CC: lecture nouveau champ: codCC


    Sub ReadSimUnitParameters(ByRef rstSimulation As DataSet, comptesim As Integer)

        IdSim = rstSimulation.Tables(0).Rows(comptesim)("IdSim")
        IdTec = rstSimulation.Tables(0).Rows(comptesim)("IdTech_Com")
        IdWeather = rstSimulation.Tables(0).Rows(comptesim)("IdWeather")
        idSoil = rstSimulation.Tables(0).Rows(comptesim)("idSoil")
        'idWeedCom = rstSimulation.Tables(0).Rows(0)("idWeedCom")
        'idbiotic_Alea = rstSimulation.Tables(0).Rows(0)("idbiotic_Alea")
        idIni = rstSimulation.Tables(0).Rows(comptesim)("idIni")
        idGenParam = rstSimulation.Tables(0).Rows(comptesim)("idGenParam")
        idCodModel = rstSimulation.Tables(0).Rows(comptesim)("idCodModel")
        StartYear = rstSimulation.Tables(0).Rows(comptesim)("StartYear")
        StartDay = rstSimulation.Tables(0).Rows(comptesim)("StartDay")
        EndYear = rstSimulation.Tables(0).Rows(comptesim)("EndYear")
        EndDay = rstSimulation.Tables(0).Rows(comptesim)("EndDay")
        SY_Bissextile = ((StartYear Mod 4) = 0)
        CodCC = rstSimulation.Tables(0).Rows(comptesim)("CodCC")
        'calcul de EndDoY prévu pour un cas général où on simule n années sans réinitialiser l'état
        'du systeme (cas pas opérationnel dans le reste du projet, seuls cas valables: EndYear-Start Year=0 ou 1
        ' et EndDOY < 366+365 soit  731)
        EndDOY = EndDay + ((EndYear - StartYear) Mod 2) * (365 - CInt(SY_Bissextile))
        NbJourSimul = EndDOY - StartDay + 1
    End Sub
    Property sIdSim() As String
        Get
            Return IdSim
        End Get
        Set()
        End Set
    End Property
    Property sIdTec() As String
        Get
            Return IdTec
        End Get
        Set()
        End Set
    End Property
    Property sIdWeather() As String
        Get
            Return IdWeather
        End Get
        Set()
        End Set
    End Property
    Property sIdSoil() As String
        Get
            Return idSoil
        End Get
        Set()
        End Set
    End Property

    Property sidGenParam() As String
        Get
            Return idGenParam
        End Get
        Set()
        End Set
    End Property
    Property nStartYear() As Integer
        Get
            Return StartYear
        End Get
        Set()
        End Set
    End Property
    Property nStartDay() As Integer
        Get
            Return StartDay
        End Get
        Set()
        End Set
    End Property
    Property nEndYear() As Integer
        Get
            Return EndYear
        End Get
        Set()
        End Set
    End Property
    Property nEndDay() As Integer
        Get
            Return EndDay
        End Get
        Set()
        End Set
    End Property
    Property bSY_Bissextile() As Boolean
        Get
            Return SY_Bissextile
        End Get
        Set()
        End Set
    End Property
    Property nEndDOY() As Integer
        Get
            Return EndDOY
        End Get
        Set()
        End Set
    End Property
    Property nidCodModel() As Integer
        Get
            Return idCodModel
        End Get
        Set()
        End Set
    End Property
    Property nNbJourSimul() As Integer
        Get
            Return NbJourSimul
        End Get
        Set(NJsimul As Integer)
            NbJourSimul = NJsimul
        End Set
    End Property
    Property nidIni() As String
        Get
            Return idIni
        End Get
        Set()
        End Set
    End Property
    Property sCodcc() As Integer
        Get
            Return CodCC
        End Get
        Set()
        End Set
    End Property


End Class
