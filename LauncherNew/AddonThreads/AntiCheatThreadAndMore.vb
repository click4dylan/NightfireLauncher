Imports System.Windows.Forms
Imports System.Text
Imports Microsoft.Win32
Imports System.Runtime.InteropServices


Class AntiCheatThreadAndMore
    Private thread As New Threading.Thread(AddressOf AntiCheatMain)
    Private exitthread As New Threading.Thread(AddressOf KillThread)
    Public Sub New()
        exitthread.Start() 'Stops crashes while in mid-loop
        thread.Start()
    End Sub
    Private Sub KillThread()
        While Not ExitGlobally
            If Globals.NightfireProcess.HasExited Then Exit While
            Threading.Thread.Sleep(5)
        End While
        Try
            thread.Abort()
        Catch ex As Exception
        End Try
    End Sub
    Private zoomed As Boolean = False
    Private userfov As Integer = 77
    Private haschangedresolution As Boolean = True
    Public Shared FixScopesinWidescreen As Boolean = True
    Public Shared BigScreenMode As Boolean = False

    'Public sys_anticheat As Int32 = &H449C9A28 + &H14
    Private Sub AntiCheatMain()
        While Not ExitGlobally 'main loop
            If Globals.NightfireProcess.HasExited Then Exit While
            Try
                cl_pitchup = (mem.ReadInt32(&H410B0814)) + &H14
                cl_pitchdown = (mem.ReadInt32(&H410B0818)) + &H14
                Try
                    userfov = GetFOV() 'get user's set fov from memory 410a8abc doesn't work on launch
                Catch ex As Exception
                End Try
                PreventHighFPS()
                If Not SinglePlayer() Then
                    DetectCheats()
                End If
                zoomed = False
                If GetFOV() < 65 Then zoomed = True
                If Not zoomed Then
                    ProcessNonZoomedTasks()
                End If
            Catch ex As Exception
            End Try
            Threading.Thread.Sleep(2)
        End While
    End Sub
    Private aspectratio As Single = 0
    Private Sub ProcessNonZoomedTasks()
        Try
            If Not aspectratio = GetAspectRatio() Then
                haschangedresolution = True
            End If
            aspectratio = GetAspectRatio()
        Catch ex As Exception 'stops crash when bond is closed, don't remove!
        End Try

        SetCorrectFOV(aspectratio)
        userfov = mem.ReadInt32(&H44B82F6E)
        If FixScopesinWidescreen Then
            FixScopes(aspectratio)
        End If
    End Sub
    Private Sub FixScopes(ByVal aspectratio As Single)
        If haschangedresolution Then
            Dim dir As String = IO.Directory.GetCurrentDirectory & "\bond\gui\hud\"
            If Not IO.Directory.Exists(dir) Then
                Try
                    If Not IO.Directory.Exists(IO.Directory.GetCurrentDirectory & "\bond\gui") Then
                        IO.Directory.CreateDirectory(IO.Directory.GetCurrentDirectory & "\bond\gui\")
                    End If
                    IO.Directory.CreateDirectory(IO.Directory.GetCurrentDirectory & "\bond\gui\hud")
                Catch ex As Exception
                    concmd("echo ERROR: " & ex.Message.ToString)
                End Try
            End If
            haschangedresolution = False
            If aspectratio >= 1.6 Then
                Try 'Widescreen
                    If IO.File.Exists(dir & "sniper_1024_wide.png") Then
                        IO.File.Copy(dir & "sniper_1024_wide.png", dir & "sniper_1024.png", True)
                    End If
                    If IO.File.Exists(dir & "scope_sig_wide.png") Then
                        IO.File.Copy(dir & "scope_sig_wide.png", dir & "scope_sig.png", True)
                    End If

                    Threading.Thread.Sleep(300)
                    concmd("r_flushtextures")
                Catch ex As Exception
                    concmd("echo ERROR: " & ex.Message.ToString)
                End Try
            Else
                Try 'Not Widescreen
                    If IO.File.Exists(dir & "sniper_1024.png") Then
                        IO.File.Delete(dir & "sniper_1024.png")
                    End If
                    If IO.File.Exists(dir & "scope_sig.png") Then
                        IO.File.Delete(dir & "scope_sig.png")
                    End If
                    Threading.Thread.Sleep(300)
                    concmd("r_flushtextures")
                Catch ex As Exception
                    concmd("echo ERROR: " & ex.Message.ToString)
                End Try
            End If
        End If
    End Sub
    Private Sub PreventHighFPS()
        Try
            If Not mem.Read(DEVELOPER, 2)(0) = 1 Then
                Dim maxfps As Single = mem.ReadSingle(fps_max)
                If maxfps > 150 Or maxfps = 0 Then
                    mem.Write(fps_max, CSng(150))
                End If
            End If
        Catch ex As Exception
        End Try
    End Sub
    Private Sub DetectCheats()
        'blocked = True
        'Dim newcheattick = Environment.TickCount
        'If newcheattick - oldcheattick >= 240000 Then
        '    oldcheattick = newcheattick
        '    Dim time As DateTime = DateTime.Now
        '    unreliable_concmd("say AntiCheat Date Check:" & time)
        'End If
        Dim gamever As String = System.Text.Encoding.ASCII.GetString(mem.Read(&H430E25CA, 13))
        If gamever.Contains("insecure") Then
            gamever = gamever.Replace("insecure", "secure  ")
            concmd("host_setinfo gamever " & ControlChars.Quote & gamever & ControlChars.Quote)
            'concmd("host_setinfo anticheat on")
            'concmd("say ENABLED ANTICHEAT " & DateTime.Now)
            'Threading.Thread.Sleep(3000)
            'concmd("cl_retry")
            mem.Write(&H430E25CA, System.Text.Encoding.ASCII.GetBytes(gamever))
        End If
        Dim zmin = mem.ReadSingle(r_zmin)
        If zmin < 1 Or zmin > 6 Then
            mem.Write(r_zmin, CSng(1))
            unreliable_concmd("say CHEAT DETECTED: r_zmin")
        End If
        If Not mem.ReadSingle(r_viewzmin) = 6 Then
            mem.Write(r_viewzmin, CSng(6))
            unreliable_concmd("say CHEAT DETECTED: r_viewzmin")
        End If
        If Not mem.ReadInt32(r_fullbright) = 0 Then
            mem.Write(r_fullbright, 0)
            unreliable_concmd("say CHEAT DETECTED: r_fullbright")
        End If
        If Not mem.ReadInt32(r_wireframe) = 0 Then
            mem.Write(r_wireframe, 0)
            unreliable_concmd("say CHEAT DETECTED: r_wireframe")
        End If
        'If Not mem.ReadInt32(r_fogdev) = 0 Then
        'mem.Write(r_fogdev, 0)
        'unreliable_concmd("say CHEAT DETECTED: r_fogdev")
        'End If
        'If Not mem.ReadInt32(chase_active) = 0 Then
        'mem.Write(chase_active, 0)
        'unreliable_concmd("say CHEAT DETECTED: chase_active")
        'End If
        If Not mem.ReadInt32(r_drawcliententities) = 1 Then
            mem.Write(r_drawcliententities, 1)
            unreliable_concmd("say CHEAT DETECTED: r_drawcliententities")
        End If
        If Not mem.ReadInt32(r_drawworldmodel) = 1 Then
            mem.Write(r_drawworldmodel, 1)
            unreliable_concmd("say CHEAT DETECTED: r_drawworldmodel")
        End If
        'If Not mem.ReadInt32(cam_tothirdperson) = 0 Then
        'mem.Write(cam_tothirdperson, 0)
        'unreliable_concmd("say CHEAT DETECTED: cam_tothirdperson")
        'End If
        If Not mem.ReadSingle(cl_pitchup) = 89 Then
            mem.Write(cl_pitchup, CSng(89))
            unreliable_concmd("say CHEAT DETECTED: cl_pitchup")
        End If
        If Not mem.ReadSingle(cl_pitchdown) = 89 Then
            mem.Write(cl_pitchdown, CSng(89))
            unreliable_concmd("say CHEAT DETECTED: cl_pitchdown")
        End If
        'If mem.ReadInt32(fps_max) > 100 Then
        '    mem.Write(fps_max, 100)
        '    unreliable_concmd("say CHEAT DETECTED: fps_max > 100")
        'End If
        'If mem.ReadInt32(r_fieldofview) < 76 Then
        '    mem.Write(r_fieldofview, 76)
        '    unreliable_concmd("say CHEAT DETECTED: r_fieldofview < 76")
        'ElseIf mem.ReadInt32(r_fieldofview) > 100 Then
        '    mem.Write(r_fieldofview, 100)
        '    unreliable_concmd("say CHEAT DETECTED: r_fieldofview > 100")
        'End If
        ''Else
        '    If blocked = True Then 'this looks bad, rewrite TODO
        '        If enabled = True Then
        '            blocked = False
        '        End If
        '    ElseIf blocked = False Then
        '        If enabled = False Then
        '            blocked = True
        '        End If
        '    End If
        'Dim gamever As String = System.Text.Encoding.ASCII.GetString(mem.Read(&H430E25CA, 13))
        'If gamever.EndsWith("secure  ") Then
        '    gamever = gamever.Replace("secure  ", "insecure")
        '    mem.Write(&H430E25CA, System.Text.Encoding.ASCII.GetBytes(gamever))
        '    concmd("host_setinfo gamever " & ControlChars.Quote & gamever & ControlChars.Quote)
        '    concmd("host_setinfo anticheat off")
        '    concmd("say DISABLED ANTICHEAT " & DateTime.Now)
        '    Threading.Thread.Sleep(3000)
        '    concmd("cl_retry")
        'End If
    End Sub
    Private Sub SetCorrectFOV(ByVal aspectratio As Single)
        If BigScreenMode Then
            SetBigScreenFOV(aspectratio)
        Else
            SetNormalFOV(aspectratio)
        End If
    End Sub
    Private Sub SetNormalFOV(ByVal aspectratio As Single)
        Select Case aspectratio
            Case Is <= 1.0
                SetFOV(65)
            Case Is <= 1.25
                SetFOV(78)
            Case Is <= 1.33
                SetFOV(76)
            Case Is <= 1.4
                SetFOV(80)
            Case Is <= 1.5
                SetFOV(85)
            Case Is <= 1.6
                SetFOV(90)
            Case Is <= 1.78
                SetFOV(100)
            Case Is <= 1.9
                SetFOV(110)
            Case Is >= 2.0
                SetFOV(120)
            Case Is >= 3.0
                SetFOV(128)
            Case Is >= 4.0
                SetFOV(135)
            Case Is >= 5.0
                SetFOV(145)
            Case Is >= 5.4
                SetFOV(150)
            Case Is >= 6.0
                SetFOV(160)
            Case Is >= 7.0
                SetFOV(170)
            Case Is >= 8.0
                SetFOV(179)
        End Select
    End Sub
    Private Sub SetBigScreenFOV(ByVal aspectratio As Single)
        Select Case aspectratio
            Case Is <= 1.25
                SetFOV(65)
                SetWeaponFOV(-4)
            Case Is <= 1.33
                SetFOV(65)
                SetWeaponFOV(-4)
            Case Is <= 1.4
                SetFOV(65)
                SetWeaponFOV(-6)
            Case Is <= 1.5
                SetFOV(65)
                SetWeaponFOV(-7)
            Case Is <= 1.6
                SetFOV(65)
                SetWeaponFOV(-7)
            Case Is <= 1.78 '16/9 res
                SetFOV(65)
                SetWeaponFOV(-7) 'Below = no couch mode! Eyefinity/Surround users are left in the dark for this.. I don't feel like testing correct values for it
            Case Is <= 1.9
                SetFOV(110)
                SetWeaponFOV(0)
            Case Is >= 2.0
                SetFOV(120)
                SetWeaponFOV(0)
            Case Is >= 3.0
                SetFOV(128)
                SetWeaponFOV(0)
            Case Is >= 4.0
                SetFOV(135)
                SetWeaponFOV(0)
            Case Is >= 5.0
                SetFOV(145)
                SetWeaponFOV(0)
            Case Is >= 5.4
                SetFOV(150)
                SetWeaponFOV(0)
            Case Is >= 6.0
                SetFOV(160)
                SetWeaponFOV(0)
            Case Is >= 7.0
                SetFOV(170)
                SetWeaponFOV(0)
            Case Is >= 8.0
                SetFOV(179)
                SetWeaponFOV(0)
        End Select
    End Sub
End Class

