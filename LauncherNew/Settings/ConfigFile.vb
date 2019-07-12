Module ConfigFile

    Private file_settings = "LauncherSettings.txt"
    Private loaded_config = Nothing

    'Load config values into launcher
    Public Sub LoadConfig()
        InitDefaultConfig()

        If IO.File.Exists(file_settings) Then
            loaded_config = LoadConfig(file_settings)
            Globals.NoUpdate = (loaded_config("CheckForUpdates").value = "false")
            Globals.DontLaunchNF = (loaded_config("LaunchNightfire").value = "false")
            Globals.AlwaysUpdate = (loaded_config("AskBeforeDownloadingUpdates").value = "false")
            Globals.RawMouseInput = (loaded_config("RawMouseInput").value = "true")
            If loaded_config("DisableMouseAcceleration").value = "true" Then
                Globals.ExtraCommandLineArgs = Globals.ExtraCommandLineArgs & "-noforcemparms "
            Else
                If Globals.ExtraCommandLineArgs.Contains("-noforcemparms ") Then
                    Globals.ExtraCommandLineArgs = Globals.ExtraCommandLineArgs.Replace("-noforcemparms ", " ")
                End If
            End If
            If loaded_config("WindowedMode").value = "true" Then
                Globals.ExtraCommandLineArgs = Globals.ExtraCommandLineArgs & "-windowed "
            Else
                If Globals.ExtraCommandLineArgs.Contains("-windowed ") Then
                    Globals.ExtraCommandLineArgs = Globals.ExtraCommandLineArgs.Replace("-windowed ", " ")
                End If
            End If
            If loaded_config("Borderless").value = "false" Then
                Globals.ExtraCommandLineArgs &= "-border "
            End If
            If loaded_config("SkipIntroMovies").value = "true" Then
                If Not Globals.ConnectArg.Contains("+connect") Or Settings.commandlineoptions.Text.Contains("+connect") Then
                    Globals.ExtraCommandLineArgs = Globals.ExtraCommandLineArgs & "-toconsole "
                End If
            End If
            If loaded_config("StartupArguments").value.Length > 0 Then
                Settings.commandlineoptions.Text = loaded_config("StartupArguments").value
                Globals.ExtraCommandLineArgs = Globals.ExtraCommandLineArgs & loaded_config("StartupArguments").value & " "
            End If
            AntiCheatThreadAndMore.FixScopesinWidescreen = (loaded_config("FixScopesInWidescreen").value = "true")
            VoiceThread.VoiceEnabled = (loaded_config("EnableVoiceChat").value = "true")
            VoiceThread.VoiceKey = loaded_config("VoiceChatKey").value
            VoiceThread.VoiceInDevice = SoundIn.DeviceGetCorrectCasing(loaded_config("VoiceInDevice").value)
            VoiceThread.VoiceOutDevice = SoundOut.DeviceGetCorrectCasing(loaded_config("VoiceOutDevice").value)
            If loaded_config("VoiceInDevice").value.ToLower.Trim = "default" Then
                VoiceThread.VoiceInDevice = "Default"
            End If
            If loaded_config("VoiceOutDevice").value.ToLower.Trim = "default" Then
                VoiceThread.VoiceOutDevice = "Default"
            End If
            VoiceClient.AmpInput = loaded_config("VoiceAmpInput").value
            VoiceClient.AmpOutput = loaded_config("VoiceAmpOutput").value

            Globals.NoJoy = (loaded_config("EnableControllerSupport").value = "false")
            AntiCheatThreadAndMore.BigScreenMode = (loaded_config("BigScreenMode").value = "true")


        End If

        If Not IO.File.Exists(file_settings) Then
            WriteConfig(file_settings, default_config)
            LoadConfig()
            SplashScreen.options_btn.PerformClick()
            'Settings.Show()
        End If

        If GetCommandLineArg("-noupdate") <> 0 Then
            NoUpdate = True
        End If

    End Sub 'loads config file

    'Save updated launcher config
    Public Sub SaveConfig()
        If IO.File.Exists("LauncherSettings.txt") Then
            Try
                IO.File.Delete("LauncherSettings.txt")
            Catch ex As Exception
                MsgBox("Failed to save config because LauncherSettings.txt couldn't be deleted", MsgBoxStyle.Critical)
                Exit Sub
            End Try
        End If
        Try
            MakeUpdatedConfig()
        Catch ex As Exception
            MsgBox("Failed to save LauncherSettings.txt, reason: " & ex.Message, MsgBoxStyle.Critical)
        End Try
    End Sub
    Public Sub MakeUpdatedConfig()
        Dim output As New Dictionary(Of String, ConfigValue)(default_config, StringComparer.OrdinalIgnoreCase)
        output("CheckForUpdates").value = Settings.checkforupdatesbx.Checked
        output("LaunchNightfire").value = Settings.launchnf.Checked
        output("LaunchNightfire").value = Settings.launchnf.Checked
        output("AskBeforeDownloadingUpdates").value = Settings.askfordownloading.Checked
        output("RawMouseInput").value = Settings.rawmousebox.Checked
        output("DisableMouseAcceleration").value = Settings.disablemouseaccel.Checked
        output("WindowedMode").value = Settings.windowed.Checked
        output("Borderless").value = Settings.borderlesschk.Checked
        output("SkipIntroMovies").value = Settings.skipintro.Checked
        output("StartupArguments").value = Settings.commandlineoptions.Text
        output("FixScopesinWidescreen").value = Settings.fixscopez.Checked
        output("EnableVoiceChat").value = Settings.voiceenabled.Checked
        output("VoiceChatKey").value = Settings.voicechat_button.Text
        output("VoiceInDevice").value = Settings.recordingdevice.Text
        output("VoiceOutDevice").value = Settings.playbackdevice.Text
        output("VoiceAmpInput").value = Settings.recordingamplification.Value - 5
        output("VoiceAmpOutput").value = Settings.playbackamplification.Value - 5
        output("EnableControllerSupport").value = Settings.controllerenabled.Checked
        output("BigScreenMode").value = Settings.couchmode.Checked
        WriteConfig(file_settings, output)
    End Sub
    Private default_config As New Dictionary(Of String, ConfigValue)(StringComparer.OrdinalIgnoreCase)
    Private Sub InitDefaultConfig()
        With default_config
            .Clear()
            .Add("CheckForUpdates", ConfigVal("true"))
            .Add("ClassicTimer", ConfigVal("true", "This value is leftover from legacy code. Ignore it"))
            .Add("LaunchNightfire", ConfigVal("true"))
            .Add("AskBeforeDownloadingUpdates", ConfigVal("true"))
            .Add("RawMouseInput", ConfigVal("true", "Raw Mouse Input provides more consistent mouse responsiveness but might not be supported by all systems"))
            .Add("DisableMouseAcceleration", ConfigVal("true", "Set to 0 if you prefer classic 1.0 mouse movement. Setting it to 1 will make your mouse movement more accurate"))
            .Add("WindowedMode", ConfigVal("false"))
            .Add("Borderless", ConfigVal("false", "Enables borderless windowed mode"))
            .Add("SkipIntroMovies", ConfigVal("false", "Press escape at the console window to get to the main menu"))
            .Add("IgnoreMemoryCheck", ConfigVal("false", "If you want to manually set -hunk or -heap, set the above value to 1. This is not recommended to be set to 1."))
            .Add("StartupArguments", ConfigVal("", "You can add extra startup arguments here, such as +connect 127.0.0.1:26015 or other commands"))
            .Add("FixScopesinWidescreen", ConfigVal("true", "Set this to 1 if you want to automatically fix stretched scopes on widescreen resolutions"))
            .Add("EnableVoiceChat", ConfigVal("true", "Enables voice chat in-game. Uses the default microphone selected in Windows sound settings"))
            .Add("VoiceChatKey", ConfigVal("k", "Push to talk key"))
            .Add("VoiceInDevice", ConfigVal("default", "If set to default, the input will be set to your default device."))
            .Add("VoiceOutDevice", ConfigVal("default", "If set to default, the output will be set to your default device."))
            .Add("VoiceAmpInput", ConfigVal("0", "This is a value between -5 and 5 to reduce or amplify the volume."))
            .Add("VoiceAmpOutput", ConfigVal("0", "This is a value between -5 and 5 to reduce or amplify the volume."))
            .Add("EnableControllerSupport", ConfigVal("true", "Enables XInput controller support for 360 controllers and other devices"))
            .Add("BigScreenMode", ConfigVal("false", "Enables a lower field of view for players at longer distances from the TV screen or monitor"))
        End With
    End Sub
    Private Function LoadConfig(ByVal path As String) As Dictionary(Of String, ConfigValue)
        Dim output As New Dictionary(Of String, ConfigValue)(default_config, StringComparer.OrdinalIgnoreCase)
        Dim defaults As New List(Of String)
        For Each entry In default_config
            defaults.Add(entry.Key.ToLower)
        Next

        Dim read As New IO.StreamReader(path)
        While Not read.EndOfStream
            Dim line = read.ReadLine.Split("//")
            If Not line.Length > 0 Then Continue While
            Dim configdata = line(0).Split("=")

            If configdata.Length > 0 Then
                defaults.Remove(configdata(0))
                If configdata(0).Trim.Length = 0 Then
                    Continue While
                End If
            End If

            If configdata.Length = 1 Then
                Dim key = configdata(0)
                If Not output.ContainsKey(key) Then
                    output.Add(key, ConfigVal(""))
                End If
                If output.ContainsKey(key) Then
                    output(key).value = ""
                End If
            ElseIf configdata.Length = 2 Then
                Dim key = configdata(0)
                If Not output.ContainsKey(key) Then
                    output.Add(key, ConfigVal(configdata(1).ToLower.Trim))
                End If
                If output.ContainsKey(key) Then
                    output(key).value = configdata(1).ToLower.Trim
                End If
            End If
        End While
        read.Close()

        If defaults.Count > 0 Then
            WriteConfig(path, output)
        End If

        Return output
    End Function
    Private Sub WriteConfig(ByVal path As String, ByRef config As Dictionary(Of String, ConfigValue))
        Dim writer As New IO.StreamWriter(path, False)
        For Each entry In config
            writer.WriteLine(entry.Key & "=" & entry.Value.value.ToLower.Trim)
            If entry.Value.comment.Length > 0 Then
                writer.WriteLine("//" & entry.Value.comment)
            End If
            writer.WriteLine()
        Next
        writer.Flush()
        writer.Close()
    End Sub
    Private Function ConfigVal(ByVal value As String, Optional ByVal comment As String = "")
        Dim new_entry = New ConfigValue(value, comment)
        Return new_entry
    End Function
    Public Class ConfigValue
        Public value As String
        Public comment As String
        Public Sub New(ByVal value As String, Optional ByVal comment As String = "")
            Me.value = value
            Me.comment = comment
        End Sub
        Public Sub New()
        End Sub
    End Class

End Module
