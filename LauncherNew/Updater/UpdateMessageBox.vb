Public Class UpdateMessageBox
    Public done As Boolean = False
    Public updatee As Boolean = False
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
    Private Sub yes_Click(sender As System.Object, e As System.EventArgs) Handles yes.Click
        updatee = True
        done = True
        Me.Hide()
    End Sub
    Private Sub no_Click(sender As System.Object, e As System.EventArgs) Handles no.Click
        updatee = False
        done = True
        Me.Hide()
    End Sub

    Private Sub UpdateMessageBox_FormClosing(sender As Object, e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        ExitMe = True
        updatee = False
        done = True
    End Sub

    Private Sub UpdateMessageBox_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
        ExitMe = False
        Dim ExitThread As New Threading.Thread(AddressOf CloseMe)
        ExitThread.Start()
    End Sub
End Class