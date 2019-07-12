Public Module Globals
    Public SplashScreenForm As SplashScreen = Nothing
    Public Initialized As Boolean = False
    Public ExitGlobally As Boolean = False
    Public NightfireProcess As Process
    Public ConnectArg As String = " " 'force the connect
    Public ExtraCommandLineArgs As String = " " 'Extra arguments
    Public RawMouseInput As Boolean = True 'Enable Raw Mouse Input?
    Public NoUpdate As Boolean = False 'No update argument
    Public NoJoy As Boolean = False 'Don't enable joystick/xinput controller support
    Public AlwaysUpdate As Boolean = False 'No Update Message Box (always update game)
    Public CPath As String = ""
    Public DontLaunchNF As Boolean = False 'Don't Launch Nightfire
    Public Hunk As Integer = 0
    Public Heap As Integer = 0
    Public Const NULL = Nothing
    Public restart As Boolean = False
    Public BondEXE As String = "Bond2.exe"
    Public Const BOND2HASH As String = "22F1C01AF2B989A874C54D151001F37A"
End Module
