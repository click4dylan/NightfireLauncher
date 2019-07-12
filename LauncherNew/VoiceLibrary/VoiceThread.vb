Class VoiceThread

    'Globals
    Private voice_host As String = "voip1.nightfirepc.com"
    Private Shared voice_threadactive As Boolean
    Private voice_client As VoiceClient
    Private voice_port As Integer = 10
    Public Shared voice_activatekey As Integer = Keys.K

    'Config
    Public Shared VoiceEnabled As Boolean = True
    Public Shared VoiceKey As String = ""
    Public Shared VoiceOutDevice As String = ""
    Public Shared VoiceInDevice As String = ""
    Private exitthread As New Threading.Thread(AddressOf KillThread)
    Private thread As New Threading.Thread(AddressOf Voice_Main)
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
    Private Sub Voice_Main()
        If Not VoiceEnabled Then Exit Sub
#If DEBUG Then
        'voice_host = "voip2.nightfirepc.com"
#End If
        DelayRuntime(5)
        'Initilization code
        voice_activatekey = KeyCodeFromString(VoiceKey.ToLower)
        Dim gameinfo = Game_GetPlayerInfo()
        voice_threadactive = True
        If VoiceOutDevice = "" Or VoiceInDevice = "" Then
            Try
                voice_client = New VoiceClient(voice_host, voice_port, "", gameinfo("cl_name"), 0, 0)
            Catch ex As Exception
                If Not ex.Message.Contains("Thread was being aborted") Then
                    MsgBox("CRITICAL ERROR on VoiceThread.vb (voice_client = New VoiceClient #1). Please send this error message to click4dylan@live.com so that we can fix this problem in the future: " & ControlChars.NewLine & ex.Message, MsgBoxStyle.Critical)
                End If
                Exit Sub
            End Try
        Else
            Try
                voice_client = New VoiceClient(voice_host, voice_port, "", gameinfo("cl_name"), SoundOut.NameToID(VoiceOutDevice), SoundIn.NameToID(VoiceInDevice))
            Catch ex As Exception
                If Not ex.Message.Contains("Thread was being aborted") Then
                    MsgBox("CRITICAL ERROR on VoiceThread.vb (voice_client = New VoiceClient #2). Please send this error message to click4dylan@live.com so that we can fix this problem in the future: " & ControlChars.NewLine & ex.Message, MsgBoxStyle.Critical)
                End If
                Exit Sub
            End Try
        End If
        Dim voiceOverlayThread As New Threading.Thread(AddressOf Menu_Draw)
        voiceOverlayThread.Start() 'This is done as a new thread to make it draw as fast as possible to prevent flicker

        'Main loop
        While voice_threadactive
            If ExitGlobally Then Exit While
            If Globals.NightfireProcess.HasExited Then Exit While
            Dim keydown = KeyGetState(voice_activatekey)
            Dim connected = Game_IsConnected()
            Dim foreground = GetWindowText.Text.Equals("Nightfire")
            Dim noconsoleorchat = (mem.Read(&H4310B7A8, 1)(0) = 0)
            If noconsoleorchat And foreground Then Menu_ProcessMenuKeys()
            Dim activestream = (connected And keydown And noconsoleorchat) 'And foreground)

            'Volume control
            Dim sndmastervol = ClampFloat(mem.ReadSingle(&H449BCCFC) * 3, 0, 1)
            voice_client.SetMasterOutputVolume(sndmastervol)

            If connected Then
                'Channel for current server
                gameinfo = Game_GetPlayerInfo()
                voice_client.SetName(gameinfo("cl_name"))
                Voice_SetChannel(Game_GetCurrentServer)
            Else
                'Reset to lobby channel
                Voice_SetChannel()
            End If

            If activestream Then
                voice_client.ActivateStream()
            Else
                Threading.Thread.Sleep(200)
                voice_client.DeactivateStream()
            End If
            Threading.Thread.Sleep(1)
        End While
        voice_client.Dispose()
    End Sub
    Public Sub Voice_SetChannel(Optional ByVal channel As String = "")
        voice_client.SetChannel(channel)
    End Sub
    Public Shared Sub Voice_Close()
        voice_threadactive = False
    End Sub

    'TO DO: Add check for main menu

    Public players_id As New Dictionary(Of Integer, String)
    Public players_names As New Dictionary(Of Integer, String)
    Public players_muted As New List(Of String)
    Public players_speaking As String()
    Public menuActive, menuCurrentlySelectedItem, menuNumItems As Byte

    Enum menu As Byte
        Off = 0
        Main = 1
        MutePlayers = 2
        VoiceSettings = 3
    End Enum
    Private menu_x As Integer = 256
    Private menu_y As Integer = 32
    Private desiredfps As Single = 0
    Private Sub LimitFPS(ByVal limiton As Boolean)
        Try
            Dim maxfps As Single = mem.ReadSingle(fps_max)
            If maxfps > 26.2 And maxfps < 26.4 And desiredfps = 0 Then
                desiredfps = 100 'game started with 26.3 fps!!! set it to 100 because bond crashed before restore previously.
                maxfps = 100
                mem.Write(fps_max, CSng(100))
            ElseIf desiredfps = 0 Then
                desiredfps = maxfps
            Else
                If limiton = False Then
                    If maxfps > 26.2 And maxfps < 26.4 Then
                        mem.Write(fps_max, CSng(desiredfps))
                    End If
                    desiredfps = mem.ReadSingle(fps_max)
                Else
                    If Not maxfps < 26.4 Then
                        mem.Write(fps_max, CSng(26.3))
                    End If
                End If
            End If
        Catch ex As Exception
        End Try
    End Sub
    Private Sub Menu_Draw()
        While Not Globals.NightfireProcess.HasExited
            If Globals.ExitGlobally Then Exit Sub
            Dim currentlydrawnitem As Byte = 0
            menu_y = 32

            'Remove non-existant users
            Dim players = voice_client.GetUsers
            players_id.Clear()
            SyncLock players
                Dim i = 0
                For Each player In players
                    players_id.Add(i, player.Key)
                    If Not players_names.ContainsKey(i) Then
                        players_names.Add(i, player.Value)
                    End If
                    players_names(i) = player.Value
                    i += 1
                Next
            End SyncLock

            If menuActive > menu.Off Then
                LimitFPS(True)
                Select Case menuActive
                    Case menu.Main
                        'ShowText("NIGHTFIRE LAUNCHER MENU", "Arial", 14, Color.Beige, Color.Beige, Nothing, FontStyle.Bold, menu_x, menu_y)
                        DrawTextOverlay("NIGHTFIRE LAUNCHER MENU", "Arial", 14, 255, 255, 255, FontStyle.Bold, menu_x, menu_y)
                        menu_y += 32
                        menuNumItems = 2
                        Menu_DrawMenuItem(currentlydrawnitem, "Mute Players")
                        menu_y += 16
                        Menu_DrawMenuItem(currentlydrawnitem + 1, "Voice Settings")
                        Exit Select
                    Case menu.MutePlayers
                        'ShowText("Muted Players", "Arial", 14, Color.Beige, Color.Beige, Nothing, FontStyle.Italic, menu_x, menu_y)
                        DrawTextOverlay("Muted Players", "Arial", 14, 255, 255, 255, FontStyle.Italic, menu_x, menu_y)
                        menu_y += 32
                        menuNumItems = players_id.Count
                        For Each player In players_id
                            Menu_DrawMenuItem(currentlydrawnitem, players_names(player.Key), players_muted.Contains(player.Value))
                            currentlydrawnitem += 1
                            menu_y += 16
                        Next
                        Exit Select
                    Case menu.VoiceSettings
                        DrawTextOverlay("Voice Settings", "Arial", 14, 255, 255, 255, FontStyle.Italic, menu_x, menu_y)
                        menu_y += 32
                        menuNumItems = 4
                        Menu_DrawMenuItem(currentlydrawnitem, "Playback Amplifier: " & VoiceClient.AmpOutput)
                        menu_y += 16
                        Menu_DrawMenuItem(currentlydrawnitem + 1, "Recording Amplifier: " & VoiceClient.AmpInput)
                        menu_y += 16
                        Dim playback = SoundOut.GetDevices
                        Menu_DrawMenuItem(currentlydrawnitem + 2, "Playback Device: " & playback(voice_client.sound_out.GetDevice))
                        menu_y += 16
                        Dim recording = SoundIn.GetDevices
                        Menu_DrawMenuItem(currentlydrawnitem + 3, "Recording Device: " & recording(voice_client.sound_in.GetDevice))
                        Exit Select
                End Select
            Else
                LimitFPS(False)
            End If
            Menu_DrawPlayersCurrentlySpeaking()
            Threading.Thread.Sleep(2)
        End While
    End Sub
    Private Sub Menu_DrawPlayersCurrentlySpeaking()
        'todo: don't draw if player is muted
        Dim players_speaking = voice_client.GetTalking
        If players_speaking.Length > 0 Then
            'ShowText("PLAYERS SPEAKING", "Arial", 11, Color.Beige, Color.Beige, Nothing, Drawing.FontStyle.Bold, 32, 32)
            DrawTextOverlay("PLAYERS SPEAKING", "Arial", 11, 255, 255, 255, Drawing.FontStyle.Bold, 32, 32)
            Dim PlayerList As String = ""
            For Each player In players_speaking
                PlayerList &= player & ControlChars.NewLine
                'this prevents having to call ShowText for each player, reducing CPU cycles dramatically
            Next
            'ShowText(PlayerList, "Arial", 10, Color.GreenYellow, Color.GreenYellow, Nothing, Drawing.FontStyle.Regular, 32, 32 + 16)
            DrawTextOverlay(PlayerList, "Arial", 10, 181, 230, 29, Drawing.FontStyle.Regular, 32, 32 + 16)
        End If
    End Sub
    Private Sub Menu_DrawMenuItem(ByVal index As Byte, ByVal item As String, Optional ByVal isHighlighted As Boolean = False)
        If index = menuCurrentlySelectedItem Then
            'Draw the currently selected menu item
            If isHighlighted Then
                'ShowText(item, "Arial", 12, Color.Orange, Color.Orange, Nothing, FontStyle.Bold, menu_x, menu_y) 'If player is muted, show as orange if we're selecting them
                DrawTextOverlay(item, "Arial", 12, 255, 128, 0, FontStyle.Bold, menu_x, menu_y) 'If player is muted, show as orange if we're selecting them
            Else
                'ShowText(item, "Arial", 12, Color.Aquamarine, Color.Aquamarine, Nothing, FontStyle.Bold, menu_x, menu_y) 'Otherwise show as green
                DrawTextOverlay(item, "Arial", 12, 0, 255, 255, FontStyle.Bold, menu_x, menu_y) 'Otherwise show as green
            End If
        Else
            'Draw other items in the menu
            If isHighlighted Then
                'ShowText(item, "Arial", 10, Color.Red, Color.Red, Nothing, FontStyle.Regular, menu_x, menu_y) 'If player is muted, show as red
                DrawTextOverlay(item, "Arial", 10, 255, 255, 0, FontStyle.Regular, menu_x, menu_y) 'If player is muted, show as red
            Else
                'ShowText(item, "Arial", 10, Color.White, Color.White, Nothing, FontStyle.Regular, menu_x, menu_y) 'Otherwise show as white
                DrawTextOverlay(item, "Arial", 10, 255, 255, 255, FontStyle.Regular, menu_x, menu_y) 'Otherwise show as white
            End If
        End If
    End Sub
    Public lastpresstime As Double = 0
    Private Sub Menu_ProcessMenuKeys()
        Dim time = Sys_FloatTime()
        If time - lastpresstime < 0.1 Then
            Exit Sub
        End If
        lastpresstime = time
        If KeyGetState(KeyCodeFromString("delete")) Then
            If menuActive > menu.Off Then
                menuCurrentlySelectedItem = 0
                menuActive = menu.Off 'Hide menu
            Else
                menuCurrentlySelectedItem = 0
                menuActive = menu.Main 'Show main menu
            End If
        ElseIf KeyGetState(KeyCodeFromString("up")) And menuActive > menu.Off And Not menuNumItems = 0 Then
            If menuCurrentlySelectedItem = 0 Then
                menuCurrentlySelectedItem = menuNumItems - 1 'Select last item in list 'todo random crash here, test it
            Else
                menuCurrentlySelectedItem = menuCurrentlySelectedItem - 1 'Move up in list
            End If
        ElseIf KeyGetState(KeyCodeFromString("down")) And menuActive > menu.Off And Not menuNumItems = 0 Then
            If menuCurrentlySelectedItem = menuNumItems - 1 Then
                menuCurrentlySelectedItem = 0 'Select first item in list
            Else
                menuCurrentlySelectedItem = menuCurrentlySelectedItem + 1 'Move down in list
            End If
        ElseIf KeyGetState(KeyCodeFromString("right")) Or KeyGetState(KeyCodeFromString("enter")) And menuActive > menu.Off Then
            Select Case menuActive
                Case menu.Main
                    If menuCurrentlySelectedItem = 0 Then
                        menuActive = menu.MutePlayers
                    ElseIf menuCurrentlySelectedItem = 1 Then
                        menuCurrentlySelectedItem = 0
                        menuActive = menu.VoiceSettings
                    End If
                    Exit Select
                Case menu.MutePlayers
                    Menu_ToggleMutePlayer(menuCurrentlySelectedItem)
                    Exit Select
                Case menu.VoiceSettings
                    'nothing to do here yet
                    Exit Select
            End Select
        ElseIf KeyGetState(KeyCodeFromString("left")) Or KeyGetState(KeyCodeFromString("escape")) And menuActive > menu.Off Then
            'Go to main menu, or close it
            If menuActive = menu.Main Then
                menuActive = menu.Off
            Else
                menuCurrentlySelectedItem = 0
                menuActive = menu.Main
            End If
        ElseIf KeyGetState(KeyCodeFromString("+")) Or KeyGetState(KeyCodeFromString("pgup")) And menuActive > menu.Off Then
            If menuActive = menu.VoiceSettings Then
                Dim amplifier As Integer = 0
                If menuCurrentlySelectedItem = 0 Then 'Playback Amplifier
                    amplifier = VoiceClient.AmpOutput + 1
                    amplifier = ClampInteger(amplifier, -20, 20)
                    VoiceClient.AmpOutput = amplifier
                ElseIf menuCurrentlySelectedItem = 1 Then 'Recording Amplifier
                    amplifier = VoiceClient.AmpInput + 1
                    amplifier = ClampInteger(amplifier, -20, 20)
                    VoiceClient.AmpInput = amplifier
                ElseIf menuCurrentlySelectedItem = 2 Then 'Playback Device
                    voice_client.DeltaOutDevice(1)
                ElseIf menuCurrentlySelectedItem = 3 Then 'Recording Device
                    voice_client.DeltaInDevice(1)
                End If
            End If
        ElseIf KeyGetState(KeyCodeFromString("-")) Or KeyGetState(KeyCodeFromString("pgdn")) And menuActive > menu.Off Then
            If menuActive = menu.VoiceSettings Then
                Dim amplifier As Integer = 0
                If menuCurrentlySelectedItem = 0 Then 'Playback Amplifier
                    amplifier = VoiceClient.AmpOutput - 1
                    amplifier = ClampInteger(amplifier, -20, 20)
                    VoiceClient.AmpOutput = amplifier
                ElseIf menuCurrentlySelectedItem = 1 Then 'Recording Amplifier
                    amplifier = VoiceClient.AmpInput - 1
                    amplifier = ClampInteger(amplifier, -20, 20)
                    VoiceClient.AmpInput = amplifier
                ElseIf menuCurrentlySelectedItem = 2 Then 'Playback Device
                    voice_client.DeltaOutDevice(-1)
                ElseIf menuCurrentlySelectedItem = 3 Then 'Recording Device
                    voice_client.DeltaInDevice(-1)
                End If
            End If
        End If
    End Sub
    Private Sub Menu_ToggleMutePlayer(ByVal menuid As Byte)
        If players_id.ContainsKey(menuid) Then
            Dim clientid = players_id(menuid)
            If players_muted.Contains(clientid) Then
                'unmute
                players_muted.Remove(clientid)
                voice_client.SetMute(clientid, False)
            Else
                players_muted.Add(clientid)
                voice_client.SetMute(clientid, True)
            End If
        End If
    End Sub

    'Input key functions
    Private Function KeyGetState(ByVal key As Integer) As Integer
        Return ProcessMemory.GetAsyncKeyState(key) 'And &HFF00US
    End Function
    Private Function KeyCodeFromString(ByVal key As String)
        Select Case key
            'Case "<"
            'Case ">"
            'Case "="
            'Case "?"
            Case ""
            Case "0" : Return &H30
            Case "1" : Return &H31
            Case "2" : Return &H32
            Case "3" : Return &H33
            Case "4" : Return &H34
            Case "5" : Return &H35
            Case "6" : Return &H36
            Case "7" : Return &H37
            Case "8" : Return &H38
            Case "9" : Return &H39
            Case "a" : Return &H41
            Case "b" : Return &H42
            Case "c" : Return &H43
            Case "d" : Return &H44
            Case "e" : Return &H45
            Case "f" : Return &H46
            Case "g" : Return &H47
            Case "h" : Return &H48
            Case "i" : Return &H49
            Case "j" : Return &H4A
            Case "k" : Return &H4B
            Case "l" : Return &H4C
            Case "m" : Return &H4D
            Case "n" : Return &H4E
            Case "o" : Return &H4F
            Case "p" : Return &H50
            Case "q" : Return &H51
            Case "r" : Return &H52
            Case "s" : Return &H53
            Case "t" : Return &H54
            Case "u" : Return &H55
            Case "v" : Return &H56
            Case "w" : Return &H57
            Case "x" : Return &H58
            Case "y" : Return &H59
            Case "z" : Return &H5A
            Case " ", "space" : Return &H20
            Case "*" : Return &H6A
            Case "-" : Return &H6D
            Case "+" : Return &H6B
            Case "[", "{" : Return &HDB
            Case "]", "}" : Return &HDD
            Case ";", ":" : Return &HBA
            Case "'", ControlChars.Quote : Return &HDE
            Case "~", "`" : Return &HC0
            Case "," : Return &HBC
            Case "." : Return &H6E
            Case "|" : Return &H6C
            Case "\" : Return &HDC
            Case "/" : Return &H6F
            Case "f1" : Return &H70
            Case "f2" : Return &H71
            Case "f3" : Return &H72
            Case "f4" : Return &H73
            Case "f5" : Return &H74
            Case "f6" : Return &H75
            Case "f7" : Return &H76
            Case "f8" : Return &H77
            Case "f9" : Return &H78
            Case "f10" : Return &H79
            Case "f11" : Return &H7A
            Case "f12" : Return &H7B
            Case "scrolllock" : Return &H91
            Case "numlock" : Return &H90
            Case "rightctrl" : Return &HA3
            Case "rightshift" : Return &HA1
            Case "sleep" : Return &H5F
            Case "backspace" : Return &H8
            Case "  ", "tab" : Return &H9
            Case "enter", "return" : Return &HD
            Case "shift" : Return &H10
            Case "ctrl", "control" : Return &H11
            Case "alt" : Return &H12
            Case "pause", "pausebreak" : Return &H13
            Case "caps", "capslock" : Return &H14
            Case "esc", "escape" : Return &H1B
            Case "pgup", "pageup" : Return &H21
            Case "pgdn", "pagedown" : Return &H22
            Case "end" : Return &H23
            Case "home" : Return &H24
            Case "left" : Return &H25
            Case "up" : Return &H26
            Case "right" : Return &H27
            Case "down" : Return &H28
            Case "select" : Return &H29
            Case "print" : Return &H2A
            Case "execute" : Return &H2B
            Case "printscreen", "prtscn", "prntscrn" : Return &H2C
            Case "insert", "ins" : Return &H2D
            Case "delete", "del" : Return &H2E
            Case "help" : Return &H2F
            Case "num0" : Return &H60
            Case "num1" : Return &H61
            Case "num2" : Return &H62
            Case "num3" : Return &H63
            Case "num4" : Return &H64
            Case "num5" : Return &H65
            Case "num6" : Return &H66
            Case "num7" : Return &H67
            Case "num8" : Return &H68
            Case "num9" : Return &H69
            Case "lwin", "leftwindows", "windows" : Return &H5C
            Case "rwin", "rightwindows", "rwindows" : Return &H5C
            Case "apps" : Return &H5D
            Case "mouse1", "lmouse", "lmb" : Return &H1
            Case "mouse2", "rmouse", "rmb" : Return &H2
            Case "mouse3", "mmouse", "mmb" : Return &H4
            Case "mouse4", "smouse1", "smb" : Return &H5
            Case "mouse5", "smouse2", "smb2" : Return &H6
        End Select
        Return &H20 'Space bar = error
    End Function

    Public Class PlayerMenuState
        Public name As String = ""
        Public muted As Boolean = False
        Public speaking As Boolean = False
        Public clientid As String = ""
        Public Sub New(ByVal name As String, ByVal muted As Boolean, ByVal speaking As Boolean, ByVal clientid As String)
            Me.name = name
            Me.muted = muted
            Me.speaking = speaking
            Me.clientid = clientid
        End Sub
    End Class
End Class
