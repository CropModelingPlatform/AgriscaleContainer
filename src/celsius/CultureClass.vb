Option Compare Text
Option Explicit On
Imports System.Math


Public Class CultureClass
    ' module représentant et calculant l'état de la culture vue comme association de plantes
    'VX: Variable explicative, VE: variable d'état, VS: variable simulée,PX: paramètre
    Dim TransPotMC(731) As Double

    Dim LaiMC(731) As Double 'VE LAI de la canopée formée per les espèces présentes
    Dim Kc(731) As Double ' VE coefficient cultural de l'association
    Dim EoSM As Double ' VE évaporation potentielle au sommet du mulch (sous la canopée) (mm)
    Dim delta As Double  'PX, coefficient de réduction de l'ETP par la canopée (extin plantes -0.2)
    Dim TPotMC(731) As Double 'VE, transpiration potentielle de l'association (mm)
    Dim TranspiMC(731) As Double 'VE, transpiration réelle de l'association (mm)
    Dim ZRacineMC(731) As Double 'VE, cote atteinte par les racines de l'association
    Dim WStressTMC(731) As Double 'VE, stress hydrique limitant la transpiration de l'association
    Dim ZracmaxMC As Integer 'VX, cote maximale atteignable par les racines de l'association (cm)
    Dim SigmaTranspiMC 'VS, cumul sur la simulation (=sur la culture) de la transpiration de l'association  (mm)
    Dim KmaxMC As Double 'VE, Kmax de l'association
    Dim NuptMC(731) As Double 'quantité d'azote prélevée par l'association au joursim (kgN/ha)
    'Dim PfactorMC As Double
    ' constantes à lire plus tard dans base et ou faire dépendre des especes impliquées

    Const beta As Double = 1.4 'coefficient d'augmentation de l'ETP en cas de faible évaporation
    ' (cf stics)
    Const PfactorMC As Double = 0.7 'seuil d'acion de la contrainte hydrique sur la transpiration (était fixé à 0.55 ds version pour EScAPE 1)
    '
    'to do note par Charlotte Poeydebat : faire dépendre Pfactor de l'espèce (PlantSpecies) et introduire une procédure dans culture qui
    'permette de calculer un PfactorMC à partir des Pfactor des plantes de l'association
    'prendre le Pfactor min qui correspond à l'espèce qui est la moins sensible au ralentissement de la transpiration
    'par le stress hydrique, celle qui transpire le plus le plus longtemps quand il y a stress hydrique,
    '(celle qui ferme ses stomates le plus tard)
    ' VERSION 3 du 8-10 nov 2017'

    'modif 10/05/2011
    'dernière modif 02/04/2012 ajout property get TpotMC pour sortie dans OutputD et utilisation Kmax lu dans plantespecies
    ' passer PfactorMC dans table à lire...
    'modifs CP octobre 13 introduction NuptMC et actualisation via nouvelle sub
    'to do : faire dépendre Pfactor de l'espèce (PlantSpecies) et introduire une procédure dans culture qui
    'permette de calculer un PfactorMC à partir des Pfactor des plantes de l'association
    'prendre le Pfactor min qui correspond à l'espèce qui est la moins sensible au ralentissement de la transpiration
    'par le stress hydrique, celle qui transpire le plus le plus longtemps quand il y a stress hydrique,
    '(celle qui ferme ses stomates le plus tard)
    ' OK relu FA le 15/11 pas de pb
    'modif FA le 12/06/15 constante Pfactor


    Sub lisCultureMC()
        ' prévu si Variables à lire et ou initialiser pour l'association
        Dim i As Integer
        For i = 0 To 731
            WStressTMC(i) = 1
        Next i
    End Sub
    ' TODO: imaginée pour être appelée en boucle sur liste des plantes ?
    'met à jour ZracmaxMC
    Sub InitMixedCanopy(extin As Double, Zracmax As Integer, Kmax As Double)
        Dim KExtinMC As Double
        If extin <> 0 Then KExtinMC = extin
        delta = KExtinMC - 0.2
        ' profond max de l'assoc = max des Zrac des especes présentes
        ZracmaxMC = Max(ZracmaxMC, Zracmax)
        ' Kmax de l'assoc = max des Max PROVISOIRE Pratique quand une seule espece !!
        KmaxMC = Max(KmaxMC, Kmax)
    End Sub
    Sub LaiMixedCanopy(joursim As Integer, Lai As Double)
        ' La procédure est appellée chaque jour en bouclant sur la liste des plantes présentes,
        ' LaiMC étant la somme des LAI de toutes les plantes
        LaiMC(joursim) = LaiMC(joursim) + Lai
    End Sub
    Sub CroiRacMixedCrop(joursim As Integer, Zrac As Double)
        'on prend la cote maximale atteinte par les plantes de l'assoc
        ZRacineMC(joursim) = Max(Zrac, ZRacineMC(joursim))
    End Sub

    Sub Evaporation_Pot_SolMulch(joursim, Etp)
        ' EoSM = évaporation potentielle au sommet du mulch (sous la canopée)
        EoSM = Etp * Exp(-delta * (LaiMC(joursim)))
    End Sub
    Sub CalcTranspiMC(joursim As Integer, Etp As Double, Esol As Double, Emulch As Double, ContrainteW As Double)

        Dim eo As Double
        Dim eop As Double

        If (LaiMC(joursim) > 0) Then 'todo verif si test utile

            Kc(joursim) = (1 + (KmaxMC - 1) / (1 + Exp(-1.5 * (LaiMC(joursim) - 3))))
            eo = Etp * Kc(joursim)
            'effet de l'assechement du sol sur l'etp de culture
            eop = (eo - EoSM) * (beta + (1 - beta) * (Esol + Emulch) / EoSM)
        End If
        TPotMC(joursim) = eop


        If ContrainteW > PfactorMC Then WStressTMC(joursim) = 1 Else WStressTMC(joursim) = ContrainteW / PfactorMC

        TranspiMC(joursim) = eop * WStressTMC(joursim)
        SigmaTranspiMC = SigmaTranspiMC + TranspiMC(joursim)

    End Sub
    Sub NuptakeMixedCrop(joursim, Nuptake)

        NuptMC(joursim) = NuptMC(joursim) + Nuptake

    End Sub
    Property dLaiMC(Joursim As Integer) As Double
        Get
            Return LaiMC(Joursim)
        End Get
        Set()
        End Set
    End Property
    Property dEoSM() As Double
        Get
            Return EoSM
        End Get
        Set()
        End Set
    End Property
    Property dZRacineMC(Joursim As Integer) As Double
        Get
            Return ZRacineMC(Joursim)
        End Get
        Set()
        End Set
    End Property
    Property dZracmaxMC() As Integer
        Get
            Return ZracmaxMC
        End Get
        Set()
        End Set
    End Property
    Property dTranspiMC(Joursim As Integer) As Double
        Get
            Return TranspiMC(Joursim)
        End Get
        Set()
        End Set
    End Property
    Property dSigmaTranspiMC() As Double
        Get
            Return SigmaTranspiMC
        End Get
        Set()
        End Set
    End Property
    Property dTPotMC(Joursim As Integer) As Double
        Get
            Return TPotMC(Joursim)
        End Get
        Set()
        End Set
    End Property
    Property dNuptMC(Joursim As Integer) As Double
        Get
            Return NuptMC(Joursim)
        End Get
        Set()
        End Set
    End Property

End Class
