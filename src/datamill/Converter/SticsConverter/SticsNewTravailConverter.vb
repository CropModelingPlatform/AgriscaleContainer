Imports System
Imports System.IO
Imports System.Text
Imports System.Globalization
Imports System.Configuration
Imports System.Data
Imports System.Data.Sqlite

Public Class SticsNewTravailConverter
    
    Inherits Converter
    
    Public Overrides Sub Export(DirectoryPath As String)
    End Sub

    Public Overrides Sub Export(
            DirectoryPath As String,
            MasterInput_Connection As SqliteConnection,
            ModelDictionary_Connection As SqliteConnection)

        Dim fileName As String = "new_travail.usm"
        Dim fileContent As StringBuilder = New StringBuilder()
        Dim Dv As String
        Dim Bissext As Integer
        Dim ST(3) As String
        ST = DirectoryPath.Split(Path.DirectorySeparatorChar)
        'MsgBox(DirectoryPath)
        DirectoryPath = ST(0) & Path.DirectorySeparatorChar & ST(1) & Path.DirectorySeparatorChar & ST(2) & Path.DirectorySeparatorChar & ST(3) & Path.DirectorySeparatorChar & ST(4) & Path.DirectorySeparatorChar & ST(5) & Path.DirectorySeparatorChar & ST(6) & Path.DirectorySeparatorChar & ST(7)
        'tempoparv6 query
        Dim fetchAllQuery As String = "SELECT SimUnitList.idsim, SimUnitList.idPoint as idPoint, SimUnitList.StartYear,SimUnitList.StartDay,SimUnitList.EndDay,SimUnitList.Endyear, SimUnitList.idsoil, SimUnitList.idMangt, SimUnitList.idIni, Coordinates.LatitudeDD, CropManagement.sowingdate, " _
        & " ListCultivars.SpeciesName FROM InitialConditions INNER JOIN ((ListCultivars INNER JOIN CropManagement ON ListCultivars.IdCultivar = CropManagement.Idcultivar) INNER JOIN (Coordinates INNER " _
        & "Join SimUnitList ON Coordinates.idPoint = SimUnitList.idPoint) ON CropManagement.idMangt = SimUnitList.idMangt) ON InitialConditions.idIni = SimUnitList.idIni Where idsim ='" + ST(7) + "';"
        'Console.WriteLine("SQL:" + fetchAllQuery)
        Dim T As String = "Select  Champ, Default_Value_Datamill, defaultValueOtherSource, IFNULL([defaultValueOtherSource],  [Default_Value_Datamill]) As dv From Variables Where ((model = 'stics') And ([Table] = 'st_new_travail'));"
        Dim DT As New DataSet()
        Dim dataSet As New DataSet()
        Dim rw() As DataRow
        Dim Cmd As New SqliteDataAdapter(T, ModelDictionary_Connection)
        Cmd.Fill(DT, "TChamp")        'Init and use DataAdapter
        Dim DA = New SqliteDataAdapter(fetchAllQuery, MasterInput_Connection)
        'MsgBox(fetchAllQuery)
        ' Filling Dataset
        Dim DSTrav As New DataSet()
        DA.Fill(DSTrav)
        Dim DTable As DataTable = DSTrav.Tables(0)
        'read all line of new_travail

        fileContent.AppendLine(":codesimul")
        rw = DT.Tables(0).Select("Champ='codesimul'")
        Dv = rw(0)("dv").ToString
        fileContent.AppendLine(Dv)
        fileContent.AppendLine(":codeoptim") 'dv 0
        rw = DT.Tables(0).Select("Champ='codeoptim'")
        Dv = rw(0)("dv").ToString
        fileContent.AppendLine(Dv)
        fileContent.AppendLine(":codesuite") 'dv 0
        rw = DT.Tables(0).Select("Champ='codesuite'")
        Dv = rw(0)("dv").ToString
        fileContent.AppendLine(Dv)
        fileContent.AppendLine(":nbplantes") 'dv 1
        rw = DT.Tables(0).Select("Champ='nbplantes'")
        Dv = rw(0)("dv").ToString
        fileContent.AppendLine(Dv)
        fileContent.AppendLine(":nom")
        fileContent.AppendLine(DTable.Rows(0)("SpeciesName"))
        
        fileContent.AppendLine(":datedebut")
        'fileContent.AppendLine(DTable.Rows(0)("sowingdate"))
        fileContent.AppendLine(DTable.Rows(0)("startday"))
        fileContent.AppendLine(":datefin") 'endday
        'If DTable.Rows(0)("latitudeDD") < 0 Then
        If CInt(DTable.Rows(0).Item("StartYear")) Mod 4 = 0 Then
            Bissext = 1
        Else
            Bissext = 0
        End If

        If CInt(DTable.Rows(0)("StartYear")) <> CInt(DTable.Rows(0)("Endyear")) Then
            fileContent.AppendLine(DTable.Rows(0)("endday") + 365 + Bissext)
        Else
            fileContent.AppendLine(DTable.Rows(0)("endday"))
        End If
        fileContent.AppendLine(":finit") 'idini
        fileContent.AppendLine("ficini.txt") '        fileContent.AppendLine(DTable.Rows(0)("idini"))
        fileContent.AppendLine(":numsol")
        fileContent.AppendLine("1")
        fileContent.AppendLine(":nomsol")
        fileContent.AppendLine("param.sol")
        'fileContent.AppendLine(DTable.Rows(0)("idsoil"))
        fileContent.AppendLine(":fstation")
        fileContent.AppendLine("station.txt")
        fileContent.AppendLine(":fclim1")
        fileContent.AppendLine("cli" & DTable.Rows(0)("idpoint") & "j." & DTable.Rows(0)("StartYear"))
        fileContent.AppendLine(":fclim2")
        fileContent.AppendLine("cli" & DTable.Rows(0)("idpoint") & "j." & (CInt(DTable.Rows(0)("StartYear")) + 1).ToString)
        fileContent.AppendLine(":nbans")
        'If DTable.Rows(0)("latitudeDD") < 0 Then
        If CInt(DTable.Rows(0)("StartYear")) <> CInt(DTable.Rows(0)("Endyear")) Then
            fileContent.AppendLine("2")
        Else
            fileContent.AppendLine("1")
        End If
        fileContent.AppendLine(":culturean")
        'If DTable.Rows(0)("latitudeDD") < 0 Then
        If CInt(DTable.Rows(0)("StartYear")) <> CInt(DTable.Rows(0)("Endyear")) Then
            fileContent.AppendLine("2")
        Else
            fileContent.AppendLine("1")
        End If
        fileContent.AppendLine(":fplt1")
        fileContent.AppendLine("ficplt1.txt") 'fileContent.AppendLine(DTable.Rows(0)("SpeciesName"))
        fileContent.AppendLine(":ftec1")
        fileContent.AppendLine("fictec1.txt") 'fileContent.AppendLine(DTable.Rows(0)("idmangt"))
        fileContent.AppendLine(":flai1")
        'fileContent.AppendLine(Dv)
        rw = DT.Tables(0).Select("Champ='flai1'")
        Dv = rw(0)("dv").ToString
        fileContent.AppendLine(Dv)
        ' Next
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

