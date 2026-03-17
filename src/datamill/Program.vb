Imports System
Imports System.IO
Imports System.Text
Imports System.Data
Imports System.Data.Sqlite
Imports System.Configuration
Imports System.Threading
Imports System.Globalization
Imports System.DateTime


Public Module GlobalVariables
    Public storeNumMinSimu As Integer = 0
    Public storeNumMaxSimu As Integer = 0
    Public storeKeyDataN As Integer = 0 'variable containing value of column 'N' to read 
    ' Public RepSource As String = "D:\donneesFA\modelisation\Arise\dataMillArise\AppliDatamill"
    Public dbMasterInput As String = String.Empty
    Public dbModelsDictionary As String = String.Empty
    Public dbCelsius As String = String.Empty
End Module


Module Program

    ' Public Class Test
    '     Inherits System.Data.Sqlite.SqliteConnection # NotInheritable !!!
    ' End Class

    Public Class MessageBox
        Public Shared Sub Show(args As String)
            Console.WriteLine(args)
        End Sub  
    End Class

    Sub Main(args As String())
        Console.WriteLine("Start : "  & DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"))
        
        ' Set Locale
        Thread.CurrentThread.CurrentCulture = New CultureInfo("fr-FR")
        Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator = "."

        ' Get db connections string
        Dim appSettings = ConfigurationManager.AppSettings 'Read Stored Value
        dbMasterInput = appSettings("db_master_input")
        dbModelsDictionary = appSettings("db_models_dictionnary")
        dbCelsius = appSettings("db_celsius")

        Console.WriteLine("dbMasterInput : " + dbMasterInput)
        Console.WriteLine("dbModelsDictionary : " + dbModelsDictionary)
        Console.WriteLine("dbCelsius : " + dbCelsius)

        ' Get the values of the command line in an array
        Dim clArgs() As String = Environment.GetCommandLineArgs()
        Dim cmd As String = String.Empty
        Dim nthreads As Integer = 1
        Dim models As String = String.Empty
        Dim fun As String = String.Empty
        Dim sstart As Integer = 0
        Dim send As Integer = -1
        
        Console.WriteLine("n : " + CStr(clArgs.Count()))
        If clArgs.Count() > 1 Then
            cmd = clArgs(1)
            For i As Integer = 1 To (clArgs.Count()-1) Step 1
                If clArgs(i) = "-m" Then
                    models = clArgs(i + 1)
                End If
                If clArgs(i) = "-dbMasterInput" Then
                    dbMasterInput = "URI=file:" & clArgs(i + 1)
                End If
                If clArgs(i) = "-dbModelsDictionary" Then
                    dbModelsDictionary = "URI=file:" & clArgs(i + 1)
                End If
                If clArgs(i) = "-dbCelsius" Then
                    dbCelsius = "URI=file:" & clArgs(i + 1)
                End If                
                If clArgs(i) = "-t" Then
                    nthreads = clArgs(i + 1)
                End If
                If clArgs(i) = "-fun" Then
                    fun = clArgs(i + 1)
                End If                
                If clArgs(i) = "-sstart" Then
                     sstart = clArgs(i + 1)
                End If                 
                If clArgs(i) = "-send" Then
                     send = clArgs(i + 1)
                End If                                 
            Next
            Console.WriteLine("cmd : " + cmd)
            Console.WriteLine("models : " + models)
            Console.WriteLine("nthreads : " + CStr(nthreads))
            Console.WriteLine("dbMasterInput : " + dbMasterInput)
            Console.WriteLine("dbModelsDictionary : " + dbModelsDictionary)
            Console.WriteLine("dbCelsius : " + dbCelsius)
            Console.WriteLine("sstart : " + CStr(sstart))
            Console.WriteLine("send : " + CStr(send))
        End If


        ' Dim Connection As New SqliteConnection
        ' Connection.ConnectionString = "URI=file:db" & Path.DirectorySeparatorChar & "ModelsDictionaryArise.db"
        ' Connection.Open()
        ' Dim strSQL As String = "ALTER TABLE Variables RENAME COLUMN Table MyTable;"
        ' Dim cmd = New SQLiteCommand(strSQL, Connection)
        ' cmd.ExecuteNonQuery()
        ' cmd.Dispose()
        ' Connection.Close()
        Dim RepSource As String = "." & Path.DirectorySeparatorChar & _
                "DonneesFA" & Path.DirectorySeparatorChar & _
                "modelisation" & Path.DirectorySeparatorChar & _
                "Arise" & Path.DirectorySeparatorChar & _
                "datamillarise" &   Path.DirectorySeparatorChar & _
                "applidatamill"

        If cmd = "convert" Then
            If models = "stics" Then
                Console.WriteLine("SticsConverter...")
                Dim converter As SticsConverter = new SticsConverter()
                converter.setNthreads(nthreads)
                converter.setSstart(sstart)
                converter.setSend(send)
                converter.Export(RepSource & Path.DirectorySeparatorChar & "Stics")
            End If
            If models = "dssat" Then
                Console.WriteLine("DssatConverter...")
                Dim converter As DssatConverter = new DssatConverter()
                converter.setNthreads(nthreads)
                converter.setSstart(sstart)
                converter.setSend(send)
                converter.Export(RepSource & Path.DirectorySeparatorChar & "Dssat")
            End If
            If models = "celsius" Then
                Console.WriteLine("CelsiusConverter...")
                Dim converter As CelsiusConverter = new CelsiusConverter()
                converter.setNthreads(nthreads)
                converter.setSstart(sstart)
                converter.setSend(send)
                converter.Export(RepSource & Path.DirectorySeparatorChar & "Celsius")
            End If                      
        End If

        If cmd = "compute" Then
            If fun = "etp" Then
                Dim tools As Tools = new Tools
                tools.setNthreads(nthreads)
                tools.computeEtp()
            End If
        End If
        Console.WriteLine("End : "  & DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"))
    End Sub
End Module
