Module Platform
    Public Sub DelayRuntime(ByVal bythismanysecs As Double)
        Dim time = Sys_FloatTime()
        While Sys_FloatTime() - time < bythismanysecs
            Application.DoEvents()
            Threading.Thread.Sleep(1)
        End While
    End Sub
    Private Delegate Sub CloseFormDelegate(ByVal form As Form)
    Public Sub CloseForm(ByVal targetform As Form)
        Dim frmCollection As System.Windows.Forms.FormCollection = System.Windows.Forms.Application.OpenForms
        Dim copy As New List(Of Form)
        Dim count = frmCollection.Count
        For Each fm As System.Windows.Forms.Form In frmCollection
            If fm.Name = targetform.Name Then
                copy.Add(fm)
            End If
            If Not frmCollection.Count = count Then Exit For
        Next

        For Each a As Form In copy
            Dim tfm = a
            If a.InvokeRequired Then
                a.Invoke(Sub()
                             tfm.Close()
                         End Sub)
            Else
                tfm.Close()
            End If
        Next

#If 0 Then
        If form.InvokeRequired Then
            On Error GoTo Failed
            form.Invoke(New CloseFormDelegate(AddressOf CloseForm))
            Exit Sub
        Else
            form.Close()
            Exit Sub
        End If
Failed:
        'Me.Close()
#End If
    End Sub
    Private Delegate Sub LabelDelegate(ByVal target As Label, ByVal text As String)
    Public Sub SetLabelText(ByVal target As Label, ByVal text As String)
        If target.InvokeRequired Then
            target.BeginInvoke(Sub()
                                   target.Text = text
                                   target.Invalidate()
                                   target.Update()
                                   target.Refresh()
                               End Sub)
        Else
            target.Text = text
            target.Invalidate()
            target.Update()
            target.Refresh()
        End If
    End Sub
    Private Delegate Sub HideDelegate(ByVal targetform As Form)
    Public Function HideForm(ByVal targetform As Form) As Boolean
        Dim frmCollection As System.Windows.Forms.FormCollection = System.Windows.Forms.Application.OpenForms
        For Each fm As System.Windows.Forms.Form In frmCollection
            If fm.Name = targetform.Name Then
                Dim tfm = fm
                If fm.InvokeRequired Then
                    fm.Invoke(Sub()
                                  tfm.Hide()
                              End Sub)
                Else
                    tfm.Hide()
                End If
                Return True
            End If
        Next
        Return False
#If 0 Then
        If targetform.InvokeRequired Then
            targetform.Invoke(New HideDelegate(AddressOf HideForm))
        Else
            FormHide(targetform)
        End If
#End If
    End Function
    Private Sub FormHide(ByVal targetform As Form)
        targetform.Opacity = 0
        targetform.Hide()
    End Sub
    Private Delegate Sub ShowDelegate(ByVal targetform As Form)
    Public Sub ShowForm(ByVal targetform As Form)
        Dim frmCollection As System.Windows.Forms.FormCollection = System.Windows.Forms.Application.OpenForms
        For Each fm As System.Windows.Forms.Form In frmCollection
            If fm.Name = targetform.Name Then
                Dim tfm = fm
                If fm.InvokeRequired Then
                    fm.Invoke(Sub()
                                  tfm.Show()
                              End Sub)
                    Return
                Else
                    tfm.Hide()
                    Return
                End If
            End If
        Next
        targetform.Show()
#If 0 Then
        If targetform.InvokeRequired Then
            targetform.Invoke(New ShowDelegate(AddressOf ShowForm))
        Else
            FormShow(targetform)
        End If
#End If
    End Sub
    Private Sub FormShow(ByVal targetform As Form)
        targetform.Opacity = 1
        targetform.Show()
    End Sub
    Public Sub FormActivate(ByVal targetform As Form)
        Dim frmCollection As System.Windows.Forms.FormCollection = System.Windows.Forms.Application.OpenForms
        For Each fm As System.Windows.Forms.Form In frmCollection
            If fm.Name = targetform.Name Then
                Dim tfm = fm
                If fm.InvokeRequired Then
                    fm.Invoke(Sub()
                                  tfm.Activate()
                              End Sub)
                Else
                    tfm.Activate()
                End If
            End If
        Next
