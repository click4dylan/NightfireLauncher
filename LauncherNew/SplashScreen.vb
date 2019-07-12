Public Class SplashScreen
    Private Sub CloseMe()
        While Not ExitGlobally
            Threading.Thread.Sleep(100)
        End While
        If Me.InvokeRequired Then
            On Error GoTo Failed
            Me.Invoke(New MethodInvoker(AddressOf CloseMe))
            Exit Sub
        Else
            Me.Close()
            Exit Sub
        End If
Failed:
        'Me.Close()
    End Sub
    Public Sub DisableButtons()
        browseservers_btn.Enabled = False
        browseservers_btn.BackgroundImage = Bond.My.Resources.serverlist_disabled
        options_btn.Enabled = False
        options_btn.BackgroundImage = Bond.My.Resources.options_disabled
    End Sub
    Private Sub SplashScreen_FormClosed(sender As Object, e As System.Windows.Forms.FormClosedEventArgs) Handles Me.FormClosed
        ExitGlobally = True
    End Sub
    Private Sub SplashScreen_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
        Me.Hide()
    End Sub
    Private Sub nightfirepclink_LinkClicked(sender As System.Object, e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles nightfirepclink.LinkClicked
        System.Diagnostics.Process.Start("http://nightfirepc.com")
    End Sub
    Private Sub nfsourcelink_LinkClicked(sender As System.Object, e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles nfsourcelink.LinkClicked
        System.Diagnostics.Process.Start("http://nightfiresource.com")
    End Sub
    Private Sub exit_btn_Click(sender As System.Object, e As System.EventArgs) Handles exit_btn.Click
        ExitGlobally = True
    End Sub
    Private Sub exit_btn_Hover(sender As System.Object, e As System.EventArgs) Handles exit_btn.MouseHover
        exit_btn.BackgroundImage = Global.Bond.My.Resources.Resources.exit_hover
    End Sub
    Private Sub exit_btn_ExitHover(sender As System.Object, e As System.EventArgs) Handles exit_btn.MouseLeave
        exit_btn.BackgroundImage = Global.Bond.My.Resources.Resources._exit
    End Sub
    Private Sub Hide_btn_Click(sender As System.Object, e As System.EventArgs) Handles Hide_btn.Click
        Me.WindowState = FormWindowState.Minimized
    End Sub
    Private Sub Hide_btn_Hover(sender As System.Object, e As System.EventArgs) Handles Hide_btn.MouseHover
        Hide_btn.BackgroundImage = Global.Bond.My.Resources.Resources.minimize_hover
    End Sub
    Private Sub Hide_btn_ExitHover(sender As System.Object, e As System.EventArgs) Handles Hide_btn.MouseLeave
        Hide_btn.BackgroundImage = Global.Bond.My.Resources.Resources.minimize
    End Sub
    Private Sub options_btn_Click(sender As System.Object, e As System.EventArgs) Handles options_btn.Click
        ShowForm(Settings)
    End Sub
    Private Sub options_btn_Hover(sender As System.Object, e As System.EventArgs) Handles options_btn.MouseHover
        options_btn.BackgroundImage = Global.Bond.My.Resources.Resources.options_hover
    End Sub
    Private Sub options_btn_ExitHover(sender As System.Object, e As System.EventArgs) Handles options_btn.MouseLeave
        options_btn.BackgroundImage = Global.Bond.My.Resources.Resources.options
    End Sub
    Private Sub browseservers_btn_Click(sender As System.Object, e As System.EventArgs) Handles browseservers_btn.Click
        HideForm(Me)
        ShowForm(ServerBrowser)
        ServerBrowser.Activate()
    End Sub
    Private Sub browseservers_btn_Hover(sender As System.Object, e As System.EventArgs) Handles browseservers_btn.MouseHover
        browseservers_btn.BackgroundImage = Global.Bond.My.Resources.Resources.serverlist_hover
    End Sub
    Private Sub browseservers_btn_ExitHover(sender As System.Object, e As System.EventArgs) Handles browseservers_btn.MouseLeave
        browseservers_btn.BackgroundImage = Global.Bond.My.Resources.Resources.serverlist
    End Sub
    Private Sub NightfireSourceBTN_Click(sender As System.Object, e As System.EventArgs) Handles NightfireSourceBTN.Click
        System.Diagnostics.Process.Start("http://nightfiresource.com")
    End Sub

    Private Sub SplashScreenInfoLabel_Paint(sender As Object, e As System.Windows.Forms.PaintEventArgs) Handles SplashScreenInfoLabel.Paint
    End Sub


    Private Sub SplashScreen_Shown(sender As Object, e As System.EventArgs) Handles Me.Shown
        If Not Initialized Then
            SplashScreenForm = Me
            Dim entry As New Main
            Dim ExitThread As New Threading.Thread(AddressOf CloseMe)
            ExitThread.Start()
            Initialized = True
        End If
    End Sub

End Class
