Imports System
Imports System.IO
Imports System.Text
Imports System.Globalization
Imports System.Configuration
Imports System.Data
Imports System.Data.Sqlite


Public Class DssatConverter
    Inherits Converter

Public Sub New()
        MasterInput_Connection = New SqliteConnection()
        MasterInput_Connection.ConnectionString = GlobalVariables.dbMasterInput

        ModelDictionary_Connection = New SqliteConnection()
        ModelDictionary_Connection.ConnectionString = GlobalVariables.dbModelsDictionary
    End Sub

    Public Overrides Sub Export(
            DirectoryPath As String,
            MasterInput_Connection As SqliteConnection,
            ModelDictionary_Connection As SqliteConnection)
    End Sub

    Public Overrides Sub Export(DirectoryPath As String)

        Try
            Console.WriteLine("dbMasterInput : " + MasterInput_Connection.ConnectionString)
            Console.WriteLine("dbModelsDictionary : " + ModelDictionary_Connection.ConnectionString )

            MasterInput_Connection.Open()
            ModelDictionary_Connection.Open()

            'Dim setWALQuery As String = "PRAGMA journal_mode = WAL;"
            'Dim command As New SQLiteCommand(setWALQuery, MasterInput_Connection)
            'command.ExecuteNonQuery()
            'command.Dispose()

            ' Set PRAGMA synchronous
            Using command As New SqliteCommand("PRAGMA synchronous = OFF", MasterInput_Connection)
                command.ExecuteNonQuery()
            End Using

            ' Set PRAGMA journal_mode to WAL
            Using command As New SqliteCommand("PRAGMA journal_mode = WAL", MasterInput_Connection)
                command.ExecuteNonQuery()
            End Using

            ' Set PRAGMA synchronous
            Using command As New SqliteCommand("PRAGMA synchronous = OFF", ModelDictionary_Connection)
                command.ExecuteNonQuery()
            End Using
        
            ' Set PRAGMA journal_mode to WAL
            Using command As New SqliteCommand("PRAGMA journal_mode = WAL", ModelDictionary_Connection)
                command.ExecuteNonQuery()
            End Using

            ' Run full checkpoint
            Using command As New SqliteCommand("PRAGMA wal_checkpoint(FULL)", MasterInput_Connection)
                command.ExecuteNonQuery()
            End Using

        Catch ex As Exception
            Console.WriteLine("Connection Error : " + ex.Message)
        End Try 

        Directory.CreateDirectory(DirectoryPath)
        
        Dim createIndexQuery2 As String = "CREATE INDEX IF NOT EXISTS idx_idsim ON SimUnitList (idsim);"
        Dim command3 As New SQLiteCommand(createIndexQuery2, MasterInput_Connection)
        command3.ExecuteNonQuery()

        Dim createIndexQuery As String = "CREATE INDEX IF NOT EXISTS idx_idPoint_year ON RaClimateD (idPoint, year);"
        Dim command_clim As New SQLiteCommand(createIndexQuery, MasterInput_Connection)
        command_clim.ExecuteNonQuery()

        Dim createIndexQuery_idClim As String = "CREATE INDEX IF NOT EXISTS idx_idPoint ON RaClimateD (idPoint);"
        Dim command_idclim As New SQLiteCommand(createIndexQuery_idClim, MasterInput_Connection)
        command_idclim.ExecuteNonQuery()

        Dim createIndexQuery_idcoord As String = "CREATE INDEX IF NOT EXISTS idx_idCoord ON Coordinates (idPoint);"
        Dim command_idcoord As New SQLiteCommand(createIndexQuery_idcoord, MasterInput_Connection)
        command_idcoord.ExecuteNonQuery()

        Dim createIndexQuery_idcropmgt As String = "CREATE INDEX IF NOT EXISTS idx_idMangt ON CropManagement (idMangt);"
        Dim command_idcropmgt As New SQLiteCommand(createIndexQuery_idcropmgt, MasterInput_Connection)
        command_idcropmgt.ExecuteNonQuery()

        Dim createIndexQuery_idsoil As String = "CREATE INDEX IF NOT EXISTS idx_idsoil ON Soil (IdSoil);"
        Dim command_idsoil As New SQLiteCommand(createIndexQuery_idsoil, MasterInput_Connection)
        command_idsoil.ExecuteNonQuery()

        Dim createIndexQuery_idsoill As String = "CREATE INDEX IF NOT EXISTS idx_idsoill ON Soil (Lower(IdSoil));"
        Dim command_idsoill As New SQLiteCommand(createIndexQuery_idsoill, MasterInput_Connection)
        command_idsoill.ExecuteNonQuery()

        Dim createIndexQuery_idsoiltl As String = "CREATE INDEX IF NOT EXISTS idx_idsoiltl ON SoilTypes (Lower(SoilTextureType));"
        Dim command_idsoiltl As New SQLiteCommand(createIndexQuery_idsoiltl, MasterInput_Connection)
        command_idsoiltl.ExecuteNonQuery()

        Dim createIndexQuery_idoption As String = "CREATE INDEX IF NOT EXISTS idx_idoption ON SimulationOptions (idOptions);"
        Dim command_idoption As New SQLiteCommand(createIndexQuery_idoption, MasterInput_Connection)
        command_idoption.ExecuteNonQuery()

        Dim createIndexQuery_listcult As String = "CREATE INDEX IF NOT EXISTS idx_cultivars ON ListCultivars (idCultivar);"
        Dim command_listcult As New SQLiteCommand(createIndexQuery_listcult, MasterInput_Connection)
        command_listcult.ExecuteNonQuery()

        Dim createIndexQuery_listcultvOpt As String = "CREATE INDEX IF NOT EXISTS idx_cultopt ON ListCultivars (CodePSpecies);"
        Dim command_listcultvOpt As New SQLiteCommand(createIndexQuery_listcultvOpt, MasterInput_Connection)
        command_listcultvOpt.ExecuteNonQuery()

        Dim createIndexQuery_listcultOpt As String = "CREATE INDEX IF NOT EXISTS idx_cultoptspec ON ListCultOption (CodePSpecies);"
        Dim command_listcultOpt As New SQLiteCommand(createIndexQuery_listcultOpt, MasterInput_Connection)
        command_listcultOpt.ExecuteNonQuery()

        Dim createIndexQuery_org As String = "CREATE INDEX IF NOT EXISTS idx_orga ON OrganicFOperations (idFertOrga);"
        Dim command_org As New SQLiteCommand(createIndexQuery_org, MasterInput_Connection)
        command_org.ExecuteNonQuery()

        Dim createIndexQuery_orgR As String = "CREATE INDEX IF NOT EXISTS idx_orga_res ON OrganicFOperations (TypeResidues);"
        Dim command_orgR As New SQLiteCommand(createIndexQuery_orgR, MasterInput_Connection)
        command_orgR.ExecuteNonQuery()

        Dim createIndexQuery_res As String = "CREATE INDEX IF NOT EXISTS idx_res ON ListResidues (TypeResidues);"
        Dim command_res As New SQLiteCommand(createIndexQuery_res, MasterInput_Connection)
        command_res.ExecuteNonQuery()

        Dim createIndexQuery4 As String = "CREATE INDEX IF NOT EXISTS idx_model_table ON Variables (model, [Table]);"
        Dim command4 As New SQLiteCommand(createIndexQuery4, ModelDictionary_Connection)
        command4.ExecuteNonQuery()
        'weather_site query
        Dim Q1 As String = "select * from SimUnitList;"
        'Dim fetchAllQuery As String = "select * from Arise_Simul Where Model='DSSAT';"

        'Init and use DataAdapter
        Dim DASL as SqliteDataAdapter = New SqliteDataAdapter(Q1, MasterInput_Connection)
        Dim DS As New DataSet()
        DASL.Fill(DS, "dssat_SL")
        Dim DT As DataTable = DS.Tables("dssat_SL")

        '******************************************
        'For Each row1 In DT.Rows
        Dim myOptions As ParallelOptions = New ParallelOptions()
        myOptions.MaxDegreeOfParallelism = nthreads
        
        If send = -1 Or send > DT.Rows.Count Then
          setSend(DT.Rows.Count)
        End If

        Console.WriteLine("******************************************")
        Console.WriteLine("Dssat convertor loop...")
        Console.WriteLine("number of simulation to generate : " & CStr(DT.Rows.Count))
        Console.WriteLine("number of threads : " & CStr(nthreads))
        Console.WriteLine("start at : " & CStr(sstart))
        Console.WriteLine("end at : " & CStr(send))        
        Console.WriteLine("******************************************")
        
        ' Dim result = System.Threading.Tasks.Parallel.For(0, 10, myOptions, Sub(i)
        Dim result = System.Threading.Tasks.Parallel.For(sstart, send, myOptions, Sub(i)
                Console.WriteLine("Iteration " + CStr(i))
                Dim row1 as DataRow = DT.Rows(i)        
                Dim simPath as String
                Dim dssatWeatherConverter As Converter = New DssatWeatherConverter
                Try
                    simPath = DirectoryPath & Path.DirectorySeparatorChar & 
                        row1.item("idsim") & Path.DirectorySeparatorChar & 
                        row1.item("IdPoint") & Path.DirectorySeparatorChar & 
                        row1.item("StartYear") & Path.DirectorySeparatorChar &
                        row1.item("IdMangt")
                    ' dssatWeatherConverter.Export(DirectoryPath & "\" & row.item("idsim") & "\" & row.item("IdPoint") & "\" & row.item("StartYear") & "\" & row.item("IdMangt"))
                    dssatWeatherConverter.Export(simPath, MasterInput_Connection, ModelDictionary_Connection)
                Catch ex As Exception
                    MessageBox.Show("Error during Export DSSAT WEATHER : " & ex.Message)
                    MessageBox.Show("Stack Trace: " & vbCrLf & ex.StackTrace)
                End Try

                Dim dssatSoilConverter As Converter = New DssatSoilConverter
                Try
                    simPath = DirectoryPath & Path.DirectorySeparatorChar &
                        row1.item("idsim") & Path.DirectorySeparatorChar & 
                        row1.item("idsoil") & Path.DirectorySeparatorChar & 
                        row1.item("IdPoint") & Path.DirectorySeparatorChar & 
                        row1.item("StartYear") & Path.DirectorySeparatorChar &
                        row1.item("IdMangt")
                    ' dssatSoilConverter.Export(DirectoryPath & "\" & row.item("idsim") & "\" & row.item("idsoil") & "\" & row.item("IdPoint") & "\" & row.item("StartYear") & "\" & row.item("IdMangt"))
                    dssatSoilConverter.Export(simPath, MasterInput_Connection, ModelDictionary_Connection)
                Catch ex As Exception
                    MessageBox.Show("Error during Export DSSAT SOIL")
                    MessageBox.Show("Stack Trace: " & vbCrLf & ex.StackTrace)
                End Try

                Dim dssatCultivarConverter As Converter = New DssatCultivarConverter
                Try
                    simPath = DirectoryPath & Path.DirectorySeparatorChar &
                        row1.item("idsim") & Path.DirectorySeparatorChar & row1.item("idsim")
                    ' dssatCultivarConverter.Export(DirectoryPath & "\" & row.item("idsim") & "\" & row.item("idsim"))
                    dssatCultivarConverter.Export(simPath, MasterInput_Connection, ModelDictionary_Connection)
                Catch ex As Exception
                    MessageBox.Show("Error during Export DSSAT CULTIVAR : " & ex.Message)
                    MessageBox.Show("Stack Trace: " & vbCrLf & ex.StackTrace)
                End Try

                Dim dssatSgxConverter As Converter = New DssatXConverter
                Try
                    simPath = DirectoryPath & Path.DirectorySeparatorChar &
                        row1.item("idsim") & Path.DirectorySeparatorChar & row1.item("IdMangt")
                    ' dssatSgxConverter.Export(DirectoryPath & "\" & row.item("idsim") & "\" & row.item("IdMangt"))
                    dssatSgxConverter.Export(simPath, MasterInput_Connection, ModelDictionary_Connection)
                Catch ex As Exception
                    MessageBox.Show("Error during Export DSSAT X")
                    MessageBox.Show("Stack Trace: " & vbCrLf & ex.StackTrace)   
                End Try
                ' Form1.msgErr_expDssat_export.Text = row.item("idsim")
                ' Form1.msgErr_expDssat_export.Refresh()
                ' 'fileC1.AppendLine("Cd " & DirectoryPath & "\" & row.item("Folder"))
                ' fileC1.AppendLine("Start /d " & DirectoryPath & "\" & row.item("idsim") & " " & row.item("idsim") & ".bat")
                ' Try
                '     ' Export file to specified directory
                '     'MsgBox(DirectoryPath & "\" & row.item("idsim"), row.item("idsim") & ".bat", "C:\DSSAT47\DSCSM047.EXE B DSSBatch.v47" & vbCrLf & "exit")
                '     WriteFile(DirectoryPath & "\" & row.item("idsim"), row.item("idsim") & ".bat", "C:\DSSAT47\DSCSM047.EXE B DSSBatch.v47" & vbCrLf & "exit")
                ' Catch ex As Exception
                '     MessageBox.Show("Error during writing file")
                ' End Try
            'Next
            End Sub
        )
        Console.WriteLine("Result: " & CStr(result.IsCompleted))
        ' Try
        '     ' Export file to specified directory
        '     WriteFile(DirectoryPath, "Dssat.bat", fileC1.ToString())
        ' Catch ex As Exception
        '     MessageBox.Show("Error during writing file")
        ' End Try
        ' 'Connection.Close()
        ' MasterInput_Connection.Close()
        Try
            MasterInput_Connection.Close()
            ModelDictionary_Connection.Close()
        Catch ex As Exception
            MessageBox.Show("Connection Error : "  + ex.Message)
        End Try
    End Sub

    Public Overrides Sub Import(DirectoryPath As String, model As String)
        MessageBox.Show("import dssat")
    End Sub

    
   
End Class
