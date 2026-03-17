Option Compare Text
Option Explicit On
Imports System.Math
Imports System.Data
Imports System.Data.Sqlite
Public Class PLanteClass
    'VX: Variable explicative, VE: variable d'état, VS: variable simulée,PX: paramètre
    Dim DataPlanteAdp As SqliteDataAdapter
    Dim rstDataPlante As DataSet 'tables "cultivars", "PlantSpecies", "StadePheno"
    Dim Cultivar(0 To 2) As String 'identifiant cultivar (lu dans tec_percrop puis recherché dans les tables plantes ici)
    Dim CodeEspece(0 To 2) As String 'lu dans la table "cultivars" ici puis recherché dans Plantspecies
    Dim CodStade(0 To 2, 0 To 10) As String '
    Dim NbStadesPheno(0 To 2) As Integer 'nombre de stades phénologiques considérés
    Dim CTstade(0 To 2, 0 To 10) As Double 'VX, Constante thermique du stade (somme de tempérture seuil de changement de stade)
    Dim TDV(0 To 2, 10) As Double 'VS, taux de développement atteint. commence à zero pour TDV(-,stade)-TDV(-,stade-1)
    'Dim CodOrgForm(1 To 2, 1 To 10) As Integer
    Dim SensPhot(0 To 2) As Double 'VX,Sensibilité à la photopériode **** FONCTIONNEMENT A VERIFIER
    Dim StrsChoc(0 To 2) As Double 'VX, Sensibilité au choc de repiquage (retard en °C.j / unité age de plantules)
    Dim MOPP(0 To 2) As Double 'VX, seuil de durée du jour à partir duquel la photopériode agit sur le développement **** FONCTIONNEMENT A VERIFIER
    Dim tdmin(0 To 2) As Double
    Dim tdmax(0 To 2) As Double 'VX, températures min max et opt de développement (°C). Tdopt présentement non utilisé
    Dim tcmin(0 To 2) As Double
    Dim tcmax(0 To 2) As Double
    Dim tcopt(0 To 2) As Double 'VX, températures min max et opt de conversion de la lumière en biomasse (°C)
    Dim LAI(0 To 2, 731) As Double 'VE, LAI journalier du cultivar dans l'association
    Dim Biom(0 To 2, 731) As Double 'VS (deviendra VE si prise en compte N), Biomasse aérienne totale journalière du cultivar dans l'association (1000kg/ha)
    Dim Grain(0 To 2, 731) As Double 'VS, rendement grain du cultivar dans l'association (T/ha)
    Dim IR(0 To 2) As Double 'VX, indice de récolte du cultivar dans l'assoc.
    Dim DLAImax(0 To 2) As Double 'VX, croit journalier maximal du lai (sd)
    Dim Currstge(0 To 2, 731) As Double 'Stade en cours dans la liste des stades considérés dans la table stadpheno pour le cultivar idCultivar
    Dim DVR(0 To 2) As Double 'VE, Development Rate (emprunté à Oryza) du jour, est égal au temps thermique du jour HU (heat unit) divisé par la somme de température du stade en cours
    ' attention, dans la méthode où DVR est calulé CT désigne l'inverse des constantes thermiques classiques
    Dim DVSt(0 To 2, 731) As Double 'VE, cumul de DVR au cours du temps. Comparé à TDV des stades pour identifier si un stade est complété
    Dim TS(0 To 2) As Double 'VE, temps thermique cumulé (cumul de HU), version non stockée par jour (°C)
    Dim SommeT(0 To 2, 731) As Double 'VE, temps thermique cumulé (cumul de HU), version tableau de stockage par jour (°C)
    Dim TSTR As Double 'VE, temperature sum at transplanting (oryza), sert à calculer l'effet du stress de repiquage: tant que TS < TSTR + stsrschc on ne reprend pas le développement
    Dim DVS(0 To 2) As Double 'VE, cumul de DVR version valeur en cours (°C)
    Dim JulPheno(0 To 2, 0 To 6) As Integer 'VS, date où le stade "n" a été atteint (en jour depuis le1/01 de l'année de début de la simulation)
    Dim cropsta(0 To 2, 731) As Integer 'VE, état du cultivar dans l'assoc
    '0=before sowing; 1=day of sowing; 2=in seedbed;
    '                                    3=after emergence and before transplanting; 4=main growth period
    Dim die(0 To 2) As Boolean 'VE, état de vie (faux) ou mort (vrai) du cultivar
    Dim Transplant(0 To 2) As Boolean 'VX il y a (vrai) où il n'y a pas repiquage/démariage
    Dim JourLaiMax(0 To 2) As Integer 'VS, jour d'atteinte du LAI maximal (en jour depuis le 1er jour de simulation)
    Dim Death_day(0 To 2) As Integer 'VS, jour de la mort éventuelle de la culture (en jour depuis le 1er jour de simulation)
    Dim JourSen(0 To 2) As Integer 'VS, jour du début de sénescence du cultivar (en jour depuis le 1er jour de simulation)
    Dim JourDrp(0 To 2) As Integer 'VS jour de début de remplissage des grains pour le cultivar (en jour depuis le 1er jour de simulation)
    Dim JourMat(0 To 2) As Integer 'VS, jour de maturité physiologique du cultivar (en jour depuis le 1er jour de simulation)
    Dim Hu(0 To 2) As Double 'VE, Heat Units (Oryza), temps thermique en °C jour, présentement calculé dans HUstics (sans effet de Tdopt)
    Dim densplt(0 To 2, 731) As Double 'VX, densité de plantes au semis ( /m2)
    Dim adens(0 To 2) As Double 'VX, coefficient de sensibilité aux densités élevées (voir Stics)
    Dim bdens(0 To 2) As Double 'VX, seuil de densité à partir duquel la surface foliaire par plante dépend de la densité
    Dim Laicomp(0 To 2) As Double 'VX, LAI seuil à partir duquel la densité de peuplement peut avoir un impact sur la surface foliaire par plante (au-delà du seuil de densité bdens)
    Dim LAIrec(0 To 2) As Double 'VX, LAI résiduel à la récolte
    Dim CoefExtin(0 To 2) As Double 'VX, coefficient d'extinction du rayonnement par le LAI
    Dim Ebmax(0 To 2) As Double 'VX, epsilon b max, taux potentiel de conversion du rayonnement en biomasse, gMS.MJ(-1)
    Dim Zrac(0 To 2, 731) As Double 'cote atteinte par les racines (cm)
    Dim DeltaRacMax(0 To 2) As Double ' VX croissance journalière de la profondeur atteinte par les racines par unité de temps thermique (cm/°.j)
    Dim Zracmax(0 To 2) As Integer  'VX  Cote maximale atteignable par les racines (cm) ***attention sera réduite à Sol.Ztotsol si Zracmx > Ztotsol
    'ajouts antérieurs au 26/07/11, spécifiques Quyen, insérés ici le 18/08/11 pour fusion avec ajouts "composantes du rendement" du 26/07

    Dim Ncold(0 To 2) As Integer 'VE, nombre de jours consécutifs où la température est inférieure au seuil de sensibilité au froid Tcold
    Dim Tcold(0 To 2) As Double 'VX, Température seuil de sensibilité au froid (°C)
    Dim NDieCold(0 To 2) As Integer 'VX, nombre maximal de jours inférieurs à Tcold supportable par la culture sans mortalité

    ' ***ajouts 26/07/2011
    Dim deltaBiom(0 To 2, 731) As Double
    Dim Vitmoy(0 To 2) As Double 'VE, vitesse moyenne de croissance pendant la phase de détermination du nombre de grains (en g/m2/j)
    Dim Nbjgrain(0 To 2) As Integer 'VX, nombre de jours déterminant le nombre de grains avant le stade 4 (début remplissage du grain)
    Dim Ngrains(0 To 2) As Long 'VE, nombre de grains par m2
    Dim Cgrain(0 To 2) As Integer 'VX, nombre de grains mis en place par gMS/jour de croissance moyenne de biomasse pendant les nbjgrain précédent le début du stade 4
    Dim Cgrainv0(0 To 2) As Integer 'VX, nombre de grains mis en place si croissance nulle pendant les nbjgrain (grains /m2)
    Dim Ngrmax(0 To 2) As Long 'VX nombre maximal de grains par plante
    Dim Vitircarb(0 To 2) As Double 'VX, augmentation journalière de l'indice de récolte (g grain. g(-1) Ms j(-1))
    Dim IRmax(0 To 2) As Double 'VX, indice de récolte maximal du cultivar
    Dim P1grainMax(0 To 2) As Double 'VX poids maximal de 1 grain (g)
    Dim P1grain(0 To 2, 731) As Double 'VE poids d'un grain (g)
    Dim Kmax(0 To 2) As Double 'Vx coefficient cultural max de l'espece quand LAI >5
    Dim TSlevee(0 To 2) As Integer 'VE, temps thermique cumulé pour la levée+germination(cumul de HU)
    Dim CTlevee(0 To 2) As Integer 'VX constante thermique de germination-levée
    Dim LevSim(0 To 2) As Integer 'VE, date de levée simulée en jour de la simulation
    Dim Tger(0 To 2) As Double 'VX, températures base pour levée (°C)
    Dim stressN As Double 'VE coefficient de réduction de l'efficience de conversion en fonction du statut azoté de la plante (0 pas de croissance, 1 pas de stress)
    Dim LegumON(0 To 2) As Boolean 'VX, légumineuse oui ou non
    Dim Nsymb(0 To 2) As Double  'VX, stock de nutriments (azote) fixé par symbiose mycorhizienne
    Dim IFertMax(0 To 2) As Double 'azote (kg/ha): seuil de fertilisation au delà duquel l'indice de satisfaction ds besoins nutritifs est égal à 1
    ' a vérifier nouvelles variables introduites par CP
    Dim NCvEmax(0 To 2) As Double 'efficience de conversion maximale de l'azote en biomasse (kg MS/kg N)
    Dim NCvEmin(0 To 2) As Double 'efficience de conversion minimale de l'azote en  biomasse (kg MS/kg N)
    Dim alphaN(0 To 2) As Double 'coefficient de réglage de l'efficience de conversion de l'azote en biomasse
    Dim PCvEmax(0 To 2) As Double 'efficience de conversion maximale du phosphore en biomasse (kg MS/kg P)
    Dim PCvEmin(0 To 2) As Double 'efficience de conversion minimale du phosphore en  biomasse (kg MS/kg P)
    Dim alphaP(0 To 2) As Double 'coefficient de réglage de l'efficience de conversion du phosphore en biomasse
    Dim KCvEmax(0 To 2) As Double 'efficience de conversion maximale du potassium en biomasse (kg MS/kg K)
    Dim KCvEmin(0 To 2) As Double 'efficience de conversion minimale du potassium en  biomasse (kg MS/kg K)
    Dim alphaK(0 To 2) As Double 'coefficient de réglage de l'efficience de conversion du potassium en biomasse
    Dim NRF(0 To 2, 0 To 731) As Double 'nitrogen reduction factor = dispo/demande
    Dim PRF(0 To 731) As Double 'phosphorus reduction factor = dispo/demande
    Dim KRF(0 To 731) As Double 'potassium reduction factor = dispo/demande
    ' a vérifier variable FA pas utilisée finalement QstressH ???
    Dim QstressH As Double 'VE signal de réchauffement de l'air en cas de stress hydrique (attention culture associée non pensée)
    Dim SensiSen(0 To 2) As Double 'VX sensibilité de la sénéscence aux stress post floraison
    Dim NJFletri(0 To 2) As Integer  'VX nombre de jours consécutifs de sol au point de flétrissement au-delà duquel il ya mort de la culture (dans plantespecies))
    'A verifier, valeurs introduites dans tables Todo a lire dans table
    Dim Zgraine(0 To 2) As Integer 'VX, épaisseur de l'horizon concerné par la germination (cm)
    'Const SeuilTurg As Double = 0.25  ' PX 1-seuil d'effet de la contrainte hydrique sur la croissance du LAI (règle lien entre contrainteH et turfac)
    'Const SeuilWS As Double = 0.35 'PX, 1-seuil d'effet de la contrainte hydrique sur la croissance de la biomasse(règle lien entre contrainteH et WSfact)
    Dim SeuilTurg(0 To 2) As Double   ' PX 1-seuil d'effet de la contrainte hydrique sur la croissance du LAI (règle lien entre contrainteH et turfac)
    Dim SeuilWS(0 To 2) As Double  'PX, 1-seuil d'effet de la contrainte hydrique sur la croissance de la biomasse(règle lien entre contrainteH et WSfact)
    'ci-dessous variables du module stressAzote
    Dim NavailCult(0 To 2, 0 To 731) As Double   'stock d'azote disponible en kg/ha en tenant compte de la fixation symbiotique (Navail + Nsymbjour)
    Dim NUPTtarget(0 To 2, 0 To 731) As Double 'demande en azote (kg/ha), dépend du croît de biomasse du jour sans stress nutritif
    Dim Nuptake(0 To 2, 0 To 731) As Double  'quantité d'azote réellement absorbée par plante = NUPTtarget si aucun autre facteur limitant
    Dim SigmaNuptake(0 To 2, 0 To 731)  'cumul de la consommation d'azote par la plante
    Dim Nsymbjour(0 To 2) As Double 'quantité d'azote fixée par jour = Nsymb/durée du cyle
    'A vérifier attention CP a vectorisé toutes ces variables - pas sur que ce soit bonne idée (tps calcul !)
    ' facteurs de stress
    Dim TurfacH(0 To 731) As Double 'utilisé dans Calcule_LAI
    Dim Turfac(0 To 731) As Double 'utilisé dans Calcule_LAI
    Dim WSfactH(0 To 731) As Double 'utilisé dans Biomasse
    Dim WSfact(0 To 731) As Double 'utilisé dans Biomasse
    'variables en lien avec la compétition pour la lumière
    Dim Competition(0 To 731) As Boolean 'si true il y a compétition pour la lumière entre les deux espèces de l'association
    Dim CompFac(0 To 2, 0 To 731) As Double 'facteur de compétition pour la lumière
    Dim raint(0 To 2, 0 To 731) As Double
    ' variables effet changement climatique
    Dim alphaCO2(0 To 2) As Double 'VX sensibilité de la conversion en biomasse à la concentration en CO2 de l'atmosphere (C3: 1.2; C4: 1.1)
    Dim CO2fact(0 To 2) As Double 'VE facteur de réduction par le CO2 de la conversion du rayonnement en biomasse
    Dim LAIpot(0 To 2) As Double 'VS lai potentiel maxi qui serait atteint sans stress à julpheno_3
    ' (prévu pour utilisation dans effet des stress post julpheno3 dans calculeLAIsemiaride mais finalement pas utilisé)
    Const Kmo = 0.25 'VX pour stresNold part de l'azote apporté par amendements organique minéralisé (net) par an
    'modif le 26/07/2011 (composantes du rendement)
    ' modif le 18/08/2011
    ' modif FA 25/08/2011 controle division par zero P1grain
    '  modif FA 2/12/2011 passage de Ngrmax en type long !!!
    '  modif FA correction parenthèses et effet parabolique raint calcul deltabiom le 16/12/11
    ' modif 02/4/2012 introduction Kmax lu dans table plantespecies
    ' modif FA 04/05/2012 germination levee
    ' modif FA 7/12/12 stress N et distinction legumineuse / non legumineuse
    ' modif FA 12/02/13 effet stressH sur développement phéno via réchauffement de l'air ds la culture
    ' 16/04 introduction  mort de la culture quand sol au pt de flétrissement pndt NJFletri
    'modif CP 30/05/13 lecture de Zgraine dans la table Plantspecies
    'modif CP 04/06/13 IFertmax lu dans Cultivars plutôt que dans PlantSpecies
    'modif CP 06/06/13 ajout d'un tApportMin pour distinguer les apports de fumure minérale des apports de fumure organique
    'modif CP 07/06/13 modification du calcul de StressN (cf module stressazote) ; Nsymb passe dans la table Plantespecies
    'modif FA fletrissement avant stade croissance rapide dans pheno_sigmaT phenosigmaT et levee revus pour simulation deux plantes associées
    'Modi FA 12_08_13: lecture vitircarb dasn cultivar et plus dans plantspecies
    ' et lai Corrigé pour cas des plantes avec lairec non nul et effet stress sur sénéscence
    'modif CP 17/09/13 nouveau module de calcul du stress azoté
    'modif CP 17/09/13 dimensionnement de WSfactH, WSfact, TurfacH et Turfac juste
    'au-dessus pour pouvoir les rendre publiques et les faire afficher dans OutputD1
    'modif CP 20/09/13 début introduction compétition pour la lumière module LAI
    'modif CP 04/10/13 modification du bilan azoté pour que tout soit calculé sur un pas de
    'temps journalier, notamment Nsymb et les pertes vers l'atmosphère
    'dernière modif CP 09/10/13 lecture de SeuilTurg et SeuilWS dans PlantSpecies
    '
    ' relu par FA 15/11/13
    ' a vérifier:
    ' pertinence de passer en vecteur jour les variables de stress
    ' effet de comptétition sur rayonnement intercepté
    ' comemntaires insérés
    ' modif le 24/04/14 introduction fonction FCO2 appelée dans iniplante, effet sur biomasse dans biomasse, lecture alphaCO2 dans lisplante (plantespecies)
    '  Fa le 15/04/15 Ajout commentaires dans PhenoSigmaT pour faciliter compréhension
    ' modif Lisplante pour lecture Cgrain et CgrainV0 dans cultivars
    'derniere modif le 20/04/15 insertion d'un message d'erreur dans iniplante si date semis avant début de la simul
    ' modif 10/06/15 introduction d'une nouvelle méthode de calcul du LAI avec réduction du LAI par les stress post-stade LAI maxi
    'modif 24/5/2016 nouvelle routine phenoCTphot pour developpement(pas de nouvelle variable declaree)
    ' modif 25/5/16 introduction JourLaiMax pour sortie de LAi à cette date (et non à début de sénéscence) dans outpourSynt (et modif des deux routines LAI en conséquence)
    ' a vérifier:
    ' pertinence de passer en vecteur jour les variables de stress
    ' effet de comptétition sur rayonnement intercepté
    ' effet stress hydrique sur développement
    ' commentaires insérés
    '
    'VERSION 3 du 8-10 nov 2017'
    Sub LisPlante(DataBase_Cnn As SqliteConnection, GestionTechnique As GestionTechniqueClass)
        Dim Trouve As Boolean
        Dim icult As Integer
        Dim Numstade As Integer

        'solnu = False attention faire test sur sol nu avant lisplante, en utilisant NbCult=0
        For icult = 1 To GestionTechnique.nNbCult
            'rstDataPlante = New ADODB.Recordset
            Trouve = False
            Cultivar(icult) = GestionTechnique.sIdCultivar(icult)
            rstDataPlante = New DataSet
            DataPlanteAdp = New SqliteDataAdapter("SELECT * FROM Cultivars where idCultivar='" & Cultivar(icult) & "'", DataBase_Cnn)
            DataPlanteAdp.Fill(rstDataPlante)
            If rstDataPlante.Tables(0).Rows.Count > 0 Then
                'While Not rstDataPlante.EOF And Not Trouve
                'If rstDataPlante("IdCultivar").Value = Cultivar(icult) Then

                Trouve = True
                CodeEspece(icult) = rstDataPlante.Tables(0).Rows(0)("CodePSpecies")
                NbStadesPheno(icult) = rstDataPlante.Tables(0).Rows(0)("NbStadesPheno")
                SensPhot(icult) = rstDataPlante.Tables(0).Rows(0)("SensPhot")
                If GestionTechnique.bRepiquageON(icult) Then
                    StrsChoc(icult) = rstDataPlante.Tables(0).Rows(0)("StrsChoc")
                    Transplant(icult) = True
                Else
                    StrsChoc(icult) = 0
                    Transplant(icult) = False
                End If
                MOPP(icult) = rstDataPlante.Tables(0).Rows(0)("MOPP")
                adens(icult) = rstDataPlante.Tables(0).Rows(0)("adens")
                bdens(icult) = rstDataPlante.Tables(0).Rows(0)("bdens")
                Laicomp(icult) = rstDataPlante.Tables(0).Rows(0)("Laicomp")
                Vitircarb(icult) = rstDataPlante.Tables(0).Rows(0)("Vitircarb")
                IRmax(icult) = rstDataPlante.Tables(0).Rows(0)("IRmax")
                P1grainMax(icult) = rstDataPlante.Tables(0).Rows(0)("P1grainMax")
                Ngrmax(icult) = rstDataPlante.Tables(0).Rows(0)("Ngrmax")
                IFertMax(icult) = rstDataPlante.Tables(0).Rows(0)("IFertMax")
                Cgrain(icult) = rstDataPlante.Tables(0).Rows(0)("Cgrain")
                Cgrainv0(icult) = rstDataPlante.Tables(0).Rows(0)("Cgrainv0")

            End If
            'rstDataPlante.MoveNext()
            'End While
            'fermeture table et libération mémoire de l'objet
            'rstDataPlante.Close()
            'rstDataPlante = New ADODB.Recordset
            Trouve = False
            rstDataPlante.Clear()
            DataPlanteAdp = New SqliteDataAdapter("SELECT * FROM PlantSpecies where CodePSpecies='" & CodeEspece(icult) & "'", DataBase_Cnn)
            DataPlanteAdp.Fill(rstDataPlante)

            'While Not rstDataPlante.EOF And Not Trouve
            'If rstDataPlante("CodePSpecies").Value = CodeEspece(icult) Then
            'If rstDataPlante.Tables(0).Rows.Count > 0 Then
            If rstDataPlante.Tables(0).Rows.Count > 0 Then
                Trouve = True
                tdmin(icult) = rstDataPlante.Tables(0).Rows(0)("tdmin")
                tdmax(icult) = rstDataPlante.Tables(0).Rows(0)("tdmax")
                tcmin(icult) = rstDataPlante.Tables(0).Rows(0)("tcmin")
                tcmax(icult) = rstDataPlante.Tables(0).Rows(0)("tcmax")
                tcopt(icult) = rstDataPlante.Tables(0).Rows(0)("tcopt")
                DLAImax(icult) = rstDataPlante.Tables(0).Rows(0)("DLAImax")
                LAIrec(icult) = rstDataPlante.Tables(0).Rows(0)("LAIrecmax")
                CoefExtin(icult) = rstDataPlante.Tables(0).Rows(0)("extin")
                Ebmax(icult) = rstDataPlante.Tables(0).Rows(0)("Ebmax")
                DeltaRacMax(icult) = rstDataPlante.Tables(0).Rows(0)("DeltaRacMax")
                Zracmax(icult) = rstDataPlante.Tables(0).Rows(0)("Zracmax")
                Tcold(icult) = rstDataPlante.Tables(0).Rows(0)("Tcold")
                NDieCold(icult) = rstDataPlante.Tables(0).Rows(0)("NDieCold")
                Nbjgrain(icult) = rstDataPlante.Tables(0).Rows(0)("Nbjgrain")
                Kmax(icult) = rstDataPlante.Tables(0).Rows(0)("Kmax")
                'simulation levee
                CTlevee(icult) = rstDataPlante.Tables(0).Rows(0)("CTlevee")
                Tger(icult) = rstDataPlante.Tables(0).Rows(0)("Tger")
                LegumON(icult) = rstDataPlante.Tables(0).Rows(0)("LegumON")
                SensiSen(icult) = rstDataPlante.Tables(0).Rows(0)("SensiSen")
                NJFletri(icult) = rstDataPlante.Tables(0).Rows(0)("NJFletri")
                Zgraine(icult) = rstDataPlante.Tables(0).Rows(0)("Zgraine")
                Nsymb(icult) = rstDataPlante.Tables(0).Rows(0)("Nsymb")
                NCvEmax(icult) = rstDataPlante.Tables(0).Rows(0)("NCvEmax")
                NCvEmin(icult) = rstDataPlante.Tables(0).Rows(0)("NCvEmin")
                alphaN(icult) = rstDataPlante.Tables(0).Rows(0)("alphaN")
                PCvEmax(icult) = rstDataPlante.Tables(0).Rows(0)("PCvEmax")
                PCvEmin(icult) = rstDataPlante.Tables(0).Rows(0)("PCvEmin")
                alphaP(icult) = rstDataPlante.Tables(0).Rows(0)("alphaP")
                Nsymbjour(icult) = rstDataPlante.Tables(0).Rows(0)("Nsymbjour")
                SeuilTurg(icult) = rstDataPlante.Tables(0).Rows(0)("SeuilTurg")
                SeuilWS(icult) = rstDataPlante.Tables(0).Rows(0)("SeuilWS")
                alphaCO2(icult) = rstDataPlante.Tables(0).Rows(0)("alphaCO2")
            End If
            '     rstDataPlante.MoveNext()
            ' End While
            'fermeture table et libération mémoire de l'objet
            rstDataPlante.Clear()
            DataPlanteAdp = New SqliteDataAdapter("Select * FROM StadePheno where CodCultivar='" & Cultivar(icult) & "' Order by NumStade", DataBase_Cnn)
            DataPlanteAdp.Fill(rstDataPlante)
            For i = 0 To rstDataPlante.Tables(0).Rows.Count - 1
                Numstade = rstDataPlante.Tables(0).Rows(i)("Numstade")
                '          CodStade(icult, Numstade) = rstDataPlante.Tables(0).Rows(0)("CodStade")
                CTstade(icult, Numstade) = rstDataPlante.Tables(0).Rows(i)("CTstade")
                TDV(icult, Numstade) = rstDataPlante.Tables(0).Rows(i)("TDV")
                '            CodOrgForm(icult, Numstade) = rstDataPlante.Tables(0).Rows(0)("CodOrgForm
            Next

            'fermeture table et libération mémoire de l'objet
            'rstDataPlante.Close()
            'rstDataPlante = Nothing
            'initialisations provisoires
            'Zracmax(icult) = 100
            'DeltaRacMax(icult) = 3
        Next icult


    End Sub
    Sub Iniplante(icult As Integer, DensSem As Double, isem As Integer, densrepiqu As Double, irepiqu As Integer, DebutDOY As Integer, finDOY As Integer, LimiteSol As Integer, CO2c As Integer)
        Dim Joursim As Integer
        Dim jourfin As Integer
        Dim Dens As Double

        CO2fact(icult) = FCO2(CO2c, icult)
        die(icult) = True
        If irepiqu <> 999 Then jourfin = irepiqu - DebutDOY + 1 Else jourfin = finDOY
        If Zracmax(icult) > LimiteSol Then Zracmax(icult) = LimiteSol
        If isem - DebutDOY + 1 < 0 Then
            Console.WriteLine("semis avant le début de la simulation, stop! corrigez TechPerCrop")
        End If

        For Joursim = isem - DebutDOY + 1 To jourfin - 1
            densplt(icult, Joursim) = DensSem
        Next Joursim
        For Joursim = jourfin To finDOY
            densplt(icult, Joursim) = densrepiqu
        Next Joursim

        If irepiqu <> 999 Then Dens = densrepiqu Else Dens = DensSem

        If Dens <= bdens(icult) Then LAIrec(icult) = LAIrec(icult) * Dens / bdens(icult)
        'ajouts pour ressemis
        TSlevee(icult) = 0
        TS(icult) = 0
        DVS(icult) = 0
        DVR(icult) = 0

    End Sub
    Sub AdapteCT(Nbcult As Integer)
        ' conversion des parametres de développement pour saisir des constantes
        ' thermiques en °C.j et conserver le formalisme de Oryza pour l'age physiologique (development stage)
        Dim icult As Integer
        Dim Istade As Integer
        For icult = 1 To Nbcult
            For Istade = 1 To NbStadesPheno(icult)
                CTstade(icult, Istade) = Inverse(CTstade(icult, Istade)) * (TDV(icult, Istade) - TDV(icult, Istade - 1))
            Next Istade
        Next icult
    End Sub
    Function HUstics(Tm As Double, icult As Integer) As Double

        Dim TT As Double
        'insérer ici correction température par stress H (QstressH)
        TT = Tm - tdmin(icult)
        If (Tm < tdmin(icult)) Then TT = 0
        If (Tm > tdmax(icult)) Then TT = tdmax(icult) - tdmin(icult)
        HUstics = TT
        'HUstics=TT*(1+QstressH)pour prise en compte effet stress hydrique sur température de culture  ????? a regarder
    End Function
    Function HUleve(Tm As Double, icult As Integer) As Double

        Dim TTleve As Double

        TTleve = Tm - Tger(icult)
        If (Tm < Tger(icult)) Then TTleve = 0
        HUleve = TTleve

    End Function
    Sub GerminLevee(DOY As Integer, icult As Integer, Joursim As Integer, ilev As Integer, simlevee As Boolean, ContrainteHlevee As Boolean, Tm As Double)
        'procédure appellée pour joursim compris entre joursemis et ilev (inclus), via test sur cropsta

        'Dim msg As String
        cropsta(icult, Joursim) = 2
        If Not simlevee And ilev <> 0 And ilev <> 999 Then
            '    If DOY < ilev Then cropsta(icult, Joursim) = 2
            If DOY = ilev Then
                If Transplant(icult) Then cropsta(icult, Joursim) = 3 Else cropsta(icult, Joursim) = 4
            End If

        Else
            If Not ContrainteHlevee Then
                TSlevee(icult) = TSlevee(icult) + HUleve(Tm, icult)
                If TSlevee(icult) >= CTlevee(icult) Then
                    ' ligne suiv. et variable LevSim pas forcément utile ?
                    '            LevSim(icult) = Joursim
                    If Transplant(icult) Then cropsta(icult, Joursim) = 3 Else cropsta(icult, Joursim) = 4
                End If
            End If
        End If
    End Sub
    Sub stressAzoteOld(icult As Integer, Joursim As Integer, StockN As Double, ApportMO As Double, ApportMin As Double)
        Dim QNut As Double
        QNut = StockN + Kmo * ApportMO + ApportMin + Nsymb(icult)
        stressN = QNut / IFertMax(icult)
        If stressN > 1 Then stressN = 1
        NRF(icult, Joursim) = stressN
        ' vérifier si utile conserver deux variables NRF et stressN

    End Sub
    Sub phenoCTphot(dataclim As DataClimClass, joursim As Integer, SignalFletrissement As Integer, icult As Integer)
        'Attention NOUVELLE approche plus directe que celle empruntée à Oryza, sans inversion des constantes thermiques.
        ' Verifications en cours. OK sur le jeu 840 Senegal sans photosensibilité, différences mineures (+- 1j sur stade6) avec Sigma_CT si photosensibilite...
        ' verifier fletrissement, froid, repiquage...

        Dim DL As Double 'mort de la culture si le sol est au point de flétrissement depuis plus de NJFletri
        Dim PPFAC As Double
        Dim TxDev As Double

        Dim StopTransplant As Integer

        Currstge(icult, joursim) = Currstge(icult, joursim - 1)
        PPFAC = 1

        If SignalFletrissement >= NJFletri(icult) And Currstge(icult, joursim) < 2 Then
            die(icult) = True
            Death_day(icult) = dataclim.nDOY(joursim)
            'Currstge(icult, joursim) = 6 'verifier
            cropsta(icult, joursim) = 0
        End If

        ' début prise en compte froid
        If dataclim.dTmoy(joursim) < Tcold(icult) Then Ncold(icult) = Ncold(icult) + 1 Else Ncold(icult) = 0

        If Ncold(icult) = NDieCold(icult) Then
            die(icult) = True
            Death_day(icult) = dataclim.nDOY(joursim)
            Currstge(icult, joursim) = 6 'verifier
            cropsta(icult, joursim) = 0 'verifier
        End If
        'fin froid

        ' les phases 1 à 5 ci dessous (currstge 1 = croissance lente du LAI, julpheno1= levée; currstge5 = sénéscence feuilles et poursuite du remplissage du grain, julpheno6= maturité, julpheno)
        Hu(icult) = HUstics(dataclim.dTmoy(joursim), icult)

        ' prise en charge photopériode
        If Currstge(icult, joursim) = 2 Then
            DL = dataclim.dDAYL(joursim) + 0.9
            If (DL < MOPP(icult)) Then
                PPFAC = 1.0#
            Else
                PPFAC = 1.0# - (DL - MOPP(icult)) * SensPhot(icult)
            End If
            PPFAC = Min(1.0#, Max(0#, PPFAC))
        End If
        'somme T sans frein photopériodique pour sortie journalière
        SommeT(icult, joursim) = SommeT(icult, joursim - 1) + Hu(icult)
        ' Somme T pour calcul du développement avec frein photopériodique
        TxDev = Hu(icult) * PPFAC
        If (cropsta(icult, joursim - 1) > 3 And TS(icult) + TxDev < (TSTR + StrsChoc(icult))) Then StopTransplant = 0 Else StopTransplant = 1

        'attention StrsChoc doit etre nul si pas de repiquage

        TxDev = TxDev * StopTransplant
        TS(icult) = TS(icult) + TxDev


        'DVS : cumul des constantes thermiques jusqu'au stade en cours inclus (le stade change quand la somme photothermique dépasse DVS)
        If TS(icult) >= DVS(icult) Then
            Currstge(icult, joursim) = Currstge(icult, joursim) + 1
            If Currstge(icult, joursim) = 6 Then
                die(icult) = True
                JourMat(icult) = joursim
            Else
                ' augmentation du seuil de temps thermique de la constante thermique du nouveau stade
                DVS(icult) = DVS(icult) + CTstade(icult, Currstge(icult, joursim))
            End If

        End If
        If Currstge(icult, joursim) < 6 Then
            'DVSt(icult, joursim): somme des taux de developpement normalisés selon échelle des TDV(stade), pour emploi dans calcul Lai
            DVSt(icult, joursim) = DVSt(icult, joursim - 1) + (TDV(icult, Currstge(icult, joursim)) - TDV(icult, Currstge(icult, joursim) - 1)) * TxDev / CTstade(icult, Currstge(icult, joursim))
        End If

        '!-----Set CROPSTA: 0=before sowing; 1=day of sowing; 2=in seedbed;
        '!                  3=day of transplanting; 4=main growth period

        'attention dans ce qui suit voir s'il faut calculer TSTR par icult si cultures associées...
        If (cropsta(icult, joursim) = 4) And cropsta(icult, joursim - 1) = 3 Then TSTR = TS(icult)



        'la date julpheno(icult, j) est la date à laquelle un nouveau currstge(icult) démarre julpheno1 le lendemain du dernier jour de currstge1
        If Currstge(icult, joursim) > Currstge(icult, joursim - 1) Then JulPheno(icult, Currstge(icult, joursim)) = dataclim.nDOY(joursim)



    End Sub
    Sub pheno_sigmaT(dataClim As DataClimClass, Joursim As Integer, SignalFletrissement As Integer, icult As Integer)
        'attention avbec cette routine CT doit être l'inverse des données entrées dans table StadePheno, voir AdapteCT
        Dim DL As Double
        Dim PPFAC As Double
        'mort de la culture si le sol est au point de flétrissement depuis plus de NJFletri

        If SignalFletrissement >= NJFletri(icult) And DVS(icult) < TDV(icult, 2) Then
            die(icult) = True
            Death_day(icult) = dataClim.nDOY(Joursim)
            DVS(icult) = TDV(icult, 5)
            cropsta(icult, Joursim) = 0
        End If

        If TS(icult) = 0 Then DVR(icult) = CTstade(icult, 1)

        ' a reprendre ce qui suit pour généricité
        Hu(icult) = HUstics(dataClim.dTmoy(Joursim), icult)
        ' redondance TS, SommeT et DVS, DVSt ?
        ' somme de temps thermique (Hu = heat unit= le delta de temps thermique du jour)
        TS(icult) = TS(icult) + Hu(icult)
        'le taux de développement
        DVR(icult) = DVR(icult) * Hu(icult)
        'le stockage du temps thermique dans tableau pour OutputD
        SommeT(icult, Joursim) = TS(icult)
        'le stade de développement en continu
        DVS(icult) = DVS(icult) + DVR(icult)
        ' début prise en compte froid
        If dataClim.dTmoy(Joursim) < Tcold(icult) Then Ncold(icult) = Ncold(icult) + 1 Else Ncold(icult) = 0

        If Ncold(icult) = NDieCold(icult) Then
            die(icult) = True
            Death_day(icult) = dataClim.nDOY(Joursim)
            DVS(icult) = TDV(icult, 5)
            ' ligne ci-dessus provoque plus loin die=true et DVR=0
        End If
        'fin froid
        ' les phases 1 à 5 ci dessous (currstge 1 = croissance lente du LAI, julpheno1= levée; currstge5 = sénéscence feuilles et poursuite du remplissage du grain, julpheno6= maturité, julpheno)
        DVSt(icult, Joursim) = DVS(icult)
        If (DVS(icult) >= 0 And DVS(icult) < TDV(icult, 1)) Then
            DVR(icult) = CTstade(icult, 1)
            Currstge(icult, Joursim) = 1
        End If
        If (DVS(icult) >= TDV(icult, 1) And DVS(icult) < TDV(icult, 2)) Then
            Currstge(icult, Joursim) = 2
            ' prise en charge photopériode
            DL = dataClim.dDAYL(Joursim) + 0.9
            If (DL < MOPP(icult)) Then
                PPFAC = 1.0#
            Else
                PPFAC = 1.0# - (DL - MOPP(icult)) * SensPhot(icult)
            End If
            PPFAC = Min(1.0#, Max(0#, PPFAC))
            DVR(icult) = CTstade(icult, 2) * PPFAC
        End If
        If (DVS(icult) >= TDV(icult, 2) And DVS(icult) < TDV(icult, 3)) Then
            DVR(icult) = CTstade(icult, 3)
            Currstge(icult, Joursim) = 3
        End If
        If (DVS(icult) >= TDV(icult, 3) And DVS(icult) < TDV(icult, 4)) Then
            DVR(icult) = CTstade(icult, 4)
            Currstge(icult, Joursim) = 4
        End If
        If (DVS(icult) >= TDV(icult, 4) And DVS(icult) < TDV(icult, 5)) Then
            DVR(icult) = CTstade(icult, 5)
            Currstge(icult, Joursim) = 5
        End If
        If DVS(icult) >= TDV(icult, 5) Then
            die(icult) = True
            DVR(icult) = 0
            Currstge(icult, Joursim) = 6
            If Currstge(icult, Joursim - 1) = 5 Then
                JourMat(icult) = Joursim
                die(icult) = True
            End If

        End If
        '!-----Set CROPSTA: 0=before sowing; 1=day of sowing; 2=in seedbed;
        '!                  3=day of transplanting; 4=main growth period

        'attention dans ce qui suit voir s'il faut calculer TSTR par icult...
        If (cropsta(icult, Joursim) = 4) And cropsta(icult, Joursim - 1) = 3 Then TSTR = TS(icult)

        If (cropsta(icult, Joursim - 1) > 3 And TS(icult) < (TSTR + StrsChoc(icult))) Then DVR(icult) = 0#

        'attention StrsChoc doit etre nul si pas de repiquage
        'la date julpheno(icult, j) est la date à laquelle un nouveau currstge(icult) démarre julpheno1 le lendemain du dernier jour de currstge1
        If Currstge(icult, Joursim) > Currstge(icult, Joursim - 1) Then JulPheno(icult, Currstge(icult, Joursim)) = dataClim.nDOY(Joursim)

    End Sub
    'Sub stressAzote(icult As Integer, joursim As Integer, Navail As Double)
    '    'proposition du calcul du stress azoté inspiré de FIELD (QUEFTS) mais sur pas de temps journalier
    '    'pas testé de manière apporfondie au 8/11/2017

    '    'On considère que l'efficience de capture de l'azote est de 1 une fois les pertes gazeuses
    '    'et par drainage retranchées à Navail, et considérant les autres nutriments non limitant
    '    'Nuptake = N availability
    '    'Mais dans l'absolu, dans FIELD (QUEFTS) NCtE = Nuptake/Navail

    '    NavailCult(icult, joursim) = Navail + Nsymbjour(icult)

    '    If Biom(icult, joursim - 1) > 0 Then
    '        NUPTtarget(icult, joursim) = Biom(icult, joursim - 1) / (NCvEmin(icult) + ((NCvEmax(icult) - NCvEmin(icult)) * alphaN(icult)))
    '        NRF(icult, joursim) = NavailCult(icult, joursim) / NUPTtarget(icult, joursim)
    '        If NRF(icult, joursim) > 1 Then NRF(icult, joursim) = 1
    '    Else
    '        NRF(icult, joursim) = 1
    '    End If
    '    'd
    '    'si alphaN = 0.5    NCvE = médiane (NCvEmin et NCvEmax)
    '    'si alphaN = 1      NCvE = NCvEmax
    '    'si alphaN = 0      NCvE = NCvEmin

    'End Sub
    Sub stressAzote(icult As Integer, Joursim As Integer, Navail As Double)
        'calcul du stress azoté inspiré de FIELD (QUEFTS) mais sur pas de temps journalier
        'dernière modif le 04/10/13 pour fonctionnement en journalier complet

        'On considère que l'efficience de capture de l'azote est de 1 une fois les pertes gazeuses
        'et par drainage retranchées à Navail, et considérant les autres nutriments non limitant
        'Nuptake = N availability
        'Mais dans l'absolu, dans FIELD (QUEFTS) NCtE = Nuptake/Navail

        NavailCult(icult, Joursim) = Navail + Nsymbjour(icult)

        If Biom(icult, Joursim - 1) > 0 Then
            NUPTtarget(icult, Joursim) = Biom(icult, Joursim - 1) / (NCvEmin(icult) + ((NCvEmax(icult) - NCvEmin(icult)) * alphaN(icult)))
            NRF(icult, Joursim) = NavailCult(icult, Joursim) / NUPTtarget(icult, Joursim)
            If NRF(icult, Joursim) > 1 Then NRF(icult, Joursim) = 1
        Else
            NRF(icult, Joursim) = 1
        End If
        'd
        'si alphaN = 0.5    NCvE = médiane (NCvEmin et NCvEmax)
        'si alphaN = 1      NCvE = NCvEmax
        'si alphaN = 0      NCvE = NCvEmin

    End Sub
    Sub Calcule_LAI(Vlaimax As Double, Joursim As Integer, icult As Integer, ContrainteW As Double, ActivestressH As Boolean, ActivestressN As Boolean, LaiMC As Double, Nbcult As Double)
        'dernière modif CP 25/09/13 introduction d'un effet de la compétition pour la lumière sur le LAi
        'ajout de la constante LAIseuilComp et de la variable Booléenne Competition (dimensionnée en haut)
        'dLAI multiplié par CompFac, le facteur de réduction de la croissance du LAI lié à la compétition
        'pour la lumière (pas pris en compte pendant la sénéscence pour l'instant)

        Dim Ulai As Double
        Dim dLAI As Double
        Dim dLAISen As Double
        Dim TurfacN As Double
        ' réintriduction FA 15/11 pour retour éventuel à variables locales pour les stress!
        'Dim TurfacH As Double
        'Dim Turfac As Double
        ' constante suivante à transformer en variable !
        Const LAIseuilComp As Double = 0.1 'lai seuil à partir duquel la compétition pour la lumière a lieu

        If ContrainteW > (1 - SeuilTurg(icult)) Or Not (ActivestressH) Then TurfacH(Joursim) = 1 Else TurfacH(Joursim) = ContrainteW / (1 - SeuilTurg(icult))
        'lecture QstressH pour correction température air en cas de stress hydrique
        QstressH = TurfacH(Joursim)
        If NRF(icult, Joursim) = 1 Or Not (ActivestressN) Then TurfacN = 1 Else TurfacN = NRF(icult, Joursim)
        Turfac(Joursim) = Min(TurfacH(Joursim), TurfacN)

        'est ce qu'il y a compétition pour la lumière entre les plantes de l'association :
        If Nbcult > 1 And LaiMC >= LAIseuilComp And cropsta(1, Joursim - 1) > 2 And cropsta(2, Joursim - 1) > 2 Then
            Competition(Joursim) = True
        Else
            Competition(Joursim) = False
        End If
        ' FA 15/11/13:à vérifier soigneusement en pas à pas...car routine executée pour chaque icult et là on affecte des valeurs pour les deux icults à chaque passage
        ' il aurait sans ndout mieux valu introduire une routine spécifique au niveau culture...
        'calcul des facteurs de réduction du LAI liés à la compétition avec autre plante de l'association
        'pour chaque plante de l'association en fonction du rapport de LAI entre les deux au jour précédent
        If (Competition(Joursim)) Then
            If LAI(1, Joursim - 1) > LAI(2, Joursim - 1) Then
                CompFac(2, Joursim) = Exp(-CoefExtin(1) * LAI(1, Joursim - 1))
                CompFac(1, Joursim) = 1
            End If
            If LAI(1, Joursim - 1) < LAI(2, Joursim - 1) Then
                CompFac(1, Joursim) = Exp(-CoefExtin(2) * LAI(2, Joursim - 1))
                CompFac(2, Joursim) = 1
            End If
        Else
            CompFac(icult, Joursim) = 1
        End If

        If Transplant(icult) Then
            If cropsta(icult, Joursim) = 3 Then
                Ulai = 1 + (Vlaimax - 1) * DVSt(icult, Joursim) / TDV(icult, 1)
                '(TDV(icult, Istade) - TDV(icult, Istade - 1))
                dLAI = DLAImax(icult) / (1 + Exp(5.5 * (Vlaimax - Ulai))) * Hu(icult)
                dLAI = dLAI * deltaidens(icult, Joursim, densplt(icult, Joursim))
                LAI(icult, Joursim) = dLAI + LAI(icult, Joursim - 1)

            End If
            ' les test et cas peuvent être plus concis ?
            ' améliorer généricité via utilisation TDV
            If cropsta(icult, Joursim - 1) = 3 And cropsta(icult, Joursim) = 4 Then

                If Currstge(icult, Joursim) = 1 Then Ulai = 1 + (Vlaimax - 1) * DVSt(icult, Joursim) / 0.4

                If Currstge(icult, Joursim) = 2 Then Ulai = Vlaimax + (3 - Vlaimax) * (DVSt(icult, Joursim) - 0.4) / 0.25

                dLAI = DLAImax(icult) / (1 + Exp(5.5 * (Vlaimax - Ulai))) * Hu(icult)
                dLAI = dLAI * deltaidens(icult, Joursim, densplt(icult, Joursim)) * Turfac(Joursim) * CompFac(icult, Joursim)
                ' ligne spécifique du jour de repiquage. Cas du démariage à réfléchir
                LAI(icult, Joursim) = dLAI + (LAI(icult, Joursim - 1) * densplt(icult, Joursim) / densplt(icult, Joursim - 1))
            End If
        End If
        If cropsta(icult, Joursim - 1) > 3 Or Not Transplant(icult) Then

            If Currstge(icult, Joursim) = 1 Then Ulai = 1 + (Vlaimax - 1) * DVSt(icult, Joursim) / 0.4

            If Currstge(icult, Joursim) = 2 Then Ulai = Vlaimax + (3 - Vlaimax) * (DVSt(icult, Joursim) - 0.4) / 0.25

            dLAI = DLAImax(icult) / (1 + Exp(5.5 * (Vlaimax - Ulai))) * Hu(icult)
            dLAI = dLAI * deltaidens(icult, Joursim, densplt(icult, Joursim)) * Turfac(Joursim) * CompFac(icult, Joursim)
            LAI(icult, Joursim) = dLAI + LAI(icult, Joursim - 1)

            If Currstge(icult, Joursim) = 3 Or Currstge(icult, Joursim) = 4 Then LAI(icult, Joursim) = LAI(icult, Joursim - 1)
            If Currstge(icult, Joursim) = 5 And Currstge(icult, Joursim - 1) = 4 Then
                JourSen(icult) = Joursim
                LAI(icult, JourSen(icult)) = LAI(icult, Joursim - 1)

            End If
            ' introduction de l'effet de turfac sur LAI apres stade 5
            If Currstge(icult, Joursim) = 5 Then
                'nouvelle formulation 21/03
                'dLAIsen moyen de la période de sénéscence des feuilles (négatif)
                ' attention  au cas où le LAI(JourSen) est > LAIrec...ne marche pas bien avec LAIRec <> 0 !!!!! et il faut Lairec <5 avec instruction ci-dessous
                If LAIrec(icult) > LAI(icult, JourSen(icult)) Then LAIrec(icult) = LAIrec(icult) * (LAI(icult, JourSen(icult)) / 5)
                dLAISen = (LAIrec(icult) - LAI(icult, JourSen(icult))) / (2 - DVSt(icult, JourSen(icult)))

                dLAISen = dLAISen * (DVSt(icult, Joursim) - DVSt(icult, Joursim - 1)) * (1 + SensiSen(icult) * (1 - Turfac(Joursim)))
                'si on veut pas d'effet du stress apres floraison: Sensisen=0
                ' effet maximal du stress apres floraison : sensisen=1: dLaisen est doublé, multiplication par deux de la vitesse de sénéscence
                ' Si Turfac vaut 1 (pas de stress), dLAI sen est inchangé
                ' A TESTER: prendre TurfacH seulement pour cet effet ?

                LAI(icult, Joursim) = LAI(icult, Joursim - 1) + dLAISen
                If LAI(icult, Joursim) < LAIrec(icult) Then LAI(icult, Joursim) = LAIrec(icult)
                ' ancienne formulation avant 21/03
                '        Lai(icult, Joursim) = ((LAIrec(icult) - Lai(icult, JourSen(icult))) / (2 - DVSt(icult, JourSen(icult)))) * (DVSt(icult, Joursim) - DVSt(icult, JourSen(icult))) + Lai(icult, JourSen(icult))
                '
                '

            End If

            If Currstge(icult, Joursim) = 6 Then LAI(icult, Joursim) = LAIrec(icult)
        End If

    End Sub
    Sub Calcule_LAI_SemiAride(Vlaimax As Double, joursim As Integer, icult As Integer, ContrainteW As Double, ActiveStressH As Boolean, ActivestressN As Boolean, LaiMC As Double, Nbcult As Double)
        'variante écrite à partire de la dernière modif CP 25/09/13
        ' pour introduire effet des stress sur la réduction du LAi dès après le stade LAI mw (apres Julpheno3)

        Dim Ulai As Double
        Dim dLAI As Double
        Dim dLAIpot As Double
        ' dlaipot et LAIpot(icult) ont été introduits mais ne sont pas variables explicatives ni envoyées vers table de sortie
        ' peut être pratique pour caler dlaimax sur un LAIpot -cible
        Dim dLAISen As Double
        Dim TurfacN As Double

        ' constante suivante à transformer en variable !
        Const LAIseuilComp As Double = 0.1 'lai seuil à partir duquel la compétition pour la lumière a lieu

        If ContrainteW > (1 - SeuilTurg(icult)) Or Not (ActiveStressH) Then TurfacH(joursim) = 1 Else TurfacH(joursim) = ContrainteW / (1 - SeuilTurg(icult))
        'lecture QstressH pour correction température air en cas de stress hydrique
        QstressH = TurfacH(joursim)
        If NRF(icult, joursim) = 1 Or Not (ActivestressN) Then TurfacN = 1 Else TurfacN = NRF(icult, joursim)
        Turfac(joursim) = Min(TurfacH(joursim), TurfacN)

        'est ce qu'il y a compétition pour la lumière entre les plantes de l'association :
        If Nbcult > 1 And LaiMC >= LAIseuilComp And cropsta(1, joursim - 1) > 2 And cropsta(2, joursim - 1) > 2 Then
            Competition(joursim) = True
        Else
            Competition(joursim) = False
        End If
        ' FA 15/11/13:à vérifier soigneusement en pas à pas...car routine executée pour chaque icult et là on affecte des valeurs pour les deux icults à chaque passage
        ' il aurait sans ndout mieux valu introduire une routine spécifique au niveau culture...
        'calcul des facteurs de réduction du LAI liés à la compétition avec autre plante de l'association
        'pour chaque plante de l'association en fonction du rapport de LAI entre les deux au jour précédent
        If (Competition(joursim)) Then
            If LAI(1, joursim - 1) > LAI(2, joursim - 1) Then
                CompFac(2, joursim) = Exp(-CoefExtin(1) * LAI(1, joursim - 1))
                CompFac(1, joursim) = 1
            End If
            If LAI(1, joursim - 1) < LAI(2, joursim - 1) Then
                CompFac(1, joursim) = Exp(-CoefExtin(2) * LAI(2, joursim - 1))
                CompFac(2, joursim) = 1
            End If
        Else
            CompFac(icult, joursim) = 1
        End If

        If Transplant(icult) Then
            If cropsta(icult, joursim) = 3 Then
                Ulai = 1 + (Vlaimax - 1) * DVSt(icult, joursim) / TDV(icult, 1)
                '(TDV(icult, Istade) - TDV(icult, Istade - 1))
                dLAI = DLAImax(icult) / (1 + Exp(5.5 * (Vlaimax - Ulai))) * Hu(icult)
                dLAI = dLAI * deltaidens(icult, joursim, densplt(icult, joursim))
                dLAIpot = dLAI
                LAIpot(icult) = LAIpot(icult) + dLAIpot
                LAI(icult, joursim) = dLAI + LAI(icult, joursim - 1)

            End If
            ' les test et cas peuvent être plus concis ?
            ' améliorer généricité via utilisation TDV
            If cropsta(icult, joursim - 1) = 3 And cropsta(icult, joursim) = 4 Then

                If Currstge(icult, joursim) = 1 Then Ulai = 1 + (Vlaimax - 1) * DVSt(icult, joursim) / 0.4

                If Currstge(icult, joursim) = 2 Then Ulai = Vlaimax + (3 - Vlaimax) * (DVSt(icult, joursim) - 0.4) / 0.25

                dLAI = DLAImax(icult) / (1 + Exp(5.5 * (Vlaimax - Ulai))) * Hu(icult)
                dLAIpot = dLAI * deltaidens(icult, joursim, densplt(icult, joursim))
                dLAI = dLAIpot * Turfac(joursim) * CompFac(icult, joursim)
                ' ligne spécifique du jour de repiquage. Cas du démariage à réfléchir
                LAI(icult, joursim) = dLAI + (LAI(icult, joursim - 1) * densplt(icult, joursim) / densplt(icult, joursim - 1))
                LAIpot(icult) = dLAIpot + (LAIpot(icult) * densplt(icult, joursim) / densplt(icult, joursim - 1))
            End If
        End If
        If cropsta(icult, joursim - 1) > 3 Or Not Transplant(icult) Then

            If Currstge(icult, joursim) <= 2 Then
                If Currstge(icult, joursim) = 1 Then Ulai = 1 + (Vlaimax - 1) * DVSt(icult, joursim) / 0.4

                If Currstge(icult, joursim) = 2 Then Ulai = Vlaimax + (3 - Vlaimax) * (DVSt(icult, joursim) - 0.4) / 0.25


                dLAI = DLAImax(icult) / (1 + Exp(5.5 * (Vlaimax - Ulai))) * Hu(icult)
                dLAIpot = dLAI * deltaidens(icult, joursim, densplt(icult, joursim))
                dLAI = dLAIpot * Turfac(joursim) * CompFac(icult, joursim)
                LAI(icult, joursim) = dLAI + LAI(icult, joursim - 1)
                LAIpot(icult) = LAIpot(icult) + dLAIpot
            Else
                If Currstge(icult, joursim) = 3 And Currstge(icult, joursim - 1) = 2 Then JourLaiMax(icult) = joursim - 1
                dLAISen = 0
                If Currstge(icult, joursim) = 5 Then
                    If Currstge(icult, joursim - 1) = 4 Then
                        JourSen(icult) = joursim
                        LAI(icult, JourSen(icult)) = LAI(icult, joursim - 1)
                    End If
                    If LAIrec(icult) > LAI(icult, joursim) Then LAIrec(icult) = LAI(icult, joursim)
                    dLAISen = (LAIrec(icult) - LAI(icult, JourSen(icult))) / (2 - DVSt(icult, JourSen(icult)))
                End If
                ' si sensisen=1 et turfac= 0 (stress maxi), la perte de LAI due au stress est égale au LAI du jour précédent
                ' rien n'interdit de fixer sensisen au dessus de 1
                ' si sensisen=0 pas de perte de LAI due au stress
                ' attention ici c'est turfac qui a été utilisé...peut être substituer par turfacH seulement ?
                dLAISen = dLAISen - SensiSen(icult) * (1 - Turfac(joursim)) * LAI(icult, joursim - 1)
                dLAISen = dLAISen * (DVSt(icult, joursim) - DVSt(icult, joursim - 1))
                LAI(icult, joursim) = LAI(icult, joursim - 1) + dLAISen
                If LAI(icult, joursim) <= 0 Then LAI(icult, joursim) = 0
                'If Currstge(icult, joursim) = 6 Then Lai(icult, joursim) = Max(LAIrec(icult), 0)
            End If

        End If


        ' ancienne formulation avant 21/03
        '        Lai(icult, Joursim) = ((LAIrec(icult) - Lai(icult, JourSen(icult))) / (2 - DVSt(icult, JourSen(icult)))) * (DVSt(icult, Joursim) - DVSt(icult, JourSen(icult))) + Lai(icult, JourSen(icult))
        '
        '



    End Sub
    Sub biomasse(dataClim As DataClimClass, ParSurRg As Double, icult As Integer, Joursim As Integer, ContrainteW As Double, ActivestressH As Boolean, ActivestressN As Boolean)

        'dernière modif 26/07/2011
        'dernière modif CP 25/09/13 introduction compétition pour la lumière, calcul du raint dépend du
        'rapport de LAI du jour précédent entre les deux espèces de l'association
        '(raint dimensionné en haut)
        ' **** Attention correction parenthèses calcul deltabiom le 16/12/11
        ' anciennes déclarations locales...
        'Dim raint As Double
        'Dim WSfactH As Double
        'Dim WSfactN As Double
        'Dim WSfact As Double
        Dim WSfactN As Double
        Dim Ftemp As Double

        If ContrainteW > (1 - SeuilWS(icult)) Or Not (ActivestressH) Then WSfactH(Joursim) = 1 Else WSfactH(Joursim) = ContrainteW / (1 - SeuilWS(icult))
        If NRF(icult, Joursim) = 1 Or Not (ActivestressN) Then WSfactN = 1 Else WSfactN = NRF(icult, Joursim)
        ' A améliorer on ne devrait pas utiliser WSfact ici mais un Sressfactor
        WSfact(Joursim) = Min(WSfactH(Joursim), WSfactN)

        If Transplant(icult) And cropsta(icult, Joursim) = 4 And cropsta(icult, Joursim - 1) = 3 Then
            raint(icult, Joursim) = 0.95 * ParSurRg * dataClim.dRg(Joursim) * (1 - Exp(-CoefExtin(icult) * (LAI(icult, Joursim) * densplt(icult, Joursim) / densplt(icult, Joursim - 1))))
        End If

        If Transplant(icult) And cropsta(icult, Joursim - 1) > 3 Or Not Transplant(icult) Then
            ' a vérifier : bé là ça va pas pour la compétition !!!
            raint(icult, Joursim) = 0.95 * ParSurRg * dataClim.dRg(Joursim) * (1 - Exp(-CoefExtin(icult) * LAI(icult, Joursim)))
        End If

        ' A verifier FA: beuh c koi ? CP a laissé ça en mode désactivé...alors qu'il faut bien en effet gérer la casacde du rayonnement à travers les deux espèces...
        ' mais bon j'aurais pas fait comme ça: de toutes façons: il faut choisir l'ordre de passage dans cette équation en fonction de l'ordre de dominance

        '
        'If Transplant(icult) And cropsta(icult, joursim - 1) > 3 Or Not Transplant(icult) Then
        '    If Competition(joursim) = True Then
        '        If LAI(1, joursim - 1) > LAI(2, joursim - 1) Then
        '            If icult = 1 Then raint(1, joursim) = 0.95 * ParSurRg * dataClim.dRg(joursim) * (1 - Exp(-CoefExtin(1) * LAI(1, joursim)))
        '            If icult = 2 Then raint(2, joursim) = 0.95 * ParSurRg * (dataClim.dRg(joursim) - raint(1, joursim)) * (1 - Exp(-CoefExtin(2) * LAI(2, joursim)))
        '       End If
        '       If LAI(1, joursim - 1) < LAI(2, joursim - 1) Then
        '           If icult = 2 Then raint(2, joursim) = 0.95 * ParSurRg * dataClim.dRg(joursim) * (1 - Exp(-CoefExtin(2) * LAI(2, joursim)))
        '           If icult = 1 Then raint(1, joursim) = 0.95 * ParSurRg * (dataClim.dRg(joursim) - raint(2, joursim)) * (1 - Exp(-CoefExtin(1) * LAI(1, joursim)))
        '       End If
        '   Else
        '   raint(icult, joursim) = 0.95 * ParSurRg * dataClim.dRg(joursim) * (1 - Exp(-CoefExtin(icult) * LAI(icult, joursim)))
        '   End If
        'End If

        If dataClim.dTmoy(Joursim) <= tcopt(icult) Then

            'deltaBiom(icult, joursim) = (Ebmax(icult) * raint * (1 - ((dataClim.dTmoy(joursim) - tcopt(icult)) / (tcmin(icult) - tcopt(icult)) ^ 2))) / 100
            Ftemp = 1 - ((dataClim.dTmoy(Joursim) - tcopt(icult)) / (tcmin(icult) - tcopt(icult))) ^ 2
        Else
            'deltaBiom(icult, joursim) = (Ebmax(icult) * raint * (1 - ((dataClim.dTmoy(joursim) - tcopt(icult)) / (tcmax(icult) - tcopt(icult)) ^ 2))) / 100
            Ftemp = 1 - ((dataClim.dTmoy(Joursim) - tcopt(icult)) / (tcmax(icult) - tcopt(icult))) ^ 2
        End If
        If Ftemp < 0 Then Ftemp = 0

        deltaBiom(icult, Joursim) = CO2fact(icult) * WSfact(Joursim) * (Ebmax(icult) * raint(icult, Joursim) - 0.0815 * raint(icult, Joursim) ^ 2) * Ftemp / 100

        Biom(icult, Joursim) = deltaBiom(icult, Joursim) + Biom(icult, Joursim - 1)

        Nuptake(icult, Joursim) = deltaBiom(icult, Joursim) / (NCvEmin(icult) + ((NCvEmax(icult) - NCvEmin(icult)) * alphaN(icult)))
        SigmaNuptake(icult, Joursim) = SigmaNuptake(icult, Joursim - 1) + Nuptake(icult, Joursim)

    End Sub
    Sub Rendement(icult As Integer, Joursim As Integer, julsim As Integer)
        Dim ng As Integer
        'modif FA 25/08/2011 controle division, par zero P1grain
        'calcul du nombre de grains lorsqu'on atteint le stade 4 (début remplissage)
        If JulPheno(icult, 4) = julsim Then
            JourDrp(icult) = Joursim
            For ng = JourDrp(icult) - Nbjgrain(icult) + 1 To JourDrp(icult)
                Vitmoy(icult) = Vitmoy(icult) + deltaBiom(icult, ng)
            Next ng
            Vitmoy(icult) = 100 * Vitmoy(icult) / Nbjgrain(icult)
            ' vitmoy en g/m2/jour)
            Ngrains(icult) = Int(Cgrain(icult) * Vitmoy(icult) + Cgrainv0(icult))

            If Ngrains(icult) < 0 Then Ngrains(icult) = 0

            ' modif FA pour limiter le nombre de grains par pied
            If (Ngrains(icult) / densplt(icult, Joursim) > Ngrmax(icult)) Then Ngrains(icult) = Ngrmax(icult) * densplt(icult, Joursim)
        End If
        ' entre stade 4 et stade fin évolution Indice de récolte (maturité): indice de recolte croissant
        ' attention  dans stics IR constant entre un stade fir et mat

        If Currstge(icult, Joursim) >= 4 And Currstge(icult, Joursim) <= 6 Then
            IR(icult) = Vitircarb(icult) * (Joursim - JourDrp(icult) + 1)
            IR(icult) = Min(IR(icult), IRmax(icult))
            Grain(icult, Joursim) = Min(Biom(icult, Joursim) * IR(icult), P1grainMax(icult) * Ngrains(icult) / 100)
            If Ngrains(icult) <> 0 Then
                P1grain(icult, Joursim) = 100 * Grain(icult, Joursim) / Ngrains(icult)
            Else
                P1grain(icult, Joursim) = -999.9
            End If
        End If
    End Sub
    Sub Croirac(icult As Integer, Joursim As Integer, ZoneHumSousRac As Double, ZracMC As Double)
        ' Todo: provisoire, prendre en compte stress repiquage sur descente racines ?
        Dim deltarac As Double
        If Currstge(icult, Joursim) < 4 Then
            ' si la plante n'est pas celle dont les racines sont les plus profondes,
            ' on ne tient pas compte du front d'humectation pour limiter le front racinaire
            ' introduire un test aussi sur Strac non nul ?

            If (Zrac(icult, Joursim - 1) < ZracMC) Then
                ZoneHumSousRac = ZoneHumSousRac + ZracMC - Zrac(icult, Joursim - 1)
            End If
            deltarac = Min(ZoneHumSousRac, DeltaRacMax(icult) * Hu(icult))
        End If
        Zrac(icult, Joursim) = Min(Zrac(icult, Joursim - 1) + deltarac, Zracmax(icult))

    End Sub
    'introduit le 24/04/13, fonction Stics, page 56 bouquin Nadine 2008
    Function FCO2(CO2c As Integer, icult As Integer) As Double
        FCO2 = 2 - Exp(Log(2 - alphaCO2(icult)) * (CO2c - 350) / (600 - 350))
    End Function
    Function deltaidens(icult As Integer, Joursim As Integer, densite As Double)
        deltaidens = densite
        If LAI(icult, Joursim - 1) > Laicomp(icult) Then
            If densite >= bdens(icult) Then
                deltaidens = deltaidens * (densite / bdens(icult)) ^ adens(icult)
            End If
        End If
    End Function
    Property ncropsta(icult As Integer, Joursim As Integer) As Integer
        Get
            Return cropsta(icult, Joursim)
        End Get
        Set(CSTA As Integer)
            cropsta(icult, Joursim) = CSTA
        End Set
    End Property
    '    Property Let ncropsta(icult As Integer, Joursim As Integer, CSTA As Integer)
    'cropsta(icult, Joursim) = CSTA
    'End Get
    '    Set()
    '    End Set
    '    End Property
    Property bDie(icult As Integer) As Boolean
        Get
            Return die(icult)
        End Get
        Set(FinCult As Boolean)
            die(icult) = FinCult
        End Set
    End Property
    'Property Let bDie(icult As Integer, FinCult As Boolean)
    'die(icult) = FinCult
    'End Get        Set()        End Set    End Property
    Property nJulPheno(icult As Integer, Istade As Integer) As Integer
        Get
            Return JulPheno(icult, Istade)
        End Get
        Set()
        End Set
    End Property
    Property dDVSt(icult As Integer, Joursim As Integer) As Double
        Get
            Return DVSt(icult, Joursim)
        End Get
        Set()
        End Set
    End Property
    Property nCurrstge(icult As Integer, Joursim As Integer) As Integer
        Get
            Return Currstge(icult, Joursim)
        End Get
        Set()
        End Set
    End Property
    Property sCultivar(icult As Integer) As String
        Get
            Return Cultivar(icult)
        End Get
        Set()
        End Set
    End Property
    Property dSommeT(icult As Integer, Joursim As Integer) As Double
        Get
            Return SommeT(icult, Joursim)
        End Get
        Set()
        End Set
    End Property
    Property dLAI(icult As Integer, Joursim As Integer) As Double
        Get
            Return LAI(icult, Joursim)
        End Get
        Set()
        End Set
    End Property
    Property dBiom(icult As Integer, Joursim As Integer) As Double
        Get
            Return Biom(icult, Joursim)
        End Get
        Set()
        End Set
    End Property
    Property nDeathDay(icult As Integer) As Integer
        Get
            Return Death_day(icult)
        End Get
        Set()
        End Set
    End Property
    Property nJourMat(icult As Integer) As Integer
        Get
            Return JourMat(icult)
        End Get
        Set()
        End Set
    End Property
    Property nJourSen(icult As Integer) As Integer
        Get
            Return JourSen(icult)
        End Get
        Set()
        End Set
    End Property
    Property dGrain(icult As Integer, Joursim As Integer) As Double
        Get
            Return Grain(icult, Joursim)
        End Get
        Set()
        End Set
    End Property
    Property dCoefExtin(icult As Integer) As Double
        Get
            Return CoefExtin(icult)
        End Get
        Set()
        End Set
    End Property
    Property dZrac(icult As Integer, Joursim As Integer) As Double
        Get
            Return Zrac(icult, Joursim)
        End Get
        Set()
        End Set
    End Property
    Property nZracmax(icult As Integer) As Integer
        Get
            Return Zracmax(icult)
        End Get
        Set()
        End Set
    End Property
    Property nNgrains(icult As Integer) As Long
        Get
            Return Ngrains(icult)
        End Get
        Set()
        End Set
    End Property
    Property dP1grain(icult As Integer, Joursim As Integer) As Double
        Get
            Return P1grain(icult, Joursim)
        End Get
        Set()
        End Set
    End Property
    Property dVitmoy(icult As Integer) As Double
        Get
            Return Vitmoy(icult)
        End Get
        Set()
        End Set
    End Property
    Property dKmax(icult As Integer) As Double
        Get
            Return Kmax(icult)
        End Get
        Set()
        End Set
    End Property
    Property nLevSim(icult As Integer) As Integer
        Get
            Return LevSim(icult)
        End Get
        Set()
        End Set
    End Property
    Property nZgraine(icult As Integer) As Integer
        Get
            Return Zgraine(icult)
        End Get
        Set()
        End Set
    End Property
    Property dNsymb(icult As Integer) As Double
        Get
            Return Nsymb(icult)
        End Get
        Set()
        End Set
    End Property
    Property dNavailCult(icult As Integer, Joursim As Integer) As Double
        Get
            Return NavailCult(icult, Joursim)
        End Get
        Set()
        End Set
    End Property
    Property dNUPTtarget(icult As Integer, Joursim As Integer) As Double
        Get
            Return NUPTtarget(icult, Joursim)
        End Get
        Set()
        End Set
    End Property
    Property dNRF(icult As Integer, Joursim As Integer) As Double
        Get
            Return NRF(icult, Joursim)
        End Get
        Set()
        End Set
    End Property
    Property dSigmaNuptake(icult As Integer, Joursim As Integer) As Double
        Get
            Return SigmaNuptake(icult, Joursim)
        End Get
        Set()
        End Set
    End Property
    Property dWSfactH(Joursim As Integer) As Double
        Get
            Return WSfactH(Joursim)
        End Get
        Set()
        End Set
    End Property
    Property dWSfact(Joursim As Integer) As Double
        Get
            Return WSfact(Joursim)
        End Get
        Set()
        End Set
    End Property
    Property dTurfacH(Joursim As Integer) As Double
        Get
            Return TurfacH(Joursim)
        End Get
        Set()
        End Set
    End Property
    Property dTurfac(Joursim As Integer) As Double
        Get
            Return Turfac(Joursim)
        End Get
        Set()
        End Set
    End Property
    Property dNuptake(icult As Integer, Joursim As Integer) As Double
        Get
            Return Nuptake(icult, Joursim)
        End Get
        Set()
        End Set
    End Property
    Property dCompFac(icult As Integer, Joursim As Integer) As Double
        Get
            Return CompFac(icult, Joursim)
        End Get
        Set()
        End Set
    End Property
    Property bCompetition(Joursim As Integer) As Boolean
        Get
            Return Competition(Joursim)
        End Get
        Set()
        End Set
    End Property
    Property draint(icult As Integer, Joursim As Integer) As Double
        Get
            Return raint(icult, Joursim)
        End Get
        Set()
        End Set
    End Property
    Property nJourLaiMax(icult As Integer) As Integer
        Get
            Return JourLaiMax(icult)
        End Get
        Set()
        End Set
    End Property

End Class
