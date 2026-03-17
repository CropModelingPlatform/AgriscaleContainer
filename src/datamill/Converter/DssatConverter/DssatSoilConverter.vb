Imports System
Imports System.IO
Imports System.Text
Imports System.Globalization
Imports System.Configuration
Imports System.Data
Imports System.Data.Sqlite

Public Class DssatSoilConverter
    
    Inherits Converter

    Public Overrides Sub Export(DirectoryPath As String)
    End Sub

    Public Overrides Sub Export(
            DirectoryPath As String,
            MasterInput_Connection As SqliteConnection,
            ModelDictionary_Connection As SqliteConnection)

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

        Dim idSoil As String
        ' Dim ST(11) As String
        ' Dim Site, Year, Mngt As String
        ' ST = DirectoryPath.Split("\")
        ' DirectoryPath = ST(0) & "\" & ST(1) & "\" & ST(2) & "\" & ST(3) & "\" & ST(4) & "\" & ST(5) & "\" & ST(6) & "\" & ST(7)
        ' idSoil = ST(8)
        ' 'Year = ST(9)
        ' 'ST = Year.Split(".")
        ' Site = ST(9)
        ' Year = ST(10)
        Dim ST(11) As String
        Dim Site, Year, Mngt As String
        ST = DirectoryPath.Split(Path.DirectorySeparatorChar)
        DirectoryPath = ST(0) & Path.DirectorySeparatorChar & ST(1) & Path.DirectorySeparatorChar & ST(2) & Path.DirectorySeparatorChar & ST(3) & Path.DirectorySeparatorChar & ST(4) & Path.DirectorySeparatorChar & ST(5) & Path.DirectorySeparatorChar & ST(6) & Path.DirectorySeparatorChar & ST(7)
        idSoil = ST(8)
        Site = ST(9)
        Year = ST(10)
        Mngt = Mid(ST(11), 1, 4)
        Dim T As String = "Select Champ, Default_Value_Datamill, defaultValueOtherSource, IFNULL([defaultValueOtherSource],[Default_Value_Datamill]) As dv From Variables Where ((model = 'dssat') And ([Table] like 'dssat_soil_%'));"

        Dim DT As New DataSet()
        Dim Dv As String
        Dim rw() As DataRow
        Dim Cmd As SqliteDataAdapter = New SqliteDataAdapter(T, ModelDictionary_Connection)
        Cmd.Fill(DT, "TChamp")      'weather_site query
        'Dim fetchAllQuery As String = "select * from Coordinates where IdPoint='" & Site & "';"
        Dim fetchAllQuery As String = "SELECT DISTINCT Coordinates.*, RunoffTypes.CurveNumber, Soil.albedo " _
        & "From Coordinates INNER Join ((RunoffTypes INNER Join Soil On RunoffTypes.RunoffType = Soil.RunoffType) " _
        & "INNER Join SimUnitList On Soil.IdSoil = SimUnitList.idsoil) ON Coordinates.idPoint = SimUnitList.idPoint where SimUnitList.IdSim='" & ST(7) & "';"
        Dim fileContent As StringBuilder = New StringBuilder()
        'Init and use DataAdapter
        Dim fileName As String = ""
        Using dataAdapter As SqliteDataAdapter = New SqliteDataAdapter(fetchAllQuery, MasterInput_Connection)
            ' MsgBox(fetchAllQuery)
            ' Filling Dataset
            Dim dataSet As New DataSet()
            dataAdapter.Fill(dataSet, "dssat_soil_site")
            Dim dataTable As DataTable = dataSet.Tables("dssat_soil_site")
            'MsgBox(dataTable.Rows.Count)
            Dim I As Integer = 1


            'read all line of dssat_weather_site
            For Each row In dataTable.Rows
                'fileContent.Append(FileHeader) ' Write File header
                rw = DT.Tables(0).Select("Champ='filename'")
                Dv = rw(0)("dv").ToString
                'fileContent.Append(Dv.PadLeft(5))
                fileName = "XX.SOL"
                'idData = row.item("id")
                Dim siteColumnsHeader1 As String = "@SITE        COUNTRY          LAT     LONG SCS FAMILY"
                Dim siteColumnsHeader2 As String = "@ SCOM  SALB  SLU1  SLDR  SLRO  SLNF  SLPF  SMHB  SMPX  SMKE"
                'fileContent.AppendLine() ' Append a line break
                'fileContent.Append("*Soils: " & row.item("latitudeDD") & " " & row.item("LongitudeDD") & " ISRIC AfricaSoilGrid")
                fileContent.Append("*Soils: Mali")
                fileContent.AppendLine()
                fileContent.Append("*XX" & Mngt & "0101")
                fileContent.AppendLine()
                fileContent.Append(siteColumnsHeader1)
                fileContent.AppendLine()
                fileContent.Append(" ")
                fileContent.Append("XX" & Mngt & "0101   ") 'site
                'fileContent.Append(row.item("idpoint").padleft(13))
                fileContent.Append(" ")
                rw = DT.Tables(0).Select("Champ='Country'")
                Dv = rw(0)("dv").ToString
                fileContent.Append(Dv.PadLeft(10))
                'fileContent.Append(Chr(9))
                fileContent.Append(row.item("latitudeDD").ToString.PadLeft(8))
                'fileContent.Append(Chr(9))
                fileContent.Append(row.item("longitudeDD").ToString.PadLeft(8))
                fileContent.Append("  ")
                rw = DT.Tables(0).Select("Champ='scs family'")
                Dv = rw(0)("dv").ToString
                fileContent.Append(Dv.PadLeft(25))
                fileContent.AppendLine() ' Append a line break.
                fileContent.Append(siteColumnsHeader2)
                fileContent.AppendLine()
                fileContent.Append(" ")
                rw = DT.Tables(0).Select("Champ='scom'")
                Dv = rw(0)("dv").ToString
                fileContent.Append(Dv.PadLeft(5))
                'fileContent.Append(formatItem_Lg(row.item("scom"), 5))
                'fileContent.Append(Chr(9))
                'salb
                fileContent.Append(row.item("albedo").ToString.PadLeft(6))
                rw = DT.Tables(0).Select("Champ='slu1'")
                Dv = rw(0)("dv").ToString
                fileContent.Append(Dv.PadLeft(6))
                'fileContent.Append(formatItem_Lg(row.item("slu1"), 6))
                'fileContent.Append(Chr(9))
                rw = DT.Tables(0).Select("Champ='sldr'")
                Dv = rw(0)("dv").ToString
                fileContent.Append(Dv.PadLeft(6))
                'fileContent.Append(formatItem_Lg(row.item("sldr"), 6))
                'fileContent.Append(Chr(9))
                'slro
                fileContent.Append(row.item("Curvenumber").ToString.PadLeft(6))
                'fileContent.Append(formatItem_Lg(row.item("slro"), 6))
                'fileContent.Append(Chr(9))
                rw = DT.Tables(0).Select("Champ='slnf'")
                Dv = rw(0)("dv").ToString
                fileContent.Append(Dv.PadLeft(6))
                'fileContent.Append(formatItem_Lg(row.item("slnf"), 6))
                'fileContent.Append(Chr(9))
                rw = DT.Tables(0).Select("Champ='slpf'")
                Dv = rw(0)("dv").ToString
                fileContent.Append(Dv.PadLeft(6))
                'fileContent.Append(formatItem_Lg(row.item("slpf"), 6))
                fileContent.Append(" ")
                rw = DT.Tables(0).Select("Champ='smhb'")
                Dv = rw(0)("dv").ToString
                fileContent.Append(Dv.PadLeft(5))
                'fileContent.Append(formatItem_Lg(row.item("smhb"), 5))
                fileContent.Append(" ")
                rw = DT.Tables(0).Select("Champ='smpx'")
                Dv = rw(0)("dv").ToString
                fileContent.Append(Dv.PadLeft(5))
                'fileContent.Append(formatItem_Lg(row.item("smpx"), 5))
                fileContent.Append(" ")
                rw = DT.Tables(0).Select("Champ='smke'")
                Dv = rw(0)("dv").ToString
                fileContent.Append(Dv.PadLeft(5))
                'fileContent.Append(formatItem_Lg(row.item("smke"), 5))
                fileContent.AppendLine()

                'soildata
                Dim dataColumnsHeader As String = "@  SLB  SLMH  SLLL  SDUL  SSAT  SRGF  SSKS  SBDM  SLOC  SLCL  SLSI  SLCF  SLNI  SLHW  SLHB  SCEC  SADC"
                fileContent.Append(String.Join(Chr(9), dataColumnsHeader))
                fileContent.AppendLine() ' Append a line break.

                'Init and use DataAdapter
                'Dim fetchAllQuery1 As String = "SELECT Soil.*, SoilTypes.* FROM SoilTypes INNER JOIN Soil ON SoilTypes.SoilTextureType = Soil.SoilTextureType where Soil.idSoil = '" + idSoil + "' ;"
                'Dim fetchAllQuery1 As String = "Select Soil.*, SoilLayers.*, SoilTypes.* FROM(SoilTypes INNER JOIN Soil On SoilTypes.SoilTextureType = Soil.SoilTextureType) LEFT JOIN SoilLayers On Soil.IdSoil = SoilLayers.idsoil where Soil.idSoil = '" + idSoil + "' ;"                
                Dim fetchAllQuery1 As String = "Select Soil.Wwp AS 'Soil.Wwp', Soil.Wfc AS 'Soil.Wfc', Soil.bd AS 'Soil.bd', Soil.OrganicC AS 'Soil.OrganicC', Soil.Cf AS 'Soil.Cf', Soil.pH AS 'Soil.pH', Soil.SoilOption AS 'SoilOption', Soil.OrganicNStock AS 'OrganicNStock', Soil.SoilTotalDepth AS 'SoilTotalDepth', " _
                    & "SoilLayers.Wwp AS 'SoilLayers.Wwp', SoilLayers.Wfc AS 'SoilLayers.Wfc', SoilLayers.bd AS 'SoilLayers.bd', SoilLayers.OrganicC AS 'SoilLayers.OrganicC', SoilLayers.Clay AS 'SoilLayers.Clay', SoilLayers.Silt AS 'SoilLayers.Silt', SoilLayers.Cf AS 'SoilLayers.Cf', SoilLayers.pH AS 'SoilLayers.pH', SoilLayers.Ldown AS 'Ldown', SoilLayers.TotalN AS 'TotalN', " _
                    & "Soiltypes.Clay AS 'Soiltypes.Clay', SoilTypes.Silt AS 'SoilTypes.Silt' FROM SoilTypes INNER JOIN Soil On Lower(SoilTypes.SoilTextureType) = Lower(Soil.SoilTextureType) LEFT JOIN SoilLayers On lower(Soil.IdSoil) = lower(SoilLayers.idsoil) where lower(Soil.idSoil) = '" + LCase(idSoil) + "' ;"
                'Console.WriteLine("dssat soil converter l169 : " & fetchAllQuery1)

                ' MsgBox(fetchAllQuery1)
                Using dataAdapter1 As SqliteDataAdapter = New SqliteDataAdapter(fetchAllQuery1, MasterInput_Connection)
                    Dim dataSet1 As New DataSet()
                    Dim occurence As DataRow
                    dataAdapter1.Fill(dataSet1, "dssat_soil_data")
                    Dim dataTable1 As DataTable = dataSet1.Tables("dssat_soil_data")
                    If LCase(dataTable1.Rows(0).Item("soilOption")) = "simple" Then
                        ''read all line of dssat_weather_site
                        occurence = dataTable1.Rows(0)
                        For i = 0 To 1 ' Each occurence As DataRow In dataTable1.Rows
                            'slb
                            If i = 0 Then
                                fileContent.Append("30".PadLeft(6))
                            Else
                                fileContent.Append(occurence.Item("SoilTotalDepth").ToString.PadLeft(6))
                            End If
                            fileContent.Append(" ")
                            'slmh
                            rw = DT.Tables(0).Select("Champ='slmh'")
                            Dv = rw(0)("dv").ToString
                            fileContent.Append(Dv.PadLeft(5))
                            'slll
                            fileContent.Append(FormatNumber((occurence.Item("Soil.Wwp") / 100), 3).ToString.PadLeft(6))
                            'sdul
                            fileContent.Append(FormatNumber((occurence.Item("Soil.Wfc") / 100), 3).ToString.PadLeft(6))
                            'ssat
                            rw = DT.Tables(0).Select("Champ='ssat'")
                            Dv = rw(0)("dv").ToString
                            fileContent.Append(Dv.PadLeft(6))
                            'srgf
                            rw = DT.Tables(0).Select("Champ='srgf'")
                            Dv = rw(0)("dv").ToString
                            fileContent.Append(Dv.PadLeft(6))
                            'ssks
                            rw = DT.Tables(0).Select("Champ='ssks'")
                            Dv = rw(0)("dv").ToString
                            fileContent.Append(Dv.PadLeft(6))
                            'sbdm
                            fileContent.Append(FormatNumber(occurence.Item("Soil.bd"), 3).ToString.PadLeft(6))
                            'sloc
                            If i = 0 Then
                                fileContent.Append(FormatNumber(occurence.Item("Soil.OrganicC"), 3).ToString.PadLeft(6))
                            Else
                                fileContent.Append("0".PadLeft(6))
                            End If
                            'slcl
                            fileContent.Append(occurence.Item("Soiltypes.Clay").ToString.PadLeft(6))
                            'slsi
                            fileContent.Append(occurence.Item("SoilTypes.Silt").ToString.PadLeft(6))
                            'slcf
                            fileContent.Append(occurence.Item("Soil.Cf").ToString.PadLeft(6))
                            'slni
                            If i = 0 Then
                                fileContent.Append(occurence.Item("OrganicNStock").ToString.PadLeft(6))
                            Else
                                fileContent.Append("0".PadLeft(6))
                            End If
                            'slhw
                            fileContent.Append(occurence.Item("Soil.pH").ToString.PadLeft(6))
                            'slhb
                            rw = DT.Tables(0).Select("Champ='slhb'")
                            Dv = rw(0)("dv").ToString
                            fileContent.Append(Dv.PadLeft(6))
                            'scec
                            rw = DT.Tables(0).Select("Champ='scec'")
                            Dv = rw(0)("dv").ToString
                            fileContent.Append(Dv.PadLeft(6))
                            'sadc
                            rw = DT.Tables(0).Select("Champ='sadc'")
                            Dv = rw(0)("dv").ToString
                            fileContent.Append(Dv.PadLeft(6))

                            fileContent.AppendLine()
                        Next
                        '                        fileContent.AppendLine()
                    Else
                        For Each occurence1 As DataRow In dataTable1.Rows
                            'slb
                            fileContent.Append(occurence1.Item("Ldown").ToString.PadLeft(6))
                            fileContent.Append(" ")
                            'slmh
                            rw = DT.Tables(0).Select("Champ='slmh'")
                            Dv = rw(0)("dv").ToString
                            fileContent.Append(Dv.PadLeft(5))
                            'slll
                            fileContent.Append(FormatNumber((occurence1.Item("SoilLayers.Wwp") / 100), 3).ToString.PadLeft(6))
                            'sdul
                            fileContent.Append(FormatNumber((occurence1.Item("SoilLayers.Wfc") / 100), 3).ToString.PadLeft(6))
                            'ssat
                            rw = DT.Tables(0).Select("Champ='ssat'")
                            Dv = rw(0)("dv").ToString
                            fileContent.Append(Dv.PadLeft(6))
                            'srgf
                            rw = DT.Tables(0).Select("Champ='srgf'")
                            Dv = rw(0)("dv").ToString
                            fileContent.Append(Dv.PadLeft(6))
                            'ssks
                            rw = DT.Tables(0).Select("Champ='ssks'")
                            Dv = rw(0)("dv").ToString
                            fileContent.Append(Dv.PadLeft(6))
                            'sbdm
                            fileContent.Append(FormatNumber(occurence1.Item("SoilLayers.bd"), 3).ToString.PadLeft(6))
                            'sloc
                            fileContent.Append(FormatNumber(occurence1.Item("SoilLayers.OrganicC"), 3).ToString.PadLeft(6))
                            'slcl
                            fileContent.Append(occurence1.Item("SoilLayers.Clay").ToString.PadLeft(6))
                            'slsi
                            fileContent.Append(occurence1.Item("SoilLayers.Silt").ToString.PadLeft(6))
                            'slcf
                            fileContent.Append(occurence1.Item("SoilLayers.Cf").ToString.PadLeft(6))
                            'slni
                            fileContent.Append(occurence1.Item("TotalN").ToString.PadLeft(6))
                            'slhw
                            fileContent.Append(occurence1.Item("SoilLayers.pH").ToString.PadLeft(6))
                            'slhb
                            rw = DT.Tables(0).Select("Champ='slhb'")
                            Dv = rw(0)("dv").ToString
                            fileContent.Append(Dv.PadLeft(6))
                            'scec
                            rw = DT.Tables(0).Select("Champ='scec'")
                            Dv = rw(0)("dv").ToString
                            fileContent.Append(Dv.PadLeft(6))
                            'sadc
                            rw = DT.Tables(0).Select("Champ='sadc'")
                            Dv = rw(0)("dv").ToString
                            fileContent.Append(Dv.PadLeft(6))

                            fileContent.AppendLine()
                        Next
                    End If
                    fileContent.AppendLine()
                End Using
            Next
            Try
                ' Export file to specified directory
                WriteFile(DirectoryPath, fileName, fileContent.ToString())
            Catch ex As Exception
                MessageBox.Show("Error during writing file " & ex.Message)
            End Try
        End Using
        ' Connection.Close()
        ' MI_Connection.Close()
    End Sub

    Public Overrides Sub Import(DirectoryPath As String, model As String)

    End Sub
End Class
