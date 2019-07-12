Module Bond2Mem
    Public mem As ProcessMemory
    Public cl_maxclients As Int32 = &H432D4BA8 'not a cvar
    Public DEVELOPER As Int32 = &H448BE6C8
    'public FIELD_OF_VIEW As Int32 = &H44B82F6E
    Public mlook As Int32 = &H410B0730
    Public r_zmin As Int32 = &H448F6AC4 + &H14
    Public r_viewzmin As Int32 = &H448F6B78 + &H14
    Public r_fullbright As Int32 = &H448F6D28 + &H14
    Public r_wireframe As Int32 = &H448F6D4C + &H14
    Public r_drawcliententities As Int32 = &H448F6DDC + &H14
    Public r_drawworldmodel As Int32 = &H448F6D94 + &H14
    'public r_fogdev As Int32 = &H448F6914 + &H14
    Public r_fieldofview As Int32 = &H44B82F5A + &H14
    'public chase_active As Int32 = &H4310F770 + &H14
    Public fps_max As Int32 = &H448BE624 + &H14
    'public cam_tothirdperson As Int32 = &H410B0498
    Public cl_pitchup As Int32 = Nothing
    Public cl_pitchdown As Int32 = Nothing
    Public Function ClampInteger(ByVal value, min, max) As Integer
        value = Math.Min(value, max)
        value = Math.Max(min, value)
        Return value
    End Function
    Public Function ClampFloat(ByVal value, min, max) As Single
        value = Math.Min(value, max)
        value = Math.Max(min, value)
        Return value
    End Function
    Public Function ClampDouble(ByVal value, min, max) As Double
        value = Math.Min(value, max)
        value = Math.Max(min, value)
        Return value
    End Function
    Public Function Inject()
        Try
            mem = New ProcessMemory(globals.NightfireProcess, ProcessMemory.ProcessAccess.All)
            mem.Open()
            'Dim engineBase As IntPtr = 0
            'Dim clientBase As IntPtr = 0
            'For Each a As System.Diagnostics.ProcessModule In main.nf.Modules
            'If a.ModuleName.ToLower = "engine.dll" Then
            'engineBase = a.BaseAddress
            funcInit()
            'End If
            'Next
            'If engineBase = 0 Then Return False
            ' For Each a As System.Diagnostics.ProcessModule In main.nf.Modules
            ' If a.ModuleName.ToLower = "client.dll" Then
            'clientBase = a.BaseAddress
            'funcInit()
            'End If
            ' Next
            'If clientBase = 0 Then Return False
            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function
    Private Sub funcInit()
