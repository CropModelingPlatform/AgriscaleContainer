Option Compare Text
Option Explicit On
Imports System.Math
Imports System.Data
Imports System.Data.Sqlite
Public Class SolClass

    'VX: Variable explicative, VE: variable d'état, VS: variable simulée,PX: paramètre
    Dim IdSol As String
    Dim typsol As String
    Dim NbCouches As Integer
    Dim epc(0 To 5) As Integer 'VX, epaisseur de la couche de sol (cm) (sur 5 couches popssibles au maximum)
    Dim hcc(0 To 5) As Double 'VX, humidité pondérale à la capacité au champ pour la couche (%)
    Dim hmin(0 To 5) As Double 'VX, humidité pondérale au point de flétrissement permanent pour la couche (%)
    Dim da(0 To 5) As Double 'VX, densité apparente (sd)
    Dim Ztotsol As Integer 'VX, profondeur totale de sol considérée (somme des epc)
    Dim Eos As Double 'VE, evaporation potentielle du sol (sous mulch et cultures)
    Dim Esol(731) As Double ' VE, evaporation sol du jour(mm)
    Dim Zsurf As Integer 'VX, épaisseur de l'horizon concerné par l'évaporation (cm)
    Dim Stsurf(731) As Double 'VE, stock hydrique disponible de la couche de sol concernée par l'évaporation (mm)
    Dim Strac(731) As Double 'VE, stock hydrique disponible dans la couche de sol colonisée par les racines (mm)
    Dim Stnonrac(731) As Double 'VE, stock hydrique disponible dans la couche de sol non encore colonisée par les racines (entre Plante.Zrac et Plante.Zracmax)(mm)
    Dim Stprofond(731) As Double 'VE, stock hydrique disponible dans la couche de sol non colonisable par les racines (entre Plante.Zracmax et Ztotsol)(mm)
    Dim Stocksol(731) As Double 'VE, Stock hydrique total disponible de 0 à Ztotsol (mm), = Strac+Stnonrac+Stprofond
    Dim Drprofmax(731) As Double 'VS, Drainage sous la cote Ztotsol (mm)
    Dim Dr(731) As Double 'VE, drainage sous la cote maxi colonisable par les racines (Plante.Zracmax) (mm)
    Dim TAW As Double 'VX, ** attention: version par cm de sol de "Total available water in the root zone, mm, as in FAO bull #56 p162" donc ici mm/cm
    ' TAW est la moyenne sur le profil donc si plusieurs couches dans soil_layers elles seront traitées comme une seule de caractéristique
    ' moyenne des couches. TAW est la capacité de stockage moyenne du sol par cm
    Dim TEW As Double 'VX, total Evaporable water, as in FAO bull #56 p144 (mm)
    Dim ZoneHumSousRacines As Double 'VE, comme son nom l'indique, mm
    Dim ContrainteW As Double 'VE, Strac(joursim) / (TAW * Zrac), sd (mm dispo /mm de capacité)
    Dim ContrainteWSurf As Double 'VE, Stsurf(joursim) / TEW, sd (mm dispo sur mm de capacité)
    Dim Zmes As Integer 'VS, profondeur de comparaison des stocks simulés avec des mesures (= profondeur maxi de mesure, par exemple), doit etre < Ztotsol et > Zsurf(cm)
    Dim StockMes(731) As Double 'VS, Stock hydrique total disponible de 0 à Zmes (mm)
    Dim Stger(731) As Double 'VE, Stock hydrique total disponible de 0 à Zger (mm)

    Dim SigmaSimEsol As Double 'VS, somme sur la simulation de Esol (mm)
    Dim SigmaCultEsol As Double 'VS, somme entre semis première culture et récolte dernière culture de Esol (mm)
    Dim SigmaSimDrprofmax As Double 'VS, somme sur la simulation de Drprofmax (mm)
    Dim SigmaCultDrprofmax As Double 'VS, somme entre semis première culture et récolte dernière culture de Drprofmax (mm)
    Dim SigmaSimDr As Double 'VS, somme sur la simulation de Dr (mm)
    Dim SigmaCultDr As Double 'VS, somme entre semis première culture et récolte dernière culture de Dr (mm)
    Dim Zger As Integer 'VX, épaisseur de l'horizon concerné par la germination (cm), transmis au sol à partir d'une lecture faite dans plante

    Dim dataSolAdp As SqliteDataAdapter
    Dim rstSolData As DataSet 'les tables de variables sol lues
    Dim SeuilEvap As Double 'VX, seuil de contrainteWsurf limitant évaporation
    Dim ZObstacleRac As Integer 'VX profondeur (cm) d'un éventuel obstacle à la croissance racinaire
    ' intérêt de passer variable suivante en vecteur du temps ???
    Dim ContrainteHlevee As Boolean 'VE contrainte hydrique pour la germination et la levee
    Dim StockN As Double 'Vx stock du sol en azote (kg/ha/an)
    Dim StockP As Double 'Vx stock du sol en phosphore (kg/ha/an)
    Dim StockK As Double 'Vx stock du sol en potassium (kg/ha/an)
    ' a vérifier intérêt de passer variable suivante en vecteur du temps ?
    Dim SignalFletrissement(0 To 731) As Integer  'VE jours consécutifs accumulés pendant lesquels le sol est au point de flétrissement
    Dim TypeRui As Integer
    Dim TypeSurf As New TypeSurfClass
    ' ajouts CP
    'Dim Drainage(0 To 731) As Boolean 'drainage joursim O/N
    Dim VolEausol As Double  'volume d'eau dans Volsol en mètres cubes = Volsol * RUsol (avec RUsol en m/m)
    Dim ConcNsol(0 To 731) As Double 'concentration en azote de l'eau du sol kg N/mètres cubes d'eau
    Dim DrainageON(0 To 731) As Boolean  'si vrai il y a eu drainage sous Zracmax le joursim
    Dim perteNDrain(0 To 731) As Double  'pertes d'azote par drainage
    ' FA a vérifier: je désactive l'utilisation de StockMesAj...
    Dim Navail(0 To 731) As Double    'stock d'azote disponible en kg/ha
    'Dim StockMesAj(731) As Double 'VS, StockMes ajusté (auquel on a retranché StockBase), lecture StockBase pas encore fonctionnelle 15/10/2013
    ' modif 1/04/2011 correction bug calcul de stock tot
    'modif 08/04/2011 description variables, lecture Zsurf table sol
    'modif 10/05/11 corrections erreurs bouclage bilan
    ' modif 30/11/11 correction bug initialisation stock sol quand stockinit > stock max ds zone rac
    ' modif 17/04/12 ajouts ZObstacleRac
    ' modif 04/05/12 introduction contrainteHlevee
    ' dernière modif 11/05/12 introduction réservoir pourla levee
    ' 16/04/13 introduction SignalFletrissement calculé dans dans eau sol
    ' Dans plante, mort de la culture si SignalFlétrissement excède un seuil
    'prévoir ressemis automatique comme une option du modele si mort avant un certain stade.
    '27/04/13 lecture typeRui dans table "soil" et lecture des parametres de ruissellement (hors effet paillis) dans table "TypeSurfSol"
    'modif CP 30/05/13 Zger n'est plus lu dans la table sol il est lu dans la table Plantspecies et est appellé depuis PlanteClasse et renommé Zger dans SolClass + calcul de Zgermax
    ' modif en cours suppression ilev dans calcul du signal de fletrissement
    'modif CP 06/09/13 introduction dans Eausol du calcul de nbjContHlev, le nb de jour pdt lequel la levée est bloquée par contrainte hydrique
    'modif CP 17/09/13 introduction Drainage Vrai/faux pour savoir les jours où il y a drainage
    'modif CP 17/09/13 module de calcul du stock initial d'azote dans le sol <== supprimé le 04/10/13
    '(sol + apports fertilisants - pertes gazeuses vers atmosphère)
    'modif CP 17/09/13 module de calcul de la concentration en azote de l'eau du sol
    'modif CP 01/10/13 SignalFletrissement dimmensionné (0 to 731)
    'modif CP 04/10/13 suppression Sub StockNini + modif sub concNeauSol
    'dernière modif CP 15/10/2013 introduction dans Eausol du calcul de StockMesAj
    '(StockMes auquel on retranche StockBase) mais pas fontionnel pour l'instant
    ' relu FA 15/11 reste des choses à vérifier:
    ' le passage de certaines variables en vecteur du temps
    'l'histoire de Stock mes ajusté (j'ai commencé à nettoyer)
    ' un critère sur Zrac pour le calcul du flétrissement
    'modif FA 25/04/2014 desactivation lecture Typsol, essai de correction initialisation TEW
    ' ne fonctionne pas si sol multicouche
    Dim txminN As Double 'taux de minéralisation moyen du sol par saison en % du stock d'azote présent - pour bilan saisonnier seulement !
    Dim Tsoil As Double ' VE, temperature du sol dans l'horizon de minéralisation de la MOS (pour l'instant identique à température de l'air). Bilan N Journalier seulement
    Dim Nmin(0 To 731) As Double 'VE, quantité de N du sol minéralisée chaque jour (Kg/ha)
    Dim CNmin(0 To 731) As Double  ' VE, Cumul de la quantité de N minéralisé jusqu'au jours de la simulation (Kg/ha).
    Dim MNORG As Double ' VE azote minéralisable de la matière organique du sol pour bilan N journalier
    Dim Fmin As Double 'Vx Fraction minéralisable de la matière organique du sol pour bilan N journalier = 1-Finert = 0.35 par défaut
    Dim NDenit(0 To 731) As Double  'VE, Quantité d'azote perdu par dénitrification chaque jour à partir du stock de N disponible (Navail)(Kg/ha)
    Dim CNDenit(0 To 731) As Double  'VE, Cumul de la quantité d'azote perdu par dénitrification jusqu'au jour de la simulation (Kg/ha).
    Dim MinNorgInput(0 To 731) As Double  'VE, Taux de Minéralisation journalière de l'azote de la MO apporté(Kg/ha)
    Dim Cres(0 To 731) As Double 'C des résidus organiques apportés
    Dim Cbio(0 To 731) As Double 'C de la biomasse microbienne
    Dim Nbio(0 To 731) As Double 'N de la biomasse microbienne
    Dim Nhum(0 To 731) As Double 'N de l'humus
    Dim DNMinOrg(0 To 731) As Double 'N de la matière organique apportée (résidus, fumiers)
    Dim CumDNMinOrg(0 To 731) As Double ' cumul du N de la matière organique apportée (résidus, fumiers)
    'VERSION 3 du 8-10 nov 2017
    '
    'modif FA 27/10/2017 calcule stock N à, partir Nstock sol de la table soil en % (da doit être correctement renseigné !)
    ' suite modif 27/10/2017: introduction TxminN lu dans table soil (taux minéralisation moyen saisonnier)
    'modifs en cours: 17/11/2017: introduction de la minéralisation de l'N, introduction de Tsoil,
    ' modif FA du 7/09/18: cumul evap sur culture

    Sub LisSol(DataBase_Cnn As SqliteConnection, SimUnit As SimulationUnitClass, Zgraine As Integer, DriveRui As Boolean)
        Dim Trouve As Boolean
        Dim Icouche As Integer
        Dim msg As String
        Dim reste As Integer
        'rstSolData = New ADODB.Recordset

        Trouve = False

        rstSolData = New DataSet
        dataSolAdp = New SqliteDataAdapter("SELECT * FROM Soil where IdSoil='" & SimUnit.sIdSoil & "'", DataBase_Cnn)
        dataSolAdp.Fill(rstSolData)
        If rstSolData.Tables(0).Rows.Count > 0 Then
            'Trouve = True
            ' typsol = rstSolData!typsol
            NbCouches = rstSolData.Tables(0).Rows(0)("NbCouches")
            Zsurf = rstSolData.Tables(0).Rows(0)("Zsurf")
            Zmes = rstSolData.Tables(0).Rows(0)("Zmes")
            SeuilEvap = rstSolData.Tables(0).Rows(0)("SeuilEvap")
            ZObstacleRac = rstSolData.Tables(0).Rows(0)("ZObstacleRac")
            '  Zger = rstSolData!Zger
            ' zger maintenant lu dans plante (Zgraine) et transmis au sol
            StockN = rstSolData.Tables(0).Rows(0)("StockN")
            ' StockP = rstSolData!StockP
            ' StockK = rstSolData!StockK
            TypeRui = rstSolData.Tables(0).Rows(0)("TypeRui")
            txminN = rstSolData.Tables(0).Rows(0)("txminN")
            Fmin = rstSolData.Tables(0).Rows(0)("Fmin")

        End If
        'rstSolData.MoveNext()
        'End While
        ' rstSolData.Close()
        'verifier que ligne ci-dessus ajoutée 19/04 fout pas le Souk...
        Zger = Zgraine
        'affectation de Zgraine lu dans Plante à Zger
        ' lecture données par horizons
        rstSolData.Clear()
        dataSolAdp = New SqliteDataAdapter("SELECT * FROM Soil_layers where IdSoil='" & SimUnit.sIdSoil & "' Order by NumCouche", DataBase_Cnn)
        dataSolAdp.Fill(rstSolData)
        For Icouche = 1 To NbCouches
            If Icouche < rstSolData.Tables(0).Rows.Count + 1 Then
                epc(Icouche) = rstSolData.Tables(0).Rows(Icouche - 1)("epc")
                hcc(Icouche) = rstSolData.Tables(0).Rows(Icouche - 1)("hcc")
                hmin(Icouche) = rstSolData.Tables(0).Rows(Icouche - 1)("hmin")
                da(Icouche) = rstSolData.Tables(0).Rows(Icouche - 1)("da")
                ' todo: verif si OK le coef 10
                TAW = TAW + (hcc(Icouche) - hmin(Icouche)) * da(Icouche) * epc(Icouche) / 10
                Ztotsol = Ztotsol + epc(Icouche)
                ' attention modif avril 2014 TP Lascar incomplete si plusieurs couches
                'If Icouche = 1 And Zsurf < epc(1) Then
                'TEW = (hcc(1) - hmin(1)) * da(Icouche) * Zsurf / 10
                If Icouche = 1 Then
                    TEW = (hcc(1) - hmin(1)) * da(Icouche) * Zsurf / 10
                    StockN = 10000.0# * StockN * da(1) * 3  ' modif du 27/10/17 N en kg/ha de N à partir de la teneur en %, en considérant
                    '30cm de sol ds lequel le N orga se minéralise (N/100)x0.3x10000xdax1000=3Nda x10e6/100=3Ndax10000
                Else
                    '         If Zsurf >= Ztotsol Then
                    ' attention modif ci-dessous effectuée lors du TP Celsius 2014 avec sol où tot=20cm (attention si plusieurs couches)
                    If Zsurf >= epc(1) And Zsurf > Ztotsol Then
                        reste = Zsurf - Ztotsol
                        TEW = TEW + (hcc(Icouche) - hmin(Icouche)) * da(Icouche) * Min(epc(Icouche), reste) / 10
                    End If
                End If
                'Ancienne version avant TP avril 2014
                '        If Icouche = 1 And Zsurf < epc(1) Then
                '        TEW = (hcc(1) - hmin(1)) * da(Icouche) * Zsurf / 10
                '        Else
                '            If Zsurf >= Ztotsol Then
                '                reste = Zsurf - Ztotsol
                '                TEW = TEW + (hcc(Icouche) - hmin(Icouche)) * da(Icouche) * Min(epc(Icouche), reste) / 10
                '            End If
                '        End If
                '    rstSolData.MoveNext     
            Else
                msg = "pb de cohérence nbre de couches tables Soil_layers et Soil"
                Console.WriteLine(msg)
            End If
        Next Icouche
        'TAW en mm/cm (TEW en mm pour la zone 0-Zsurf)
        TAW = TAW / Ztotsol
        Nhum(0) = Fmin * StockN ' initialisation de l'azote minéral du sol à zero pour bilan journalier de N, impose de démarrer avec un sol sec...?
        StockN = StockN * txminN / 100 ' dans le cas d'un bilan saisonnier une fraction de l'azote organique total est minéralisée par saison

        If TEW = 0 Then
            msg = "pb de calcul de TEW; TEW =0"
            Console.WriteLine(msg)
        End If
        If TAW = 0 Then
            msg = "pb de calcul de TAW; TAW =0"
            Console.WriteLine(msg)
        End If
        If Zmes > Ztotsol Or Zmes < Zsurf Then
            msg = "Attention Zmes > Ztotsol Ou Zmes < Zsurf"
            Console.WriteLine(msg)
        End If


        '
        'fermeture table et libération mémoire de l'objet

        '        rstSolData.Close()
        '        rstSolData = Nothing
        Call TypeSurf.readSurf(DataBase_Cnn, TypeRui)

    End Sub
    Sub initsol(Zracmax As Integer, StockIni As Double, IniSolhautON As Boolean)
        Dim msg As String
        ContrainteHlevee = True
        StockMes(0) = Min(StockIni, TAW * Zmes)
        If StockIni >= TAW * Ztotsol Then
            msg = "attention vous avez mis un stock initial >= capacité de stockage du sol, l'excès ne sera pas compté en drainage"
            Console.WriteLine(msg)
            Stprofond(0) = TAW * (Ztotsol - Zracmax)
            Stnonrac(0) = TAW * Zracmax
            Stsurf(0) = TEW
            Stger(0) = TAW * Zger
        Else
            If IniSolhautON Then

                Stsurf(0) = Min(StockIni, TEW)
                Stger(0) = Min(StockIni, TAW * Zger)
                If StockIni > TAW * Zracmax Then
                    Stnonrac(0) = TAW * Zracmax
                    Stprofond(0) = StockIni - TAW * Zracmax
                Else
                    Stnonrac(0) = StockIni
                End If

            Else
                If StockIni > TAW * (Ztotsol - Zracmax) Then
                    Stprofond(0) = TAW * (Ztotsol - Zracmax)
                    Stnonrac(0) = StockIni - Stprofond(0)
                    If Stnonrac(0) > TAW * Zracmax - TEW Then
                        Stsurf(0) = Stnonrac(0) - (TAW * Zracmax - TEW)
                    End If
                    If Stnonrac(0) > TAW * (Zracmax - Zger) Then
                        Stger(0) = Stnonrac(0) - (TAW * (Zracmax - Zger))
                    End If
                Else
                    Stprofond(0) = StockIni
                End If
            End If
        End If

        Stocksol(0) = Stprofond(0) + Stnonrac(0)
        Navail(0) = 0

    End Sub
    Sub evaporation(Joursim As Integer, EoSM As Double, Eomulch As Double, ContrainteWSurf As Double, pousse As Boolean)

        Eos = EoSM - Eomulch

        If ContrainteWSurf < SeuilEvap Then
            Esol(Joursim) = Eos * ContrainteWSurf / SeuilEvap
        Else
            Esol(Joursim) = Eos
        End If
        ' Attention correction de Esol en fonction du niveau d'eau du jour précédent ici dans version 3 et non plus dans Eausol apres calcul de StSurf
        Esol(Joursim) = Min(Stsurf(Joursim - 1), Esol(Joursim))
        SigmaSimEsol = SigmaSimEsol + Esol(Joursim)
        If pousse Then
            SigmaCultEsol = SigmaCultEsol + Esol(Joursim)
        End If
        ' Calcul de SigmasimEsol est à faire dans Eausol si on y déplace la correction de Esol en fonction du stok de surface

    End Sub
    Sub TempSoil(T As Double)
        Tsoil = T

    End Sub
    Sub MinNorgSS(joursim As Integer)
        'modele de mineralisation du N emprunté à Soltani et Sinclair, modelling physiology of crop development, growth and yield, CABI, 2012
        Dim Tfact As Double
        Dim KnMin As Double
        Dim Wfact As Double

        If Tsoil > 35 Then Tsoil = 35
        KnMin = 24 * Exp(17.753 - 6350.5 / (Tsoil + 273))
        Tfact = 1 - Exp(-KnMin / 168)

        If ContrainteWSurf < 0.9 Then
            Wfact = 1.111 * ContrainteWSurf
        Else
            Wfact = 10 - 10 * ContrainteWSurf
        End If
        If Wfact < 0 Then Wfact = 0
        Nmin(joursim) = Nhum(joursim - 1) * Tfact * Wfact
        Nmin(joursim) = ((Nmin(joursim) * (0.0002 - ConcNsol(joursim)) / 0.0002)) * 10

        If Nmin(joursim) < 0 Then Nmin(joursim) = 0
        Nhum(joursim) = Nhum(joursim - 1) - Nmin(joursim)

        If joursim = 0 Then
            CNmin(joursim) = Nmin(joursim)
        Else
            CNmin(joursim) = (CNmin(joursim - 1) + Nmin(joursim))
        End If
        'If CNmin(Joursim) Then MsgBox ("MinNorg:Ok")
    End Sub
    Sub DenitNavail(joursim As Integer)
        'modele de dénitrification du N du sol emprunté à Soltani et Sinclair, modelling phsiollogy of crop development, growth and yield, CABI, 2012
        Dim XConcNsol As Double
        Dim KDenit As Double

        NDenit(joursim) = 0
        If ContrainteWSurf = 1 Then XConcNsol = ConcNsol(joursim)
        If XConcNsol > 0.0004 Then XConcNsol = 0.0004
        KDenit = 6 * Exp(0.07735 * Tsoil - 6.593)
        NDenit(joursim) = XConcNsol * (1 - Exp(-KDenit))
        NDenit(joursim) = (NDenit(joursim) * Stsurf(joursim) * 1000) * 10


        If joursim = 0 Then
            CNDenit(joursim) = NDenit(joursim)
        Else
            CNDenit(joursim) = CNDenit(joursim - 1) + NDenit(joursim)
        End If
    End Sub
    'Sub MinMOSStics()
    '    Dim Fmin1 As Double 'constante de minéralisation de la MOS du sol
    '    Dim Fmin2 As Double 'parametre d'effet de la teneur en argile sur la minéralisation de la MOS
    '    Dim Fmin3 As Double 'parametre d'effet de la teneur en calcair sur la minéralisation de la MOS
    '    Dim FTH As Double ' fonction de la température du sol
    '    Dim FH As Double ' fonction de l'humidité du sol
    '    Dim K2hum As Double 'taux de minéralisation potentiel
    '    'VminH, Nhum(j) à déclarer en global
    '    'argi et calc = global ? ou K2hum à calculer lors de l'initialisation du sol ?

    '    Fmin1 = 0.0006
    '    Fmin2 = 0.0272
    '    Fmin3 = 0.0167

    '    FH = 0.2 + 0.8 * ContrainteWSurf 'attention version stics3
    '    FTH = 25 / (1 + 145 * Exp(-0.12 * Tsoil))
    '    K2hum = Fmin1 * Exp(-Fmin2 * Argi) / (1 + Fmin3 * Calc)
    '    VminH = Nhum(joursim - 1) * K2hum * FH * FTH
    '    Nhum(joursim) = Nhum(joursim - 1) - VminH
    '    Nmin(joursim) = VminH
    '    If joursim = 0 Then
    '        CNmin(joursim) = Nmin(joursim)
    '    Else
    '        CNmin(joursim) = (CNmin(joursim - 1) + Nmin(joursim))
    '    End If

    'End Sub
    Sub MinNorgapporteStics(joursim As Integer, Qorga As Double, CsurNRes As Double, CsurNhum As Double, ApportOrga As ApportsOrgaClass)
        'Modèle de minéralisation du N de la MO apportée emprunté à STICS (livre rouge 2012)
        ' attention pas au point: procedure jamais appellée, erreurs de calcul à vérifier
        ' Lectures des parametres des matières organbique pas fini du tout, à revoir...
        Dim Kres As Double
        Dim Hres As Double

        Dim FTR As Double ' fonction de la température du sol
        Dim FH As Double ' fonction de l'humidité du sol
        Dim FN As Double ' Fonction de l'azote au voisinage des résidus (désactivée à ce jour)
        Dim FBio As Double
        Dim CNBio As Double

        Dim DCres As Double 'décomposition journalière du C des résidus
        Dim DNres As Double ' variation journalière du N des résidus
        Dim DCbio As Double 'décomposition journalière du C de la biomasse microbienne
        Dim DNbio As Double 'décomposition journalière du N de la biomasse microbienne
        Dim DChum As Double 'variation journalière du C de l'humus avant minéralisation de ce dernier
        Dim DNhum As Double 'variation journalière du N de l'humus avant minéralisation de ce dernier

        FN = 1
        Fbio = 1
        FTR = 12 / (1 + 51.6 * Exp(-0.103 * Tsoil))
        FH = 0.2 + 0.8 * ContrainteWSurf 'attention ici c'est parti de la version Stics3 1998  !
        If Qorga > 0 Then
            If Cres(joursim - 1) > 0 Then Qorga = ((Cres(joursim - 1) / CsurNRes) / ApportOrga.dNrec) + Qorga
            Cres(joursim) = Qorga * ApportOrga.dNrec * CsurNRes
        End If
        Kres = ApportOrga.dAkres + ApportOrga.dBkres / CsurNRes
        Hres = 1 - ApportOrga.dAHres + CsurNRes / (ApportOrga.dBHres + CsurNRes)
        CNBio = ApportOrga.dAWB + ApportOrga.dBWB / CsurNRes
        DCres = -Kres * Cres(joursim) * FTR * FH * FN
        DNres = DCres / CsurNRes
        DCbio = -ApportOrga.dYres * DCres - ApportOrga.dKbio * Cbio(joursim - 1) * FTR * FH
        DNbio = -(ApportOrga.dYres * DCres / (CNBio * ApportOrga.dFbio)) - ApportOrga.dKbio * Nbio(joursim - 1) * FTR * FH
        DChum = ApportOrga.dKbio * Hres * FTR * FH
        ' Comment on obtient le CsurNhum ????
        DNhum = DChum / CsurNhum
        Cres(joursim) = Cres(joursim) + DCbio
        Cbio(joursim) = Cbio(joursim - 1) + DCbio
        Nbio(joursim) = Nbio(joursim - 1) + DNbio
        DNMinOrg(joursim) = -DNres - DNhum - DNbio
        If joursim = 0 Then
            CumDNMinOrg(joursim) = DNMinOrg(joursim)
        Else
            CumDNMinOrg(joursim) = (CumDNMinOrg(joursim - 1) + DNMinOrg(joursim))
        End If

        Nhum(joursim) = Nhum(joursim) + DNhum ' Nhum(joursim) a déjà été mis à jour 1 premiere fois avec la minéralisation de la MOS
    End Sub
    Sub StockNavail(joursim As Integer, AppMinNjour As Double, NuptMC As Double, Nprecip As Double)
        Const pcentNgaz As Double = 0.01 '% du stock d'azote perdu sous forme de gaz par jour
        'Selon Soltani et Sinclair (2012) les Pertes d'N lors de l'application de l'engrais dépendent de la nature de l'engrais apporté, du type d'application (en surface ou incorporé) et de l'environnement (humide, subhumide ou sèche). Par exemple, dans les régions sèches, si l'engrais azoté est appliqué en surface sous forme d'urée juste avant un événement pluvieux, on peut utiliser une pourcentage de perte de 0,25 (25%) .


        'actualisation journalière du stock d'azote disponible
        ' attention pas au point: la minéralisation des apports de MO n'est pas faite (MinNorgINput toujours à 0) et ne doit pas etre appliquée seulement le jour des apports
        ' mais s'appliquer un stock de MO qui est augmenté par les apports et se décompose quotidiennement

        Navail(joursim) = Navail(joursim - 1) + Nmin(joursim) + (AppMinNjour) - (AppMinNjour * 0.35) + DNMinOrg(joursim) - perteNDrain(joursim) - NuptMC - (Navail(joursim - 1) * pcentNgaz) - NDenit(joursim)

        If Navail(joursim) < 0 Then Navail(joursim) = 0
        If Navail(joursim) > 0 Then
            Navail(joursim) = Navail(joursim)
        End If
    End Sub
    'Sub StockNavail(Joursim, AppMinNjour, AppOrgNjour, NuptMC)
    '    Const pcentNgaz As Double = 0.01 '% du stock d'azote perdu sous forme de gaz par jour
    '    ' a vérifier attention tout l'azote orga est vu comme dispo !!
    '    'actualisation journalière du stock d'azote disponible
    '    Navail(Joursim) = Navail(Joursim - 1) + AppMinNjour + AppOrgNjour - perteNDrain(Joursim) - NuptMC - (Navail(Joursim - 1) * pcentNgaz)

    '    If Navail(Joursim) < 0 Then Navail(Joursim) = 0

    'End Sub
    Sub ConcNEauSol(Joursim As Integer, ZracmaxMC As Double)
        'calcul de la concentration en azote de l'eau du sol
        'utilisé pour le calcul des pertes d'azote par les eaux de drainage sous ZracmaxMC

        'calcul du volume d'eau du sol quand il est rempli jusqu'à ZracmaxMC (drainage au-delà de ZracmaxMC)
        VolEausol = ZracmaxMC * TAW

        'concentration en azote de l'eau du sol
        ConcNsol(Joursim) = Navail(Joursim) / VolEausol

    End Sub
    Sub EauSol(Joursim As Integer, precip As Double, transpi As Double, Zrac As Double, Deltazrac As Double, Zracmax As Integer)

        ' attention dans la forme actuelle la description du sol en couches n'est pas prise en compte
        ' dans le calcul: TAW est considérée comme uniformément répartie en
        ' dans le sol en fonction de la profondeur

        Dim bil As Double
        Dim E_Srac As Double
        Dim finrac As Boolean
        'Dim reportfinrac As Double


        bil = Stsurf(Joursim - 1) + precip
        Stsurf(Joursim) = Min(bil, TEW)
        'dans V2 on empêchait ici l'évapopration d'excéder le stock dispo (utile ds cas où seuilevap bas..), mais replacé dans sub evaporation
        ' en fonction stock du jour précédent pour cohérence avec forçage Esol par co,ntrainte du jour précédent et cohérence du calcul du cumul d'éavaporation
        ' Esol(joursim) = Min(Stsurf(joursim), Esol(joursim))
        Stsurf(Joursim) = Stsurf(Joursim) - Esol(Joursim)
        If Zrac > 0 Then Stsurf(Joursim) = Stsurf(Joursim) - Min(Zsurf / Zrac, 1) * transpi
        Stsurf(Joursim) = Max(0, Stsurf(Joursim))
        'attention dans les cas où transpi +evap excède le stock, celui ci est capé à zero mais  transpi
        'n'est pas corrigée

        ContrainteWSurf = Stsurf(Joursim) / TEW
        ' introduction du calcul de Stger attention comme on eleve l'evaporation en même temps qu'on remplit, Stger est toujoutrs tres inférieur à Zger*TAW
        If Zrac = 0 Then
            If Zger < Zsurf Then
                Stger(Joursim) = Min(Stger(Joursim - 1) + precip, TAW * Zger) - Esol(Joursim) * Zger / Zsurf
            Else
                Stger(Joursim) = Min(Stger(Joursim - 1) + precip, TAW * Zger) - Esol(Joursim)
            End If
            Stger(Joursim) = Max(0, Stger(Joursim))
        End If
        'contrainte hydrique pour la levée
        If Stger(Joursim) >= 0.14 * TAW * Zger Then ContrainteHlevee = False
        If Stger(Joursim) <= 0.1 * TAW * Zger Then ContrainteHlevee = True


        ' introduction du calcul de StockMes
        If Zrac < Zmes Then
            StockMes(Joursim) = StockMes(Joursim - 1) + precip - Esol(Joursim) - transpi
        Else

            StockMes(Joursim) = StockMes(Joursim - 1) + precip - Esol(Joursim) - transpi * Zmes / Zrac
            If StockMes(Joursim) < 0 Then StockMes(Joursim) = 0

            'option plus raisonnable ?
            'StockMes(joursim) = -999 ' en effet dans ce sol non discrétisé en petites couches
            ' il ne parait pas possible de répartir la transpiration entre deux "tranches" de sol où les racines sont présentes
        End If
        'If StockMes(joursim) < 0 Then StockMes(joursim) = 0
        If StockMes(Joursim) > TAW * Zmes Then StockMes(Joursim) = TAW * Zmes

        'actualisation du stock dans la zone colonisée par les recines (Stockrac)
        ' traitement du jour de fin de présence d'une plante transpirant par finrac et reportfinrac
        If Zrac = 0 And Deltazrac < 0 Then
            Deltazrac = 0
            finrac = True
            '    reportfinrac = Strac(joursim - 1)
        Else
            finrac = False
            '    reportfinrac = 0
        End If
        bil = precip + Strac(Joursim - 1) + (Deltazrac * TAW)
        ' Todo: attention on exagère l'acces à l'eau par les recines en croissance...(Deltazrac*TAW)
        ' sera résolu en discrétisant le sol en couches élémentaires...ou en calculant un fronthum

        If bil > Zrac * TAW Then
            Strac(Joursim) = Zrac * TAW
            precip = bil - (Zrac * TAW)
        Else
            Strac(Joursim) = bil
            If Not finrac Then precip = 0 Else precip = precip + Strac(Joursim - 1)
            'si finrac, precip reste inchangé (sera affecté à Stnonrac)
        End If
        'drainage sous racines vers Stnonrac = bil-(Zrac*Taw) si positif
        'si transpi + Evap excède contenu de Strac Strac devient nul
        'TODO: mais il faudrait réduire transpi+évap en conséquence
        ' normalement y'a pas de raison de faire le test suivant car evap ne peut excéder Stsurf et tranpi
        ' ne peut excéder Strac

        'Strac(joursim) = Max(0, Strac(joursim))


        'chgt variable pour cas où racines < zsurf
        E_Srac = Esol(Joursim) * Min(1, Zrac / Zsurf)
        Strac(Joursim) = Strac(Joursim) - E_Srac - transpi

        If Strac(Joursim) < 0 Then Strac(Joursim) = 0
        'et dans ce cas y'a une ptite quantité d'eau qui échappe au bilan...
        '

        bil = Stnonrac(Joursim - 1) + precip - (Deltazrac * TAW)

        'si Zrac < Zsurf on affecte la part de l'évaporation correspondante à Snonrac via bil
        If Zrac < Zsurf Then bil = bil - Esol(Joursim) + E_Srac

        If bil > TAW * (Zracmax - Zrac) Then
            Dr(Joursim) = bil - TAW * (Zracmax - Zrac)
            Stnonrac(Joursim) = TAW * (Zracmax - Zrac)
            Stprofond(Joursim) = Min(Stprofond(Joursim - 1) + Dr(Joursim), TAW * (Ztotsol - Zracmax))
            Drprofmax(Joursim) = Max(Stprofond(Joursim - 1) + Dr(Joursim) - TAW * (Ztotsol - Zracmax), 0)
        Else
            Dr(Joursim) = 0
            Stnonrac(Joursim) = Max(0, bil)
            ZoneHumSousRacines = Stnonrac(Joursim) / TAW
            Stprofond(Joursim) = Stprofond(Joursim - 1)
        End If
        Stocksol(Joursim) = Stprofond(Joursim) + Stnonrac(Joursim) + Strac(Joursim)
        If Zrac > 0 Then ContrainteW = Strac(Joursim) / (TAW * Zrac) Else ContrainteW = 1
        'If Joursim <= ilev + 20 And Stnonrac(Joursim) <= 0.0001 And Strac(Joursim) <= 0.02 Then
        ' FA: a vérifier introduction par CP de Zrac>0 dans les critères pour test avant accumulation signal flétrissement
        ' (mais ça paraît OK sinon on risque accumuler du signal avant levée)
        If Zrac > 0 And Stnonrac(Joursim) <= 0.0001 And Strac(Joursim) <= 0.02 Then
            'If SignalFletrissement = 0 Or (joursim <= ilev + 20 And Stnonrac(joursim - 1) <= 0.0001 And Strac(joursim - 1) <= 0.02) Then
            If SignalFletrissement(Joursim - 1) = 0 Or (Stnonrac(Joursim - 1) <= 0.0001 And Strac(Joursim - 1) <= 0.02) Then
                SignalFletrissement(Joursim) = SignalFletrissement(Joursim - 1) + 1
                '    Else
                '       SignalFletrissement = 0
                'pas sur ce soit utile, une seule des deux conditions amenant à 0 doit suffire
            End If
        Else
            SignalFletrissement(Joursim) = 0

        End If
        SigmaSimDrprofmax = SigmaSimDrprofmax + Drprofmax(Joursim)
        SigmaSimDr = SigmaSimDr + Dr(Joursim)
        'TODO introduire plage des joursims correspondant à présence culture...
        'SigmaCultDrprofmax = SigmaCultDrprofmax + Drprofmax(joursim)
        'SigmaCultDr = SigmaCultDr + Dr(joursim)

        If Dr(Joursim) > 0 Then DrainageON(Joursim) = True

        'calcul des pertes d'azote par drainage en kgN/ha
        perteNDrain(Joursim) = ConcNsol(Joursim - 1) * Dr(Joursim)

    End Sub
    Property dEsol(Joursim As Integer)
        Get
            Return Esol(Joursim)
        End Get
        Set()
        End Set
    End Property
    Property dZoneHumSousRacines() As Double
        Get
            Return ZoneHumSousRacines
        End Get
        Set()
        End Set
    End Property
    Property dContrainteW() As Double
        Get
            Return ContrainteW
        End Get
        Set()
        End Set
    End Property
    Property dContrainteWSurf() As Double
        Get
            Return ContrainteWSurf
        End Get
        Set()
        End Set
    End Property
    Property dStsurf(Joursim As Integer) As Double
        Get
            Return Stsurf(Joursim)
        End Get
        Set()
        End Set
    End Property
    Property dStrac(Joursim As Integer) As Double
        Get
            Return Strac(Joursim)
        End Get
        Set()
        End Set
    End Property
    Property dStnonrac(Joursim As Integer) As Double
        Get
            Return Stnonrac(Joursim)
        End Get
        Set()
        End Set
    End Property
    Property dDrprofmax(Joursim As Integer) As Double
        Get
            Return Drprofmax(Joursim)
        End Get
        Set()
        End Set
    End Property
    Property dStockSol(Joursim As Integer) As Double
        Get
            Return Stocksol(Joursim)
        End Get
        Set()
        End Set
    End Property
    Property dStprofond(Joursim As Integer) As Double
        Get
            Return Stprofond(Joursim)
        End Get
        Set()
        End Set
    End Property
    Property iZtotsol() As Integer
        Get
            Return Ztotsol
        End Get
        Set()
        End Set
    End Property
    Property dStockMes(Joursim As Integer) As Double
        Get
            Return StockMes(Joursim)
        End Get
        Set()
        End Set
    End Property
    Property dSigmaCultEsol() As Double
        Get
            Return SigmaCultEsol
        End Get
        Set()
        End Set
    End Property
    Property dSigmaSimEsol() As Double
        Get
            Return SigmaSimEsol
        End Get
        Set()
        End Set
    End Property
    Property dSigmaSimDrprofmax() As Double
        Get
            Return SigmaSimDrprofmax
        End Get
        Set()
        End Set
    End Property
    Property dSigmaSimDr() As Double
        Get
            Return SigmaSimDr
        End Get
        Set()
        End Set
    End Property
    Property nZObstacleRac() As Integer
        Get
            Return ZObstacleRac
        End Get
        Set()
        End Set
    End Property
    Property bContrainteHlevee() As Boolean
        Get
            Return ContrainteHlevee
        End Get
        Set()
        End Set
    End Property
    Property dStger(Joursim As Integer) As Double
        Get
            Return Stger(Joursim)
        End Get
        Set()
        End Set
    End Property
    Property dStockN() As Double
        Get
            Return StockN
        End Get
        Set()
        End Set
    End Property
    Property dStockP() As Double
        Get
            Return StockP
        End Get
        Set()
        End Set
    End Property
    Property dStockK() As Double
        Get
            Return StockK
        End Get
        Set()
        End Set
    End Property
    Property nSignalFletrissement(Joursim As Integer) As Integer
        Get
            Return SignalFletrissement(Joursim)
        End Get
        Set()
        End Set
    End Property
    Property ClTypeSurf() As Object
        Get
            Return TypeSurf
        End Get
        Set()
        End Set
    End Property
    Property dConcNsol(Joursim As Integer) As Double
        Get
            Return ConcNsol(Joursim)
        End Get
        Set()
        End Set
    End Property
    Property dVolEausol() As Double
        Get
            Return VolEausol
        End Get
        Set()
        End Set
    End Property
    Property bDrainageON(Joursim As Integer) As Boolean
        Get
            Return DrainageON(Joursim)
        End Get
        Set()
        End Set
    End Property
    Property dperteNDrain(Joursim As Integer) As Double
        Get
            Return perteNDrain(Joursim)
        End Get
        Set()
        End Set
    End Property
    Property dDr(Joursim As Integer) As Double
        Get
            Return Dr(Joursim)
        End Get
        Set()
        End Set
    End Property
    Property dNavail(Joursim As Integer) As Double
        Get
            Return Navail(Joursim)
        End Get
        Set()
        End Set
    End Property
    Property dCNmin(Joursim As Integer) As Double
        Get
            Return CNmin(Joursim)
        End Get
        Set()
        End Set
    End Property
    Property dNmin(Joursim As Integer) As Double
        Get
            Return Nmin(Joursim)
        End Get
        Set()
        End Set
    End Property
    Property dNDenit(Joursim As Integer) As Double
        Get
            Return NDenit(Joursim)
        End Get
        Set()
        End Set
    End Property
    Property dCNDenit(Joursim As Integer) As Double
        Get
            Return CNDenit(Joursim)
        End Get
        Set()
        End Set
    End Property
    Property dMinNorgInput(Joursim As Integer) As Double
        Get
            Return MinNorgInput(Joursim)
        End Get
        Set()
        End Set
    End Property
End Class
