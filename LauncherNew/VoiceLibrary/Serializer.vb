Imports System.IO
Imports System.Runtime.Serialization.Formatters.Binary

Module Serializer

    Public Function ObjectToData(obj As [Object]) As Byte()
        If obj Is Nothing Then
            Return Nothing
        End If
        Dim bf As New BinaryFormatter()
        Dim ms As New MemoryStream()
        bf.Serialize(ms, obj)
        Return ms.ToArray()
    End Function

    Public Function DataToObject(arrBytes As Byte()) As [Object]
        Dim memStream As New MemoryStream()
        Dim binForm As New BinaryFormatter()
        memStream.Write(arrBytes, 0, arrBytes.Length)
        memStream.Seek(0, SeekOrigin.Begin)
        Dim obj As [Object] = DirectCast(binForm.Deserialize(memStream), [Object])
        Return obj
    End Function

End Module