Start:
        If Globals.NightfireProcess.HasExited Or ExitGlobally Then Exit Sub
        'Setup Download Interception
        'Dim infl1 As Byte() = {&H6A, &H1, &HFF, &H15, &HC, &H32, &HE, &H43, &HEB, &H54, &H90} '430E0CA3 Initialize lplock / Reset lplock 1
        'Dim infl2 As Byte() = {&H80, &H3D, &HE9, &H2E, &HB8, &H44, &H1, &H74, &H99, &HE9, &HE1, &H1A, &HF4, &HFF, &H90, &HB9, &HFC, &HF6, &H10, &H43, &HE9, &HC6, &H6E, &HF4, &HFF} '430E0D01 Initialize lplock / Reset lplock 2
        'Dim infl3 As Byte() = {&HC6, &H5, &HE9, &H2E, &HB8, &H44, &H1, &HE9, &H46, &HFF, &HFF, &HFF} '430E0D51 Initialize lplock / Reset lplock 3
        'Dim lkcl As Byte() = {&HE8, &HA6, &HAF, &HA, &H0} '43035DA6 Setup call
        Dim loadscreenfunc As Byte() = {&H80, &H3D, &H4, &HD8, &HC9, &H44, &H0, &H74, &H1C, &H80, &H3D, &H5, &HD8, &HC9, &H44, &H1, &H74, &H2D, &HC6, &H5, &H5, &HD8, &HC9, &H44, &H1, &H6A, &H1, &HE8, &H5C, &H92, &H3B, &HFE, &H83, &HC4, &H4, &HEB, &H1A, &H80, &H3D, &H5, &HD8, &HC9, &H44, &H1, &H75, &H11, &H6A, &H0, &HE8, &H7, &H94, &H3B, &HFE, &HC6, &H5, &H5, &HD8, &HC9, &H44, &H0, &H83, &HC4, &H4, &HE9, &H99, &H86, &H38, &H0, &H90}
        Dim interceptjmp As Byte() = {&HE9, &HED, &H45, &HC4, &H1}
        mem.Write(&H44C9D814, loadscreenfunc)
        mem.Write(&H43059222, interceptjmp)
        'mem.Write(&H430E0CA3, infl1)
        'mem.Write(&H430E0D01, infl2)
        'mem.Write(&H430E0D51, infl3)
        'mem.Write(&H43035DA6, lkcl)
        'Setup Console Command try it now xDDwai now try xddd
        'Dim bypass1 As Byte() = {&H80, &H3D, &H90, &HFD, &H1, &H0, &H0, &H7F, &H7, &H8B, &HD, &H84, &HB, &H48, &H43, &HC3, &H6A, &H1, &H68, &H90, &HFD, &H1, &H0, &HE8, &H2A, &H2B, &HF6, &HFF, &H83, &HC4, &H8, &HC6, &H5, &H90, &HFD, &H1, &H0, &H0, &H8B, &HD, &H84, &HB, &H48, &H43, &HC3} '430E262B - Command code
        Dim bypass1 As Byte() = {&H80, &H3D, &H80, &H2F, &HB8, &H44, &H0, &H7F, &H7, &H8B, &HD, &H84, &HB, &H48, &H43, &HC3, &H6A, &H1, &H68, &H80, &H2F, &HB8, &H44, &HE8, &H2A, &H2B, &HF6, &HFF, &H83, &HC4, &H8, &HC6, &H5, &H80, &H2F, &HB8, &H44, &H0, &H8B, &HD, &H84, &HB, &H48, &H43, &HC3}
        Dim bypass2 As Byte() = {&HE8, &H6E, &H54, &HA, &H0, &HC3} '4303DB47 - Call
        mem.Write(&H430E2FBA, bypass1)
        mem.Write(&H4303DB47, bypass2) 'Address of bypass1
        If Not mem.Read(&H4303DB47, 1)(0) = &HE8 Then
            GoTo Start 'This is required because not all modules are available from the start
        End If
    End Sub
    Public Sub unreliable_concmd(ByVal cmd As String)
        cmd = cmd & "  "
        Dim entry As Byte() = System.Text.Encoding.ASCII.GetBytes(cmd)
        entry(entry.Length - 1) = &H0
        entry(entry.Length - 2) = &HA
        mem.Write(&H44B82F80, entry)
    End Sub
    Public Sub concmd(ByVal cmd As String)
        cmd = cmd & "  "
        Dim entry As Byte() = System.Text.Encoding.ASCII.GetBytes(cmd)
        entry(entry.Length - 1) = &H0
        entry(entry.Length - 2) = &HA
        Do
            If Not mem.Read(&H44B82F80, 1)(0) = &H0 Then
                Threading.Thread.Sleep(1)
            Else
                Exit Do
            End If
        Loop
        mem.Write(&H44B82F80, entry)
        Do
            If Not mem.Read(&H44B82F80, 1)(0) = &H0 Then
                Threading.Thread.Sleep(1)
            Else
                Exit Do
            End If
        Loop
    End Sub
    Public Function SinglePlayer() As Boolean
        Return mem.ReadInt32(cl_maxclients) = 1
    End Function
    Public Function GetScreenWidth() As Integer
        Return mem.ReadInt32(&H43493D38)
    End Function
    Public Function GetScreenHeight() As Integer
        Return mem.ReadInt32(&H43493D3C)
    End Function
    Public Function GetAspectRatio() As Single
        Return Math.Round(GetScreenWidth() / GetScreenHeight(), 2)
    End Function
    Public Sub SetWeaponFOV(ByVal x As Single)
        Dim scr_ofsx = (mem.ReadInt32(&H410B5840)) + &H14
        mem.Write(scr_ofsx, CSng(x))
    End Sub
    Public Sub SetFOVSmooth(ByVal amount As Integer)
        Dim ms As Integer = 100 'how fast to zoom in and out in milliseconds

        'This can be used to see how long it has been since it started
        Dim start = Environment.TickCount

        Dim userfovB As Integer = mem.ReadInt32(&H44B82F6E)
        Dim diff = amount - userfovB
        Dim diff2 = Math.Abs(diff)
        Dim last = userfovB
        Dim update = 0

        'Calculate how many ms to increment by 1
        Dim stepc = ms / diff

        Dim a = 0

        While Not diff = 0

            'Calculate difference from start and now
            a = Environment.TickCount - start
            If a > ms Then
                a = ms
            End If

            a = a / Math.Abs(stepc)

            If stepc < 0 Then
                a = -a
            End If

            update = userfovB + a
            diff = diff2 - Math.Abs(a)

            If Not update = last Then
                mem.Write(&H44B82F6E, {CByte(update)})
                last = update
            End If
            Threading.Thread.Sleep(1)
        End While
    End Sub 'Sets field of view
    Public Sub SetFOV(ByVal fov As Integer)
        mem.Write(&H44B82F6E, {CByte(fov)})
    End Sub
    Public Function GetFOV() As Integer
        Return mem.ReadInt32(r_fieldofview)
    End Function
    Public Function Game_GetCurrentServer() As String
        Return mem.ReadString(&H448F4AC8, 21, System.Text.ASCIIEncoding.ASCII)
    End Function
    Public Function Game_IsConnected() As Boolean
        Return Not (mem.Read(&H410A9AB8, 1)(0) = 0)
    End Function
    Public Function Game_GetPlayerInfo() As Dictionary(Of String, String)
        Dim keyvals As New Dictionary(Of String, String)
        On Error GoTo Failed
        Dim arr = mem.Read(&H4311D010, 512)
        Dim len = 0
        For Each a In arr
            If a = &H0 Then Exit For
            len += 1
        Next
        Dim info = System.Text.Encoding.ASCII.GetString(arr, 0, len).Replace(vbNullChar, "").Remove(0, 1) ' mem.ReadString(&H4311D010, 512, Encoding.ASCII)
        Dim pieces = info.Split("\")

        Dim key As String = ""
        For Each piece In pieces
            If key = "" Then
                key = piece
            Else
                keyvals.Add(key, piece)
                key = ""
            End If
        Next
        If Not keyvals.ContainsKey("cl_name") Then GoTo Failed
        Return keyvals
Failed:
        keyvals.Clear()
        keyvals.Add("cl_name", "Error Retrieving Name")
        Return keyvals
    End Function
    'Private Sub SetCrosshair(ByVal toggle As String)
    '    '41326fe0 address to set crosshair
    '    Select Case toggle
    '        Case "on"
    '            mem.Write(&H41326FE0, {&H1})
    '        Case "off"
    '            mem.Write(&H41326FE0, {&H0})
    '    End Select
    'End Sub 'Sets crosshair
End Module