#If 0 Then
        If targetform.InvokeRequired Then
            targetform.Invoke(Sub()
                                  targetform.Activate()
                              End Sub)
        Else
            targetform.Activate()
        End If
#End If
    End Sub
    Public Function FormIsOpen(ByVal targetform As Form) As Boolean
        For i As Integer = 0 To Application.OpenForms.Count - 1
            Dim frm As Form = Application.OpenForms.Item(i)
            If frm.Name = targetform.Name Then Return True
        Next
        Return False
    End Function
    Public timer_frequency As Double = Stopwatch.Frequency
    Public Function Sys_FloatTime() As Double
        Dim tick As Double = Stopwatch.GetTimestamp
        Dim ms As Double = (tick / timer_frequency)
        Return ms
        'Dim ms = Environment.TickCount
        'Return ms / 1000 'return seconds
    End Function 'Gets the current time in seconds
    Public Function GetCommandLineArgs() As String
        Dim fullargs() As String = Environment.GetCommandLineArgs()
        Dim allargs As String = ""
        For Each a In fullargs
            If Not a = fullargs(0) Then
                allargs &= a & " "
            End If
        Next
        Return allargs
    End Function 'Returns all args as a single string, excluding path to the exe
    Public Function GetCommandLineArg(ByVal name As String) As Object
        Dim fullargs() As String = Environment.GetCommandLineArgs()
        Dim allargs As New List(Of String)
        For Each a In fullargs
            If Not a = fullargs(0) Then 'Don't return the path to the exe
                allargs.Add(a)
            End If
        Next
        If allargs.Contains(name) Then
            Dim num = allargs.IndexOf(name)
            Dim count = allargs.Count
            If count > num + 1 Then
                Return allargs(num + 1)
            Else
                Return True
            End If
        End If
        Return False

        'old useless code below
        'Dim argl As New Dictionary(Of String, String)
        'Dim argsingle As New List(Of String)
        'Dim arg = System.Environment.GetCommandLineArgs
        'Dim hasargs = False
        'If arg.Length > 1 Then
        '    hasargs = True
        'End If
        'Dim key As String = ""
        'Dim value As String = ""
        'For i As Integer = 0 To arg.Length - 1
        '    Dim temp = arg(i)
        '    If temp.StartsWith("-") Then
        '        key = temp
        '        argsingle.Add(key.ToLower)
        '    ElseIf key.Length > 0 Then
        '        value = temp
        '        If argl.ContainsKey(key) Then
        '            argl(key) = value
        '        Else
        '            argl.Add(key, value)
        '        End If
        '        key = ""
        '    End If
        'Next

        'For Each arg As String In Environment.GetCommandLineArgs()
        '    If arg.Contains("-tobrowser") Then
        '        SplashScreen.browseservers_btn.Enabled = False
        '        SplashScreen.browseservers_btn.BackgroundImage = My.Resources.Resources.serverlist_disabled
        '        ShowForm(ServerBrowser)
        '    End If
        '    If arg.Contains("+connect") Then
        '        If ExtraCommandLineArgs.Contains("-toconsole") Then ExtraCommandLineArgs = ExtraCommandLineArgs.Replace("-toconsole ", "")
        '    ElseIf arg.Contains("-fullscreen") Then
        '        If ExtraCommandLineArgs.Contains("-windowed") Then ExtraCommandLineArgs = Args.Replace("-windowed", "")
        '    End If
        '    If Not Args = " " Then
        '        If arg.ToLower = "-dedicated" Then
        '            Dedicated = True
        '            If Args.Length > 0 Then
        '                Args += " " & arg
        '            Else
        '                Args += arg
        '            End If
        '        Else
        '            If Args.Length > 0 Then
        '                Args += " " & arg
        '            Else
        '                Args += arg
        '            End If
        '        End If
        '    Else
        '        CPath = arg
        '        Args = ""
        '    End If
        'Next arg
    End Function 'Returns if an arg exists and its value if it has one
    Private DeletingWasSuccessful As Boolean = False
    Public Sub TryWritingBond2()
        Dim firsttime = Environment.TickCount
        Dim numfails = 1
        While DeletingWasSuccessful = False
            If numfails = 1 Or Environment.TickCount - firsttime >= 1000 Then
                firsttime = Environment.TickCount
                TryDeletingBond2()
                Try
                    If IO.File.Exists(IO.Directory.GetCurrentDirectory & "\" & BondEXE) Then
                        DeletingWasSuccessful = False
                        SetLabelText(SplashScreenForm.SplashScreenInfoLabel, "Failed to delete Bond2.exe.. Waiting " & ((1000) / 1000).ToString & " second(s) before trying again. Do not close this window or your game install will be corrupt! Number of attempts: " & numfails & ControlChars.NewLine)
                        'MsgBox("ERROR: Unable to delete Bond.exe. Waiting " & ((10000 * numfails) / 1000).ToString & " seconds before trying again. Please do not try to start the game during this time or you will risk data corruption!", MsgBoxStyle.Critical)
                    Else
                        DeletingWasSuccessful = True
                    End If
                Catch error999 As Exception
                    Console.WriteLine(error999.Message)
                End Try
                If DeletingWasSuccessful = False Then numfails += 1
            End If
            If numfails = 120 And DeletingWasSuccessful = False Then
                SetLabelText(SplashScreenForm.SplashScreenInfoLabel, "Failed " & numfails & " times to delete Bond2.exe")
                MsgBox("Sorry, Bond2.exe failed to update. Please try to delete it manually from your folder (" & IO.Directory.GetCurrentDirectory & ") and then run Updater.exe to update it yourself. Now exiting.", MsgBoxStyle.Critical)
                Environment.Exit(0)
            End If
            Threading.Thread.Sleep(1)
        End While
        Threading.Thread.Sleep(500)
        For tries As Integer = 1 To 5
            Try
                Dim outbond As New IO.FileStream(IO.Directory.GetCurrentDirectory & "\" & BondEXE, System.IO.FileMode.Create, System.IO.FileAccess.Write)
                outbond.Write(My.Resources.Bond2, 0, My.Resources.Bond2.Length)
                outbond.Flush()
                outbond.Close()
                'IO.File.WriteAllBytes(IO.Directory.GetCurrentDirectory & "\" & BondEXE, My.Resources.Bond_qpc) 'unzip Bond2.exe which is the actual game exe
                SetLabelText(SplashScreenForm.SplashScreenInfoLabel, "Successfully extracted Bond2.exe")
                Exit For
            Catch ex2 As Exception
                SetLabelText(SplashScreenForm.SplashScreenInfoLabel, "Failed to extract Bond2.exe, trying again! Reason: " & ex2.Message)

            End Try
        Next
    End Sub
    Public Sub TryDeletingBond2()
        Try
            Dim Bond2Exe = New System.IO.FileInfo(IO.Directory.GetCurrentDirectory & "\" & BondEXE)
            If Bond2Exe.Exists Then
                Try
                    If (Bond2Exe.Attributes And System.IO.FileAttributes.ReadOnly) > 0 Then
                        Bond2Exe.Attributes = Bond2Exe.Attributes Xor System.IO.FileAttributes.ReadOnly
                    End If
                Catch ex0 As Exception
                    Console.WriteLine(ex0.Message & ControlChars.NewLine)
                End Try
                DeletingWasSuccessful = True 'temporary
                Try
                    'IO.File.Delete(IO.Directory.GetCurrentDirectory & "\Bond.exe") 'This shit is flawed, access is denied errors all over the place
                    'My.Computer.FileSystem.DeleteFile("bond.exe").
                    Dim rand As New Random
                    Dim randnum As Integer = rand.NextDouble * 100000
                    Dim randname = IO.Directory.GetCurrentDirectory & "\Bond2_old_" & randnum.ToString & ".exe"
                    While IO.File.Exists(randname)
                        randnum = rand.NextDouble * 100000
                        randname = IO.Directory.GetCurrentDirectory & "\Bond2_old_" & randnum.ToString & ".exe"
                    End While
                    IO.File.Move(IO.Directory.GetCurrentDirectory & "\" & BondEXE, randname)
                    IO.File.Delete(randname)
                Catch ex As Exception
                    Console.WriteLine(ex.Message & ControlChars.NewLine)
                End Try
                DeletingWasSuccessful = False
            Else
                DeletingWasSuccessful = True
            End If
        Catch ex2 As Exception
            SetLabelText(SplashScreenForm.SplashScreenInfoLabel, "Failed to check if Bond2.exe exists, reason: " & ex2.Message)
        End Try
    End Sub
End Module
