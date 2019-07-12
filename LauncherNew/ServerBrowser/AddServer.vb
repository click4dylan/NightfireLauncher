Public Class AddAServer
    Public failed = False
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
    Private Sub AddAServer_FormClosed(sender As Object, e As System.Windows.Forms.FormClosedEventArgs) Handles Me.FormClosed
        ExitMe = True
    End Sub
    Private Sub AddAServer_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
        ExitMe = False
        Dim ExitThread As New Threading.Thread(AddressOf CloseMe)
        ExitThread.Start()
    End Sub
    Private Sub addserverdone_Click(sender As Object, e As EventArgs) Handles addserverdone.Click
        failed = False 'this function is coded like garbage, fix it
        Dim testip As String = ipqueryport.Text
        Dim testip2 As Net.IPAddress = Nothing
        If Not testip.Length >= 9 Then ErrorAddingServer()
        If Not failed And Not testip.Contains(":") Then ErrorAddingServer()
        If Not failed And Not testip.Contains(".") Then ErrorAddingServer()
        Dim spl = testip.Split(":")
        If Not failed And Net.IPAddress.TryParse(spl(0), testip2) = False Then ErrorAddingServer()
        If Not failed Then
            TellTheMaster()
        End If
        ipqueryport.Text = ""
        Me.Close()
    End Sub
    Private Sub ErrorAddingServer()
        MsgBox("Error in address formatting. Please specify an IP address and the query port in the format: 123.123.123.123:6550", MsgBoxStyle.Critical)
        ipqueryport.Text = ""
        failed = True
    End Sub
    Private Sub TellTheMaster()
        Dim udp As Net.Sockets.UdpClient
        Try
            udp = New Net.Sockets.UdpClient
        Catch ex As Exception
            'udp.Close()
            Exit Sub
        End Try
        Try
            udp.Client.ReceiveTimeout = 1000
            Dim receivedpacket As String
            Dim dns As Net.IPHostEntry = System.Net.Dns.GetHostEntry(ServerBrowser.masterip)
            Dim masterip2 = dns.AddressList(0)
            Dim endpoint As New Net.IPEndPoint(masterip2, 27900)

            ServerBrowser.SendPacket(endpoint.Address.ToString, endpoint.Port, System.Text.Encoding.ASCII.GetBytes("\manualheartbeat\" & ipqueryport.Text & "\"), udp)
            Dim ipqport = ipqueryport.Text.Split(":")
            endpoint = New System.Net.IPEndPoint(System.Net.IPAddress.Parse(ipqport(0)), ipqport(1))
            Dim timesent = Environment.TickCount
            ServerBrowser.SendPacket(endpoint.Address.ToString, endpoint.Port, System.Text.Encoding.ASCII.GetBytes("\status\"), udp)
            Threading.Thread.Sleep(5)
            receivedpacket = ServerBrowser.ReceivePacket(udp, endpoint)
            Dim timereceived = Environment.TickCount - 5
            If receivedpacket.StartsWith("\gamename\") Then
                Dim serverstatus As New SortedDictionary(Of String, String)
                receivedpacket = receivedpacket.Replace("\gamename\", "gamename\")
                Dim spl1 = receivedpacket.Split("\")
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
                str(1) = timereceived - timesent 'ping
                str(2) = numplayers & "/" & maxplayers
                str(5) = endpoint.Address.ToString & ":" & hostport
                str(6) = endpoint.Port.ToString
                Dim itm As New ListViewItem(str)
                If Not ServerBrowser.ServerList.Items.Contains(itm) Then
                    ServerBrowser.ServerList.Items.Add(itm)
                End If
            End If
            udp.Close()
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical)
        End Try
    End Sub
End Class