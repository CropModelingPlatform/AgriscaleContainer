Imports System
Imports System.IO
Imports System.Text
Imports System.Globalization
Imports System.Configuration
Imports System.Data
Imports System.Data.Sqlite
Imports System.Math

Public Class DssatWeatherConverter
    
    Inherits Converter  

    Public Overrides Sub Export(DirectoryPath As String)
    End Sub

    Public Overrides Sub Export(
            DirectoryPath As String,
            MasterInput_Connection As SqliteConnection,
            ModelDictionary_Connection As SqliteConnection)

        ' 'Init Connection with connection string from app.config
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

        ' Dim ST(10) As String
        ' Dim Site, Year, Mngt As String
        ' ST = DirectoryPath.Split("\")
        ' DirectoryPath = ST(0) & "\" & ST(1) & "\" & ST(2) & "\" & ST(3) & "\" & ST(4) & "\" & ST(5) & "\" & ST(6) & "\" & ST(7)
        ' Site = ST(8)
        ' 'Site.Replace(".", "_")
        ' Year = ST(9)
        Dim ST(10) As String
        Dim Site, Year, Mngt As String
        ST = DirectoryPath.Split(Path.DirectorySeparatorChar)
        DirectoryPath = ST(0) & Path.DirectorySeparatorChar & 
            ST(1) & Path.DirectorySeparatorChar & 
            ST(2) & Path.DirectorySeparatorChar & 
            ST(3) & Path.DirectorySeparatorChar & 
            ST(4) & Path.DirectorySeparatorChar & 
            ST(5) & Path.DirectorySeparatorChar & ST(6) & Path.DirectorySeparatorChar & ST(7)
        Site = ST(8)
        Year = ST(9)        
        Mngt = Mid(ST(10), 1, 4)
        'ST = Year.Split(".")
        'Year = ST(1)
        'weather_site query
        Dim T As String = "Select Champ, Default_Value_Datamill, defaultValueOtherSource, IFNULL([defaultValueOtherSource],[Default_Value_Datamill]) As dv From Variables Where ((model = 'dssat') And ([Table] = 'dssat_weather_site'));"

        Dim DT As New DataSet()
        Dim Dv As String
        Dim rw() As DataRow
        Dim Cmd As SqliteDataAdapter = New SqliteDataAdapter(T, ModelDictionary_Connection)
        Cmd.Fill(DT, "TChamp")
        Dim fetchAllQuery = "select * from Coordinates where idPoint='" + Site + "';"
        'Dim fetchAllQuery As String = "select * from dssat_weather_site where filename='" & ST(3) & "';"
        Dim Tdew As Single
        Dim fileNameArray(3) As String
        fileNameArray(0) = ""
        fileNameArray(1) = "00"
        fileNameArray(2) = "01"
        fileNameArray(3) = ".WTH"
        For i = 0 To 1
            Year = Year + i
            'Init and use DataAdapter
            'Dim dataAdapter As SqliteDataAdapter = New OleDbDataAdapter(fetchAllQuery, MasterInput_Connection)
            Using dataAdapter As SqliteDataAdapter = New SqliteDataAdapter(fetchAllQuery, MasterInput_Connection)
                ' Filling Dataset
                Dim dataSet As New DataSet()
                dataAdapter.Fill(dataSet, "dssat_weather_site")
                Dim dataTable As DataTable = dataSet.Tables("dssat_weather_site")
                Dim fileName As String = ""
                'read all line of dssat_weather_site
                For Each row In dataTable.Rows
                    Dim fileContent As StringBuilder = New StringBuilder()
                    'filename is composed by "insi"+"aa"+"number, usually 01
                    'aa is deducted from "date" : fisrt two characters
                    Dim siteColumnsHeader() As String = {"@", "INSI", "     LAT", "    LONG", " ELEV", "  TAV", "  AMP", "REFHT", "WNDHT"}

                    fileContent.Append("*WEATHER DATA : " & Site & "," & Year)
                    'fileContent.Append(row.item("header_weather_data"))
                    fileContent.AppendLine() ' Append a line break.
                    fileContent.Append(String.Join(Chr(32), siteColumnsHeader))
                    fileContent.AppendLine() ' Append a line break.

                    fileContent.Append(Chr(32))
                    fileContent.Append(Chr(32))
                    'fileContent.Append(formatItem_Lg((row.item("insi")), 6))
                    fileContent.Append(Mid(Site, 1, 4).PadRight(6)) 'formatItem_Lg((row.item("insi")), 6))
                    fileNameArray(0) = Mngt.ToUpper() 'Mid(Site, 1, 4) 'formatItem(row.item("insi"))
                    fileNameArray(1) = Mid(Year, 3, 2)
                    fileContent.Append(Chr(32))
                    fileContent.Append(row.item("latitudeDD").ToString.PadLeft(6))
                    fileContent.Append(Chr(32))
                    fileContent.Append(row.item("longitudeDD").ToString.PadLeft(8))
                    fileContent.Append(Chr(32))
                    fileContent.Append(row.item("altitude").ToString.PadLeft(5))
                    'fileContent.Append(Chr(9))
                    fileContent.Append(Chr(32))
                    rw = DT.Tables(0).Select("Champ='tav'")
                    Dv = rw(0)("dv").ToString
                    fileContent.Append(Dv.PadLeft(5))
                    fileContent.Append(Chr(32))
                    rw = DT.Tables(0).Select("Champ='amp'")
                    Dv = rw(0)("dv").ToString
                    fileContent.Append(Dv.PadLeft(5))
                    fileContent.Append(Chr(32))
                    rw = DT.Tables(0).Select("Champ='refht'")
                    Dv = rw(0)("dv").ToString
                    fileContent.Append(Dv.PadLeft(5))
                    'fileContent.Append(Chr(9))
                    fileContent.Append(Chr(32))
                    rw = DT.Tables(0).Select("Champ='wndht'")
                    Dv = rw(0)("dv").ToString
                    fileContent.Append(Dv.PadLeft(5))
                    fileContent.AppendLine() ' Append a line break.

                    'weatherdata
                    Dim dataColumnsHeader() As String = {"@DATE", " SRAD", " TMAX", "  TMIN", " RAIN", " DEWP", " WIND", "  PAR", " EVAP", " RHUM"}
                    fileContent.Append(String.Join(Chr(32), dataColumnsHeader))
                    fileContent.AppendLine() ' Append a line break.
                    'Init and use DataAdapter
                    Dim fetchAllQuery1 As String = "select * from RaClimateD where idPoint='" + Site + "' And Year=" & Year & ";"

                    Using dataAdapter1 As SqliteDataAdapter = New SqliteDataAdapter(fetchAllQuery1, MasterInput_Connection)
                        'Dim dataAdapter1 = New OleDbDataAdapter(fetchAllQuery1, MasterInput_Connection)
                        Dim dataSet1 As New DataSet()
                        dataAdapter1.Fill(dataSet1, "dssat_weather_data")
                        Dim dataTable1 As DataTable = dataSet1.Tables("dssat_weather_data")
                        'MsgBox(Site & " " & Year & " " & dataTable1.Rows.Count & " " & fetchAllQuery1)
                        fileNameArray(2) = "01" '(i + 1).ToString.PadLeft(2, "0")
                        ''read all line of dssat_weather_site
                        For Each occurence As DataRow In dataTable1.Rows
                            fileContent.AppendLine() ' Append a line break.
                            'fileContent.Append(formatItem(occurence.Item("date")))
                            fileContent.Append(Mid(occurence.Item("year").ToString, 3, 2) & occurence.Item("doy").ToString.PadLeft(3, "0"))
                            'store fisrt two years
                            'fileNameArray(1) = formatItem(occurence.Item("date"))

                            fileContent.Append(Chr(32))
                            fileContent.Append(FormatNumber(occurence.Item("srad"), 2).ToString.PadLeft(5))
                            fileContent.Append(Chr(32))
                            fileContent.Append(occurence.Item("tmax").ToString.PadLeft(5))
                            fileContent.Append(Chr(32))
                            fileContent.Append(occurence.Item("tmin").ToString.PadLeft(6))
                            fileContent.Append(Chr(32))
                            fileContent.Append(FormatNumber(occurence.Item("rain"), 2).ToString.PadLeft(5))
                            fileContent.Append(Chr(32))
                            If IsDBNull(occurence.Item("Tdewmin")) Or IsDBNull(occurence.Item("Tdewmax")) Then
                                fileContent.Append(" ".PadLeft(5))
                            Else
                                Tdew = (CSng(occurence.Item("Tdewmin")) + CSng(occurence.Item("Tdewmax"))) / 2
                                fileContent.Append(FormatNumber(Tdew, 2).ToString.PadLeft(5)) 'dewp
                            End If
                            fileContent.Append(Chr(32))
                            fileContent.Append(FormatNumber(occurence.Item("wind") * 86.4, 1).PadLeft(5))
                            fileContent.Append(Chr(32))
                            fileContent.Append(" ".PadLeft(5)) 'par
                            fileContent.Append(Chr(32))
                            fileContent.Append(" ".PadLeft(5)) 'evap
                            fileContent.Append(Chr(32))
                            'Console.WriteLine("RHUM :" & occurence.Item("rhum") & ":")
                            If IsDBNull(occurence.Item("rhum")) Then
                                fileContent.Append(" ".PadLeft(5))
                            Else
                                fileContent.Append(FormatNumber(occurence.Item("rhum"), 2).PadLeft(5))
                            End If
                        Next
                    End Using
                    'file name
                    'fileName = insiValue + yyFile + "01"
                    fileName = fileNameArray(0) + fileNameArray(1) + fileNameArray(2) + fileNameArray(3)
                        'MsgBox(fileName)
                        ' Export file to specified directory
                        WriteFile(DirectoryPath, fileName, fileContent.ToString())
                Next

            End Using
        Next
        ' Connection.Close()
        ' MI_Connection.Close()
    End Sub

    Public Overrides Sub Import(DirectoryPath As String, model As String)

    End Sub
End Class
