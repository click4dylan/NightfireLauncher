Imports System.Runtime.InteropServices

Module Processes
    <Runtime.InteropServices.DllImport("user32.dll")> _
    Private Function GetForegroundWindow() As IntPtr
    End Function
    <Runtime.InteropServices.DllImport("user32.dll")> _
    Private Function GetWindowText(ByVal hWnd As IntPtr, ByVal lpWindowText As System.Text.StringBuilder, _
    ByVal nMaxCount As Integer) As Integer
    End Function
    ''' <summary>
    ''' Get's the handle and text of the foreground window
    ''' </summary>
    Public Function GetWindowText() As Window
        Dim title As New System.Text.StringBuilder(255)
        Dim titleLength As Integer = GetWindowText(GetForegroundWindow(), title, title.Capacity + 1)
        title.Length = titleLength
        Dim w As New Window
        w.Text = title.ToString()
        w.Handle = GetForegroundWindow()
        Return w
    End Function
    ''' <summary>
    ''' Represents an open, visible window
    ''' </summary>
    Public Structure Window
        Public Handle As IntPtr
        Public Text As String
    End Structure
    Public Function GetProcessesByName(ByVal name As String) As Process()
        Return Process.GetProcessesByName(name)
    End Function
    Public Sub ProcessesKillAll(ByVal name As String)
        On Error Resume Next
        Dim processes = GetProcessesByName(name)
        For Each process In processes
            process.Kill()
        Next
    End Sub

    Public Sub ProcessesKillExceptSelf(ByVal name As String)
        Dim bond = GetProcessesByName(name)
        For Each b In bond
            If b.StartInfo.FileName = Process.GetCurrentProcess.StartInfo.FileName Then
                If Not b.Id = Process.GetCurrentProcess.Id Then
                    Try
                        b.Kill()
                    Catch ex As Exception
                    End Try
                End If
            End If
        Next
    End Sub
End Module
