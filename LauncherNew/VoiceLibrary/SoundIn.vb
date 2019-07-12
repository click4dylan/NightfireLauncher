Imports NAudio.Wave

Public Class SoundIn
    Public WithEvents wave_device As WaveIn
    Public Delegate Sub data_available(sender As Object, e As WaveInEventArgs)
    Private callback_available As New List(Of data_available)
    Private playback_active As Boolean = False

    Public Sub New(Optional autostart As Boolean = False, Optional ByVal device As Integer = 0, Optional ByVal callback As data_available = Nothing)
        wave_device = New WaveIn(WaveCallbackInfo.FunctionCallback)
        Dim format = WaveFormat.CreateIeeeFloatWaveFormat(sound_samplerate, sound_channels)
        With (wave_device)
            .NumberOfBuffers = 3
            .BufferMilliseconds = sound_input_buffer
            .DeviceNumber = device
            .WaveFormat = format
            If Not callback = Nothing Then
                AddCallback(callback)
            End If
            If autostart Then
                StartRecording()
            End If
        End With
    End Sub

    Public Sub SetDevice(ByVal device As Integer)
        wave_device.DeviceNumber = device
    End Sub

    Public Function GetDevice() As Integer
        Return wave_device.DeviceNumber
    End Function

    Public Sub AddCallback(ByVal callback As data_available)
        callback_available.Add(callback)
    End Sub

    Public Sub StartRecording()
        If playback_active Then Exit Sub
        playback_active = True
        On Error Resume Next
        wave_device.StartRecording()
    End Sub

    Public Sub StopRecording()
        If Not playback_active Then Exit Sub
        playback_active = False
        On Error Resume Next
        wave_device.StopRecording()
    End Sub

    Private Sub wavein_DataAvailable(sender As Object, e As WaveInEventArgs) Handles wave_device.DataAvailable
        If Not playback_active Then Return
        For Each a In callback_available
            a.Invoke(sender, e)
        Next
    End Sub

    Public Shared Function GetDevices() As String()
        Dim count = WaveIn.DeviceCount
        Dim output(WaveIn.DeviceCount - 1) As String
        For i As Integer = 0 To count - 1
            Dim cap = WaveIn.GetCapabilities(i)
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
        Dim list = SoundIn.GetDevices()
        Dim index = 0
        For Each device In list
            If device.ToLower.Trim = name.ToLower.Trim Then
                Return device
            End If
        Next
        Return "Default"
    End Function

End Class
