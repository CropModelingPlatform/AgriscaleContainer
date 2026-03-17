Imports System
Imports System.IO
Imports System.Text
Imports System.Globalization
Imports System.Data
Imports System.Math
Imports System.Data.Sqlite
Imports System.Data.SqlClient

Public Class Tools

  Protected nthreads As Integer

  Public Function Ra(lat, J) As Double
  'returns Solar radiation at the top of the atmosphere as a Public Function of day of the year and latitude
  ' ra in Mj/m2/day
      Dim Dr As Double, Declin As Double, SolarAngle As Double
      Dim latrad As Double
      Const pi = 3.14159265
      latrad = pi * lat / 180
      Dr = 1 + 0.033 * Cos(2 * pi * J / 365)
      Declin = 0.409 * Sin((2 * pi * J / 365) - 1.39)
      SolarAngle = Tan(latrad) * Tan(Declin)
      SolarAngle = -Atan(-SolarAngle / Sqrt(-SolarAngle * SolarAngle + 1)) + 2 * Atan(1)
      Ra = (SolarAngle * Sin(latrad) * Sin(Declin)) + (Cos(latrad) * Cos(Declin) * Sin(SolarAngle))
      Ra = 24 * 60 * 0.082 * Dr * Ra / pi
  End Function

  Public Function ET0pm_Tdew(lat, Alt, J, Tn, Tx, Tm, Tdewn, Tdewx, Vm, Rg) As Double
    'Calculates ET0 according to FAO Penman-Monteith Equation (Bull.FAO#56)
    'altitude Alt in m,
    'J in number of the day in the year
    'Tn, Tx and Tm respectively minimal,maximal and mean daily temperature in degrees C, Tx, Tm,
    'Tdewn, Tdewx, respectively minimal and maximal dewpoint temperature in degrees C
    'Vm,average wind distance perr day  in km,
    'Rg global radiation in MJ/m2/day
    ' All variables assumed measured at 2m above soil

    Dim adv As Double, Rad As Double, gamma As Double, E0SatTn As Double, E0SatTx As Double, SlopeSat As Double, Ea As Double
    Dim VPD As Double, Rso As Double, Rns As Double, Rnl As Double

    Const sigma = 0.000000004903
    'jeu test (exemple du bull FAO56)
    'lat = 50.8
    'Alt = 100
    'Tn = 12.3
    'Tx = 21.5
    'Tm = 16.9
    'Ux = 84
    'Un = 63
    'Vm = 2.078
    'Rg = 22.07
    'J = 188
    'fin jeu test
    'terme advectif
    If (lat + Alt + J + Tn + Tx + Tm + Tdewn + Tdewx + Vm + Rg) Is Nothing Then
        ET0pm_Tdew = Nothing
    Else
      gamma = 101.3 * ((293 - 0.0065 * (Alt)) / 293) ^ 5.26
      gamma = 0.000665 * gamma
      E0SatTn = 0.6108 * Exp(17.27 * Tn / (Tn + 237.3))
      E0SatTx = 0.6108 * Exp(17.27 * Tx / (Tx + 237.3))
      SlopeSat = 4098 * (0.6108 * Exp(17.27 * Tm / (Tm + 237.3))) / ((Tm + 237.3) ^ 2)
      'Ea = 0.5 * ((E0SatTn * Ux) + (E0SatTx * Un))
      ' remplacé par ligne suivante puisque Tdewpoint
      Ea = 0.5 * 0.6108 * (Exp(17.27 * Tdewx / (Tdewx + 237.3)) + Exp(17.27 * Tdewn / (Tdewn + 237.3)))
      VPD = ((E0SatTn + E0SatTx) / 2) - Ea
      adv = gamma * 900 * Vm * VPD / (Tm + 273)
    'terme radiatif
          'shortwave
      Rso = (0.00002 * Alt + 0.75) * Ra(lat, J)
      Rns = (1 - 0.23) * Rg
          'longwave
      Rnl = Rg / Rso
      If Rnl > 1 Then Rnl = 1
      Rnl = (Rnl * 1.35 - 0.35) * (-0.14 * Sqrt(Ea) + 0.34)
      Tn = Tn + 273.16
      Tx = Tx + 273.16
      Rnl = sigma * (Tn ^ 4 + Tx ^ 4) * Rnl / 2
            'radiation balance assuming soil heat flux is 0 at a day time step
      Rad = 0.408 * SlopeSat * (Rns - Rnl)
    '        ajout des deux termes
      ET0pm_Tdew = (Rad + adv) / (SlopeSat + gamma * (0.34 * Vm + 1))
    End If

    return ET0pm_Tdew
  End Function

  Public Function getCoordinates(idPoint As String, MasterInput_Connection As SqliteConnection) As DataRow
    Dim fetchAllQuery As String
    Dim dataSet As New DataSet()
    Dim dataTable As New DataTable()
    Dim arow as DataRow

    fetchAllQuery = "select altitude, latitudeDD from Coordinates where idPoint='" + idPoint + "';"
    'Init and use DataAdapter
    Using dataAdapter As SqliteDataAdapter = New SqliteDataAdapter(fetchAllQuery, MasterInput_Connection)
      ' Filling Dataset
      dataSet = New DataSet
      dataSet.Clear()
      dataTable.Clear()
      dataAdapter.Fill(dataSet, "st_coord")
      dataTable = dataSet.Tables("st_coord")
      arow = dataTable.Rows(0)
    End Using

    return arow
  End Function

  Sub computeEtp()
    Dim MasterInput_Connection As SqliteConnection = New SqliteConnection()
    MasterInput_Connection.ConnectionString = GlobalVariables.dbMasterInput
    Try
        MasterInput_Connection.Open()
    Catch ex As Exception
        Console.WriteLine("Connection Error : "  & ex.Message)
    End Try

    'weather_site query
    Dim Q1 As String = "select * from RaClimateD;"

    'Init and use DataAdapter
    Dim DASL As SqliteDataAdapter = New SqliteDataAdapter(Q1, MasterInput_Connection)
    ' Dim cmdBuilder As SQLiteCommandBuilder
    ' cmdBuilder = New SQLiteCommandBuilder(DASL)
    Dim DS As New DataSet()
    DASL.Fill(DS, "Stics_SL")
    Dim DT As DataTable = DS.Tables("Stics_SL")
    Dim column As DataColumn = DT.Columns("Etppm")
    Dim sql_cmd As String = ""
    Dim results() As String = new String(40000){}
    '******************************************
    'For Each row1 In DT.Rows
    Dim myOptions As ParallelOptions = New ParallelOptions()
    myOptions.MaxDegreeOfParallelism = nthreads

    Console.WriteLine("******************************************")
    Console.WriteLine("ETP compute loop...")
    Console.WriteLine("number of ETP to compute : " & CStr(DT.Rows.Count))
    Console.WriteLine("number of threads : " & CStr(nthreads))
    Console.WriteLine("******************************************")
        'System.Threading.Tasks.Parallel.For(0, DT.Rows.Count, myOptions,
        Dim result = System.Threading.Tasks.Parallel.For(0, 40000, myOptions,
            sub(i)
                'Console.WriteLine("Iteration " + CStr(i))
                Dim row1 as DataRow = DT.Rows(i)
                Dim Site As String = row1.item("IdPoint")
                Dim Alt As Double = Nothing
                Dim lat As Double = Nothing
                'Dim tools As Tools = new Tools
                Dim arow As DataRow = getCoordinates(Site, MasterInput_Connection)
                Alt = arow.item("altitude")
                lat = arow.item("latitudeDD")
                'Console.WriteLine("Alt : " + CStr(Alt))
                'Console.WriteLine("latitudeDD : " + CStr(lat))
                
                Dim J as Double = row1.item("DOY")
                Dim Tn as Double = row1.item("tmin")
                Dim Tx as Double = row1.item("tmax")
                Dim Tm as Double = row1.item("tmoy")
                Dim Tdewn as Double = row1.item("Tdewmin")
                Dim Tdewx as Double = row1.item("Tdewmax")
                Dim Vm as Double = row1.item("wind")
                Dim Rg as Double = row1.item("srad")
                ' Dim Etppm as Double = row1.item("Etppm")
                Dim etp as Double = ET0pm_Tdew(lat, Alt, J, Tn, Tx, Tm, Tdewn, Tdewx, Vm, Rg)

                'Console.WriteLine("ETP for " + CStr(Site))

                Dim w_date as DateTime = row1.item("w_date")
                'Console.WriteLine("w_date : " & CStr(w_date))

                'Dim sql_str As String = "('" & row1.item("IdPoint") & "', '" & w_date.ToString("yyyy-MM-dd") & "', " & etp & ")"
                Dim sql_str As String = "UPDATE RAClimateD SET Etppm=" & etp &
                      " WHERE IdPoint='" & row1.item("IdPoint") & "'" & _
                      " AND w_date='"  & w_date.ToString("yyyy-MM-dd") & "';"
                ''" AND w_date='"  & w_date.ToString("yyyy-MM-dd 00:00:00.000") & "';"

                ' Console.WriteLine("SQL UPDATE : " & cmd.CommandText)
                results(i) = sql_str
            End Sub
        )
    Console.WriteLine("Result: " & CStr(result.IsCompleted))
    Dim n As Integer = 0
    For i As Integer = 0 To results.Length
        If n = 1000 or n = results.Length Then
            Console.WriteLine("SQL UPDATE " & CStr(i))
            Dim s() as String = results.Skip((i-n)).Take(n).ToArray
                Dim sql_str As String = Join(s, "")
                Using cmd As New SQLiteCommand(MasterInput_Connection)
            Using transaction = MasterInput_Connection.BeginTransaction()
                ' cmd.Transaction = transaction
                cmd.CommandText = sql_str
                cmd.ExecuteNonQuery()
                transaction.Commit()
            End Using
            End Using
            n = 0
        Else
            n = n + 1
        End If
    Next
    ' Dim vals As String = Join(results, ",")

    ' sql_cmd = "INSERT INTO EtppmTmp (IdPoint, w_date, etppm) VALUES " & vals.SubString(0, vals.Length - 1)
    ' sql_cmd = sql_cmd & "; UPDATE RAClimateD
    ' SET etppm = (SELECT etppm FROM EtppmTmp
    ' WHERE idPoint = RAClimateD.idPoint AND w_date = RAClimateD.w_date)"
    ' Console.WriteLine(sql_cmd)
    ' Console.WriteLine("SQL INSERT : ")
    ' Using cmd As New SQLiteCommand(MasterInput_Connection)
    ' Using transaction = MasterInput_Connection.BeginTransaction()
    '    ' cmd.Transaction = transaction
    '    cmd.CommandText = sql_cmd
    '    cmd.ExecuteNonQuery()
    '    transaction.Commit()
    'End Using
    'End Using

    Try
        MasterInput_Connection.Close()
    Catch ex As Exception
        MessageBox.Show("Connection Error : "  + ex.Message)
    End Try
    Console.WriteLine("ETP COMPUTED!")
  End Sub  

  Public Function computeOneEtp(MasterInput_Connection As SqliteConnection, arow as DataRow)
    Dim dr As DataRow = getCoordinates(arow.item("idPoint"), MasterInput_Connection)
    Dim Alt as Double = dr.item("altitude")
    Dim lat as Double = dr.item("latitudeDD")

    Dim J as Double = arow.item("DOY")
    Dim Tn as Double = arow.item("tmin")
    Dim Tx as Double = arow.item("tmax")
    Dim Tm as Double = arow.item("tmoy")
    Dim Tdewn as Double = arow.item("Tdewmin")
    Dim Tdewx as Double = arow.item("Tdewmax")
    Dim Vm as Double = arow.item("wind")
    Dim Rg as Double = arow.item("srad")

    Dim etp as Double = ET0pm_Tdew(lat, Alt, J, Tn, Tx, Tm, Tdewn, Tdewx, Vm, Rg)

    return etp

  End Function

  Public Sub setNthreads(ByVal number_of_threads As Integer)
    nthreads = number_of_threads
  End Sub

End Class