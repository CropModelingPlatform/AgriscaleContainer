Imports System
Imports System.IO
Imports System.Text
Imports System.Globalization
Imports System.Configuration
Imports System.Data
Imports System.Data.Sqlite

Public Class SticsFiciniConverter
    
    Inherits Converter
    
    Public Overrides Sub Export(DirectoryPath As String)
    End Sub

    Public Overrides Sub Export(
            DirectoryPath As String,
            MasterInput_Connection As SqliteConnection,
            ModelDictionary_Connection As SqliteConnection)

        Dim fileName As String = "ficini.txt"
        Dim fileContent As StringBuilder = New StringBuilder()

        Dim ST(3) As String
        ST = DirectoryPath.Split(Path.DirectorySeparatorChar)
        DirectoryPath = ST(0) & Path.DirectorySeparatorChar & ST(1) & Path.DirectorySeparatorChar & ST(2) & Path.DirectorySeparatorChar & ST(3) & Path.DirectorySeparatorChar & ST(4) & Path.DirectorySeparatorChar & ST(5) & Path.DirectorySeparatorChar & ST(6) & Path.DirectorySeparatorChar & ST(7)
        'Ficini query
        Dim T As String = "Select  Champ, Default_Value_Datamill, defaultValueOtherSource, IFNULL([defaultValueOtherSource],  [Default_Value_Datamill]) As dv From Variables Where ((model = 'stics') And ([Table] = 'st_ficini'));"
        Dim DT As New DataSet()
        Dim rw() As DataRow
        Dim Sql As String

        Dim Cmd As New SqliteDataAdapter(T, ModelDictionary_Connection)
        Cmd.Fill(DT, "TChamp")

        Dim fetchAllQuery As String = "SELECT SimUnitList.idIni, Soil.IdSoil, Soil.SoilOption, Soil.Wwp, Soil.Wfc, Soil.bd, InitialConditions.WStockinit, InitialConditions.Ninit " _
        & "FROM InitialConditions INNER JOIN (Soil INNER JOIN SimUnitList ON Lower(Soil.IdSoil) = Lower(SimUnitList.idsoil)) ON InitialConditions.idIni = SimUnitList.idIni " _
        & " where idSim='" + ST(7) + "';"

