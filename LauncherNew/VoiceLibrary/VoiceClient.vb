Imports System
Imports System.Net.Sockets
Imports NAudio.Wave

Public Class VoiceClient
    Implements IDisposable
    Private WithEvents poll As Timers.Timer

    Private udp As UdpClient
    Private opus As OpusCodec
    Public sound_in As SoundIn
    Public sound_out As SoundOut
    Private channel As String
    Private name As String
    Private nextpacketno As UInt32
    Private net_recv As Threading.Thread
    Private closing = False
    Private channel_users As Dictionary(Of String, String)
    Private channel_users_lastvoice As Dictionary(Of String, Double)

    Public Sub New(ByVal host As String, ByVal port As Integer, Optional ByVal channel As String = "", Optional name As String = "", Optional ByVal device_out As Integer = 0, Optional ByVal device_in As Integer = 0)
        nextpacketno = 0
        opus = New OpusCodec
        udp = New UdpClient(host, port)
        sound_in = New SoundIn(False, device_in)
        sound_out = New SoundOut(True, device_out)
        channel_users = New Dictionary(Of String, String)
        channel_users_lastvoice = New Dictionary(Of String, Double)

        sound_in.AddCallback(AddressOf SoundIn_Callback)
        net_recv = New Threading.Thread(AddressOf Net_Receive)
        net_recv.Start()
        SetName(name)
        SetChannel(channel)
        poll = New Timers.Timer(333)
        poll.Start()
    End Sub

    Private LastVolumeSet As Single = 1
    Public Sub SetMasterOutputVolume(ByVal level As Single)
        If Not level = LastVolumeSet Then
            sound_out.SetVolume(level)
            LastVolumeSet = level
        End If
    End Sub

    Public Shared AmpOutput As Single = 0
    Public Shared AmpInput As Single = 0

    Private Function GetAmpMultiplier(ByVal ampvalue As Single) As Single
        If ampvalue >= 0 Then
            Return ampvalue + 1.0
        Else
            Dim abs As Single = Math.Abs(ampvalue) + 1
            Dim out As Single = 1.0
            For i As Integer = 0 To abs - 1
                out /= 1.4
            Next
            Return out
        End If
    End Function

    Public Sub SetHostAndPort(ByVal host As String, ByVal port As Integer)
        udp.Connect(host, port)
    End Sub
    Public Sub SetChannel(Optional ByVal channel As String = "", Optional ByVal forcechange As Boolean = False)
        If (channel = Me.channel) And (forcechange = False) Then Return
        Dim packet As New sVoicePacket_t
        packet.command = "channel|" & channel
        Net_SendPacket(packet)
        Me.channel = channel
    End Sub
    Public Sub SetName(ByVal name As String, Optional ByVal forcechange As Boolean = False)
        ' If (name = Me.name) And (forcechange = False) Then Return
        Dim packet As New sVoicePacket_t
        packet.command = "name|" & Base64(name)
        Net_SendPacket(packet)
        Me.name = name
    End Sub
    Public Sub SetMute(ByVal userid As String, ByVal muted As Boolean)
        Dim state = "false"
        If muted Then state = "true"
        Dim packet As New sVoicePacket_t
        packet.command = "mute|" & state & "|" & userid
        Net_SendPacket(packet)
        Me.name = name
    End Sub
    Public Function GetUsers() As Dictionary(Of String, String)
        On Error GoTo BeginGetUsers
        Dim users As New Dictionary(Of String, String)
        Do
