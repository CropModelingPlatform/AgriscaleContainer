Imports System
Imports System.IO
Imports System.Text
Imports System.Globalization
Imports System.Configuration
Imports System.Data
Imports System.Data.Sqlite

Public Class SticsFicplt1Converter
    
    Inherits Converter
    
    Public Overrides Sub Export(DirectoryPath As String)
    End Sub

    Public Overrides Sub Export(
            DirectoryPath As String,
            MasterInput_Connection As SqliteConnection,
            ModelDictionary_Connection As SqliteConnection)

        Dim fileName As String = "ficplt1.txt"
        Dim fileContent As StringBuilder = New StringBuilder()
        Dim ST(3) As String
        ST = DirectoryPath.Split(Path.DirectorySeparatorChar)
        DirectoryPath = ST(0) & Path.DirectorySeparatorChar & ST(1) & Path.DirectorySeparatorChar & ST(2) & Path.DirectorySeparatorChar & ST(3) & Path.DirectorySeparatorChar & ST(4) & Path.DirectorySeparatorChar & ST(5) & Path.DirectorySeparatorChar & ST(6) & Path.DirectorySeparatorChar & ST(7)
        Dim T As String = "Select  Champ, Default_Value_Datamill, defaultValueOtherSource, IFNULL([defaultValueOtherSource],  [Default_Value_Datamill]) As dv From Variables Where ((model='stics') AND ([Table]='st_ficplt1') or (model='stics') AND ([Table]='st_ficplt1_2'));"
        Dim DT As New DataSet()

        Dim Cmd As New SqliteDataAdapter(T, ModelDictionary_Connection)
        Cmd.Fill(DT, "TChamp")

        'Ficplt1 query
        Dim fetchAllQuery As String = "select * from simunitlist where idsim='" + ST(7) + "';"

        FormatSticsData(fileContent, DT, "codeplante")
        FormatSticsData(fileContent, DT, "codemonocot")
        FormatSticsData(fileContent, DT, "alphaco2")
        FormatSticsData(fileContent, DT, "tdmin")
        FormatSticsData(fileContent, DT, "tdmax")
        FormatSticsData(fileContent, DT, "codetemp")
        FormatSticsData(fileContent, DT, "codegdh")
        FormatSticsData(fileContent, DT, "coeflevamf")
        FormatSticsData(fileContent, DT, "coefamflax")
        FormatSticsData(fileContent, DT, "coeflaxsen")
        FormatSticsData(fileContent, DT, "coefsenlan")
        FormatSticsData(fileContent, DT, "coeflevdrp")
        FormatSticsData(fileContent, DT, "coefdrpmat")
        FormatSticsData(fileContent, DT, "coefflodrp")
        FormatSticsData(fileContent, DT, "codephot")
        FormatSticsData(fileContent, DT, "phobase", 3)
        FormatSticsData(fileContent, DT, "phosat", 3)
        FormatSticsData(fileContent, DT, "coderetflo")
        FormatSticsData(fileContent, DT, "stressdev")
        FormatSticsData(fileContent, DT, "codebfroid")
        FormatSticsData(fileContent, DT, "jvcmini")
        FormatSticsData(fileContent, DT, "julvernal")
        FormatSticsData(fileContent, DT, "tfroid")
        FormatSticsData(fileContent, DT, "ampfroid")
        FormatSticsData(fileContent, DT, "stdordebour", 3)
        FormatSticsData(fileContent, DT, "tdmindeb")
        FormatSticsData(fileContent, DT, "tdmaxdeb")
        FormatSticsData(fileContent, DT, "codedormance")
        FormatSticsData(fileContent, DT, "ifindorm")
        FormatSticsData(fileContent, DT, "q10")
        FormatSticsData(fileContent, DT, "idebdorm")
        FormatSticsData(fileContent, DT, "codegdhdeb")
        FormatSticsData(fileContent, DT, "tgmin")
        FormatSticsData(fileContent, DT, "nbfeuilplant")
        FormatSticsData(fileContent, DT, "codeperenne")
        FormatSticsData(fileContent, DT, "codegermin")
        FormatSticsData(fileContent, DT, "stpltger")
        FormatSticsData(fileContent, DT, "potgermi")
        FormatSticsData(fileContent, DT, "nbjgerlim")
        FormatSticsData(fileContent, DT, "propjgermin")
        FormatSticsData(fileContent, DT, "codehypo")
        FormatSticsData(fileContent, DT, "belong")
        FormatSticsData(fileContent, DT, "celong")
        FormatSticsData(fileContent, DT, "elmax")
        FormatSticsData(fileContent, DT, "nlevlim1")
        FormatSticsData(fileContent, DT, "nlevlim2")
        FormatSticsData(fileContent, DT, "vigueurbat")
        FormatSticsData(fileContent, DT, "laiplantule")
        FormatSticsData(fileContent, DT, "masecplantule")
        FormatSticsData(fileContent, DT, "zracplantule")
        FormatSticsData(fileContent, DT, "phyllotherme")
        FormatSticsData(fileContent, DT, "bdens")
        FormatSticsData(fileContent, DT, "laicomp")
        FormatSticsData(fileContent, DT, "hautbase")
        FormatSticsData(fileContent, DT, "hautmax")
        FormatSticsData(fileContent, DT, "tcmin")
        FormatSticsData(fileContent, DT, "tcmax")
        FormatSticsData(fileContent, DT, "tcxstop")
        FormatSticsData(fileContent, DT, "codelaitr")
        FormatSticsData(fileContent, DT, "vlaimax")
        FormatSticsData(fileContent, DT, "pentlaimax")
        FormatSticsData(fileContent, DT, "udlaimax")
        FormatSticsData(fileContent, DT, "ratiodurvieI")
        FormatSticsData(fileContent, DT, "ratiosen")
        FormatSticsData(fileContent, DT, "abscission")
        FormatSticsData(fileContent, DT, "parazofmorte")
        FormatSticsData(fileContent, DT, "innturgmin")
        FormatSticsData(fileContent, DT, "dlaimin")
        FormatSticsData(fileContent, DT, "codlainet")
        FormatSticsData(fileContent, DT, "dlaimax", 6)
        FormatSticsData(fileContent, DT, "tustressmin")
        FormatSticsData(fileContent, DT, "dlaimaxbrut", 6)
        FormatSticsData(fileContent, DT, "durviesupmax")
        FormatSticsData(fileContent, DT, "innsen")
        FormatSticsData(fileContent, DT, "rapsenturg")
        FormatSticsData(fileContent, DT, "codestrphot")
        FormatSticsData(fileContent, DT, "phobasesen")
        FormatSticsData(fileContent, DT, "dltamsmaxsen")
        FormatSticsData(fileContent, DT, "dltamsminsen")
        FormatSticsData(fileContent, DT, "alphaphot")
        FormatSticsData(fileContent, DT, "tauxrecouvmax")
        FormatSticsData(fileContent, DT, "tauxrecouvkmax")
        FormatSticsData(fileContent, DT, "pentrecouv")
        FormatSticsData(fileContent, DT, "infrecouv")
        FormatSticsData(fileContent, DT, "codetransrad")
        FormatSticsData(fileContent, DT, "extin")
        FormatSticsData(fileContent, DT, "ktrou")
        FormatSticsData(fileContent, DT, "forme")
        FormatSticsData(fileContent, DT, "rapforme")
        FormatSticsData(fileContent, DT, "adfol")
        FormatSticsData(fileContent, DT, "dfolbas")
        FormatSticsData(fileContent, DT, "dfolhaut")
        FormatSticsData(fileContent, DT, "temin")
        FormatSticsData(fileContent, DT, "temax")
        FormatSticsData(fileContent, DT, "teopt")
        FormatSticsData(fileContent, DT, "teoptbis")
        FormatSticsData(fileContent, DT, "efcroijuv")
        FormatSticsData(fileContent, DT, "efcroiveg")
        FormatSticsData(fileContent, DT, "efcroirepro")
        FormatSticsData(fileContent, DT, "remobres")
        FormatSticsData(fileContent, DT, "coefmshaut")
        FormatSticsData(fileContent, DT, "slamax", 3)
        FormatSticsData(fileContent, DT, "slamin", 3)
        FormatSticsData(fileContent, DT, "tigefeuil")
        FormatSticsData(fileContent, DT, "envfruit")
        FormatSticsData(fileContent, DT, "sea")
        FormatSticsData(fileContent, DT, "codeindetermin")
        FormatSticsData(fileContent, DT, "nbjgrain")
        FormatSticsData(fileContent, DT, "cgrain", 3)
        FormatSticsData(fileContent, DT, "cgrainv0", 3)
        FormatSticsData(fileContent, DT, "nbgrmin", 3)
        FormatSticsData(fileContent, DT, "codeir")
        FormatSticsData(fileContent, DT, "vitircarbT")
        FormatSticsData(fileContent, DT, "nboite")
        FormatSticsData(fileContent, DT, "allocfrmax")
        FormatSticsData(fileContent, DT, "afpf")
        FormatSticsData(fileContent, DT, "bfpf")
        FormatSticsData(fileContent, DT, "cfpf")
        FormatSticsData(fileContent, DT, "dfpf")
        FormatSticsData(fileContent, DT, "stdrpnou")
        FormatSticsData(fileContent, DT, "spfrmin")
        FormatSticsData(fileContent, DT, "spfrmax")
        FormatSticsData(fileContent, DT, "splaimin")
        FormatSticsData(fileContent, DT, "splaimax")
        FormatSticsData(fileContent, DT, "codcalinflo")
        FormatSticsData(fileContent, DT, "nbinflo")
        FormatSticsData(fileContent, DT, "inflomax")
        FormatSticsData(fileContent, DT, "pentinflores")
        FormatSticsData(fileContent, DT, "codetremp")
        FormatSticsData(fileContent, DT, "tminremp")
        FormatSticsData(fileContent, DT, "tmaxremp")
        FormatSticsData(fileContent, DT, "vitpropsucre")
        FormatSticsData(fileContent, DT, "vitprophuile")
        FormatSticsData(fileContent, DT, "vitirazo")
        FormatSticsData(fileContent, DT, "vitircarb")
        FormatSticsData(fileContent, DT, "irmax")
        FormatSticsData(fileContent, DT, "sensanox")
        FormatSticsData(fileContent, DT, "stoprac")
        FormatSticsData(fileContent, DT, "sensrsec")
        FormatSticsData(fileContent, DT, "contrdamax")
        FormatSticsData(fileContent, DT, "codetemprac")
        FormatSticsData(fileContent, DT, "coderacine")
        FormatSticsData(fileContent, DT, "zlabour")
        FormatSticsData(fileContent, DT, "zpente")
        FormatSticsData(fileContent, DT, "zprlim")
        FormatSticsData(fileContent, DT, "draclong", 3)
        FormatSticsData(fileContent, DT, "debsenrac", 3)
        FormatSticsData(fileContent, DT, "lvfront")
        FormatSticsData(fileContent, DT, "longsperac", 3)
        FormatSticsData(fileContent, DT, "codazorac")
        FormatSticsData(fileContent, DT, "minefnra")
        FormatSticsData(fileContent, DT, "minazorac")
        FormatSticsData(fileContent, DT, "maxazorac")
        FormatSticsData(fileContent, DT, "codtrophrac")
        FormatSticsData(fileContent, DT, "repracpermax")
        FormatSticsData(fileContent, DT, "repracpermin")
        FormatSticsData(fileContent, DT, "krepracperm")
        FormatSticsData(fileContent, DT, "repracseumax")
        FormatSticsData(fileContent, DT, "repracseumin")
        FormatSticsData(fileContent, DT, "krepracseu")
        FormatSticsData(fileContent, DT, "tletale")
        FormatSticsData(fileContent, DT, "tdebgel")
        FormatSticsData(fileContent, DT, "codgellev")
        FormatSticsData(fileContent, DT, "nbfgellev")
        FormatSticsData(fileContent, DT, "tgellev10")
        FormatSticsData(fileContent, DT, "tgellev90")
        FormatSticsData(fileContent, DT, "codgeljuv")
        FormatSticsData(fileContent, DT, "tgeljuv10")
        FormatSticsData(fileContent, DT, "tgeljuv90")
        FormatSticsData(fileContent, DT, "codgelveg")
        FormatSticsData(fileContent, DT, "tgelveg10")
        FormatSticsData(fileContent, DT, "tgelveg90")
        FormatSticsData(fileContent, DT, "codgelflo")
        FormatSticsData(fileContent, DT, "tgelflo10")
        FormatSticsData(fileContent, DT, "tgelflo90")
        FormatSticsData(fileContent, DT, "psisto")
        FormatSticsData(fileContent, DT, "psiturg")
        FormatSticsData(fileContent, DT, "h2ofeuilverte")
        FormatSticsData(fileContent, DT, "h2ofeuiljaune")
        FormatSticsData(fileContent, DT, "h2otigestruc")
        FormatSticsData(fileContent, DT, "h2oreserve")
        FormatSticsData(fileContent, DT, "h2ofrvert")
        FormatSticsData(fileContent, DT, "deshydbase")
        FormatSticsData(fileContent, DT, "tempdeshyd")
        FormatSticsData(fileContent, DT, "codebeso")
        FormatSticsData(fileContent, DT, "kmax")
        FormatSticsData(fileContent, DT, "rsmin")
        FormatSticsData(fileContent, DT, "codeintercept")
        FormatSticsData(fileContent, DT, "mouillabil")
        FormatSticsData(fileContent, DT, "stemflowmax")
        FormatSticsData(fileContent, DT, "kstemflow")
        FormatSticsData(fileContent, DT, "Vmax1")
        FormatSticsData(fileContent, DT, "Kmabs1", 3)
        FormatSticsData(fileContent, DT, "Vmax2")
        FormatSticsData(fileContent, DT, "Kmabs2", 3)
        FormatSticsData(fileContent, DT, "adil")
        FormatSticsData(fileContent, DT, "bdil")
        FormatSticsData(fileContent, DT, "masecNmax")
        FormatSticsData(fileContent, DT, "INNmin")
        FormatSticsData(fileContent, DT, "INNimin")
        FormatSticsData(fileContent, DT, "inngrain1")
        FormatSticsData(fileContent, DT, "inngrain2")
        FormatSticsData(fileContent, DT, "bdilmax")
        FormatSticsData(fileContent, DT, "codeplisoleN")
        FormatSticsData(fileContent, DT, "adilmax")
        FormatSticsData(fileContent, DT, "Nmeta")
        FormatSticsData(fileContent, DT, "masecmeta")
        FormatSticsData(fileContent, DT, "Nreserve")
        FormatSticsData(fileContent, DT, "codeINN")
        FormatSticsData(fileContent, DT, "codelegume")
        FormatSticsData(fileContent, DT, "stlevdno")
        FormatSticsData(fileContent, DT, "stdnofno")
        FormatSticsData(fileContent, DT, "stfnofvino")
        FormatSticsData(fileContent, DT, "vitno")
        FormatSticsData(fileContent, DT, "profnod")
        FormatSticsData(fileContent, DT, "concNnodseuil")
        FormatSticsData(fileContent, DT, "concNrac0")
        FormatSticsData(fileContent, DT, "concNrac100")
        FormatSticsData(fileContent, DT, "tempnod1")
        FormatSticsData(fileContent, DT, "tempnod2")
        FormatSticsData(fileContent, DT, "tempnod3")
        FormatSticsData(fileContent, DT, "tempnod4")
        FormatSticsData(fileContent, DT, "codefixpot")
        FormatSticsData(fileContent, DT, "fixmax")
        FormatSticsData(fileContent, DT, "fixmaxveg")
        FormatSticsData(fileContent, DT, "fixmaxgr")
        FormatSticsData(fileContent, DT, "codazofruit")
        FormatSticsData(fileContent, DT, "stadebbchplt")
        FormatSticsData(fileContent, DT, "stadebbchger")
        FormatSticsData(fileContent, DT, "stadebbchlev")
        FormatSticsData(fileContent, DT, "stadebbchamf")
        FormatSticsData(fileContent, DT, "stadebbchlax")
        FormatSticsData(fileContent, DT, "stadebbchsen")
        FormatSticsData(fileContent, DT, "stadebbchflo")
        FormatSticsData(fileContent, DT, "stadebbchdrp")
        FormatSticsData(fileContent, DT, "stadebbchnou")
        FormatSticsData(fileContent, DT, "stadebbchdebdes")
        FormatSticsData(fileContent, DT, "stadebbchmat")
        FormatSticsData(fileContent, DT, "stadebbchrec")
        FormatSticsData(fileContent, DT, "stadebbchfindorm")
        FormatSticsData(fileContent, DT, "codevar")
        FormatSticsData(fileContent, DT, "stlevamf")
        FormatSticsData(fileContent, DT, "stamflax")
        FormatSticsData(fileContent, DT, "stlevdrp")
        FormatSticsData(fileContent, DT, "stflodrp")
        FormatSticsData(fileContent, DT, "stdrpdes", 2)
        FormatSticsData(fileContent, DT, "pgrainmaxi")
        FormatSticsData(fileContent, DT, "adens", 2)
        FormatSticsData(fileContent, DT, "croirac", 3)
        FormatSticsData(fileContent, DT, "durvieF")
        FormatSticsData(fileContent, DT, "jvc")
        FormatSticsData(fileContent, DT, "sensiphot", 1)
        FormatSticsData(fileContent, DT, "stlaxsen")
        FormatSticsData(fileContent, DT, "stsenlan")
        FormatSticsData(fileContent, DT, "nbgrmax", 1)
        FormatSticsData(fileContent, DT, "stdrpmat")
        FormatSticsData(fileContent, DT, "afruitpot", 1)
        FormatSticsData(fileContent, DT, "dureefruit", 1)

        Try
            ' Export file to specified directory
            WriteFile(DirectoryPath, fileName, fileContent.ToString())
        Catch ex As Exception
            MessageBox.Show("Error during writing file : " + ex.Message)
        End Try
    End Sub

    Public Sub FormatSticsData(ByRef fileContent As StringBuilder, ByRef row As Object, ByVal champ As String, Optional ByVal precision As Integer = 5, Optional ByVal fieldIt As Integer = 0)
        Dim res As String
        Dim typeData As String
        Dim rw() As DataRow
        Dim data As Object
        Dim fieldName As String

        fieldName = champ
        'For repeated fields, build field name 
        If (fieldIt <> 0) Then
            fieldName = Mid(fieldName, 1, fieldName.Length - 1) & "(" & Mid(fieldName, fieldName.Length) & ")"
            'champ = champ + fieldIt.ToString()
        End If

        'fetch data
        rw = row.tables(0).select("Champ='" & champ & "'")
        If rw.Count = 0 Then MsgBox(champ)
        data = rw(0)("dv")
        res = ""
        typeData = data.GetType().ToString()

        'if type is string or int
        If ((typeData = "System.String") Or (typeData = "System.Int32")) Then
            res = data.ToString()
        End If
        'if type is real
        If (typeData = "System.Single") Then
            Dim tmp As Single
            'Convert object to double
            tmp = Convert.ToDouble(data)
            If precision > 0 And precision < 7 Then
                res = FormatNumber(tmp, precision)
            Else
                res = tmp.ToString("0.###e+0", CultureInfo.InvariantCulture)
            End If
        End If
        'if cell is null
        If (typeData = "System.DBNull") Then
            res = ""
        End If
        'Print data in file
        fileContent.Append(fieldName)
        fileContent.AppendLine()
        fileContent.Append(res)
        fileContent.AppendLine()
    End Sub


    Public Overrides Sub Import(DirectoryPath As String, model As String)

    End Sub
End Class



