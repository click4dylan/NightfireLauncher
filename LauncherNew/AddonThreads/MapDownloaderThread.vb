Imports Ionic
Imports System.Diagnostics
Imports System.Windows.Forms
Imports System.Net
Class MapDownloaderThread
    Private thread As New Threading.Thread(AddressOf MapDownloader)
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
    Private reconnect As Boolean = True
    Private haserrored As Boolean = False
    Private faileddownloading As Boolean = False
    Private errormsg As String
    Private DownloadComplete As Boolean = False
    Private alreadyloading As Boolean = False 'already showing load screen
    'private defaultmaps As New List(Of String)
    'MEM ADDRESSES
    Private ADR_ShowLoadScreenFunc As Int32 = &H44C9D804
    Private ADR_CurrentlyShowingLoadScreen As Int32 = &H44C9D805
    Private ADR_MapLoadFailure As Int32 = &H44B5C823
    Private ADR_LastMapThatFailed As Int32 = &H44BF5DB5
    Private Sub MapDownloader() '44b82ee9 is the old game freezeadr
        'AddAllDefaultMapsToList()
        DeleteIfExists("mapdl.zip")
        While Not Globals.ExitGlobally 'MAIN LOOP
            If NightfireProcess.HasExited Then Exit While
            Try
                If MapFailedToLoad() Then 'Did the map fail to load?
                    Dim mappath = GetLastMap()
                    ClearMapFailure()
                    ClearLastMap()
                    Dim mapSp = mappath.Split("/") 'maps/dm_japan.bsp
                    Dim mapName As String = mapSp(1) 'dm_japan.bsp
                    mapName = mapName.Trim(ChrW(0)) 'remove trailing null bytes
                    mapName = mapName.Substring(0, mapName.Length - 4) 'dm_japan
                    ShowLoadScreen()
                    DownloadMap(mapName, mappath)
                    HideLoadScreen()
                    HandleErrors(mapName)
                    If reconnect Then ReconnectToServer()
                    reconnect = True
                    Threading.Thread.Sleep(400)
                    PrintToGameUnreliable("You can upload new maps at ftp:/ /nightfiremaps.myftp.biz")
                End If
                HideLoadScreen() 'This probably is not needed, but placed for extra precaution
            Catch ex As Exception
            End Try
            Threading.Thread.Sleep(100)
        End While
    End Sub
    Private Function DownloadData(ByVal path As String) As String
        Dim wc As New Net.WebClient()
        Return wc.DownloadString(path)
    End Function
    'Private Sub RenderDownloadOverlay()
    '    While DownloadComplete = False
    '        If main.nf.HasExited Then Exit While
    '        DrawTextOverlay("Verifying and Downloading Map: " & mapnametodownload, "Arial", 20, 255, 255, 255, FontStyle.Bold, 1, 24)
    '        Threading.Thread.Sleep(1)
    '    End While
    'End Sub
    Private Sub RenderDownloadProgressOverlayAsync()
        Dim kbytes As Double = KBIn
        While kbytes = KBIn
            If DownloadComplete Or Globals.NightfireProcess.HasExited Then Exit While
            DrawTextOverlay("Verifying and Downloading Map(" & mapnametodownload & "): " & KBIn.ToString & "KB/" & totalKB.ToString & "KB  " & percentage.ToString & "%", "Arial", 20, 255, 255, 255, FontStyle.Bold, 1, 64)
            Threading.Thread.Sleep(1)
        End While
    End Sub
    Private Sub RenderTimedOverlay(ByVal text As String)
        Dim time = Sys_FloatTime()
        While Sys_FloatTime() - time < 5
            DrawTextOverlay(text, "Arial", 24, 255, 0, 0, FontStyle.Bold, 1, 40)
            Threading.Thread.Sleep(1)
        End While
    End Sub
    'Private Function MapExists(ByVal map As String)
    '    If IO.File.Exists(IO.Directory.GetCurrentDirectory & "\bond\maps\" & map & ".bsp") Then Return True
    '    If IO.File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) & "\Gearbox Software\Nightfire\bond\maps\" & map & ".bsp") Then Return True
    '    map = "maps/" & map & ".bsp"
    '    For Each a In defaultmaps
    '        If map = a Then Return True
    '    Next
    '    Return False
    'End Function
    Private mapnametodownload As String = ""
    Private bytesIn As Double
    Private KBIn As Double
    Private totalBytes As Double
    Private totalKB As Double
    Private percentage As Double
    Private Sub client_ProgressChanged(ByVal sender As Object, ByVal e As DownloadProgressChangedEventArgs)
        bytesIn = Double.Parse(e.BytesReceived.ToString())
        KBIn = Math.Round(bytesIn / 1000, 0)
        totalBytes = Double.Parse(e.TotalBytesToReceive.ToString())
        totalKB = Math.Round(totalBytes / 1000, 0)
        percentage = Math.Round(bytesIn / totalBytes * 100, 0)
        RenderDownloadProgressOverlayAsync()
        Dim progressoverlaythr As New Threading.Thread(AddressOf RenderDownloadProgressOverlayAsync)
        progressoverlaythr.Start()
        'DrawTextOverlay("Progress:  " & KBIn.ToString & "KB/" & totalKB.ToString & "KB  " & percentage.ToString & "%", "Arial", 14, 255, 255, 255, FontStyle.Bold, 1, 32)
        'Overlay.ShowText("Progress:  " & KBIn.ToString & "KB/" & totalKB.ToString & "KB  " & percentage.ToString & "%", 1, 80)
    End Sub
    Private Sub client_DownloadCompleted(ByVal sender As Object, ByVal e As System.ComponentModel.AsyncCompletedEventArgs)
        'Overlay.HideText(0)
        ' Overlay.HideText(1)
        'DrawTextOverlay("Download Complete!", "Arial", 14, 255, 255, 255, FontStyle.Bold, 1, 32)
        'Overlay.ShowText("Download Complete!", 0, 50)
        DownloadComplete = True
    End Sub
    Private Sub StartDownload(ByVal file As String)
        Dim client As WebClient = New WebClient
        AddHandler client.DownloadProgressChanged, AddressOf client_ProgressChanged
        AddHandler client.DownloadFileCompleted, AddressOf client_DownloadCompleted
        client.DownloadFileAsync(New Uri(file), "mapdl.zip")
    End Sub
    Private Sub DownloadMap(ByVal mapName As String, ByVal mapPath As String)
        faileddownloading = False
        Try
            DeleteFile("mapdl.zip")
            Threading.Thread.Sleep(10)

            PrintToGame("Verifying and downloading map: " & mapName)
            'Overlay.ShowText("Downloading map: " & mapName, 0, 50)
            'Dim downloadoverlay As New Threading.Thread(AddressOf RenderDownloadOverlay)
            'downloadoverlay.Start()
            'Dim progressoverlaythr As New Threading.Thread(AddressOf RenderDownloadProgressOverlayAsync)
            'progressoverlaythr.Start()
            mapnametodownload = mapName
            StartDownload("http://nightfire.no-ip.org/maps/" & mapName & ".zip")
            While Not DownloadComplete
                Application.DoEvents()
                Threading.Thread.Sleep(1)
            End While
            Dim currentdir = IO.Directory.GetCurrentDirectory
            Dim z As New Zip.ZipFile("mapdl.zip")
            Try
                If Not IO.Directory.Exists("mapdl") Then
                    IO.Directory.CreateDirectory("mapdl")
                Else
                    IO.Directory.Delete("mapdl", True)
                End If
            Catch ex As Exception
            End Try
            z.ExtractAll("mapdl")
            z.Dispose()
            DeleteFile("mapdl.zip")
            Try
                CopyDirectory("mapdl", "bond")
            Catch ex As Exception
            End Try
            Try
                IO.Directory.Delete("mapdl")
            Catch ex As Exception
                haserrored = True
                errormsg = ex.Message
            End Try
        Catch ex As Exception
            PrintToGame(ex.Message)
            errormsg = ex.Message
            DeleteFile("mapdl.zip")
            haserrored = True
        End Try
        Try
            IO.Directory.Delete("mapdl", True)
        Catch ex As Exception
        End Try
        'Overlay.HideText(0)
        'Overlay.HideText(1)
        DownloadComplete = False
    End Sub
    'Private Sub AddAllDefaultMapsToList()
    '    defaultmaps.Add("maps/ctf_austria.bsp")
    '    defaultmaps.Add("maps/ctf_island.bsp")
    '    defaultmaps.Add("maps/ctf_japan.bsp")
    '    defaultmaps.Add("maps/ctf_jungle.bsp")
    '    defaultmaps.Add("maps/ctf_knox.bsp")
    '    defaultmaps.Add("maps/ctf_office.bsp")
    '    defaultmaps.Add("maps/ctf_romania.bsp")
    '    defaultmaps.Add("maps/ctf_tower.bsp")
    '    defaultmaps.Add("maps/dm_austria.bsp")
    '    defaultmaps.Add("maps/dm_casino.bsp")
    '    defaultmaps.Add("maps/dm_caviar.bsp")
    '    defaultmaps.Add("maps/dm_island.bsp")
    '    defaultmaps.Add("maps/dm_japan.bsp")
    '    defaultmaps.Add("maps/dm_jungle.bsp")
    '    defaultmaps.Add("maps/dm_knox.bsp")
    '    defaultmaps.Add("maps/dm_maint.bsp")
    '    defaultmaps.Add("maps/dm_office.bsp")
    '    defaultmaps.Add("maps/dm_power.bsp")
    '    defaultmaps.Add("maps/m1_austria01.bsp")
    '    defaultmaps.Add("maps/m1_austria02.bsp")
    '    defaultmaps.Add("maps/m1_austria03.bsp")
    '    defaultmaps.Add("maps/m1_austria04.bsp")
    '    defaultmaps.Add("maps/m2_airfield01.bsp")
    '    defaultmaps.Add("maps/m3_japan01.bsp")
    '    defaultmaps.Add("maps/m3_japan02.bsp")
    '    defaultmaps.Add("maps/m3_japan03.bsp")
    '    defaultmaps.Add("maps/m3_japan04.bsp")
    '    defaultmaps.Add("maps/m4_infiltrate01.bsp")
    '    defaultmaps.Add("maps/m4_infiltrate02.bsp")
    '    defaultmaps.Add("maps/m4_infiltrate03.bsp")
    '    defaultmaps.Add("maps/m4_infiltrate04.bsp")
    '    defaultmaps.Add("maps/m4_infiltrate05.bsp")
    '    defaultmaps.Add("maps/m4_infiltrate06.bsp")
    '    defaultmaps.Add("maps/m4_infiltrate07.bsp")
    '    defaultmaps.Add("maps/m4_infiltrate07.bsp")
    '    defaultmaps.Add("maps/m5_power01.bsp")
    '    defaultmaps.Add("maps/m5_power02.bsp")
    '    defaultmaps.Add("maps/m6_escape01.bsp")
    '    defaultmaps.Add("maps/m6_escape03.bsp")
    '    defaultmaps.Add("maps/m6_escape04.bsp")
    '    defaultmaps.Add("maps/m6_escape05.bsp")
    '    defaultmaps.Add("maps/m6_escape06.bsp")
    '    defaultmaps.Add("maps/m6_escape07.bsp")
    '    defaultmaps.Add("maps/m7_island01.bsp")
    '    defaultmaps.Add("maps/m7_island02.bsp")
    '    defaultmaps.Add("maps/m7_island03.bsp")
    '    defaultmaps.Add("maps/m7_island04.bsp")
    '    defaultmaps.Add("maps/m7_island05.bsp")
    '    defaultmaps.Add("maps/m7_island06.bsp")
    '    defaultmaps.Add("maps/m8_missile01.bsp")
    '    defaultmaps.Add("maps/m8_missile02.bsp")
    '    defaultmaps.Add("maps/m8_missile03.bsp")
    '    defaultmaps.Add("maps/m8_missile04.bsp")
    '    defaultmaps.Add("maps/m9_space01.bsp")
    'End Sub
    Private Sub ShowLoadScreen()
        If Not alreadyloading Then
            alreadyloading = True
            mem.Write(ADR_ShowLoadScreenFunc, New Byte() {1}) 'show load screen
        End If
    End Sub
    Private Sub HideLoadScreen()
        If alreadyloading Then
            alreadyloading = False
            mem.Write(ADR_ShowLoadScreenFunc, New Byte() {0}) 'hide load screen
        End If
    End Sub
    Private Sub ClearMapFailure()
        mem.Write(ADR_MapLoadFailure, New Byte() {0})
    End Sub
    Private Sub ReconnectToServer()
        concmd("cl_retry") ': Threading.Thread.Sleep(200) : concmd("cl_retry")
    End Sub
    Private Function MapFailedToLoad()
        If mem.Read(ADR_MapLoadFailure, 1)(0) = &H1 Then
            Return True
        End If
        Return False
    End Function
    Private Function GetLastMap()
        Return System.Text.Encoding.ASCII.GetString(mem.Read(ADR_LastMapThatFailed, 64)) '432D4B80, 64)) '32, some maps have long names so changed to 64
    End Function
    Private Sub ClearLastMap()
        For i As Integer = 0 To 64
            mem.Write(ADR_LastMapThatFailed + i, &H0)
        Next
    End Sub
    Private Sub HandleErrors(ByVal mapName As String)
        If haserrored = True Then
            ' Echo commands have a max length before they are cut off, so we must shorten them (better way would be to truncate and create a separate echo command
            Dim isfatal As Boolean = False

            If errormsg.Contains("already exists in this path") Then
                faileddownloading = True
                errormsg = "Mapdl.zip already exists, please manually delete it." 'Could not complete operation since a file already exists in this path 'mapdl.zip'.
                isfatal = True
            ElseIf errormsg.Contains("Could not connect to remote server") Then
                isfatal = True
                faileddownloading = True
            ElseIf errormsg.Contains("Unable to connect to remote server") Then
                errormsg = errormsg.Replace("Unable to connect to remote server", "Map Download Failed: Remote server is offline!")
                isfatal = True
                faileddownloading = True
            ElseIf errormsg.Contains("The remote name could not be resolved") Then
                errormsg = errormsg.Replace("The remote name could not be resolved:", "Can't resolve: ")
                isfatal = True
                faileddownloading = True
            ElseIf errormsg.Contains("The process cannot access the file '" & IO.Directory.GetCurrentDirectory & "mapdl.zip' because it is being used by another process.") Then
                'PrintToGameUnreliable("Map download failed for " & mapName & ":")
                errormsg = "Another process is currently using mapdl.zip"
                faileddownloading = True
                isfatal = True
            ElseIf errormsg.Contains("404") Or errormsg.Contains("Not Found") Or errormsg = "Could not read mapdl.zip as a zip file" Then
                isfatal = True
                errormsg = "This map is not uploaded (404 NOT FOUND)"
                faileddownloading = True
            ElseIf errormsg.Contains("already exists.") Then
                errormsg = " "
            ElseIf errormsg = "mapdl.zip is not a valid zip file" Then
                errormsg = "The download is corrupt and couldn't be unzipped, deleting."
                DeleteFile("mapdl.zip")
                isfatal = True
                faileddownloading = True
            End If
            If isfatal = True Then
                Threading.Thread.Sleep(500)
                'Overlay.HideText(0)
                'Overlay.HideText(1)
                RenderTimedOverlay("ERROR: The map failed to download, reason:" & ControlChars.NewLine & errormsg)
                'DrawTextOverlay("ERROR: The map failed to download, reason:", "Arial", 14, 255, 255, 255, FontStyle.Bold, 1, 32)
                'DrawTextOverlay(errormsg, "Arial", 14, 255, 255, 255, FontStyle.Bold, 1, 40)

                'Overlay.ShowText("ERROR: The map failed to download, reason:", 0, 50)
                'Overlay.ShowText(errormsg, 1, 80)
                reconnect = False
                PrintToGameUnreliable("Failed to download the following map:")
                Threading.Thread.Sleep(500) 'Must have sleep between echo commands or they won't work
                PrintToGameUnreliable("" & "nightfire.no-ip.org/maps/" & mapName & ".zip") 'map
                Threading.Thread.Sleep(500)
                PrintToGameUnreliable("-------------------------------------")
                Threading.Thread.Sleep(2000)
                'Overlay.HideText(0)
                'Overlay.HideText(1)
            End If
            Threading.Thread.Sleep(350)
            PrintToGameUnreliable("" & errormsg)
            isfatal = False
        End If
        If faileddownloading = False Then
            RenderTimedOverlay("Download completed!")
            'DrawTextOverlay("Successfully downloaded and extracted mapdl.zip", "Arial", 14, 255, 255, 255, FontStyle.Bold, 1, 32)
            'Overlay.ShowText("Successfully downloaded and extracted mapdl.zip", 0, 50)
            PrintToGameUnreliable("Successfully downloaded and extracted the following file:")
            Threading.Thread.Sleep(1500)
            PrintToGameUnreliable("" & "nightfire.no-ip.org/maps/" & mapName & ".zip")
            'Overlay.HideText(0)
        End If
        haserrored = False
        faileddownloading = False
    End Sub
    Private Sub CopyDirectory(ByVal strSrc As String, ByVal strDest As String) 'source: http://bytes.com/topic/visual-basic-net/answers/365349-copying-all-contents-one-folder-into-another
        Dim dirInfo As New System.IO.DirectoryInfo(strSrc)
        Dim fsInfo As System.IO.FileSystemInfo

        If Not System.IO.Directory.Exists(strDest) Then
            System.IO.Directory.CreateDirectory(strDest)
        End If

        For Each fsInfo In dirInfo.GetFileSystemInfos
            Dim strDestFileName As String = System.IO.Path.Combine(strDest,
            fsInfo.Name)

            If TypeOf fsInfo Is System.IO.FileInfo Then
                System.IO.File.Copy(fsInfo.FullName, strDestFileName, True)
                'This will overwrite files that already exist
            Else
                CopyDirectory(fsInfo.FullName, strDestFileName)
            End If

        Next
    End Sub
    'Private Declare Function ShowWindow Lib "user32.dll" (ByVal hWnd As IntPtr, ByVal nCmdShow As SHOW_WINDOW) As Boolean
    '<Flags()> Private Enum SHOW_WINDOW As Integer
    '    SW_HIDE = 0
    '    SW_SHOWNORMAL = 1
    '    SW_NORMAL = 1
    '    SW_SHOWMINIMIZED = 2
    '    SW_SHOWMAXIMIZED = 3
    '    SW_MAXIMIZE = 3
    '    SW_SHOWNOACTIVATE = 4
    '    SW_SHOW = 5
    '    SW_MINIMIZE = 6
    '    SW_SHOWMINNOACTIVE = 7
    '    SW_SHOWNA = 8
    '    SW_RESTORE = 9
    '    SW_SHOWDEFAULT = 10
    '    SW_FORCEMINIMIZE = 11
    '    SW_MAX = 11
    'End Enum
    Private Sub PrintToGameUnreliable(ByVal line As String)
        unreliable_concmd("echo " & line)
    End Sub
    Private Sub PrintToGame(ByVal line As String)
        concmd("echo " & line)
    End Sub
    'Private Sub MinimizeMe()
    '    For Each p As Process In Process.GetProcessesByName("MapDownloader")
    '        ShowWindow(p.MainWindowHandle, SHOW_WINDOW.SW_MINIMIZE)
    '    Next
    'End Sub
    'Private Sub ShowMe()
    '    For Each p As Process In Process.GetProcessesByName("MapDownloader")
    '        ShowWindow(p.MainWindowHandle, SHOW_WINDOW.SW_SHOWNORMAL)
    '    Next
    'End Sub
    'Private Sub MinimizeBond()
    '    For Each p As Process In Process.GetProcessesByName(NightfireEXE)
    '        ShowWindow(p.MainWindowHandle, SHOW_WINDOW.SW_FORCEMINIMIZE)
    '        'ShowWindow(p.MainWindowHandle, SHOW_WINDOW.SW_MINIMIZE) 'infinite loop dont use
    '    Next
    'End Sub
    'Private Sub MaximizeBond()
    '    For Each p As Process In Process.GetProcessesByName(NightfireEXE)
    '        ShowWindow(p.MainWindowHandle, SHOW_WINDOW.SW_RESTORE)
    '    Next
    'End Sub
    'Private Declare Function GetTickCount& Lib "kernel32" ()
End Class

