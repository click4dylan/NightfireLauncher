Imports NAudio.Wave

Public Class OpusCodec
    Private encoder As POpusCodec.OpusEncoder
    Private decoder As POpusCodec.OpusDecoder

    Public bytes_encoded As UInt64 = 0

    Public Sub New()
        encoder = New POpusCodec.OpusEncoder(sound_samplerate, sound_channels, encoder_bitrate, encoder_type, encoder_delay)
        decoder = New POpusCodec.OpusDecoder(sound_samplerate, sound_channels)

        With encoder
            .MaxBandwidth = encoder_maxband
            .EncoderDelay = encoder_delay
            .UseUnconstrainedVBR = True
        End With
    End Sub

    Public Function Encode(ByVal pcmdata As Byte(), ByVal length As Integer) As Byte()
        If encoder_raw Then Return pcmdata
        Dim prepare = FloatsFromBytes(pcmdata, length)
        Dim encoded = encoder.Encode(prepare)

        bytes_encoded += encoded.Length

        Return encoded
    End Function

    Public Function EncodeFloats(ByVal floats As Single()) As Byte()
        If encoder_raw Then Return FloatsToBytes(floats, floats.Length)
        Dim encoded = encoder.Encode(floats)
        bytes_encoded += encoded.Length
        Return encoded
    End Function

    Public Function Encode(ByVal e As WaveInEventArgs) As Byte()
        If encoder_raw Then Return e.Buffer
        Return Encode(e.Buffer, e.BytesRecorded)
    End Function

    Public Function Decode(ByVal data As Byte()) As Byte()
        If encoder_raw Then Return data
        Dim decoded = decoder.DecodePacketFloat(data)
        Dim decoded2 = FloatsToBytes(decoded, decoded.Length)
        Return decoded2
    End Function

    Public Function DecodeFloats(ByVal data As Byte()) As Single()
        If encoder_raw Then Return FloatsFromBytes(data, data.Length)
        Return decoder.DecodePacketFloat(data)
    End Function

    Public Sub DecodeAndPlay(ByVal data As Byte(), ByVal output As SoundOut)
        If encoder_raw Then
            output.AddSamples(data, data.Length)
        Else
            Dim decoded = Decode(data)
            output.AddSamples(decoded, decoded.Length)
        End If
        
    End Sub

    Public Function DecodeLost() As Byte()
        Dim decoded_raw = decoder.DecodePacketLostFloat
        Dim decoded = FloatsToBytes(decoded_raw, decoded_raw.Length)
        Return decoded
    End Function

    Public Function DecodeLostFloats() As Single()
        Dim decoded_raw = decoder.DecodePacketLostFloat
        Return decoded_raw
    End Function

    Public Sub DecodeLostAndPlay(ByVal output As SoundOut)
        Dim decoded = DecodeLost()
        output.AddSamples(decoded, decoded.Length)
    End Sub

    Public Function FloatsFromBytes(ByRef data As Byte(), ByVal length As Integer) As Single()
        Dim floats As Integer = Math.Floor(length / 4)
        Dim result(floats - 1) As Single

        For i As Integer = 0 To floats - 1
            result(i) = BitConverter.ToSingle(data, i * 4)
        Next

        Return result
    End Function

    Public Function FloatsToBytes(ByRef floats As Single(), ByVal length As Integer) As Byte()
        Dim bytes As Integer = length * 4
        Dim output(bytes - 1) As Byte

        For i As Integer = 0 To length - 1
            Dim index = i * 4
            Dim converted = BitConverter.GetBytes(floats(i))
            output(index) = converted(0)
            output(index + 1) = converted(1)
            output(index + 2) = converted(2)
            output(index + 3) = converted(3)
        Next

        Return output
    End Function


End Class
