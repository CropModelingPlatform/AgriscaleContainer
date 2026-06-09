Option Compare Text
Option Explicit On
Imports System.Data
Imports System.Data.Sqlite
Public Class ApportsOrgaClass


    'classe décrivanht les apports Organiques (résidus, fumiers)
    'VX: variable explicative, VE: variable d 'état, VS: variable simulée,PX: paramètre
    Dim Trouve As Boolean
    Dim Akres As Double ' parametre de la vitesse de décomposition des résidus
    Dim Bkres As Double ' parametre de la vitesse de décomposition des résidus
    Dim AHres As Double ' parametre de la vitesse de décomposition de la biomasse microbienne
    Dim BHres As Double ' parametre de la vitesse de décomposition de la biomasse microbienne
    Dim Nres As Double 'teneur en N des résidus
    Dim AWB As Double 'parametre du C/N de la biomasse microbienne
    Dim BWB As Double 'parametre du C/N de la biomasse microbienne
    Dim Yres As Double 'constante de partition de la décomposition des apports organiques veres la biomasse microbienne
    Dim Kbio As Double ' constante de décomposition de la biomasse microbienne
    Dim Fbio As Double 'facteur de correction du C/N de la biomasse microbienne quand le N est limitant. Fixé à 1 pour l'instant
    Dim NRec As Double
    Dim dataResidusAdp As SqliteDataAdapter
    Dim rstResidus As DataSet


    Sub LisApportsOrga(DataBase_Cnn As SqliteConnection, TypeMorga As String)
        'attentio nce qui suit lecture de fertiorgaList pas fini
        rstResidus = New DataSet
        dataResidusAdp = New SqliteDataAdapter("SELECT * FROM ListResidus where TypeMorga='" & TypeMorga & "'", DataBase_Cnn)
        dataResidusAdp.Fill(rstResidus)

        'rstResidus.Open "ListResidus", DataBase_Cnn
        'rstResidus.MoveFirst
        Trouve = False
        If rstResidus.Tables(0).Rows.Count > 0 Then

            'While Not rstResidus.EOF And Not Trouve
            'If rstResidus!TypeMorga = TypeMorga Then
            Trouve = True
            Akres = rstResidus.Tables(0).Rows(0)("Akres")
            Bkres = rstResidus.Tables(0).Rows(0)("Bkres")
            AHres = rstResidus.Tables(0).Rows(0)("AHres")
            BHres = rstResidus.Tables(0).Rows(0)("BHres")
            AWB = rstResidus.Tables(0).Rows(0)("AWB")
            BWB = rstResidus.Tables(0).Rows(0)("BWB")
            Yres = rstResidus.Tables(0).Rows(0)("Yres")
            Kbio = rstResidus.Tables(0).Rows(0)("Kbio")
            Fbio = rstResidus.Tables(0).Rows(0)("Fbio")
            Nrec = rstResidus.Tables(0).Rows(0)("Nrec")
        End If
        'rstResidus.MoveNext
        'Wend
        'rstResidus.Close


        'Set rstResidus = Nothing
        '
    End Sub
    Property dNrec() As Double
        Get
            Return Nrec
        End Get
        Set()
        End Set
    End Property
    '    Property Get dNrec() As Double
    'dNrec = Nrec
    'End Property

    '    Property Get dAkres() As Double
    'dAkres = Akres
    'End Property
    Property dAkres() As Double
        Get
            Return Akres
        End Get
        Set()
        End Set
    End Property
    '    Property Get dBkres() As Double
    'dBkres = Bkres
    'End Property
    Property dBkres() As Double
        Get
            Return Bkres
        End Get
        Set()
        End Set
    End Property
    '    Property Get dAHres() As Double
    'dAHres = AHres
    'End Property
    Property dAHres() As Double
        Get
            Return AHres
        End Get
        Set()
        End Set
    End Property

    '    Property Get dBHres() As Double
    'dBHres = BHres
    'End Property
    Property dBHres() As Double
        Get
            Return BHres
        End Get
        Set()
        End Set
    End Property
    '    Property Get dAWB() As Double
    'dAWB = AWB
    'End Property
    Property dAWB() As Double
        Get
            Return dAWB = AWB

        End Get
        Set()
        End Set
    End Property
    '    Property Get dBWB() As Double
    'dBWB = BWB
    'End Property
    Property dBWB() As Double
        Get
            Return BWB
        End Get
        Set()
        End Set
    End Property
    '    Property Get dYres() As Double
    'dYres = Yres
    'End Property
    Property dYres() As Double
        Get
            Return Yres
        End Get
        Set()
        End Set
    End Property
    '    Property Get dKbio() As Double
    'dKbio = Kbio
    'End Property
    Property dKbio() As Double
        Get
            Return Kbio
        End Get
        Set()
        End Set
    End Property
    '    Property Get dFbio() As Double
    'dFbio = Fbio
    'End Property
    Property dFbio() As Double
        Get
            Return Fbio
        End Get
        Set()
        End Set
    End Property
End Class