BeginGetUsers:
            users.Clear()
            SyncLock channel_users
                For Each user In channel_users
                    If Not users.ContainsKey(user.Key) Then
                        users.Add(user.Key, user.Value)
                    End If
                Next
                Exit Do
            End SyncLock
        Loop
        Return users
    End Function
    Public Function GetTalking() As String()
        Dim result As New List(Of String)
        Dim allusers = GetUsers()
        SyncLock channel_users_lastvoice
            For Each user In channel_users_lastvoice
                Dim time = Sys_FloatTime() - user.Value
                If time < 1 Then
                    If allusers.ContainsKey(user.Key) Then
                        result.Add(allusers(user.Key))
                    End If
                End If
            Next
        End SyncLock
        Return result.ToArray
    End Function
    Public Sub ExitServer()
        Dim packet As New sVoicePacket_t
        packet.command = "disconnect"
        Net_SendPacket(packet)
        poll.Stop()
    End Sub


    'Device switching
    Private current_input = 0
    Private current_output = 0
    Public Function DeltaInDevice(ByVal amount As Integer)
        Dim device = sound_in.GetDevice + amount
        Dim max = SoundIn.GetDevices.Length - 1
        If device > max Then
            device = max
        End If
        If device < 0 Then
            device = 0
        End If
        sound_in.SetDevice(device)
        Return device
    End Function
    Public Function DeltaOutDevice(ByVal amount As Integer)
        Dim device = sound_out.GetDevice + amount
        Dim max = SoundOut.GetDevices.Length - 1
        If device > max Then
            device = max
        End If
        If device < 0 Then
            device = 0
        End If
        sound_out.SetDevice(device)
        Return device
    End Function

    'Stream activation
    Private StreamState = False
    Public Sub ActivateStream()
        If StreamState Then Return
        StreamState = True
        Packet_First = 2
        sound_in.StartRecording()
    End Sub
    Public Sub DeactivateStream()
        If Not StreamState Then Return
        StreamState = False
        sound_in.StopRecording()
    End Sub
    Public Sub SetStreamActive(ByVal state As Boolean)
        If state Then
            ActivateStream()
        Else
            DeactivateStream()
        End If
    End Sub
    Public Function IsStreamActive() As Boolean
        Return StreamState
    End Function
    Public Sub ToggleStream()
        If StreamState Then
            DeactivateStream()
        Else
            ActivateStream()
        End If
    End Sub

    'Network
    Private Sub Net_Receive()
        On Error GoTo Net_Receive_Delay
        Dim remote As New Net.IPEndPoint(Net.IPAddress.Any, 0)
        Do
Net_Receive_Begin:
            If ExitGlobally Then Exit Sub
            udp.Client.ReceiveTimeout = 3000
            Dim data = udp.Receive(remote)
            If data.Length = 0 Then Continue Do
            Dim capsule As New sVoicePacket_t(data)
            If capsule.samples_encoded.Length > 0 Then
                Dim decoded = opus.Decode(capsule.samples_encoded)
                SoundOut_Play(capsule.playback_channel, decoded)
            End If
            If capsule.command.Length > 0 Then
                Net_ProcessCommand(capsule.command)
            End If
            Continue Do
