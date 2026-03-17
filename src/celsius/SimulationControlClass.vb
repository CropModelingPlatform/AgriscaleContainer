Option Explicit On
Option Compare Text
Imports System.Data
Imports System.Data.Sqlite
Imports System.Text
Public Class SimulationControlClass
    Dim SimUnit As SimulationUnitClass
    Dim MemoSU As SimulationUnitClass
    Dim dataClim As DataClimClass
    Dim dataIrrig As IrrigClass
    Dim dataFertiMin As FertiMinClass
    Dim dataFertiOrg As FertiOrgaClass
    Dim GestionTechnique As GestionTechniqueClass
    Dim Plante As PLanteClass
    Dim Culture As CultureClass
    Dim GenParam As GenParamClass
    Dim Sol As SolClass
    Dim Mulch As MulchClass
    Dim OptionsModel As OptionsModelClass
    Dim ApportOrga As ApportsOrgaClass
    Public EtatInitial As EtatInitialClass
    Public Etatfinal As EtatFinalClass
    'modif: 08/04/2011
    'introduction OptionsModelClass et call OptionsModel.LisOptionsModel(DataBase_Cnn, SimUnit.nidCodModel)
    'modif OptionsModel pour option écriture résus journaliers
    ' modif 10/05/11
    'introduction appels vers ruissellement et bilans mulch
    ' 27/07/11 ajout variables sortie (composantes rendement)
    ' modif  13/11/2011 introduction correction altitude
    ' modif  08/03/2012 introduction irrigations classe IrrigClass dans subreadparameters et ajout irrigations aux pluies dans simulation
    ' modif 02/04/2012 deux nouvelles sorties dans outputD (etp et TpotMC), et lecture Kmax (anciennement constante) par plante puis transmission à assoc
    ' modif  04/05/2012 activation module de levee
    ' dernière modif 7/12/12 introduction stress nutriments
    ' dernière modif 28/04/13 sub simulation appel ruissellement modifié pour prise en compte fonctio ns albergel et al., et introduction ressemis auto
    'modifs juil 13 corrections pour simulation deux cultures successives
    'modif CP 04/10/13 modifs suite à modif bilan azote pour fonctionnement journalier complet
    '=lecture des tables FertiMin et FertiOrga journalières + suppression Calcul StockNini
    '+ introduction irrigation dans le calcul de l'eau infiltrée servant au déclenchement du semis automatique
    'OK révisé FA le 10//11/13 en entier, commentaires introduits pour choses à faire.
    'VERSION 3 du 8-10 nov 2017'
    'Modif FA du 30/08/18 pour scenarios CC:déplacement de la lecture des options avant lecture du climat..pas fini, pas opérationnel, pas gênant masi champs supplementaires lus
    ' modif FA du 7/09/18: cumul evap sur culture avec intro de la variable "pousse" dans Simulation


    Sub ReadParameters(ByRef rstSimulation As DataSet, DataBase_Cnn As SqliteConnection, comptesim As Integer, codesuite As Integer)
        ' Steplait A faire : introduire une mémoire de SimUnit pour comparer d'une ligne de SimUnitList à
        ' l'autre et économiser les lectures de tables qui ne seraient pas nécessaires
        ' selon modele fait pour GenParam (important surtout pour DataClim)
        Dim icult As Integer

        If comptesim > 0 Then MemoSU = SimUnit Else MemoSU = New SimulationUnitClass
        SimUnit = New SimulationUnitClass
        Call SimUnit.ReadSimUnitParameters(rstSimulation, comptesim)
        If comptesim = 0 Or (SimUnit.sidGenParam <> MemoSU.sidGenParam) Then
            GenParam = New GenParamClass
            Call GenParam.LisGenParam(DataBase_Cnn, SimUnit)
        End If

        dataClim = New DataClimClass
        Call dataClim.LisClimD(DataBase_Cnn, SimUnit)
        If comptesim = 0 Or (SimUnit.nidCodModel <> MemoSU.nidCodModel) Then
            OptionsModel = New OptionsModelClass
            Call OptionsModel.LisOptionsModel(DataBase_Cnn, SimUnit.nidCodModel)
        End If
        EtatInitial = New EtatInitialClass
        If codesuite = 0 Then
            Call EtatInitial.LisInitialData(DataBase_Cnn, SimUnit.nidIni)
        Else
            Call EtatInitial.recursive(Etatfinal)
        End If

        GestionTechnique = New GestionTechniqueClass
        Call GestionTechnique.LisTech(DataBase_Cnn, SimUnit)
        dataIrrig = New IrrigClass
        If GestionTechnique.bIrrigON Then Call dataIrrig.LisIrrD(DataBase_Cnn, SimUnit, GestionTechnique)
        dataFertiMin = New FertiMinClass
        If GestionTechnique.bfertiminON Then Call dataFertiMin.LisFertiMinD(DataBase_Cnn, SimUnit, GestionTechnique)
        dataFertiOrg = New FertiOrgaClass
        If GestionTechnique.bfertiorgON Then Call dataFertiOrg.LisFertiOrgD(DataBase_Cnn, SimUnit, GestionTechnique)
        Call dataClim.update_DAP(SimUnit.nStartDay, GestionTechnique.niplt(1))
        ' correction des températures en fonction de l'altitude si demandé dans OptionsModel
        If OptionsModel.bCorAlti Then Call dataClim.CorrigTAlt(GestionTechnique.nAltiCult, SimUnit.nNbJourSimul)
        Plante = New PLanteClass
        Call Plante.LisPlante(DataBase_Cnn, GestionTechnique)
        Sol = New SolClass
        Call Sol.LisSol(DataBase_Cnn, SimUnit, Plante.nZgraine(1), GestionTechnique.bDriveRuiObs)
        Culture = New CultureClass
        For icult = 1 To GestionTechnique.nNbCult
            Call Plante.Iniplante(icult, GestionTechnique.dDensSem(icult), GestionTechnique.niplt(icult), GestionTechnique.dDensRepiqu(icult), GestionTechnique.nirepiqu(icult), SimUnit.nStartDay, SimUnit.nEndDOY, Min(Sol.iZtotsol, Sol.nZObstacleRac), dataClim.nCO2c)
            Call Culture.InitMixedCanopy(Plante.dCoefExtin(icult), Plante.nZracmax(icult), Plante.dKmax(icult))
        Next icult
        Call Sol.initsol(Culture.dZracmaxMC, EtatInitial.dStockinit, EtatInitial.bIniSolhautON)
        ' TODO: 2 lsuiv provisoire à adapter pour pl assoc
        Call Culture.lisCultureMC()
        Mulch = New MulchClass
        Call Mulch.LisMulch(DataBase_Cnn, GestionTechnique.nCodParamMulch, EtatInitial.dQpaillisinit)
        If GestionTechnique.bDriveRuiObs Then Call Mulch.LisRuiObs(DataBase_Cnn, SimUnit)
        'introduire ici les appels vers les autres initialisation d'objets (weeds, aleas abiotiques..)
        'Fin lecture des tables /initialisation des objets
        'MsgBox (SimUnit.sIdSim)

    End Sub
    Sub Simulation()
        Dim joursim As Integer
        Dim icult As Integer
        Dim pousse As Boolean



        'Const Codevelop As String = "dev.oryza"
        If OptionsModel.sCodeDevelop = "Oryza" Then Call Plante.AdapteCT(GestionTechnique.nNbCult)


        'debut boucle sur pas de temps
        For joursim = 1 To SimUnit.nNbJourSimul

            ' à améliorer pour ne calculer que pour les plantes pas mortes
            For icult = 1 To GestionTechnique.nNbCult
                ' ancienne version, calcul stess N saisonnier
                If OptionsModel.bActiveStressN And OptionsModel.nTypeNPKstress = 1 Then Call Plante.stressAzoteOld(icult, joursim, Sol.dStockN, GestionTechnique.dtApportMON, GestionTechnique.dtApportMinN)
                If OptionsModel.bCyberST Then Call GestionTechnique.CyberPlouck(icult, Plante.bDie(icult), Plante.nDeathDay(icult), joursim, SimUnit.nStartDay)
                If GestionTechnique.bRessemis(icult) And joursim > 2 Then
                    Call GestionTechnique.SemisAuto(icult, joursim, SimUnit.nStartDay, ((dataClim.dPlu(joursim - 1) + dataIrrig.dIrr(joursim - 1)) - Mulch.dRuis(joursim - 1)) + ((dataClim.dPlu(joursim - 2) + dataIrrig.dIrr(joursim - 2)) - Mulch.dRuis(joursim - 2)))
                    ' attention si la date de début de la période de semis est inférieure à Startday+2, on ne semetra pas avant le 2ejour même si le 1er aurait satisfait la condition
                End If

                If Not GestionTechnique.bRessemis(icult) And joursim = GestionTechnique.niplt(icult) - SimUnit.nStartDay + 1 Then
                    Call Plante.Iniplante(icult, GestionTechnique.dDensSem(icult), GestionTechnique.niplt(icult), GestionTechnique.dDensRepiqu(icult), GestionTechnique.nirepiqu(icult), SimUnit.nStartDay, SimUnit.nEndDOY, Min(Sol.iZtotsol, Sol.nZObstacleRac), dataClim.nCO2c)
                    ' voir si cette intialisation se substitue complètement à celle qui suit la lecture du fichier plante
                    Plante.bDie(icult) = False
                    Plante.ncropsta(icult, joursim - 1) = 1
                    Call GestionTechnique.CompteSemis(icult)

                End If
            Next icult


            If Not (Plante.bDie(1) And Plante.bDie(2)) Then
                ' s'il y a au moins une plante en croissance

                ' piege bugs
                'If joursim = 211 Then
                '              joursim = joursim
                '              End If

                For icult = 1 To GestionTechnique.nNbCult
                    If Not Plante.bDie(icult) Then
                        pousse = True
                        Plante.ncropsta(icult, joursim) = Plante.ncropsta(icult, joursim - 1)
                        '                    If Plante.ncropsta(icult, joursim) = 3 And GestionTechnique.bSerreTunnelON Then Call dataClim.CorrigTSerres(joursim) 'appel du modele de temperature sous serre tunnel
                        If Plante.ncropsta(icult, joursim) >= 1 And Plante.ncropsta(icult, joursim) < 3 Then
                            Call Plante.GerminLevee(dataClim.nDOY(joursim), icult, joursim, GestionTechnique.nilev(icult), OptionsModel.bSimLevee, Sol.bContrainteHlevee, dataClim.dTmoy(joursim))
                        End If
                        If Plante.ncropsta(icult, joursim) >= 3 Then
                            If Plante.ncropsta(icult, joursim) < 4 Then
                                If dataClim.nDOY(joursim) >= GestionTechnique.nirepiqu(icult) Or Not GestionTechnique.bRepiquageON(icult) Then Plante.ncropsta(icult, joursim) = 4
                            End If
                            ' vérifier si positionnement adequat appel plante.stressN ici.
                            If OptionsModel.bActiveStressN And OptionsModel.nTypeNPKstress = 2 Then Call Plante.stressAzote(icult, joursim, Sol.dNavail(joursim - 1))
                            'repasser signal fletrissement en scalaire
                            If OptionsModel.sCodeDevelop = "Oryza" Then
                                Call Plante.pheno_sigmaT(dataClim, joursim, Sol.nSignalFletrissement(joursim - 1), icult)
                            Else
                                ' codeDevelop est suppose etre "Direct"
                                Call Plante.phenoCTphot(dataClim, joursim, Sol.nSignalFletrissement(joursim - 1), icult)
                            End If
                            '                        Call Plante.Calcule_LAI(GenParam.dVlaimax, joursim, icult, Sol.dContrainteW, OptionsModel.bActiveStressH, OptionsModel.bActiveStressN, Culture.dLaiMC(joursim - 1), GestionTechnique.nNbCult)
                            Call Plante.Calcule_LAI_SemiAride(GenParam.dVlaimax, joursim, icult, Sol.dContrainteW, OptionsModel.bActiveStressH, OptionsModel.bActiveStressN, Culture.dLaiMC(joursim - 1), GestionTechnique.nNbCult)
                            Call Plante.biomasse(dataClim, GenParam.dParSurRg, icult, joursim, Sol.dContrainteW, OptionsModel.bActiveStressH, OptionsModel.bActiveStressN)
                            Call Plante.Rendement(icult, joursim, dataClim.nDOY(joursim))
                            Call Plante.Croirac(icult, joursim, Sol.dZoneHumSousRacines, Culture.dZRacineMC(joursim - 1))
                            Call Culture.LaiMixedCanopy(joursim, Plante.dLAI(icult, joursim))
                            Call Culture.CroiRacMixedCrop(joursim, Plante.dZrac(icult, joursim))
                            Call Culture.NuptakeMixedCrop(joursim, Plante.dNuptake(icult, joursim))
                        End If
                    End If
                Next icult


            End If 'fin  condition sur présence plantes
            Call Mulch.BiomasseMulch(joursim, dataClim.nDOY(joursim) = GestionTechnique.nimulch, GestionTechnique.dQpaillisApport)
            Call Culture.Evaporation_Pot_SolMulch(joursim, dataClim.dEtp(joursim))
            'on appelle bilanmulch même si pas de mulch
            Call Mulch.Ruissellement(joursim, dataClim.dPlu(joursim) + dataIrrig.dIrr(joursim), Sol.ClTypeSurf, Culture.dLaiMC(joursim))
            Call Mulch.BilanMulch(joursim, Culture.dEoSM, Mulch.dEau_vers_Mulch)
            Call Sol.evaporation(joursim, Culture.dEoSM, Mulch.dEomulch, Sol.dContrainteWSurf, pousse)
            'TODO faudrait faire d'abord actualisation des stocks avant transpi puis transpi puis réactualiser stocks...
            Call Culture.CalcTranspiMC(joursim, dataClim.dEtp(joursim), Sol.dEsol(joursim), Mulch.dEmulch(joursim), Sol.dContrainteW)

            Call Sol.EauSol(joursim, Mulch.dEauVersSol, Culture.dTranspiMC(joursim), Culture.dZRacineMC(joursim), Culture.dZRacineMC(joursim) - Culture.dZRacineMC(joursim - 1), Culture.dZracmaxMC)
            ' pourquoi conditionner le bilan à l'activation du stress ?
            If OptionsModel.bActiveStressN And OptionsModel.nTypeNPKstress = 2 Then
                Call Sol.TempSoil(dataClim.dTmoy(joursim))
                Call Sol.MinNorgSS(joursim)
                Call Sol.DenitNavail(joursim)
                'attention ligne suivante reste à parametrer le CsurNhum, mis à 12 en attendant
                Call Sol.MinNorgapporteStics(joursim, dataFertiOrg.dQorga(joursim), dataFertiOrg.dCsurNRes, 12, ApportOrga)
                Call Sol.StockNavail(joursim, dataFertiMin.dNmin(joursim), Culture.dNuptMC(joursim), dataClim.dPlu(joursim) * dataClim.dConcNplu)
                Call Sol.ConcNEauSol(joursim, Culture.dZracmaxMC)

            End If

        Next joursim
    End Sub
    Sub SortieSynthesis(StrCh As String, Db_Cnn As SqliteConnection)
        Dim Istade, i As Integer
        Dim ErrFa As Boolean
        Dim msg As String
        Dim sbVal As New StringBuilder(1000) ' Pré-allouer pour ~1000 caractères
        Dim Sortie As New SqliteCommand("", Db_Cnn)
        
        Try
            ' Utilisation de StringBuilder au lieu de String +=
            sbVal.Append("'").Append(SimUnit.sIdSim).Append("',")
            
            For Istade = 1 To 6
                sbVal.Append(Plante.nJulPheno(1, Istade)).Append(",")
            Next Istade
            
            sbVal.Append(Plante.nDeathDay(1)).Append(",")
            sbVal.Append(Plante.dBiom(1, Plante.nJourMat(1))).Append(",")
            sbVal.Append(Plante.dGrain(1, Plante.nJourMat(1))).Append(",")
            sbVal.Append(Plante.dLAI(1, Plante.nJourLaiMax(1))).Append(",")
            sbVal.Append(Sol.dSigmaSimEsol).Append(",")
            sbVal.Append(Sol.dSigmaSimDr).Append(",")
            sbVal.Append(Sol.dSigmaSimDrprofmax).Append(",")
            sbVal.Append(Sol.dStockSol(0)).Append(",")
            sbVal.Append(Sol.dStockSol(SimUnit.nNbJourSimul)).Append(",")
            sbVal.Append(Mulch.dSigmaSimEmulch).Append(",")
            sbVal.Append(Mulch.dSigmaSimRuis).Append(",")
            sbVal.Append(Culture.dSigmaTranspiMC).Append(",")
            sbVal.Append(Mulch.dSigmaSimPluM).Append(",")
            sbVal.Append(Plante.nNgrains(1)).Append(",")
            sbVal.Append(Plante.dP1grain(1, Plante.nJourMat(1))).Append(",")
            sbVal.Append(Plante.dVitmoy(1)).Append(",")
            sbVal.Append(GestionTechnique.niplt(1)).Append(",")
            sbVal.Append(GestionTechnique.nNbsemis(1)).Append(",")
            
            For Istade = 1 To 6
                sbVal.Append(Plante.nJulPheno(2, Istade)).Append(",")
            Next Istade
            
            sbVal.Append(Plante.nDeathDay(2)).Append(",")
            sbVal.Append(Plante.dBiom(2, Plante.nJourMat(2))).Append(",")
            sbVal.Append(Plante.dGrain(2, Plante.nJourMat(2))).Append(",")
            sbVal.Append(Plante.dLAI(2, Plante.nJourSen(2))).Append(",")
            sbVal.Append(Plante.nNgrains(2)).Append(",")
            sbVal.Append(Plante.dP1grain(2, Plante.nJourMat(2))).Append(",")
            sbVal.Append(Plante.dVitmoy(2)).Append(",")
            sbVal.Append(GestionTechnique.niplt(2)).Append(",")
            sbVal.Append(GestionTechnique.nNbsemis(2)).Append(",")
            sbVal.Append("0,") 'Sol.nnbjContHlev(1)
            sbVal.Append(Sol.dStockN).Append(",")
            sbVal.Append(Mulch.bRuisEtrange).Append(",")
            sbVal.Append(Sol.dSigmaCultEsol)

            Sortie.CommandText = "Insert into OutputSynt (" & StrCh & ") Values (" & sbVal.ToString() & ");"
            Sortie.ExecuteNonQuery()
        'TabSynt.AddNew()
        'TabSynt.Fields(0).Value = SimUnit.sIdSim
        'For Istade = 1 To 6
        '    TabSynt.Fields(Istade).Value = Plante.nJulPheno(1, Istade)
        'Next Istade

        'TabSynt.Fields(7).Value = Plante.nDeathDay(1)
        'TabSynt.Fields(8).Value = Plante.dBiom(1, Plante.nJourMat(1))
        'TabSynt.Fields(9).Value = Plante.dGrain(1, Plante.nJourMat(1))
        'TabSynt.Fields(10).Value = Plante.dLAI(1, Plante.nJourSen(1))
        'TabSynt.Fields(11).Value = Sol.dSigmaSimEsol
        'TabSynt.Fields(12).Value = Sol.dSigmaSimDr
        'TabSynt.Fields(13).Value = Sol.dSigmaSimDrprofmax
        'TabSynt.Fields(14).Value = Sol.dStockSol(0)
        'TabSynt.Fields(15).Value = Sol.dStockSol(SimUnit.nNbJourSimul)
        'TabSynt.Fields(16).Value = Mulch.dSigmaSimEmulch
        'TabSynt.Fields(17).Value = Mulch.dSigmaSimRuis
        'TabSynt.Fields(18).Value = Culture.dSigmaTranspiMC
        'TabSynt.Fields(19).Value = Mulch.dSigmaSimPluM
        'TabSynt.Fields(20).Value = Plante.nNgrains(1)
        'TabSynt.Fields(21).Value = Plante.dP1grain(1, Plante.nJourMat(1))
        'TabSynt.Fields(22).Value = Plante.dVitmoy(1)
        'TabSynt.Fields(23).Value = GestionTechnique.niplt(1)
        'TabSynt.Fields(24).Value = GestionTechnique.nNbsemis(1)
        'For Istade = 1 To 6
        '    TabSynt.Fields(24 + Istade).Value = Plante.nJulPheno(2, Istade)
        'Next Istade
        'TabSynt.Fields(31).Value = Plante.nDeathDay(2)
        'TabSynt.Fields(32).Value = Plante.dBiom(2, Plante.nJourMat(2))
        'TabSynt.Fields(33).Value = Plante.dGrain(2, Plante.nJourMat(2))
        'TabSynt.Fields(34).Value = Plante.dLAI(2, Plante.nJourSen(2))
        'TabSynt.Fields(35).Value = Plante.nNgrains(2)
        'TabSynt.Fields(36).Value = Plante.dP1grain(2, Plante.nJourMat(2))
        'TabSynt.Fields(37).Value = Plante.dVitmoy(2)
        'TabSynt.Fields(38).Value = GestionTechnique.niplt(2)
        'TabSynt.Fields(39).Value = GestionTechnique.nNbsemis(2)
        ''TabSynt.Fields(40).Value = Sol.nnbjContHlev(1)
        'TabSynt.Fields(41).Value = Sol.dStockN
        ''TabSynt.Fields(42).Value = Sol.dVolEausol
        'TabSynt.Update()



        Catch ex As Exception
            Console.WriteLine("Erreur SortieSynthesis: " & ex.Message)
            Throw
        End Try

    End Sub
    Sub EcritDresu(Db_Cnn As SqliteConnection, comptesim As Integer)
        Dim jour As Integer
        Dim icult As Integer
        Dim StrCh As String
        Dim sbVal As StringBuilder
        Dim TabDayAdp As SqliteDataAdapter
        Dim TabDayCmd As SqliteCommand
        Dim TabDayPlante(0 To 2) As DataTable
        Dim transaction As SqliteTransaction = Nothing
        
        Try
            For icult = 1 To GestionTechnique.nNbCult
                TabDayPlante(icult) = New DataTable
                If comptesim = 0 Then
                    TabDayCmd = New SqliteCommand("", Db_Cnn)
                    TabDayCmd.CommandText = "delete from OutputD_" & Trim(Str(icult))
                    TabDayCmd.ExecuteNonQuery()
                End If
                
                TabDayAdp = New SqliteDataAdapter("SELECT * FROM OutputD_" & Trim(Str(icult)), Db_Cnn)
                TabDayAdp.Fill(TabDayPlante(icult))
                
                If OptionsModel.bEcritDresus Then
                    ' Construction de la liste des colonnes avec StringBuilder
                    Dim sbCh As New StringBuilder(500)
                    For i = 0 To TabDayPlante(icult).Columns.Count - 1
                        sbCh.Append(TabDayPlante(icult).Columns(i).ColumnName)
                        If i < TabDayPlante(icult).Columns.Count - 1 Then sbCh.Append(",")
                    Next
                    StrCh = sbCh.ToString()
                    
                    ' Démarrer UNE transaction pour TOUS les jours (gain majeur de performance)
                    transaction = Db_Cnn.BeginTransaction()
                    TabDayCmd = New SqliteCommand("", Db_Cnn, transaction)
                    
                    ' Construire la requête préparée UNE SEULE FOIS avec des placeholders
                    Dim sbPlaceholders As New StringBuilder(500)
                    For i = 0 To TabDayPlante(icult).Columns.Count - 1
                        sbPlaceholders.Append("@p").Append(i)
                        If i < TabDayPlante(icult).Columns.Count - 1 Then sbPlaceholders.Append(",")
                    Next
                    
                    TabDayCmd.CommandText = "Insert into OutputD_" & Trim(Str(icult)) & " (" & StrCh & ") Values (" & sbPlaceholders.ToString() & ")"
                    
                    ' Ajouter tous les paramètres (ils seront réutilisés à chaque itération)
                    For i = 0 To TabDayPlante(icult).Columns.Count - 1
                        TabDayCmd.Parameters.Add("@p" & i, DbType.String)
                    Next
                    
                    For jour = 1 To SimUnit.nNbJourSimul
                    'Console.WriteLine("jour : " & Str(jour) & ", idsim : " & SimUnit.sIdSim)
                    'TabDayPlante(icult - 1).AddNew()
                    'TabDayPlante(icult - 1).Fields(0).Value = SimUnit.sIdSim & Str(dataClim.nDAP(jour))
                    'TabDayPlante(icult - 1).Fields(1).Value = SimUnit.sIdWeather
                    'TabDayPlante(icult - 1).Fields(2).Value = dataClim.nCurrentYear(jour)
                    'TabDayPlante(icult - 1).Fields(3).Value = SimUnit.sIdTec
                    'TabDayPlante(icult - 1).Fields(4).Value = Plante.sCultivar(icult)
                    'TabDayPlante(icult - 1).Fields(5).Value = dataClim.nDAP(jour)
                    'TabDayPlante(icult - 1).Fields(6).Value = dataClim.nDOY(jour)
                    '' TabDayPlante(icult - 1).Fields(7).Value = dataClim.dDAYL(jour)
                    'TabDayPlante(icult - 1).Fields(7).Value = dataClim.dTmax(jour)
                    'TabDayPlante(icult - 1).Fields(8).Value = dataClim.dTmin(jour)
                    'TabDayPlante(icult - 1).Fields(9).Value = Plante.dDVSt(icult, jour)
                    'TabDayPlante(icult - 1).Fields(10).Value = Plante.nCurrstge(icult, jour)
                    'TabDayPlante(icult - 1).Fields(11).Value = Plante.ncropsta(icult, jour)
                    'TabDayPlante(icult - 1).Fields(12).Value = Plante.dSommeT(icult, jour)
                    'TabDayPlante(icult - 1).Fields(13).Value = Plante.dLAI(icult, jour)
                    'TabDayPlante(icult - 1).Fields(14).Value = Plante.dBiom(icult, jour)
                    'TabDayPlante(icult - 1).Fields(15).Value = Plante.dGrain(icult, jour)
                    'TabDayPlante(icult - 1).Fields(16).Value = Plante.dZrac(icult, jour)
                    'TabDayPlante(icult - 1).Fields(17).Value = Sol.dStsurf(jour)
                    'TabDayPlante(icult - 1).Fields(18).Value = Sol.dEsol(jour)
                    'TabDayPlante(icult - 1).Fields(19).Value = Sol.dStnonrac(jour)
                    'TabDayPlante(icult - 1).Fields(20).Value = Sol.dStrac(jour)
                    'TabDayPlante(icult - 1).Fields(21).Value = Culture.dTranspiMC(jour)
                    'TabDayPlante(icult - 1).Fields(22).Value = Sol.dDrprofmax(jour)


                    'TabDayPlante(icult - 1).Fields(23).Value = Sol.dStprofond(jour)
                    'TabDayPlante(icult - 1).Fields(24).Value = Sol.dStockSol(jour)
                    'TabDayPlante(icult - 1).Fields(25).Value = dataClim.dPlu(jour)
                    'TabDayPlante(icult - 1).Fields(26).Value = Sol.dStockMes(jour)

                    'TabDayPlante(icult - 1).Fields(27).Value = Mulch.dEmulch(jour)
                    'TabDayPlante(icult - 1).Fields(28).Value = Mulch.dRuis(jour)
                    'TabDayPlante(icult - 1).Fields(29).Value = Mulch.dQpaillis(jour)
                    'TabDayPlante(icult - 1).Fields(30).Value = Plante.dP1grain(icult, jour)
                    'TabDayPlante(icult - 1).Fields(31).Value = SimUnit.sIdSim
                    'TabDayPlante(icult - 1).Fields(32).Value = dataClim.dEtp(jour)
                    'TabDayPlante(icult - 1).Fields(33).Value = Culture.dTPotMC(jour)
                    'TabDayPlante(icult - 1).Fields(34).Value = Sol.bDrainageON(jour)
                    'TabDayPlante(icult - 1).Fields(35).Value = Sol.dConcNsol(jour)
                    'TabDayPlante(icult - 1).Fields(36).Value = Sol.dDr(jour)
                    'TabDayPlante(icult - 1).Fields(37).Value = Sol.dperteNDrain(jour)
                    'TabDayPlante(icult - 1).Fields(38).Value = Plante.dNuptake(icult, jour)
                    'TabDayPlante(icult - 1).Fields(39).Value = Plante.dSigmaNuptake(icult, jour)
                    'TabDayPlante(icult - 1).Fields(40).Value = Sol.dNavail(jour) * 0.01
                    'TabDayPlante(icult - 1).Fields(41).Value = dataFertiMin.dNmin(jour)
                    'TabDayPlante(icult - 1).Fields(42).Value = dataFertiOrg.dNorg(jour)
                    'TabDayPlante(icult - 1).Fields(43).Value = Sol.dNavail(jour)
                    'TabDayPlante(icult - 1).Fields(44).Value = Plante.dNUPTtarget(icult, jour)
                    'TabDayPlante(icult - 1).Fields(45).Value = Plante.dNRF(icult, jour)
                    'TabDayPlante(icult - 1).Fields(46).Value = Plante.dWSfactH(jour)
                    'TabDayPlante(icult - 1).Fields(47).Value = Plante.dWSfact(jour)
                    'TabDayPlante(icult - 1).Fields(48).Value = Plante.dTurfacH(jour)
                    'TabDayPlante(icult - 1).Fields(49).Value = Plante.dTurfac(jour)
                    'TabDayPlante(icult - 1).Fields(50).Value = Sol.dStger(jour)
                    ''           TabDayPlante(icult - 1).Fields(51).Value = Sol.bContrainteHlevee(jour)
                    'TabDayPlante(icult - 1).Fields(52).Value = Plante.bCompetition(jour)
                        ' Remplir les paramètres avec les valeurs du jour
                        ' Plus besoin de StringBuilder ni de reconstruction de requête SQL !
                        Dim paramIdx As Integer = 0
                        TabDayCmd.Parameters(paramIdx).Value = SimUnit.sIdSim & dataClim.nDAP(jour) : paramIdx += 1
                        TabDayCmd.Parameters(paramIdx).Value = SimUnit.sIdSim : paramIdx += 1
                        TabDayCmd.Parameters(paramIdx).Value = SimUnit.sIdWeather : paramIdx += 1
                        TabDayCmd.Parameters(paramIdx).Value = dataClim.nCurrentYear(jour) : paramIdx += 1
                        TabDayCmd.Parameters(paramIdx).Value = SimUnit.sIdTec : paramIdx += 1
                        TabDayCmd.Parameters(paramIdx).Value = Plante.sCultivar(icult) : paramIdx += 1
                        TabDayCmd.Parameters(paramIdx).Value = dataClim.nDAP(jour) : paramIdx += 1
                        TabDayCmd.Parameters(paramIdx).Value = dataClim.nDOY(jour) : paramIdx += 1
                        TabDayCmd.Parameters(paramIdx).Value = dataClim.dTmax(jour) : paramIdx += 1
                        TabDayCmd.Parameters(paramIdx).Value = dataClim.dTmin(jour) : paramIdx += 1
                        TabDayCmd.Parameters(paramIdx).Value = Plante.dDVSt(icult, jour) : paramIdx += 1
                        TabDayCmd.Parameters(paramIdx).Value = Plante.nCurrstge(icult, jour) : paramIdx += 1
                        TabDayCmd.Parameters(paramIdx).Value = Plante.ncropsta(icult, jour) : paramIdx += 1
                        TabDayCmd.Parameters(paramIdx).Value = Plante.dSommeT(icult, jour) : paramIdx += 1
                        TabDayCmd.Parameters(paramIdx).Value = Plante.dLAI(icult, jour) : paramIdx += 1
                        TabDayCmd.Parameters(paramIdx).Value = Plante.dBiom(icult, jour) : paramIdx += 1
                        TabDayCmd.Parameters(paramIdx).Value = Plante.dGrain(icult, jour) : paramIdx += 1
                        TabDayCmd.Parameters(paramIdx).Value = Plante.dZrac(icult, jour) : paramIdx += 1
                        TabDayCmd.Parameters(paramIdx).Value = Sol.dStsurf(jour) : paramIdx += 1
                        TabDayCmd.Parameters(paramIdx).Value = Sol.dEsol(jour) : paramIdx += 1
                        TabDayCmd.Parameters(paramIdx).Value = Sol.dStnonrac(jour) : paramIdx += 1
                        TabDayCmd.Parameters(paramIdx).Value = Sol.dStrac(jour) : paramIdx += 1
                        TabDayCmd.Parameters(paramIdx).Value = Culture.dTranspiMC(jour) : paramIdx += 1
                        TabDayCmd.Parameters(paramIdx).Value = Sol.dDrprofmax(jour) : paramIdx += 1
                        TabDayCmd.Parameters(paramIdx).Value = Sol.dStprofond(jour) : paramIdx += 1
                        TabDayCmd.Parameters(paramIdx).Value = Sol.dStockSol(jour) : paramIdx += 1
                        TabDayCmd.Parameters(paramIdx).Value = dataClim.dPlu(jour) : paramIdx += 1
                        TabDayCmd.Parameters(paramIdx).Value = Sol.dStockMes(jour) : paramIdx += 1
                        TabDayCmd.Parameters(paramIdx).Value = Mulch.dEmulch(jour) : paramIdx += 1
                        TabDayCmd.Parameters(paramIdx).Value = Mulch.dRuis(jour) : paramIdx += 1
                        TabDayCmd.Parameters(paramIdx).Value = Mulch.dQpaillis(jour) : paramIdx += 1
                        TabDayCmd.Parameters(paramIdx).Value = Plante.dP1grain(icult, jour) : paramIdx += 1
                        TabDayCmd.Parameters(paramIdx).Value = dataClim.dEtp(jour) : paramIdx += 1
                        TabDayCmd.Parameters(paramIdx).Value = Culture.dTPotMC(jour) : paramIdx += 1
                        TabDayCmd.Parameters(paramIdx).Value = Sol.bDrainageON(jour) : paramIdx += 1
                        TabDayCmd.Parameters(paramIdx).Value = Sol.dConcNsol(jour) : paramIdx += 1
                        TabDayCmd.Parameters(paramIdx).Value = Sol.dDr(jour) : paramIdx += 1
                        TabDayCmd.Parameters(paramIdx).Value = Sol.dperteNDrain(jour) : paramIdx += 1
                        TabDayCmd.Parameters(paramIdx).Value = Plante.dNuptake(icult, jour) : paramIdx += 1
                        TabDayCmd.Parameters(paramIdx).Value = Plante.dSigmaNuptake(icult, jour) : paramIdx += 1
                        TabDayCmd.Parameters(paramIdx).Value = Sol.dNavail(jour) * 0.01 : paramIdx += 1
                        TabDayCmd.Parameters(paramIdx).Value = dataFertiMin.dNmin(jour) : paramIdx += 1
                        TabDayCmd.Parameters(paramIdx).Value = dataFertiOrg.dNorg(jour) : paramIdx += 1
                        TabDayCmd.Parameters(paramIdx).Value = Sol.dNavail(jour) : paramIdx += 1
                        TabDayCmd.Parameters(paramIdx).Value = Plante.dNUPTtarget(icult, jour) : paramIdx += 1
                        TabDayCmd.Parameters(paramIdx).Value = Plante.dNRF(icult, jour) : paramIdx += 1
                        TabDayCmd.Parameters(paramIdx).Value = Plante.dWSfactH(jour) : paramIdx += 1
                        TabDayCmd.Parameters(paramIdx).Value = Plante.dWSfact(jour) : paramIdx += 1
                        TabDayCmd.Parameters(paramIdx).Value = Plante.dTurfacH(jour) : paramIdx += 1
                        TabDayCmd.Parameters(paramIdx).Value = Plante.dTurfac(jour) : paramIdx += 1
                        TabDayCmd.Parameters(paramIdx).Value = Sol.dStger(jour) : paramIdx += 1
                        TabDayCmd.Parameters(paramIdx).Value = 0 : paramIdx += 1 ' ContrainteHlevee
                        TabDayCmd.Parameters(paramIdx).Value = Plante.bCompetition(jour) : paramIdx += 1
                        TabDayCmd.Parameters(paramIdx).Value = Plante.dCompFac(icult, jour) : paramIdx += 1
                        TabDayCmd.Parameters(paramIdx).Value = Plante.draint(icult, jour) : paramIdx += 1
                        TabDayCmd.Parameters(paramIdx).Value = 0 : paramIdx += 1 ' SignalFletrissement
                        TabDayCmd.Parameters(paramIdx).Value = 0 : paramIdx += 1 ' Pfactor
                        TabDayCmd.Parameters(paramIdx).Value = 0 ' PfactorMC
                        
                        ' Exécuter la requête préparée (BEAUCOUP plus rapide que de reconstruire la requête à chaque fois)
                        TabDayCmd.ExecuteNonQuery()
                    Next jour
                    
                    ' Commiter la transaction APRÈS tous les jours (gain de performance majeur)
                    transaction.Commit()
                End If 'fin cas où ecritdresus est vrai
            Next icult
            
        Catch ex As Exception
            If transaction IsNot Nothing Then
                transaction.Rollback()
            End If
            Console.WriteLine("Erreur EcritDresu: " & ex.Message)
            Throw
        Finally
            If transaction IsNot Nothing Then
                transaction.Dispose()
            End If
        End Try
    End Sub
    Sub MemoEtatFinal()
        Etatfinal = New EtatFinalClass
        Call Etatfinal.EcritEtatFinal(Sol.dStockSol(SimUnit.nNbJourSimul), Mulch.dQpaillis(SimUnit.nNbJourSimul), False)
    End Sub
    Property Coptionsmodel() As OptionsModelClass
        Get
            Coptionsmodel = OptionsModel
        End Get
        Set()
        End Set
    End Property

End Class
