Imports System
Imports System.IO
Imports System.Text
Imports System.Globalization
Imports System.Configuration
Imports System.Data
Imports System.Data.Sqlite

Public Class DssatCultivarConverter
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

        Dim idSoil As String
        ' Dim ST(10) As String
        ' Dim Site, Year As String
        ' ST = DirectoryPath.Split("\")
        ' DirectoryPath = ST(0) & "\" & ST(1) & "\" & ST(2) & "\" & ST(3) & "\" & ST(4) & "\" & ST(5) & "\" & ST(6) & "\" & ST(7)
        ' Site = ST(8)
        ' Year = ST(7)
        ' ST = Year.Split(".")
        ' Year = ST(1)
        Dim ST(10) As String
        Dim Site, Year As String
        ST = DirectoryPath.Split(Path.DirectorySeparatorChar)
        DirectoryPath = ST(0) & Path.DirectorySeparatorChar & ST(1) & Path.DirectorySeparatorChar & ST(2) & Path.DirectorySeparatorChar & ST(3) & Path.DirectorySeparatorChar & ST(4) & Path.DirectorySeparatorChar & ST(5) & Path.DirectorySeparatorChar & ST(6) & Path.DirectorySeparatorChar & ST(7)
        Site = ST(8)
        Year = ST(7)
        'ST = Year.Split(".")
        'Year = ST(1)        
        'weather_site query
        Dim fileContent As StringBuilder = New StringBuilder()
        Dim dssat_queryRead As String = "Select Champ, Default_Value_Datamill, defaultValueOtherSource, IFNULL([defaultValueOtherSource],[Default_Value_Datamill]) As dv From Variables Where ((model = 'dssat') And ([Table] = 'dssat_cultivar_site'));"
        Dim DT As New DataSet()
        Dim rw As DataRow()
        Dim Dv As String
        Dim fileName As String = ""
        Dim dataAdapter1 As SqliteDataAdapter = New SqliteDataAdapter(dssat_queryRead, ModelDictionary_Connection)
        Using dataAdapter1
            dataAdapter1.Fill(DT)
            ' Filling Dataset

            'Dim idData As Integer

            'read all line of dssat_weather_site
            'For Each row In dataTable.Rows


            'fileContent.Append(FileHeader) ' Write File header
            rw = DT.Tables(0).Select("Champ='filename'")
            Dv = rw(0)("dv").ToString
            'fileContent.Append(Dv)
            fileName = Dv
            'rw = DT.Tables(0).Select("Champ='LNSA'")
            'Dv = rw(0)("dv").ToString
            'fileContent.Append(Dv.PadLeft(5))
            'idData = row.item("id")

            'Dim siteColumnsHeader() As String = {"@", "INSI", "LAT", " LONG", "ELEV", "TAV", "AMP", "REFHT", "WNDHT"}
            Dim siteColumnsHeader1 As String = "!"
            Dim siteColumnsHeader2 As String = "! COEFF       DEFINITIONS"
            Dim siteColumnsHeader3 As String = "! ========    ==========="
            Dim siteColumnsHeader4 As String = "! VAR#        Identification code or number for a specific cultivar."
            Dim siteColumnsHeader5 As String = "! VAR-NAME    Name of cultivar"
            Dim siteColumnsHeader6 As String = "! EXPNO       Number of experiments used to estimate cultivar parameters"
            Dim siteColumnsHeader7 As String = "! ECO#        Ecotype code for this cultivar, points to the Ecotype in"
            Dim siteColumnsHeader8 As String = "!             the ecotype file"
            Dim siteColumnsHeader9 As String = "! P1          Thermal time from seedling emergence to the end of the"
            Dim siteColumnsHeader10 As String = "!             juvenile phase (expressed in degree days above TBASE"
            Dim siteColumnsHeader11 As String = "!             during which the plant is not responsive to changes"
            Dim siteColumnsHeader12 As String = "!             in photoperiod"
            Dim siteColumnsHeader13 As String = "! P2          Thermal time from the end of the juvenile stage to tassel initiation"
            Dim siteColumnsHeader14 As String = "!             under short days (degree days above TBASE)"
            Dim siteColumnsHeader15 As String = "! P2O         Critical photoperiod or the longest day length (in hours) at"
            Dim siteColumnsHeader16 As String = "!             which development occurs at a maximum rate. At values higher"
            Dim siteColumnsHeader17 As String = "!             than P2O, the rate of development is reduced"
            Dim siteColumnsHeader18 As String = "! P2R         Extent to which phasic development leading to panicle"
            Dim siteColumnsHeader19 As String = "!             initiation (expressed in degree days) is delayed for each hour"
            Dim siteColumnsHeader20 As String = "!             increase in photoperiod above P2O"
            Dim siteColumnsHeader21 As String = "! PANTH       Thermal time from the end of tassel initiation to anthesis (degree days"
            Dim siteColumnsHeader22 As String = "!             above TBASE)"
            Dim siteColumnsHeader23 As String = "! P3          Thermal time from to end of flag leaf expansion to anthesis (degree days"
            Dim siteColumnsHeader24 As String = "!             above TBASE)"
            Dim siteColumnsHeader25 As String = "! P4          Thermal time from anthesis to beginning grain filling (degree "
            Dim siteColumnsHeader26 As String = "!             days above TBASE)"
            Dim siteColumnsHeader27 As String = "! P5          Thermal time from beginning of grain filling to physiological"
            Dim siteColumnsHeader28 As String = "!             maturity (degree days above TBASE)"
            Dim siteColumnsHeader29 As String = "! PHINT       Phylochron interval; the interval in thermal time between"
            Dim siteColumnsHeader30 As String = "!             successive leaf tip appearances (degree days)"
            Dim siteColumnsHeader31 As String = "! G1          Scaler for relative leaf size"
            Dim siteColumnsHeader32 As String = "! G2          Scaler for partitioning of assimilates to the panicle (head)."
            Dim siteColumnsHeader33 As String = "! PSAT        Critical photoperiod below which development is not delayed (optional)"
            Dim siteColumnsHeader34 As String = "! PBASE       Ceiling photoperiod above which development is delayed indefinitely (optional)"
            Dim siteColumnsHeader35 As String = "!                                                                                                     |-optional-|   "
            Dim siteColumnsHeader36 As String = "@VAR#  VAR-NAME........ EXPNO   ECO#    P1    P2   P2O   P2R PANTH    P3    P4    P5 PHINT    G1    G2 PBASE  PSAT"
            Dim siteColumnsHeader37 As String = "!                                  1     2     3     4     5     6     7     8     9    10    11    12    13    14"
            Dim siteColumnsHeader38 As String = "!IB0051 CSM335               .IB0002 380.0 102.0 12.60 450.0 617.5 202.5  61.5 540.0 55.00   4.5   1.0 Alternative"

            rw = DT.Tables(0).Select("Champ='header_cultivar_data'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv)
            'fileContent.Append(row.item("header_cultivar_data"))
            fileContent.AppendLine() ' Append a line break.

            fileContent.Append(siteColumnsHeader1)
            fileContent.AppendLine()
            fileContent.Append(siteColumnsHeader2)
            fileContent.AppendLine()
            fileContent.Append(siteColumnsHeader3)
            fileContent.AppendLine()
            fileContent.Append(siteColumnsHeader4)
            fileContent.AppendLine()
            fileContent.Append(siteColumnsHeader5)
            fileContent.AppendLine()
            fileContent.Append(siteColumnsHeader6)
            fileContent.AppendLine()
            fileContent.Append(siteColumnsHeader7)
            fileContent.AppendLine()
            fileContent.Append(siteColumnsHeader8)
            fileContent.AppendLine()
            fileContent.Append(siteColumnsHeader9)
            fileContent.AppendLine()
            fileContent.Append(siteColumnsHeader10)
            fileContent.AppendLine()
            fileContent.Append(siteColumnsHeader11)
            fileContent.AppendLine()
            fileContent.Append(siteColumnsHeader12)
            fileContent.AppendLine()
            fileContent.Append(siteColumnsHeader13)
            fileContent.AppendLine()
            fileContent.Append(siteColumnsHeader14)
            fileContent.AppendLine()
            fileContent.Append(siteColumnsHeader15)
            fileContent.AppendLine()
            fileContent.Append(siteColumnsHeader16)
            fileContent.AppendLine()
            fileContent.Append(siteColumnsHeader17)
            fileContent.AppendLine()
            fileContent.Append(siteColumnsHeader18)
            fileContent.AppendLine()
            fileContent.Append(siteColumnsHeader19)
            fileContent.AppendLine()
            fileContent.Append(siteColumnsHeader20)
            fileContent.AppendLine()
            fileContent.Append(siteColumnsHeader21)
            fileContent.AppendLine()
            fileContent.Append(siteColumnsHeader22)
            fileContent.AppendLine()
            fileContent.Append(siteColumnsHeader23)
            fileContent.AppendLine()
            fileContent.Append(siteColumnsHeader24)
            fileContent.AppendLine()
            fileContent.Append(siteColumnsHeader25)
            fileContent.AppendLine()
            fileContent.Append(siteColumnsHeader26)
            fileContent.AppendLine()
            fileContent.Append(siteColumnsHeader27)
            fileContent.AppendLine()
            fileContent.Append(siteColumnsHeader28)
            fileContent.AppendLine()
            fileContent.Append(siteColumnsHeader29)
            fileContent.AppendLine()
            fileContent.Append(siteColumnsHeader30)
            fileContent.AppendLine()
            fileContent.Append(siteColumnsHeader31)
            fileContent.AppendLine()
            fileContent.Append(siteColumnsHeader32)
            fileContent.AppendLine()
            fileContent.Append(siteColumnsHeader33)
            fileContent.AppendLine()
            fileContent.Append(siteColumnsHeader34)
            fileContent.AppendLine()
            fileContent.Append(siteColumnsHeader35)
            fileContent.AppendLine()
            fileContent.Append(siteColumnsHeader36)
            fileContent.AppendLine()
            fileContent.Append(siteColumnsHeader37)
            fileContent.AppendLine()
            fileContent.Append(siteColumnsHeader38)
            fileContent.AppendLine()
            fileContent.AppendLine()
        End Using
        'Init and use DataAdapter
        'Dim fetchAllQuery1 As String = "select * from dssat_cultivar_data where dssat_cultivar_site_id = " + idData.ToString + " ;"

        dssat_queryRead = "Select Champ, Default_Value_Datamill, defaultValueOtherSource, IFNULL([defaultValueOtherSource],[Default_Value_Datamill]) As dv From Variables Where ((model = 'dssat') And ([Table] = 'dssat_cultivar_data'));"
        dataAdapter1 = New SqliteDataAdapter(dssat_queryRead, ModelDictionary_Connection)
        Using dataAdapter1

                'Dim DT As New DataSet()
                'Dim rw AsEnd Using DataRow()
                'Dim Dv As String
                dataAdapter1.Fill(DT)
            ''read all line of dssat_cultivar_site
            'For Each occurence As DataRow In dataTable1.Rows
            fileContent.AppendLine() ' Append a line break.
            rw = DT.Tables(0).Select("Champ='var'")
                    Dv = rw(0)("dv").ToString
                    fileContent.Append(Dv)
                    'fileContent.Append(formatItem(occurence.Item("var")))
                    fileContent.Append(" ")
                    rw = DT.Tables(0).Select("Champ='var_name'")
                    Dv = rw(0)("dv").ToString
                    fileContent.Append(Dv.PadLeft(21))
                    'fileContent.Append(formatItem_Lg(occurence.Item("var_name"), 21))
                    fileContent.Append(" ")
                    rw = DT.Tables(0).Select("Champ='expno'")
                    Dv = rw(0)("dv").ToString
                    fileContent.Append(Dv)
                    'fileContent.Append(convertZeroToDot(occurence.Item("expno")))
                    'fileContent.Append(" ")
                    rw = DT.Tables(0).Select("Champ='eco'")
                    Dv = rw(0)("dv").ToString
                    fileContent.Append(Dv)
                    'fileContent.Append(formatItem(occurence.Item("eco")))
                    fileContent.Append(" ")
                    rw = DT.Tables(0).Select("Champ='p1'")
                    Dv = rw(0)("dv").ToString
                    fileContent.Append(Dv.PadLeft(5))
                    'fileContent.Append(formatItem_Lg(occurence.Item("p1"), 5))
                    fileContent.Append(" ")
                    rw = DT.Tables(0).Select("Champ='p2'")
                    Dv = rw(0)("dv").ToString
                    fileContent.Append(Dv.PadLeft(5))
                    'fileContent.Append(formatItem_Lg(occurence.Item("p2"), 5))
                    fileContent.Append(" ")
                    rw = DT.Tables(0).Select("Champ='p2o'")
                    Dv = rw(0)("dv").ToString
                    fileContent.Append(Dv.PadLeft(5))
                    'fileContent.Append(formatItem_Lg(occurence.Item("p2o"), 5))
                    fileContent.Append(" ")
                    rw = DT.Tables(0).Select("Champ='p2r'")
                    Dv = rw(0)("dv").ToString
                    fileContent.Append(Dv.PadLeft(5))
                    'fileContent.Append(formatItem_Lg(occurence.Item("p2r"), 5))
                    fileContent.Append(" ")
                    rw = DT.Tables(0).Select("Champ='panth'")
                    Dv = rw(0)("dv").ToString
                    fileContent.Append(Dv.PadLeft(5))
                    'fileContent.Append(formatItem_Lg(occurence.Item("panth"), 5))
                    fileContent.Append(" ")
                    rw = DT.Tables(0).Select("Champ='p3'")
                    Dv = rw(0)("dv").ToString
                    fileContent.Append(Dv.PadLeft(5))
                    'fileContent.Append(formatItem_Lg(occurence.Item("p3"), 5))
                    fileContent.Append(" ")
                    rw = DT.Tables(0).Select("Champ='p4'")
                    Dv = rw(0)("dv").ToString
                    fileContent.Append(Dv.PadLeft(5))
                    'fileContent.Append(formatItem_Lg(occurence.Item("p4"), 5))
                    fileContent.Append(" ")
                    rw = DT.Tables(0).Select("Champ='p5'")
                    Dv = rw(0)("dv").ToString
                    fileContent.Append(Dv.PadLeft(5))
                    'fileContent.Append(formatItem_Lg(occurence.Item("p5"), 5))
                    fileContent.Append(" ")
                    rw = DT.Tables(0).Select("Champ='phint'")
                    Dv = rw(0)("dv").ToString
                    fileContent.Append(Dv.PadLeft(5))
                    'fileContent.Append(formatItem_Lg(occurence.Item("phint"), 5))
                    fileContent.Append(" ")
                    rw = DT.Tables(0).Select("Champ='g1'")
                    Dv = rw(0)("dv").ToString
                    fileContent.Append(Dv.PadLeft(5))
                    'fileContent.Append(formatItem_Lg(occurence.Item("g1"), 5))
                    fileContent.Append(" ")
                    rw = DT.Tables(0).Select("Champ='g2'")
                    Dv = rw(0)("dv").ToString
                    fileContent.Append(Dv.PadLeft(5))
                    'fileContent.Append(formatItem_Lg(occurence.Item("g2"), 5))
                    fileContent.Append(" ")
                    rw = DT.Tables(0).Select("Champ='psat'")
                    Dv = rw(0)("dv").ToString
                    fileContent.Append(Dv.PadLeft(5))
                    'fileContent.Append(formatItem_Lg(occurence.Item("psat"), 5))
                    fileContent.Append(" ")
                    rw = DT.Tables(0).Select("Champ='pbase'")
                    Dv = rw(0)("dv").ToString
                    fileContent.Append(Dv.PadLeft(5))
                    'fileContent.Append(formatItem_Lg(occurence.Item("pbase"), 5))
                    'Next
                End Using

        Try
            ' Export file to specified directory
            WriteFile(DirectoryPath, fileName, fileContent.ToString())
            fileContent.Clear()
        Catch ex As Exception
            MessageBox.Show("Error during writing file")
        End Try

        'Next

        'End Using

        'Init and use DataAdapter
        dssat_queryRead = "Select Champ, Default_Value_Datamill, defaultValueOtherSource, IFNULL([defaultValueOtherSource],[Default_Value_Datamill]) As dv From Variables Where ((model = 'dssat') And ([Table] = 'dssat_cultivar_site'));"
        dataAdapter1 = New SqliteDataAdapter(dssat_queryRead, ModelDictionary_Connection)
        Using dataAdapter1

            'Dim DT As New DataSet()
            'Dim rw As DataRow()
            'Dim Dv As String
            dataAdapter1.Fill(DT)
        fileName = ""
        'Dim idData As Integer

        'read all line of dssat_weather_site
        'For Each row In dataTable.Rows

        'Dim fileContent As StringBuilder = New StringBuilder()
        'fileContent.Append(FileHeader) ' Write File header
        rw = DT.Tables(0).Select("Champ='filename'")
            Dv = rw(0)("dv").ToString
            'fileContent.Append(Dv.PadLeft(5))
            fileName = Dv 'row.item("filename")
            fileName = Mid(fileName, 1, fileName.Length - 3) & "ECO"
            '    idData = row.item("id")

            'Dim siteColumnsHeader() As String = {"@", "INSI", "LAT", " LONG", "ELEV", "TAV", "AMP", "REFHT", "WNDHT"}
            Dim siteColumnsHeader1 As String = "!"
            Dim siteColumnsHeader2 As String = "! COEFF       DEFINITIONS"
            Dim siteColumnsHeader3 As String = "! ========    ==========="
            Dim siteColumnsHeader4 As String = "! ECO#        Code for the ecotype to which a cultivar belongs (see *.cul file)"
            Dim siteColumnsHeader5 As String = "! ECONAME     Name of the ecotype, which is referenced from the cultivar file"
            Dim siteColumnsHeader6 As String = "! TBASE   Base temperature below which no development occurs (oC)"
            Dim siteColumnsHeader7 As String = "! TOPT    Temperature at which maximum development occurs for vegetative stages (oC)"
            Dim siteColumnsHeader8 As String = "! ROPT    Temperature at which maximum development occurs for reproductive stages (oC)"
            Dim siteColumnsHeader9 As String = "! GDDE    Growing degree days per cm seed depth required for emergence (degree days/cm)"
            Dim siteColumnsHeader10 As String = "! RUE     Radiation use efficiency (g plant dry matter/MJ PAR)"
            Dim siteColumnsHeader11 As String = "! KCAN    Canopy light extinction coefficient for daily PAR"
            Dim siteColumnsHeader12 As String = "! STPC    Partitioning to stem growth as a fraction of potential leaf growth"
            Dim siteColumnsHeader13 As String = "! RTPC    Partitioning to root growth as a fraction of available carbohydrates"
            Dim siteColumnsHeader14 As String = "! TILFC   Tillering factor (0.0 no tillering; 1.0 full tillering)"
            Dim siteColumnsHeader15 As String = "! PLAM    Plant leaf area maximun (Initial leaf area)"
            Dim siteColumnsHeader16 As String = "!"
            Dim siteColumnsHeader17 As String = "@ECO#  ECONAME.........  TBASE  TOPT  ROPT  GDDE   RUE  KCAN  STPC  RTPC TILFC  PLAM"
            Dim siteColumnsHeader18 As String = "!                            1     2     3     4     5     6     7     8     9    10"

            rw = DT.Tables(0).Select("Champ='header_cultivar_data'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv)
            'fileContent.Append(row.item("header_cultivar_data"))
            fileContent.AppendLine() ' Append a line break.

            fileContent.Append(siteColumnsHeader1)
            fileContent.AppendLine()
            fileContent.Append(siteColumnsHeader2)
            fileContent.AppendLine()
            fileContent.Append(siteColumnsHeader3)
            fileContent.AppendLine()
            fileContent.Append(siteColumnsHeader4)
            fileContent.AppendLine()
            fileContent.Append(siteColumnsHeader5)
            fileContent.AppendLine()
            fileContent.Append(siteColumnsHeader6)
            fileContent.AppendLine()
            fileContent.Append(siteColumnsHeader7)
            fileContent.AppendLine()
            fileContent.Append(siteColumnsHeader8)
            fileContent.AppendLine()
            fileContent.Append(siteColumnsHeader9)
            fileContent.AppendLine()
            fileContent.Append(siteColumnsHeader10)
            fileContent.AppendLine()
            fileContent.Append(siteColumnsHeader11)
            fileContent.AppendLine()
            fileContent.Append(siteColumnsHeader12)
            fileContent.AppendLine()
            fileContent.Append(siteColumnsHeader13)
            fileContent.AppendLine()
            fileContent.Append(siteColumnsHeader14)
            fileContent.AppendLine()
            fileContent.Append(siteColumnsHeader15)
            fileContent.AppendLine()
            fileContent.Append(siteColumnsHeader16)
            fileContent.AppendLine()
            fileContent.Append(siteColumnsHeader17)
            fileContent.AppendLine()
            fileContent.Append(siteColumnsHeader18)
            fileContent.AppendLine()
        End Using
        'Init and use DataAdapter

        dssat_queryRead = "Select Champ, Default_Value_Datamill, defaultValueOtherSource, IFNULL([defaultValueOtherSource],[Default_Value_Datamill]) As dv From Variables Where ((model = 'dssat') And ([Table] = 'dssat_cultivar_eco'));"
            dataAdapter1 = New SqliteDataAdapter(dssat_queryRead, ModelDictionary_Connection)
            Using dataAdapter1
                'Dim DT As New DataSet()
                'Dim rw As DataRow()
                'Dim Dv As String
                DT.Clear()
                dataAdapter1.Fill(DT)
                ''read all line of dssat_cultivar_site
                'For Each occurence As DataRow In dataTable1.Rows
                fileContent.AppendLine() ' Append a line break.
                rw = DT.Tables(0).Select("Champ='ecov'")
                Dv = rw(0)("dv").ToString
                fileContent.Append(Dv)
                'fileContent.Append(formatItem(occurence.Item("ecov")))
                fileContent.Append(" ")
                rw = DT.Tables(0).Select("Champ='eco_name'")
                Dv = rw(0)("dv").ToString
                fileContent.Append(Dv.PadLeft(18))
            'fileContent.Append(formatItem_Lg(occurence.Item("eco_name"), 18))
            'fileContent.Append(" ")
            rw = DT.Tables(0).Select("Champ='tbase'")
                Dv = rw(0)("dv").ToString
                fileContent.Append(Dv.PadLeft(5))
                'fileContent.Append(formatItem_Lg(occurence.Item("tbase"), 5))
                fileContent.Append(" ")
                rw = DT.Tables(0).Select("Champ='topt'")
                Dv = rw(0)("dv").ToString
                fileContent.Append(Dv.PadLeft(5))
                'fileContent.Append(formatItem_Lg(occurence.Item("topt"), 5))
                fileContent.Append(" ")
                rw = DT.Tables(0).Select("Champ='ropt'")
                Dv = rw(0)("dv").ToString
                fileContent.Append(Dv.PadLeft(5))
                'fileContent.Append(formatItem_Lg(occurence.Item("ropt"), 5))
                fileContent.Append(" ")
                rw = DT.Tables(0).Select("Champ='gdde'")
                Dv = rw(0)("dv").ToString
                fileContent.Append(Dv.PadLeft(5))
                'fileContent.Append(formatItem_Lg(occurence.Item("gdde"), 5))
                fileContent.Append(" ")
                rw = DT.Tables(0).Select("Champ='rue'")
                Dv = rw(0)("dv").ToString
                fileContent.Append(Dv.PadLeft(5))
                'fileContent.Append(formatItem_Lg(occurence.Item("rue"), 5))
                fileContent.Append(" ")
                rw = DT.Tables(0).Select("Champ='kcan'")
                Dv = rw(0)("dv").ToString
                fileContent.Append(Dv.PadLeft(5))
                'fileContent.Append(formatItem_Lg(occurence.Item("kcan"), 5))
                fileContent.Append(" ")
                rw = DT.Tables(0).Select("Champ='stpc'")
                Dv = rw(0)("dv").ToString
                fileContent.Append(Dv.PadLeft(5))
                'fileContent.Append(formatItem_Lg(occurence.Item("stpc"), 5))
                fileContent.Append(" ")
                rw = DT.Tables(0).Select("Champ='rtpc'")
                Dv = rw(0)("dv").ToString
                fileContent.Append(Dv.PadLeft(5))
                'fileContent.Append(formatItem_Lg(occurence.Item("rtpc"), 5))
                fileContent.Append(" ")
                rw = DT.Tables(0).Select("Champ='tilfc'")
                Dv = rw(0)("dv").ToString
                fileContent.Append(Dv.PadLeft(5))
                'fileContent.Append(formatItem_Lg(occurence.Item("tilfc"), 5))
                fileContent.Append(" ")
                rw = DT.Tables(0).Select("Champ='plam'")
                Dv = rw(0)("dv").ToString
                fileContent.Append(Dv.PadLeft(5))
                'fileContent.Append(formatItem_Lg(occurence.Item("plam"), 5))
                'Next
            End Using

            Try
                    ' Export file to specified directory
                    WriteFile(DirectoryPath, fileName, fileContent.ToString())
                Catch ex As Exception
                    MessageBox.Show("Error during writing file")
                End Try

        'Next

        'End Using
        ' Connection.Close()
        ' MI_Connection.Close()

    End Sub


    Public Overrides Sub Import(DirectoryPath As String, model As String)

    End Sub
End Class
