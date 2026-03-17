Imports System
Imports System.IO
Imports System.Text
Imports System.Globalization
Imports System.Configuration
Imports System.Data
Imports System.Data.Sqlite
Imports System.Threading.Tasks

Public Class CelsiusConverter
    Inherits Converter

    Public Sub New()
        MasterInput_Connection = New SqliteConnection()
        MasterInput_Connection.ConnectionString = GlobalVariables.dbMasterInput

        ' ModelDictionary_Connection = New SqliteConnection()
        ' ModelDictionary_Connection.ConnectionString = GlobalVariables.dbModelsDictionary
    End Sub

    Public Overrides Sub Export(
            DirectoryPath As String,
            MasterInput_Connection As SqliteConnection,
            ModelDictionary_Connection As SqliteConnection)
    End Sub

    Public Overrides Sub Export(DirectoryPath As String)

        Try
            Console.WriteLine("dbMasterInput : " + MasterInput_Connection.ConnectionString)
            ' Console.WriteLine("dbModelsDictionary : " + ModelDictionary_Connection.ConnectionString )

            MasterInput_Connection.Open()
            ' ModelDictionary_Connection.Open()
        Catch ex As Exception
            Console.WriteLine("Connection Error : "  & ex.Message)
        End Try
        
        Directory.CreateDirectory(DirectoryPath)

        Dim createIndexQuery2 As String = "CREATE INDEX IF NOT EXISTS idx_idsim ON SimUnitList (idsim);"
        Dim command3 As New SQLiteCommand(createIndexQuery2, MasterInput_Connection)
        command3.ExecuteNonQuery()

        'Dim createIndexQuery As String = "CREATE INDEX IF NOT EXISTS idx_idPoint_year ON RaClimateD (idPoint, year);"
        'Dim command_clim As New SQLiteCommand(createIndexQuery, MasterInput_Connection)
        'command_clim.ExecuteNonQuery()

        'Dim createIndexQuery_idClim As String = "CREATE INDEX IF NOT EXISTS idx_idPoint ON RaClimateD (idPoint);"
        'Dim command_idclim As New SQLiteCommand(createIndexQuery_idClim, MasterInput_Connection)
        'command_idclim.ExecuteNonQuery()

        Dim createIndexQuery_idcoord As String = "CREATE INDEX IF NOT EXISTS idx_idCoord ON Coordinates (idPoint);"
        Dim command_idcoord As New SQLiteCommand(createIndexQuery_idcoord, MasterInput_Connection)
        command_idcoord.ExecuteNonQuery()

        Dim createIndexQuery_idcropmgt As String = "CREATE INDEX IF NOT EXISTS idx_idMangt ON CropManagement (idMangt);"
        Dim command_idcropmgt As New SQLiteCommand(createIndexQuery_idcropmgt, MasterInput_Connection)
        command_idcropmgt.ExecuteNonQuery()

        Dim createIndexQuery_idsoil As String = "CREATE INDEX IF NOT EXISTS idx_idsoil ON Soil (IdSoil);"
        Dim command_idsoil As New SQLiteCommand(createIndexQuery_idsoil, MasterInput_Connection)
        command_idsoil.ExecuteNonQuery()

        Dim createIndexQuery_idsoill As String = "CREATE INDEX IF NOT EXISTS idx_idsoill ON Soil (Lower(IdSoil));"
        Dim command_idsoill As New SQLiteCommand(createIndexQuery_idsoill, MasterInput_Connection)
        command_idsoill.ExecuteNonQuery()

        Dim createIndexQuery_idsoiltl As String = "CREATE INDEX IF NOT EXISTS idx_idsoiltl ON SoilTypes (Lower(SoilTextureType));"
        Dim command_idsoiltl As New SQLiteCommand(createIndexQuery_idsoiltl, MasterInput_Connection)
        command_idsoiltl.ExecuteNonQuery()

        Dim createIndexQuery_idoption As String = "CREATE INDEX IF NOT EXISTS idx_idoption ON SimulationOptions (idOptions);"
        Dim command_idoption As New SQLiteCommand(createIndexQuery_idoption, MasterInput_Connection)
        command_idoption.ExecuteNonQuery()

        Dim createIndexQuery_listcult As String = "CREATE INDEX IF NOT EXISTS idx_cultivars ON ListCultivars (idCultivar);"
        Dim command_listcult As New SQLiteCommand(createIndexQuery_listcult, MasterInput_Connection)
        command_listcult.ExecuteNonQuery()

        Dim createIndexQuery_listcultvOpt As String = "CREATE INDEX IF NOT EXISTS idx_cultopt ON ListCultivars (CodePSpecies);"
        Dim command_listcultvOpt As New SQLiteCommand(createIndexQuery_listcultvOpt, MasterInput_Connection)
        command_listcultvOpt.ExecuteNonQuery()

        Dim createIndexQuery_listcultOpt As String = "CREATE INDEX IF NOT EXISTS idx_cultoptspec ON ListCultOption (CodePSpecies);"
        Dim command_listcultOpt As New SQLiteCommand(createIndexQuery_listcultOpt, MasterInput_Connection)
        command_listcultOpt.ExecuteNonQuery()

        Dim createIndexQuery_org As String = "CREATE INDEX IF NOT EXISTS idx_orga ON OrganicFOperations (idFertOrga);"
        Dim command_org As New SQLiteCommand(createIndexQuery_org, MasterInput_Connection)
        command_org.ExecuteNonQuery()

        Dim createIndexQuery_orgR As String = "CREATE INDEX IF NOT EXISTS idx_orga_res ON OrganicFOperations (TypeResidues);"
        Dim command_orgR As New SQLiteCommand(createIndexQuery_orgR, MasterInput_Connection)
        command_orgR.ExecuteNonQuery()

        Dim createIndexQuery_res As String = "CREATE INDEX IF NOT EXISTS idx_res ON ListResidues (TypeResidues);"
        Dim command_res As New SQLiteCommand(createIndexQuery_res, MasterInput_Connection)
        command_res.ExecuteNonQuery()

        'Dim createIndexQuery4 As String = "CREATE INDEX IF NOT EXISTS idx_model_table ON Variables (model, [Table]);"
        'Dim command4 As New SQLiteCommand(createIndexQuery4, ModelDictionary_Connection)
        'command4.ExecuteNonQuery()

        Dim Sql3, Sql1, Sql2, Sql4, Sql5 As String
        Dim i As Integer
        Dim Sorg, SMin As Double
        Dim R, R2, R3, R4 As DataRow
        Dim restrictions(3) As String
        Dim Ap_ADP As New SqliteDataAdapter()
        Dim Ap_ADP1 As New SqliteDataAdapter()
        Dim Ap_ADP2 As New SqliteDataAdapter()
        Dim Ap_ADP3 As New SqliteDataAdapter()
        Dim Jeu As New DataSet ' Associé aux noms des simulations et a R
        Dim Jeu1 As New DataSet
        Dim Jeu2 As New DataSet
        Dim Jeu3 As New DataSet
        Dim Jeu4 As New DataSet
        Dim command As SqliteCommand
        ' Dim MasterInput_Connection = New SqliteConnection
        ' MasterInput_Connection.ConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" & RepSource & "\MasterInput.accdb"
        Dim Connection = New SqliteConnection
        Connection.ConnectionString = GlobalVariables.dbCelsius ' "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" & RepSource & "\Celsius\CelsiusV3nov17_dataArise.accdb"
        Try
            Console.WriteLine("dbCelsius : " + Connection.ConnectionString)
            'Open DB connection
            Connection.Open()

            Dim sql_pragma As String = "PRAGMA synchronous = OFF"
            command = New SqliteCommand(sql_pragma, Connection)
            command.ExecuteNonQuery()
            command.Dispose()
            
            sql_pragma = "PRAGMA journal_mode = MEMORY"
            command = New SqliteCommand(sql_pragma, Connection)
            command.ExecuteNonQuery()
            command.Dispose()                    
        Catch ex As Exception
            Console.WriteLine("Connection Error : " + ex.Message)
        End Try        '---------------------------
        'Console.WriteLine("select RAClimated")
        'Jeu1.Clear()
        'Sql1 = "Select idPoint,year,DOY,Nmonth,NdayM,srad,tmax,tmin,tmoy,rain,Etppm from RAclimateD Where  Idpoint in (select distinct idpoint from simunitlist)"
        'Ap_ADP1 = New SqliteDataAdapter(Sql1, MasterInput_Connection)
        'Ap_ADP1.Fill(Jeu1)

        'Console.WriteLine("delete Dweather")
        'Sql2 = "Delete from Dweather"
        'command = New SqliteCommand(Sql2, Connection)
        'command.ExecuteNonQuery()
        'command.Dispose()
        'Console.WriteLine("fill Dweather toto: " + CStr(Jeu1.Tables(0).Rows.Count))
        'For i = 0 To Jeu1.Tables(0).Rows.Count - 1
            'R = Jeu1.Tables(0).Rows(i)
            'Sql2 = "insert into Dweather (IdDClim,idjourclim,annee,jda,mois,jour,rg,tmax,tmin,tmoy,plu,Etp) values ('" & R("idPoint") & "','" & R("idPoint") & "." & R("Year").ToString & "." & R("DOY").ToString & "'," & R("Year") & "," & R("DOY") & "," & R("Nmonth") & "," & R("NdayM") & "," & R("srad") & "," & R("tmax") & "," & R("tmin") & "," & R("tmoy") & "," & R("rain") & "," & R("Etppm") & ");"
            'command = New SqliteCommand(Sql2, Connection)
            'command.ExecuteNonQuery()
            ' If Form1.msgErr_expCelsius_export.Text <> "Weather " & R.Item("idpoint") Then
            '     Form1.msgErr_expCelsius_export.Text = "Weather " & R.Item("idpoint")
            '     Form1.msgErr_expCelsius_export.Refresh()
            ' End If
            'command.Dispose()
        'Next
        '----------------------------
        Jeu1.Clear()
        Jeu2.Clear()
        Sql1 = "Select idPoint,latitudeDD,longitudeDD,altitude from Coordinates"
        Ap_ADP1 = New SqliteDataAdapter(Sql1, MasterInput_Connection)
        Ap_ADP1.Fill(Jeu1)
        Sql2 = "Delete from ListPAnnexes"
        command = New SqliteCommand(Sql2, Connection)
        command.ExecuteNonQuery()
        command.Dispose()
        Sql3 = "Select * from ListPAnnexesDV"
        Ap_ADP2 = New SqliteDataAdapter(Sql3, Connection)
        Ap_ADP2.Fill(Jeu2)
        R2 = Jeu2.Tables(0).Rows(0)
        Console.WriteLine("ListPAnnexes")
        For i = 0 To Jeu1.Tables(0).Rows.Count - 1
            R = Jeu1.Tables(0).Rows(i)
            If IsDBNull(R("altitude")) Then
                R("altitude") = -99
            End If
            Sql2 = "insert into ListPAnnexes (IdDClim,latitudeDD,longitude,altitude,CO2c,ConcNPlu) values ('" & R("idPoint") & "'," & R("latitudeDD") & "," & R("longitudeDD") & "," & R("altitude") & "," & R2("CO2c") & "," & R2("ConcNPlu") & ")"
            command = New SqliteCommand(Sql2, Connection)
            command.ExecuteNonQuery()
            ' Form1.msgErr_expCelsius_export.Text = "ListPAnnexes " & R("idpoint")
            ' Form1.msgErr_expCelsius_export.Refresh()
            command.Dispose()
        Next
        '----------------------------    
        Jeu1.Clear()
        Jeu2.Clear()
        Sql1 = "Select IdIni,Wstockinit from InitialConditions"
        Ap_ADP1 = New SqliteDataAdapter(Sql1, MasterInput_Connection)
        Ap_ADP1.Fill(Jeu1)
        Sql2 = "Delete from ParamIni"
        command = New SqliteCommand(Sql2, Connection)
        command.ExecuteNonQuery()
        command.Dispose()
        Sql3 = "Select * from ParamIniDV"
        Ap_ADP2 = New SqliteDataAdapter(Sql3, Connection)
        Ap_ADP2.Fill(Jeu2)
        R2 = Jeu2.Tables(0).Rows(0)
        For i = 0 To Jeu1.Tables(0).Rows.Count - 1
            R = Jeu1.Tables(0).Rows(i)
            Sql2 = "insert into ParamIni (IdIni,Qpaillisinit,Stockinit,iniSolhautON) values ('" & R("IdIni") & "'," & R2("Qpaillisinit") & "," & R("Wstockinit") & "," & R2("iniSolhautON") & ")"
            command = New SqliteCommand(Sql2, Connection)
            command.ExecuteNonQuery()
            ' Form1.msgErr_expCelsius_export.Text = "ParamIni " & R.Item("idini")
            ' Form1.msgErr_expCelsius_export.Refresh()
            command.Dispose()
        Next
        '----------------------------    
        Jeu1.Clear()
        Jeu2.Clear()
        Jeu3.Clear()
        Sql1 = "SELECT CropManagement.idMangt, CropManagement.sdens, CropManagement.sowingdate, CropManagement.OFertiPolicyCode," &
        "CropManagement.InoFertiPolicyCode, ListCultivars.IdcultivarCelsius FROM ListCultivars INNER JOIN CropManagement ON ListCultivars.IdCultivar = CropManagement.Idcultivar;"
        Ap_ADP1 = New SqliteDataAdapter(Sql1, MasterInput_Connection)
        Ap_ADP1.Fill(Jeu1)
        Sql2 = "Delete from Tech_Commun"
        command = New SqliteCommand(Sql2, Connection)
        command.ExecuteNonQuery()
        command.Dispose()
        Sql2 = "Delete from Tech_perCrop"
        command = New SqliteCommand(Sql2, Connection)
        command.ExecuteNonQuery()
        command.Dispose()
        Sql3 = "Select * from Tech_CommunDV"
        Ap_ADP2 = New SqliteDataAdapter(Sql3, Connection)
        Ap_ADP2.Fill(Jeu2)
        R2 = Jeu2.Tables(0).Rows(0)
        Sql4 = "Select * from Tech_perCropDV"
        Ap_ADP2 = New SqliteDataAdapter(Sql4, Connection)
        Ap_ADP2.Fill(Jeu3)
        R3 = Jeu3.Tables(0).Rows(0)
        For i = 0 To Jeu1.Tables(0).Rows.Count - 1
            'CodParamMulch :IdResiduesCelsius corresponding to Typeresidues of first OFnumber in OrganicFOperations 
            '               For which In_OnManure="on" And OFertiPolicyCode=CropManangement.OFertiPolicyCode
            '               (If ResiduesInOn = "on").If no such Case Then  "1"
            'imulch : sowingdate+OrganicFOperations.Dferti of first OFnumber in OrganicFOperations 
            '           For which In_OnManure="on" and OFertiPolicyCode=CropManangement.OFertiPolicyCode
            'QpaillisApport : Qmanure for first OFnumber in OrganicFOperations 
            '                   For which In_OnManure="on" And OFertiPolicyCode=CropManangement.OFertiPolicyCode
            '               If ResiduesInOn = "on".If no such Case Then  0
            R = Jeu1.Tables(0).Rows(i)
            Sql5 = "SELECT OrganicFOperations.OFertiPolicyCode, OrganicFOperations.OFNumber, OrganicFOperations.Dferti, " _
                & "OrganicFOperations.Qmanure, OrganicFOperations.TypeResidues, OrganicFOperations.In_OnManure, " _
                & "ListResidues.IdResidueCelsius FROM ListResidues INNER JOIN OrganicFOperations ON ListResidues.TypeResidues = " _
                & "OrganicFOperations.TypeResidues Where ((OrganicFOperations.In_OnManure='on') and ((OrganicFOperations.OFertiPolicyCode) = '" & R("OfertiPolicyCode") & "' )) order by OFNumber;"
            Jeu4.Clear()
            Ap_ADP2 = New SqliteDataAdapter(Sql5, MasterInput_Connection)
            Ap_ADP2.Fill(Jeu4)
            If Jeu4.Tables(0).Rows.Count > 0 Then
                R4 = Jeu4.Tables(0).Rows(0)
                'If ResiduesInOn="On" then Qresidues else 0
                R2("imulch") = R("sowingdate") + R4("Dferti")
                R2("QPaillisApport") = R4("Qmanure")
                R2("CodParamMulch") = R4("IdResidueCelsius") 'command.ExecuteScalar
            Else
                R2("imulch") = 0
                R2("CodParamMulch") = "1"
                R2("QPaillisApport") = 0
                'End If
            End If
            '            Sum of InorganicFOperations.N (for the NumInorganicFerti of the InorgFertiPolicyCode)
            Sql1 = "SELECT Sum(InOrganicFOperations.N) AS SommeDeNFerti FROM InOrganicFOperations GROUP BY InOrganicFOperations.InOrgFertiPolicyCode HAVING (((InOrganicFOperations.InOrgFertiPolicyCode)='" & R("InOFertiPolicyCode") & "'));"
            command = New SqliteCommand(Sql1, MasterInput_Connection)
            SMin = command.ExecuteScalar()
            command.Dispose()
            'Sum of OrganicFOperations.Nferti x Qmanure (for the  NumOrganicFerti of the OFertiPolicyCode)
            Sql1 = "SELECT Sum(OrganicFOperations.NFerti * OrganicFOperations.QManure) AS SommeDeNFerti FROM OrganicFOperations GROUP BY OrganicFOperations.OFertiPolicyCode HAVING (((OrganicFOperations.OFertiPolicyCode)='" & R("OFertiPolicyCode") & "'));"
            command = New SqliteCommand(Sql1, MasterInput_Connection)
            Sorg = command.ExecuteScalar()
            'Sorg = 1
            command.Dispose()
            Sql2 = "insert into Tech_Commun (IdTech_Com,imulch,CodParamMulch,QPaillisApport,AltiCult, DriveRuiObs, fertiminON, fertiorgON, IrrigON, NbCult,NomSC,SerreTunnelON,tApportMon,tApportMinN) values " &
            "('" & R("idMangt") & "'," & R2("imulch") & "," & R2("CodParamMulch") & "," & R2("QPaillisApport") & "," & R2("AltiCult") & "," & R2("DriveRuiObs") & "," & R2("fertiminON") & "," & R2("fertiorgON") & "," & R2("IrrigON") & "," & R2("NbCult") & ",'" & R2("NomSC") & "'," & R2("SerreTunnelON") & "," & Sorg & "," & SMin & ")"
            command = New SqliteCommand(Sql2, Connection)
            command.ExecuteNonQuery()
            command.Dispose()
            'codePspecies, DbutoirNouvSemis, irepiqu, NumCrop, NumCultivar, RepiquageON, SemisAutoDebut, SeuilCumPrecip, TypInstal
            Sql2 = "insert into Tech_perCrop (idTech_Com,idTechPerCrop,DensSem,isem,codePspecies, DbutoirNouvSemis, irepiqu,NumCrop,NumCultivar,RepiquageON,SemisAutoDebut,SeuilCumPrecip,TypInstal,Densrepiqu,Ilev,IdCultivar) values " &
            "('" & R("idMangt") & "','" & R("idMangt") & "'," & R("sdens") & "," & R("sowingdate") & ",'" & R3("codePspecies") & "'," & R3("DbutoirNouvSemis") &
            "," & R3("irepiqu") & "," & R3("NumCrop") & ",'" & R3("NumCultivar") & "'," & R3("RepiquageON") & "," & R3("SemisAutoDebut") & "," & R3("SeuilCumPrecip") &
            "," & R3("TypInstal") & "," & R("sdens") & "," & CInt(R("sowingdate")) + 5 & "," & R("IdcultivarCelsius") & ")"
            command = New SqliteCommand(Sql2, Connection)
            command.ExecuteNonQuery()
            ' If Form1.msgErr_expCelsius_export.Text <> "Tech_commun " & R.Item("idpoint") Then
            '     Form1.msgErr_expCelsius_export.Text = "Tech_commun " & R.Item("idpoint")
            '     Form1.msgErr_expCelsius_export.Refresh()
            ' End If
            command.Dispose()
        Next
        '---------------------------- 
        Jeu1.Clear()
        'Sql1 = "SELECT SimUnitList.idIni, SimUnitList.idsim, SimUnitList.idMangt, SimUnitList.idsoil, SimUnitList.idPoint,StartYear,StartDay,EndYear,EndDay,idOption, Coordinates.latitudeDD, CropManagement.sowingdate " _
        '& " FROM CropManagement INNER JOIN (InitialConditions INNER JOIN (Coordinates INNER JOIN SimUnitList ON Coordinates.idPoint = SimUnitList.idPoint) ON InitialConditions.idIni = SimUnitList.idIni) " _
        '& " ON CropManagement.idMangt = SimUnitList.idMangt;"

        Sql1 = "SELECT SimUnitList.idIni, SimUnitList.idsim, SimUnitList.idMangt, SimUnitList.idsoil, SimUnitList.idPoint,SimUnitList.StartYear,SimUnitList.StartDay,SimUnitList.EndYear,SimUnitList.EndDay,SimUnitList.idOption " _
        & " FROM SimUnitList;"

        '"Select  idMangt, InitialConditions.idIni, idsim,IdSoil,Coordinates.idPoint,StartYear,EndDay,Coordinates.latitudeDD FROM InitialConditions INNER JOIN (Coordinates INNER JOIN SimUnitList On Coordinates.idPoint = SimUnitList.idPoint) On InitialConditions.idIni = SimUnitList.idIni;"
        Ap_ADP1 = New SqliteDataAdapter(Sql1, MasterInput_Connection)
        Ap_ADP1.Fill(Jeu1)
        Sql2 = "Delete from SimUnitList"
        command = New SqliteCommand(Sql2, Connection)
        command.ExecuteNonQuery()
        command.Dispose()
        Console.WriteLine("SimuList")
        For i = 0 To Jeu1.Tables(0).Rows.Count - 1
            R = Jeu1.Tables(0).Rows(i)
            Sql2 = "insert into SimUnitList (idTech_Com,IdIni,idSim,IdSoil,IdWeather,StartYear,EndDay,codCC,EndYear,idCodModel,idGenParam,StartDay) values " &
        "('" & R("idMangt") & "','" & R("idIni") & "','" & R("idsim") & "','" & R("idsoil") & "','" & R("idPoint") & "'," &
         R("StartYear") & "," & R("EndDay") & ",'0'," & CInt(R("Endyear")) & "," & CInt(R("idOption")) & ",1," & R("StartDay") & ")"
            'R("StartYear") & "," & R("EndDay") & ",'0'," & IIf(R("LatitudeDD") > 0, R("StartYear"), CInt(R("StartYear")) + 1) & ",1,1," & IIf(R("LatitudeDD") > 0, 50, 240) & ")"
            'MsgBox(Sql2)
            command = New SqliteCommand(Sql2, Connection)
            command.ExecuteNonQuery()
            ' If Form1.msgErr_expCelsius_export.Text <> "SimUnitList " & R.Item("idpoint") Then
            '     Form1.msgErr_expCelsius_export.Text = "SimUnitList " & R.Item("idpoint")
            '     Form1.msgErr_expCelsius_export.Refresh()
            ' End If
            command.Dispose()
        Next
        '----------------------------
        Jeu1.Clear()
        Jeu2.Clear()
        Sql1 = "Select IdSoil, bd, OrganicNStock, RunoffType, Slope, SoilRDepth, SoilTextureType, SoilTotalDepth,cf,Wfc,Wwp FROM Soil;"
        Ap_ADP1 = New SqliteDataAdapter(Sql1, MasterInput_Connection)
        Ap_ADP1.Fill(Jeu1)
        Sql2 = "Delete from Soil"
        command = New SqliteCommand(Sql2, Connection)
        command.ExecuteNonQuery()
        command.Dispose()
        Sql2 = "Delete from Soil_Layers"
        command = New SqliteCommand(Sql2, Connection)
        command.ExecuteNonQuery()
        command.Dispose()
        Sql3 = "Select * from SoilDV"
        Ap_ADP2 = New SqliteDataAdapter(Sql3, Connection)
        Ap_ADP2.Fill(Jeu2)
        R2 = Jeu2.Tables(0).Rows(0)
        For i = 0 To Jeu1.Tables(0).Rows.Count - 1
            R = Jeu1.Tables(0).Rows(i)
            Sql2 = "insert into Soil (IdSoil,StockN,TypeRui,Zmes,ZObstacleRac,FMin,NbCouches,SeuilEvap,txMinN,Zsurf) values " &
            "('" & R("IdSoil") & "'," & R("OrganicNStock") & ",'" & R("RunoffType") & "'," & R("SoilTotalDepth") & "," & R("SoilRDepth") & "," & R2("FMin") & ",1," & R2("SeuilEvap") & "," & R2("txminN") & "," & R2("Zsurf") & ")"
            command = New SqliteCommand(Sql2, Connection)
            command.ExecuteNonQuery()
            command.Dispose()
            Sql2 = "insert into Soil_Layers (IdSoil,numcouche,da,epc,hcc,hmin) values " &
            "('" & R("IdSoil") & "',1," & R("bd") & "," & R("SoilTotalDepth") & "," & CDbl((1 - R("cf") / 100) * R("wfc")) / CDbl(R("bd")) & "," & CDbl((1 - R("cf") / 100) * R("wwp")) / CDbl(R("bd")) & ")"
            command = New SqliteCommand(Sql2, Connection)
            command.ExecuteNonQuery()
            command.Dispose()
            ' If Form1.msgErr_expCelsius_export.Text <> "Soil " & R.Item("idSoil") Then
            '     Form1.msgErr_expCelsius_export.Text = "Soil " & R.Item("idSoil")
            '     Form1.msgErr_expCelsius_export.Refresh()
            ' End If
        Next
        '----------------------------
        Connection.Close()
        MasterInput_Connection.Close()
        'MsgBox("Fini")
    End Sub

    Public Overrides Sub Import(DirectoryPath As String, model As String)
        Throw New NotImplementedException()
    End Sub

    'Public Overrides Sub Export(DirectoryPath As String)
    '    Throw New NotImplementedException()
    'End Sub
End Class
