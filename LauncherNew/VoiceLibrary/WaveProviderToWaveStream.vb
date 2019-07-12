Imports NAudio.Wave

Public Class WaveProviderToWaveStream
    Inherits WaveStream
    Private ReadOnly source As IWaveProvider
    Private m_position As Long

    Public Sub New(source As IWaveProvider)
        Me.source = source
    End Sub

    Public Overrides ReadOnly Property WaveFormat() As WaveFormat
        Get
            Return source.WaveFormat
        End Get
    End Property

    ''' <summary>
    ''' Don't know the real length of the source, just return a big number
    ''' </summary>
    Public Overrides ReadOnly Property Length() As Long
        Get
            Return Int32.MaxValue
        End Get
    End Property

    Public Overrides Property Position() As Long
        Get
            ' we'll just return the number of bytes read so far
            Return m_position
        End Get
        Set(value As Long)
            ' can't set position on the source
            ' n.b. could alternatively ignore this
            'Throw New NotImplementedException()
        End Set
    End Property

    Public Overrides Function Read(buffer As Byte(), offset As Integer, count As Integer) As Integer
        Dim read__1 As Integer = source.Read(buffer, offset, count)
        m_position += read__1
        Return read__1
    End Function
End Class
