Imports System
Imports System.IO
Imports System.Text
Imports System.Globalization
Imports System.Configuration
Imports System.Data
Imports System.Data.Sqlite

Public Class SticsParamSolConverter
    
    Inherits Converter
    
    Public Overrides Sub Export(DirectoryPath As String)
    End Sub

    Public Overrides Sub Export(
            DirectoryPath As String,
            MasterInput_Connection As SqliteConnection,
            ModelDictionary_Connection As SqliteConnection)

        Dim fileName As String = "param.sol"
        Dim fileContent As StringBuilder = New StringBuilder()
        Dim Dv As String
        Dim ST(3) As String
        ST = DirectoryPath.Split(Path.DirectorySeparatorChar)
        DirectoryPath = ST(0) & Path.DirectorySeparatorChar & ST(1) & Path.DirectorySeparatorChar & ST(2) & Path.DirectorySeparatorChar & ST(3) & Path.DirectorySeparatorChar & ST(4) & Path.DirectorySeparatorChar & ST(5) & Path.DirectorySeparatorChar & ST(6) & Path.DirectorySeparatorChar & ST(7)
        'Param_sol query
        Dim T As String = "Select  Champ, Default_Value_Datamill, defaultValueOtherSource, IFNULL([defaultValueOtherSource],  [Default_Value_Datamill]) As dv From Variables Where ((model = 'stics') And ([Table] = 'st_param_sol'));"
        Dim DT As New DataSet()
        Dim rw() As DataRow
        Dim Cmd As New SqliteDataAdapter(T, ModelDictionary_Connection)
        Cmd.Fill(DT, "TChamp")
        'Init and use DataAdapter
        Dim fetchAllQuery As String = "SELECT Soil.IdSoil,Soil.SoilOption, Soil.OrganicC,Soil.OrganicNStock, Soil.SoilRDepth, Soil.SoilTotalDepth, Soil.SoilTextureType, Soil.Wwp, Soil.Wfc, Soil.bd, Soil.albedo, Soil.Ph, Soil.cf, RunoffTypes.RunoffCoefBSoil, SoilTypes.Clay" _
        & " FROM SoilTypes INNER JOIN (RunoffTypes INNER JOIN (Soil INNER JOIN SimUnitList ON Lower(Soil.IdSoil) = Lower(SimUnitList.idsoil)) ON RunoffTypes.RunoffType = Soil.RunoffType) ON Lower(SoilTypes.SoilTextureType) = Lower(Soil.SoilTextureType)" _
        & " where idSim='" + ST(7) + "';"
        Dim dataAdapter As SqliteDataAdapter = New SqliteDataAdapter(fetchAllQuery, MasterInput_Connection)
        ' Filling Dataset
        Dim dataSet As New DataSet()
        dataAdapter.Fill(dataSet, "st_param_sol")
        Dim dataTable As DataTable = dataSet.Tables("st_param_sol")
        'read all line of st_param_sol
        For Each row In dataTable.Rows
            fileContent.Append("1".PadLeft(5))
            fileContent.Append("Sol".PadLeft(6))
            'rw = DT.Tables(0).Select("Champ='argi'")
            'Dv = rw(0)("dv").ToString
            fileContent.Append(FormatNumber(dataTable.Rows(0).Item("Clay"), 4).PadLeft(8))
            fileContent.Append(FormatNumber(dataTable.Rows(0).Item("OrganicNStock"), 4).PadLeft(8))
            rw = DT.Tables(0).Select("Champ='profhum'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(FormatNumber(CDbl(Dv), 4).PadLeft(8))

            rw = DT.Tables(0).Select("Champ='calc'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(FormatNumber(CDbl(Dv), 4).PadLeft(8))

            'rw = DT.Tables(0).Select("Champ='ph'")
            'Dv = rw(0)("dv").ToString
            fileContent.Append(FormatNumber(dataTable.Rows(0).Item("Ph"), 4).PadLeft(8))

            rw = DT.Tables(0).Select("Champ='concseuil'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(FormatNumber(CDbl(Dv), 4).PadLeft(8))

            'Cmd.CommandText = "select top 1 albedo from St_param_sol"
            'rw = DT.Tables(0).Select("Champ='albedo'")
            'Dv = rw(0)("dv").ToString
            fileContent.Append(FormatNumber(dataTable.Rows(0).Item("Albedo"), 4).PadLeft(8))

            rw = DT.Tables(0).Select("Champ='q0'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(FormatNumber(CDbl(Dv), 4).PadLeft(8))

            'rw = DT.Tables(0).Select("Champ='ruisolnu'")
            'Dv = rw(0)("dv").ToString
            fileContent.Append(FormatNumber(dataTable.Rows(0).Item("RunoffCoefBSoil"), 4).PadLeft(8))

            fileContent.Append(FormatNumber(dataTable.Rows(0).Item("SoilRDepth"), 4).PadLeft(9))

            rw = DT.Tables(0).Select("Champ='pluiebat'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(FormatNumber(CDbl(Dv), 4).PadLeft(8))

            rw = DT.Tables(0).Select("Champ='mulchbat'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(FormatNumber(CDbl(Dv), 4).PadLeft(8))

            rw = DT.Tables(0).Select("Champ='zesx'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(FormatNumber(CDbl(Dv), 4).PadLeft(8))

            rw = DT.Tables(0).Select("Champ='cfes'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(FormatNumber(CDbl(Dv), 4).PadLeft(8))

            'Cmd.CommandText = "select top 1 z0solnu from St_param_sol"
            rw = DT.Tables(0).Select("Champ='z0solnu'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(FormatNumber(CDbl(Dv), 4).PadLeft(8))
            'csurnsol=soil.OrganicC/soil.OrganicNStock
            'rw = DT.Tables(0).Select("Champ='csurnsol'")
            ' Dv = rw(0)("dv").ToString
            fileContent.Append(FormatNumber(dataTable.Rows(0).Item("OrganicC") / dataTable.Rows(0).Item("OrganicNStock"), 4).PadLeft(8))

            rw = DT.Tables(0).Select("Champ='penterui'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(FormatNumber(CDbl(Dv), 4).PadLeft(8))

            fileContent.AppendLine()

            fileContent.Append("1".PadLeft(5))
            rw = DT.Tables(0).Select("Champ='codecailloux'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(FormatNumber(CDbl(Dv), 0).PadLeft(4))
            rw = DT.Tables(0).Select("Champ='codemacropor'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(FormatNumber(CDbl(Dv), 0).PadLeft(2))
            rw = DT.Tables(0).Select("Champ='codefente'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(FormatNumber(CDbl(Dv), 0).PadLeft(2))
            rw = DT.Tables(0).Select("Champ='codrainage'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(FormatNumber(CDbl(Dv), 0).PadLeft(2))
            rw = DT.Tables(0).Select("Champ='coderemontcap'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(FormatNumber(CDbl(Dv), 0).PadLeft(2))
            rw = DT.Tables(0).Select("Champ='codenitrif'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(FormatNumber(CDbl(Dv), 0).PadLeft(2))
            rw = DT.Tables(0).Select("Champ='codedenit'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(FormatNumber(CDbl(Dv), 0).PadLeft(2))
            fileContent.AppendLine()

            fileContent.Append("1".PadLeft(5))
            rw = DT.Tables(0).Select("Champ='profimper'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(FormatNumber(CDbl(Dv), 4).PadLeft(8))
            rw = DT.Tables(0).Select("Champ='ecartdrain'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(FormatNumber(CDbl(Dv), 4).PadLeft(8))
            rw = DT.Tables(0).Select("Champ='ksol'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(FormatNumber(CDbl(Dv), 4).PadLeft(8))
            rw = DT.Tables(0).Select("Champ='profdrain'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(FormatNumber(CDbl(Dv), 4).PadLeft(8))
            rw = DT.Tables(0).Select("Champ='capiljour'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(FormatNumber(CDbl(Dv), 4).PadLeft(8))
            rw = DT.Tables(0).Select("Champ='humcapil'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(FormatNumber(CDbl(Dv), 4).PadLeft(8))
            rw = DT.Tables(0).Select("Champ='profdenit'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(FormatNumber(CDbl(Dv), 0).PadLeft(5))
            rw = DT.Tables(0).Select("Champ='vpotdenit'")
            Dv = rw(0)("dv").ToString
            fileContent.Append(FormatNumber(CDbl(Dv), 0).PadLeft(5))
            fileContent.AppendLine()
            'profil du sol
            If LCase(dataTable.Rows(0).Item("soilOption")) = "simple" Then

            End If
            Dim Sql As String
            Sql = "Select * From soillayers where Lower(idsoil)= '" & LCase(dataTable.Rows(0).Item("idsoil")) & "' Order by NumLayer"
            Dim Adp As SqliteDataAdapter = New SqliteDataAdapter(Sql, MasterInput_Connection)
            ' Filling Dataset
            Dim dataSet2 As New DataSet()
            Adp.Fill(dataSet2, "st_param_sol")
            Dim dataLayer As DataTable = dataSet2.Tables("st_param_sol")
            For i = 0 To 4
                'epc hcc hmin daf
                If LCase(dataTable.Rows(0).Item("soilOption")) = "simple" Then
                    fileContent.Append("1")
                    If i = 0 Then
                        fileContent.Append(FormatNumber(dataTable.Rows(0).Item("SoilTotalDepth"), 4).PadLeft(10))
                    Else
                        fileContent.Append("  0.00 ")
                    End If
                    fileContent.Append(FormatNumber(dataTable.Rows(0).Item("Wfc") / dataTable.Rows(0).Item("Bd"), 2).PadLeft(8))
                    fileContent.Append(FormatNumber(dataTable.Rows(0).Item("Wwp") / dataTable.Rows(0).Item("Bd"), 2).PadLeft(8))
                    fileContent.Append(FormatNumber(dataTable.Rows(0).Item("Bd"), 2).PadLeft(8))
                    fileContent.Append(FormatNumber(dataTable.Rows(0).Item("cf"), 2).PadLeft(8))
                    'rw = DT.Tables(0).Select("Champ='cailloux'")
                    'Dv = rw(0)("dv").ToString
                    'fileContent.Append(FormatNumber(CDbl(Dv), 2).PadLeft(8))
                    rw = DT.Tables(0).Select("Champ='typecailloux'")
                    Dv = rw(0)("dv").ToString
                    fileContent.Append(FormatNumber(CInt(Dv), 0).PadLeft(8))
                    rw = DT.Tables(0).Select("Champ='infil'")
                    Dv = rw(0)("dv").ToString
                    fileContent.Append(FormatNumber(CInt(Dv), 0).PadLeft(5))
                    rw = DT.Tables(0).Select("Champ='epd'")
                    Dv = rw(0)("dv").ToString
                    fileContent.Append(FormatNumber(CInt(Dv), 0).PadLeft(5))
                    fileContent.AppendLine()
                Else
                    If i < dataLayer.Rows.Count Then
                        fileContent.Append("1")
                        fileContent.Append(FormatNumber(dataLayer.Rows(i).Item("Ldown") - dataLayer.Rows(i).Item("LUp"), 4).PadLeft(10))
                        fileContent.Append(FormatNumber(dataLayer.Rows(i).Item("Wfc") / dataLayer.Rows(i).Item("Bd"), 2).PadLeft(8))
                        fileContent.Append(FormatNumber(dataLayer.Rows(i).Item("Wwp") / dataLayer.Rows(i).Item("Bd"), 2).PadLeft(8))
                        fileContent.Append(FormatNumber(dataLayer.Rows(i).Item("Bd"), 2).PadLeft(8))
                        fileContent.Append(FormatNumber(dataTable.Rows(0).Item("cf"), 2).PadLeft(8))
                        'rw = DT.Tables(0).Select("Champ='cailloux'")
                        'Dv = rw(0)("dv").ToString
                        'fileContent.Append(FormatNumber(CDbl(Dv), 2).PadLeft(8))
                        rw = DT.Tables(0).Select("Champ='typecailloux'")
                        Dv = rw(0)("dv").ToString
                        fileContent.Append(FormatNumber(CInt(Dv), 0).PadLeft(8))
                        rw = DT.Tables(0).Select("Champ='infil'")
                        Dv = rw(0)("dv").ToString
                        fileContent.Append(FormatNumber(CInt(Dv), 0).PadLeft(5))
                        rw = DT.Tables(0).Select("Champ='epd'")
                        Dv = rw(0)("dv").ToString
                        fileContent.Append(FormatNumber(CInt(Dv), 0).PadLeft(5))
                        fileContent.AppendLine()
                    Else
                        fileContent.Append("1")
                        fileContent.Append("  0.00 ")
                        fileContent.Append("  0.00 ")
                        fileContent.Append("  0.00 ")
                        fileContent.Append("  0.00 ")
                        fileContent.Append("  0.00 ")
                        'rw = DT.Tables(0).Select("Champ='cailloux'")
                        'Dv = rw(0)("dv").ToString
                        'fileContent.Append(FormatNumber(CDbl(Dv), 2).PadLeft(8))
                        rw = DT.Tables(0).Select("Champ='typecailloux'")
                        Dv = rw(0)("dv").ToString
                        fileContent.Append(FormatNumber(CInt(Dv), 0).PadLeft(8))
                        rw = DT.Tables(0).Select("Champ='infil'")
                        Dv = rw(0)("dv").ToString
                        fileContent.Append(FormatNumber(CInt(Dv), 0).PadLeft(5))
                        rw = DT.Tables(0).Select("Champ='epd'")
                        Dv = rw(0)("dv").ToString
                        fileContent.Append(FormatNumber(CInt(Dv), 0).PadLeft(5))
                        fileContent.AppendLine()

                    End If
                End If

            Next

            'fileContent.Append("1")
            'fileContent.Append(FormatNumber(dataTable.Rows(0).Item("SoilTotalDepth"), 4).PadLeft(10))
            'fileContent.Append(FormatNumber(dataTable.Rows(0).Item("Wfc") / dataTable.Rows(0).Item("Bd"), 2).PadLeft(8))
            'fileContent.Append(FormatNumber(dataTable.Rows(0).Item("Wwp") / dataTable.Rows(0).Item("Bd"), 2).PadLeft(8))
            'fileContent.Append(FormatNumber(dataTable.Rows(0).Item("Bd"), 2).PadLeft(8))
            'rw = DT.Tables(0).Select("Champ='cailloux'")
            'Dv = rw(0)("dv").ToString
            'fileContent.Append(FormatNumber(CDbl(Dv), 2).PadLeft(8))
            'rw = DT.Tables(0).Select("Champ='typecailloux'")
            'Dv = rw(0)("dv").ToString
            'fileContent.Append(FormatNumber(CInt(Dv), 0).PadLeft(8))
            'rw = DT.Tables(0).Select("Champ='infil'")
            'Dv = rw(0)("dv").ToString
            'fileContent.Append(FormatNumber(CInt(Dv), 0).PadLeft(5))
            'rw = DT.Tables(0).Select("Champ='epd'")
            'Dv = rw(0)("dv").ToString
            'fileContent.Append(FormatNumber(CInt(Dv), 0).PadLeft(5))
            'fileContent.AppendLine()

            'fileContent.Append("1")
            'fileContent.Append("  0.00 ")
            'fileContent.Append(FormatNumber(dataTable.Rows(0).Item("Wfc") / dataTable.Rows(0).Item("Bd"), 2).PadLeft(8))
            'fileContent.Append(FormatNumber(dataTable.Rows(0).Item("Wwp") / dataTable.Rows(0).Item("Bd"), 2).PadLeft(8))
            'fileContent.Append(FormatNumber(dataTable.Rows(0).Item("Bd"), 2).PadLeft(8))
            'rw = DT.Tables(0).Select("Champ='cailloux'")
            'Dv = rw(0)("dv").ToString
            'fileContent.Append(FormatNumber(CDbl(Dv), 2).PadLeft(8))
            'rw = DT.Tables(0).Select("Champ='typecailloux'")
            'Dv = rw(0)("dv").ToString
            'fileContent.Append(FormatNumber(CInt(Dv), 0).PadLeft(8))
            'rw = DT.Tables(0).Select("Champ='infil'")
            'Dv = rw(0)("dv").ToString
            'fileContent.Append(FormatNumber(CInt(Dv), 0).PadLeft(5))
            'rw = DT.Tables(0).Select("Champ='epd'")
            'Dv = rw(0)("dv").ToString
            'fileContent.Append(FormatNumber(CInt(Dv), 0).PadLeft(5))
            'fileContent.AppendLine()
            ''profil du sol
            'fileContent.Append("1")
            'fileContent.Append("  0.00 ")
            'fileContent.Append(FormatNumber(dataTable.Rows(0).Item("Wfc") / dataTable.Rows(0).Item("Bd"), 2).PadLeft(8))
            'fileContent.Append(FormatNumber(dataTable.Rows(0).Item("Wwp") / dataTable.Rows(0).Item("Bd"), 2).PadLeft(8))
            'fileContent.Append(FormatNumber(dataTable.Rows(0).Item("Bd"), 2).PadLeft(8))
            'rw = DT.Tables(0).Select("Champ='cailloux'")
            'Dv = rw(0)("dv").ToString
            'fileContent.Append(FormatNumber(CDbl(Dv), 2).PadLeft(8))
            'rw = DT.Tables(0).Select("Champ='typecailloux'")
            'Dv = rw(0)("dv").ToString
            'fileContent.Append(FormatNumber(CInt(Dv), 0).PadLeft(8))
            'rw = DT.Tables(0).Select("Champ='infil'")
            'Dv = rw(0)("dv").ToString
            'fileContent.Append(FormatNumber(CInt(Dv), 0).PadLeft(5))
            'rw = DT.Tables(0).Select("Champ='epd'")
            'Dv = rw(0)("dv").ToString
            'fileContent.Append(FormatNumber(CInt(Dv), 0).PadLeft(5))
            'fileContent.AppendLine()


            'fileContent.Append("1")
            'fileContent.Append("  0.00 ")
            'fileContent.Append(FormatNumber(dataTable.Rows(0).Item("Wfc") / dataTable.Rows(0).Item("Bd"), 2).PadLeft(8))
            'fileContent.Append(FormatNumber(dataTable.Rows(0).Item("Wwp") / dataTable.Rows(0).Item("Bd"), 2).PadLeft(8))
            'fileContent.Append(FormatNumber(dataTable.Rows(0).Item("Bd"), 2).PadLeft(8))
            'rw = DT.Tables(0).Select("Champ='cailloux'")
            'Dv = rw(0)("dv").ToString
            'fileContent.Append(FormatNumber(CDbl(Dv), 2).PadLeft(8))
            'rw = DT.Tables(0).Select("Champ='typecailloux'")
            'Dv = rw(0)("dv").ToString
            'fileContent.Append(FormatNumber(CInt(Dv), 0).PadLeft(8))
            'rw = DT.Tables(0).Select("Champ='infil'")
            'Dv = rw(0)("dv").ToString
            'fileContent.Append(FormatNumber(CInt(Dv), 0).PadLeft(5))
            'rw = DT.Tables(0).Select("Champ='epd'")
            'Dv = rw(0)("dv").ToString
            'fileContent.Append(FormatNumber(CInt(Dv), 0).PadLeft(5))
            'fileContent.AppendLine()


            'fileContent.Append("1")
            'fileContent.Append("  0.00 ")
            'fileContent.Append(FormatNumber(dataTable.Rows(0).Item("Wfc") / dataTable.Rows(0).Item("Bd"), 2).PadLeft(8))
            'fileContent.Append(FormatNumber(dataTable.Rows(0).Item("Wwp") / dataTable.Rows(0).Item("Bd"), 2).PadLeft(8))
            'fileContent.Append(FormatNumber(dataTable.Rows(0).Item("Bd"), 2).PadLeft(8))
            'rw = DT.Tables(0).Select("Champ='cailloux'")
            'Dv = rw(0)("dv").ToString
            'fileContent.Append(FormatNumber(CDbl(Dv), 2).PadLeft(8))
            'rw = DT.Tables(0).Select("Champ='typecailloux'")
            'Dv = rw(0)("dv").ToString
            'fileContent.Append(FormatNumber(CInt(Dv), 0).PadLeft(8))
            'rw = DT.Tables(0).Select("Champ='infil'")
            'Dv = rw(0)("dv").ToString
            'fileContent.Append(FormatNumber(CInt(Dv), 0).PadLeft(5))
            'rw = DT.Tables(0).Select("Champ='epd'")
            'Dv = rw(0)("dv").ToString
            'fileContent.Append(FormatNumber(CInt(Dv), 0).PadLeft(5))
            'fileContent.AppendLine()

        Next

        Try
            ' Export file to specified directory
            WriteFile(DirectoryPath, fileName, fileContent.ToString())
        Catch ex As Exception
            MessageBox.Show("Error during writing file :  " + ex.Message)
        End Try
    End Sub
    Public Overrides Sub Import(DirectoryPath As String, model As String)

    End Sub
End Class


