Option Compare Text
Option Explicit On
Imports System.Math
Imports System.Data
Imports System.Data.Sqlite
Public Class DataClimClass
    'VX: Variable explicative, VE: variable d'état, VS: variable simulée,PX: paramètre
    Dim CO2c As Integer 'Vx Concentration en CO2 de l'atmosphere (ppm)
    Dim ddat As Date
    Dim Tmoy(0 To 731) As Double 'VX
    Dim Tmax(0 To 731) As Double 'VX
    Dim Tmin(0 To 731) As Double 'VX
    Dim Rg(0 To 731) As Double 'VX
    Dim Etp(0 To 731) As Double 'VX
    Dim Plu(0 To 731) As Double 'VX
    Dim DOY(0 To 731) As Integer 'jour de l'année
    Dim DAP(0 To 731) As Integer 'jour après semis / plantation (day after planting)
    Dim CurrentYear(0 To 731) As Integer
    Dim Altitude As Integer 'VX
    Dim Latitude As Double 'VX
    Dim DAYL(0 To 731) As Double 'VE longueur astronomique du jour (h)
    'Dim rstDataClim As ADODB.Recordset
    Dim Ndyear1 As Integer
    Dim ConcNplu As Double 'VX concentration en N des précipitations UNITE a compléter
    Dim dataclimAdp As SqliteDataAdapter
    Dim rstDataClim As DataSet
    'VERSION 3 du 8-10 nov 2017'
    ' introduction teneur en N des pluies

    ' attention, lecture des données climatiques actuellement configurée pour ouvrir
    'la table "Wstations" et y lire le code de station, l'altitude et la latitude
    ' puis la table "Meteo" pour y lire les données climatiques correspondant au code de station
    ' et aux années postérieures à l'année du semis et les jours compris entre jour de début et de fin de
    'simulation.
    ' Modifier la sub LisClimD ci-dessous en cas de changement de noms de tables
    ' et de champs dans lesquels lire
    ' modif 8/04/2011
    ' dernière modif 13/11/2011 introduction correction altitude
    ' modifs 1/08/13 correction calcul durée du jour, Ndyear1 (anciennement variable de LisClimD) passé en variable de la classe
    ' modif FA 24/04/13 introduction lecture CO2c dans ListPAnnexes


    Sub LisClimD(DataBase_Cnn As SqliteConnection, SimUnit As SimulationUnitClass)
        Dim Complete As Boolean
        Dim LackClim As Boolean
        Dim Trouve As Boolean
        Dim ErrFa As Boolean
        Dim n As Integer
        Dim DR() As DataRow
        Dim NJsimul, I As Integer
        Dim msg As String
        On Error GoTo Err_LisClimD
        Dim dataclimAdp As SqliteDataAdapter
        Dim rstDataClim As New DataSet
        'Lecture table "Wstations" à modifier si nécessaire
        'ligne suivante version Mada
        'rstDataClim.Open "WStations", DataBase_Cnn, adOpenDynamic
        'ligne suivante version Escape

        dataclimAdp = New SqliteDataAdapter("Select * From ListPAnnexes where idDclim='" & SimUnit.sIdWeather & "'", DataBase_Cnn)
        dataclimAdp.Fill(rstDataClim)
        Trouve = False
        If rstDataClim.Tables(0).Rows.Count = 1 Then
            Latitude = rstDataClim.Tables(0).Rows(0)("latitudeDD")
            Altitude = rstDataClim.Tables(0).Rows(0)("Altitude")
            CO2c = rstDataClim.Tables(0).Rows(0)("CO2c")
            Trouve = True
        Else
            'End If
            'If Not Trouve Then
            msg = "error in weather station list table"
            GoTo ErrFA_LisClimD
        End If
        '        While Not rstDataClim.EOF And Not Trouve
        '            'ligne suivante version Escape
        '            If rstDataClim!idDclim = SimUnit.sIdWeather Then Trouve = True
        '            'ligne suivante version Mada
        '            '    If rstDataClim!CodeWS = SimUnit.sIdWeather Then Trouve = True
        '            rstDataClim.MoveNext()
        'Wend
        If Not Trouve Then
            msg = "error in weather station list table"
            GoTo ErrFA_LisClimD
        End If
        'rstDataClim.MovePrevious()
        '' Attention aux noms des champs dans la table Wstations
        'Latitude = rstDataClim!latitudeDD
        'Altitude = rstDataClim!Altitude
        'CO2c = rstDataClim!CO2c
        'rstDataClim.Close()
        ' faudra  éventuellement
        ' vérifier la cohérence des années de simulation avec les années présentes dans la table
        'Set rstDataClim = New ADODB.Recordset
        ' important de faire le tri croissant sur date dans la lecture de la table!
        ' modifier si nécessaire nom de table et de champs
        'ligne suivante version Escape
        'rstDataClim.Open "SELECT * FROM Dweather where idDclim='" & SimUnit.sIdWeather & "'AND annee>=" & SimUnit.nStartYear & " Order by annee, Jda", DataBase_Cnn, adOpenDynamic
        rstDataClim.Clear()
        dataclimAdp = New SqliteDataAdapter("SELECT * FROM Dweather where idDclim='" & SimUnit.sIdWeather & "' AND annee>=" & SimUnit.nStartYear & " Order by annee, Jda", DataBase_Cnn)
        dataclimAdp.Fill(rstDataClim)
        'ligne suivante version Mada
        'rstDataClim.Open "Select * FROM Meteo where CodeStat='" & SimUnit.sIdWeather & "'AND annee >=" & SimUnit.nStartYear & " Order by annee, Jda", DataBase_Cnn, adOpenDynamic
        ' attention ligne suivante pas valable pour hémisphère sud...
        'rstDataClim.Open "SELECT * FROM Meteo where CodeStat='" & SimUnit.sIdWeather & "'AND annee=" & SimUnit.nStartYear & " Order by Jda", DataBase_Cnn, adOpenDynamic

        Complete = False
        LackClim = False
        Trouve = False
        DR = rstDataClim.Tables(0).Select("idDclim='" & SimUnit.sIdWeather & "' AND annee>=" & SimUnit.nStartYear)
        'While Not rstDataClim.EOF And Not Trouve
        ''attention idDclim si Escape, codeStat si mada
        'If rstDataClim!idDclim = SimUnit.sIdWeather And rstDataClim!annee = SimUnit.nStartYear Then Trouve = True
        'rstDataClim.MoveNext()
        'Wend
        If DR.Count = 0 Then 'If Not Trouve Then
            msg = "error in climatic data table"
            GoTo ErrFA_LisClimD
        End If

        'rstDataClim.MovePrevious
        'ligne suivante si requete sur année et station...
        n = 1
        I = 0
        ' n est dans le calendrier de simulation (n augmente à partir du jour de début de simulation
        Do While (Not Complete) And (I < rstDataClim.Tables(0).Rows.Count) And (Not LackClim) 'Not rstDataClim.EOF
            'ddat = rstDataClim!Ddate
            CurrentYear(n) = rstDataClim.Tables(0).Rows(I)("annee")
            DOY(n) = rstDataClim.Tables(0).Rows(I)("Jda")

            If (DOY(n) >= SimUnit.nStartDay Or CurrentYear(n) = SimUnit.nStartYear + 1) Then

                'introduction ici (avant transformation de DOY ds le calendrier de 0 à 731) du calcul de la durée du jour
                DAYL(n) = LenDay(Latitude, DOY(n))

                If SimUnit.bSY_Bissextile Then Ndyear1 = 366 Else Ndyear1 = 365
                If DOY(n) <= DOY(n - 1) Then DOY(n) = DOY(n) + Ndyear1

                Tmin(n) = rstDataClim.Tables(0).Rows(I)("Tmin")
                Tmax(n) = rstDataClim.Tables(0).Rows(I)("Tmax")
                Tmoy(n) = Tmoy_TnTx(Tmin(n), Tmax(n))
                'lignes suivantes version Mada
                '    Rg(n) = rstDataClim!RAYONGLOB
                '    Etp(n) = rstDataClim!ETO
                '    Plu(n) = rstDataClim!PLUIE
                '3 lignes suivantes version Escape
                Rg(n) = rstDataClim.Tables(0).Rows(I)("Rg")
                Etp(n) = rstDataClim.Tables(0).Rows(I)("Etp")
                Plu(n) = rstDataClim.Tables(0).Rows(I)("Plu")

                'LackClim = IsNothing(Tmin(n) + Tmax(n))
                LackClim = IsDBNull(rstDataClim.Tables(0).Rows(I)("Tmin")) Or IsDBNull(rstDataClim.Tables(0).Rows(I)("Tmax"))

                If DOY(n) = SimUnit.nEndDOY Then Complete = True
                n = n + 1
            End If
            I += 1
            'rstDataClim.MoveNext
        Loop

        'fermeture table et libération mémoire de l'objet
        'rstDataClim.Close()
        'Set rstDataClim = Nothing


        NJsimul = n - 1
        If LackClim Or Not Complete Then
            msg = SimUnit.sIdSim
            If LackClim Then msg = msg & " Caution ! Missing climatic data. Simulation performed until first missing data"
            If Not Complete Then msg = msg & "Fin de simulation apres fin des données climatiques !!! Réduisez la période de simulation"
            Console.WriteLine(msg)
            NJsimul = NJsimul - 1
            SimUnit.nNbJourSimul = NJsimul
        End If