Net_Receive_Delay:
            Threading.Thread.Sleep(1)
            Resume Next
        Loop

    End Sub
    Private Sub Net_ProcessCommand(ByVal command As String)
        If command.Length = 0 Then Exit Sub
        Dim segments_pieces = command.Split("|")
        Dim segments As New Queue(Of String)
        For Each pieces In segments_pieces
            segments.Enqueue(pieces)
        Next


        Dim seg1 As String = segments.Dequeue
        Select Case seg1
            Case "redirect"
                Dim host = segments.Dequeue
                Dim port = segments.Dequeue
                SetHostAndPort(host, port)
            Case "lostpacket"
            Case "users"
                Dim users = segments.ToArray
                SyncLock channel_users
                    channel_users.Clear()
                    For Each user In users
                        Dim split = user.Split(":")
                        Dim name = DecodeBase64(split(0))
                        Dim userid = split(1)
                        channel_users.Add(userid, name)
                    Next
                End SyncLock
            Case "#"
                SyncLock channel_users_lastvoice
                    Dim allusers = GetUsers()
                    Dim clientid = segments.Dequeue

                    If allusers.ContainsKey(clientid) Then
                        Dim user = allusers(clientid)
                        If Not channel_users_lastvoice.ContainsKey(clientid) Then
                            channel_users_lastvoice.Add(clientid, Sys_FloatTime)
                        Else
                            channel_users_lastvoice(clientid) = Sys_FloatTime()
                        End If
                    End If

                    'Remove old users from last voice packet table
                    Dim olduser As List(Of String) = Nothing
                    For Each entry In channel_users_lastvoice
                        If Not allusers.ContainsKey(entry.Key) Then
                            olduser.Add(entry.Key)
                        End If
                    Next
                    For Each entry In olduser
                        channel_users_lastvoice.Remove(entry)
                    Next
                End SyncLock
        End Select
    End Sub
    Private Sub Net_SendPacket(ByRef packet As sVoicePacket_t)
        On Error Resume Next
        packet.packetno = nextpacketno
        Dim data = packet.ToArray
        udp.Send(data, data.Length)
        nextpacketno += 1
    End Sub
    Private Function Packet_Create(Optional ByVal command As String = "") As sVoicePacket_t
        Dim packet As New sVoicePacket_t
        packet.packetno = nextpacketno
        packet.command = command

        Return packet
    End Function
    Private Function Packet_CreateFromSound(ByRef samples As Byte(), ByVal bytesrecorded As Integer)
        On Error Resume Next
        Dim packet As New sVoicePacket_t
        packet.command = Command()
        packet.samples_encoded = opus.Encode(samples, bytesrecorded)
        Return packet
    End Function

    'Playback
    Private Sub SoundOut_Play(ByVal channel As Byte, ByVal samples As Byte())
        If closing Then Exit Sub
        'Increase volume
        Dim floats = opus.FloatsFromBytes(samples, samples.Length)
        Dim ampval = GetAmpMultiplier(AmpOutput)
        For i As Integer = 0 To floats.Length - 1
            floats(i) *= ampval
        Next
        Dim pcm = opus.FloatsToBytes(floats, floats.Length)
        sound_out.AddSamples(pcm, pcm.Length, channel)
    End Sub

    'Record
    Private Packet_First = 2
    Private Sub SoundIn_Callback(ByVal sender As Object, ByVal e As WaveInEventArgs)
        If closing Then Exit Sub
        If Packet_First > 0 Then
            Packet_First -= 1
            Return
        End If

        'Increase volume
        Dim floats = opus.FloatsFromBytes(e.Buffer, e.BytesRecorded)
        Dim ampval = GetAmpMultiplier(AmpInput)
        For i As Integer = 0 To floats.Length - 1
            floats(i) *= ampval
        Next
        Dim pcm = opus.FloatsToBytes(floats, floats.Length)

        Dim packet = Packet_CreateFromSound(pcm, e.BytesRecorded)
        Net_SendPacket(packet)
    End Sub


    Private Function Base64(ByVal data As String)
        Dim bytes As Byte() = Text.Encoding.UTF8.GetBytes(data)
        Return Convert.ToBase64String(bytes)
    End Function
    Private Function DecodeBase64(ByVal base64 As String)
        Dim bytes As Byte() = Convert.FromBase64String(base64)
        Return Text.Encoding.UTF8.GetString(bytes)
    End Function

    Private Sub poll_Elapsed(ByVal sender As Object, ByVal e As System.Timers.ElapsedEventArgs) Handles poll.Elapsed
        Dim packet As New sVoicePacket_t
        'Ping current channel
        SetChannel(channel, True)
    End Sub
    Public Sub Dispose() Implements IDisposable.Dispose
        closing = True
        ExitServer()
        sound_in.StopRecording()
        net_recv.Abort()
        opus = Nothing
        udp = Nothing
        sound_in = Nothing
        sound_out.Dispose()
        poll.Stop()
        poll.Dispose()
    End Sub

End Class
