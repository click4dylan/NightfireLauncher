Imports Microsoft.Win32
Public Class Main
    Public Sub New()
        Dim MainThread = New Threading.Thread(AddressOf MainLaunch)
        MainThread.Start()
    End Sub
    'Private Sub CheckForExit()
    '    While Not ExitGlobally
    '        Threading.Thread.Sleep(10)
    '    End While
    '    CloseForm(ServerBrowser)
    '    CloseForm(UpdateMessageBox)
    '    CloseForm(InputServerPassword)
    '    CloseForm(AddAServer)
    '    CloseForm(Settings)
    '    CloseForm(SplashScreen)
    'End Sub
    Public Sub MainLaunch()
        DoStartupTasks()
        LoadConfig()
        DelayStartup()
        If SplashScreen.InvokeRequired Then
            SplashScreen.Invoke(Sub()
                                    SplashScreen.TopMost = False
                                    SplashScreen.Invalidate()
                                End Sub)
        Else
            SplashScreen.TopMost = False
            SplashScreen.Invalidate()
        End If
        CheckRAM()
RestartNightfire:
        ShowForm(SplashScreen)
        SplashScreen.DisableButtons()
        If NoUpdate Then
            SetLabelText(SplashScreenForm.SplashScreenInfoLabel, "Update check has been disabled")
            DelayRuntime(0.1)
            If Re_StartNightfire() Then
                GoTo RestartNightfire
            End If
        Else
            SetLabelText(SplashScreenForm.SplashScreenInfoLabel, "Checking for updates from NightfirePC.com")
            If GetDownloadSocket() Then
                If Not CheckAndDownloadForUpdates() Then
                    SetLabelText(SplashScreenForm.SplashScreenInfoLabel, "Exiting!")
                    DelayRuntime(2)
                    ExitGlobally = True
                    Exit Sub
                End If
            End If
            If Re_StartNightfire() Then
                GoTo RestartNightfire
            End If
        End If
        'All Done, Close everything
        ExitGlobally = True
        CloseForm(SplashScreen)
        CloseForm(Settings)
        CloseForm(ServerBrowser)
        CloseForm(AddAServer)
        CloseForm(InputServerPassword)
        CloseForm(UpdateMessageBox)
    End Sub
    Private Sub DelayStartup()
        Dim AbsStartTime = Sys_FloatTime()
        Dim FormsAreOpen As Boolean = (FormIsOpen(Settings)) Or (FormIsOpen(ServerBrowser))
        While ((Sys_FloatTime() - AbsStartTime) < 3 Or FormsAreOpen)
            If ExitGlobally Then Exit While
            If Not FormsAreOpen Then
                If SplashScreen.InvokeRequired Then
                    SplashScreen.Invoke(Sub()
                                            SplashScreen.Invalidate()
                                            SplashScreen.TopMost = True
                                        End Sub)
                Else
                    SplashScreen.Invalidate()
                    SplashScreen.TopMost = True
                End If
            End If
            Application.DoEvents()
            Threading.Thread.Sleep(2)
            FormsAreOpen = (FormIsOpen(Settings)) Or (FormIsOpen(ServerBrowser))
        End While
    End Sub
    Private Sub SetBond2ToCompatibilityMode()
        Dim OSVersion = Environment.OSVersion.Version
        If OSVersion.Major > 5 Then
            Try
                Dim dir As String = System.IO.Directory.GetCurrentDirectory & "\"
                Dim regKey As RegistryKey = Registry.CurrentUser.OpenSubKey("Software\Microsoft\Windows NT\CurrentVersion\AppCompatFlags\Layers", True)
                Dim result As String = regKey.GetValue(dir & BondEXE, Nothing)
                If result = Nothing Then
                    regKey.SetValue(dir & BondEXE, "WINXPSP3 DISABLEDWM RUNASADMIN", RegistryValueKind.String)
                Else
                    If Not result.Contains("WINXPSP3") Then
                        regKey.SetValue(dir & BondEXE, result & " WINXPSP3", RegistryValueKind.String)
                    End If

                    If Not result.Contains("DISABLEDWM") Then
                        regKey.SetValue(dir & BondEXE, result & " DISABLEDWM", RegistryValueKind.String)
                    End If

                    If Not result.Contains("RUNASADMIN") Then
                        regKey.SetValue(dir & BondEXE, result & " RUNASADMIN", RegistryValueKind.String)
                    End If
                End If
            Catch ex As Exception
            End Try
        End If
    End Sub
    Private Sub DoStartupTasks()
        SetBond2ToCompatibilityMode()

        If IO.File.Exists(IO.Directory.GetCurrentDirectory & "\d3d8.dll") Then
            Dim d3d8hash = Hash(IO.Directory.GetCurrentDirectory & "\d3d8.dll")
            If d3d8hash = "42CD3139B69A53F140BD18C063F72D3F" Then
                DeleteFile(IO.Directory.GetCurrentDirectory & "\d3d8.dll")
            End If
        End If
        DeleteIfExists("Toolbox.dll")
        DeleteIfExists("gpcomms.dll")
        DeleteIfExists("MapDownloader.exe")
        DeleteIfExists("fpsboost.exe")
        ProcessesKillAll("Updater")
        DeleteIfExists("Updater.exe")

        If GetCommandLineArg("-tobrowser") <> NULL Then
            ShowForm(ServerBrowser)
        Else
            'ShowForm(SplashScreen)
            FormActivate(SplashScreen)
            KillConcurrentProcesses()
        End If

        CPath = Environment.GetCommandLineArgs(0)
        If IO.File.Exists("engine.dll") Then EngineHash = Hash("engine.dll")
    End Sub
    Public Shared Function Re_StartNightfire() As Boolean
        If ExitGlobally Then Return False
        If NoUpdate Then
            If PressedNoToUpdate = True Then
                SetLabelText(SplashScreenForm.SplashScreenInfoLabel, "Skipped downloading updates")
            Else
                SetLabelText(SplashScreenForm.SplashScreenInfoLabel, "Skipped checking/downloading updates")
            End If
            DelayRuntime(0.2)
        End If
        If DontLaunchNF Then
            SetLabelText(SplashScreenForm.SplashScreenInfoLabel, "Bond is set to not run, closing.")
            DelayRuntime(0.5)
            ExitGlobally = True
            Return False
        End If
        Dim dir As String = System.IO.Directory.GetCurrentDirectory & "\"
        If GetCommandLineArg("-dedicated") <> NULL Then : BondEXE = "Bond_ded.exe" : Else : BondEXE = "Bond2.exe" : End If
        Try
            IO.Directory.CreateDirectory(dir) 'used for getting proper directory name
        Catch ex As Exception
        End Try
        If Not GetCommandLineArg("-dedicated") Then
            'Check if Bond2.exe exists and is the newest one
            If IO.File.Exists(dir & BondEXE) = True Then
                If Not Hash(BondEXE) = BOND2HASH Then
                    SetLabelText(SplashScreenForm.SplashScreenInfoLabel, "Attempting to write Bond2.exe, Please Wait!")
                    TryWritingBond2()
                End If
            Else
                'Create Bond2.exe
                Try
                    Dim outbond As New IO.FileStream(IO.Directory.GetCurrentDirectory & "\" & BondEXE, System.IO.FileMode.Create, System.IO.FileAccess.Write)
                    outbond.Write(My.Resources.Bond2, 0, My.Resources.Bond2.Length)
                    outbond.Flush()
                    outbond.Close()
                    'IO.File.WriteAllBytes(tp & BondEXE, My.Resources.Bond_qpc) 'unzip Bond2.exe which is the actual game exe
                Catch ex As Exception
                    MsgBox("Failed to write " & BondEXE & ", reason: " & ex.Message, MsgBoxStyle.Critical)
                End Try
            End If
        End If
        Dim starttime As Double = 0
        Try
            Application.DoEvents()
            If DontLaunchNF Then
                SetLabelText(SplashScreenForm.SplashScreenInfoLabel, "LauncherSettings.txt is set to not launch Nightfire. Closing launcher.")
                DelayRuntime(1)
                ExitGlobally = True
                Return False
            Else
                SetLabelText(SplashScreenForm.SplashScreenInfoLabel, "Ready, Launching Nightfire")
                HideForm(SplashScreen)
                'Set memory allocations to bond
                Dim temphunk = GetCommandLineArg("-hunk")
                If temphunk <> NULL Then
                    Hunk = temphunk
                End If
                Dim tempheap = GetCommandLineArg("-heap")
                If temphunk <> NULL Then
                    Heap = tempheap
                End If
                ExtraCommandLineArgs &= GetCommandLineArgs()
                ExtraCommandLineArgs &= "-heap " & Heap.ToString & " "
                ExtraCommandLineArgs &= "-hunk " & Hunk.ToString & " "
                Dim tempargs As String = ExtraCommandLineArgs & " "
                If ConnectArg.Length > 0 Then tempargs &= ConnectArg
                Dim nfinfo As New ProcessStartInfo(dir & BondEXE, tempargs) 'Required for Windows XP or bond won't start properly
                nfinfo.UseShellExecute = False
                NightfireProcess = Process.Start(nfinfo)
                starttime = Sys_FloatTime()
            End If
#If 1 Then
            If GetCommandLineArg("-dedicated") = 0 Then
                DelayRuntime(0.5)
                While Not Bond2Mem.Inject()
                    If NightfireProcess.HasExited Or ExitGlobally Then
                        ExitGlobally = True
                        Return False
                    End If
                    Bond2Mem.Inject()
                    Threading.Thread.Sleep(1)
                End While
                If RawMouseInput Then
                    If GetCommandLineArg("-rinput") = 0 Then
                        ExtraCommandLineArgs &= "-rinput"
                    End If
#If 0 Then
                    If IO.File.Exists(IO.Directory.GetCurrentDirectory & "\RawInput.dll") Then
                        Try
                            InjectFile("RawInput.dll")
                        Catch ex As Exception
                        End Try
                    End If
#End If
                End If
                Dim mapdownload = New MapDownloaderThread
                Dim anticheat = New AntiCheatThreadAndMore
                Dim voicet = New VoiceThread
                Dim fixmapbugs = New Fix_Game_Bugs
                Dim joy = Nothing
                Dim xinputpath As String = Environment.GetFolderPath(Environment.SpecialFolder.System) & "\xinput1_3.dll"
                If Not IO.File.Exists(xinputpath) And NoJoy = False Then
                    NoJoy = True
                    MsgBox("Failed to find " & xinputpath & "! Controller support will be disabled. You can disable this message by turning off XInput controller support in the launcher options.", MsgBoxStyle.Critical)
                End If
                If Not NoJoy Then
                    joy = New XInputController
                End If
            End If
#End If
            While Not NightfireProcess.HasExited
                If ExitGlobally Then Exit While
                Application.DoEvents()
                Threading.Thread.Sleep(20) 'prevent high cpu load
            End While
        Catch ex As Exception
            MsgBox("Failed to start Nightfire, reason: " & ex.Message, MsgBoxStyle.Critical)
            Return False
        End Try

        'NIGHTFIRE HAS CLOSED, SHUT DOWN OTHER PROCESSES
        Try
            ShowForm(SplashScreen)
            SplashScreen.TopMost = True
            VoiceThread.Voice_Close()
            'mem.Close() 'Don't crash on exit, no reason to close handle if process is dead..

            'DrawSplashThread.Resume()
            'Me.Visible = True
            SetLabelText(SplashScreenForm.SplashScreenInfoLabel, "Nightfire has closed")
            'RestoreD3D8ProxyOnWindows8() 'Restore the proxy on Win8 so the auto updater doesn't go berserk
            restart = False
            If Sys_FloatTime() - starttime < 10 Then
                Dim restartmsg = MsgBox("Nightfire has exited within 10 seconds of launching. Would you like to restart it with fail-safe defaults?", MsgBoxStyle.YesNo)
                If restartmsg = MsgBoxResult.Yes Then restart = True
            End If
            If restart Then
                SetLabelText(SplashScreenForm.SplashScreenInfoLabel, "Shutting down and restarting")
                NoUpdate = True
                SplashScreen.TopMost = False
                Return True
            End If
            SetLabelText(SplashScreenForm.SplashScreenInfoLabel, "Nightfire closed. Shutting down.")
        Catch ex As Exception
            MsgBox("Failed to shut down launcher, reason: " & ex.Message, MsgBoxStyle.Critical)
        End Try
        Return False
    End Function
    Private Sub KillConcurrentProcesses()
        Dim bond2 = GetProcessesByName("Bond2")
        For Each bond_proc In bond2
            Dim msg = MsgBox("Nightfire is already running, would you like to close it? Automatic updates and map downloading will be disabled if you leave it running.", MsgBoxStyle.YesNoCancel, "Launcher")
            If msg = MsgBoxResult.No Then
                NoUpdate = True
            ElseIf msg = MsgBoxResult.Cancel Then
                ExitGlobally = True
            Else
                While bond2.Length > 0
                    Application.DoEvents()
                    Threading.Thread.Sleep(1)
                    For Each a In bond2
                        Try
                            a.Kill()
                        Catch ex As Exception
                            Exit While
                        End Try
                    Next
                    bond2 = GetProcessesByName("Bond2")
                End While
                ProcessesKillExceptSelf("Bond")
            End If
            Exit For
        Next
    End Sub
    Private Sub CheckRAM()
        Dim commit = My.Computer.Info.AvailableVirtualMemory / 1024 / 1024 'convert bytes to MB
        Dim ram As ULong = My.Computer.Info.AvailablePhysicalMemory / 1024 / 1024 'convert bytes to MB
        SetLabelText(SplashScreenForm.SplashScreenInfoLabel, "Available RAM: " & ram & " MB")
        If Not Globals.restart Then
            Select Case ram
                Case Is <= 128
                    'Hunk = 20 'compatibility with original system requirements
                    Hunk = 28
                    Heap = 128
                Case Is <= 256
                    Hunk = 40
                    Heap = 128
                Case Is <= 384
                    Hunk = 64
                    Heap = 192
                Case Is <= 512
                    Hunk = 128
                    Heap = 256
                Case Else
                    Hunk = 256
                    Heap = 384
            End Select
        Else
            RawMouseInput = False 'not supported by all systems
            Hunk = 20 'fail safe defaults (1.0 default hunk/heap)
            Heap = 128
        End If
    End Sub
End Class
