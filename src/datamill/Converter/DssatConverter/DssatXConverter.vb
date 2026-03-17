Imports System
Imports System.IO
Imports System.Text
Imports System.Globalization
Imports System.Configuration
Imports System.Data
Imports System.Data.Sqlite

Public Class DssatXConverter
    Inherits Converter

    
    Public Overrides Sub Export(DirectoryPath As String)
    End Sub

    Public Overrides Sub Export(
            DirectoryPath As String,
            MasterInput_Connection As SQLiteConnection,
            ModelDictionary_Connection As SQLiteConnection)

        ' Dim Connection As New SqliteConnection
        ' Connection.ConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" & RepSource & "\ModelsDictionaryArise.accdb"
        ' Dim MI_Connection = New SqliteConnection()
        ' MI_Connection.ConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" & RepSource & "\MasterInput.accdb"
        ' Try
        '     'Open DB connection
        '     Connection.Open()
        '     MI_Connection.Open()
        ' Catch ex As Exception
        '     MessageBox.Show("Connection Error : " + ex.Message)
        ' End Try
        Dim idSim, idMangt As String
        Dim ST(10) As String
        Dim Site, Year As String
        ' ST = DirectoryPath.Split("\")
        ' DirectoryPath = ST(0) & "\" & ST(1) & "\" & ST(2) & "\" & ST(3) & "\" & ST(4) & "\" & ST(5) & "\" & ST(6) & "\" & ST(7)
        ST = DirectoryPath.Split(Path.DirectorySeparatorChar)
        DirectoryPath = ST(0) & Path.DirectorySeparatorChar &
            ST(1) & Path.DirectorySeparatorChar &
            ST(2) & Path.DirectorySeparatorChar &
            ST(3) & Path.DirectorySeparatorChar &
            ST(4) & Path.DirectorySeparatorChar &
            ST(5) & Path.DirectorySeparatorChar &
            ST(6) & Path.DirectorySeparatorChar & ST(7)
        Site = ST(8)
        idMangt = ST(8)
        'Year = ST(7)
        idSim = ST(7)
        'ST = Year.Split(".")
        'Site = ST(0)
        'Year = ST(1)
        Dim T As String = "Select Champ, Default_Value_Datamill, defaultValueOtherSource, IFNULL([defaultValueOtherSource],[Default_Value_Datamill]) As dv From Variables Where ((model = 'dssat') And ([Table] like 'dssat_x_%'));"

        Dim DT As New DataSet()
        Dim Dv As String
        Dim rw() As DataRow
        Dim Cmd As SQLiteDataAdapter = New SQLiteDataAdapter(T, ModelDictionary_Connection)
        Cmd.Fill(DT, "TChamp")      'weather_site query

        'dssat_x_exp query
        'Dim fetchAllQuery As String = "select * from dssat_x_exp where filename='" & ST(3) & "';"

        'Init and use DataAdapter
        'Using dataAdapter As SqliteDataAdapter = New OleDbDataAdapter(fetchAllQuery, Connection)

        ' Filling Dataset
        'Dim dataSet As New DataSet()
        'dataAdapter.Fill(dataSet, "dssat_x_exp")
        'Dim dataTable As DataTable = dataSet.Tables("dssat_x_exp")
        Dim fileName As String = ""
        Dim idData As Integer
        Dim header As String = ""
        Dim siteColumnsHeader() As String = {"@", "PAREA", " PRNO", " PLEN", " PLDR", " PLSP", " PLAY", "HAREA", " HRNO", " HLEN", "HARM........."}

        Dim fileContent As StringBuilder = New StringBuilder()
        'fileContent.Append(FileHeader) ' Write File header

        'read all line of dssat_x_exp
        'For Each row In dataTable.Rows
        rw = DT.Tables(0).Select("Champ='filename'")
        Dv = rw(0)("dv").ToString
        'fileContent.Append(Dv.PadLeft(5))
        fileName = Dv.ToString
        'idData = row.item("id")
        rw = DT.Tables(0).Select("Champ='header'")
        Dv = rw(0)("dv").ToString
        fileContent.Append(Dv)
        'fileContent.Append(row.item("header"))
        'store header to retrieve name
        header = Dv ' row.item("header")


        fileContent.AppendLine() ' Append a line break.
        fileContent.AppendLine() ' Append a line break.
        fileContent.Append("*GENERAL")
        fileContent.AppendLine() ' Append a line break.
        fileContent.Append("@PEOPLE")
        fileContent.AppendLine() ' Append a line break.
        rw = DT.Tables(0).Select("Champ='PEOPLE'")
        Dv = rw(0)("dv").ToString
        fileContent.Append(Dv)
        'fileContent.Append(row.item("PEOPLE"))
        fileContent.AppendLine() ' Append a line break.
        fileContent.Append("@ADDRESS")
        fileContent.AppendLine() ' Append a line break.
        rw = DT.Tables(0).Select("Champ='ADDRESS'")
        Dv = rw(0)("dv").ToString
        fileContent.Append(Dv)
        'fileContent.Append(row.item("ADDRESS"))
        fileContent.AppendLine() ' Append a line break.
        fileContent.Append("@SITE")
        fileContent.AppendLine() ' Append a line break.
        rw = DT.Tables(0).Select("Champ='SITE'")
        Dv = rw(0)("dv").ToString
        fileContent.Append(Dv)
        'fileContent.Append(row.item("SITE"))
        fileContent.AppendLine() ' Append a line break.
        fileContent.Append(String.Join(Chr(32), siteColumnsHeader))
        'fileContent.Append("@ PAREA  PRNO  PLEN  PLDR  PLSP  PLAY HAREA  HRNO  HLEN  HARM.........")
        fileContent.AppendLine() ' Append a line break.
        fileContent.Append(Chr(32))
        rw = DT.Tables(0).Select("Champ='PAREA'")
        Dv = rw(0)("dv").ToString
        fileContent.Append(Dv.PadLeft(6))
        'fileContent.Append(formatItem_Lg(row.Item("PAREA"), 6))
        fileContent.Append(Chr(32))
        rw = DT.Tables(0).Select("Champ='PRNO'")
        Dv = rw(0)("dv").ToString
        fileContent.Append(Dv.PadLeft(5))
        'fileContent.Append(formatItem_Lg(row.Item("PRNO"), 5))
        fileContent.Append(Chr(32))
        rw = DT.Tables(0).Select("Champ='PLEN'")
        Dv = rw(0)("dv").ToString
        fileContent.Append(Dv.PadLeft(5))
        'fileContent.Append(formatItem_Lg(row.Item("PLEN"), 5))
        fileContent.Append(Chr(32))
        rw = DT.Tables(0).Select("Champ='PLDR'")
        Dv = rw(0)("dv").ToString
        fileContent.Append(Dv.PadLeft(5))
        'fileContent.Append(formatItem_Lg(row.Item("PLDR"), 5))
        fileContent.Append(Chr(32))
        rw = DT.Tables(0).Select("Champ='PLSP'")
        Dv = rw(0)("dv").ToString
        fileContent.Append(Dv.PadLeft(5))
        'fileContent.Append(formatItem_Lg(row.Item("PLSP"), 5))
        fileContent.Append(Chr(32))
        rw = DT.Tables(0).Select("Champ='PLAY'")
        Dv = rw(0)("dv").ToString
        fileContent.Append(Dv.PadLeft(5))
        'fileContent.Append(formatItem_Lg(row.Item("PLAY"), 5))
        fileContent.Append(Chr(32))
        rw = DT.Tables(0).Select("Champ='HAREA'")
        Dv = rw(0)("dv").ToString
        fileContent.Append(Dv.PadLeft(5))
        'fileContent.Append(formatItem_Lg(row.Item("HAREA"), 5))
        fileContent.Append(Chr(32))
        rw = DT.Tables(0).Select("Champ='HRNO'")
        Dv = rw(0)("dv").ToString
        fileContent.Append(Dv.PadLeft(5))
        'fileContent.Append(formatItem_Lg(row.Item("HRNO"), 5))
        fileContent.Append(Chr(32))
        rw = DT.Tables(0).Select("Champ='HLEN'")
        Dv = rw(0)("dv").ToString
        fileContent.Append(Dv.PadLeft(5))
        'fileContent.Append(formatItem_Lg(row.Item("HLEN"), 5))
        fileContent.Append(Chr(32))
        rw = DT.Tables(0).Select("Champ='HARM'")
        Dv = rw(0)("dv").ToString
        fileContent.Append(Dv)
        'fileContent.Append(formatItem(row.Item("HARM")))
        fileContent.AppendLine() ' Append a line break.
        'rw = DT.Tables(0).Select("Champ='PEOPLE'")
        'Dv = rw(0)("dv").ToString
        'fileContent.Append(Dv)
        fileContent.Append("@NOTES")
        fileContent.AppendLine() ' Append a line break.
        rw = DT.Tables(0).Select("Champ='NOTES'")
        Dv = rw(0)("dv").ToString
        fileContent.Append(Dv)
        'fileContent.Append(formatItem(row.item("NOTES")))
        fileContent.AppendLine() ' Append a line break.

        '---------------------------------------------------------------------------------------------
        '*TREATMENTS  
        'table dssat_x_treatment
        '---------------------------------------------------------------------------------------------

        Dim dssat_tableName As String = "dssat_x_treatment"
        Dim dssat_tableId As String = "dssat_x_exp_id"
        Dim dssat_tableId_value As String = idData.ToString
        writeBlockTreatment(dssat_tableName, idSim, dssat_tableId_value, fileContent, ModelDictionary_Connection, MasterInput_Connection)
        '---------------------------------------------------------------------------------------------
        '*CULTIVARS"
        'table dssat_x_cultivar
        '---------------------------------------------------------------------------------------------
        dssat_tableName = "dssat_x_cultivar"
        dssat_tableId = "dssat_x_exp_id"
        writeBlockCultivar(dssat_tableName, idMangt, dssat_tableId_value, fileContent, ModelDictionary_Connection, MasterInput_Connection)

        '---------------------------------------------------------------------------------------------
        '*FIELDS
        'table x_field
        '---------------------------------------------------------------------------------------------
        dssat_tableName = "dssat_x_field"
        dssat_tableId = "dssat_x_exp_id"
        writeBlockField(dssat_tableName, dssat_tableId, idMangt, fileContent, ModelDictionary_Connection) 'site

        '---------------------------------------------------------------------------------------------
        'table soil_analysis
        dssat_tableName = "dssat_x_soil_analysis"
        dssat_tableId = "dssat_x_exp_id"
        writeBlockSoilAnalysis(dssat_tableName, dssat_tableId, dssat_tableId_value, fileContent, ModelDictionary_Connection)

        '---------------------------------------------------------------------------------------------
        'table dssat_x_initial_condition
        dssat_tableName = "dssat_x_initial_condition"
        dssat_tableId = "dssat_x_exp_id"
        writeBlockInitialCondition(dssat_tableName, idSim, dssat_tableId_value, fileContent, ModelDictionary_Connection, MasterInput_Connection)

        '---------------------------------------------------------------------------------------------
        'table dssat_x_planting_detail
        dssat_tableName = "dssat_x_planting_detail"
        dssat_tableId = "dssat_x_exp_id"
        writeBlockPlantingDetail(dssat_tableName, idSim, dssat_tableId_value, fileContent, ModelDictionary_Connection, MasterInput_Connection)

        '---------------------------------------------------------------------------------------------
        'irrigation and water management
        'table dssat_x_irrigation_water
        '---------------------------------------------------------------------------------------------
        dssat_tableName = "dssat_x_irrigation_water"
        dssat_tableId = "dssat_x_exp_id"
        writeBlockIrrigationWater(dssat_tableName, dssat_tableId, dssat_tableId_value, fileContent, ModelDictionary_Connection)

        '---------------------------------------------------------------------------------------------
        ' fertilizer
        ' table dssat_x_fertilizer
        '---------------------------------------------------------------------------------------------
        dssat_tableName = "dssat_x_fertilizer"
        '         dssat_tableId = "dssat_x_exp_id"
        writeBlockFertilizer(dssat_tableName, idSim, dssat_tableId_value, fileContent, ModelDictionary_Connection, MasterInput_Connection)

        '---------------------------------------------------------------------------------------------
        'RESIDUES AND ORGANIC FERTILIZER
        'table dssat_x_residues
        '---------------------------------------------------------------------------------------------
        dssat_tableName = "dssat_x_residues"
        dssat_tableId = "dssat_x_exp_id"
        writeBlockResidues(dssat_tableName, idSim, dssat_tableId_value, fileContent, ModelDictionary_Connection, MasterInput_Connection)

        '---------------------------------------------------------------------------------------------
        '*CHEMICAL APPLICATIONS
        'table(dssat_x_chemical_application)
        '---------------------------------------------------------------------------------------------
        dssat_tableName = "dssat_x_chemical_application"
        dssat_tableId = "dssat_x_exp_id"
        writeBlockChemicalApplication(dssat_tableName, dssat_tableId, dssat_tableId_value, fileContent, ModelDictionary_Connection)

        '---------------------------------------------------------------------------------------------
        '*TILLAGE AND ROTATIONS
        ' table(dssat_x_tillage)
        '--------------------------------------------------------------------------------------------
        dssat_tableName = "dssat_x_tillage"
        dssat_tableId = "dssat_x_exp_id"
        writeBlockTillageRotation(dssat_tableName, idSim, dssat_tableId_value, fileContent, ModelDictionary_Connection, MasterInput_Connection)

        '---------------------------------------------------------------------------------------------
        '*ENVIRONMENT MODIFICATIONS
        'table(dssat_x_environnement)
        '---------------------------------------------------------------------------------------------
        dssat_tableName = "dssat_x_environment"
        dssat_tableId = "dssat_x_exp_id"
        writeBlockEnvironment(dssat_tableName, dssat_tableId, dssat_tableId_value, fileContent, ModelDictionary_Connection)

        '---------------------------------------------------------------------------------------------
        '*HARVEST DETAILS
        'table(dssat_x_harvest)
        '---------------------------------------------------------------------------------------------
        dssat_tableName = "dssat_x_harvest"
        dssat_tableId = "dssat_x_exp_id"
        writeBlockHarvest(dssat_tableName, idSim, dssat_tableId_value, fileContent, ModelDictionary_Connection, MasterInput_Connection)

        '---------------------------------------------------------------------------------------------
        '*SIMULATION CONTROLS
        'Automatic Managment
        '---------------------------------------------------------------------------------------------
        fileContent.AppendLine() ' Append a line break.
        fileContent.Append("*SIMULATION CONTROLS")
        fileContent.AppendLine() ' Append a line break.

        writeBlockEndFile(fileContent, idSim, ModelDictionary_Connection, MasterInput_Connection)

        '---------------------------------------------------------------------------------------------
        ' write file
        '---------------------------------------------------------------------------------------------
        Try
            ' Export file to specified directory
            WriteFile(DirectoryPath, fileName, fileContent.ToString())
            fileContent.Clear()
        Catch ex As Exception
            MessageBox.Show("Error during writing file")
        End Try
        '---------------------------------------------------------------------------------------------
        '*Fichier DSSBatch.v47  
        'table dssat_x_treatment
        '---------------------------------------------------------------------------------------------
        fileContent.Clear()
        dssat_tableName = "dssat_x_treatment"
        dssat_tableId = "dssat_x_exp_id"
        dssat_tableId_value = idData.ToString
        writeBlockTreatment2(dssat_tableName, fileName, dssat_tableId_value, idSim, fileContent, ModelDictionary_Connection)
        '        Try
        ' Export file to specified directory
        WriteFile(DirectoryPath, "DSSBatch.v47", fileContent.ToString())
        fileContent.Clear()
        '        Catch ex As Exception
        '            MessageBox.Show("Error during writing file")
        '        End Try

        'next occurence of dssat_x_exp
        'Next
        'End Using

        ' Connection.Close()
        ' MI_Connection.Close()
    End Sub

    Public Overrides Sub Import(DirectoryPath As String, model As String)

    End Sub
End Class
