Public Class Resampler

    Private source_rate As Integer
    Private source_samples = 0
    Private output_rate As Integer
    Private output_buffer As Single()
    Private carrier = False
    Private rate_gcd As Integer = 0

    Public Sub New(ByVal source_samplerate As Integer, ByVal target_samplerate As Integer)
        SetRate(source_samplerate, target_samplerate)
    End Sub

    'Parabolic 2x interpolator
    Public Function Interp(ByVal time As Single, ByRef data As Single(), ByVal index As UInt32) As Single
        If time = 0 Then Return data(index)
        If index = 0 Then Return data(0)
        Dim y1mym1 As Single = data(index + 1) - data(index - 1)
        Dim c0 As Single = 1 / 2.0 * data(index) + 1 / 4.0 * (data(index - 1) + data(index + 1))
        Dim c1 As Single = 1 / 2.0 * y1mym1
        Dim c2 As Single = 1 / 4.0 * (data(index + 2) - data(index) - y1mym1)
        Return (c2 * time + c1) * time + c0
    End Function

    Private Function GCD(a As Long, b As Long) As Long
        While b <> 0
            Dim tmp As Long = b
            b = a Mod b
            a = tmp
        End While
        Return a
    End Function

    Private SampleQueue As New Queue(Of Single())
    Private SampleCarry As New Queue(Of Single())

    Public Sub AddSample(ByVal sample As Single())
        SampleQueue.Enqueue(sample)
    End Sub

    Public Function GetOutput(ByVal length As Integer) As Single()
        Dim accumulate As New List(Of Single)
        For Each a In SampleCarry
            For Each b In a
                accumulate.Add(b)
            Next
        Next
        SampleCarry.Clear()
        Dim getsample = SampleQueue.Dequeue
        For Each a In getsample
            accumulate.Add(a)
        Next
        Dim raw = accumulate.ToArray


        Dim forward = output_rate / source_rate
        Dim size = (forward * raw.Length)
        If size < (length + 2) Then
            SampleCarry.Enqueue(raw)
            Return Nothing
        End If


        size -= 2

        Dim carry_size = raw.Length - size
        Dim output(size - 1) As Single

        Dim translate As Integer = 0
        Dim translate_floor As Integer = 0
        Dim reverse = source_rate / output_rate

        For i As Integer = 0 To size - 1
            translate = i * reverse
            translate_floor = Math.Floor(translate)
            output(i) = Interp(translate - translate_floor, raw, translate_floor)
        Next

        Dim carry_over(carry_size - 1) As Single
        Array.Copy(raw, CInt(raw.Length - carry_size), carry_over, 0, CInt(carry_size))
        SampleCarry.Enqueue(carry_over)

        Return output
    End Function

    Public Sub SetRate(ByVal source_samplerate As Integer, ByVal target_samplerate As Integer)
        source_rate = source_samplerate
        output_rate = target_samplerate
        rate_gcd = GCD(source_samplerate, target_samplerate)
    End Sub

End Class
