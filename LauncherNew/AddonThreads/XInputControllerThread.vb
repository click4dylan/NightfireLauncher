Imports SharpDX.XInput
Imports System.Threading.Thread
Imports InputManager
Class XInputController
    Private Const A As Short = GamepadButtonFlags.A
    Private Const B As Short = GamepadButtonFlags.B
    Private Const X As Short = GamepadButtonFlags.X
    Private Const Y As Short = GamepadButtonFlags.Y
    Private Const RightShoulder As Short = GamepadButtonFlags.RightShoulder
    Private Const LeftShoulder As Short = GamepadButtonFlags.LeftShoulder
    Private Const DPadDown As Short = GamepadButtonFlags.DPadDown
    Private Const DPadUp As Short = GamepadButtonFlags.DPadUp
    Private Const DPadLeft As Short = GamepadButtonFlags.DPadLeft
    Private Const DPadRight As Short = GamepadButtonFlags.DPadRight
    Private Const LeftThumb As Short = GamepadButtonFlags.LeftThumb
    Private Const RightThumb As Short = GamepadButtonFlags.RightThumb
    Private Const Start As Short = GamepadButtonFlags.Start
    Private Const Back As Short = GamepadButtonFlags.Back
    Private controllers As New List(Of Controller)
    Private controller As Controller = Nothing
    Private state As State
    Public Declare Function SetCursorPos Lib "user32" (ByVal x As Integer, ByVal y As Integer) As Integer
    Public Declare Function GetCursorPos Lib "user32" (ByRef lpPoint As PointAPI) As Boolean
    Public Sub New()
        controllers.Add(New Controller(UserIndex.One))
        controllers.Add(New Controller(UserIndex.Two))
        controllers.Add(New Controller(UserIndex.Three))
        controllers.Add(New Controller(UserIndex.Four))
        For Each selectController In controllers
            If selectController.IsConnected Then
                controller = selectController
                Exit For
            End If
        Next
        On Error GoTo NoController
        Dim checkifpluggedin = controller.GetState

        Dim pollingthread As New Threading.Thread(AddressOf UpdateControllerState)
        pollingthread.Start()
        Dim buttonthr As New Threading.Thread(AddressOf ButtonThread)
        buttonthr.Start()
        Dim rightanalogthr As New Threading.Thread(AddressOf RightAnalogThread)
        rightanalogthr.Start()
        Dim leftanalogthr As New Threading.Thread(AddressOf LeftAnalogThread)
        leftanalogthr.Start()
        Dim nfactivewindowthr As New Threading.Thread(AddressOf ActiveWindow)
        nfactivewindowthr.Start()
