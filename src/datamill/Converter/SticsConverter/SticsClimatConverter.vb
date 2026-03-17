Imports System
Imports System.IO
Imports System.Text
Imports System.Globalization
Imports System.Configuration
Imports System.Data
Imports System.Data.Sqlite

Public Class SticsClimatConverter
    
    Inherits Converter
    
    Public Overrides Sub Export(DirectoryPath As String)
    End Sub

    Public Overrides Sub Export(
            DirectoryPath As String,
            MasterInput_Connection As SqliteConnection,
            ModelDictionary_Connection As SqliteConnection)

        Dim tools As Tools = new Tools
        Dim fileName As String = "climat.txt"
        Dim fileContent As StringBuilder = New StringBuilder()
        ' Dim i As Integer
        Dim Dv As String
        Dim ST(10) As String
        Dim Site, Year As String
        ST = DirectoryPath.Split(Path.DirectorySeparatorChar)
        DirectoryPath = ST(0) & Path.DirectorySeparatorChar & ST(1) & Path.DirectorySeparatorChar & ST(2) & Path.DirectorySeparatorChar & ST(3) & Path.DirectorySeparatorChar & ST(4) & Path.DirectorySeparatorChar & ST(5) & Path.DirectorySeparatorChar & ST(6) & Path.DirectorySeparatorChar & ST(7)
        Site = ST(8)
        Year = ST(9)
        'ST = Year.Split(".")
        'Year = ST(1)
        Dim T As String = "Select   Champ, Default_Value_Datamill, defaultValueOtherSource, IFNULL([defaultValueOtherSource],  [Default_Value_Datamill]) As dv From Variables Where ((model = 'stics') And ([Table]= 'st_climat'));"
        Dim DT As New DataSet()

        Dim rw() As DataRow
        Dim Cmd As New SqliteDataAdapter(T, ModelDictionary_Connection)
        Cmd.Fill(DT, "TChamp")
        'Climat query
        Dim fetchAllQuery As String
        Dim dataSet As New DataSet()
        Dim dataTable As New DataTable()
        Dim jour As String
        Dim mois As String
        Dim jjulien As String
        Dim mintemp As String
        Dim maxtemp As String
        Dim gradiation As String
        Dim ppet As String
        Dim precipitation As String
        Dim vent As String
        'For i = 0 To 1
        fetchAllQuery = "select * from RaClimateD where idPoint='" + Site + "' And (Year=" & Year & " or Year=" & Year + 1 & ");"
        fileContent.Clear()
        'Init and use DataAdapter
        Using dataAdapter As SqliteDataAdapter = New SqliteDataAdapter(fetchAllQuery, MasterInput_Connection)
                ' Filling Dataset
                dataSet = New DataSet
                dataSet.Clear()
                dataTable.Clear()
                dataAdapter.Fill(dataSet, "st_climat")
                dataTable = dataSet.Tables("st_climat")
                'read all line of Climat
                For Each row In dataTable.Rows
                    fileContent.Append(row.item("IdPoint"))
                    fileContent.Append(Chr(32))
                    fileContent.Append(row.item("year"))
                    'Mois
                    mois = row.item("Nmonth")
                    fileContent.Append(mois.PadLeft(3))
                    'jour
                    jour = row.item("NDayM")
                    fileContent.Append(jour.PadLeft(3))
                    'jour julien
                    jjulien = (DateDiff(DateInterval.Day, CDate("01/01/" & row.item("year")), CDate(jour & "/" & mois & "/" & row.item("year"))) + 1)
                    fileContent.Append(jjulien.PadLeft(4))
                    'minTemp
                    mintemp = row.item("tmin")
                    fileContent.Append(FormatNumber(mintemp, 1).PadLeft(7))
                    'maxTemp
                    maxtemp = row.item("tmax")
                    fileContent.Append(FormatNumber(maxtemp, 1).PadLeft(7))
                    'gradiation
                    gradiation = row.item("srad")
                    fileContent.Append(FormatNumber(gradiation, 3).PadLeft(7))
                    'ppet
                    'sline = sline & " " & FormatNumber(CDbl(Mid(L1, 35)) - 273.15, 3,,,).PadLeft(7, " ")
                    ' ppet = row.item("EtpPM")
                    If NOT IsDbNull(row.item("EtpPM")) Then
                        ppet = row.item("EtpPM")
                    Else
                        ppet = tools.computeOneEtp(MasterInput_Connection, row)
                    End If
                    fileContent.Append(FormatNumber(ppet, 3,,,).PadLeft(7))
                    'precipitation
                    precipitation = row.item("rain")
                    fileContent.Append(FormatNumber(precipitation, 1).PadLeft(7))
                    'vent
                    vent = row.item("wind")
                    fileContent.Append(FormatNumber(vent, 3,,,).PadLeft(7))
                    'vapeurp
                    rw = DT.Tables(0).Select("Champ='vapeurp'")
                    Dv = rw(0)("dv").ToString
                    fileContent.Append(Dv.PadLeft(7))
                    'co2
                    rw = DT.Tables(0).Select("Champ='co2'")
                    Dv = rw(0)("dv").ToString
                    fileContent.Append(Dv.PadLeft(7))
                    fileContent.AppendLine()
                Next

            End Using

        Try
            ' Export file to specified directory
            WriteFile(DirectoryPath, fileName, fileContent.ToString())
        Catch ex As Exception
            MessageBox.Show("Error during writing file : " + ex.Message)
        End Try
    End Sub

    Public Overrides Sub Import(DirectoryPath As String, model As String)
    End Sub

End Class