Exit_LisClimD:
        Exit Sub
ErrFA_LisClimD:
        ErrFa = True
        Console.WriteLine("ERREUR ! " & msg)
        Resume Exit_LisClimD
Err_LisClimD:
        Console.WriteLine(Err.Description)
        Resume Exit_LisClimD
    End Sub
    Sub update_DAP(idebut As Integer, iplt As Integer)
        'calcul des jours après semis / plantation et stockage dans tableau
        ' n est dans le calendrier de simulation (n=1 pour la date de début de simulation)
        Dim n As Integer
        'Dim nbjouran As Integer
        For n = 0 To 731
            DAP(n) = idebut - iplt + n - 1
            '    jouran = DOY(n)
            '    If DOY(n) >= Ndyear1 Then jouran = DOY(n) - Ndyear1

            '    DAYL(n) = LenDay(Latitude, jouran)
        Next n
    End Sub
    Sub CorrigTAlt(AltiCult As Integer, NJsimul As Integer)
        Dim i As Integer
        For i = 1 To NJsimul

            Tmin(i) = Tmin(i) - (AltiCult - Altitude) * 0.6 / 100
            Tmax(i) = Tmax(i) - (AltiCult - Altitude) * 0.6 / 100
            Tmoy(i) = Tmoy_TnTx(Tmin(i), Tmax(i))
        Next i
    End Sub
    Sub CorrigTSerres(joursim As Integer)
        ' modèle empirique FA/ Jenny Montagne

        Const SERA As Double = 0.24 'constantes empiriques Jenny Montagne
        Const SERB As Double = 0.095

        Tmin(joursim) = Tmax(joursim) - SERB * (Tmax(joursim) + Tmin(joursim)) + (SERA * (1 - SERB) * Rg(joursim))
        Tmax(joursim) = Tmax(joursim) + SERA * Rg(joursim)
        Tmoy(joursim) = Tmoy_TnTx(Tmin(joursim), Tmax(joursim))

    End Sub
    Function LenDay(lat As Double, J As Integer) As Double
        ' calcul de la durée astronomique du jour
        'attention, VBasic utilise le passage d'arguments Byref par défaut, et la variable DataClim.Latitude est modifiée par cette fonction
        ' si on ne passe pas par la variable intérmédiaire locale "latrad". On aurait pê pu spécifier aussi "Byval" et laisser lat=pi*lat/180, à vérifier
        Dim Declin As Double, SolarAngle As Double, latrad As Double
        Const pi = 3.14159265
        latrad = pi * lat / 180
        Declin = 0.409 * Sin((2 * pi * J / 365) - 1.39)
        SolarAngle = Tan(latrad) * Tan(Declin)

        SolarAngle = -Atan(-SolarAngle / Sqrt(-SolarAngle * SolarAngle + 1)) + 2 * Atan(1)
        ' note: Atn(1)= pi/4
        LenDay = 24 * SolarAngle / pi
    End Function
    Function Tmoy_TnTx(Tmin As Double, Tmax As Double) As Double
        Tmoy_TnTx = (Tmin + Tmax) / 2
    End Function
    Property dTmin(J As Integer) As Double
        Get
            Return Tmin(J)
        End Get
        Set()
        End Set
    End Property
    Property dTmax(J As Integer) As Double
        Get
            Return Tmax(J)
        End Get
        Set()
        End Set
    End Property
    Property dTmoy(J As Integer) As Double
        Get
            Return Tmoy(J)
        End Get
        Set()
        End Set
    End Property
    Property dRg(J As Integer) As Double
        Get
            Return Rg(J)
        End Get
        Set()
        End Set
    End Property
    Property dEtp(J As Integer) As Double
        Get
            Return Etp(J)
        End Get
        Set()
        End Set
    End Property
    Property dPlu(J As Integer) As Double
        Get
            Return Plu(J)
        End Get
        Set()
        End Set
    End Property
    Property nDAP(J As Integer) As Integer
        Get
            Return DAP(J)
        End Get
        Set()
        End Set
    End Property
    Property nDOY(J As Integer) As Integer
        Get
            Return DOY(J)
        End Get
        Set()
        End Set
    End Property
    Property nCurrentYear(J As Integer) As Integer
        Get
            Return CurrentYear(J)
        End Get
        Set()
        End Set
    End Property
    Property dDAYL(J As Integer) As Double
        Get
            Return DAYL(J)
        End Get
        Set()
        End Set
    End Property
    Property nCO2c() As Integer
        Get
            Return CO2c
        End Get
        Set()
        End Set
    End Property
    Property dConcNplu() As Integer
        Get
            Return ConcNplu
        End Get
        Set()
        End Set
    End Property
End Class
