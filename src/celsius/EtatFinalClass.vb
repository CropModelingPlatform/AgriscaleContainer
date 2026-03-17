Option Compare Text

Public Class EtatFinalClass

    Public Stockfinal As Double 'VX, Valeur du stock hydrique utile au dernier jour de la simulation (mm)
    Public QpaillisFinal As Double
    Public FinalSolhautON As Boolean
    'VERSION 3 du 8-10 nov 2017'
    Sub EcritEtatFinal(stock As Double, Qpaillis As Double, SolHaut As Boolean)
        Stockfinal = stock
        QpaillisFinal = Qpaillis
        FinalSolhautON = SolHaut
    End Sub

End Class