'        Console.WriteLine("SQL 2 : " & fetchAllQuery)

        'Init and use DataAdapter
        Using dataAdapter As SqliteDataAdapter = New SqliteDataAdapter(fetchAllQuery, MasterInput_Connection)
            ' Filling Dataset
            Dim dataSet As New DataSet()
            dataAdapter.Fill(dataSet, "st_ficini")
            Dim dataTable As DataTable = dataSet.Tables("st_ficini")

            'read all line of st_ficini
            For Each row In dataTable.Rows
                fileContent.AppendLine(":nbplantes:")
                rw = DT.Tables(0).Select("Champ='nbplantes'")
                fileContent.AppendLine(rw(0)("dv").ToString)
                fileContent.AppendLine(":plante:")
                rw = DT.Tables(0).Select("Champ='stade0'")
                fileContent.AppendLine(rw(0)("dv").ToString)
                rw = DT.Tables(0).Select("Champ='lai0'")
                fileContent.AppendLine(rw(0)("dv").ToString)
                rw = DT.Tables(0).Select("Champ='masec0'")
                fileContent.AppendLine(rw(0)("dv").ToString)
                rw = DT.Tables(0).Select("Champ='zrac0'")
                fileContent.AppendLine(rw(0)("dv").ToString)
                rw = DT.Tables(0).Select("Champ='magrain0'")
                fileContent.AppendLine(rw(0)("dv").ToString)
                rw = DT.Tables(0).Select("Champ='qnplante0'")
                fileContent.AppendLine(rw(0)("dv").ToString)
                rw = DT.Tables(0).Select("Champ='resperenne0'")
                fileContent.AppendLine(rw(0)("dv").ToString)
                fileContent.AppendLine(":densinitial:")
                rw = DT.Tables(0).Select("Champ='densinitial'")
                fileContent.AppendLine(rw(0)("dv").ToString & " 0.0 0.0 0.0 0.0")
                fileContent.AppendLine(":plante:")
                fileContent.AppendLine()
                fileContent.AppendLine()
                fileContent.AppendLine()
                fileContent.AppendLine()
                fileContent.AppendLine()
                fileContent.AppendLine()
                fileContent.AppendLine()
                fileContent.AppendLine()
                fileContent.AppendLine(":densinitial:")
                fileContent.AppendLine("     ")
                Sql = "Select * From soillayers where Lower(idsoil)= '" & LCase(dataTable.Rows(0).Item("idsoil")) & "' Order by NumLayer"
                Dim Adp As New SqliteDataAdapter(Sql, MasterInput_Connection)
                Dim jeu As New DataSet
                Adp.Fill(jeu)
                fileContent.AppendLine(":hinit:")
                'if soilOption= "simple" then 
                '       (Wwp+Wstockinit*(Wfc-Wwp)/100)/bd for layer 1 
                'Else If soilOption = "detailed" Then 
                '       (SoilLayer.Wwp+Wstockinit*(SoilLayer.Wfc-SoilLayer.Wwp)/100)/SoilLayer.bd 
                '       For Each Of the five soil layers
                If LCase(dataTable.Rows(0).Item("soilOption")) = "simple" Then
                    'fileContent.Append(FormatNumber(dataTable.Rows(0).Item("Wfc") / dataTable.Rows(0).Item("Bd"), 4).PadLeft(8))
                    fileContent.Append(FormatNumber((dataTable.Rows(0).Item("Wwp") + dataTable.Rows(0).Item("WStockinit") * (dataTable.Rows(0).Item("Wfc") - dataTable.Rows(0).Item("Wwp")) / 100) / dataTable.Rows(0).Item("Bd"), 4).PadLeft(8))
                    fileContent.AppendLine(" 0.0 0.0 0.0 0.0")
                Else
                    For i = 0 To 4
                        If i < jeu.Tables(0).Rows.Count Then
                            row = jeu.Tables(0).Rows(i)
                            fileContent.Append(FormatNumber((row("Wwp") + dataTable.Rows(0).Item("WStockinit") * (row("Wfc") - row("Wwp")) / 100) / row("Bd"), 4).PadLeft(8))
                        Else
                            fileContent.Append(" 0.0")
                        End If
                    Next
                    fileContent.AppendLine()
                End If
                'fileContent.AppendLine(" 0.0 0.0 0.0 0.0")
                fileContent.AppendLine(":NO3init:")
                'if soilOption= "simple" then Ninit for layer 1 (and zero for the other layers)  else if  soilOption= "detailed" then Ninit/5 for each of the 5 layers
                If LCase(dataTable.Rows(0).Item("soilOption")) = "simple" Then
                    fileContent.AppendLine(FormatNumber(dataTable.Rows(0).Item("Ninit"), 1).ToString.PadLeft(5) & " 0.0 0.0 0.0 0.0")
                Else
                    For i = 0 To 4
                        If i < jeu.Tables(0).Rows.Count Then
                            fileContent.Append(FormatNumber(dataTable.Rows(0).Item("Ninit") / jeu.Tables(0).Rows.Count, 1).ToString.PadLeft(5))
                        Else
                            fileContent.Append(" 0.0")
                        End If
                    Next
                    fileContent.AppendLine()
                End If
                'rw = DT.Tables(0).Select("Champ='NO3initf'")
                'fileContent.AppendLine(rw(0)("dv").ToString & " 0.0 0.0 0.0 0.0")
                fileContent.AppendLine(":NH4init:")
                rw = DT.Tables(0).Select("Champ='NH4initf'")
                fileContent.AppendLine(rw(0)("dv").ToString & " 0.0 0.0 0.0 0.0")
            Next
            fileContent.AppendLine()
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