NoController:
    End Sub
    Public Structure PointAPI
        Dim x As Integer
        Dim y As Integer
    End Structure
    Public Function GetCursorPositionX()
        Dim lpPoint As PointAPI
        Dim MousePOS As Integer = GetCursorPos(lpPoint)
        Return lpPoint.x
    End Function
    Public Function GetCursorPositionY()
        Dim lpPoint As PointAPI
        Dim MousePOS As Integer = GetCursorPos(lpPoint)
        Return lpPoint.y
    End Function
    Private Sub SetMousePos(ByVal x As Integer, ByVal y As Integer)
        SetCursorPos(x, y)
    End Sub
    Private NFIsActiveWindow As Boolean = False
    Private Sub ActiveWindow()
        While controller.IsConnected And Globals.NightfireProcess.HasExited = False
            If ExitGlobally Then Exit While
            NFIsActiveWindow = GetWindowText.Text.Equals("Nightfire")
            Sleep(200)
        End While
        NFIsActiveWindow = False
    End Sub
    Private Sub LeftAnalogThread()
        While controller.IsConnected And Globals.NightfireProcess.HasExited = False
            If ExitGlobally Then Exit While
            If NFIsActiveWindow Then LeftAnalogHandler(state)
            Sleep(1)
        End While
    End Sub
    Private Sub RightAnalogThread()
        While controller.IsConnected And Globals.NightfireProcess.HasExited = False
            If ExitGlobally Then Exit While
            If NFIsActiveWindow Then RightAnalogHandler(state)
            Sleep(1)
        End While
    End Sub
    Private Sub UpdateControllerState()
        While controller.IsConnected And Globals.NightfireProcess.HasExited = False
            If ExitGlobally Then Exit While
            If NFIsActiveWindow Then state = controller.GetState()
            Sleep(1)
        End While
    End Sub
    Private Sub ButtonThread()
        While controller.IsConnected And Globals.NightfireProcess.HasExited = False
            If ExitGlobally Then Exit While
            If NFIsActiveWindow Then ButtonHandler(state)
            Sleep(1)
        End While
    End Sub
    Private btn_previous As Short = 0
    Private btn_delta As Short = 0
    Private Sub ButtonHandler(ByVal state As State)
        btn_delta = state.Gamepad.Buttons Xor btn_previous
        btn_previous = state.Gamepad.Buttons
        With state
            Jump(.Gamepad)
            Reload(.Gamepad)
            Crouch(.Gamepad)
            Walk(.Gamepad)
            Fire(.Gamepad)
            AltFire(.Gamepad)
            VoiceActivate(.Gamepad)
            ToggleSensitivity(state.Gamepad)
            ShowScoreboard(state.Gamepad)
        End With
        SwitchWeapons(state.Gamepad)
        ToggleGadgetMode(state.Gamepad)
        ToggleGlasses(state.Gamepad)
        ChangeVisionMode(state.Gamepad)
        EscapeButton(state.Gamepad)
        SetControlDefaults(state.Gamepad)
        DropItem(state.Gamepad)
    End Sub
    Private lasttimeforward As Double = Sys_FloatTime()
    Private Sub MoveForward()
        Dim currenttime = Sys_FloatTime()
        Dim subtime = currenttime - lasttimeforward
        If left_y > 0 Then
            If subtime > left_y / 100 Then
                lasttimeforward = currenttime
                If left_y < 0.95 Then InputManager.Keyboard.KeyUp(Keys.W)
            Else
                InputManager.Keyboard.KeyDown(Keys.W)
            End If
        Else
            InputManager.Keyboard.KeyUp(Keys.W)
        End If
    End Sub
    Private lasttimebackward As Double = Sys_FloatTime()
    Private Sub MoveBackward()
        Dim currenttime = Sys_FloatTime()
        Dim subtime = currenttime - lasttimebackward
        If left_y < 0 Then
            If subtime > (-left_y / 100) Then
                lasttimebackward = currenttime
                If left_y > -0.95 Then InputManager.Keyboard.KeyUp(Keys.S)
            Else
                InputManager.Keyboard.KeyDown(Keys.S)
            End If
        Else
            InputManager.Keyboard.KeyUp(Keys.S)
        End If
    End Sub
    Private lasttimeleft As Double = Sys_FloatTime()
    Private Sub MoveLeft()
        Dim currenttime = Sys_FloatTime()
        Dim subtime = currenttime - lasttimeleft
        If left_x < 0 Then
            If subtime > (-left_x / 100) Then
                lasttimeleft = currenttime
                If left_x > -0.95 Then InputManager.Keyboard.KeyUp(Keys.A)
            Else
                InputManager.Keyboard.KeyDown(Keys.A)
            End If
        Else
            InputManager.Keyboard.KeyUp(Keys.A)
        End If
    End Sub
    Private lasttimeright As Double = Sys_FloatTime()
    Private Sub MoveRight()
        Dim currenttime = Sys_FloatTime()
        Dim subtime = currenttime - lasttimeright
        If left_x > 0 Then
            If subtime > left_x / 100 Then
                lasttimeright = currenttime
                If left_x < 0.95 Then InputManager.Keyboard.KeyUp(Keys.D)
            Else
                InputManager.Keyboard.KeyDown(Keys.D)
            End If
        Else
            InputManager.Keyboard.KeyUp(Keys.D)
        End If
    End Sub
    'Private lastx, lasty As Single
    Public idealSensitivity As Single = 0.3
    Private Sub MoveMouse(ByVal newx As Single, ByVal newy As Single)
        If Not GetSensitivity() = idealSensitivity Then
            SetSensitivity(idealSensitivity) 'Set optimal sensitivity, TODO, recreate nightfire's sensitivity scaling in vb so this isn't necessary
        End If

        If Not GetZoomSensitivityRatio() = 0 Then SetZoomSensitivityRatio(0) 'fixes really fast sensitivity in scopes (at least somewhat)

        Dim accelx As Single = 1
        Dim accely As Single = 1
        If newx > 0.95 Or newx < -0.95 Then
            accelx = 1.5 'if we are pushing the stick all the way, give it some extra speed
        End If
        If newy > 0.95 Or newy < -0.95 Then
            accely = 1.5 'if we are pushing the stick all the way, give it some extra speed
        End If

        newx *= accelx 'apply multiplier
        newy *= accely 'apply multiplier
        newx *= 100 'make tiny numbers big ones
        newy *= 100 'make tiny numbers big ones
        'Then make them whole numbers
        newx = Math.Round(newx)
        newy = Math.Round(newy)

        If newx > 1 And Not accelx = 2 Then newx -= 1 'Make short movements slower
        If newx > 1 And Not accelx = 2 Then newx -= 1 'Make short movements slower
        If newx < -1 And Not accelx = 2 Then newx -= -1 'Make short movements slower
        If newx < -1 And Not accelx = 2 Then newx -= -1 'Make short movements slower
        If newy > 1 And Not accely = 2 Then newy -= 1 'Make short movements slower
        If newy > 1 And Not accely = 2 Then newy -= 1 'Make short movements slower
        If newy < -1 And Not accely = 2 Then newy -= -1 'Make short movements slower
        If newy < -1 And Not accely = 2 Then newy -= -1 'Make short movements slower

        If idealSensitivity > 0.48 And newx > 0 Then newx = 1 'Make players able to use the mouse in the menus
        If idealSensitivity > 0.48 And newy > 0 Then newy = 1
        If idealSensitivity > 0.48 And newx < -1 Then newx = -1
        If idealSensitivity > 0.48 And newy < -1 Then newy = -1

        SetMousePos(GetCursorPositionX() + newx, GetCursorPositionY() + -newy) 'Set the mouse position
        Threading.Thread.Sleep(1)
    End Sub
    Private right_x, right_y As Single
    Private right_deadzoneX As Single = 0.22
    Private right_deadzoneY As Single = 0.22
    Private Sub RightAnalogHandler(ByVal state As State)
        Dim raw_right_x As Single = Math.Max(-1, Convert.ToSingle(state.Gamepad.RightThumbX / 32767))
        Dim raw_right_y As Single = Math.Max(-1, Convert.ToSingle(state.Gamepad.RightThumbY / 32767))

        If Math.Abs(raw_right_x) < right_deadzoneX Then
            right_x = 0
        Else
            right_x = (Math.Abs(raw_right_x) - right_deadzoneX) * (raw_right_x / Math.Abs(raw_right_x))
        End If

        If Math.Abs(raw_right_y) < right_deadzoneY Then
            right_y = 0
        Else
            right_y = (Math.Abs(raw_right_y) - right_deadzoneY) * (raw_right_y / Math.Abs(raw_right_y))
        End If

        If (right_deadzoneX > 0) Then right_x /= 1 - right_deadzoneX
        If (right_deadzoneY > 0) Then right_y /= 1 - right_deadzoneY


        If right_x = 0 And right_y = 0 Then
        Else
            MoveMouse(right_x, right_y)
        End If
    End Sub
    Private left_x, left_y As Single
    Private left_deadzoneX As Single = 0.2
    Private left_deadzoneY As Single = 0.2
    Private Sub LeftAnalogHandler(ByVal state As State)
        Dim raw_left_x As Single = Math.Max(-1, Convert.ToSingle(state.Gamepad.LeftThumbX / 32767))
        Dim raw_left_y As Single = Math.Max(-1, Convert.ToSingle(state.Gamepad.LeftThumbY / 32767))

        If Math.Abs(raw_left_x) < left_deadzoneX Then
            left_x = 0
        Else
            left_x = (Math.Abs(raw_left_x) - left_deadzoneX) * (raw_left_x / Math.Abs(raw_left_x))
        End If

        If Math.Abs(raw_left_y) < left_deadzoneY Then
            left_y = 0
        Else
            left_y = (Math.Abs(raw_left_y) - left_deadzoneY) * (raw_left_y / Math.Abs(raw_left_y))
        End If

        If (left_deadzoneX > 0) Then left_x /= 1 - left_deadzoneX
        If (left_deadzoneY > 0) Then left_y /= 1 - left_deadzoneY

        'If left_x = 0 And left_y = 0 Then
        'Else
        MoveForward()
        MoveBackward()
        MoveLeft()
        MoveRight()
        'End If

    End Sub
    Private Fire_prev As Boolean = False
    Private Sub Fire(ByVal gamepad As Gamepad)
        Dim pulled = TriggerPulled(gamepad.RightTrigger)
        If Not Fire_prev = pulled Then
            If pulled Then
                InputManager.Mouse.SendButton(Mouse.MouseButtons.LeftDown)
            Else
                InputManager.Mouse.SendButton(Mouse.MouseButtons.LeftUp)
            End If
        End If
        Fire_prev = pulled
    End Sub
    Private AltFire_prev As Boolean = False
    Private Sub AltFire(ByVal gamepad As Gamepad)
        Dim pulled = TriggerPulled(gamepad.LeftTrigger)
        If Not AltFire_prev = pulled Then
            If pulled Then
                InputManager.Mouse.SendButton(Mouse.MouseButtons.RightDown)
            Else
                InputManager.Mouse.SendButton(Mouse.MouseButtons.RightUp)
            End If
        End If
        AltFire_prev = pulled
    End Sub
    Private Sub Jump(ByVal gamepad As Gamepad)
        Dim updated = btn_delta And Y
        If updated = 0 Then Exit Sub

        If ButtonPressed(gamepad.Buttons, Y) Then
            Keyboard.KeyDown(Keys.Space)
        Else
            Keyboard.KeyUp(Keys.Space)
        End If
    End Sub
    Private Sub Reload(ByVal gamepad As Gamepad)
        Dim updated = btn_delta And X
        If updated = 0 Then Exit Sub

        If ButtonPressed(gamepad.Buttons, X) Then
            Keyboard.KeyDown(Keys.R)
            Keyboard.KeyDown(Keys.E)
        Else
            Keyboard.KeyUp(Keys.R)
            Keyboard.KeyUp(Keys.E)
        End If
    End Sub
    Private Sub Crouch(ByVal gamepad As Gamepad)
        Dim updated = btn_delta And B
        If updated = 0 Then Exit Sub

        If ButtonPressed(gamepad.Buttons, B) Then
            Keyboard.KeyDown(Keys.ControlKey)
        Else
            Keyboard.KeyUp(Keys.ControlKey)
        End If
    End Sub
    Private Sub ShowScoreboard(ByVal gamepad As Gamepad)
        Dim updated = btn_delta And LeftThumb
        If updated = 0 Then Exit Sub

        If ButtonPressed(gamepad.Buttons, LeftThumb) Then
            Keyboard.KeyDown(Keys.Tab)
        Else
            Keyboard.KeyUp(Keys.Tab)
        End If
    End Sub
    Private Sub Walk(ByVal gamepad As Gamepad)
        Dim updated = btn_delta And A
        If updated = 0 Then Exit Sub

        If ButtonPressed(gamepad.Buttons, A) Then
            Keyboard.KeyDown(Keys.ShiftKey)
        Else
            Keyboard.KeyUp(Keys.ShiftKey)
        End If
    End Sub
    Private Sub VoiceActivate(ByVal gamepad As Gamepad)
        Dim updated = btn_delta And RightShoulder
        If updated = 0 Then Exit Sub

        If ButtonPressed(gamepad.Buttons, RightShoulder) Then
            Keyboard.KeyDown(VoiceThread.voice_activatekey)
        Else
            Keyboard.KeyUp(VoiceThread.voice_activatekey)
        End If
    End Sub
    Private Sub ToggleSensitivity(ByVal gamepad As Gamepad)
        Dim updated = btn_delta And LeftShoulder
        If updated = 0 Then Exit Sub
        If ButtonPressed(gamepad.Buttons, LeftShoulder) Then
            If idealSensitivity < 0.38 Then
                idealSensitivity = 0.4
            ElseIf idealSensitivity < 0.48 Then
                idealSensitivity = 0.5
            Else
                idealSensitivity = 0.3
            End If
        End If
    End Sub
    Private lastglassestoggle As Double = Sys_FloatTime()
    Private Sub ToggleGlasses(ByVal gamepad As Gamepad)
        If ButtonPressed(gamepad.Buttons, DPadUp) Then
            Dim time = Sys_FloatTime()
            If time - lastglassestoggle > 0.5 Then
                lastglassestoggle = time
                Keyboard.KeyDown(Keys.F)
                Keyboard.KeyUp(Keys.F)
            End If
        End If
    End Sub
    Private lastvisionmode As Double = Sys_FloatTime()
    Private Sub ChangeVisionMode(ByVal gamepad As Gamepad)
        If ButtonPressed(gamepad.Buttons, DPadDown) Then
            Dim time = Sys_FloatTime()
            If time - lastvisionmode > 0.5 Then
                lastvisionmode = time
                Keyboard.KeyDown(Keys.V)
                Keyboard.KeyUp(Keys.V)
            End If
        End If
    End Sub
    Private lastgadgettoggle As Double = Sys_FloatTime()
    Private Sub ToggleGadgetMode(ByVal gamepad As Gamepad)
        If ButtonPressed(gamepad.Buttons, Back) Then
            Dim time = Sys_FloatTime()
            If time - lastgadgettoggle > 0.5 Then
                lastgadgettoggle = time
                Keyboard.KeyDown(Keys.G)
                Keyboard.KeyUp(Keys.G)
            End If
        End If
    End Sub
    Private lastescape As Double = Sys_FloatTime()
    Private Sub EscapeButton(ByVal gamepad As Gamepad)
        If ButtonPressed(gamepad.Buttons, Start) Then
            Dim time = Sys_FloatTime()
            If time - lastescape > 0.2 Then
                lastescape = time
                Keyboard.KeyDown(Keys.Escape)
                Keyboard.KeyUp(Keys.Escape)
            End If
        End If
    End Sub
    Private lastdrop As Double = Sys_FloatTime()
    Private Sub DropItem(ByVal gamepad As Gamepad)
        If ButtonPressed(gamepad.Buttons, RightThumb) Then
            Dim time = Sys_FloatTime()
            If time - lastdrop > 0.2 Then
                lastdrop = time
                Keyboard.KeyDown(Keys.T)
                Keyboard.KeyUp(Keys.T)
            End If
        End If
    End Sub
    Private lastdefault As Double = Sys_FloatTime()
    Private Sub SetControlDefaults(ByVal gamepad As Gamepad)
        Exit Sub
        'If ButtonPressed(gamepad.Buttons, LeftThumb) And ButtonPressed(gamepad.Buttons, RightThumb) Then
        '    Dim time = Sys_FloatTime()
        '    If time - lastdefault > 1 Then
        '        lastdefault = time
        '        MapDownloaderThread.concmd("bind f activateglasses")
        '        MapDownloaderThread.concmd("bind g gadgetmode")
        '        MapDownloaderThread.concmd("bind v glassesmode")
        '        MapDownloaderThread.concmd("bind mouse1 +attack")
        '        MapDownloaderThread.concmd("bind mouse2 +attack2")
        '        MapDownloaderThread.concmd("bind ctrl +duck")
        '        MapDownloaderThread.concmd("bind shift +speed")
        '        MapDownloaderThread.concmd("bind tab +showscores")
        '        MapDownloaderThread.concmd("bind space +jump")
        '        MapDownloaderThread.concmd("bind t " & ControlChars.Quote & "impulse 205" & ControlChars.Quote)
        '        MapDownloaderThread.concmd("bind e +use")
        '        MapDownloaderThread.concmd("bind r +reload")
        '        MapDownloaderThread.concmd("bind w +forward")
        '        MapDownloaderThread.concmd("bind a +moveleft")
        '        MapDownloaderThread.concmd("bind d +moveright")
        '        MapDownloaderThread.concmd("bind s +back")
        '        MapDownloaderThread.concmd("bind [ PrevWeapon")
        '        MapDownloaderThread.concmd("bind ] NextWeapon")
        '    End If
        'End If
    End Sub
    Private lastlswitch As Double = Sys_FloatTime()
    Private lastrswitch As Double = Sys_FloatTime()
    Private Sub SwitchWeapons(ByVal gamepad As Gamepad)
        Dim time = Sys_FloatTime()
        If ButtonPressed(gamepad.Buttons, DPadLeft) Then
            If time - lastlswitch > 0.2 Then
                lastlswitch = time
                Keyboard.KeyDown(Keys.OemCloseBrackets)
                Keyboard.KeyUp(Keys.OemCloseBrackets)
            End If
        End If
        If ButtonPressed(gamepad.Buttons, DPadRight) Then
            If time - lastrswitch > 0.2 Then
                lastrswitch = time
                Keyboard.KeyDown(Keys.OemOpenBrackets)
                Keyboard.KeyUp(Keys.OemOpenBrackets)
            End If
        End If
    End Sub
    Function ButtonPressed(ByVal buttons, ByVal button)
        Dim button_pressed As Boolean = (buttons And button)
        Return button_pressed
    End Function
    Function TriggerPulled(ByVal triggeramt As Byte)
        Dim result As Boolean = triggeramt > 100
        Return result
    End Function
    Private Function GetSensitivity() As Single
        Dim offset = mem.ReadInt32(&H410B0CCC)
        Return mem.ReadSingle(offset + &H14)
    End Function 'Gets the mouse sensitivity
    Private Sub SetSensitivity(ByVal sensitivity As Single)
        Dim offset = mem.ReadInt32(&H410B0CCC)
        mem.Write(offset + &H14, CSng(sensitivity))
    End Sub 'Sets the mouse sensitivity
    Private Sub SetZoomSensitivityRatio(ByVal sensitivity As Single)
        Dim offset = mem.ReadInt32(&H410B6104)
        mem.Write(offset + &H14, CSng(sensitivity))
    End Sub
    Private Function GetZoomSensitivityRatio() As Single
        Dim offset = mem.ReadInt32(&H410B6104)
        Return mem.ReadSingle(offset + &H14)
    End Function
End Class
