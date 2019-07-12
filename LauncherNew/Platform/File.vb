Imports System.IO

Module Files
    Public Sub DeleteFile(ByVal file As String)
        IO.File.Delete(file)
    End Sub

    Public Sub MakeAllFilesWritable()
        On Error Resume Next
        Dim list_files = IO.Directory.GetFiles(IO.Directory.GetCurrentDirectory, "*.*", IO.SearchOption.AllDirectories)
        For Each entry In list_files
            Dim attributes As FileAttributes = File.GetAttributes(entry)
            If (attributes And FileAttributes.ReadOnly) = FileAttributes.ReadOnly Then
                File.SetAttributes(entry, FileAttributes.Normal)
            End If
        Next
    End Sub

    Public Sub DeleteIfExists(ByVal file As String)
        On Error Resume Next
        If IO.File.Exists(file) Then
            IO.File.Delete(file)
        End If
    End Sub

    Public Function Hash(ByVal Path As String) As String 'Function to get MD5 hash of a file
        Try
            Using reader As New System.IO.FileStream(Path, IO.FileMode.Open, IO.FileAccess.Read)
                Using md5 As New System.Security.Cryptography.MD5CryptoServiceProvider
                    Dim hashb() As Byte = md5.ComputeHash(reader)
                    Return BitConverter.ToString(hashb, 0).Replace("-", "")
                End Using
            End Using
        Catch ex As Exception
            'main.SplashLabel = "FAILED TO HASH A FILE!"
            Application.DoEvents()
            'main.TopMost = False
            'main.Opacity = 0
            My.Computer.Audio.PlaySystemSound(Media.SystemSounds.Exclamation)
            'main.Opacity = 100
        End Try
        Return 0
    End Function

    Public Function IsDirectoryWritable(ByVal path As String) 'Is the directory read-only or not
        Dim info As New DirectoryInfo(path)
        Return (info.Attributes And FileAttributes.ReadOnly) <> FileAttributes.ReadOnly
    End Function
End Module
