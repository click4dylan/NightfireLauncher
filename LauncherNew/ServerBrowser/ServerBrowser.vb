Public Class ServerBrowser
    Public Shared AutoRefreshServerList As Boolean = True
    Public Shared EnteredServerPassword As Boolean = False
    Public Shared ServerPassword As String = ""
    Public startuprefresh As New Threading.Thread(AddressOf RefreshServers)
    Private ExitMe As Boolean = False
    Private Sub CloseMe()
        While Not ExitMe
            If ExitGlobally Then Exit While
            Threading.Thread.Sleep(100)
        End While
        If Me.InvokeRequired Then
            On Error GoTo Failed
            Me.Invoke(New MethodInvoker(AddressOf CloseMe))
            Exit Sub
        End If
Failed:
        Me.Close()
    End Sub
    Private Sub ServerBrowser_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
        ExitMe = False
        Dim ExitThread As New Threading.Thread(AddressOf CloseMe)
        ExitThread.Start()
        If AutoRefreshServerList Then startuprefresh.Start()
    End Sub
    Private ServerBrowserDebugLabel As String = ""
    Private RefreshButtonEnabled As Boolean = True
    Private ServerListContainer As New ListView
    Private Sub ServerBrowser_Paint(sender As Object, e As System.Windows.Forms.PaintEventArgs) Handles Me.Paint
        notifylabel.Text = ServerBrowserDebugLabel
        Refreshh.Enabled = RefreshButtonEnabled
        ServerList.Items.Clear()
        For Each itm As ListViewItem In ServerListContainer.Items
            Dim newLvi As ListViewItem = itm.Clone()
            For Each itmsubitem As ListViewItem.ListViewSubItem In itm.SubItems
                newLvi.SubItems.Add(New ListViewItem.ListViewSubItem(newLvi, itmsubitem.Text, itmsubitem.ForeColor, itmsubitem.BackColor, itmsubitem.Font))
            Next
            ServerList.Items.Add(newLvi)
        Next
    End Sub
    Private Sub ServerBrowser_FormClosed(sender As Object, e As System.Windows.Forms.FormClosedEventArgs) Handles Me.FormClosed
        ExitMe = True
        If AutoRefreshServerList = True Then
            ExitGlobally = True
        Else
            If Not SplashScreen.Opacity = 100 Then
                SplashScreen.Opacity = 100
                SplashScreen.Activate()
                Application.DoEvents()
            End If
        End If
    End Sub
    Private Sub Refresh_Click(sender As System.Object, e As System.EventArgs) Handles Refreshh.Click
        startuprefresh = New Threading.Thread(AddressOf RefreshServers)
        startuprefresh.Start()
    End Sub
    Private Sub addserver_Click(sender As Object, e As EventArgs) Handles addserver.Click
        ShowForm(AddAServer)
    End Sub
    Private Sub Connect_btn_Click(sender As System.Object, e As System.EventArgs) Handles Connect_btn.Click
        JoinServer()
    End Sub
    Private Sub DoubleClickServer(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles ServerList.MouseDoubleClick
        JoinServer()
    End Sub
    Public Function ReceivePacket(ByVal client As Net.Sockets.UdpClient, ByRef endpoint As Net.IPEndPoint) As String
        client.Client.ReceiveTimeout = 1000
        Dim a As String = ""
        Try
            a = System.Text.Encoding.ASCII.GetString(client.Receive(endpoint))
        Catch ex As Exception
            If Not ex.Message = "A connection attempt failed because the connected party did not properly respond after a period of time, or established connection failed because connected host has failed to respond" Then
                Console.WriteLine("ERROR: An error occured on the ReceivePacket function. The message contained:")
                Console.WriteLine(ex.Message)
            End If
        End Try
        Return a
    End Function
    Public Sub SendPacket(ByVal host As String, ByVal port As Integer, ByVal data As Byte(), ByVal client As Net.Sockets.UdpClient)
        client.Send(data, data.Length, host, port)
    End Sub
#If DEBUG Then
    'Public masterip = "96.254.179.222"
    Public masterip = "master.nightfirepc.com"
#Else
        Public masterip = "master.nightfirepc.com"
#End If
    Public PasswordedServersList As New List(Of String)
    Private Sub RefreshServers()
        Dim retried As Byte = 0
RESTARTDUETOFAILURE:
        ServerBrowserDebugLabel = "Contacting master.nightfirepc.com"
        Application.DoEvents()
        Me.Invalidate()
        Dim listofservers As New Dictionary(Of Net.IPEndPoint, String) 'IP:Queryport, Hostport
        RefreshButtonEnabled = False
        PasswordedServersList.Clear()
        ServerListContainer.Items.Clear()
        Me.Invalidate()
        'Dim Servers As New List(Of _IPPort)

        Dim tcp As New Net.Sockets.TcpClient
        Try
            tcp.Connect(masterip, 28900)
        Catch ex As Exception
            RefreshButtonEnabled = True
            Exit Sub
        End Try

        Dim timeout As Integer = 10
        Do
            System.Threading.Thread.Sleep(100)
            timeout -= 1
        Loop Until tcp.Available > 0 Or timeout = 0
        If timeout = 0 Then
            ServerBrowserDebugLabel = "Failed to contact master.nightfirepc.com"
            Application.DoEvents()
            Me.Invalidate()
            Threading.Thread.Sleep(300)
            RefreshButtonEnabled = True
            Exit Sub
        End If
        Dim response As String = ""
        While (tcp.Available > 0)
            response = response & Chr(tcp.GetStream.ReadByte)
        End While
        Dim chrs() As Char = {"\", Chr(0)}
        Dim spl() As String = response.Split(chrs, StringSplitOptions.RemoveEmptyEntries)

        Dim RequestCode As String = gsseckey(System.Text.Encoding.ASCII.GetBytes(spl(spl.Length - 1)), 2, "S9j3L2")

        Dim packet() As Byte = System.Text.Encoding.ASCII.GetBytes("\basic\gamename\jbnightfire\enctype\2\location\0\validate\" & RequestCode & "\final\list\\gamename\jbnightfire\final\")
        tcp.GetStream.Write(packet, 0, packet.Length)
        Do
            System.Threading.Thread.Sleep(100)
            timeout -= 1
        Loop Until tcp.Available > 0 Or timeout = 0

        If timeout = 0 Then
            ServerBrowserDebugLabel = "Failed to contact master.nightfirepc.com"
            Application.DoEvents()
            Me.Invalidate()
            RefreshButtonEnabled = True
            Exit Sub
        End If

        System.Threading.Thread.Sleep(100)
        Dim buff(500000) As Byte
        Dim idn As Integer = 0
        ServerBrowserDebugLabel = "Decrypting server list from " & masterip
        Application.DoEvents()
        Me.Invalidate()
        While (tcp.Available > 0)
            buff(idn) = tcp.GetStream.ReadByte
            If idn Mod 120 = 0 Then
                System.Threading.Thread.Sleep(1)
            End If
            idn += 1
        End While
        tcp.Close()
        Dim ip As String = enctype2_decoder(buff, idn, "S9j3L2")
#If 0 Then
            While Not ip.StartsWith("\final\")
                ip = ip.Remove(0, 1) 'remove all the ips
            End While
            ip = ip.Remove(0, 7) 'remove \final\
            If ip.Length > 0 Then
                Dim stringofbytes As String = ""
                For Each a In System.Text.Encoding.ASCII.GetBytes(ip)
                    If a = 0 Then
                        stringofbytes = stringofbytes & " 00" 'replace 0 with 00 so it's cleaner
                    Else
                        stringofbytes = stringofbytes & " " & a
                    End If
                Next
                stringofbytes = stringofbytes.Remove(0, 1) 'remove beginning space
                ServerBrowserDebugLabel = ip.Length & " bytes of junk: " & ControlChars.Quote & stringofbytes & ControlChars.Quote & " ASCII:" & ip
            End If
            RefreshButtonEnabled = True
            ServerBrowserDebugLabel = "Success: No debug errors to report with the IP list!"
            Exit Sub
#End If
        While Not ip.EndsWith("\final\")
            ip = ip.Substring(0, ip.Length - 1)
        End While

        Dim splt() As String = ip.Split("\")
        Dim nextip As Boolean = False
        For i As Integer = 0 To splt.Length - 1
            If Not nextip Then
                If splt(i) = "ip" Then
                    nextip = True
                End If
            Else
                Dim ipport() As String = splt(i).Split(":")
                Dim ipqueryport As New System.Net.IPEndPoint(System.Net.IPAddress.Parse(ipport(0)), ipport(1))
                listofservers.Add(ipqueryport, ipqueryport.Port)
                nextip = False
            End If
        Next

        Dim udp As Net.Sockets.UdpClient
        Try
            udp = New Net.Sockets.UdpClient
        Catch ex As Exception
            ServerBrowserDebugLabel = "ERROR: Failed to open UDP socket"
            Application.DoEvents()
            Me.Invalidate()
            RefreshButtonEnabled = True
            'udp.Close()
            Exit Sub
        End Try


        Dim receivedpacket As String

        'Send query to all servers
        Dim svquerystate As New Dictionary(Of String, String)
        Dim retry As Byte = 0

Retry:

        For Each sv In listofservers
            Dim ip2 As New System.Net.IPEndPoint(sv.Key.Address, sv.Key.Port)
            ServerBrowserDebugLabel = "Querying " & ip2.Address.ToString & ":" & ip2.Port.ToString
            Application.DoEvents()
            Me.Invalidate()
            SendPacket(ip2.Address.ToString, ip2.Port, System.Text.Encoding.ASCII.GetBytes("\status\"), udp)
            svquerystate.Add(ip2.Address.ToString & ":" & ip2.Port, "sent:" & Environment.TickCount)
        Next

        'Check for a reply
        For Each sv In listofservers
            Dim ip2 As System.Net.IPEndPoint = New Net.IPEndPoint(0, 0) ' what was the fix to this one xd
            udp.Client.ReceiveTimeout = 1000
            ServerBrowserDebugLabel = "Retrieving packet from " & sv.Key.Address.ToString & ":" & sv.Key.Port.ToString
            Me.Invalidate()
            receivedpacket = ReceivePacket(udp, ip2)
            If receivedpacket = "" Then
                ServerBrowserDebugLabel = "Server not responding " & sv.Key.Address.ToString & ":" & sv.Key.Port.ToString
                Me.Invalidate()
                Threading.Thread.Sleep(500)
            Else
                ServerBrowserDebugLabel = "Received response from " & sv.Key.Address.ToString & ":" & sv.Key.Port.ToString
                Me.Invalidate()
            End If
            Dim key = ip2.Address.ToString & ":" & ip2.Port
            If svquerystate.ContainsKey(key) Then
                Dim current = svquerystate(key).Split(":")
                Dim difference = Environment.TickCount - current(1)
                If difference > 10000 Or difference = 0 AndAlso retry < 3 Then
                    ServerListContainer.Items.Clear()
                    svquerystate.Clear()
                    Me.Invalidate()
                    udp.Close()
                    ServerBrowserDebugLabel = "Retrieved results were inaccurate, retrying"
                    Threading.Thread.Sleep(500)
                    udp = New Net.Sockets.UdpClient

                    If Not retry > 3 Then
                        retry += 1 'this is retrying the whole list while the one below is retrying the bottom one is retrying only a portion that failed
                        GoTo Retry
                    Else
                        ServerBrowserDebugLabel = "Failed to refresh, please try again"
                    End If

                End If

                svquerystate(key) = "received:" & difference
                AddToList(receivedpacket, svquerystate(key), ip2)
                Me.Invalidate()
                Application.DoEvents()
            End If
        Next

        udp.Close()
        udp = New Net.Sockets.UdpClient

        For Each sv In svquerystate
            Dim split = sv.Key.Split(":")
            Dim address = split(0)
            Dim port = split(1)
            listofservers.Remove(New Net.IPEndPoint(Net.IPAddress.Parse(address), port))
        Next

        If listofservers.Count > 0 Then
            svquerystate.Clear()
            If Not retry > 3 Then
                retry += 1
                GoTo Retry
            End If
        End If

        ServerBrowserDebugLabel = "Finished querying " & masterip ' CRASH HERE IF FORM IS CLOSED, FIX LATER
        RefreshButtonEnabled = True
        Me.Invalidate()
        Application.DoEvents()
    End Sub
    Public Sub AddToList(ByVal packet As String, ByVal state As String, ByVal ipport As Net.IPEndPoint)
        Dim ping = state.Split(":")(1)

        If packet.StartsWith("\gamename\") Then
            Dim serverstatus As New SortedDictionary(Of String, String)
            packet = packet.Replace("\gamename\", "gamename\")
            Dim spl1 = packet.Split("\")
            Dim temp As String = ""
            Dim temp2 As String = ""
            For i As Integer = 0 To spl1.Length - 1
                If temp.Length = 0 Then
                    temp = spl1(i)
                Else
                    temp2 = spl1(i)
                    serverstatus.Add(temp, temp2)
                    temp = ""
                End If
            Next
            Dim str(6) As String
            serverstatus.TryGetValue("hostname", str(0))
            serverstatus.TryGetValue("mapname", str(3))
            serverstatus.TryGetValue("gamever", str(4))
            Dim hostport As String = "26015"
            serverstatus.TryGetValue("hostport", hostport)
            Dim numplayers As String = "0"
            serverstatus.TryGetValue("numplayers", numplayers)
            Dim maxplayers As String = "32"
            serverstatus.TryGetValue("maxplayers", maxplayers)
            Dim passworded As String = "0"
            serverstatus.TryGetValue("password", passworded)

            str(1) = ping
            str(2) = numplayers & "/" & maxplayers
            str(5) = ipport.Address.ToString & ":" & hostport
            str(6) = ipport.Port.ToString
            Dim itm As New ListViewItem(str)
            If Not ServerListContainer.Items.Contains(itm) Then
                ServerListContainer.Items.Add(itm)
                If passworded = "1" Then PasswordedServersList.Add(str(5))
                Me.Invalidate()
            End If
        End If
    End Sub
    Private Sub JoinServer()
        AutoRefreshServerList = False
        If ServerListContainer.Items.Count = 0 Then Exit Sub
        If ServerList.SelectedItems.Count = 0 Then Exit Sub
        Dim selecteditem As ListViewItem = ServerList.SelectedItems(0)
        Dim ip As String = selecteditem.SubItems(5).Text
        If PasswordedServersList.Contains(ip) Then
            ShowForm(InputServerPassword)
        Else
            EnteredServerPassword = True
        End If
        While InputServerPassword.Visible = True
            Application.DoEvents()
            Threading.Thread.Sleep(1)
        End While
        If ServerPassword.Length > 0 Then
            Globals.ConnectArg = " +password " & ServerPassword
        End If
        If EnteredServerPassword Then
            EnteredServerPassword = False
            Globals.ConnectArg = Globals.ConnectArg & " +connect " & ControlChars.Quote & ip & ControlChars.Quote
            AutoRefreshServerList = False
            Application.DoEvents()
            Me.Close()
        End If
    End Sub
    'Gamespy-Specific Functions Below
    Private Function gsseckey(ByRef SecureKey() As Byte, ByVal enctype As Integer, ByVal handoff As String) As String

        Dim gamekey As String = ""

        Dim Temp(3) As Integer
        Dim Table(255) As Byte
        Dim Length(1) As Integer
        Dim Key(5) As Byte
        Dim i As Integer

        Dim enctype1_data() As Byte = {1, 186, 250, 178, 81, 0, 84, 128, 117, 22, 142, 142, 2, 8, 54, 165, 45, 5, 13, 22, 82, 7, 180, 34, 140, 233, 9, 214, 185, 38, 0, 4, 6, 5, 0, 19, 24, 196, 30, 91, 29, 118, 116, 252, 80, 81, 6, 22, 0, 81, 40, 0, 4, 10, 41, 120, 81, 0, 1, 17, 82, 22, 6, 74, 32, 132, 1, 162, 30, 22, 71, 22, 50, 81, 154, 196, 3, 42, 115, 225, 45, 79, 24, 75, 147, 76, 15, 57, 10, 0, 4, 192, 18, 12, 154, 94, 2, 179, 24, 184, 7, 12, 205, 33, 5, 192, 169, 65, 67, 4, 60, 82, 117, 236, 152, 128, 29, 8, 2, 29, 88, 132, 1, 78, 59, 106, 83, 122, 85, 86, 87, 30, 127, 236, 184, 173, 0, 112, 31, 130, 216, 252, 151, 139, 240, 131, 254, 14, 118, 3, 190, 57, 41, 119, 48, 224, 43, 255, 183, 158, 1, 4, 248, 1, 14, 232, 83, 255, 148, 12, 178, 69, 158, 10, 199, 6, 24, 1, 100, 176, 3, 152, 1, 235, 2, 176, 1, 180, 18, 73, 7, 31, 95, 94, 93, 160, 79, 91, 160, 90, 89, 88, 207, 82, 84, 208, 184, 52, 2, 252, 14, 66, 41, 184, 218, 0, 186, 177, 240, 18, 253, 35, 174, 182, 69, 169, 187, 6, 184, 136, 20, 36, 169, 0, 20, 203, 36, 18, 174, 204, 87, 86, 238, 253, 8, 48, 217, 253, 139, 62, 10, 132, 70, 250, 119, 184}

        ' 1) buffer creation with incremental data 

        For i = 0 To 255
            Table(i) = i
        Next
        Length(0) = handoff.Length
        Length(1) = SecureKey.Length

        ' 2) buffer scrambled with key 

        For i = 0 To 255
            Temp(0) = (Temp(0) + Table(i) + AscW(handoff(i Mod 6))) And 255
            Temp(2) = Table(Temp(0))
            Table(Temp(0)) = Table(i)
            Table(i) = Temp(2)
        Next

        Temp(0) = 0
        Dim keyredim As Integer = Length(1) - 1
        Length(1) = Length(1) / 3

        ' 3) source string scrambled with the buffer

        For i = 0 To keyredim
            Key(i) = SecureKey(i)

            Temp(0) = (Temp(0) + Key(i) + 1) And 255
            Temp(2) = Table(Temp(0))

            Temp(1) = (Temp(1) + Temp(2)) And 255
            Temp(3) = Table(Temp(1))

            Table(Temp(1)) = Temp(2)
            Table(Temp(0)) = Temp(3)
            Key(i) = (Key(i) Xor Table((Temp(2) + Temp(3)) And 255))

        Next

        If enctype = 1 Then
            For i = 0 To 5
                Key(i) = enctype1_data(Key(i))
            Next i
        ElseIf enctype = 2 Then
            For i = 0 To 5
                Key(i) = Key(i) Xor AscW(handoff(i Mod 6))
            Next i
        End If

        ' 5) splitting of the source string from 3 to 4 bytes 

        Dim str As String = ""
        Dim j As Integer = 0
        While (Length(1) >= 1)
            Length(1) = Length(1) - 1

            Temp(2) = Key(j)
            Temp(3) = Key(j + 1)

            str = str & gsvalfunc(Temp(2) >> 2)
            str = str & gsvalfunc(((Temp(2) And 3) << 4) Or Temp(3) >> 4)

            Temp(2) = Key(j + 2)

            str = str & gsvalfunc(((Temp(3) And 15) << 2) Or (Temp(2) >> 6))
            str = str & gsvalfunc(Temp(2) And 63)

            j += 3
        End While

        Return str
    End Function
    Private Function gsvalfunc(ByVal number As Integer) As Char
        Dim newChar As Char

        Select Case number
            Case Is < 26
                newChar = Chr(number + 65)
            Case Is < 52
                newChar = Chr(number + 71)
            Case Is < 62
                newChar = Chr(number - 4)
            Case 62
                newChar = "+"
            Case 63
                newChar = "/"
        End Select

        Return newChar
    End Function
    Function enctype2_decoder(ByVal data() As Byte, ByVal datalength As Integer, ByVal key As String) As String

        Dim dest(325) As UInteger
        For i As Integer = 256 To 325
            dest(i) = 0
        Next
        data(0) = data(0) Xor 236
        For i As Integer = 0 To key.Length - 1
            data(i + 1) = data(i + 1) Xor AscW(key(i))
        Next
        dest = encshare4(data, dest)

        Dim datap(datalength - 2 - data(0)) As Byte

        For i As Integer = 0 To datalength - data(0) - 2
            datap(i) = data(data(0) + i + 1)
        Next
        If datap.Length < 6 Then
            Return System.Text.Encoding.ASCII.GetString(data)
        End If

        datap = encshare1(dest, datap, datap.Length)

        Dim lal As String = System.Text.Encoding.ASCII.GetString(datap)
        Return lal
    End Function
    Dim p_ind As Integer
    Function encshare1(ByVal tbuff() As UInteger, ByVal datap() As Byte, ByVal len As Integer) As Byte()
        p_ind = 309
        Dim s_ind As Integer = 309

        Dim datap_ind As Integer = 0
        Dim lalind As Integer = 309

        Dim bytepart As Integer = 4
        Dim ByteArray(3) As Byte
        While (len > 0)
            If (datap_ind Mod 63 = 0) Then
                p_ind = s_ind
                lalind = 309
                bytepart = 4
                tbuff = encshare2(tbuff, 16)
            End If

            If bytepart > 3 Then
                Dim t As UInteger = tbuff(lalind)
                ByteArray = BitConverter.GetBytes(t)
                bytepart = 0
                lalind += 1
            End If
            datap(datap_ind) = (datap(datap_ind) Xor ByteArray(bytepart)) Mod 256
            datap_ind += 1
            p_ind += 1
            bytepart += 1
            len -= 1
        End While

        Return datap
    End Function
    Function SumaOverF(ByVal a As ULong, ByVal b As ULong) As UInteger
        Dim res As ULong = a + b
        res = res Mod 4294967296
        Return res
    End Function
    Function encshare3(ByVal data() As UInteger, ByVal n1 As Integer, ByVal n2 As Integer)
        Dim t1 As UInteger
        Dim t2 As UInteger
        Dim t3 As UInteger
        Dim t4 As UInteger
        Dim i As Integer
        t2 = n1
        t1 = 0
        t4 = 1
        data(304) = 0
        i = 32768

        While (Not i = 0)
            t2 = SumaOverF(t2, t4)
            t1 = SumaOverF(t1, t2)
            t2 = SumaOverF(t2, t1)
            If (Not ((n2 And i) = 0)) Then
                t2 = Not t2
                t4 = (t4 << 1) + 1
                t3 = (t2 << 24) Or (t2 >> 8)
                t3 = t3 Xor data(t3 And 255)
                t1 = t1 Xor data(t1 And 255)
                t2 = (t3 << 24) Or (t3 >> 8)
                t3 = (t1 >> 24) Or (t1 << 8)
                t2 = t2 Xor data(t2 And 255)
                t3 = t3 Xor data(t3 And 255)
                t1 = (t3 >> 24) Or (t3 << 8)
            Else
                data(data(304) + 256) = t2
                data(data(304) + 272) = t1
                data(data(304) + 288) = t4
                data(304) += 1
                t3 = ((t1 << 24) Or (t1 >> 8))
                t2 = t2 Xor data(t2 And 255)
                t3 = t3 Xor data(t3 And 255)
                t1 = ((t3 << 24) Or (t3 >> 8))
                t3 = ((t2 >> 24) Or (t2 << 8))
                t3 = t3 Xor data(t3 And 255)
                t1 = t1 Xor data(t1 And 255)
                t2 = ((t3 >> 24) Or (t3 << 8))
                t4 <<= 1
            End If

            i >>= 1
        End While
        data(305) = t2
        data(306) = t1
        data(307) = t4
        data(308) = n1
        Return data
    End Function
    Function encshare4(ByVal data() As Byte, ByVal dest() As UInteger) As UInteger()
        Dim src(data.Length - 2) As Byte
        For i As Integer = 0 To data.Length - 2
            src(i) = data(i + 1)
        Next
        Dim pos As Byte
        Dim x As Byte
        Dim y As Byte
        Dim size As Integer = data(0)

        For i As Integer = 0 To 255
            dest(i) = 0
        Next
        For y = 0 To 3
            For i As Integer = 0 To 255
                dest(i) = ((dest(i) << 8) + i) And 4294967295
            Next

            pos = y
            For x = 0 To 1
                For i As Integer = 0 To 255
                    Dim tmp As UInteger = dest(i)
                    pos = (pos + (tmp + src(i Mod size)) And 255)

                    dest(i) = dest(pos)
                    dest(pos) = tmp
                Next
            Next
        Next
        For i As Integer = 0 To 255
            dest(i) = dest(i) Xor i
        Next

        dest = encshare3(dest, 0, 0)

        Return dest
    End Function
    Function encshare2(ByVal tbuff() As UInteger, ByVal len As Integer) As UInteger()
        Dim t1 As UInteger
        Dim t2 As UInteger
        Dim t3 As UInteger
        Dim t4 As UInteger
        Dim t5 As UInteger
        Dim old_p_ind As Integer = p_ind
        t2 = tbuff(304)
        t1 = tbuff(305)
        t3 = tbuff(306)
        t5 = tbuff(307)
        Dim cnt As Integer = 0
        For i As Integer = 0 To len - 1
            p_ind = t2 + 272
            While (t5 < 65536)
                t1 = SumaOverF(t1, t5)
                p_ind += 1
                t3 = SumaOverF(t3, t1)
                t1 = SumaOverF(t1, t3)

                tbuff(p_ind - 17) = t1
                tbuff(p_ind - 1) = t3
                t4 = (t3 << 24) Or (t3 >> 8)
                tbuff(p_ind + 15) = t5

                t5 <<= 1

                t2 += 1

                t1 = t1 Xor tbuff(t1 And 255)
                t4 = t4 Xor tbuff(t4 And 255)

                t3 = (t4 << 24) Or (t4 >> 8)

                t4 = (t1 >> 24) Or (t1 << 8)
                t4 = t4 Xor tbuff(t4 And 255)
                t3 = t3 Xor tbuff(t3 And 255)

                t1 = (t4 >> 24) Or (t4 << 8)
            End While
            t3 = t3 Xor t1
            tbuff(old_p_ind + i) = t3
            t2 -= 1

            t1 = tbuff(t2 + 256)
            t5 = tbuff(t2 + 272)
            t1 = Not t1 't1 = ~t1;

            t3 = (t1 << 24) Or (t1 >> 8)

            t3 = t3 Xor tbuff(t3 And 255)
            t5 = t5 Xor tbuff(t5 And 255)
            t1 = (t3 << 24) Or (t3 >> 8)

            t4 = (t5 >> 24) Or (t5 << 8)

            t1 = t1 Xor tbuff(t1 And 255)
            t4 = t4 Xor tbuff(t4 And 255)

            t3 = (t4 >> 24) Or (t4 << 8)

            t5 = (tbuff(t2 + 288) << 1) + 1
            cnt += 1
        Next
        tbuff(304) = t2
        tbuff(305) = t1
        tbuff(306) = t3
        tbuff(307) = t5
        Return tbuff
    End Function
End Class