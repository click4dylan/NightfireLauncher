Imports NAudio.Wave

'TO DO: Pause/Volume down after channel no longer receiving packets

Public Class SoundOut
    Implements IDisposable
    Public wave_device
    Public wave_buffer() As BufferedWaveProvider
    Public wave_mix As WaveMixerStream32
    Public Sub New(Optional ByVal autostart As Boolean = True, Optional ByVal device As Integer = 0)
        Dim format = WaveFormat.CreateIeeeFloatWaveFormat(sound_samplerate, sound_channels)
        wave_mix = New WaveMixerStream32
        wave_mix.AutoStop = False
        Dim channels = sound_mixerchannels
        ReDim wave_buffer(channels - 1)
        For i As Integer = 0 To channels - 1
            wave_buffer(i) = New BufferedWaveProvider(format)
            wave_buffer(i).DiscardOnBufferOverflow = True
            wave_mix.AddInputStream(New WaveProviderToWaveStream(wave_buffer(i)))
        Next

        If sound_output_dx Then
            wave_device = New DirectSoundOut(DirectSoundOut.DSDEVID_DefaultPlayback, CInt(sound_output_buffer))
            With wave_device
                .Volume = 1
                .Init(wave_mix)
                If autostart Then
                    .Play()
                End If
            End With
        Else
            wave_device = New WaveOut(WaveCallbackInfo.FunctionCallback)
            With wave_device
                .NumberOfBuffers = 3
                .Volume = 1
                .DeviceNumber = device
                .DesiredLatency = sound_output_buffer
                .Init(wave_mix)
                If autostart Then
                    .Play()
                End If
            End With
        End If


    End Sub

    Public Sub AddSamples(ByRef samples As Byte(), Optional ByVal length As Integer = -1, Optional channel As Integer = 0)
        If length = -1 Then
            wave_buffer(channel).AddSamples(samples, 0, samples.Length)
        Else
            wave_buffer(channel).AddSamples(samples, 0, length)
        End If
    End Sub

    Public Sub Play()
        wave_device.Play()
    End Sub

    Public Sub Pause()
        wave_device.Pause()
    End Sub

    Public Sub StopPlaying()
        wave_device.Stop()
    End Sub

    Public Sub SetVolume(ByVal volume As Single)
        wave_device.Volume = volume
    End Sub

    Public Sub SetDevice(ByVal device As Integer)
        wave_device.DeviceNumber = device
    End Sub

    Public Function GetDevice() As Integer
        Return wave_device.DeviceNumber
    End Function

    Public Shared Function GetDevices() As String()
        Dim count = WaveOut.DeviceCount
        Dim output(WaveOut.DeviceCount - 1) As String
        For i As Integer = 0 To count - 1
            Dim cap = WaveOut.GetCapabilities(i)
            Dim name = cap.ProductName
            output(i) = name
        Next
        Return output
    End Function

    Public Shared Function NameToID(ByVal name As String) As Integer
        If name.ToLower.Trim = "default" Then Return 0
        Dim list = GetDevices()
        Dim index = 0
        For Each device In list
            If device.ToLower.Trim = name.ToLower.Trim Then
                Return index
            End If
            index = +1
        Next
        Return index
    End Function

    Public Shared Function DeviceGetCorrectCasing(ByVal name As String) As String
        Dim list = SoundOut.GetDevices()
        Dim index = 0
        For Each device In list
            If device.ToLower.Trim = name.ToLower.Trim Then
                Return device
            End If
        Next
        Return "Default"
    End Function


    Public Sub Dispose() Implements IDisposable.Dispose
        wave_device.Volume = 0
        StopPlaying()
        wave_device.Dispose()
        GC.SuppressFinalize(Me)
    End Sub

    Protected Overrides Sub Finalize()
        Dispose()
    End Sub
End Class
