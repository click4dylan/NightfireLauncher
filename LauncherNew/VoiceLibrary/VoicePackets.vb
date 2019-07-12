Imports System
Imports System.Net


Public Class sIncomingPacket_t
    Public from As New IPEndPoint(IPAddress.Any, 0)
    Public timestamp As Double = 0
    Public packet As New sVoicePacket_t()
End Class
Public Class sVoicePacket_t
    Public packetno As Integer = 0
    Public command As String = ""
    Public playback_channel As Byte = 0
    Public samples_encoded As Byte() = {}
    Public Function ToArray() As Byte()
        Dim b_packetno = BitConverter.GetBytes(packetno)
        Dim b_command = Text.ASCIIEncoding.ASCII.GetBytes(command)
        Dim b_commandsz As Byte = b_command.Length
        Dim b_samplesz = BitConverter.GetBytes(CShort(samples_encoded.Length))

        Dim output_size = 4 + 1 + b_command.Length + 3 + samples_encoded.Length
        Dim output(output_size - 1) As Byte
        Dim offset = 0

        'Write packet no
        b_packetno.CopyTo(output, offset)
        offset += b_packetno.Length

        'Write command size
        output(offset) = b_commandsz
        offset += 1

        'Write command
        b_command.CopyTo(output, offset)
        offset += b_command.Length

        'Write playback channel
        output(offset) = playback_channel
        offset += 1

        'Write samples size
        b_samplesz.CopyTo(output, offset)
        offset += 2

        'Write samples
        samples_encoded.CopyTo(output, offset)
        offset += samples_encoded.Length

        Return output
    End Function
    Public Sub New(ByRef data As Byte())
        Dim offset = 0

        'Read packet no
        packetno = BitConverter.ToInt32(data, offset)
        offset += 4

        'Read command length
        Dim commandlength = data(offset)
        offset += 1

        'Read command
        command = Text.ASCIIEncoding.ASCII.GetString(data, offset, commandlength).Replace(vbNullChar, "")
        offset += commandlength

        'Read playback channel
        playback_channel = data(offset)
        offset += 1

        'Read sample size
        Dim samples_size = BitConverter.ToInt16(data, offset)
        offset += 2

        'Read samples
        ReDim samples_encoded(samples_size - 1)
        Buffer.BlockCopy(data, offset, samples_encoded, 0, samples_size)

    End Sub
    Public Sub New()
    End Sub
End Class
Public Class sClient_t
    Public name As String = ""
    Public lastrecv_time As Double = 0
    Public lastrecv_packetno As UInteger = 0
    Public lastsent_packetno As UInteger = 0
    Public current_channel As String = ""
End Class
Public Class sChannel_t
    Public last_ping As Double = 0
    Public mixerchannels As New Dictionary(Of String, Byte)
End Class
