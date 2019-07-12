Imports Microsoft.Win32

Public Class Settings
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
        Else
            Me.Close()
            Exit Sub
        End If
Failed:
        'Me.Close()
    End Sub
    Private Sub Settings_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
        ExitMe = False
        SetLabelText(SplashScreenForm.SplashScreenInfoLabel, "Waiting for user to close settings")
        HideForm(SplashScreen)
        LoadConfig()
        GetCheckboxSettings()
        GetRegistrySettings()
        Dim ExitThread As New Threading.Thread(AddressOf CloseMe)
        ExitThread.Start()
    End Sub
    Private Sub Settings_FormClosed(sender As Object, e As System.Windows.Forms.FormClosedEventArgs) Handles Me.FormClosed
        ExitMe = True
        'SetLabelText(SplashScreenForm.SplashScreenInfoLabel, "Settings dialog closed.")
        ShowForm(SplashScreen)
    End Sub
    Private Sub Settings_FormClosing(sender As Object, e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        SaveRegistrySettings()
        SaveLanguageRegistryKey()
        SaveConfig()
        LoadConfig()
    End Sub
    Private Sub SaveRegistrySettings()
        Dim regKey As RegistryKey
        Dim resolution = resobox.Text.Split("x")
        Dim vsync As Integer = 0
        If vsyncbox.Checked Then vsync = 1
        Dim color As Int32 = Convert.ToInt32(colorbox.Text)
        If color > 24 Then colorbox.Text = "24"
        Try
            regKey = Registry.CurrentUser.OpenSubKey("Software\Gearbox Software\Nightfire", True)
            regKey.SetValue("DisplayWidth", Convert.ToInt32(resolution(0)))
            regKey.SetValue("DisplayHeight", Convert.ToInt32(resolution(1)))
            regKey.SetValue("DisplayRefreshRate", Convert.ToInt32(resolution(2)))
            regKey.SetValue("DisplayDepth", Convert.ToInt32(colorbox.Text))
            regKey.SetValue("DisplayVSYNC", Convert.ToInt32(vsync))
            regKey.SetValue("DisplayMultisampleCount", Convert.ToInt32(aabox.Text))
            regKey.Close()
        Catch ex As Exception
        End Try
    End Sub
    Private Sub GetCheckboxSettings()
        launchnf.Checked = Not Globals.DontLaunchNF
        checkforupdatesbx.Checked = Not Globals.NoUpdate
        askfordownloading.Checked = Not Globals.AlwaysUpdate
        skipintro.Checked = Globals.ExtraCommandLineArgs.Contains("-toconsole")
        fixscopez.Checked = AntiCheatThreadAndMore.FixScopesinWidescreen
        rawmousebox.Checked = Globals.RawMouseInput
        windowed.Checked = Globals.ExtraCommandLineArgs.Contains("-windowed")
        borderlesschk.Checked = Globals.ExtraCommandLineArgs.Contains("-border") = False
        disablemouseaccel.Checked = Globals.ExtraCommandLineArgs.Contains("-noforcemparms")

        voiceenabled.Checked = VoiceThread.VoiceEnabled
        voicechat_button.Text = VoiceThread.VoiceKey

        couchmode.Checked = AntiCheatThreadAndMore.BigScreenMode
        controllerenabled.Checked = Not Globals.NoJoy

        playbackdevice.Items.Clear()
        recordingdevice.Items.Clear()

        playbackdevice.Items.Add("Default")
        recordingdevice.Items.Add("Default")

        Dim output_devices = SoundOut.GetDevices
        Dim input_devices = SoundIn.GetDevices

        For Each device In output_devices
            playbackdevice.Items.Add(device)
        Next
        playbackdevice.SelectedText = VoiceThread.VoiceOutDevice

        For Each device In input_devices
            recordingdevice.Items.Add(device)
        Next
        recordingdevice.SelectedText = VoiceThread.VoiceInDevice

        playbackamplification.Value = VoiceClient.AmpOutput + 5
        recordingamplification.Value = VoiceClient.AmpInput + 5

        keyboardlayoutcombo.Items.Clear()
        Languages.Clear()
        AddLanguages()
        For Each lang In Languages
            keyboardlayoutcombo.Items.Add(lang)
        Next
        GetLanguageRegistryKey()
        keyboardlayoutcombo.SelectedText = Language

        'vsync handled by getregistrysettings()
    End Sub
    Private Sub GetRegistrySettings()
        Dim regKey As RegistryKey
        Dim width As Integer = 0
        Dim height As Integer = 0
        Dim color As Byte = 0
        Dim refreshrate As Int16 = 0
        Dim antialiasing As Byte = 0
        Dim vsync As Byte = 0
        Dim stringresolution As String = ""
        Try
            regKey = Registry.CurrentUser.OpenSubKey("Software\Gearbox Software\Nightfire", True)
            width = regKey.GetValue("DisplayWidth", 0)
            height = regKey.GetValue("DisplayHeight", 0)
            refreshrate = regKey.GetValue("DisplayRefreshRate", 0)
            color = regKey.GetValue("DisplayDepth", 0)
            vsync = regKey.GetValue("DisplayVSYNC", 0)
            antialiasing = regKey.GetValue("DisplayMultisampleCount", 0)
            regKey.Close()
        Catch ex As Exception
        End Try
        If width > 0 Then
            stringresolution = width.ToString
            If height > 0 Then
                stringresolution = stringresolution & "x" & height.ToString
            Else
                stringresolution = "800x600"
            End If
            If refreshrate > 0 Then
                stringresolution = stringresolution & "x" & refreshrate.ToString
            Else
                stringresolution = stringresolution & "x0"
            End If
        Else
            stringresolution = "800x600x60"
        End If
        resobox.Text = stringresolution 'Update the window's textbox
        If color > 0 Then
            colorbox.Text = color.ToString 'Update the window's colorbox
        Else
            colorbox.Text = "24"
        End If
        vsyncbox.Checked = False
        If vsync = 1 Then vsyncbox.Checked = True 'Update the vsync box
        If antialiasing > 0 Then
            aabox.Text = antialiasing.ToString 'Update the antialiasing box
        End If
    End Sub
    Public Language As String = Nothing
    Public Languages As New List(Of String)
    Public Sub AddLanguages()
        Languages.Add("EN")
        Languages.Add("FR")
        Languages.Add("DE")
        Languages.Add("IT")
        Languages.Add("NL")
        Languages.Add("SV")
        Languages.Add("ES")
    End Sub

    Public Function GetLanguageRegistryKey() As Boolean
        Try
            Dim regKey As RegistryKey = Registry.LocalMachine.OpenSubKey("Software\GEARBOX\NIGHTFIRE", True)
            Language = regKey.GetValue("region", "EN")
            Dim found = False
            For Each lang In Languages
                If Language = lang Then
                    found = True
                    Exit For
                End If
            Next
            If Not found Then
                Language = "EN"
                regKey.SetValue("region", "EN", RegistryValueKind.String)
                regKey.Close()
                Return False
            End If
            regKey.Close()
            Return True
        Catch ex As Exception
        End Try
        Return False
    End Function

    Public Sub SaveLanguageRegistryKey()
        Try
            Dim regKey As RegistryKey = Registry.LocalMachine.OpenSubKey("Software\GEARBOX\NIGHTFIRE", True)
            regKey.SetValue("region", keyboardlayoutcombo.SelectedText, RegistryValueKind.String)
            regKey.Close()
        Catch ex As Exception
        End Try
    End Sub
End Class