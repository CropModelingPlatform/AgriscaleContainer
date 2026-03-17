Option Compare Text
Option Explicit On
Imports System.Data
Imports System.Data.Sqlite
Public Class GestionTechniqueClass

    'VX: Variable explicative, VE: variable d'état, VS: variable simulée,PX: paramètre, CS: controle de simulation (lien vers autres variables)
    Dim DataTechAdp As SqliteDataAdapter
    Dim rstDataTech As DataSet
    Dim IdCultivar(0 To 2) As String 'champ identifiant l'espece et le cultivar, lu par le modèle et utilisé pour trouver les paramètres du cultivar et de l'espece dans les tables correspondantes
    Dim Nbcult As Integer 'nombre de cultivars dans l'association, lu dans "Tech_Commun"
    Dim Numcrop As Integer 'numéro du cultivar de l'association, lu dans "Tech_perCrop"
    Dim TypeInstal(0 To 2) As Integer 'VX, Type d'installation du cultivar de numéro Numcrop. 1: semis graine pré-germée; 2: semis graine sèche; 3: repiquage
    Dim RepiquageON(0 To 2) As Boolean 'VX, repiquage oui non (pour le cultivar numcrop)

    Dim iplt(0 To 2) As Integer 'VX, date de semis en jour de 1 à 731 pour le cult. numcrop
    'Dim jourplt(1 To 2) As Integer
    ' jourplt est dans le calendrier de simulation ***PAS UTILISE
    Dim DensSem(0 To 2) As Single 'VX, densité de plantes au semis pour le cultivar numcrop
    Dim irepiqu(0 To 2) As Integer 'VX jour de repiquage ou démariage pour le cultivar numcrop
    Dim densrepiqu(0 To 2) As Single 'VX,densité après repiquage ou démariage pour le cultivar numcrop
    Dim ilev(0 To 2) As Integer 'VX, jour de la levée dans le calendrieer annuel pour le cultivar numcrop
    Dim SerreTunnelON As Boolean 'VX, Si vrai pépinière en serre tunnel, la température de l'air sera corrigée par procédure CorrigTSerres pendant tout la durée de la pépinière (cropsta=3)
    Dim imulch As Integer 'VX date mise en place du mulch
    Dim CodParamMulch As Integer 'CS code du type de mulch dans la table "mulch"
    Dim QpaillisApport As Double 'VX quantité de paillis apportée au jour imulch (Mg/ha)
    Dim AltiCult As Integer 'VX, altitude de la culture pour correction temperature station
    Dim IrrigON As Boolean 'VX, culture irriguée Vrai/ Faux

    Dim tApportMON As Double 'VX indice d'apport de MO *** attention chgt nom depuis version 1
    Dim tApportMinN As Double 'VX indice d'apport d'engrais minéral *** attention chgt nom depuis version 1
    'variables de la gestion technique auto
    Dim DbutoirNouvSemis(0 To 2) As Integer 'VX: date en jours après levée au delà de laquelle une culture détruite par un aléa n'est plus ressemée
    Dim Ressemis(0 To 2) As Boolean 'VE ressemis à faire vrai ou faux (si CyberST activé dans options model, devient vrai en cas de mort de la plante avant un certain jour
    Dim SeuilCumPrecip(0 To 2) As Double 'VX seuil de cumul de pluies infiltrées consécutives déclenchant le semis en cas de semis automatique
    Dim JourNouvSem(0 To 2) As Integer
    Dim Nbsemis(0 To 2)
    Dim fertiminON As Boolean '??
    Dim fertiorgON As Boolean '??
    'Dim AjustStockON As Boolean '?? concerne le stock hydrique de dé"but de cycle ?
    'Dim jourlev(1 To 2) As Integer
    'jourlev est dans le calendrier de simulation finalement pas utilisé, tableau DOY plus pratique
    ' modif: 10/05/2011
    '  modif 13/11/2011 introduction correction altitude
    'modif 08/03/2012 introduction irrigation: nouveau champ dans Tech_commun
    ' modif FA 7/12/12 stress N et distinction legumineuse / non legumineuse
    ' modif 04/13 ressemis auto
    ' modif CP 30/05/13 Dans Sub SemisAuto remplacement de Plu par Stger comme critère pour déclencher le semis
    ' dernière modif CP 15/10/2013 lecture de AjustStockON dans TechCommun (si vrai = lecture de StockBase pour calcul de StockMesAj, pas encore fonctionnel)
    ' OK révisé FA en entier le 14/11, faut désactiver l'histoire du stock inistial ajusté, le reste OK
    Dim DriveRuiObs As Boolean 'VX, si vrai forçage du modèle par le ruissellement observé, lu dans table RuissellementObs
    '
    'VERSION 3 du 8-10 nov 2017'
    Sub LisTech(DataBase_Cnn As SqliteConnection, SimUnit As SimulationUnitClass)
        Dim Trouve As Boolean
        Dim msg As String

        'rstDataTech = New ADODB.Recordset
        Trouve = False
        rstDataTech = New DataSet
        DataTechAdp = New SqliteDataAdapter("SELECT * FROM Tech_Commun where IdTech_Com='" & SimUnit.sIdTec & "'", DataBase_Cnn)
        DataTechAdp.Fill(rstDataTech)
        If rstDataTech.Tables(0).Rows.Count > 0 Then
            'rstDataTech.Open "Tech_Commun", DataBase_Cnn
            'rstDataTech.MoveFirst
            'Do While Not rstDataTech.EOF And Not Trouve
            'If rstDataTech.Tables(0).Rows(0)("IdTech_Com ") = SimUnit.sIdTec Then
            Trouve = True
            Nbcult = rstDataTech.Tables(0).Rows(0)("Nbcult")
            SerreTunnelON = rstDataTech.Tables(0).Rows(0)("SerreTunnelON")
            imulch = rstDataTech.Tables(0).Rows(0)("imulch")
            CodParamMulch = rstDataTech.Tables(0).Rows(0)("CodParamMulch")
            QpaillisApport = rstDataTech.Tables(0).Rows(0)("QpaillisApport")
            AltiCult = rstDataTech.Tables(0).Rows(0)("AltiCult")
            IrrigON = rstDataTech.Tables(0).Rows(0)("IrrigON")
            tApportMON = rstDataTech.Tables(0).Rows(0)("tApportMON")
            tApportMinN = rstDataTech.Tables(0).Rows(0)("tApportMinN")
            fertiminON = rstDataTech.Tables(0).Rows(0)("fertiminON")
            fertiorgON = rstDataTech.Tables(0).Rows(0)("fertiorgON")
            'AjustStockON = rstDataTech.Tables(0).Rows(0)("AjusStockON")
            DriveRuiObs = rstDataTech.Tables(0).Rows(0)("DriveRuiObs")
        End If
        'rstDataTech.MoveNext()
        'End While
        'rstDataTech.Close()
        ' vérifier si faut pas rstDataTech=nothing
        rstDataTech.Clear()
        DataTechAdp = New SqliteDataAdapter("Select * FROM Tech_perCrop where IdTech_Com='" & SimUnit.sIdTec & "' Order by NumCrop", DataBase_Cnn)
            DataTechAdp.Fill(rstDataTech)

        For I = 0 To rstDataTech.Tables(0).Rows.Count - 1
            'Do While Not rstDataTech.EOF
            Numcrop = rstDataTech.Tables(0).Rows(I)("Numcrop")
            IdCultivar(Numcrop) = rstDataTech.Tables(0).Rows(I)("IdCultivar")
            TypeInstal(Numcrop) = rstDataTech.Tables(0).Rows(I)("TypInstal")
            RepiquageON(Numcrop) = rstDataTech.Tables(0).Rows(I)("RepiquageON")
            iplt(Numcrop) = rstDataTech.Tables(0).Rows(I)("isem")
            '        jourplt(Numcrop) = Calendrier_versSim(iplt(Numcrop), SimUnit.nStartDay, SimUnit.bSY_Bissextile)
            DensSem(Numcrop) = rstDataTech.Tables(0).Rows(I)("DensSem")
            ilev(Numcrop) = rstDataTech.Tables(0).Rows(I)("ilev")

            '        jourlev(Numcrop) = Calendrier_versSim(ilev(Numcrop), SimUnit.nStartDay, SimUnit.bSY_Bissextile)
            irepiqu(Numcrop) = rstDataTech.Tables(0).Rows(I)("irepiqu")
            densrepiqu(Numcrop) = rstDataTech.Tables(0).Rows(I)("densrepiqu")
            DbutoirNouvSemis(Numcrop) = rstDataTech.Tables(0).Rows(I)("DbutoirNouvSemis")
            SeuilCumPrecip(Numcrop) = rstDataTech.Tables(0).Rows(I)("SeuilCumPrecip")
            Ressemis(Numcrop) = rstDataTech.Tables(0).Rows(I)("SemisAutoDebut")

            ' Steplait, complete ici lecture de tous les champs de tech_perCrop
            Call CoherenceGTech(Numcrop)
            '    rstDataTech.MoveNext
        Next
        ' test cohérence numcrop NbCult
        If Numcrop <> Nbcult Then
            Nbcult = Numcrop
            msg = "nombre de cultures retenues: " & Str(Numcrop)
            Console.WriteLine(msg)
        End If
        'fermeture table et libération mémoire de l'objet
        'rstDataTech.Close()
        'rstDataTech = Nothing

    End Sub
    Sub CoherenceGTech(Numcrop As Integer)
        Dim CoherenceSemis As Boolean
        Dim CoherenceIlev As Boolean
        Dim CoherenceDensite As Boolean
        Dim msg As String

        CoherenceSemis = True
        CoherenceIlev = True
        CoherenceDensite = True

        If (TypeInstal(Numcrop) = 3 And irepiqu(Numcrop) <> 999) Then CoherenceSemis = False
        If (TypeInstal(Numcrop) = 3 And irepiqu(Numcrop) <> 999) Then CoherenceSemis = False
        If (TypeInstal(Numcrop) = 3 And Not RepiquageON(Numcrop) And irepiqu(Numcrop) <> 999) Then CoherenceSemis = False

        If Not CoherenceSemis Then
            msg = "Attention pb potentiel de coherence date repiquage / type d'installation"
            msg = msg & "si pas de repiquage (TypInstal<>3) et RepiquageON=non, irepiqu doit être 999"
            Console.WriteLine(msg)
            msg = "seule possibilité où typinstal=3 et repiquageON=Oui : il y a démariage à irepiqu (réduction de la densité)"
            Console.WriteLine(msg)
        End If
        If TypeInstal(Numcrop) = 1 And ilev(Numcrop) <> iplt(Numcrop) + 1 Then CoherenceIlev = False
        'If TypeInstal(Numcrop) = 2 And ilev(Numcrop) <= iplt(Numcrop) + 1 Then CoherenceIlev = False
        If Not CoherenceIlev Then
            msg = "pb de coherence date de levée, typinstal, date semis"
            msg = msg & "si semis graine prégermée, typinstal=1 et datelevée doit être = isem+1"
            Console.WriteLine(msg)
        End If
        If RepiquageON(Numcrop) = False And densrepiqu(Numcrop) <> DensSem(Numcrop) Then CoherenceDensite = False
        If Not CoherenceDensite Then
            msg = " pb de coherence densité de repiquage / densité de semis "
            Console.WriteLine(msg)
        End If
    End Sub
    Sub CyberPlouck(icult As Integer, die As Boolean, Deathday As Integer, Joursim As Integer, StartDay As Integer)
        'automate ajustant des actes techniques aux conditions du milieu
        If (die And (Deathday > 0)) And (Joursim + StartDay - 1 < DbutoirNouvSemis(icult)) Then
            Ressemis(icult) = True

        End If
    End Sub
    Sub SemisAuto(icult As Integer, Joursim As Integer, StartDay As Integer, Cumpluinf As Double)
        Dim nouvsemJA As Integer, EcartSem As Integer

        If (Joursim + StartDay - 1 >= iplt(icult)) And (Cumpluinf >= SeuilCumPrecip(icult)) Then
            JourNouvSem(icult) = Joursim
            nouvsemJA = Joursim + StartDay - 1
            EcartSem = nouvsemJA - iplt(icult)
            If irepiqu(icult) <> 999 Then irepiqu(icult) = irepiqu(icult) + EcartSem
            iplt(icult) = nouvsemJA
            'histoire de ne plus repasser par ici:
            Ressemis(icult) = False
            '
            ' attention il y a peut etre besoin de réinitialiser la plante...?
        End If
    End Sub
    Sub CompteSemis(icult As Integer)
        Nbsemis(icult) = Nbsemis(icult) + 1
    End Sub
    Property sIdCultivar(icult As Integer) As String
        Get
            Return IdCultivar(icult)
        End Get
        Set()
        End Set
    End Property
    ' steplait complete les property get pour tous les paramètres lus dans table
    Property nNbCult() As Integer
        Get
            Return Nbcult
        End Get
        Set()
        End Set
    End Property
    Property niplt(icult As Integer) As Integer
        Get
            Return iplt(icult)
        End Get
        Set()
        End Set
    End Property
    Property nilev(icult As Integer) As Integer
        Get
            Return ilev(icult)
        End Get
        Set()
        End Set
    End Property
    Property nirepiqu(icult As Integer) As Integer
        Get
            Return irepiqu(icult)
        End Get
        Set()
        End Set
    End Property
    Property sTypeInstal(icult As Integer) As Integer
        Get
            Return TypeInstal(icult)
        End Get
        Set()
        End Set
    End Property
    Property bRepiquageON(icult As Integer) As Boolean
        Get
            Return RepiquageON(icult)
        End Get
        Set()
        End Set
    End Property
    Property dDensSem(icult As Integer) As Double
        Get
            Return DensSem(icult)
        End Get
        Set()
        End Set
    End Property
    Property dDensRepiqu(icult As Integer) As Double
        Get
            Return densrepiqu(icult)
        End Get
        Set()
        End Set
    End Property
    Property bSerreTunnelON() As Boolean
        Get
            Return SerreTunnelON
        End Get
        Set()
        End Set
    End Property
    Property nCodParamMulch() As Integer
        Get
            Return CodParamMulch
        End Get
        Set()
        End Set
    End Property
    Property nimulch() As Integer
        Get
            Return imulch
        End Get
        Set()
        End Set
    End Property
    Property dQpaillisApport() As Double
        Get
            Return QpaillisApport
        End Get
        Set()
        End Set
    End Property
    Property nAltiCult() As Integer
        Get
            Return AltiCult
        End Get
        Set()
        End Set
    End Property
    Property bIrrigON() As Boolean
        Get
            Return IrrigON
        End Get
        Set()
        End Set
    End Property
    Property bfertiminON() As Boolean
        Get
            Return fertiminON
        End Get
        Set()
        End Set
    End Property
    Property bfertiorgON() As Boolean
        Get
            Return fertiorgON
        End Get
        Set()
        End Set
    End Property
    Property dtApportMON() As Double
        Get
            Return tApportMON
        End Get
        Set()
        End Set
    End Property
    Property bRessemis(icult As Integer) As Boolean
        Get
            Return Ressemis(icult)
        End Get
        Set()
        End Set
    End Property
    Property nJourNouvSem(icult As Integer) As Integer
        Get
            Return JourNouvSem(icult)
        End Get
        Set()
        End Set
    End Property
    Property nNbsemis(icult As Integer) As Integer
        Get
            Return Nbsemis(icult)
        End Get
        Set()
        End Set
    End Property
    Property dtApportMinN() As Double
        Get
            Return tApportMinN
        End Get
        Set()
        End Set
    End Property
    Property bDriveRuiObs() As Double
        Get
            Return DriveRuiObs
        End Get
        Set()
        End Set
    End Property
End Class
