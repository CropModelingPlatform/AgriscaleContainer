Imports System.Data
Imports System.Data.Sqlite
Imports System.Configuration
Imports System.Threading
Imports System.Globalization
Imports System.DateTime
Imports System.IO

Public Module GlobalVariables
    Public dbCelsius As String = String.Empty
    Public dbCelsius_ori As String = String.Empty
End Module

'Public Class MotCelsius
Public Module MotCelsius
    Sub Main(args As String())
        Console.WriteLine("Start : "  & DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"))
        
        ' Set Locale
        Thread.CurrentThread.CurrentCulture = New CultureInfo("fr-FR")
        Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator = "."

        ' Get db connections string
        Dim appSettings = ConfigurationManager.AppSettings 'Read Stored Value
        dbCelsius_ori = appSettings("db_celsius")

        Console.WriteLine("dbCelsius ori : " + dbCelsius_ori)

        ' Get the values of the command line in an array
        Dim clArgs() As String = Environment.GetCommandLineArgs()
        Dim cmd As String = String.Empty
        Dim nthreads As Integer = 1
        Dim models As String = String.Empty
        Dim fun As String = String.Empty
        Dim sstart As Integer = 0
        Dim send As Integer = -1
        
        Console.WriteLine("n args : " + CStr(clArgs.Count()))
        If clArgs.Count() > 1 Then
            cmd = clArgs(1)
            For i As Integer = 1 To (clArgs.Count() - 1) Step 1
                If clArgs(i) = "-m" Then
                    models = clArgs(i + 1)
                End If
                'If clArgs(i) = "-dbMasterInput" Then
                '    dbMasterInput = "URI=file:" & clArgs(i + 1)
                'End If
                'If clArgs(i) = "-dbModelsDictionary" Then
                '    dbModelsDictionary = "URI=file:" & clArgs(i + 1)
                'End If
                If clArgs(i) = "-dbCelsius" Then
                    dbCelsius = "URI=file:" & clArgs(i + 1)
                End If
                If clArgs(i) = "-t" Then
                    nthreads = clArgs(i + 1)
                End If
                If clArgs(i) = "-fun" Then
                    fun = clArgs(i + 1)
                End If
                If clArgs(i) = "-sstart" Then
                    sstart = clArgs(i + 1)
                End If
                If clArgs(i) = "-send" Then
                    send = clArgs(i + 1)
                End If
            Next
            Console.WriteLine("cmd 111 : " + cmd)
            Console.WriteLine("models 1111 : " + models)
            Console.WriteLine("nthreads 1111 : " + CStr(nthreads))
            'Console.WriteLine("dbMasterInput : " + dbMasterInput)
            'Console.WriteLine("dbModelsDictionary : " + dbModelsDictionary)
            Console.WriteLine("dbCelsius : " + dbCelsius)
            Console.WriteLine("sstart : " + CStr(sstart))
            Console.WriteLine("send : " + CStr(send))
        End If

        'Console.WriteLine("calcul1")
        'Private Sub Form1_Load_1(sender As Object, e As EventArgs) Handles MyBase.Load
        'Me.Show()
        'dernière modif 04/13 numsimul est compté grace à une requête regroupement, n'est plus lu dans Simulinfo
        'modi 25/04/14 numsimul et comptesim passés en Long
        'Calcul du temps passé
        'Dim stopWatch As New Stopwatch()
        Dim ts As Long
        Dim elapsedTime, Pth, FN, StrCh As String
        Dim y, Ecrit, Calcul As Double
        Dim x As Double
        'dernière modif:08/04/2011
        'Dim dbe As New JRO.JetEngine
        Dim Db_Cnn As SQLiteCommand
        Dim SimInfoAdp As SqliteDataAdapter
        Dim SimInfo As New DataSet
        'Dim ReqSim As New DataSet
        Dim TabSyntAdp As SqliteDataAdapter
        Dim TabSynt As New DataSet
        Dim NumSimul As Long
        Dim CodeOptim As Boolean
        Dim comptesim As Long
        Dim MsgFin As String
        Dim chronoStart As Date
        'Dim StartChronoCalc As Date
        'Dim StartChronoEcrit As Date
        'Dim Chrono1 As Date
        'Dim Chrono2 As Date
        'Dim chronotot As Date
        Dim nrecurs As Integer
        Dim SimCtrl As New SimulationControlClass
        Dim ncpsui As Integer
        Dim codesuite As Integer 'gestion de la récursivité
        Dim strConn As SqliteConnection
        'If Ofd.ShowDialog() = System.Windows.Forms.DialogResult.OK Then
        'FN = "D:\Celsius_VB2021\CelsiusV3nov17_dataArise.accdb"
        'Pth = Mid(FN, 1, InStrRev(FN, "\"))
        strConn = New SqliteConnection
        'strConn.ConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" & FN & ";"
        'strConn.ConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" & FN & ";"
        strConn.ConnectionString = dbCelsius
        'strConn2 = New SqliteConnection
        'strConn2.ConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" & Pth & "Celsius_Temp.mdb;"
        'strConn2.ConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" & Pth & "Celsius_Temp.accdb;"
        'c modif de la gestion de la récursivité repris de sticpail
        'c si nrecurs=1, toutes les situations sont initialisées avec recup.tmp
        ' si nrecurs est >1 la premiere sim est initialisée avec le contenu
        'c de simul.usm, les codesuite-1 suivantes sont initialisées avec recup.tmp
        'c et la suivante de nouveau avec simul.usm et ainsi de suite
        'c exemples:
        'c codesuite dans simul.usm:  dans la séquence des simulations:
        'c                  2                 010101.
        'c                  3                   011011011....
        'c                4                 011101110111....
        'c si codesuite=1
        'c la simul de l'année 1 prend fin à la récolte et celle de l'année 2
        'c reprend automatiquement au jour suivant, avec semis décalé de iplt+1 jours par rapport à récolte
        'c avec cette option les stade obs (dont iplt) pour la 2e année, si présents, doivent être aussi
        'c donnés en jours depuis la récolte de la culture antérieure.
        
        Try
            strConn.Open()
            Dim setWALQuery As String = "PRAGMA journal_mode = WAL;"
            Dim command As New SQLiteCommand(setWALQuery, strConn)
            command.ExecuteNonQuery()
            command.Dispose()


            'Dim sql_pragma As String = "PRAGMA synchronous = OFF"
            'Dim command As New SQLiteCommand(sql_pragma, strConn)
            'command.ExecuteNonQuery()
            'command.Dispose()

            'sql_pragma = "PRAGMA journal_mode = MEMORY"
            'command = New SQLiteCommand(sql_pragma, strConn)
            'command.ExecuteNonQuery()
            'command.Dispose()
        Catch ex As Exception
            Console.WriteLine("Connection Error : " + ex.Message)
        End Try        '---------------------------


        'Console.WriteLine("calcul2")
        'Label5.Text = "Modèle Celsius : Vidage des tables de sortie. Patientez"
        Db_Cnn = New SqliteCommand("", strConn)
        Db_Cnn.CommandText = "delete from outputsynt"
        Db_Cnn.ExecuteNonQuery()
        'Application.DoEvents()
        Db_Cnn = New SqliteCommand("", strConn)
        Db_Cnn.CommandText = "delete from outputD_1"
        Db_Cnn.ExecuteNonQuery()

        'Application.DoEvents()
        Db_Cnn = New SqliteCommand("", strConn)
        Db_Cnn.CommandText = "delete from outputD_2"
        Db_Cnn.ExecuteNonQuery()

        'Application.DoEvents()
        'If strConn.State = ConnectionState.Open Then strConn.Close()
        'Console.WriteLine("calcul3")

        'Dim dbe As New JRO.JetEngine
        ' Label5.Text = "Modèle Celsius : Compactage de la base de données. Patientez"
        'dbe.CompactDatabase(strConn.ConnectionString, strConn2.ConnectionString)
        'Application.DoEvents()
        'System.IO.File.Delete(Ofd.FileName)
        'System.IO.Directory.Move(Pth & "Celsius_Temp.mdb", Ofd.FileName)
        ''System.IO.Directory.Move(Pth & "Celsius_Temp.accdb", Ofd.FileName)
        'Db_Cnn.Open(strConn)
        'If strConn.State = ConnectionState.Closed Then strConn.Open()
        'Label5.Text = "Modèle Celsius : Calculs"
        'Console.WriteLine(strConn.State)
        


        Dim createIndexQuery_idgenParam As String = "CREATE INDEX IF NOT EXISTS idx_idGenParam ON General_Parameters (idGenParam);"
        Dim command_genParam As New SQLiteCommand(createIndexQuery_idgenParam, strConn)
        command_genParam.ExecuteNonQuery()

        Dim createIndexQuery_idDclim As String = "CREATE INDEX IF NOT EXISTS idx_idDclim ON Dweather (idDclim, annee);"
        Dim command_idDclim As New SQLiteCommand(createIndexQuery_idDclim, strConn)
        command_idDclim.ExecuteNonQuery()

        Dim createIndexQuery_idIniP As String = "CREATE INDEX IF NOT EXISTS idx_idIniP ON ParamIni (idIni);"
        Dim command_idIniP As New SQLiteCommand(createIndexQuery_idIniP, strConn)
        command_idIniP.ExecuteNonQuery()

        Dim createIndexQuery_idTechF As String = "CREATE INDEX IF NOT EXISTS idx_idTechF ON FertiMin_List (IdTech_Com);"
        Dim command_idTechF As New SQLiteCommand(createIndexQuery_idTechF, strConn)
        command_idTechF.ExecuteNonQuery()

        Dim createIndexQuery_idTechO As String = "CREATE INDEX IF NOT EXISTS idx_idTechO ON FertiOrga_List (IdTech_Com);"
        Dim command_idTechO As New SQLiteCommand(createIndexQuery_idTechO, strConn)
        command_idTechO.ExecuteNonQuery()

        Dim createIndexQuery_idTechC As String = "CREATE INDEX IF NOT EXISTS idx_idTechC ON Tech_Commun (IdTech_Com);"
        Dim command_idTechC As New SQLiteCommand(createIndexQuery_idTechC, strConn)
        command_idTechC.ExecuteNonQuery()


        Dim createIndexQuery_idTechP As String = "CREATE INDEX IF NOT EXISTS idx_idTechP ON Tech_perCrop (IdTech_Com);"
        Dim command_idTechP As New SQLiteCommand(createIndexQuery_idTechP, strConn)
        command_idTechP.ExecuteNonQuery()


        Dim createIndexQuery_idTechI As String = "CREATE INDEX IF NOT EXISTS idx_idTechI ON Irrigation_List (IdTech_Com);"
        Dim command_idTechI As New SQLiteCommand(createIndexQuery_idTechI, strConn)
        command_idTechI.ExecuteNonQuery()

        Dim createIndexQuery_idMulch As String = "CREATE INDEX IF NOT EXISTS idx_idMulch ON Mulch (idMulch);"
        Dim command_idMulch As New SQLiteCommand(createIndexQuery_idMulch, strConn)
        command_idMulch.ExecuteNonQuery()

        Dim createIndexQuery_idTechR As String = "CREATE INDEX IF NOT EXISTS idx_idTechR ON RuissellementObs (idTechCom);"
        Dim command_idTechR As New SQLiteCommand(createIndexQuery_idTechR, strConn)
        command_idTechR.ExecuteNonQuery()

        Dim createIndexQuery_idCult As String = "CREATE INDEX IF NOT EXISTS idx_cultivars ON Cultivars (idCultivar);"
        Dim command_idCult As New SQLiteCommand(createIndexQuery_idCult, strConn)
        command_idCult.ExecuteNonQuery()

        Dim createIndexQuery_idCod As String = "CREATE INDEX IF NOT EXISTS idx_idCod ON OptionsModel (idCodModel);"
        Dim command_idCod As New SQLiteCommand(createIndexQuery_idCod, strConn)
        command_idCod.ExecuteNonQuery()

        Dim createIndexQuery_codP As String = "CREATE INDEX If Not EXISTS idx_codP On PlantSpecies (CodePSpecies);"
        Dim command_codP As New SQLiteCommand(createIndexQuery_codP, strConn)
        command_codP.ExecuteNonQuery()

        Dim createIndexQuery_codS As String = "CREATE INDEX If Not EXISTS idx_codS On StadePheno (CodCultivar);"
        Dim command_codS As New SQLiteCommand(createIndexQuery_codS, strConn)
        command_codS.ExecuteNonQuery()

        Dim createIndexQuery_idsoil As String = "CREATE INDEX If Not EXISTS idx_idSoil On Soil (IdSoil);"
        Dim command_idsoil As New SQLiteCommand(createIndexQuery_idsoil, strConn)
        command_idsoil.ExecuteNonQuery()

        Dim createIndexQuery_typS As String = "CREATE INDEX If Not EXISTS idx_typS On TypeSurfSol (TypeRui);"
        Dim command_idTypS As New SQLiteCommand(createIndexQuery_typS, strConn)
        command_idTypS.ExecuteNonQuery()



        SimInfoAdp = New SqliteDataAdapter("Select * From SimUnitList", strConn) 'cy
        SimInfoAdp.Fill(SimInfo)   'cy

        '' boucle sur la liste des simulations
        NumSimul = SimInfo.Tables(0).Rows.Count 'cy
        Console.WriteLine("NumSimul : " + CStr(NumSimul))
        'Console.WriteLine(NumSimul)

        'Me.Update() 'ajout FA
        'Me.Label4.Text = NumSimul.ToString 'Afficher la quantité totale de sim (modif FA du 25/08: déplacement de cette ligne)
        'Me.Label4.Update()
        '   Thread.Sleep(0)
        If strConn.State = ConnectionState.Closed Then strConn.Open()
        TabSyntAdp = New SqliteDataAdapter("Select * From Outputsynt", strConn)
        TabSyntAdp.Fill(TabSynt)
        StrCh = ""
        For i = 0 To TabSynt.Tables(0).Columns.Count - 1
            StrCh += "[" + TabSynt.Tables(0).Columns(i).ColumnName + "]"
            If i < TabSynt.Tables(0).Columns.Count - 1 Then StrCh += ","
        Next
        TabSynt.Dispose()
        TabSyntAdp.Dispose()
        chronoStart = Now 'cy
        codesuite = 0 'cy
        nrecurs = codesuite 'cy
        ncpsui = 0 'cy
        'stopWatch.Start() 'Debut du temps d'execution
        comptesim = 0 'cy
        Ecrit = 0
        Calcul = 0

        'Dim SimPointAdp As SQLiteDataAdapter
        'Dim SimPoint As New DataSet

        'Dim idPoints As List(Of String) = New List(Of String)()
        'Dim sql As String = "SELECT DISTINCT idweather FROM SimUnitList;"
        'Using command As SQLiteCommand = New SQLiteCommand(sql, strConn)

            ' Execute the query to get the IdPoint values
            'Using reader As SQLiteDataReader = command.ExecuteReader()
                'While reader.Read()
                    'idPoints.Add(reader("idweather").ToString())
                'End While
            'End Using
        'End Using
        'Console.WriteLine("number Of points:" + CStr(idPoints.Count))

        'nthreads = 7
        'Dim parallelOptions As ParallelOptions = New ParallelOptions()
        'parallelOptions.MaxDegreeOfParallelism = nthreads

        'Dim firstTwoPoints = idPoints.Take(3).ToList()
        'replace idPoints by firstTwoPoints
        ' get nthreads and its type
        'console.writeline("nthreads 1 : " + CStr(nthreads))
        'Dim tot = 0
        'Parallel.ForEach(idPoints, parallelOptions, Sub(idPoint)
                                                        'Dim baseDb As String = dbCelsius_ori
                                                        'Dim baseDb As String = dbCelsius_ori.Replace("URI=file:", "")
                                                        'Dim dbPathOnly As String = dbCelsius.Replace("URI=file:", "")
                                                        'Dim tempDb As String = Path.Combine(Path.GetDirectoryName(dbPathOnly), $"Celsius_temp_{idPoint}.db")
                                                        'console.WriteLine("tempDb : " + tempDb)
                                                        'File.Copy(baseDb, tempDb, True)
                                                        'Console.WriteLine(" simul point:" + CStr(idPoint))
                                                        'Using connRead As New SQLiteConnection("Data Source=" & dbPathOnly),
                                                        'connWrite As New SQLiteConnection("Data Source=" & tempDb)
                                          
                                                            'connRead.Open()
                                                            'connWrite.Open()

                                                            'Dim nrecurs As Integer
                                                            'Dim SimCtrl As New SimulationControlClass
                                                            'Dim ncpsui As Integer
                                                            'Dim codesuite As Integer 'gestion de la récursivité
                                                            'Dim comptesim = 0
                                                            'Ecrit = 0
                                                            'Calcul = 0
                                                            'codesuite = 0   
                                                            'nrecurs = codesuite
                                                            'ncpsui = 0
                                                            'Dim SimPointAdp = New SQLiteDataAdapter("Select * From SimUnitList WHERE idweather = '" & idPoint & "' order by StartYear", connRead)
                                                            'Dim SimPoint As New DataSet()
                                                            'SimPointAdp.Fill(SimPoint)
                                                            'Dim total = SimPoint.Tables(0).Rows.Count
                                                            For i = 0 To SimInfo.Tables(0).Rows.Count - 1
                                                            'For i = 0 To total - 1
                                                                Console.WriteLine(i & "/" & SimInfo.Tables(0).Rows.Count)
                                                                'console.writeline("num : " + CStr(i))
                                                                If (nrecurs > 1) Then
                                                                    codesuite = 1
                                                                    If ((ncpsui = nrecurs) Or (comptesim = 0)) Then
                                                                        codesuite = 0
                                                                        ncpsui = 0
                                                                    End If
                                                                    ncpsui = ncpsui + 1
                                                                End If
                                                                'StartChronoEcrit = Now
                                                                Call SimCtrl.ReadParameters(SimInfo, strConn, comptesim, codesuite)
                                                                'Call SimCtrl.ReadParameters(SimPoint, connRead, comptesim, codesuite)
                                                                'Ecrit += (Now.Subtract(StartChronoEcrit)).TotalMilliseconds 'stopWatch.Elapsed
                                                                'StartChronoCalc = Now
                                                                Call SimCtrl.Simulation()
                                                                Call SimCtrl.SortieSynthesis(StrCh, strConn)
                                                                'Call SimCtrl.SortieSynthesis(StrCh, connWrite)
                                                                'Call SimCtrl.SortieSynthesis(TabSynt, strConn)
                                                                Call SimCtrl.EcritDresu(strConn, comptesim)
                                                                If codesuite = 1 Then Call SimCtrl.MemoEtatFinal()
                                                                'Ecrit += (Now.Subtract(StartChronoEcrit)).TotalMilliseconds 'stopWatch.Elapsed
                                                                comptesim = comptesim + 1
                                                                '----------------------------------------------------------------------------------
                                                            Next
                                                        'End Using

                                                    'End Sub)

        Console.WriteLine("End : " & DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"))
    End Sub
End Module
'Private Sub Label3_Click(sender As Object, e As EventArgs) Handles Label3.Click
'    ProgressBar2.Maximum = 100
'    ProgressBar2.Minimum = 1
'End Sub


'Private Sub Form1_Closed(sender As Object, e As EventArgs) Handles Me.Closed
'        End
'    End Sub
'End Class
