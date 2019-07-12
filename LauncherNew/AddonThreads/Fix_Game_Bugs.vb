Class Fix_Game_Bugs
    Private thread As New Threading.Thread(AddressOf DoWork)
    Private exitthread As New Threading.Thread(AddressOf KillThread)
    Public Sub New()
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
    Private Sub DoWork()
        While Not ExitGlobally 'main loop
            If Globals.NightfireProcess.HasExited Then Exit While
            Try
                Fix_M6_Escape01_Elevator()
                FixMLook()
            Catch ex As Exception
            End Try
            Threading.Thread.Sleep(5)
        End While
    End Sub
    Private CurrentlyCappedFPS As Boolean = False
    Private StoredFPSMax As Single = 100
    Private gamepaused As Boolean = False
    Private Sub Fix_M6_Escape01_Elevator()
        If Not CurrentlyCappedFPS Then
            StoredFPSMax = mem.ReadSingle(fps_max)
        End If
        Dim activewindow As Boolean = GetWindowText.Text.Equals("Nightfire")
        Dim Console_And_Chat_Is_Closed = (mem.Read(&H4310B7A8, 1)(0) = 0)
        Dim map As String = System.Text.Encoding.ASCII.GetString(mem.Read(&H432D4B80, 64))
        Dim mapSp = map.Split("/")
        map = mapSp(mapSp.Length - 1).Split(".")(0)
        gamepaused = False
        'Check if game is paused or menu is open; This only works for single player!
        If SinglePlayer() And mem.ReadDouble(&H448BE138) - mem.ReadSingle(&H4311D110) > 0.15 Then 'realtime - lastpackettime
            gamepaused = True
        End If
        If map.ToLower = "m6_escape01" And Console_And_Chat_Is_Closed And activewindow And gamepaused = False Then
            If Not CurrentlyCappedFPS Then
                    CurrentlyCappedFPS = True
                    mem.Write(fps_max, CSng(64))
            End If
        Else
            If CurrentlyCappedFPS Then
                CurrentlyCappedFPS = False
                mem.Write(fps_max, CSng(StoredFPSMax))
            End If
        End If
    End Sub
    Private Sub FixMLook()
        On Error Resume Next
        If Not mem.Read(mlook, 1)(0) = 7 Then
            mem.Write(mlook, 7) 'Prevents players from not being able to look around with their mouse (config.cfg length problem)
        End If
    End Sub
End Class
