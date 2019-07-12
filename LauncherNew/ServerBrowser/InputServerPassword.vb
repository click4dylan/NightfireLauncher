Public Class InputServerPassword
    Private ExitMe As Boolean = False
    Private Sub CloseMe()
        While Not ExitMe
            If ExitGlobally Then Exit While
            Threading.Thread.Sleep(100)
        End While
        If Me.InvokeRequired Then
            On Error GoTo Failed
            Me.Invoke(New MethodInvoker(AddressOf CloseMe))
            Exit Sub
        End If
Failed:
        Me.Close()
    End Sub
    Private Sub InputServerPassword_FormClosed(sender As Object, e As System.Windows.Forms.FormClosedEventArgs) Handles Me.FormClosed
        ExitMe = True
    End Sub
    Private Sub InputServerPassword_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
        Dim ExitThread As New Threading.Thread(AddressOf CloseMe)
        ExitThread.Start()
    End Sub
    Private Sub txtUser_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles passwdbox.KeyDown
        If e.KeyCode = Keys.Enter Then
            ServerBrowser.EnteredServerPassword = True
            ServerBrowser.ServerPassword = Me.passwdbox.Text
            Me.Close()
        End If
    End Sub
End Class