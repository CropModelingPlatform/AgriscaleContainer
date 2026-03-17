Imports System.IO
'Imports OleDb = System.Data.OleDb
Imports System.Data
Imports System.Data.Sqlite
Imports System.Text
Imports System.Math

''' <summary>
''' Abstract class to handle Converters.
''' Must redefined import and export methods
''' </summary>
Public MustInherit Class Converter

    Protected usmString As String
    Protected usmId As String
    Protected nthreads As Integer
    Protected sstart As Integer
    Protected send As Integer
    Protected MasterInput_Connection As SqliteConnection
    Protected ModelDictionary_Connection As SqliteConnection

    ''' <summary>
    ''' Import from selected file to database.
    ''' Mostly depends of selected Model/SubTheme pair.
    ''' </summary>
    ''' <remarks>Actually excluded from current project scope.</remarks>
    Public MustOverride Sub Import(ByVal DirectoryPath As String, model As String)

    ''' <summary>
    ''' Export data from Access database to file.
    ''' Mostly depends of selected Model/SubTheme pair.
    ''' </summary>
    ''' <param name="DirectoryPath">Path where to write the new file</param>
    Public MustOverride Sub Export(ByVal DirectoryPath As String)

    ''' <summary>
    ''' Export data from Access database to file.
    ''' Mostly depends of selected Model/SubTheme pair.
    ''' </summary>
    ''' <param name="DirectoryPath">Path where to write the new file</param>
    ''' <param name="MasterInput_Connection">MasterInput SqlLite connexion</param>
    ''' <param name="ModelDictionary_Connection">ModelDictionary SqlLite connexion</param>
    Public MustOverride Sub Export(
                ByVal DirectoryPath As String,
                ByVal MasterInput_Connection As SqliteConnection,
                ByVal ModelDictionary_Connection As SqliteConnection)

    ''' <summary>
    ''' Write some data into a file, with a given name, from a specific directory
    ''' </summary>
    ''' <param name="DirectoryPath">Path where to write the new file</param>
    ''' <param name="Filename">Name of the file to write</param>
    ''' <remarks>Mostly used during export process.</remarks>
    Public Sub WriteFile(ByVal DirectoryPath As String, ByVal FileName As String, ByVal FileContent As String)
        If Not IO.Directory.Exists(DirectoryPath) Then IO.Directory.CreateDirectory(DirectoryPath)
        Using outfile As StreamWriter = New StreamWriter(DirectoryPath + Path.DirectorySeparatorChar + FileName, False)
            outfile.Write(FileContent)
        End Using
    End Sub

    ''' <summary>
    ''' public function : 
    ''' -convert data into string
    ''' -date access with time => date without time
    ''' </summary>
    ''' <param name="Item">object</param>
    ''' <returns>itemformated and usable</returns>
    ''' <remarks></remarks>
    Public Function formatItem(ByVal Item As Object) As String

        Dim errorFormatted As Boolean = False
        Dim itemFormatted As String

        If TypeOf (Item) Is DateTime Then
            Dim Datetime As DateTime = CDate(Item)
            itemFormatted = Datetime.ToString("dd/MM/yyyy")
        ElseIf TypeOf (Item) Is Single Then
            Dim s As Single = CType(Item, Single)
            Dim chaine As String
            chaine = s.ToString
            itemFormatted = Replace(chaine, ",", ".")

        ElseIf TypeOf (Item) Is Double Then
            Dim s As Double = CType(Item, Double)
            Dim chaine As String
            chaine = s.ToString
            itemFormatted = Replace(chaine, ",", ".")

        ElseIf TypeOf (Item) Is DBNull Then
            itemFormatted = Nothing
        Else
            itemFormatted = Item
        End If

        'Do While itemFormatted.Length < 6
        'itemFormatted = " " & itemFormatted
        'Loop
        Return itemFormatted

    End Function

    ''' <summary>
    ''' public function : 
    ''' -convert data into string with an determinated lenght (Lg_Zone)
    ''' -date access with time => date without time
    ''' </summary>
    ''' <param name="Item">object</param>
    ''' <returns>itemformated and usable</returns>
    ''' <remarks></remarks>
    Public Function formatItem_Lg(ByVal Item As Object, ByVal Lg_Zone As Single) As String

        Dim errorFormatted As Boolean = False
        Dim itemFormatted As String
        '    Dim itemFormatted_lg As String
        Dim lg As Single

        If TypeOf (Item) Is DateTime Then
            Dim Datetime As DateTime = CDate(Item)
            itemFormatted = Datetime.ToString("dd/MM/yyyy")
        ElseIf TypeOf (Item) Is Single Then
            Dim s As Single = CType(Item, Single)
            Dim chaine As String
            chaine = s.ToString
            itemFormatted = Replace(chaine, ",", ".")

        ElseIf TypeOf (Item) Is Double Then
            Dim s As Double = CType(Item, Double)
            Dim chaine As String
            chaine = s.ToString
            itemFormatted = Replace(chaine, ",", ".")

        ElseIf TypeOf (Item) Is DBNull Then
            itemFormatted = Nothing
        Else
            itemFormatted = Item
        End If
        If TypeOf (Item) Is DBNull Then
            lg = 0
        Else
            lg = itemFormatted.Length
        End If

        If TypeOf (Item) Is Single Then
            For i = 0 To (Lg_Zone - (lg + 1))
                itemFormatted = String.Concat(Chr(32), itemFormatted)
            Next i
        ElseIf TypeOf (Item) Is Double Then
            For i = 0 To (Lg_Zone - (lg + 1))
                itemFormatted = String.Concat(Chr(32), itemFormatted)
            Next i
        ElseIf TypeOf (Item) Is String Then
            For i = 0 To (Lg_Zone - (lg + 1))
                itemFormatted = String.Concat(itemFormatted, Chr(32))
            Next i

        End If

        Return itemFormatted

    End Function
    ''' <summary>
    ''' public function : verify date format stocked with YYDDD format
    ''' </summary>
    ''' <param name="ItemDate">String</param>
    ''' <returns>String field</returns>
    ''' <remarks></remarks>
    Public Function formatItemDate(ByVal ItemDate As String) As String

        Dim errorFormatted As Boolean = False
        Dim itemFormatted As String
        Dim année As String
        Dim quantième As String
        Dim longueur As Integer

        If ItemDate = "-99" Then
            Return ItemDate
        End If
        longueur = ItemDate.Length
        année = ItemDate.Substring(0, longueur - 3)
        quantième = ItemDate.Substring(longueur - 3)
        If année < 10 Then
            année = String.Concat("0", année.Substring(1))
        End If
        If année / 4 = 1 Then
            If quantième > 366 Then
                errorFormatted = True
            End If
        Else
            If quantième > 365 Then
                errorFormatted = True
            End If
        End If
        itemFormatted = String.Concat(année, quantième)
        Return itemFormatted

    End Function
    ''' <summary>
    ''' public function : 
    ''' -convert data into string
    ''' -date access with time => date without time
    ''' </summary>
    ''' <param name="Item">object</param>
    ''' <returns>itemformated and usable</returns>
    ''' <remarks></remarks>
    Public Function Right_Justified(ByVal Item As String) As String

        Dim itemFormatted As String
        Dim Array(Item.Length - 1) As Char
        Dim ArrayFormated(Item.Length - 1) As Char
        Dim Lg As Integer

        Array = Item

        For i = 0 To (Item.Length - 1)
            If Array(i) = Chr(32) Then
                Lg = i
                Exit For
            End If
        Next

        For i = Lg To (Item.Length - 1)
            ArrayFormated(i - Lg) = Array(i)
        Next

        For i = 0 To Lg - 1
            ArrayFormated(Item.Length - Lg + i) = Array(i)
        Next
        itemFormatted = ArrayFormated
        Return itemFormatted

    End Function
    ''' <summary>
    ''' public function : convert numeric field 0.0 to dysplay only .
    ''' </summary>
    ''' <param name="item">object</param>
    ''' <returns>numeric field containing 0 will be returned as "." </returns>
    ''' <remarks></remarks>

    Public Function convertZeroToDot(ByVal Item As Object) As String
        Dim itemFormatted As String

        If TypeOf (Item) Is DBNull Then
            itemFormatted = Nothing
        ElseIf TypeOf (Item) Is Single And Item = 0 Then
            Dim itemDot As String = "."
            itemFormatted = itemDot
        Else
            itemFormatted = Item
        End If

        Return itemFormatted

    End Function
    ''' <summary>
    ''' read file
    ''' </summary>
    ''' <param name="selectedFile"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function readFile(ByVal selectedFile As String) As String
        Try
            'file reading variable
            Dim sr As IO.StreamReader
            Dim Ligne As String
            Dim arrText As New ArrayList()
            '
            Dim aryTextFile() As String
            Dim i As Integer

            sr = New IO.StreamReader(selectedFile.ToString())
            Do
                'ind = ind + 1
                Ligne = sr.ReadLine
                If Not Ligne Is Nothing Then
                    arrText.Add(Ligne)
                    aryTextFile = Split(Ligne, Chr(9))

                    MessageBox.Show("contenu fichier :" & Ligne)
                    For i = 0 To UBound(aryTextFile)
                        MessageBox.Show(aryTextFile(i))
                    Next i
                End If
            Loop Until Ligne Is Nothing
            sr.Close()
        Catch ex As Exception
            MessageBox.Show("Veuillez sélectionner un fichier")
        End Try
        Return Nothing
    End Function

    ''' <summary>
    ''' function : read table dssat_x_treatment and write into fileContent
    ''' </summary>
    ''' <returns>fileContent</returns>
    ''' <remarks></remarks>

    Public Function writeBlockTreatment(ByVal dssat_tableName As String, ByVal dssat_tableId As String, ByVal dssat_tableId_value As String, ByVal fileContent As StringBuilder, ByVal Connection As SqliteConnection, ByVal MI_Connection As SqliteConnection) As StringBuilder
        'Dim dssat_queryRead As String = "select * from " + dssat_tableName + " where " + dssat_tableId + " = " + dssat_tableId_value + " ;"
        Dim fetchAllQuery As String = "Select SimUnitList.idsim, SoilTillPolicy.NumTillOperations, OrganicFertilizationPolicy.NumOrganicFerti " _
        & "From OrganicFertilizationPolicy INNER Join (SoilTillPolicy INNER Join (CropManagement INNER Join SimUnitList " _
        & "On CropManagement.idMangt = SimUnitList.idMangt) ON SoilTillPolicy.SoilTillPolicyCode = CropManagement.SoilTillPolicyCode) " _
        & "ON OrganicFertilizationPolicy.OFertiPolicyCode = CropManagement.OFertiPolicyCode Where IdSim='" & dssat_tableId & "'"
        'Init and use DataAdapter
        Dim dataAdapter As SqliteDataAdapter = New SqliteDataAdapter(fetchAllQuery, MI_Connection)
        ' Filling Dataset
        Dim dataSet As New DataSet()
        dataAdapter.Fill(dataSet, "dssat_x_exp")
        Dim dataTable As DataTable = dataSet.Tables("dssat_x_exp")
        
        Dim dssat_queryRead As String = "Select Champ, Default_Value_Datamill, defaultValueOtherSource, IFNULL([defaultValueOtherSource],  [Default_Value_Datamill]) As dv From Variables Where ((model = 'dssat') And ([Table] = '" & dssat_tableName & "'));"
        Using dataAdapter1 As SqliteDataAdapter = New SqliteDataAdapter(dssat_queryRead, Connection)
            Dim DT As New DataSet()
            Dim rw As DataRow()
            Dim Dv As String
            dataAdapter1.Fill(DT, dssat_tableName)
            'Dim DT As DataTable = dataSet1.Tables(dssat_tableName)
            Dim siteColumnsHeader() As String = {"@N", "R", "O", "C", "TNAME....................", "CU", "FL", "SA", "IC", "MP", "MI", "MF", "MR", "MC", "MT", "ME", "MH", "SM"}

            'header treatment
            fileContent.AppendLine() ' Append a line break.
            fileContent.Append("*TREATMENTS                        -------------FACTOR LEVELS------------")
            fileContent.AppendLine() ' Append a line break.
            'fileContent.Append("@N R O C TNAME.................... CU FL SA IC MP MI MF MR MC MT ME MH SM")
            fileContent.Append(String.Join(Chr(32), siteColumnsHeader))
            fileContent.AppendLine() ' Append a line break.

            'init storeNumMaxSimu
            storeNumMaxSimu = 0
            ''read all line of ddssat_x_treatment
            'For Each occurence As DataRow In dataTable1.Rows
            rw = DT.Tables(0).Select("Champ='TRTNO'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(2))
            'fileContent.Append(formatItem_Lg(occurence.Item("TRTNO"), 2))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='ROTNO'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(1))
            'fileContent.Append(formatItem_Lg(occurence.Item("ROTNO"), 1))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='ROTOPT'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(1))
            'fileContent.Append(formatItem_Lg(occurence.Item("ROTOPT"), 1))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='CRPNO'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(1))
            'fileContent.Append(formatItem_Lg(occurence.Item("CRPNO"), 1))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='TITLET'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(25))
            'fileContent.Append(formatItem_Lg(occurence.Item("TITLET"), 25))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='LNCU'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(2))
            'fileContent.Append(formatItem_Lg(occurence.Item("LNCU"), 2))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='LNFLD'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(2))
            'fileContent.Append(formatItem_Lg(occurence.Item("LNFLD"), 2))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='LNSA'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(2))
            'fileContent.Append(formatItem_Lg(occurence.Item("LNSA"), 2))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='LNIC'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(2))
            'fileContent.Append(formatItem_Lg(occurence.Item("LNIC"), 2))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='LNPLT'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(2))
            'fileContent.Append(formatItem_Lg(occurence.Item("LNPLT"), 2))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='LNIR'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(2))
            'fileContent.Append(formatItem_Lg(occurence.Item("LNIR"), 2))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='LNFER'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(2))
            'fileContent.Append(formatItem_Lg(occurence.Item("LNFER"), 2))
            fileContent.Append(Chr(32))
            'rw = DT.Tables(0).Select("Champ='LNRES'")
            'Dv = rw(0)("dv").ToString
            If dataTable.Rows(0).Item("NumOrganicFerti") = 0 Then
                fileContent.Append("0".PadLeft(2))
            Else
                fileContent.Append("1".PadLeft(2))
            End If
            'fileContent.Append(formatItem_Lg(occurence.Item("LNRES"), 2))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='LNCHE'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(2))
            'fileContent.Append(formatItem_Lg(occurence.Item("LNCHE"), 2))
            fileContent.Append(Chr(32))
            'rw = DT.Tables(0).Select("Champ='LNTIL'")
            'Dv = rw(0)("dv").ToString
            'fileContent.Append(Dv.PadLeft(2))
            If dataTable.Rows(0).Item("NumTillOperations") = 0 Then
                fileContent.Append("0".PadLeft(2))
            Else
                fileContent.Append("1".PadLeft(2))
            End If
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='LNENV'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(2))
            'fileContent.Append(formatItem_Lg(occurence.Item("LNENV"), 2))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='LNHAR'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(2))
            'fileContent.Append(formatItem_Lg(occurence.Item("LNHAR"), 2))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='LNSIM'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(2))
            'fileContent.Append(formatItem_Lg(occurence.Item("LNSIM"), 2))
            fileContent.Append(Chr(32))
                fileContent.AppendLine() ' Append a line break.

            'If storeNumMaxSimu <> formatItem(occurence.Item("LNSIM")) Then
            ' storeNumMaxSimu = formatItem(occurence.Item("LNSIM"))
            ' End If
            If storeNumMaxSimu <> Dv Then
                storeNumMaxSimu = Dv
            End If
            'Next
        End Using
        Return fileContent
    End Function
    ''' <summary>
    ''' function : read table dssat_x_treatment and write into fileContent
    ''' </summary>
    ''' <returns>fileContent</returns>
    ''' <remarks></remarks>

    Public Function writeBlockTreatment2(ByVal dssat_tableName As String, ByVal dssat_tableId As String, ByVal dssat_tableId_value As String, ByVal filex As String, ByVal fileContent As StringBuilder, ByVal Connection As SqliteConnection) As StringBuilder
        'Dim dssat_queryRead As String = "select * from " + dssat_tableName + " where " + dssat_tableId + " = " + dssat_tableId_value + " ;"

        Dim dssat_queryRead As String = "Select Champ, Default_Value_Datamill, defaultValueOtherSource, IFNULL([defaultValueOtherSource],  [Default_Value_Datamill]) As dv From Variables Where ((model = 'dssat') And ([Table] = '" & dssat_tableName & "'));"
        Using dataAdapter1 As SqliteDataAdapter = New SqliteDataAdapter(dssat_queryRead, Connection)
            Dim DT As New DataSet()
            Dim rw As DataRow()
            Dim Dv As String
            dataAdapter1.Fill(DT, dssat_tableName)
            Dim dataTable1 As DataTable = DT.Tables(dssat_tableName)
            Dim siteColumnsHeader() As String = {"@FILEX                                                                                        TRTNO     RP     SQ     OP     CO"}

            'header treatment
            fileContent.AppendLine() ' Append a line break.
            fileContent.Append("$BATCH(EXPERIMENT)")
            fileContent.AppendLine() ' Append a line break.
            'fileContent.Append("@N R O C TNAME.................... CU FL SA IC MP MI MF MR MC MT ME MH SM")
            fileContent.Append(String.Join(Chr(32), siteColumnsHeader))
            fileContent.AppendLine() ' Append a line break.

            'init storeNumMaxSimu
            storeNumMaxSimu = 0

            ''read all line of ddssat_x_treatment
            'For Each occurence As DataRow In dataTable1.Rows

            'rw = DT.Tables(0).Select("Champ='LNHAR'")
            'Dv = rw(0)("dv").ToString
            'fileContent.Append(Dv.PadLeft(2))
            fileContent.Append(dssat_tableId.PadRight(95))
            fileContent.Append(Chr(32))
                rw = DT.Tables(0).Select("Champ='TRTNO'")
                Dv = rw(0)("dv").ToString
                fileContent.Append(Dv.PadLeft(2))
                'fileContent.Append(formatItem_Lg(occurence.Item("TRTNO"), 2))
                fileContent.Append(Chr(32))
                rw = DT.Tables(0).Select("Champ='ROTNO'")
                Dv = rw(0)("dv").ToString
                fileContent.Append(Dv.PadLeft(1))
                'fileContent.Append(formatItem_Lg(occurence.Item("ROTNO"), 1))
                fileContent.Append(Chr(32))
                'rw = DT.Tables(0).Select("Champ='LNHAR'")
                'Dv = rw(0)("dv").ToString
                'fileContent.Append(Dv.PadLeft(2))
                fileContent.Append(formatItem_Lg("1", 1))
                fileContent.Append(Chr(32))
                rw = DT.Tables(0).Select("Champ='ROTOPT'")
                Dv = rw(0)("dv").ToString
                fileContent.Append(Dv.PadLeft(1))
                'fileContent.Append(formatItem_Lg(occurence.Item("ROTOPT"), 1))
                fileContent.Append(Chr(32))
                rw = DT.Tables(0).Select("Champ='CRPNO'")
                Dv = rw(0)("dv").ToString
                fileContent.Append(Dv.PadLeft(1))
                'fileContent.Append(formatItem_Lg(occurence.Item("CRPNO"), 1))
                fileContent.Append(Chr(32))
                fileContent.AppendLine() ' Append a line break.

            'If storeNumMaxSimu <> formatItem(occurence.Item("LNSIM")) Then
            ' storeNumMaxSimu = formatItem(occurence.Item("LNSIM"))
            'End If
            If storeNumMaxSimu <> 1 Then
                storeNumMaxSimu = 1
            End If
            'Next
        End Using
        Return fileContent
    End Function
    ''' <summary>
    ''' function : read table dssat_x_cultivar and write into fileContent
    ''' </summary>
    ''' <param name="dssat_tableName"></param>
    ''' <param name="dssat_tableId"></param>
    ''' <param name="dssat_tableId_value"></param>
    ''' <param name="fileContent"></param>
    ''' <param name="Connection"></param>
    ''' <returns>fileContent</returns>
    ''' <remarks></remarks>
    Public Function writeBlockCultivar(ByVal dssat_tableName As String, ByVal dssat_tableId As String, ByVal dssat_tableId_value As String, ByVal fileContent As StringBuilder, ByVal Connection As SqliteConnection, ByVal MI_Connection As SqliteConnection) As StringBuilder

        'Dim dssat_queryRead As String = "select * from " + dssat_tableName + " where " + dssat_tableId + " = " + dssat_tableId_value + " ;"
        Dim dssat_queryRead As String = "Select Champ, Default_Value_Datamill, defaultValueOtherSource, IFNULL([defaultValueOtherSource],  [Default_Value_Datamill]) As dv From Variables Where ((model = 'dssat') And ([Table] = '" & dssat_tableName & "'));"
        Using dataAdapter1 As SqliteDataAdapter = New SqliteDataAdapter(dssat_queryRead, Connection)
            Dim DT As New DataSet()
            Dim rw As DataRow()
            Dim Dv As String
            dataAdapter1.Fill(DT, dssat_tableName)
            Dim fetchAllQuery As String = "SELECT CropManagement.idMangt, ListCultivars.CodCultivar, ListCultivars.IdcultivarDssat, ListCultOption.CG From ListCultOption INNER JOIN (ListCultivars INNER Join CropManagement On ListCultivars.IdCultivar = CropManagement.Idcultivar) On ListCultOption.CodePSpecies = ListCultivars.CodePSpecies Where Idmangt ='" & dssat_tableId & "';"
            'Init and use DataAdapter
            Dim dataAdapter As SqliteDataAdapter = New SqliteDataAdapter(fetchAllQuery, MI_Connection)
            ' Filling Dataset
            Dim dataSet As New DataSet()
            dataAdapter.Fill(dataSet, "dssat_x_exp")
            Dim dataTable As DataTable = dataSet.Tables("dssat_x_exp")
            'Dim dataTable1 As DataTable = dataSet1.Tables(dssat_tableName)
            Dim siteColumnsHeader() As String = {"@C", "CR", "INGENO", "CNAME"}

            'header cultivar
            fileContent.AppendLine() ' Append a line break.
            fileContent.Append("*CULTIVARS")
            fileContent.AppendLine() ' Append a line break.
            'fileContent.Append("@C CR INGENO CNAME")
            fileContent.Append(String.Join(Chr(32), siteColumnsHeader))
            fileContent.AppendLine() ' Append a line break.

            ''read all line of dssat_cultivar_site
            'For Each occurence As DataRow In dataTable1.Rows
            rw = DT.Tables(0).Select("Champ='LNCU'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(2))
            'fileContent.Append(formatItem_Lg(occurence.Item("LNCU"), 2))
            fileContent.Append(Chr(32))

            fileContent.Append(dataTable.Rows(0).Item("CG").ToString.PadLeft(2))
            fileContent.Append(Chr(32))

            'rw = DT.Tables(0).Select("Champ='CG'")
            'Dv = rw(0)("dv").ToString
            'fileContent.Append(Dv.PadLeft(2))
            'fileContent.Append(formatItem_Lg(occurence.Item("CG"), 2))
            'fileContent.Append(Chr(32))
            fileContent.Append(dataTable.Rows(0).Item("IdCultivarDssat").ToString.PadLeft(6))
            fileContent.Append(Chr(32))
            fileContent.Append(dataTable.Rows(0).Item("CodCultivar").ToString)
            fileContent.AppendLine() ' Append a line break.
            'Next

        End Using
        Return fileContent
    End Function
    ''' <summary>
    ''' function : read table dssat_x_field and write into fileContent
    ''' </summary>
    ''' <param name="dssat_tableName"></param>
    ''' <param name="dssat_tableId"></param>
    ''' <param name="dssat_tableId_value"></param>
    ''' <param name="fileContent"></param>
    ''' <param name="Connection"></param>
    ''' <returns>fileContent</returns>
    ''' <remarks></remarks>
    Public Function writeBlockField(ByVal dssat_tableName As String, ByVal dssat_tableId As String, ByVal dssat_tableId_value As String, ByVal fileContent As StringBuilder, ByVal Connection As SqliteConnection) As StringBuilder

        'Dim dssat_queryRead As String = "select * from " + dssat_tableName + " where " + dssat_tableId + " = " + dssat_tableId_value + " ;"
        Dim dssat_queryRead As String = "Select Champ, Default_Value_Datamill, defaultValueOtherSource, IFNULL([defaultValueOtherSource], [Default_Value_Datamill]) As dv From Variables Where ((model = 'dssat') And ([Table] = '" & dssat_tableName & "'));"
        Using dataAdapter1 As SqliteDataAdapter = New SqliteDataAdapter(dssat_queryRead, Connection)
            Dim DT As New DataSet()
            Dim rw As DataRow()
            Dim Dv As String
            dataAdapter1.Fill(DT, dssat_tableName)
            'Using dataAdapter1 As SqliteDataAdapter = New SqliteDataAdapter(dssat_queryRead, Connection)
            '    Dim dataSet1 As New DataSet()
            '    dataAdapter1.Fill(dataSet1, dssat_tableName)
            '    Dim dataTable1 As DataTable = dataSet1.Tables(dssat_tableName)
            'Dim idData2 As Single
            Dim siteColumnsHeader() As String = {"@L", "ID_FIELD", "WSTA....", " FLSA", " FLOB", " FLDT", " FLDD", " FLDS", " FLST", "SLTX ", "SLDP ", "ID_SOIL   ", "FLNAME"}
            Dim siteColumnsHeader2() As String = {"@L", "...........XCRD", "...........YCRD", ".....ELEV", ".............AREA", ".SLEN", ".FLWR", ".SLAS", "FLHST", "FHDUR"}
            'header x_field
            fileContent.AppendLine() ' Append a line break.
            fileContent.Append("*FIELDS")
            fileContent.AppendLine() ' Append a line break.
            ''read all line of dssat_x_field
            'For Each occurence As DataRow In dataTable1.Rows
            'idData2 = occurence.Item("id")

            'fileContent.Append("@L ID_FIELD WSTA....  FLSA  FLOB  FLDT  FLDD  FLDS  FLST SLTX  SLDP  ID_SOIL    FLNAME")
            fileContent.Append(String.Join(Chr(32), siteColumnsHeader))
            fileContent.AppendLine() ' Append a line break.

            rw = DT.Tables(0).Select("Champ='FL'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(2))
            'fileContent.Append(formatItem_Lg(occurence.Item("FL"), 2))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='ID_FIELD'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(8))
            'fileContent.Append(formatItem_Lg(occurence.Item("ID_FIELD"), 8))
            fileContent.Append(Chr(32))
            'rw = DT.Tables(0).Select("Champ='WSTA'")
            'Dv = rw(0)("dv").ToString
            fileContent.Append(Mid(dssat_tableId_value, 1, 4).PadRight(8))
            'fileContent.Append(formatItem_Lg(occurence.Item("WSTA"), 8))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='FLSA'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(5))
            'fileContent.Append(formatItem_Lg(occurence.Item("FLSA"), 5))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='FLOB'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(5))
            'fileContent.Append(formatItem_Lg(occurence.Item("FLOB"), 5))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='FLDT'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(5))
            'fileContent.Append(formatItem_Lg(occurence.Item("FLDT"), 5))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='FLDD'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(5))
            'fileContent.Append(formatItem_Lg(occurence.Item("FLDD"), 5))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='FLDS'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(5))
            'fileContent.Append(formatItem_Lg(occurence.Item("FLDS"), 5))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='FLST'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(5))
            'fileContent.Append(formatItem_Lg(occurence.Item("FLST"), 5))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='SLTX'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(4))
            'fileContent.Append(formatItem_Lg(occurence.Item("SLTX"), 4))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='SLDP'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(5))
            'fileContent.Append(formatItem_Lg(occurence.Item("SLDP"), 5))
            fileContent.Append(Chr(32))
            fileContent.Append(Chr(32))
            'rw = DT.Tables(0).Select("Champ='ID_SOIL'")
            'Dv = rw(0)("dv").ToString
            fileContent.Append("XX" & Mid(dssat_tableId_value, 1, 4) & "0101")
            'fileContent.Append(Dv.PadLeft(10))
            'fileContent.Append(formatItem_Lg(occurence.Item("ID_SOIL"), 10))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='FLNAME'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(60))
            'fileContent.Append(formatItem_Lg(occurence.Item("FLNAME"), 60))
            fileContent.AppendLine() ' Append a line break.

            'query x_field_data
            'dssat_tableName = "dssat_x_field_data"
            'dssat_tableId = "dssat_x_field_id"
            'dssat_tableId_value = idData2
            'writeBlockFieldData(dssat_tableName, dssat_tableId, dssat_tableId_value, fileContent, Connection)
            fileContent.Append(String.Join(Chr(32), siteColumnsHeader2))
            fileContent.AppendLine() ' Append a line break.

            rw = DT.Tables(0).Select("Champ='FL'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(2))
            'fileContent.Append(formatItem_Lg(occurence.Item("FL"), 2))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='XCRD'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(15))
            'fileContent.Append(formatItem_Lg(occurence.Item("XCRD"), 15))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='YCRD'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(15))
            'fileContent.Append(formatItem_Lg(occurence.Item("YCRD"), 15))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='ELEV'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(9))
            'fileContent.Append(formatItem_Lg(occurence.Item("ELEV"), 9))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='AREA'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(17))
            'fileContent.Append(formatItem_Lg(occurence.Item("AREA"), 17))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='SLEN'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(5))
            'fileContent.Append(formatItem_Lg(occurence.Item("SLEN"), 5))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='FLWR'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(5))
            'fileContent.Append(formatItem_Lg(occurence.Item("FLWR"), 5))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='SLAS'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(5))
            'fileContent.Append(formatItem_Lg(occurence.Item("SLAS"), 5))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='FLHST'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(5))
            'fileContent.Append(formatItem_Lg(occurence.Item("FLHST"), 5))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='FHDUR'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(5))
            'fileContent.Append(formatItem_Lg(occurence.Item("FHDUR"), 5))
            fileContent.Append(Chr(32))
            'Next
        End Using

            Return fileContent
    End Function
    ''' <summary>
    ''' function : read table dssat_x_field_data and write into fileContent
    ''' </summary>
    ''' <param name="dssat_tableName"></param>
    ''' <param name="dssat_tableId"></param>
    ''' <param name="dssat_tableId_value"></param>
    ''' <param name="fileContent"></param>
    ''' <param name="Connection"></param>
    ''' <returns>fileContent</returns>
    ''' <remarks></remarks>
    Public Function writeBlockFieldData(ByVal dssat_tableName As String, ByVal dssat_tableId As String, ByVal dssat_tableId_value As String, ByVal fileContent As StringBuilder, ByVal Connection As SqliteConnection) As StringBuilder

        Dim dssat_queryRead As String = "select * from " + dssat_tableName + " where " + dssat_tableId + " = " + dssat_tableId_value + " ;"
        Using dataAdapter2 As SqliteDataAdapter = New SqliteDataAdapter(dssat_queryRead, Connection)
            Dim dataSet2 As New DataSet()
            dataAdapter2.Fill(dataSet2, dssat_tableName)
            Dim dataTable2 As DataTable = dataSet2.Tables(dssat_tableName)
            Dim siteColumnsHeader() As String = {"@L", "...........XCRD", "...........YCRD", ".....ELEV", ".............AREA", ".SLEN", ".FLWR", ".SLAS", "FLHST", "FHDUR"}

            'fileContent.AppendLine() ' Append a line break.
            'fileContent.Append("@L ...........XCRD ...........YCRD .....ELEV .............AREA .SLEN .FLWR .SLAS FLHST FHDUR")
            fileContent.Append(String.Join(Chr(9), siteColumnsHeader))
            fileContent.AppendLine() ' Append a line break.

            For Each ligne As DataRow In dataTable2.Rows
                fileContent.Append(formatItem(ligne.Item("l")))
                fileContent.Append(Chr(32))
                fileContent.Append(formatItem(ligne.Item("xcrd")))
                fileContent.Append(Chr(32))
                fileContent.Append(formatItem(ligne.Item("ycrd")))
                fileContent.Append(Chr(32))
                fileContent.Append(formatItem(ligne.Item("elev")))
                fileContent.Append(Chr(32))
                fileContent.Append(formatItem(ligne.Item("area")))
                fileContent.Append(Chr(32))
                fileContent.Append(formatItem(ligne.Item("slen")))
                fileContent.Append(Chr(32))
                fileContent.Append(formatItem(ligne.Item("flwr")))
                fileContent.Append(Chr(32))
                fileContent.Append(formatItem(ligne.Item("slas")))
                fileContent.Append(Chr(32))
                fileContent.Append(formatItem(ligne.Item("flhst")))
                fileContent.Append(Chr(32))
                fileContent.Append(formatItem(ligne.Item("fhdur")))
                fileContent.Append(Chr(32))
            Next
        End Using

        Return fileContent
    End Function
    ''' <summary>
    ''' function : read table dssat_x_soilanalysis and write into fileContent
    ''' </summary>
    ''' <param name="dssat_tableName"></param>
    ''' <param name="dssat_tableId"></param>
    ''' <param name="dssat_tableId_value"></param>
    ''' <param name="fileContent"></param>
    ''' <param name="Connection"></param>
    ''' <returns>fileContent</returns>
    ''' <remarks></remarks>
    Public Function writeBlockSoilAnalysis(ByVal dssat_tableName As String, ByVal dssat_tableId As String, ByVal dssat_tableId_value As String, ByVal fileContent As StringBuilder, ByVal Connection As SqliteConnection) As StringBuilder

        'Dim dssat_queryRead As String = "select * from " + dssat_tableName + " where " + dssat_tableId + " = " + dssat_tableId_value + " ;"
        Dim dssat_queryRead As String = "Select Champ, Default_Value_Datamill, defaultValueOtherSource, IFNULL([defaultValueOtherSource],  [Default_Value_Datamill]) As dv From Variables Where ((model = 'dssat') And ([Table] = '" & dssat_tableName & "'));"
        Using dataAdapter1 As SqliteDataAdapter = New SqliteDataAdapter(dssat_queryRead, Connection)
            Dim DT As New DataSet()
            Dim rw As DataRow()
            Dim Dv As String
            dataAdapter1.Fill(DT, dssat_tableName)
            Dim idData2 As Single
            Dim siteColumnsHeader() As String = {"@A", "SADAT", " SMHB", " SMPX", " SMKE", " SANAME"}

            'header soil_analysis
            fileContent.AppendLine() ' Append a line break.
            fileContent.AppendLine() ' Append a line break.
            fileContent.Append("*SOIL ANALYSIS")
            fileContent.AppendLine() ' Append a line break.

            ''read all line of dssat_soil_analysis
            'For Each occurence As DataRow In dataTable1.Rows

            'idData2 = occurence.Item("id")

            'fileContent.Append("@A SADAT  SMHB  SMPX  SMKE  SANAME")
            fileContent.Append(String.Join(Chr(32), siteColumnsHeader))
            fileContent.AppendLine() ' Append a line break.
            rw = DT.Tables(0).Select("Champ='LNSA'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(2))
            'fileContent.Append(formatItem_Lg(occurence.Item("LNSA"), 2))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='SADAT'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(5))
            'fileContent.Append(formatItemDate(formatItem_Lg(occurence.Item("SADAT"), 5)))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='SMHB'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(5))
            'fileContent.Append(formatItem_Lg(occurence.Item("SMHB"), 5))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='SMPX'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(5))
            'fileContent.Append(formatItem_Lg(occurence.Item("SMPX"), 5))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='SMKE'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(5))
            'fileContent.Append(formatItem_Lg(occurence.Item("SMKE"), 5))
            fileContent.Append(Chr(32))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='SANAME'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv)
            'fileContent.Append(formatItem_Lg(occurence.Item("SANAME"), 60))
            fileContent.Append(Chr(32))
                fileContent.AppendLine() ' Append a line break.

                'query x_soil_analysis_data
                dssat_tableName = "dssat_x_soil_analysis_data"
                dssat_tableId = "dssat_x_soil_analysis_id"
                dssat_tableId_value = idData2
                writeBlockSoilAnalysisData(dssat_tableName, dssat_tableId, dssat_tableId_value, fileContent, Connection)
            'Next
        End Using
        Return fileContent
    End Function
    ''' <summary>
    ''' function : read table dssat_x_soilAnalysis_data and write into fileContent
    ''' </summary>
    ''' <param name="dssat_tableName"></param>
    ''' <param name="dssat_tableId"></param>
    ''' <param name="dssat_tableId_value"></param>
    ''' <param name="fileContent"></param>
    ''' <param name="Connection"></param>
    ''' <returns>fileContent</returns>
    ''' <remarks></remarks>
    Public Function writeBlockSoilAnalysisData(ByVal dssat_tableName As String, ByVal dssat_tableId As String, ByVal dssat_tableId_value As String, ByVal fileContent As StringBuilder, ByVal Connection As SqliteConnection) As StringBuilder

        Dim dssat_queryRead As String = "Select Champ, Default_Value_Datamill, defaultValueOtherSource, IFNULL([defaultValueOtherSource],  [Default_Value_Datamill]) As dv From Variables Where ((model = 'dssat') And ([Table] = '" & dssat_tableName & "'));"
        Using dataAdapter1 As SqliteDataAdapter = New SqliteDataAdapter(dssat_queryRead, Connection)
            Dim DT As New DataSet()
            Dim rw As DataRow()
            Dim Dv As String
            dataAdapter1.Fill(DT, dssat_tableName)
            Dim siteColumnsHeader() As String = {"@A", " SABL", " SADM", " SAOC", " SANI", "SAPHW", "SAPHB", " SAPX", " SAKE", " SASC"}

            'fileContent.AppendLine() ' Append a line break.
            'fileContent.Append("@A  SABL  SADM  SAOC  SANI SAPHW SAPHB  SAPX  SAKE  SASC")
            fileContent.Append(String.Join(Chr(32), siteColumnsHeader))
            fileContent.AppendLine() ' Append a line break.

            'For Each ligne As DataRow In dataTable2.Rows
            rw = DT.Tables(0).Select("Champ='LNSA'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(2))
            'fileContent.Append(formatItem_Lg(ligne.Item("LNSA"), 2))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='SABL'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(5))
            'fileContent.Append(formatItem_Lg(ligne.Item("SABL"), 5))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='SADM'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(5))
            'fileContent.Append(formatItem_Lg(ligne.Item("SADM"), 5))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='SAOC'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(5))
            'fileContent.Append(formatItem_Lg(ligne.Item("SAOC"), 5))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='SANI'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(5))
            'fileContent.Append(formatItem_Lg(ligne.Item("SANI"), 5))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='SAPHW'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(5))
            'fileContent.Append(formatItem_Lg(ligne.Item("SAPHW"), 5))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='SAPHB'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(5))
            'fileContent.Append(formatItem_Lg(ligne.Item("SAPHB"), 5))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='SAPX'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(5))
            'fileContent.Append(formatItem_Lg(ligne.Item("SAPX"), 5))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='SAKE'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(5))
            'fileContent.Append(formatItem_Lg(ligne.Item("SAKE"), 5))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='SASC'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(5))
            'fileContent.Append(formatItem_Lg(ligne.Item("SASC"), 5))
            fileContent.Append(Chr(32))
            ' Next
        End Using

        Return fileContent
    End Function
    ''' <summary>
    ''' function : read table dssat_x_initial_condition and write into fileContent
    ''' </summary>
    ''' <param name="dssat_tableName"></param>
    ''' <param name="dssat_tableId"></param>
    ''' <param name="dssat_tableId_value"></param>
    ''' <param name="fileContent"></param>
    ''' <param name="Connection"></param>
    ''' <returns>fileContent</returns>
    ''' <remarks></remarks>
    Public Function writeBlockInitialCondition(ByVal dssat_tableName As String, ByVal dssat_tableId As String, ByVal dssat_tableId_value As String, ByVal fileContent As StringBuilder, ByVal Connection As SqliteConnection, ByVal MI_Connection As SqliteConnection) As StringBuilder

        Dim dssat_queryRead As String = "Select Champ, Default_Value_Datamill, defaultValueOtherSource, IFNULL([defaultValueOtherSource],  [Default_Value_Datamill]) As dv From Variables Where ((model = 'dssat') And ([Table] = '" & dssat_tableName & "'));"
        Using dataAdapter1 As SqliteDataAdapter = New SqliteDataAdapter(dssat_queryRead, Connection)
            Dim DT As New DataSet()
            Dim rw As DataRow()
            Dim Dv As String
            dataAdapter1.Fill(DT, dssat_tableName)

            Dim fetchAllQuery As String = "Select SimUnitList.idsim, SimUnitList.StartYear, SimUnitList.StartDay,CropManagement.SowingDate, ListCultOption.PRCROP FROM (ListCultOption INNER JOIN (ListCultivars INNER JOIN CropManagement ON ListCultivars.IdCultivar = CropManagement.Idcultivar) ON ListCultOption.CodePSpecies = ListCultivars.CodePSpecies) INNER JOIN SimUnitList ON CropManagement.idMangt = SimUnitList.idMangt Where IdSim ='" & dssat_tableId & "';"
            'Dim fetchAllQuery As String = "Select SimUnitList.idsim, SimUnitList.StartYear,InitialConditions.StartDay From InitialConditions INNER Join SimUnitList On InitialConditions.idIni = SimUnitList.idIni Where IdSim ='" & dssat_tableId & "';"
            Dim dataAdapter As SqliteDataAdapter = New SqliteDataAdapter(fetchAllQuery, MI_Connection)
            ' Filling Dataset
            Dim dataSet As New DataSet()
            dataAdapter.Fill(dataSet, "dssat_x_exp")
            Dim dataTable As DataTable = dataSet.Tables("dssat_x_exp")
            Dim siteColumnsHeader() As String = {"@C", "  PCR", "ICDAT", " ICRT", " ICND", " ICRN", " ICRE", " ICWD", "ICRES", "ICREN", "ICREP", "ICRIP", "ICRID", "ICNAME"}

            'header initial condition
            fileContent.AppendLine() ' Append a line break.
            fileContent.AppendLine() ' Append a line break.
            fileContent.Append("*INITIAL CONDITIONS")
            fileContent.AppendLine() ' Append a line break.

            ''read all line of dssat_x_initial_condition
            'For Each occurence As DataRow In dataTable1.Rows

            'idData2 = occurence.Item("id")

            'fileContent.Append("@C   PCR ICDAT  ICRT  ICND  ICRN  ICRE  ICWD ICRES ICREN ICREP ICRIP ICRID ICNAME")
            fileContent.Append(String.Join(Chr(32), siteColumnsHeader))
                fileContent.AppendLine() ' Append a line break.

            rw = DT.Tables(0).Select("Champ='LNIC'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(2))
            'fileContent.Append(formatItem_Lg(occurence.Item("LNIC"), 2))
            fileContent.Append(Chr(32))

            fileContent.Append(dataTable.Rows(0).Item("PRCROP").ToString.PadLeft(5))
            fileContent.Append(Chr(32))

            'rw = DT.Tables(0).Select("Champ='PRCROP'")
            'Dv = rw(0)("dv").ToString
            'fileContent.Append(Dv.PadLeft(5))
            'fileContent.Append(Right_Justified(formatItem_Lg(occurence.Item("PRCROP"), 5)))
            'fileContent.Append(Chr(32))
            'If CInt(dataTable.Rows(0).Item("Sowingdate")) > 365 Then
            '    fileContent.Append(Mid((CInt(dataTable.Rows(0).Item("StartYear")) + 1).ToString, 3, 2) & (CInt(dataTable.Rows(0).Item("Sowingdate")) - 365).ToString.PadLeft(3, "0"))
            'Else
            '    fileContent.Append(Mid(dataTable.Rows(0).Item("StartYear").ToString, 3, 2) & dataTable.Rows(0).Item("Sowingdate").ToString.PadLeft(3, "0"))
            'End If
            fileContent.Append(Mid(dataTable.Rows(0).Item("StartYear").ToString, 3, 2) & dataTable.Rows(0).Item("StartDay").ToString.PadLeft(3, "0"))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='WRESR'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(5))
            'fileContent.Append(formatItem_Lg(occurence.Item("WRESR"), 5))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='WRESND'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(5))
            'fileContent.Append(formatItem_Lg(occurence.Item("WRESND"), 5))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='EFINOC'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(5))
            'fileContent.Append(formatItem_Lg(occurence.Item("EFINOC"), 5))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='EFNFIX'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(5))
            'fileContent.Append(formatItem_Lg(occurence.Item("EFNFIX"), 5))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='ICWD'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(5))
            'fileContent.Append(formatItem_Lg(occurence.Item("ICWD"), 5))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='ICRES'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(5))
            'fileContent.Append(formatItem_Lg(occurence.Item("ICRES"), 5))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='ICREN'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(5))
            'fileContent.Append(formatItem_Lg(occurence.Item("ICREN"), 5))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='ICREP'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(5))
            'fileContent.Append(formatItem_Lg(occurence.Item("ICREP"), 5))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='ICRIP'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(5))
            'fileContent.Append(formatItem_Lg(occurence.Item("ICRIP"), 5))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='ICRID'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(5))
            'fileContent.Append(formatItem_Lg(occurence.Item("ICRID"), 5))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='ICNAME'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv)
            'fileContent.Append(formatItem_Lg(occurence.Item("ICNAME"), 60))
            fileContent.Append(Chr(32))
            fileContent.AppendLine() ' Append a line break.

            'query dssat_x_initial_condition_data
            dssat_tableName = "dssat_x_initial_condition_data"
            'dssat_tableId = "dssat_x_initial_condition_id"
            'dssat_tableId_value = idData2
            writeBlockInitialConditionData(dssat_tableName, dssat_tableId, dssat_tableId_value, fileContent, Connection, MI_Connection)
            'Next

        End Using
        Return fileContent
    End Function
    ''' <summary>
    ''' function : read table dssat_x_initial_condition_data and write into fileContent
    ''' </summary>
    ''' <param name="dssat_tableName"></param>
    ''' <param name="dssat_tableId"></param>
    ''' <param name="dssat_tableId_value"></param>
    ''' <param name="fileContent"></param>
    ''' <param name="Connection"></param>
    ''' <returns>fileContent</returns>
    ''' <remarks></remarks>
    Public Function writeBlockInitialConditionData(ByVal dssat_tableName As String, ByVal dssat_tableId As String, ByVal dssat_tableId_value As String, ByVal fileContent As StringBuilder, ByVal Connection As SqliteConnection, ByVal MI_Connection As SqliteConnection) As StringBuilder

        Dim dssat_queryRead As String = "Select Champ, Default_Value_Datamill, defaultValueOtherSource, IFNULL([defaultValueOtherSource],  [Default_Value_Datamill]) As dv From Variables Where ((model = 'dssat') And ([Table] = '" & dssat_tableName & "'));"
        Using dataAdapter1 As SqliteDataAdapter = New SqliteDataAdapter(dssat_queryRead, Connection)
            Dim DT As New DataSet()
            Dim rw As DataRow()
            Dim Dv As String
            Dim i As Integer
            dataAdapter1.Fill(DT, dssat_tableName)

            'Dim fetchAllQuery As String = "SELECT SimUnitList.idsim, Soil.SoilTotalDepth FROM Soil INNER JOIN SimUnitList ON Soil.IdSoil = SimUnitList.idsoil Where IdSim ='" & dssat_tableId & "';"
            Dim fetchAllQuery As String = "SELECT DISTINCT Soil.Wwp AS 'Soil.Wwp', Soil.Wfc AS 'Soil.Wfc', soil.bd as 'soil.bd', Soil.*," _
                & "SoilLayers.Wwp AS 'SoilLayers.Wwp', SoilLayers.Wfc AS 'SoilLayers.Wfc', SoilLayers.*, " _ 
                & "InitialConditions.* FROM InitialConditions INNER JOIN " _
                & "((Soil INNER JOIN SimUnitList ON Lower(Soil.IdSoil) = Lower(SimUnitList.idsoil)) LEFT JOIN SoilLayers ON Lower(Soil.IdSoil) = Lower(SoilLayers.idsoil)) " _
                & "ON InitialConditions.idIni = SimUnitList.idIni Where IdSim ='" & dssat_tableId & "' Order by NumLayer;"

            'Console.WriteLine("converter l1100 : " & fetchAllQuery)

            Dim dataAdapter As SqliteDataAdapter = New SqliteDataAdapter(fetchAllQuery, MI_Connection)
            ' Filling Dataset
            Dim dataSet As New DataSet()
            dataAdapter.Fill(dataSet, "dssat_x_exp")
            Dim dataTable As DataTable = dataSet.Tables("dssat_x_exp")
            Dim siteColumnsHeader() As String = {"@C", " ICBL", " SH2O", " SNH4", " SNO3"}

            'header data
            'fileContent.Append("@C  ICBL  SH2O  SNH4  SNO3")
            fileContent.Append(String.Join(Chr(32), siteColumnsHeader))
            fileContent.AppendLine() ' Append a line break.
            If LCase(dataTable.Rows(0).Item("SoilOption")) = "simple" Then
                'For Each ligne As DataRow In dataTable2.Rows
                For i = 0 To 1
                    'lnic ? i+1
                    rw = DT.Tables(0).Select("Champ='LNIC'")
                    Dv = rw(0)("dv").ToString
                    fileContent.Append(Dv.ToString.PadLeft(2))
                    'fileContent.Append(formatItem_Lg(ligne.Item("LNIC"), 2))
                    fileContent.Append(Chr(32))
                    If i = 0 Then
                        fileContent.Append("30.0".PadLeft(5))
                    Else
                        fileContent.Append(dataTable.Rows(0).Item("SoilTotalDepth").ToString.PadLeft(5))
                    End If
                    fileContent.Append(Chr(32))
                    'swinit
                    'If i = 0 Then
                    'fileContent.Append("0.0".PadLeft(5))
                    'Else
                    '(Soil.Wwp/100)+Wstockinit*(Soil.Wfc-Soil.Wwp)/10000 for the two layers 
                    fileContent.Append(FormatNumber(((dataTable.Rows(0).Item("Soil.Wwp") / 100) + dataTable.Rows(0).Item("Wstockinit") * (dataTable.Rows(0).Item("Soil.Wfc") - dataTable.Rows(0).Item("Soil.Wwp")) / 10000), 2).ToString.PadLeft(5))
                    'End If
                    'rw = DT.Tables(0).Select("Champ='SWINIT'")
                    'Dv = rw(0)("dv").ToString
                    'fileContent.Append(Dv.PadLeft(5))
                    'fileContent.Append(formatItem_Lg(ligne.Item("SWINIT"), 5))
                    'inh4
                    fileContent.Append(Chr(32))
                    rw = DT.Tables(0).Select("Champ='INH4'")
                    Dv = rw(0)("dv").ToString
                    fileContent.Append(Dv.PadLeft(5))
                    'fileContent.Append(formatItem_Lg(ligne.Item("INH4"), 5))
                    'ino3
                    fileContent.Append(Chr(32))
                    fileContent.Append(FormatNumber(10 * dataTable.Rows(0).Item("Ninit") / (dataTable.Rows(0).Item("soil.bd") * dataTable.Rows(0).Item("SoilTotalDepth")), 2).ToString.PadLeft(5))

                    'rw = DT.Tables(0).Select("Champ='INO3'")
                    'Dv = rw(0)("dv").ToString
                    'fileContent.Append(Dv.PadLeft(5))
                    'fileContent.Append(formatItem_Lg(ligne.Item("INO3"), 5))
                    fileContent.AppendLine() ' Append a line break.
                Next
            Else
                For i = 0 To dataTable.Rows.Count - 1
                    'lnic ? i+1
                    rw = DT.Tables(0).Select("Champ='LNIC'")
                    Dv = rw(0)("dv").ToString
                    fileContent.Append(Dv.ToString.PadLeft(2))
                    'fileContent.Append(formatItem_Lg(ligne.Item("LNIC"), 2))
                    fileContent.Append(Chr(32))
                    fileContent.Append(dataTable.Rows(i).Item("Ldown").ToString.PadLeft(5))
                    fileContent.Append(Chr(32))
                    'swinit
                    fileContent.Append(FormatNumber((dataTable.Rows(i).Item("SoilLayers.Wwp") / 100 + dataTable.Rows(i).Item("Wstockinit") * (dataTable.Rows(i).Item("SoilLayers.Wfc") - dataTable.Rows(i).Item("SoilLayers.Wwp")) / 10000), 2).ToString.PadLeft(5))
                    'rw = DT.Tables(0).Select("Champ='SWINIT'")
                    'Dv = rw(0)("dv").ToString
                    'fileContent.Append(Dv.PadLeft(5))
                    'fileContent.Append(formatItem_Lg(ligne.Item("SWINIT"), 5))
                    'inh4
                    fileContent.Append(Chr(32))
                    rw = DT.Tables(0).Select("Champ='INH4'")
                    Dv = rw(0)("dv").ToString
                    fileContent.Append(Dv.PadLeft(5))
                    'fileContent.Append(formatItem_Lg(ligne.Item("INH4"), 5))
                    'ino3
                    fileContent.Append(Chr(32))
                    fileContent.Append((10 * dataTable.Rows(i).Item("Ninit") / (dataTable.Rows(i).Item("soil.bd") * dataTable.Rows(i).Item("SoilTotalDepth")).ToString.PadLeft(5)))

                    'rw = DT.Tables(0).Select("Champ='INO3'")
                    'Dv = rw(0)("dv").ToString
                    'fileContent.Append(Dv.PadLeft(5))
                    'fileContent.Append(formatItem_Lg(ligne.Item("INO3"), 5))
                    fileContent.AppendLine() ' Append a line break.
                Next
            End If
        End Using

        Return fileContent
    End Function
    ''' <summary>
    ''' function : read table dssat_x_planting_detail and write into fileContent
    ''' </summary>
    ''' <param name="dssat_tableName"></param>
    ''' <param name="dssat_tableId"></param>
    ''' <param name="dssat_tableId_value"></param>
    ''' <param name="fileContent"></param>
    ''' <param name="Connection"></param>
    ''' <returns>fileContent</returns>
    ''' <remarks></remarks>
    Public Function writeBlockPlantingDetail(ByVal dssat_tableName As String, ByVal dssat_tableId As String, ByVal dssat_tableId_value As String, ByVal fileContent As StringBuilder, ByVal Connection As SqliteConnection, ByVal MI_Connection As SqliteConnection) As StringBuilder
        Dim dssat_queryRead As String = "Select Champ, Default_Value_Datamill, defaultValueOtherSource, IFNULL([defaultValueOtherSource],  [Default_Value_Datamill]) As dv From Variables Where ((model = 'dssat') And ([Table] = '" & dssat_tableName & "'));"
        Dim Bissext As Integer
        Using dataAdapter1 As SqliteDataAdapter = New SqliteDataAdapter(dssat_queryRead, Connection)
            Dim DT As New DataSet()
            Dim rw As DataRow()
            Dim Dv As String
            dataAdapter1.Fill(DT, dssat_tableName)

            Dim fetchAllQuery As String = "SELECT SimUnitList.idsim,  SimUnitList.StartYear, CropManagement.sdens, CropManagement.sowingdate FROM CropManagement INNER JOIN SimUnitList ON CropManagement.idMangt = SimUnitList.idMangt Where IdSim ='" & dssat_tableId & "';"
            Dim dataAdapter As SqliteDataAdapter = New SqliteDataAdapter(fetchAllQuery, MI_Connection)
            ' Filling Dataset
            Dim dataSet As New DataSet()
            dataAdapter.Fill(dataSet, "dssat_x_exp")
            Dim dataTable As DataTable = dataSet.Tables("dssat_x_exp")
            Dim siteColumnsHeader() As String = {"@P", "PDATE", "EDATE", " PPOP", " PPOE", " PLME", " PLDS", " PLRS", " PLRD", " PLDP", " PLWT", " PAGE", " PENV", " PLPH", " SPRL", "                       PLNAME"}

            'header planting detail
            fileContent.AppendLine() ' Append a line break.
            fileContent.Append("*PLANTING DETAILS")
            fileContent.AppendLine() ' Append a line break.
            'fileContent.Append("@P PDATE EDATE  PPOP  PPOE  PLME  PLDS  PLRS  PLRD  PLDP  PLWT  PAGE  PENV  PLPH  SPRL                        PLNAME")
            fileContent.Append(String.Join(Chr(32), siteColumnsHeader))
            fileContent.AppendLine() ' Append a line break.

            ''read all line of dssat_x_planting_detail
            'For Each occurence As DataRow In dataTable1.Rows

            rw = DT.Tables(0).Select("Champ='LNPLT'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(2))
            'fileContent.Append(formatItem_Lg(occurence.Item("LNPLT"), 2))
            fileContent.Append(Chr(32))
            If CInt(dataTable.Rows(0).Item("StartYear")) Mod 4 = 0 Then
                Bissext = 1
            Else
                Bissext = 0
            End If

            If CInt(dataTable.Rows(0).Item("Sowingdate")) > 365 + Bissext Then
                fileContent.Append(Mid((CInt(dataTable.Rows(0).Item("StartYear")) + 1).ToString, 3, 2) & (CInt(dataTable.Rows(0).Item("Sowingdate")) - 365 - Bissext).ToString.PadLeft(3, "0"))
            Else
                fileContent.Append(Mid(dataTable.Rows(0).Item("StartYear").ToString, 3, 2) & dataTable.Rows(0).Item("Sowingdate").ToString.PadLeft(3, "0"))
            End If
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='IEMRG'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(5))
            'fileContent.Append(formatItem_Lg(occurence.Item("IEMRG"), 5))
            fileContent.Append(Chr(32))
            fileContent.Append(dataTable.Rows(0).Item("Sdens").ToString.PadLeft(5))
            fileContent.Append(Chr(32))
            fileContent.Append(dataTable.Rows(0).Item("Sdens").ToString.PadLeft(5))
            fileContent.Append(Chr(32))
            fileContent.Append(Chr(32))
            fileContent.Append(Chr(32))
            fileContent.Append(Chr(32))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='PLME'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(1))
            'fileContent.Append(formatItem_Lg(occurence.Item("PLME"), 1))
            fileContent.Append(Chr(32))
            fileContent.Append(Chr(32))
            fileContent.Append(Chr(32))
            fileContent.Append(Chr(32))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='PLDS'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(1))
            'fileContent.Append(formatItem_Lg(occurence.Item("PLDS"), 1))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='ROWSPC'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(5))
            'fileContent.Append(formatItem_Lg(occurence.Item("ROWSPC"), 5))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='AZIR'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(5))
            'fileContent.Append(formatItem_Lg(occurence.Item("AZIR"), 5))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='SDEPHT'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(5))
            'fileContent.Append(formatItem_Lg(occurence.Item("SDEPHT"), 5))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='SDWTPL'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(5))
            'fileContent.Append(formatItem_Lg(occurence.Item("SDWTPL"), 5))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='SDAGE'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(5))
            'fileContent.Append(formatItem_Lg(occurence.Item("SDAGE"), 5))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='ATEMP'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(5))
            'fileContent.Append(formatItem_Lg(occurence.Item("ATEMP"), 5))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='PLPH'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(5))
            'fileContent.Append(formatItem_Lg(occurence.Item("PLPH"), 5))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='SPRLAP'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(5))
            'fileContent.Append(formatItem_Lg(occurence.Item("SPRLAP"), 5))
            For i = 1 To 24
                fileContent.Append(Chr(32))
            Next
            rw = DT.Tables(0).Select("Champ='PLNAME'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(60))
            'fileContent.Append(formatItem_Lg(occurence.Item("PLNAME"), 60))
            fileContent.AppendLine() ' Append a line break.
            'Next

        End Using

        Return fileContent
    End Function
    ''' <summary>
    ''' function : read table dssat_x_irrigation_water and write into fileContent
    ''' </summary>
    ''' <param name="dssat_tableName"></param>
    ''' <param name="dssat_tableId"></param>
    ''' <param name="dssat_tableId_value"></param>
    ''' <param name="fileContent"></param>
    ''' <param name="Connection"></param>
    ''' <returns>fileContent</returns>
    ''' <remarks></remarks>
    Public Function writeBlockIrrigationWater(ByVal dssat_tableName As String, ByVal dssat_tableId As String, ByVal dssat_tableId_value As String, ByVal fileContent As StringBuilder, ByVal Connection As SqliteConnection) As StringBuilder

        Dim dssat_queryRead As String = "Select Champ, Default_Value_Datamill, defaultValueOtherSource, IFNULL([defaultValueOtherSource],  [Default_Value_Datamill]) As dv From Variables Where ((model = 'dssat') And ([Table] = '" & dssat_tableName & "'));"
        Using dataAdapter1 As SqliteDataAdapter = New SqliteDataAdapter(dssat_queryRead, Connection)
            Dim DT As New DataSet()
            Dim rw As DataRow()
            Dim Dv As String
            dataAdapter1.Fill(DT, dssat_tableName)
            'Dim idData2 As Single
            Dim siteColumnsHeader() As String = {"@I", " EFIR", " IDEP", " ITHR", " IEPT", " IOFF", " IAME", " IAMT", "IRNAME"}
            'header irrigation
            fileContent.AppendLine() ' Append a line break.
            fileContent.Append("*IRRIGATION AND WATER MANAGEMENT")
            fileContent.AppendLine() ' Append a line break.

            ''read all line of dssat_x_irrigation_water
            'For Each occurence As DataRow In dataTable1.Rows

            'idData2 = occurence.Item("id")

            'fileContent.Append("@I  EFIR  IDEP  ITHR  IEPT  IOFF  IAME  IAMT IRNAME")
            fileContent.Append(String.Join(Chr(32), siteColumnsHeader))
                fileContent.AppendLine() ' Append a line break.

            rw = DT.Tables(0).Select("Champ='LNIR'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(2))
            'fileContent.Append(formatItem_Lg(occurence.Item("LNIR"), 2))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='EFFIRX'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(5))
            'fileContent.Append(formatItem_Lg(occurence.Item("EFFIRX"), 5))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='DSOILX'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(5))
            'fileContent.Append(formatItem_Lg(occurence.Item("DSOILX"), 5))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='THETCX'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(5))
            'fileContent.Append(formatItem_Lg(occurence.Item("THETCX"), 5))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='IEPTX'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(5))
            '4fileContent.Append(formatItem_Lg(occurence.Item("IEPTX"), 5))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='IOFFX'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(5))
            '4fileContent.Append(formatItem_Lg(occurence.Item("IOFFX"), 5))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='IAMEX'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(5))
            '4fileContent.Append(formatItem_Lg(occurence.Item("IAMEX"), 5))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='AIRAMX'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(5))
            '4fileContent.Append(formatItem_Lg(occurence.Item("AIRAMX"), 5))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='IRNAME'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(5))
            '4fileContent.Append(formatItem_Lg(occurence.Item("IRNAME"), 5))
            fileContent.AppendLine() ' Append a line break.

                dssat_tableName = "dssat_x_irrigation_water_data"
                dssat_tableId = "dssat_x_irrigation_water_id"
            'dssat_tableId_value = idData2
            writeBlockIrrigationWaterData(dssat_tableName, dssat_tableId, dssat_tableId_value, fileContent, Connection)
            'Next

        End Using

        Return fileContent
    End Function
    ''' <summary>
    ''' function : read table dssat_x_irrigation_water_data and write into fileContent
    ''' </summary>
    ''' <param name="dssat_tableName"></param>
    ''' <param name="dssat_tableId"></param>
    ''' <param name="dssat_tableId_value"></param>
    ''' <param name="fileContent"></param>
    ''' <param name="Connection"></param>
    ''' <returns>fileContent</returns>
    ''' <remarks></remarks>
    Public Function writeBlockIrrigationWaterData(ByVal dssat_tableName As String, ByVal dssat_tableId As String, ByVal dssat_tableId_value As String, ByVal fileContent As StringBuilder, ByVal Connection As SqliteConnection) As StringBuilder

        Dim dssat_queryRead As String = "Select Champ, Default_Value_Datamill, defaultValueOtherSource, IFNULL([defaultValueOtherSource],  [Default_Value_Datamill]) As dv From Variables Where ((model = 'dssat') And ([Table] = '" & dssat_tableName & "'));"
        Using dataAdapter1 As SqliteDataAdapter = New SqliteDataAdapter(dssat_queryRead, Connection)
            Dim DT As New DataSet()
            Dim rw As DataRow()
            Dim Dv As String
            dataAdapter1.Fill(DT, dssat_tableName)
            Dim siteColumnsHeader() As String = {"@I", "IDATE", " IROP", "IRVAL"}

            'fileContent.AppendLine() ' Append a line break.
            'fileContent.Append("@I IDATE  IROP IRVAL")
            fileContent.Append(String.Join(Chr(32), siteColumnsHeader))
            fileContent.AppendLine() ' Append a line break.

            'For Each ligne As DataRow In dataTable2.Rows
            rw = DT.Tables(0).Select("Champ='LNIR'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(2))
            'fileContent.Append(formatItem_Lg(ligne.Item("LNIR"), 2))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='IDLAPL'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(5))
            'fileContent.Append(formatItemDate(formatItem_Lg(ligne.Item("IDLAPL"), 5)))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='IRRCOD'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(5))
            'fileContent.Append(formatItem_Lg(ligne.Item("IRRCOD"), 5))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='IIRV'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(5))
            'fileContent.Append(formatItem_Lg(ligne.Item("IIRV"), 5))
            fileContent.AppendLine() ' Append a line break.
            'Next

        End Using
        Return fileContent
    End Function
    ''' <summary>
    ''' function : read table dssat_x_fertilizer and write into fileContent
    ''' </summary>
    ''' <param name="dssat_tableName"></param>
    ''' <param name="dssat_tableId"></param>
    ''' <param name="dssat_tableId_value"></param>
    ''' <param name="fileContent"></param>
    ''' <param name="Connection"></param>
    ''' <returns>fileContent</returns>
    ''' <remarks></remarks>
    Public Function writeBlockFertilizer(ByVal dssat_tableName As String, ByVal dssat_tableId As String, ByVal dssat_tableId_value As String, ByVal fileContent As StringBuilder, ByVal Connection As SqliteConnection, ByVal MI_Connection As SqliteConnection) As StringBuilder

        Dim dssat_queryRead As String = "Select Champ, Default_Value_Datamill, defaultValueOtherSource, IFNULL([defaultValueOtherSource],  [Default_Value_Datamill]) As dv From Variables Where ((model = 'dssat') And ([Table] = '" & dssat_tableName & "'));"
        Dim Bissext As Integer
        Using dataAdapter1 As SqliteDataAdapter = New SqliteDataAdapter(dssat_queryRead, Connection)
            Dim DT As New DataSet()
            Dim rw As DataRow()
            Dim Dv As String
            dataAdapter1.Fill(DT, dssat_tableName)
Dim fetchAllQuery As String = "SELECT SimUnitList.idsim, SimUnitList.StartYear, CropManagement.Sowingdate, InorganicFOperations.IFNumber, " _
                & "InorganicFOperations.N, InorganicFOperations.P, InorganicFOperations.Dferti FROM (InorganicFertilizationPolicy INNER JOIN " _
                & "(CropManagement INNER JOIN SimUnitList ON CropManagement.idMangt = SimUnitList.idMangt) ON InorganicFertilizationPolicy.InorgFertiPolicyCode " _
                & "= CropManagement.InoFertiPolicyCode) INNER JOIN InorganicFOperations ON InorganicFertilizationPolicy.InorgFertiPolicyCode = " _
                & "InorganicFOperations.InorgFertiPolicyCode Where Idsim ='" & dssat_tableId & "' Order by InorganicFOperations.IFNumber;"
            'Init and use DataAdapter
            Dim dataAdapter As SqliteDataAdapter = New SqliteDataAdapter(fetchAllQuery, MI_Connection)
            ' Filling Dataset
            Dim dataSet As New DataSet()
            dataAdapter.Fill(dataSet, "dssat_x_exp")
            Dim dataTable1 As DataTable = dataSet.Tables("dssat_x_exp")

            Dim siteColumnsHeader() As String = {"@F", "FDATE", " FMCD", " FACD", " FDEP", " FAMN", " FAMP", " FAMK", " FAMC", " FAMO", " FOCD", "FERNAME"}

            'header fertilizer
            fileContent.AppendLine() ' Append a line break.
            fileContent.Append("*FERTILIZERS (INORGANIC)")
            fileContent.AppendLine() ' Append a line break.
            'fileContent.Append("@F FDATE  FMCD  FACD  FDEP  FAMN  FAMP  FAMK  FAMC  FAMO  FOCD FERNAME")
            fileContent.Append(String.Join(Chr(32), siteColumnsHeader))
            fileContent.AppendLine() ' Append a line break.

            ''read all line of dssat_x_planting_detail
            For Each occurence As DataRow In dataTable1.Rows

                rw = DT.Tables(0).Select("Champ='LNFER'")
                Dv = rw(0)("dv").ToString
                fileContent.Append(Dv.PadLeft(2))
                'fileContent.Append(formatItem_Lg(occurence.Item("LNFER"), 2))
                fileContent.Append(Chr(32))
                Dim ifert As Integer = occurence.Item("sowingdate") + occurence.Item("Dferti")
                If occurence.Item("Dferti") = 0 Then ifert += 1
                If CInt(occurence.Item("StartYear")) Mod 4 = 0 Then
                    Bissext = 1
                Else
                    Bissext = 0
                End If

                If ifert > 365 + Bissext Then
                    fileContent.Append(Mid((occurence.Item("StartYear") + 1).ToString, 3, 2) & (ifert - 365 - Bissext).ToString.PadLeft(3, "0"))
                Else
                    fileContent.Append(Mid(occurence.Item("StartYear").ToString, 3, 2) & ifert.ToString.PadLeft(3, "0"))
                End If
                fileContent.Append(Chr(32))
                rw = DT.Tables(0).Select("Champ='IFTYPE'")
                Dv = rw(0)("dv").ToString
                fileContent.Append(Dv.PadLeft(5))
                'fileContent.Append(formatItem_Lg(occurence.Item("IFTYPE"), 5))
                fileContent.Append(Chr(32))
                rw = DT.Tables(0).Select("Champ='FERCOD'")
                Dv = rw(0)("dv").ToString
                fileContent.Append(Dv.PadLeft(5))
                'fileContent.Append(formatItem_Lg(occurence.Item("FERCOD"), 5))
                fileContent.Append(Chr(32))
                rw = DT.Tables(0).Select("Champ='DFERT'")
                Dv = rw(0)("dv").ToString
                fileContent.Append(Dv.PadLeft(5))
                'fileContent.Append(formatItem_Lg(occurence.Item("DFERT"), 5))
                fileContent.Append(Chr(32))
                fileContent.Append(occurence.Item("N").ToString.PadLeft(5))
                fileContent.Append(Chr(32))
                fileContent.Append(occurence.Item("P").ToString.PadLeft(5))
                fileContent.Append(Chr(32))
                rw = DT.Tables(0).Select("Champ='AKFER'")
                Dv = rw(0)("dv").ToString
                fileContent.Append(Dv.PadLeft(5))
                'fileContent.Append(formatItem_Lg(occurence.Item("AKFER"), 5))
                fileContent.Append(Chr(32))
                rw = DT.Tables(0).Select("Champ='ACFER'")
                Dv = rw(0)("dv").ToString
                fileContent.Append(Dv.PadLeft(5))
                'fileContent.Append(formatItem_Lg(occurence.Item("ACFER"), 5))
                fileContent.Append(Chr(32))
                rw = DT.Tables(0).Select("Champ='AOFER'")
                Dv = rw(0)("dv").ToString
                fileContent.Append(Dv.PadLeft(5))
                'fileContent.Append(formatItem_Lg(occurence.Item("AOFER"), 5))
                fileContent.Append(Chr(32))
                rw = DT.Tables(0).Select("Champ='FOCOD'")
                Dv = rw(0)("dv").ToString
                fileContent.Append(Dv.PadLeft(5))
                'fileContent.Append(formatItem_Lg(occurence.Item("FOCOD"), 5))
                fileContent.Append(Chr(32))
                rw = DT.Tables(0).Select("Champ='FERNAM'")
                Dv = rw(0)("dv").ToString
                fileContent.Append(Dv.PadLeft(5))
                'fileContent.Append(formatItem_Lg(occurence.Item("FERNAM"), 60))
                fileContent.AppendLine() ' Append a line break.
            Next

        End Using
        Return fileContent
    End Function
    ''' <summary>
    ''' function : read table dssat_x_residues and write into fileContent
    ''' </summary>
    ''' <param name="dssat_tableName"></param>
    ''' <param name="dssat_tableId"></param>
    ''' <param name="dssat_tableId_value"></param>
    ''' <param name="fileContent"></param>
    ''' <param name="Connection"></param>
    ''' <returns>fileContent</returns>
    ''' <remarks></remarks>
    Public Function writeBlockResidues(ByVal dssat_tableName As String, ByVal dssat_tableId As String, ByVal dssat_tableId_value As String, ByVal fileContent As StringBuilder, ByVal Connection As SqliteConnection, ByVal MI_Connection As SqliteConnection) As StringBuilder
        Dim dssat_queryRead As String = "Select Champ, Default_Value_Datamill, defaultValueOtherSource, IFNULL([defaultValueOtherSource],  [Default_Value_Datamill]) As dv From Variables Where ((model = 'dssat') And ([Table] = '" & dssat_tableName & "'));"
        Dim Bissext As Integer
        Using dataAdapter1 As SqliteDataAdapter = New SqliteDataAdapter(dssat_queryRead, Connection)
            Dim DT As New DataSet()
            Dim rw As DataRow()
            Dim Dv As String
            dataAdapter1.Fill(DT, dssat_tableName)
            Dim fetchAllQuery As String = "SELECT SimUnitList.idsim, SimUnitList.StartYear, CropManagement.sowingdate, ListResidues.idresidueDssat, " _
                & "OrganicFOperations.In_OnManure, OrganicFOperations.Qmanure, OrganicFOperations.DFerti, OrganicFOperations.NFerti, " _
                & "OrganicFOperations.PFerti, SoilTillageOperations.STNumber, SoilTillageOperations.DepthResLow, OrganicFOperations.OFNumber " _
                & "FROM ((SoilTillageOperations INNER JOIN CropManagement ON SoilTillageOperations.SoilTillPolicyCode = CropManagement.SoilTillPolicyCode) " _
                & "INNER JOIN SimUnitList ON CropManagement.idMangt = SimUnitList.idMangt) INNER JOIN (ListResidues INNER JOIN OrganicFOperations ON " _
                & "ListResidues.TypeResidues = OrganicFOperations.TypeResidues) ON CropManagement.OFertiPolicyCode = OrganicFOperations.OFertiPolicyCode " _
                & " Where Idsim ='" & dssat_tableId & "' Order by Ofnumber;"
            'Init and use DataAdapter
            Dim dataAdapter As SqliteDataAdapter = New SqliteDataAdapter(fetchAllQuery, MI_Connection)
            ' Filling Dataset
            Dim dataSet As New DataSet()
            dataAdapter.Fill(dataSet, "dssat_x_exp")
            Dim dataTable1 As DataTable = dataSet.Tables("dssat_x_exp")
            Dim siteColumnsHeader() As String = {"@R", "RDATE", " RCOD", " RAMT", " RESN", " RESP", " RESK", " RINP", " RDEP", " RMET", "RENAME"}

            'header residu and organic fertilizer
            fileContent.AppendLine() ' Append a line break.
            fileContent.Append("*RESIDUES AND ORGANIC FERTILIZER")
            fileContent.AppendLine() ' Append a line break.
            'fileContent.Append("@R RDATE  RCOD  RAMT  RESN  RESP  RESK  RINP  RDEP  RMET RENAME")
            fileContent.Append(String.Join(Chr(32), siteColumnsHeader))
            fileContent.AppendLine() ' Append a line break.

            ''read all line of dssat_x_residues
            For Each occurence As DataRow In dataTable1.Rows

                rw = DT.Tables(0).Select("Champ='LNRES'")
                Dv = rw(0)("dv").ToString
                                
                fileContent.Append(Dv.PadLeft(2))
                'fileContent.Append(formatItem_Lg(occurence.Item("LNRES"), 2))
                fileContent.Append(Chr(32))
                Dim ifert As Integer = occurence.Item("sowingdate") + occurence.Item("Dferti")
                If occurence.Item("Dferti") = 0 Then ifert += 1
                If CInt(occurence.Item("StartYear")) Mod 4 = 0 Then
                    Bissext = 1
                Else
                    Bissext = 0
                End If

                If ifert > 365 + Bissext Then
                    fileContent.Append(Mid((occurence.Item("StartYear") + 1).ToString, 3, 2) & (ifert - 365 - Bissext).ToString.PadLeft(3, "0"))
                Else
                    fileContent.Append(Mid(occurence.Item("StartYear").ToString, 3, 2) & ifert.ToString.PadLeft(3, "0"))
                End If
                '            fileContent.Append(Mid(dataTable.Rows(0).Item("StartYear").ToString, 3, 2) & ifert.ToString.PadLeft(3, "0"))
                'fileContent.Append(formatItemDate(formatItem_Lg(occurence.Item("RESDAY"), 5)))
                fileContent.Append(Chr(32))
                'rw = DT.Tables(0).Select("Champ='RESCOD'")
                'Dv = rw(0)("dv").ToString
                'fileContent.Append(Dv.PadLeft(5))
                'fileContent.Append(formatItem_Lg(occurence.Item("RESCOD"), 5))
                fileContent.Append(occurence.Item("idresidueDssat").ToString.PadLeft(5))
                fileContent.Append(Chr(32))
                fileContent.Append(occurence.Item("Qmanure").ToString.PadLeft(5))
                fileContent.Append(Chr(32))
                fileContent.Append(Round((100 * occurence.Item("Nferti")), 2).ToString.PadLeft(5))
                fileContent.Append(Chr(32))
                fileContent.Append((100 * occurence.Item("PFerti")).ToString.PadLeft(5))
                fileContent.Append(Chr(32))
                rw = DT.Tables(0).Select("Champ='RESK'")
                Dv = rw(0)("dv").ToString
                fileContent.Append(Dv.PadLeft(5))
                'fileContent.Append(formatItem_Lg(occurence.Item("RESK"), 5))
                fileContent.Append(Chr(32))
                rw = DT.Tables(0).Select("Champ='RINP'")
                Dv = rw(0)("dv").ToString
                fileContent.Append(Dv.PadLeft(5))
                'fileContent.Append(formatItem_Lg(occurence.Item("RINP"), 5))
                fileContent.Append(Chr(32))
                'rw = DT.Tables(0).Select("Champ='DEPRES'")
                'Dv = rw(0)("dv").ToString
                If LCase(occurence.Item("in_onManure")) = "on" Then
                    fileContent.Append("0.0".PadLeft(5))
                Else
                    fileContent.Append(occurence.Item("DepthResLow").ToString.PadLeft(5))
                End If
                'fileContent.Append(formatItem_Lg(occurence.Item("DEPRES"), 5))
                fileContent.Append(Chr(32))
                rw = DT.Tables(0).Select("Champ='RMET'")
                Dv = rw(0)("dv").ToString
                fileContent.Append(Dv.PadLeft(5))
                'fileContent.Append(formatItem_Lg(occurence.Item("RMET"), 5))
                fileContent.Append(Chr(32))
                rw = DT.Tables(0).Select("Champ='RENAME'")
                Dv = rw(0)("dv").ToString
                fileContent.Append(Dv.PadLeft(60))
                'fileContent.Append(formatItem_Lg(occurence.Item("RENAME"), 60))
                fileContent.AppendLine() ' Append a line break.
            Next

        End Using
        Return fileContent
    End Function
    ''' <summary>
    '''  function : read table dssat_x_chemical_application and write into fileContent
    ''' </summary>
    ''' <param name="dssat_tableName"></param>
    ''' <param name="dssat_tableId"></param>
    ''' <param name="dssat_tableId_value"></param>
    ''' <param name="fileContent"></param>
    ''' <param name="Connection"></param>
    ''' <returns>fileContent</returns>
    ''' <remarks></remarks>
    Public Function writeBlockChemicalApplication(ByVal dssat_tableName As String, ByVal dssat_tableId As String, ByVal dssat_tableId_value As String, ByVal fileContent As StringBuilder, ByVal Connection As SqliteConnection) As StringBuilder
        Dim dssat_queryRead As String = "Select Champ, Default_Value_Datamill, defaultValueOtherSource, IFNULL([defaultValueOtherSource],  [Default_Value_Datamill]) As dv From Variables Where ((model = 'dssat') And ([Table] = '" & dssat_tableName & "'));"
        Using dataAdapter1 As SqliteDataAdapter = New SqliteDataAdapter(dssat_queryRead, Connection)
            Dim DT As New DataSet()
            Dim rw As DataRow()
            Dim Dv As String
            dataAdapter1.Fill(DT, dssat_tableName)

            Dim siteColumnsHeader() As String = {"@C", "CDATE", "CHCOD", "CHAMT", " CHME", "CHDEP", "  CHT", ".CHNAME"}

            'header chemical application
            fileContent.AppendLine() ' Append a line break.
            fileContent.Append("*CHEMICAL APPLICATIONS")
            fileContent.AppendLine() ' Append a line break.
            'fileContent.Append("@C CDATE CHCOD CHAMT  CHME CHDEP   CHT..CHNAME")
            fileContent.Append(String.Join(Chr(32), siteColumnsHeader))
            fileContent.AppendLine() ' Append a line break.

            ''read all line of dssat_x_residues
            'For Each occurence As DataRow In dataTable1.Rows

            rw = DT.Tables(0).Select("Champ='LNCHE'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(2))
            'fileContent.Append(formatItem_Lg(occurence.Item("LNCHE"), 2))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='CDATE'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(5, "0"))
            'fileContent.Append(formatItemDate(formatItem_Lg(occurence.Item("CDATE"), 5)))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='CHCOD'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadRight(5))
            'fileContent.Append(formatItem_Lg(occurence.Item("CHCOD"), 5))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='CHAMT'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadRight(5))
            'fileContent.Append(formatItem_Lg(occurence.Item("CHAMT"), 5))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='CHMET'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadRight(5))
            'fileContent.Append(formatItem_Lg(occurence.Item("CHMET"), 5))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='CHDEP'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadRight(5))
            'fileContent.Append(formatItem_Lg(occurence.Item("CHDEP"), 5))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='CHT'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadRight(5))
            'fileContent.Append(formatItem_Lg(occurence.Item("CHT"), 5))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='CHNAME'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv)
            'fileContent.Append(formatItem_Lg(occurence.Item("CHNAME"), 60))
            fileContent.AppendLine() ' Append a line break.
            'Next
        End Using
        Return fileContent
    End Function
    ''' <summary>
    '''  function : read table dssat_x_tillage and write into fileContent
    ''' </summary>
    ''' <param name="dssat_tableName"></param>
    ''' <param name="dssat_tableId"></param>
    ''' <param name="dssat_tableId_value"></param>
    ''' <param name="fileContent"></param>
    ''' <param name="Connection"></param>
    ''' <returns>fileContent</returns>
    ''' <remarks></remarks>
    Public Function writeBlockTillageRotation(ByVal dssat_tableName As String, ByVal dssat_tableId As String, ByVal dssat_tableId_value As String, ByVal fileContent As StringBuilder, ByVal Connection As SqliteConnection, ByVal MI_Connection As SqliteConnection) As StringBuilder
        Dim dssat_queryRead As String = "Select Champ, Default_Value_Datamill, defaultValueOtherSource, IFNULL([defaultValueOtherSource], [Default_Value_Datamill]) As dv From Variables Where ((model = 'dssat') And ([Table] = '" & dssat_tableName & "'));"
        Dim Bissext As Integer
        Using dataAdapter1 As SqliteDataAdapter = New SqliteDataAdapter(dssat_queryRead, Connection)
            Dim DT As New DataSet()
            Dim rw As DataRow()
            Dim Dv As String
            dataAdapter1.Fill(DT, dssat_tableName)

            Dim siteColumnsHeader() As String = {"@T", "TDATE", "TIMPL", " TDEP", "TNAME"}
            Dim fetchAllQuery As String = "SELECT SimUnitList.idsim, SimUnitList.StartYear, CropManagement.sowingdate, OrganicFOperations.Qmanure, OrganicFOperations.DFerti, OrganicFOperations.NFerti, OrganicFOperations.PFerti, SoilTillageOperations.STNumber, SoilTillageOperations.DStill, SoilTillageOperations.DepthResLow " _
                & "FROM SoilTillageOperations INNER JOIN ((OrganicFertilizationPolicy INNER JOIN (CropManagement INNER JOIN SimUnitList ON CropManagement.idMangt = SimUnitList.idMangt) " _
                & "ON OrganicFertilizationPolicy.OFertiPolicyCode = CropManagement.OFertiPolicyCode) INNER JOIN OrganicFOperations ON OrganicFertilizationPolicy.OFertiPolicyCode = " _
                & "OrganicFOperations.OFertiPolicyCode) ON SoilTillageOperations.SoilTillPolicyCode = CropManagement.SoilTillPolicyCode Where Idsim ='" & dssat_tableId & "';"
            'Init and use DataAdapter
            Dim dataAdapter As SqliteDataAdapter = New SqliteDataAdapter(fetchAllQuery, MI_Connection)
            ' Filling Dataset
            Dim dataSet As New DataSet()
            dataAdapter.Fill(dataSet, "dssat_x_exp")
            Dim dataTable As DataTable = dataSet.Tables("dssat_x_exp")
            'header titillage and rotation
            fileContent.AppendLine() ' Append a line break.
            fileContent.Append("*TILLAGE AND ROTATIONS")
            fileContent.AppendLine() ' Append a line break.
            'fileContent.Append("@T TDATE TIMPL  TDEP TNAME")
            fileContent.Append(String.Join(Chr(32), siteColumnsHeader))
            fileContent.AppendLine() ' Append a line break.

            ''read all line of dssat_x_residues
            'For Each occurence As DataRow In dataTable1.Rows

            rw = DT.Tables(0).Select("Champ='LNTIL'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(2))
            'fileContent.Append(formatItem_Lg(occurence.Item("LNTIL"), 2))
            fileContent.Append(Chr(32))
            Dim ifert As Integer = dataTable.Rows(0).Item("sowingdate") + dataTable.Rows(0).Item("Dstill")
            'If dataTable.Rows(0).Item("Dstill") = 0 Then ifert += 1
            If CInt(dataTable.Rows(0).Item("StartYear")) Mod 4 = 0 Then
                Bissext = 1
            Else
                Bissext = 0
            End If

            If ifert > 365 + Bissext Then
                fileContent.Append(Mid((dataTable.Rows(0).Item("StartYear") + 1).ToString, 3, 2) & (ifert - 365 - Bissext).ToString.PadLeft(3, "0"))
            Else
                fileContent.Append(Mid(dataTable.Rows(0).Item("StartYear").ToString, 3, 2) & ifert.ToString.PadLeft(3, "0"))
            End If
            'rw = DT.Tables(0).Select("Champ='TDATE'")
            'Dv = rw(0)("dv").ToString
            'fileContent.Append(ifert.ToString.PadLeft(5, "0"))
            'fileContent.Append(formatItemDate(formatItem_Lg(occurence.Item("TDATE"), 5)))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='TIMPL'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(5))
            'fileContent.Append(formatItem_Lg(occurence.Item("TIMPL"), 5))
            fileContent.Append(Chr(32))
            'rw = DT.Tables(0).Select("Champ='TDEP'")
            'Dv = rw(0)("dv").ToString
            fileContent.Append(dataTable.Rows(0).Item("DepthResLow").ToString.PadLeft(5))
            'fileContent.Append(formatItem_Lg(occurence.Item("TDEP"), 5))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='TNAME'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(60))
            'fileContent.Append(formatItem_Lg(occurence.Item("TNAME"), 60))
            fileContent.AppendLine() ' Append a line break.
            'Next
        End Using
        Return fileContent
    End Function
    ''' <summary>
    '''  function : read table dssat_x_environment and write into fileContent
    ''' </summary>
    ''' <param name="dssat_tableName"></param>
    ''' <param name="dssat_tableId"></param>
    ''' <param name="dssat_tableId_value"></param>
    ''' <param name="fileContent"></param>
    ''' <param name="Connection"></param>
    ''' <returns>fileContent</returns>
    ''' <remarks></remarks>
    Public Function writeBlockEnvironment(ByVal dssat_tableName As String, ByVal dssat_tableId As String, ByVal dssat_tableId_value As String, ByVal fileContent As StringBuilder, ByVal Connection As SqliteConnection) As StringBuilder
        Dim dssat_queryRead As String = "Select Champ, Default_Value_Datamill, defaultValueOtherSource, IFNULL([defaultValueOtherSource],  [Default_Value_Datamill]) As dv From Variables Where ((model = 'dssat') And ([Table] = '" & dssat_tableName & "'));"
        Using dataAdapter1 As SqliteDataAdapter = New SqliteDataAdapter(dssat_queryRead, Connection)
            Dim DT As New DataSet()
            Dim rw As DataRow()
            Dim Dv As String
            dataAdapter1.Fill(DT, dssat_tableName)
            Dim siteColumnsHeader() As String = {"@E", "ODATE", "EDAY ", "ERAD ", "EMAX ", "EMIN ", "ERAIN", "ECO2 ", "EDEW ", "EWIND", "ENVNAME"}

            'header environnement
            fileContent.AppendLine() ' Append a line break.
            fileContent.Append("*ENVIRONMENT MODIFICATIONS")
            fileContent.AppendLine() ' Append a line break.
            'fileContent.Append("@E ODATE EDAY  ERAD  EMAX  EMIN  ERAIN ECO2  EDEW  EWIND ENVNAME")
            fileContent.Append(String.Join(Chr(32), siteColumnsHeader))
            fileContent.AppendLine() ' Append a line break.

            ''read all line of dssat_x_environment
            'For Each occurence As DataRow In dataTable1.Rows

            rw = DT.Tables(0).Select("Champ='LNENV'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(2))
            'fileContent.Append(formatItem_Lg(occurence.Item("LNENV"), 2))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='WMDATE'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(5, "0"))
            'fileContent.Append(formatItemDate(formatItem_Lg(occurence.Item("WMDATE"), 5)))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='DAYFAC'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(1))
            'fileContent.Append(formatItem_Lg(occurence.Item("DAYFAC"), 1))
            rw = DT.Tables(0).Select("Champ='DAYADJ'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(4))
            'fileContent.Append(formatItem_Lg(occurence.Item("DAYADJ"), 4))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='RADFAC'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(1))
            'fileContent.Append(formatItem_Lg(occurence.Item("RADFAC"), 1))
            rw = DT.Tables(0).Select("Champ='RADADJ'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(4))
            'fileContent.Append(formatItem_Lg(occurence.Item("RADADJ"), 4))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='TXFAC'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(1))
            'fileContent.Append(formatItem_Lg(occurence.Item("TXFAC"), 1))
            rw = DT.Tables(0).Select("Champ='TXADJ'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(4))
            'fileContent.Append(formatItem_Lg(occurence.Item("TXADJ"), 4))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='TMFAC'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(1))
            'fileContent.Append(formatItem_Lg(occurence.Item("TMFAC"), 1))
            rw = DT.Tables(0).Select("Champ='TMADJ'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(4))
            'fileContent.Append(formatItem_Lg(occurence.Item("TMADJ"), 4))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='PRCFAC'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(1))
            'fileContent.Append(formatItem_Lg(occurence.Item("PRCFAC"), 1))
            rw = DT.Tables(0).Select("Champ='PRCADJ'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(4))
            'fileContent.Append(formatItem_Lg(occurence.Item("PRCADJ"), 4))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='CO2FAC'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(1))
            'fileContent.Append(formatItem_Lg(occurence.Item("CO2FAC"), 1))
            rw = DT.Tables(0).Select("Champ='CO2ADJ'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(4))
            'fileContent.Append(formatItem_Lg(occurence.Item("CO2ADJ"), 4))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='DPTFAC'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(1))
            'fileContent.Append(formatItem_Lg(occurence.Item("DPTFAC"), 1))
            rw = DT.Tables(0).Select("Champ='DPTADJ'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(4))
            'fileContent.Append(formatItem_Lg(occurence.Item("DPTADJ"), 4))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='WNDFAC'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(1))
            'fileContent.Append(formatItem_Lg(occurence.Item("WNDFAC"), 1))
            rw = DT.Tables(0).Select("Champ='WNDADJ'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(4))
            '    fileContent.Append(formatItem_Lg(occurence.Item("WNDADJ"), 4))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='ENVNAME'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv)
            'fileContent.Append(formatItem_Lg(occurence.Item("ENVNAME"), 60))
            fileContent.AppendLine() ' Append a line break.
            'Next
        End Using
        Return fileContent
    End Function
    ''' <summary>
    '''  function : read table dssat_x_harvest and write into fileContent
    ''' </summary>
    ''' <param name="dssat_tableName"></param>
    ''' <param name="dssat_tableId"></param>
    ''' <param name="dssat_tableId_value"></param>
    ''' <param name="fileContent"></param>
    ''' <param name="Connection"></param>
    ''' <returns>fileContent</returns>
    ''' <remarks></remarks>
    Public Function writeBlockHarvest(ByVal dssat_tableName As String,  ByVal dssat_tableId As String, ByVal dssat_tableId_value As String, ByVal fileContent As StringBuilder, ByVal Connection As SqliteConnection, ByVal MI_Connection As SqliteConnection) As StringBuilder

        Dim dssat_queryRead As String = "Select Champ, Default_Value_Datamill, defaultValueOtherSource, IFNULL([defaultValueOtherSource],  [Default_Value_Datamill]) As dv From Variables Where ((model = 'dssat') And ([Table] = '" & dssat_tableName & "' or [Table] = 'dssat_x_simulation_management'));"
        Using dataAdapter1 As SqliteDataAdapter = New SqliteDataAdapter(dssat_queryRead, Connection)
            Dim DT As New DataSet()
            Dim rw As DataRow()
            Dim Dv As String
            dataAdapter1.Fill(DT, dssat_tableName)
            Dim siteColumnsHeader() As String = {"@H", "HDATE", " HSTG", " HCOM", "HSIZE", "  HPC", " HBPC", "HNAME"}

            Dim fetchAllQuery As String = "SELECT SimUnitList.idsim, SimUnitList.EndYear,SimUnitList.EndDay FROM SimUnitList  Where Idsim ='" & dssat_tableId & "';"
            Dim dataAdapter As SqliteDataAdapter = New SqliteDataAdapter(fetchAllQuery, MI_Connection)
            Dim dataSet As New DataSet()
            dataAdapter.Fill(dataSet, "dssat_x_exp")
            Dim dataTable As DataTable = dataSet.Tables("dssat_x_exp")

            'header harvest
            fileContent.AppendLine() ' Append a line break.
            fileContent.Append("*HARVEST DETAILS")
            fileContent.AppendLine() ' Append a line break.
            'fileContent.Append("@H HDATE  HSTG  HCOM HSIZE   HPC  HBPC HNAME")
            fileContent.Append(String.Join(Chr(32), siteColumnsHeader))
            fileContent.AppendLine() ' Append a line break.

            ''read all line of dssat_x_harvest
            'For Each occurence As DataRow In dataTable1.Rows

            rw = DT.Tables(0).Select("Champ='LNHAR'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(2))
            'fileContent.Append(formatItem_Lg(occurence.Item("LNHAR"), 2))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='IHARI'")
            Dv = rw(0)("dv").ToString
            'If Dv = "R" Then
            fileContent.Append(Mid(dataTable.Rows(0).Item("Endyear").ToString, 3, 2) & dataTable.Rows(0).Item("EndDay").ToString.PadLeft(3, "0"))
            'Else
                'rw = DT.Tables(0).Select("Champ='HDATE'")
                'Dv = rw(0)("dv").ToString
                'fileContent.Append(Dv.PadLeft(5, "0"))
            'End If
            'fileContent.Append(formatItemDate(formatItem_Lg(occurence.Item("HDATE"), 5)))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='HTSG'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadRight(5))
            'fileContent.Append(formatItem_Lg(occurence.Item("HTSG"), 5))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='HCOM'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadRight(5))
            'fileContent.Append(formatItem_Lg(occurence.Item("HCOM"), 5))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='HSIZ'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadRight(5))
            'fileContent.Append(formatItem_Lg(occurence.Item("HSIZ"), 5))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='HPC'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadRight(5))
            'fileContent.Append(formatItem_Lg(occurence.Item("HPC"), 5))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='HBPC'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadRight(5))
            'fileContent.Append(formatItem_Lg(occurence.Item("HBPC"), 5))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='HNAME'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv)
            'fileContent.Append(formatItem_Lg(occurence.Item("HNAME"), 60))
            fileContent.AppendLine() ' Append a line break.
            'Next
        End Using
        Return fileContent
    End Function

    ''' <summary>
    ''' function : writeBlockEndFile(fileContent, Connection)
    ''' calling function to read all the table of simulation and automatic
    ''' </summary>
    ''' <param name="fileContent"></param>
    ''' <param name="Connection"></param>
    ''' <returns>fileContent</returns>
    ''' <remarks></remarks>
    Public Function writeBlockEndFile(ByVal fileContent As StringBuilder, ByVal dssat_tableId_value As String, ByVal Connection As SqliteConnection, ByVal MI_Connection As SqliteConnection) As StringBuilder
        storeKeyDataN = 0
        'read loop table simulation and automatic
        storeNumMaxSimu = 1 'cyrille
        While storeNumMaxSimu > storeKeyDataN
            'add 1 to storeKeyDataN

            storeKeyDataN = storeKeyDataN + 1
            'table dssat_x_simulation_general
            Dim dssat_tableId1 = "dssat_x_exp_id"
            Dim dssat_tableName1 As String = "dssat_x_simulation_general"

            writeBlockGeneral(dssat_tableName1, dssat_tableId1, dssat_tableId_value, fileContent, Connection, MI_Connection)


            'dssat_tableId_value = storeKeySimuOption + 1
            dssat_tableId1 = "dssat_x_exp_id"
            dssat_tableName1 = "dssat_x_simulation_option"
            writeBlockOption(dssat_tableName1, dssat_tableId1, dssat_tableId_value, fileContent, Connection, MI_Connection)

            'dssat_tableId_value = storeKeySimuMethod + 1
            dssat_tableId1 = "dssat_x_exp_id"
            dssat_tableName1 = "dssat_x_simulation_method"
            writeBlockMethod(dssat_tableName1, dssat_tableId1, dssat_tableId_value, fileContent, Connection)

            'dssat_tableId_value = storeKeySimuManagement + 1
            dssat_tableId1 = "dssat_x_exp_id"
            dssat_tableName1 = "dssat_x_simulation_management"
            writeBlockManagement(dssat_tableName1, dssat_tableId1, dssat_tableId_value, fileContent, Connection)

            'dssat_tableId_value = storeKeySimuOutput + 1
            dssat_tableId1 = "dssat_x_exp_id"
            dssat_tableName1 = "dssat_x_simulation_outputs"
            writeBlockoutputs(dssat_tableName1, dssat_tableId1, dssat_tableId_value, fileContent, Connection)

            'empty line
            fileContent.AppendLine() ' Append a line break.
            fileContent.Append("@  AUTOMATIC MANAGEMENT")
            fileContent.AppendLine() ' Append a line break.


            'dssat_tableId_value = storeKeySimuOption + 1
            dssat_tableId1 = "dssat_x_exp_id"
            dssat_tableName1 = "dssat_x_automatic_planting"
            writeBlockAutomaticPlanting(dssat_tableName1, dssat_tableId1, dssat_tableId_value, fileContent, Connection)

            'dssat_tableId_value = storeKeySimuOption + 1
            dssat_tableId1 = "dssat_x_exp_id"
            dssat_tableName1 = "dssat_x_automatic_irrigation"
            writeBlockAutomaticIrrigation(dssat_tableName1, dssat_tableId1, dssat_tableId_value, fileContent, Connection)

            'dssat_tableId_value = storeKeySimuOption + 1
            dssat_tableId1 = "dssat_x_exp_id"
            dssat_tableName1 = "dssat_x_automatic_nitrogen"
            writeBlockAutomaticNitrogen(dssat_tableName1, dssat_tableId1, dssat_tableId_value, fileContent, Connection)

            'dssat_tableId_value = storeKeySimuOption + 1
            dssat_tableId1 = "dssat_x_exp_id"
            dssat_tableName1 = "dssat_x_automatic_residues"
            writeBlockAutomaticResidue(dssat_tableName1, dssat_tableId1, dssat_tableId_value, fileContent, Connection)

            'dssat_tableId_value = storeKeySimuOption + 1
            dssat_tableId1 = "dssat_x_exp_id"
            dssat_tableName1 = "dssat_x_automatic_harvest"
            writeBlockAutomaticHarvest(dssat_tableName1, dssat_tableId1, dssat_tableId_value, fileContent, Connection, MI_Connection)

            'empty line
            fileContent.AppendLine() ' Append a line break.
            'Console.WriteLine("length of file content : " & fileContent.Length)
        End While

        Return fileContent
    End Function

    ''' <summary>
    '''  function : read table dssat_x_simulation_general and write into fileContent
    ''' </summary>
    ''' <param name="dssat_tableName"></param>
    ''' <param name="dssat_tableId"></param>
    ''' <param name="dssat_tableId_value"></param>
    ''' <param name="fileContent"></param>
    ''' <param name="Connection"></param>
    ''' <returns>fileContent</returns>
    ''' <remarks></remarks>
    Public Function writeBlockGeneral(ByVal dssat_tableName As String, ByVal dssat_tableId As String, ByVal dssat_tableId_value As String, ByVal fileContent As StringBuilder, ByVal Connection As SqliteConnection, ByVal MI_Connection As SqliteConnection) As StringBuilder

        'Dim dssat_queryRead As String = "select * from " + dssat_tableName + " where " + dssat_tableId + " = " + dssat_tableId_value + " and LNSIM = " + storeKeyDataN.ToString + " order by " + dssat_tableId + " asc, LNSIM asc ;"
        Dim siteColumnsHeader() As String = {"@N", "GENERAL    ", "NYERS", "NREPS", "START", "SDATE", "RSEED", "SNAME....................", "SMODEL"}
        Dim Bissext As Integer
        Dim fetchAllQuery As String = "SELECT SimUnitList.idsim, SimUnitList.StartYear,SimUnitList.StartDay, CropManagement.Sowingdate FROM CropManagement INNER JOIN SimUnitList ON CropManagement.idMangt = SimUnitList.idMangt WHERE Idsim ='" & dssat_tableId_value & "';"
        'Init and use DataAdapter
        Dim dataAdapter As SqliteDataAdapter = New SqliteDataAdapter(fetchAllQuery, MI_Connection)
        ' Filling Dataset
        Dim dataSet As New DataSet()
        dataAdapter.Fill(dataSet, "dssat_x_exp")
        Dim dataTable As DataTable = dataSet.Tables("dssat_x_exp")

        Dim dssat_queryRead As String = "Select Champ, Default_Value_Datamill, defaultValueOtherSource, IFNULL([defaultValueOtherSource],  [Default_Value_Datamill]) As dv From Variables Where ((model = 'dssat') And ([Table] = '" & dssat_tableName & "'));"
        Using dataAdapter1 As SqliteDataAdapter = New SqliteDataAdapter(dssat_queryRead, Connection)
            Dim DT As New DataSet()
            Dim rw As DataRow()
            Dim Dv As String
            dataAdapter1.Fill(DT, dssat_tableName)
            fileContent.Append(String.Join(Chr(32), siteColumnsHeader))
            fileContent.AppendLine() ' Append a line break.

            ''read all line of dssat_x_simulation_general
            'For Each occurence As DataRow In dataTable.Rows

            rw = DT.Tables(0).Select("Champ='LNSIM'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(2))
            'fileContent.Append(formatItem_Lg(occurence.Item("LNSIM"), 2))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='TITCOM'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(11))
            'fileContent.Append(formatItem_Lg(occurence.Item("TITCOM"), 11))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='NYRS'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(5))
            'fileContent.Append(formatItem_Lg(occurence.Item("NYRS"), 5))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='NREPSQ'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(5))
            'fileContent.Append(formatItem_Lg(occurence.Item("NREPSQ"), 5))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='ISIMI'")
            Dv = Trim(rw(0)("dv").ToString)
            fileContent.Append(Dv.PadLeft(5))
            'If CInt(dataTable.Rows(0).Item("StartYear")) Mod 4 = 0 Then
            '    Bissext = 1
            'Else
            '    Bissext = 0
            'End If

            'If CInt(dataTable.Rows(0).Item("StartDay")) > 365 + Bissext Then
            '    fileContent.Append(Mid((CInt(dataTable.Rows(0).Item("StartYear")) + 1).ToString, 3, 2) & (CInt(dataTable.Rows(0).Item("StartDay")) - 365 - Bissext).ToString.PadLeft(3, "0"))
            'Else
            '    fileContent.Append(Mid(dataTable.Rows(0).Item("StartYear").ToString, 3, 2) & dataTable.Rows(0).Item("StartDay").ToString.PadLeft(3, "0"))
            'End If
            'fileContent.Append(Right_Justified(formatItem_Lg(occurence.Item("ISIMI"), 5)))
            fileContent.Append(Chr(32))
            'rw = DT.Tables(0).Select("Champ='YRSIM'")
            'Dv = rw(0)("dv").ToString
            If CInt(dataTable.Rows(0).Item("StartYear")) Mod 4 = 0 Then
                Bissext = 1
            Else
                Bissext = 0
            End If

            If CInt(dataTable.Rows(0).Item("StartDay")) > 365 + Bissext Then
                fileContent.Append(Mid((CInt(dataTable.Rows(0).Item("StartYear")) + 1).ToString, 3, 2) & (CInt(dataTable.Rows(0).Item("StartDay")) - 365 - Bissext).ToString.PadLeft(3, "0"))
            Else
                fileContent.Append(Mid(dataTable.Rows(0).Item("StartYear").ToString, 3, 2) & dataTable.Rows(0).Item("StartDay").ToString.PadLeft(3, "0"))
            End If
            'fileContent.Append(formatItemDate(formatItem_Lg(occurence.Item("YRSIM"), 5)))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='RSEED'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(5))
            'fileContent.Append(formatItem_Lg(occurence.Item("RSEED"), 5))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='TITSIM'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv)
            'fileContent.Append(formatItem_Lg(occurence.Item("TITSIM"), 25))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='CROP_MODE'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv)
            'fileContent.Append(formatItem_Lg(occurence.Item("CROP_MODE"), 20))
            fileContent.AppendLine() ' Append a line break.

            'Next
        End Using

        Return fileContent

    End Function
    ''' <summary>
    ''' function : read table dssat_x_simulation_option and write into fileContent
    ''' </summary>
    ''' <param name="dssat_tableName"></param>
    ''' <param name="dssat_tableId"></param>
    ''' <param name="dssat_tableId_value"></param>
    ''' <param name="fileContent"></param>
    ''' <param name="Connection"></param>
    ''' <returns>fileContent</returns>
    ''' <remarks></remarks>
    Public Function writeBlockOption(ByVal dssat_tableName As String, ByVal dssat_tableId As String, ByVal dssat_tableId_value As String, ByVal fileContent As StringBuilder, ByVal Connection As SqliteConnection, ByVal MI_Connection As SqliteConnection) As StringBuilder

        'Dim dssat_queryRead As String = "select * from " + dssat_tableName + " where " + dssat_tableId + " = " + dssat_tableId_value + " and LNSIM = " + storeKeyDataN.ToString + " order by " + dssat_tableId + " asc, LNSIM asc ;"
        Dim siteColumnsHeader() As String = {"@N", "OPTIONS    ", "WATER", "NITRO", "SYMBI", "PHOSP", "POTAS", "DISES", " CHEM", " TILL", "  CO2"}
        Dim fetchAllQuery As String = "SELECT SimUnitList.idsim, SimulationOptions.StressW_YN, SimulationOptions.StressN_YN, SimulationOptions.StressP_YN, SimulationOptions.StressK_YN " _
        & "FROM SimUnitList INNER JOIN SimulationOptions ON SimUnitList.IdOption = SimulationOptions.IdOptions Where idsim ='" + dssat_tableId_value + "';"
        Dim DA = New SqliteDataAdapter(fetchAllQuery, MI_Connection)
        Dim row As DataRow
        'MsgBox(fetchAllQuery)
        ' Filling Dataset
        Dim DSTrav As New DataSet()
        DA.Fill(DSTrav)
        Dim DTable As DataTable = DSTrav.Tables(0)
        row = DTable.Rows(0)

        Dim dssat_queryRead As String = "Select Champ, Default_Value_Datamill, defaultValueOtherSource, IFNULL([defaultValueOtherSource],  [Default_Value_Datamill]) As dv From Variables Where ((model = 'dssat') And ([Table] = '" & dssat_tableName & "'));"
        Using dataAdapter1 As SqliteDataAdapter = New SqliteDataAdapter(dssat_queryRead, Connection)
            Dim DT As New DataSet()
            Dim rw As DataRow()
            Dim Dv As String
            dataAdapter1.Fill(DT, dssat_tableName)

            fileContent.Append(String.Join(Chr(32), siteColumnsHeader))
            fileContent.AppendLine() ' Append a line break.

            ''read all line of dssat_x_simulation_option
            'For Each occurence As DataRow In dataTable.Rows

            rw = DT.Tables(0).Select("Champ='LNSIM'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(2))
            'fileContent.Append(formatItem_Lg(occurence.Item("LNSIM"), 2))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='TITOPT'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(11))
            'fileContent.Append(formatItem_Lg(occurence.Item("TITOPT"), 11))
            fileContent.Append(Chr(32))
            'rw = DT.Tables(0).Select("Champ='ISWWAT'")
            If row.Item("StressW_YN") Then
                fileContent.Append("Y".PadLeft(5))
            Else
                fileContent.Append("N".PadLeft(5))
            End If
            'Dv = rw(0)("dv").ToString
            'fileContent.Append(Dv.PadLeft(5))
            'fileContent.Append(Right_Justified(formatItem_Lg(occurence.Item("ISWWAT"), 5)))
            fileContent.Append(Chr(32))
            If row.Item("StressN_YN") Then
                fileContent.Append("Y".PadLeft(5))
            Else
                fileContent.Append("N".PadLeft(5))
            End If
            'rw = DT.Tables(0).Select("Champ='ISWNIT'")
            'Dv = rw(0)("dv").ToString
            'fileContent.Append(Dv.PadLeft(5))
            'fileContent.Append(Right_Justified(formatItem_Lg(occurence.Item("ISWNIT"), 5)))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='ISWSYM'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(5))
            'fileContent.Append(Right_Justified(formatItem_Lg(occurence.Item("ISWSYM"), 5)))
            fileContent.Append(Chr(32))
            If row.Item("StressP_YN") Then
                fileContent.Append("Y".PadLeft(5))
            Else
                fileContent.Append("N".PadLeft(5))
            End If
            'rw = DT.Tables(0).Select("Champ='ISWPHO'")
            'Dv = rw(0)("dv").ToString
            'fileContent.Append(Dv.PadLeft(5))
            'fileContent.Append(Right_Justified(formatItem_Lg(occurence.Item("ISWPHO"), 5)))
            fileContent.Append(Chr(32))
            If row.Item("StressK_YN") Then
                fileContent.Append("Y".PadLeft(5))
            Else
                fileContent.Append("N".PadLeft(5))
            End If
            'rw = DT.Tables(0).Select("Champ='ISWPOT'")
            'Dv = rw(0)("dv").ToString
            'fileContent.Append(Dv.PadLeft(5))
            'fileContent.Append(Right_Justified(formatItem_Lg(occurence.Item("ISWPOT"), 5)))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='ISWDIS'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(5))
            'fileContent.Append(Right_Justified(formatItem_Lg(occurence.Item("ISWDIS"), 5)))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='ISCHEM'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(5))
            'fileContent.Append(Right_Justified(formatItem_Lg(occurence.Item("ISCHEM"), 5)))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='ISTILL'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(5))
            'fileContent.Append(Right_Justified(formatItem_Lg(occurence.Item("ISTILL"), 5)))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='ISCO2'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(5))
            'fileContent.Append(Right_Justified(formatItem_Lg(occurence.Item("ISCO2"), 5)))
            fileContent.AppendLine() ' Append a line break.
            'Exit For
            'Next

        End Using
        Return fileContent

    End Function
    ''' <summary>
    ''' function : read table dssat_x_simulation_method and write into fileContent
    ''' </summary>
    ''' <param name="dssat_tableName"></param>
    ''' <param name="dssat_tableId"></param>
    ''' <param name="dssat_tableId_value"></param>
    ''' <param name="fileContent"></param>
    ''' <param name="Connection"></param>
    ''' <returns>fileContent</returns>
    ''' <remarks></remarks>
    Public Function writeBlockMethod(ByVal dssat_tableName As String, ByVal dssat_tableId As String, ByVal dssat_tableId_value As String, ByVal fileContent As StringBuilder, ByVal Connection As SqliteConnection) As StringBuilder

        'Dim dssat_queryRead As String = "select * from " + dssat_tableName + " where " + dssat_tableId + " = " + dssat_tableId_value + " and LNSIM = " + storeKeyDataN.ToString + " order by " + dssat_tableId + " asc, LNSIM asc ;"
        Dim siteColumnsHeader() As String = {"@N", "METHODS    ", "WTHER", "INCON", "LIGHT", "EVAPO", "INFIL", "PHOTO", "HYDRO", "NSWIT", "MESOM", "MESEV", "MESOL"}

        Dim dssat_queryRead As String = "Select Champ, Default_Value_Datamill, defaultValueOtherSource, IFNULL([defaultValueOtherSource],  [Default_Value_Datamill]) As dv From Variables Where ((model = 'dssat') And ([Table] = '" & dssat_tableName & "'));"
        Using dataAdapter1 As SqliteDataAdapter = New SqliteDataAdapter(dssat_queryRead, Connection)
            Dim DT As New DataSet()
            Dim rw As DataRow()
            Dim Dv As String
            dataAdapter1.Fill(DT, dssat_tableName)

            fileContent.Append(String.Join(Chr(32), siteColumnsHeader))
            fileContent.AppendLine() ' Append a line break.

            'read all line of dssat_x_simulation_method
            'For Each occurence As DataRow In dataTable.Rows

            rw = DT.Tables(0).Select("Champ='LNSIM'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(2))
            'fileContent.Append(formatItem_Lg(occurence.Item("LNSIM"), 2))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='TITMET'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(11))
            'fileContent.Append(formatItem_Lg(occurence.Item("TITMET"), 11))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='MEWTH'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(5))
            'fileContent.Append(Right_Justified(formatItem_Lg(occurence.Item("MEWTH"), 5)))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='MESIC'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(5))
            'fileContent.Append(Right_Justified(formatItem_Lg(occurence.Item("MESIC"), 5)))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='MELI'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(5))
            'fileContent.Append(Right_Justified(formatItem_Lg(occurence.Item("MELI"), 5)))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='MEEVP'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(5))
            'fileContent.Append(Right_Justified(formatItem_Lg(occurence.Item("MEEVP"), 5)))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='MEINF'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(5))
            'fileContent.Append(Right_Justified(formatItem_Lg(occurence.Item("MEINF"), 5)))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='MEPHO'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(5))
            'fileContent.Append(Right_Justified(formatItem_Lg(occurence.Item("MEPHO"), 5)))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='HYDRO'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(5))
            'fileContent.Append(Right_Justified(formatItem_Lg(occurence.Item("HYDRO"), 5)))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='NSWIT'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(5))
            'fileContent.Append(Right_Justified(formatItem_Lg(occurence.Item("NSWIT"), 5)))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='MESOM'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(5))
            'fileContent.Append(Right_Justified(formatItem_Lg(occurence.Item("MESOM"), 5)))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='MESEV'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(5))
            'fileContent.Append(Right_Justified(formatItem_Lg(occurence.Item("MESEV"), 5)))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='MESOL'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(5))
            'fileContent.Append(Right_Justified(formatItem_Lg(occurence.Item("MESOL"), 5)))
            fileContent.AppendLine() ' Append a line break.
            '  Exit For
            ' Next


        End Using
        Return fileContent

    End Function
    ''' <summary>
    ''' function : read table dssat_x_simulation_management and write into fileContent
    ''' </summary>
    ''' <param name="dssat_tableName"></param>
    ''' <param name="dssat_tableId"></param>
    ''' <param name="dssat_tableId_value"></param>
    ''' <param name="fileContent"></param>
    ''' <param name="Connection"></param>
    ''' <returns>FileContent</returns>
    ''' <remarks></remarks>
    Public Function writeBlockManagement(ByVal dssat_tableName As String, ByVal dssat_tableId As String, ByVal dssat_tableId_value As String, ByVal fileContent As StringBuilder, ByVal Connection As SqliteConnection) As StringBuilder

        'Dim dssat_queryRead As String = "select * from " + dssat_tableName + " where " + dssat_tableId + " = " + dssat_tableId_value + " and LNSIM = " + storeKeyDataN.ToString + " order by " + dssat_tableId + " asc, LNSIM asc ;"
        Dim siteColumnsHeader() As String = {"@N", "MANAGEMENT ", "PLANT", "IRRIG", "FERTI", "RESID", "HARVS"}

        Dim dssat_queryRead As String = "Select Champ, Default_Value_Datamill, defaultValueOtherSource, IFNULL([defaultValueOtherSource],  [Default_Value_Datamill]) As dv From Variables Where ((model = 'dssat') And ([Table] = '" & dssat_tableName & "'));"
        Using dataAdapter1 As SqliteDataAdapter = New SqliteDataAdapter(dssat_queryRead, Connection)
            Dim DT As New DataSet()
            Dim rw As DataRow()
            Dim Dv As String
            dataAdapter1.Fill(DT, dssat_tableName)

            fileContent.Append(String.Join(Chr(32), siteColumnsHeader))
            fileContent.AppendLine() ' Append a line break.

            ''read all line of dssat_x_simulation_general
            'For Each occurence As DataRow In dataTable.Rows

            rw = DT.Tables(0).Select("Champ='LNSIM'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(2))
            'fileContent.Append(formatItem_Lg(occurence.Item("LNSIM"), 2))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='TITMAT'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(11))
            'fileContent.Append(formatItem_Lg(occurence.Item("TITMAT"), 11))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='IPLTI'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(5))
            'fileContent.Append(Right_Justified(formatItem_Lg(occurence.Item("IPLTI"), 5)))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='IIRRI'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(5))
            'fileContent.Append(Right_Justified(formatItem_Lg(occurence.Item("IIRRI"), 5)))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='IFERI'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(5))
            'fileContent.Append(Right_Justified(formatItem_Lg(occurence.Item("IFERI"), 5)))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='IRESI'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(5))
            'fileContent.Append(Right_Justified(formatItem_Lg(occurence.Item("IRESI"), 5)))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='IHARI'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(5))
            'fileContent.Append(Right_Justified(formatItem_Lg(occurence.Item("IHARI"), 5)))
            fileContent.AppendLine() ' Append a line break.

            ' Exit For
            'Next


        End Using
        Return fileContent

    End Function
    ''' <summary>
    ''' function : read table dssat_x_simulation_outputs and write into fileContent
    ''' </summary>
    ''' <param name="dssat_tableName"></param>
    ''' <param name="dssat_tableId"></param>
    ''' <param name="dssat_tableId_value"></param>
    ''' <param name="fileContent"></param>
    ''' <param name="Connection"></param>
    ''' <returns>fileContent</returns>
    ''' <remarks></remarks>
    Public Function writeBlockoutputs(ByVal dssat_tableName As String, ByVal dssat_tableId As String, ByVal dssat_tableId_value As String, ByVal fileContent As StringBuilder, ByVal Connection As SqliteConnection) As StringBuilder

        'Dim dssat_queryRead As String = "select * from " + dssat_tableName + " where " + dssat_tableId + " = " + dssat_tableId_value + " and LNSIM = " + storeKeyDataN.ToString + " order by " + dssat_tableId + " asc, LNSIM asc ;"
        Dim siteColumnsHeader() As String = {"@N", "OUTPUTS    ", "FNAME", "OVVEW", "SUMRY", "FROPT", "GROUT", "CAOUT", "WAOUT", "NIOUT", "MIOUT", "DIOUT", "VBOSE", "CHOUT", "OPOUT"}

        Dim dssat_queryRead As String = "Select Champ, Default_Value_Datamill, Variables.defaultValueOtherSource, IFNULL([defaultValueOtherSource],  [Default_Value_Datamill]) As dv From Variables Where ((model = 'dssat') And ([Table] = '" & dssat_tableName & "'));"
        Using dataAdapter1 As SqliteDataAdapter = New SqliteDataAdapter(dssat_queryRead, Connection)
            Dim DT As New DataSet()
            Dim rw As DataRow()
            Dim Dv As String
            dataAdapter1.Fill(DT, dssat_tableName)

            fileContent.Append(String.Join(Chr(32), siteColumnsHeader))
            fileContent.AppendLine() ' Append a line break.

            ''read all line of dssat_x_simulation_outputs
            'For Each occurence As DataRow In dataTable.Rows

            rw = DT.Tables(0).Select("Champ='LNSIM'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(2))
            'fileContent.Append(formatItem_Lg(occurence.Item("LNSIM"), 2))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='TITOUT'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(11))
            'fileContent.Append(formatItem_Lg(occurence.Item("TITOUT"), 11))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='IOX'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(5))
            'fileContent.Append(Right_Justified(formatItem_Lg(occurence.Item("IOX"), 5)))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='IDETO'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(5))
            'fileContent.Append(Right_Justified(formatItem_Lg(occurence.Item("IDETO"), 5)))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='IDETS'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(5))
            'fileContent.Append(Right_Justified(formatItem_Lg(occurence.Item("IDETS"), 5)))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='FROP'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(5))
            'fileContent.Append(Right_Justified(formatItem_Lg(occurence.Item("FROP"), 5)))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='IDETG'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(5))
            'fileContent.Append(Right_Justified(formatItem_Lg(occurence.Item("IDETG"), 5)))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='IDETC'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(5))
            'fileContent.Append(Right_Justified(formatItem_Lg(occurence.Item("IDETC"), 5)))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='IDETG'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(5))
            'fileContent.Append(Right_Justified(formatItem_Lg(occurence.Item("IDETW"), 5)))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='IDETN'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(5))
            'fileContent.Append(Right_Justified(formatItem_Lg(occurence.Item("IDETN"), 5)))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='IDETP'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(5))
            'fileContent.Append(Right_Justified(formatItem_Lg(occurence.Item("IDETP"), 5)))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='IDETD'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(5))
            'fileContent.Append(Right_Justified(formatItem_Lg(occurence.Item("IDETD"), 5)))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='VBOSE'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(5))
            'fileContent.Append(Right_Justified(formatItem_Lg(occurence.Item("VBOSE"), 5)))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='CHOUT'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(5))
            'fileContent.Append(Right_Justified(formatItem_Lg(occurence.Item("CHOUT"), 5)))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='OPOUT'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(5))
            'fileContent.Append(Right_Justified(formatItem_Lg(occurence.Item("OPOUT"), 5)))
            fileContent.Append(Chr(32))
                fileContent.AppendLine() ' Append a line break.
            '   Exit For
            ' Next


        End Using
        Return fileContent

    End Function

    ''' <summary>
    ''' function : read table dssat_x_automatic_planting and write into fileContent
    ''' </summary>
    ''' <param name="dssat_tableName"></param>
    ''' <param name="dssat_tableId"></param>
    ''' <param name="dssat_tableId_value"></param>
    ''' <param name="fileContent"></param>
    ''' <param name="Connection"></param>
    ''' <returns>fileContent</returns>
    ''' <remarks></remarks>
    Public Function writeBlockAutomaticPlanting(ByVal dssat_tableName As String, ByVal dssat_tableId As String, ByVal dssat_tableId_value As String, ByVal fileContent As StringBuilder, ByVal Connection As SqliteConnection) As StringBuilder

        'Dim dssat_queryRead As String = "select * from " + dssat_tableName + " where " + dssat_tableId + " = " + dssat_tableId_value + " and LNSIM = " + storeKeyDataN.ToString + " order by " + dssat_tableId + " asc, LNSIM asc ;"
        Dim siteColumnsHeader() As String = {"@N", "PLANTING   ", "PFRST", "PLAST", "PH2OL", "PH2OU", "PH2OD", "PSTMX", "PSTMN"}

        Dim dssat_queryRead As String = "Select Champ, Default_Value_Datamill, defaultValueOtherSource, IFNULL([defaultValueOtherSource],  [Default_Value_Datamill]) As dv From Variables Where ((model = 'dssat') And ([Table] = '" & dssat_tableName & "'));"
        Using dataAdapter1 As SqliteDataAdapter = New SqliteDataAdapter(dssat_queryRead, Connection)
            Dim DT As New DataSet()
            Dim rw As DataRow()
            Dim Dv As String
            dataAdapter1.Fill(DT, dssat_tableName)


            fileContent.Append(String.Join(Chr(32), siteColumnsHeader))
            fileContent.AppendLine() ' Append a line break.

            ''read all line of dssat_x_automatic_planting
            'For Each occurence As DataRow In dataTable.Rows

            rw = DT.Tables(0).Select("Champ='LNSIM'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(2))
            'fileContent.Append(formatItem_Lg(occurence.Item("LNSIM"), 2))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='TITPLA'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(11))
            'fileContent.Append(formatItem_Lg(occurence.Item("TITPLA"), 11))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='PWDINF'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(5, "0"))
            'fileContent.Append(formatItemDate(formatItem_Lg(occurence.Item("PWDINF"), 5)))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='PWDINL'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(5, "0"))
            'fileContent.Append(formatItemDate(formatItem_Lg(occurence.Item("PWDINL"), 5)))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='SWPLTL'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(5))
            'fileContent.Append(formatItem_Lg(occurence.Item("SWPLTL"), 5))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='SWPLTH'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(5))
            'fileContent.Append(formatItem_Lg(occurence.Item("SWPLTH"), 5))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='SWPLTD'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(5))
            'fileContent.Append(formatItem_Lg(occurence.Item("SWPLTD"), 5))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='PTX'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(5))
            'fileContent.Append(formatItem_Lg(occurence.Item("PTX"), 5))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='PTTN'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(5))
            'fileContent.Append(formatItem_Lg(occurence.Item("PTTN"), 5))
            fileContent.Append(Chr(32))
                fileContent.AppendLine() ' Append a line break.
            ' Exit For
            ' Next


        End Using
        Return fileContent

    End Function
    ''' <summary>
    ''' function : read table dssat_x_automatic_irrigation and write into fileContent
    ''' </summary>
    ''' <param name="dssat_tableName"></param>
    ''' <param name="dssat_tableId"></param>
    ''' <param name="dssat_tableId_value"></param>
    ''' <param name="fileContent"></param>
    ''' <param name="Connection"></param>
    ''' <returns>fileContent</returns>
    ''' <remarks></remarks>
    Public Function writeBlockAutomaticIrrigation(ByVal dssat_tableName As String, ByVal dssat_tableId As String, ByVal dssat_tableId_value As String, ByVal fileContent As StringBuilder, ByVal Connection As SqliteConnection) As StringBuilder

        ' Dim dssat_queryRead As String = "select * from " + dssat_tableName + " where " + dssat_tableId + " = " + dssat_tableId_value + " and LNSIM = " + storeKeyDataN.ToString + " order by " + dssat_tableId + " asc, LNSIM asc ;"
        Dim siteColumnsHeader() As String = {"@N", "IRRIGATION ", "IMDEP", "ITHRL", "ITHRU", "IROFF", "IMETH", "IRAMT", "IREFF"}

        Dim dssat_queryRead As String = "Select Champ, Default_Value_Datamill, defaultValueOtherSource, IFNULL([defaultValueOtherSource],  [Default_Value_Datamill]) As dv From Variables Where ((model = 'dssat') And ([Table] = '" & dssat_tableName & "'));"
        Using dataAdapter1 As SqliteDataAdapter = New SqliteDataAdapter(dssat_queryRead, Connection)
            Dim DT As New DataSet()
            Dim rw As DataRow()
            Dim Dv As String
            dataAdapter1.Fill(DT, dssat_tableName)
            fileContent.Append(String.Join(Chr(32), siteColumnsHeader))
            fileContent.AppendLine() ' Append a line break.

            ''read all line of dssat_x_automatic_irrigation
            ' For Each occurence As DataRow In dataTable.Rows

            rw = DT.Tables(0).Select("Champ='LNSIM'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(2))
            'fileContent.Append(formatItem_Lg(occurence.Item("LNSIM"), 2))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='TITIRR'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(11))
            'fileContent.Append(formatItem_Lg(occurence.Item("TITIRR"), 11))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='DSOIL'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(5))
            'fileContent.Append(formatItem_Lg(occurence.Item("DSOIL"), 5))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='THETAC'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(5))
            'fileContent.Append(formatItem_Lg(occurence.Item("THETAC"), 5))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='IEPT'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(5))
            'fileContent.Append(formatItem_Lg(occurence.Item("IEPT"), 5))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='IOFF'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(5))
            'fileContent.Append(formatItem_Lg(occurence.Item("IOFF"), 5))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='IAME'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(5))
            'fileContent.Append(formatItem_Lg(occurence.Item("IAME"), 5))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='AIRAMT'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(5))
            'fileContent.Append(formatItem_Lg(occurence.Item("AIRAMT"), 5))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='EFFIRR'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(5))
            'fileContent.Append(formatItem_Lg(occurence.Item("EFFIRR"), 5))
            fileContent.Append(Chr(32))
                fileContent.AppendLine() ' Append a line break.
            '     Exit For
            ' Next

        End Using
        Return fileContent

    End Function
    ''' <summary>
    ''' function : read table dssat_x_automatic_nitrogen and write into fileContent
    ''' </summary>
    ''' <param name="dssat_tableName"></param>
    ''' <param name="dssat_tableId"></param>
    ''' <param name="dssat_tableId_value"></param>
    ''' <param name="fileContent"></param>
    ''' <param name="Connection"></param>
    ''' <returns>fileContent</returns>
    ''' <remarks></remarks>
    Public Function writeBlockAutomaticNitrogen(ByVal dssat_tableName As String, ByVal dssat_tableId As String, ByVal dssat_tableId_value As String, ByVal fileContent As StringBuilder, ByVal Connection As SqliteConnection) As StringBuilder

        ' Dim dssat_queryRead As String = "select * from " + dssat_tableName + " where " + dssat_tableId + " = " + dssat_tableId_value + " and LNSIM = " + storeKeyDataN.ToString + " order by " + dssat_tableId + " asc, LNSIM asc ;"
        Dim siteColumnsHeader() As String = {"@N", "NITROGEN   ", "NMDEP", "NMTHR", "NAMNT", "NCODE", "NAOFF"}

        Dim dssat_queryRead As String = "Select Champ, Default_Value_Datamill, defaultValueOtherSource, IFNULL([defaultValueOtherSource],  [Default_Value_Datamill]) As dv From Variables Where ((model = 'dssat') And ([Table] = '" & dssat_tableName & "'));"
        Using dataAdapter1 As SqliteDataAdapter = New SqliteDataAdapter(dssat_queryRead, Connection)
            Dim DT As New DataSet()
            Dim rw As DataRow()
            Dim Dv As String
            dataAdapter1.Fill(DT, dssat_tableName)
            fileContent.Append(String.Join(Chr(32), siteColumnsHeader))
            fileContent.AppendLine() ' Append a line break.

            ''read all line of dssat_x_automatic_nitrogen
            'For Each occurence As DataRow In dataTable.Rows

            rw = DT.Tables(0).Select("Champ='LNSIM'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(2))
            'fileContent.Append(formatItem_Lg(occurence.Item("LNSIM"), 2))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='TITNIT'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(11))
            'fileContent.Append(formatItem_Lg(occurence.Item("TITNIT"), 11))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='DSOILN'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(5))
            'fileContent.Append(formatItem_Lg(occurence.Item("DSOILN"), 5))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='SOILNC'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(5))
            'fileContent.Append(formatItem_Lg(occurence.Item("SOILNC"), 5))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='SOILNX'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(5))
            'fileContent.Append(formatItem_Lg(occurence.Item("SOILNX"), 5))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='NCODE'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(5))
            'fileContent.Append(formatItem_Lg(occurence.Item("NCODE"), 5))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='NEND'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(5))
            'fileContent.Append(formatItem_Lg(occurence.Item("NEND"), 5))
            fileContent.AppendLine() ' Append a line break.

            '  Exit For
            ' Next


        End Using
        Return fileContent

    End Function
    ''' <summary>
    ''' function : read table dssat_x_automatic_residues and write into fileContent
    ''' </summary>
    ''' <param name="dssat_tableName"></param>
    ''' <param name="dssat_tableId"></param>
    ''' <param name="dssat_tableId_value"></param>
    ''' <param name="fileContent"></param>
    ''' <param name="Connection"></param>
    ''' <returns>fileContent</returns>
    ''' <remarks></remarks>
    Public Function writeBlockAutomaticResidue(ByVal dssat_tableName As String, ByVal dssat_tableId As String, ByVal dssat_tableId_value As String, ByVal fileContent As StringBuilder, ByVal Connection As SqliteConnection) As StringBuilder

        'Dim dssat_queryRead As String = "select * from " + dssat_tableName + " where " + dssat_tableId + " = " + dssat_tableId_value + " and LNSIM = " + storeKeyDataN.ToString + " order by " + dssat_tableId + " asc, LNSIM asc ;"
        Dim siteColumnsHeader() As String = {"@N", "RESIDUES   ", "RIPCN", "RTIME", "RIDEP"}

        Dim dssat_queryRead As String = "Select Champ, Default_Value_Datamill, defaultValueOtherSource, IFNULL([defaultValueOtherSource],  [Default_Value_Datamill]) As dv From Variables Where ((model = 'dssat') And ([Table] = '" & dssat_tableName & "'));"
        Using dataAdapter1 As SqliteDataAdapter = New SqliteDataAdapter(dssat_queryRead, Connection)
            Dim DT As New DataSet()
            Dim rw As DataRow()
            Dim Dv As String
            dataAdapter1.Fill(DT, dssat_tableName)
            fileContent.Append(String.Join(Chr(32), siteColumnsHeader))
            fileContent.AppendLine() ' Append a line break.

            ''read all line of dssat_x_automatic_residues
            ' For Each occurence As DataRow In dataTable.Rows

            rw = DT.Tables(0).Select("Champ='LNSIM'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(2))
            'fileContent.Append(formatItem_Lg(occurence.Item("LNSIM"), 2))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='TITRES'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(11))
            'fileContent.Append(formatItem_Lg(occurence.Item("TITRES"), 11))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='RIP'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(5))
            'fileContent.Append(formatItem_Lg(occurence.Item("RIP"), 5))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='NRESDL'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(5))
            'fileContent.Append(formatItem_Lg(occurence.Item("NRESDL"), 5))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='DRESMG'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(5))
            'fileContent.Append(formatItem_Lg(occurence.Item("DRESMG"), 5))
            fileContent.AppendLine() ' Append a line break.

            '  Exit For
            ' Next


        End Using
        Return fileContent

    End Function
    ''' <summary>
    ''' function : read table dssat_x_automatic_harvest and write into fileContent
    ''' </summary>
    ''' <param name="dssat_tableName"></param>
    ''' <param name="dssat_tableId"></param>
    ''' <param name="dssat_tableId_value"></param>
    ''' <param name="fileContent"></param>
    ''' <param name="Connection"></param>
    ''' <returns>fileContent</returns>
    ''' <remarks></remarks>
    Public Function writeBlockAutomaticHarvest(ByVal dssat_tableName As String, ByVal dssat_tableId As String, ByVal dssat_tableId_value As String, ByVal fileContent As StringBuilder, ByVal Connection As SqliteConnection, ByVal MI_Connection As SqliteConnection) As StringBuilder

        '     Dim dssat_queryRead As String = "select * from " + dssat_tableName + " where " + dssat_tableId + " = " + dssat_tableId_value + " and LNSIM = " + storeKeyDataN.ToString + " order by " + dssat_tableId + " asc, LNSIM asc ;"

        Dim siteColumnsHeader() As String = {"@N", "HARVEST    ", "HFRST", "HLAST", "HPCNP", "HPCNR"}

        Dim dssat_queryRead As String = "Select Champ, Default_Value_Datamill, defaultValueOtherSource, IFNULL([defaultValueOtherSource],  [Default_Value_Datamill]) As dv From Variables Where ((model = 'dssat') And ([Table] = '" & dssat_tableName & "'));"
        Using dataAdapter1 As SqliteDataAdapter = New SqliteDataAdapter(dssat_queryRead, Connection)
            Dim DT As New DataSet()
            Dim rw As DataRow()
            Dim Dv As String
            dataAdapter1.Fill(DT, dssat_tableName)
            Dim fetchAllQuery As String = "SELECT SimUnitList.idsim, SimUnitList.EndYear,SimUnitList.EndDay FROM SimUnitList  Where Idsim ='" & dssat_tableId_value & "';"
            'Init and use DataAdapter
            Dim dataAdapter As SqliteDataAdapter = New SqliteDataAdapter(fetchAllQuery, MI_Connection)
            ' Filling Dataset
            Dim dataSet As New DataSet()
            dataAdapter.Fill(dataSet, "dssat_x_exp")
            Dim dataTable As DataTable = dataSet.Tables("dssat_x_exp")
            fileContent.Append(String.Join(Chr(32), siteColumnsHeader))
            fileContent.AppendLine() ' Append a line break.

            ''read all line of dssat_x_automatic_harvest
            'For Each occurence As DataRow In dataTable.Rows

            rw = DT.Tables(0).Select("Champ='LNSIM'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(2))
            'fileContent.Append(formatItem_Lg(occurence.Item("LNSIM"), 2))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='TITHAR'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(11))
            'fileContent.Append(formatItem_Lg(occurence.Item("TITHAR"), 11))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='HDLAY'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(5))
            'fileContent.Append(formatItem_Lg(occurence.Item("HDLAY"), 5))
            fileContent.Append(Chr(32))
            'rw = DT.Tables(0).Select("Champ='HLATE'")
            'Dv = rw(0)("dv").ToString
            'fileContent.Append(Dv.PadLeft(5, "0"))
            fileContent.Append(Mid(dataTable.Rows(0).Item("Endyear").ToString, 3, 2) & dataSet.Tables(0).Rows(0).Item("EndDay").ToString.PadLeft(3, "0"))
            'fileContent.Append(formatItemDate(formatItem_Lg(occurence.Item("HLATE"), 5)))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='HPP'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(5))
            'fileContent.Append(formatItem_Lg(occurence.Item("HPP"), 5))
            fileContent.Append(Chr(32))
            rw = DT.Tables(0).Select("Champ='HRP'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(Dv.PadLeft(5))
            'fileContent.Append(formatItem_Lg(occurence.Item("HRP"), 5))
            fileContent.AppendLine() ' Append a line break.

            '  Exit For
            '  Next


        End Using
        Return fileContent

    End Function


    Public Sub setUsm(ByVal UsmSelected As String)
        usmString = UsmSelected
    End Sub

    Public Sub setUsm(ByVal UsmSelected As Integer)
        usmId = UsmSelected
    End Sub

    Public Sub setNthreads(ByVal number_of_threads As Integer)
        nthreads = number_of_threads
    End Sub

    Public Sub setSstart(ByVal start_at As Integer)
        sstart = start_at
    End Sub

    Public Sub setSend(ByVal end_at As Integer)
        send = end_at
    End Sub    
End Class
