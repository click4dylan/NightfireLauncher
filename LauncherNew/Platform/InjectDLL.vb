Imports System.Runtime.InteropServices
Imports System.Text
Module Inject
    Private Declare Function OpenProcess Lib "kernel32" (ByVal dwDesiredAccess As Integer, ByVal bInheritHandle As Integer, ByVal dwProcessId As Integer) As Integer
    Private Declare Function VirtualAllocEx Lib "kernel32" (ByVal hProcess As Integer, ByVal lpAddress As Integer, ByVal dwSize As Integer, ByVal flAllocationType As Integer, ByVal flProtect As Integer) As Integer
    Private Declare Function WriteProcessMemory Lib "kernel32" (ByVal hProcess As Integer, ByVal lpBaseAddress As Integer, ByVal lpBuffer() As Byte, ByVal nSize As Integer, ByVal lpNumberOfBytesWritten As UInteger) As Boolean
    Private Declare Function GetProcAddress Lib "kernel32" (ByVal hModule As Integer, ByVal lpProcName As String) As Integer
    Private Declare Function GetModuleHandle Lib "kernel32" Alias "GetModuleHandleA" (ByVal lpModuleName As String) As Integer
    Private Declare Function CreateRemoteThread Lib "kernel32" (ByVal hProcess As Integer, ByVal lpThreadAttributes As Integer, ByVal dwStackSize As Integer, ByVal lpStartAddress As Integer, ByVal lpParameter As Integer, ByVal dwCreationFlags As Integer, ByVal lpThreadId As Integer) As Integer
    Private Declare Function CreateEvent Lib "kernel32" Alias "CreateEventA" (ByVal lpEventAttributes As IntPtr, ByVal bManualReset As Boolean, ByVal bInitialState As Boolean, ByVal lpName As String) As IntPtr
    Private Declare Function WaitForSingleObject Lib "kernel32" (ByVal hHandle As Integer, ByVal dwMilliseconds As Integer) As Integer
    Private Declare Function CloseHandle Lib "kernel32" (ByVal hObject As Integer) As Integer
    Public Function InjectFile(ByVal DLLPath As String) As Boolean
#If 0 Then
        Dim ProcessName As Process() = Process.GetProcessesByName("Bond2")
        Dim ProcessID = ProcessName(0).Id
        DLLPath = IO.Directory.GetCurrentDirectory & "\" & DLLPath
        On Error GoTo exiterror
        Dim DProc As Integer
        Dim DAdd As Integer
        Dim DWrote As UInteger
        Dim DAll As Integer
        Dim DLLEntry As Integer
        Dim DThe As Integer
        Dim DThe2 As Integer
        Dim DMHD As Integer
        DProc = OpenProcess(&H1F0FFF, 1, ProcessID)
        DAdd = VirtualAllocEx(DProc, 0, DLLPath.Length, &H1000, &H4)
        If DAdd = Nothing Then
            GoTo exiterror
        End If
        Dim DByte() As Byte
        DByte = StrChar(DLLPath)
        WriteProcessMemory(DProc, DAdd, DByte, DLLPath.Length, DWrote)
        DMHD = GetModuleHandle("kernel32.dll")
        If DMHD = Nothing Then
            GoTo exiterror
        End If
        DAll = GetProcAddress(DMHD, "LoadLibraryA")
        If DAll = Nothing Then
            GoTo exiterror
        End If
        DThe = CreateRemoteThread(DProc, 0, 0, DAll, DAdd, 0, 0)
        If DThe = Nothing Then
            GoTo exiterror
        End If
        If WaitForSingleObject(DThe, &HFFFF) Then
            GoTo exiterror
        End If
        DLLEntry = GetProcAddress(DThe, "entryPoint")
        If DLLEntry = Nothing Then
            GoTo exiterror
        End If
        Dim eventc As IntPtr = CreateEvent(0, False, False, "RInputEvent32")
        If eventc = Nothing Then
            GoTo exiterror
        End If
        DThe2 = CreateRemoteThread(DProc, 0, 0, DLLEntry, 0, 0, eventc)
        If DThe2 = Nothing Then
            GoTo exiterror
        End If
        If WaitForSingleObject(DThe2, &H1964) Then
            GoTo exiterror
        End If
        CloseHandle(DThe)
        CloseHandle(DThe2)
        CloseHandle(DProc)
        InjectFile = True
        Return True
exiterror:
        If Not DThe = Nothing Then
            CloseHandle(DThe)
        End If
        If Not DThe2 = Nothing Then
            CloseHandle(DThe2)
        End If
        If Not DProc = Nothing Then
            CloseHandle(DProc)
        End If
        InjectFile = False
        Return False
#Else
        Return True
#End If
    End Function
    Private Function StrChar(ByRef strString As String) As Byte()
        Dim bytTemp() As Byte
        Dim i As Short
        ReDim bytTemp(0)
        For i = 1 To Len(strString)
            If bytTemp(UBound(bytTemp)) <> 0 Then ReDim Preserve bytTemp(UBound(bytTemp) + 1)
            bytTemp(UBound(bytTemp)) = Asc(Mid(strString, i, 1))
        Next i
        ReDim Preserve bytTemp(UBound(bytTemp) + 1)
        bytTemp(UBound(bytTemp)) = 0
        StrChar = bytTemp
    End Function
End Module