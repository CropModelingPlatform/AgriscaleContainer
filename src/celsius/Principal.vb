Option Compare Text
Option Explicit On

Imports System
Imports System.Diagnostics
Imports System.Threading
Imports System.Data
Imports System.Data.Sqlite
Module Principal
    ''dernière modif 04/13 numsimul est compté grace à une requête regroupement, n'est plus lu dans Simulinfo
    ''modi 25/04/14 numsimul et comptesim passés en Long

    ''Calcul du temps passé


    'Public Sub Principal()
    '    Dim stopWatch As New Stopwatch()
    '    Dim ts As TimeSpan
    '    Dim elapsedTime As String
    '    Dim w As Integer
    '    Dim x As Double
    '    Dim y As Integer
    '    Dim z As Integer
    '    Dim t As Integer

    '    'dernière modif:08/04/2011
    '    Dim Db_Cnn As New ADODB.Connection

    '    Dim SimInfo As New ADODB.Recordset
    '    Dim ReqSim As New ADODB.Recordset
    '    Dim TabSynt As New ADODB.Recordset
    '    Dim NumSimul As Long
    '    Dim CodeOptim As Boolean
    '    Dim comptesim As Long
    '    Dim MsgFin As String
    '    Dim chronoStart As Date
    '    Dim StartChronoCalc As Date
    '    Dim StartChronoEcrit As Date
    '    Dim Chrono1 As Date
    '    Dim Chrono2 As Date
    '    Dim chronotot As Date
    '    Dim nrecurs As Integer
    '    Dim SimCtrl As New SimulationControlClass
    '    Dim ncpsui As Integer
    '    Dim codesuite As Integer 'gestion de la récursivité
    '    Dim strConn, strConn2 As String

    '    '  strConn = "Provider=Microsoft.Jet.OLEDB.4.0;" & _
    '    '                "Data Source=C:\Users\talavera\Desktop\Test Center\Celsius.mdb;"
    '    strConn = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=C:\Users\giner\Desktop\Celsius_Marcelo\celsius_louise.mdb;"
    '    strConn2 = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=C:\Users\giner\Desktop\Celsius_Marcelo\celsius_louise2.mdb;"
    '    '                "Data Source=C:\Users\giner\Desktop\Celsius_Marcelo\ModeleCELSIUS_projetV2.mdb;"
    '    Db_Cnn.Open(strConn)

    '    'c modif de la gestion de la récursivité repris de sticpail
    '    'c si nrecurs=1, toutes les situations sont initialisées avec recup.tmp
    '    ' si nrecurs est >1 la premiere sim est initialisée avec le contenu
    '    'c de simul.usm, les codesuite-1 suivantes sont initialisées avec recup.tmp
    '    'c et la suivante de nouveau avec simul.usm et ainsi de suite
    '    'c exemples:
    '    'c codesuite dans simul.usm:  dans la séquence des simulations:
    '    'c                  2                 010101.
    '    'c                  3                   011011011....
    '    'c                4                 011101110111....
    '    'c si codesuite=1
    '    'c la simul de l'année 1 prend fin à la récolte et celle de l'année 2
    '    'c reprend automatiquement au jour suivant, avec semis décalé de iplt+1 jours par rapport à récolte
    '    'c avec cette option les stade obs (dont iplt) pour la 2e année, si présents, doivent être aussi
    '    'c donnés en jours depuis la récolte de la culture antérieure.
    '    Db_Cnn.Execute("delete * from outputsynt")
    '    Application.DoEvents()
    '    Db_Cnn.Execute("delete * from outputD_1")
    '    Application.DoEvents()
    '    Db_Cnn.Execute("delete * from outputD_2")
    '    Application.DoEvents()
    '    Db_Cnn.Close()
    '    Application.DoEvents()
    '    Dim dbe As New JRO.JetEngine
    '    dbe.CompactDatabase(strConn, strConn2)
    '    Application.DoEvents()
    '    System.IO.File.Delete("C:\Users\giner\Desktop\Celsius_Marcelo\celsius_louise.mdb")
    '    System.IO.Directory.Move("C:\Users\giner\Desktop\Celsius_Marcelo\celsius_louise2.mdb", "C:\Users\giner\Desktop\Celsius_Marcelo\celsius_louise.mdb")
    '    Db_Cnn.Open(strConn)

    '    chronoStart = Now
    '    codesuite = 0
    '    nrecurs = codesuite
    '    ncpsui = 0
    '    ' vidage tables sorties
    '    'MsgBox("0")
    '    'Dim Db_cnn2 As SqliteConnection
    '    'Db_cnn2.ConnectionString = strConn
    '    'MsgBox("1")
    '    'Dim xx As New SqliteCommand("delete * from outputsynt", Db_cnn2)
    '    ''xx.ExecuteNonQuery()
    '    'MsgBox("2")

    '    TabSynt.Open("OutputSynt", Db_Cnn,, ADODB.LockTypeEnum.adLockOptimistic)
    '    ''TabSynt.MoveFirst()
    '    'Dim i As Integer = 0
    '    'While Not TabSynt.EOF
    '    '    TabSynt.Delete()
    '    '    TabSynt.MoveNext()
    '    '    i += 1
    '    '    Form1.Label4.Text = i
    '    '    Application.DoEvents()
    '    'End While

    '    ' SimInfo.Open("Simul_Info", Db_Cnn)
    '    'SimInfo.MoveFirst()
    '    'xx = New SqliteCommand("Select Count(SimUnitList.idsim) As nimul FROM SimUnitList", Db_cnn2)
    '    'xx.ExecuteNonQuery()
    '    SimInfo.Open("Select Count(SimUnitList.idsim) As nimul FROM SimUnitList", Db_Cnn)
    '    NumSimul = SimInfo.Fields.Item(0).Value

    '    '   MessageBox.Show(NumSimul)

    '    comptesim = 0
    '    SimInfo = Nothing

    '    ReqSim.Open("SimUnitList", Db_Cnn)
    '    ReqSim.MoveFirst()

    '    ' boucle sur la liste des simulations

    '    stopWatch.Start() 'Debut du temps d'execution
    '    MotCelsius.Update() 'ajout FA
    '    MotCelsius.Label4.Text = NumSimul.ToString 'Afficher la quantité totale de sim (modif FA du 25/08: déplacement de cette ligne)
    '    MotCelsius.Label4.Update()
    '    '   Thread.Sleep(0)

    '    While Not ReqSim.EOF
    '        'introduire ici boucle sur les années de simulation si EndYear-StartYear > 1
    '        'prévoir de passer l'année en argument des appels pour controle des lectures de parametres en fonction de l'année...
    '        If (nrecurs > 1) Then
    '            codesuite = 1
    '            If ((ncpsui = nrecurs) Or (comptesim = 0)) Then
    '                codesuite = 0
    '                ncpsui = 0
    '            End If
    '            ncpsui = ncpsui + 1
    '        End If

    '        StartChronoEcrit = Now
    '        Call SimCtrl.ReadParameters(ReqSim, Db_Cnn, comptesim, codesuite)
    '        Chrono1 = Chrono1 + (Now - StartChronoEcrit)
    '        StartChronoCalc = Now
    '        Call SimCtrl.Simulation()
    '        Chrono2 = Chrono2 + (Now - StartChronoCalc)
    '        StartChronoEcrit = Now
    '        Call SimCtrl.SortieSynthesis(TabSynt)
    '        Call SimCtrl.EcritDresu(Db_Cnn, comptesim)
    '        If codesuite = 1 Then Call SimCtrl.MemoEtatFinal()
    '        Chrono1 = Chrono1 + (Now - StartChronoEcrit)
    '        'introduire ici tous les appels de procédures et méthodes de la simulation
    '        'Debug.Print "simulation numero ", comptesim + 1, "/ ", NumSimul

    '        'Forms("MenuPrincipal").Caption = "simulation numero " & comptesim + 1 & "/ " & NumSimul
    '        'DoEvents() //Affichage du calcul


    '        ' Fin des appels
    '        'fin boucle sur les années de simulation si EndYear-StartYear > 1
    '        ReqSim.MoveNext()
    '        comptesim = comptesim + 1
    '        '--------------------------------------------------------------------------------
    '        'Calcule de la simulation actuelle

    '        ts = stopWatch.Elapsed
    '        elapsedTime = String.Format("{0:00}:{1:00}:{2:00}", ts.Hours, ts.Minutes, ts.Seconds)
    '        MotCelsius.Label6.Text = elapsedTime
    '        '        Form1.Label6.Show()
    '        MotCelsius.Label6.Update()
    '        x = Math.Round(comptesim / ts.TotalSeconds, 5)
    '        MotCelsius.Label7.Text = x.ToString + "  sim/s"
    '        MotCelsius.Label7.Update()
    '        If x > 0 Then y = (NumSimul - comptesim) / x
    '        z = Int(y / 3600)
    '        w = Int((y - z * 3600) / 60)
    '        t = y - z * 3600 - w * 60
    '        'y = ((NumSimul - comptesim) / Math.Ceiling(x)) / 60
    '        'z = Int(y / 60)
    '        'w = y - (z * 60)
    '        MotCelsius.Label11.Text = z.ToString + " h : " + w.ToString + " min " + t.ToString + " sec"
    '        MotCelsius.Label11.Update()

    '        MotCelsius.Label3.Text = comptesim.ToString 'Modifier la quantité actuelle de sim
    '        MotCelsius.ProgressBar2.Value = (100 * comptesim) / NumSimul 'Calculé la valeur barre prog
    '        MotCelsius.Label3.Update() 'Mettre à jour la valeur
    '        Application.DoEvents()
    '        Application.RaiseIdle(New EventArgs)
    '        '----------------------------------------------------------------------------------
    '    End While
    '    ReqSim.Close()

    '    ReqSim = Nothing
    '    TabSynt = Nothing
    '    ' chronotot = Now - chronoStart
    '    MsgFin = "OK simulation complete; tps total= " & chronotot & "tps ecriture/lecture= " & Chrono1 & " tps calcul= " & Chrono2
    '    If comptesim <> NumSimul Then MsgFin = "Attention ! Nombre de simulations différent du nombre indiqué"

    '    MsgBox(MsgFin)
    '    MotCelsius.Close()
    'End Sub
    Function Inverse(x) As Double
        Dim msg As String
        Dim ErrFa As Boolean

        If x = 0 Then
            msg = "constantes thermiques nulles !"
            GoTo ErrFA_Inverse
        End If
        Inverse = 1 / x

Exit_Inverse:
        Exit Function
ErrFA_Inverse:
        ErrFa = True
        Console.WriteLine("ERREUR ! " & msg)
        Resume Exit_Inverse

    End Function
    Function Max(x, y) As Object
        If x > y Then Max = x Else Max = y
    End Function
    Function Min(x, y) As Object
        If x > y Then Min = y Else Min = x
    End Function
    Function Calendrier_versSim(jouraconvertir As Integer, jourdebut As Integer, Bissextile As Boolean)
        ' ********NON UTILISE
        ' Utilisation à la place du tableau DOY(joursimul) qui permet de passer à tout moment du
        ' calendrier standard à celui de la simulation

        ' si le jouraconvertir est supérieur à 365, il sera interprété correctement dans le
        'calendrier de simulation
        ' si le jouraconvertir est inférieur à jourdebut on considère qu'il concerne l'année suivant la 1ere année de simul
        If jouraconvertir < jourdebut Then jouraconvertir = jouraconvertir + 365 - CInt(Bissextile)
        Calendrier_versSim = jouraconvertir - jourdebut + 1
    End Function
    'Function Zyva() As Boolean
    '    Call Principal()
    '    Zyva = True
    'End Function
End Module
