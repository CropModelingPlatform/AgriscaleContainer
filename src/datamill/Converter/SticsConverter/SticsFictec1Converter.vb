Imports System
Imports System.IO
Imports System.Text
Imports System.Globalization
Imports System.Configuration
Imports System.Data
Imports System.Data.Sqlite

Public Class SticsFictec1Converter
    
    Inherits Converter
    
    Public Overrides Sub Export(DirectoryPath As String)
    End Sub

    Public Overrides Sub Export(
            DirectoryPath As String,
            MasterInput_Connection As SqliteConnection,
            ModelDictionary_Connection As SqliteConnection)

        Dim fileName As String = "fictec1.txt"
        Dim fileContent As StringBuilder = New StringBuilder()
        Dim ST(3) As String
        ST = DirectoryPath.Split(Path.DirectorySeparatorChar)
        DirectoryPath = ST(0) & Path.DirectorySeparatorChar & ST(1) & Path.DirectorySeparatorChar & ST(2) & Path.DirectorySeparatorChar & ST(3) & Path.DirectorySeparatorChar & ST(4) & Path.DirectorySeparatorChar & ST(5) & Path.DirectorySeparatorChar & ST(6) & Path.DirectorySeparatorChar & ST(7)
        'fictec1 query
        Dim T As String = "Select Champ, Default_Value_Datamill, defaultValueOtherSource, IFNULL([defaultValueOtherSource],  [Default_Value_Datamill]) As dv From Variables Where ((model='stics') AND ([Table]='st_fictec'));"
        Dim DT As New DataSet()

        Dim Cmd As New SqliteDataAdapter(T, ModelDictionary_Connection)
        Cmd.Fill(DT, "TChamp")
        Dim fetchAllQuery As String = "SELECT SimUnitList.idsim, SimUnitList.idMangt, Soil.SoilTotalDepth, ListCultivars.idcultivarStics, CropManagement.sdens," _
        & " CropManagement.sowingdate, CropManagement.SoilTillPolicyCode FROM Soil INNER JOIN (ListCultivars INNER JOIN (CropManagement INNER JOIN SimUnitList ON CropManagement.idMangt = SimUnitList.idMangt)" _
        & " ON ListCultivars.IdCultivar = CropManagement.Idcultivar) ON Lower(Soil.IdSoil) = Lower(SimUnitList.idsoil)  where idSim='" + ST(7) + "' ;"

        'Init and use DataAdapter
        Dim dataAdapter As SqliteDataAdapter = New SqliteDataAdapter(fetchAllQuery, MasterInput_Connection)

        ' Filling Dataset
        Dim dataSet, DS2 As New DataSet()
        dataAdapter.Fill(dataSet, "st_fictec")
        Dim dataTable As DataTable = dataSet.Tables("st_fictec")
        Dim DT2 As DataTable = dataSet.Tables("Inorg")
        Dim fetchallquery2 As String
        'Dim nbInterventions As Integer
        Dim rw As DataRow
        rw = dataTable.Rows(0)
        'read all lines of st_fictec
        'FormatSticsData(fileContent, DT, "supply of organic residus.nbinterventions", 1, 1)
        fileContent.Append("nbinterventions")
        fileContent.AppendLine()

        DS2.Clear()
        fetchallquery2 = "SELECT SimUnitList.idsim, CropManagement.sowingdate, OrganicFOperations.Dferti, OrganicFOperations.OFNumber, OrganicFOperations.CNferti, " _
                & "OrganicFOperations.NFerti, OrganicFOperations.Qmanure, OrganicFOperations.TypeResidues, ListResidues.idresidueStics, CropManagement.SoilTillPolicyCode " _
                & "FROM ListResidues INNER JOIN ((OrganicFertilizationPolicy INNER JOIN (CropManagement INNER JOIN SimUnitList ON CropManagement.idMangt = SimUnitList.idMangt) " _
                & "ON OrganicFertilizationPolicy.OFertiPolicyCode = CropManagement.OFertiPolicyCode) INNER JOIN OrganicFOperations ON OrganicFertilizationPolicy.OFertiPolicyCode " _
                & "= OrganicFOperations.OFertiPolicyCode) ON ListResidues.TypeResidues = OrganicFOperations.TypeResidues where idSim='" + ST(7) + "' Order by OFNumber ;"
        Dim dataAdapter2 As SqliteDataAdapter = New SqliteDataAdapter(fetchallquery2, MasterInput_Connection)
        dataAdapter2.Fill(DS2)
        'nbInterventions = DS2.Tables(0).Rows.Count
        If IsDBNull(DS2.Tables(0).Rows(0).Item("idresidueStics")) Then
            fileContent.AppendLine("0")
        Else
            fileContent.AppendLine(DS2.Tables(0).Rows.Count)
            'Display opp1 only if nbinterventions <> 0
            If (DS2.Tables(0).Rows.Count <> 0) Then
                For i = 0 To DS2.Tables(0).Rows.Count - 1
                    fileContent.AppendLine("opp1")
                    fileContent.Append(CInt(DS2.Tables(0).Rows(i).Item("sowingDate")) + CInt(DS2.Tables(0).Rows(i).Item("Dferti")))
                    fileContent.Append(" ")
                    'idresiduesStics corresponding to Typeresidues  of OFNumber for OFertiPolicyCode=CropManangement.OFertiPolicyCode
                    fileContent.Append(DS2.Tables(0).Rows(i).Item("idresidueStics"))
                    fileContent.Append(" ")
                    fileContent.Append(DS2.Tables(0).Rows(i).Item("qmanure"))
                    fileContent.Append(" ")
                    fileContent.Append(DS2.Tables(0).Rows(i).Item("CNferti") * DS2.Tables(0).Rows(i).Item("Nferti"))
                    fileContent.Append(" ")
                    fileContent.Append(DS2.Tables(0).Rows(i).Item("CNferti"))
                    fileContent.Append(" ")
                    fileContent.Append(DS2.Tables(0).Rows(i).Item("Nferti"))
                    fileContent.Append(" ")
                    fileContent.Append(FormatSticsRawData(DT, "supply of organic residus.eaures"))
                    fileContent.AppendLine()
                Next
                'fileContent.Append(FormatOppData(row.item("opp1").ToString()))
            End If
        End If

        'nbInterventions = FormatSticsRawData(row.item("nbinterventions2"))
        Dim Sql As String
        Sql = "SELECT SoilTillPolicy.SoilTillPolicyCode, SoilTillageOperations.STNumber, SoilTillPolicy.NumTillOperations, SoilTillageOperations.DepthResUp, SoilTillageOperations.DepthResLow, SoilTillageOperations.DSTill" _
            & " FROM SoilTillPolicy INNER JOIN SoilTillageOperations ON SoilTillPolicy.SoilTillPolicyCode = SoilTillageOperations.SoilTillPolicyCode " _
            & " where SoilTillPolicy.SoilTillPolicyCode= '" & dataTable.Rows(0).Item("SoilTillPolicyCode") & "'"
        Dim Adp As SqliteDataAdapter = New SqliteDataAdapter(Sql, MasterInput_Connection)
        ' Filling Dataset
        Dim dataSet2 As New DataSet()
        Adp.Fill(dataSet2, "st_param_sol")
        Dim dataTill As DataTable = dataSet2.Tables("st_param_sol")
        fileContent.AppendLine("nbinterventions") 'soil tillage
        fileContent.AppendLine(dataTill.Rows(0).Item("NumTillOperations"))
        If CInt(dataTill.Rows(0).Item("NumTillOperations")) > 0 Then
            For i = 0 To dataTill.Rows.Count - 1
                fileContent.AppendLine("opp1")
                fileContent.Append(rw("sowingdate") + dataTill.Rows(i).Item("DStill")) 'jultrav
                fileContent.Append(" ")
                fileContent.Append(dataTill.Rows(i).Item("DepthResUp")) 'profres
                fileContent.Append(" ")
                fileContent.Append(dataTill.Rows(i).Item("DepthResLow")) 'proftrav
                fileContent.AppendLine()
            Next
        End If
        'nbInterventions
        '1
        'opp1
        '30 0.00 20.00 
        'rajouter jultrav porfres...
        fileContent.AppendLine("iplt0")
        fileContent.Append(rw.Item("sowingdate"))
        fileContent.AppendLine()
        FormatSticsData(fileContent, DT, "profsem")
        fileContent.AppendLine("densitesem")
        fileContent.Append(rw.Item("Sdens"))
        fileContent.AppendLine()
        fileContent.AppendLine("variete")
        fileContent.Append(rw.Item("idcultivarstics"))
        fileContent.AppendLine()
        FormatSticsData(fileContent, DT, "codetradtec")
        FormatSticsData(fileContent, DT, "interrang")
        FormatSticsData(fileContent, DT, "orientrang")
        FormatSticsData(fileContent, DT, "codedecisemis")
        FormatSticsData(fileContent, DT, "nbjmaxapressemis")
        FormatSticsData(fileContent, DT, "nbjseuiltempref")
        FormatSticsData(fileContent, DT, "codestade")
        FormatSticsData(fileContent, DT, "ilev")
        FormatSticsData(fileContent, DT, "iamf")
        FormatSticsData(fileContent, DT, "ilax")
        FormatSticsData(fileContent, DT, "isen")
        FormatSticsData(fileContent, DT, "ilan")
        FormatSticsData(fileContent, DT, "iflo")
        FormatSticsData(fileContent, DT, "idrp")
        FormatSticsData(fileContent, DT, "imat")
        FormatSticsData(fileContent, DT, "irec")
        fileContent.AppendLine("irecbutoir")
        fileContent.Append(rw.Item("sowingdate") + 250)
        fileContent.AppendLine()
        FormatSticsData(fileContent, DT, "effirr")
        FormatSticsData(fileContent, DT, "codecalirrig")
        FormatSticsData(fileContent, DT, "ratiol")
        FormatSticsData(fileContent, DT, "dosimx")
        FormatSticsData(fileContent, DT, "doseirrigmin")
        FormatSticsData(fileContent, DT, "codedateappH2O")
        fileContent.AppendLine("nbinterventions") 'irrigation
        'nbInterventions = FormatSticsRawData(row.item("nbinterventions3"))
        fileContent.AppendLine(0)
        FormatSticsData(fileContent, DT, "codlocirrig")
        FormatSticsData(fileContent, DT, "locirrig")
        'FormatSticsData(fileContent, DT, "profmes")
        fileContent.AppendLine("profmes")
        fileContent.Append(rw.Item("SoilTotalDepth"))
        fileContent.AppendLine()        
        FormatSticsData(fileContent, DT, "engrais")
        FormatSticsData(fileContent, DT, "concirr")
        FormatSticsData(fileContent, DT, "codedateappN")
        FormatSticsData(fileContent, DT, "codefracappN")
        FormatSticsData(fileContent, DT, "fertilisation.Qtot_N",, 1)
        DS2.Clear()
        fetchallquery2 = "Select SimUnitList.idsim, InorganicFOperations.N, CropManagement.sowingdate, InorganicFOperations.Dferti, InorganicFertilizationPolicy.NumInorganicFerti " _
        & " FROM(InorganicFertilizationPolicy INNER JOIN InorganicFOperations On InorganicFertilizationPolicy.InorgFertiPolicyCode = InorganicFOperations.InorgFertiPolicyCode)" _
        & "INNER JOIN (CropManagement INNER JOIN SimUnitList On CropManagement.idMangt = SimUnitList.idMangt) On InorganicFertilizationPolicy.InorgFertiPolicyCode = " _
        & " CropManagement.InoFertiPolicyCode where idSim='" + ST(7) + "' ;"
        dataAdapter2 = New SqliteDataAdapter(fetchallquery2, MasterInput_Connection)
        dataAdapter2.Fill(DS2)

        fileContent.AppendLine("nbinterventions")
        'nbInterventions = FormatSticsRawData(DT.item("nbinterventions4"))
        fileContent.AppendLine(DS2.Tables(0).Rows.Count)
        If DS2.Tables(0).Rows.Count > 0 Then
            For i = 0 To DS2.Tables(0).Rows.Count - 1
                fileContent.AppendLine("opp1")
                fileContent.Append(DS2.Tables(0).Rows(i).Item("sowingDate") + DS2.Tables(0).Rows(i).Item("Dferti"))
                fileContent.Append(" ")
                fileContent.Append(DS2.Tables(0).Rows(i).Item("N"))
                fileContent.AppendLine()
            Next
        End If

        FormatSticsData(fileContent, DT, "codlocferti")
        FormatSticsData(fileContent, DT, "locferti")
        FormatSticsData(fileContent, DT, "ressuite")
        FormatSticsData(fileContent, DT, "codceuille")
        FormatSticsData(fileContent, DT, "nbceuille")
        FormatSticsData(fileContent, DT, "cadencerec")
        FormatSticsData(fileContent, DT, "codrecolte")
        FormatSticsData(fileContent, DT, "codeaumin")
        FormatSticsData(fileContent, DT, "h2ograinmin")
        FormatSticsData(fileContent, DT, "h2ograinmax")
        FormatSticsData(fileContent, DT, "sucrerec")
        FormatSticsData(fileContent, DT, "CNgrainrec")
        FormatSticsData(fileContent, DT, "huilerec")
        FormatSticsData(fileContent, DT, "coderecolteassoc")
        FormatSticsData(fileContent, DT, "codedecirecolte")
        FormatSticsData(fileContent, DT, "nbjmaxapresrecolte")
        FormatSticsData(fileContent, DT, "codefauche")
        FormatSticsData(fileContent, DT, "mscoupemini")
        FormatSticsData(fileContent, DT, "codemodfauche")
        FormatSticsData(fileContent, DT, "hautcoupedefaut")
        FormatSticsData(fileContent, DT, "stadecoupedf")

        fileContent.AppendLine("nbinterventions")
        fileContent.AppendLine("0")
        fileContent.AppendLine("nbinterventions")
        fileContent.AppendLine("0")
        'nbInterventions = FormatSticsRawData(DT.item("nbinterventions5"))

        FormatSticsData(fileContent, DT, "codepaillage")
        FormatSticsData(fileContent, DT, "couvermulchplastique")
        FormatSticsData(fileContent, DT, "albedomulchplastique")
        FormatSticsData(fileContent, DT, "codrognage")
        FormatSticsData(fileContent, DT, "largrogne")
        FormatSticsData(fileContent, DT, "hautrogne")
        FormatSticsData(fileContent, DT, "biorognem")
        FormatSticsData(fileContent, DT, "codcalrogne")
        FormatSticsData(fileContent, DT, "julrogne")
        FormatSticsData(fileContent, DT, "margerogne")
        FormatSticsData(fileContent, DT, "codeclaircie")
        FormatSticsData(fileContent, DT, "juleclair")
        FormatSticsData(fileContent, DT, "nbinfloecl")
        FormatSticsData(fileContent, DT, "codeffeuil")
        FormatSticsData(fileContent, DT, "codhauteff")
        FormatSticsData(fileContent, DT, "codcaleffeuil")
        FormatSticsData(fileContent, DT, "laidebeff")
        FormatSticsData(fileContent, DT, "effeuil")
        FormatSticsData(fileContent, DT, "juleffeuil")
        FormatSticsData(fileContent, DT, "laieffeuil")
        FormatSticsData(fileContent, DT, "codetaille")
        FormatSticsData(fileContent, DT, "jultaille")
        FormatSticsData(fileContent, DT, "codepalissage")
        FormatSticsData(fileContent, DT, "hautmaxtec")
        FormatSticsData(fileContent, DT, "largtec")
        FormatSticsData(fileContent, DT, "codabri")
        FormatSticsData(fileContent, DT, "transplastic")
        FormatSticsData(fileContent, DT, "surfouvre1")
        FormatSticsData(fileContent, DT, "julouvre2")
        FormatSticsData(fileContent, DT, "surfouvre2")
        FormatSticsData(fileContent, DT, "julouvre3")
        FormatSticsData(fileContent, DT, "surfouvre3")
        FormatSticsData(fileContent, DT, "codeDST")
        FormatSticsData(fileContent, DT, "dachisel")
        FormatSticsData(fileContent, DT, "dalabour")
        FormatSticsData(fileContent, DT, "rugochisel")
        FormatSticsData(fileContent, DT, "rugolabour")
        FormatSticsData(fileContent, DT, "codeDSTtass")
        FormatSticsData(fileContent, DT, "profhumsemoir")
        FormatSticsData(fileContent, DT, "dasemis")
        FormatSticsData(fileContent, DT, "profhumrecolteuse")
        FormatSticsData(fileContent, DT, "darecolte")
        FormatSticsData(fileContent, DT, "codeDSTnbcouche")


        Try
            ' Export file to specified directory
            WriteFile(DirectoryPath, fileName, fileContent.ToString())
        Catch ex As Exception
            MessageBox.Show("Error during writing file : " + ex.Message)
        End Try

    End Sub

    Public Sub FormatSticsData(ByRef fileContent As StringBuilder, ByRef row As Object, ByVal champ As String, Optional ByVal precision As Integer = 5, Optional ByVal fieldIt As Integer = 0)
        Dim res As String
        Dim typeData As String
        Dim rw() As DataRow
        Dim data As Object
        Dim fieldName As String

        fieldName = champ
        'For repeated fields, build field name 
        If (fieldIt <> 0) Then
            'champ = champ + fieldIt.ToString()
            InStr(fieldName, ".")
            fieldName = Mid(fieldName, InStr(fieldName, ".") + 1)
        End If

        'fetch data
        rw = row.tables(0).select("Champ='" & champ & "'")
        data = rw(0)("dv")
        res = ""
        typeData = data.GetType().ToString()

        'if type is string or int
        If ((typeData = "System.String") Or (typeData = "System.Int32")) Then
            res = data.ToString()
        End If
        'if type is real
        If (typeData = "System.Single") Then
            Dim tmp As Single
            'Convert object to double
            tmp = Convert.ToDouble(data)
            If precision > 0 And precision < 7 Then
                res = FormatNumber(tmp, precision)
            Else
                res = tmp.ToString("0.###e+0", CultureInfo.InvariantCulture)
            End If
        End If
        'if cell is null
        If (typeData = "System.DBNull") Then
            res = ""
        End If
        'Print data in file
        fileContent.Append(fieldName)
        fileContent.AppendLine()
        fileContent.Append(res)
        fileContent.AppendLine()
    End Sub
    Public Function FormatSticsRawData(ByVal data As Object, ByVal champ As String, Optional ByVal precision As Integer = 1) As String
        Dim res As String
        Dim typeData As String
        Dim rw2() As DataRow
        rw2 = data.Tables(0).Select("champ='" & champ & "'")
        If rw2.Count = 0 Then MsgBox(champ)
        res = rw2(0).Item("dv").ToString
        'res = ""
        typeData = res.GetType().ToString()


        Return res
    End Function
    Public Overrides Sub Import(DirectoryPath As String, model As String)

    End Sub
End Class


