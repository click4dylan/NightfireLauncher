Module Overlay
    Private Declare Function DrawTextA Lib "user32" (ByVal hdc As IntPtr, ByVal lpStr As String, ByVal nCount As IntPtr, ByRef lpRect As RECT, ByVal wFormat As IntPtr) As IntPtr
    Private Declare Function SetTextColor Lib "gdi32" (hdc As IntPtr, crColor As Integer) As UInteger
    'Private Declare Function SetBkColor Lib "gdi32" (hdc As IntPtr, crColor As Integer) As UInteger
    Private Declare Function SetBkMode Lib "gdi32" (ByVal prmlngHDc As IntPtr, ByVal ColorRef As Integer) As Integer
    'Private Declare Function GetDesktopWindow Lib "user32" () As IntPtr
    Private Declare Function UpdateWindow Lib "user32" (ByVal hwnd As IntPtr) As Boolean
    Private Declare Function GetClientRect Lib "user32" (ByVal hWnd As System.IntPtr, ByRef lpRECT As RECT) As Integer
    Private Declare Function ReleaseDC Lib "user32" (ByVal hwnd As IntPtr, ByVal hDC As IntPtr) As IntPtr
    'Private Declare Auto Function BitBlt Lib "gdi32" (ByVal hdcDest As IntPtr, ByVal nXDest As Integer, ByVal nYDest As Integer, ByVal nWidth As Integer, ByVal nHeight As Integer, ByVal hdcSrc As IntPtr, ByVal nXSrc As Integer, ByVal nYSrc As Integer, ByVal dwRop As System.Int32) As Boolean
    'Private Const SRCCOPY As Integer = &HCC0020
    Private Declare Function GetWindowDC Lib "user32" (ByVal hwnd As IntPtr) As IntPtr
    Private Structure RECT
        Dim Left As Integer
        Dim Top As Integer
        Dim Right As Integer
        Dim Bottom As Integer
    End Structure
    Private Const TRANSPARENT As Integer = 1
    'Private Const OPAQUE As Integer = 2
    'Private Const BKMODE_LAST As Integer = 2
    'Private Const WHITE_BRUSH As Integer = 0
    'Private Const LTGRAY_BRUSH As Integer = 1
    'Private Const GRAY_BRUSH As Integer = 2
    'Private Const DKGRAY_BRUSH As Integer = 3
    'Private Const BLACK_BRUSH As Integer = 4
    'Private Const NULL_BRUSH As Integer = 5
    'Private Const HOLLOW_BRUSH As Integer = NULL_BRUSH
    'Private Const WHITE_PEN As Integer = 6
    'Private Const BLACK_PEN As Integer = 7
    'Private Const NULL_PEN As Integer = 8
    'Private Const OEM_FIXED_FONT As Integer = 10
    'Private Const ANSI_FIXED_FONT As Integer = 11
    'Private Const ANSI_VAR_FONT As Integer = 12
    'Private Const SYSTEM_FONT As Integer = 13
    'Private Const DEVICE_DEFAULT_FONT As Integer = 14
    'Private Const DEFAULT_PALETTE As Integer = 15
    'Private Const SYSTEM_FIXED_FONT As Integer = 16
    Private layhWnd As IntPtr
    Private layhdc As IntPtr
    Private layrect As RECT
    Private Function GetClientRectWrapper(ByVal hWnd As IntPtr) As RECT
        Dim result As RECT = New RECT
        GetClientRect(hWnd, result)
        Return result
    End Function
    Public Sub DrawTextOverlay(ByVal text As String, ByVal fontname As String, ByVal size As Integer, ByVal red As Integer, ByVal green As Integer, ByVal blue As Integer, ByVal style As FontStyle, ByVal x As Integer, ByVal y As Integer)
        If Globals.NightfireProcess.HasExited Then Exit Sub
        layhWnd = Globals.NightfireProcess.MainWindowHandle 'GetDesktopWindow()
        On Error GoTo Failed
        layhdc = GetWindowDC(layhWnd)
        'Dim layfont As New Drawing.Font("Arial", 15)
        'Dim laypointf As New Drawing.PointF(5, 5)
        layrect = GetClientRectWrapper(layhWnd)
        layrect.Left = x
        layrect.Top = y
        SetTextColor(layhdc, RGB(red, green, blue))
        SetBkMode(layhdc, TRANSPARENT)
        DrawTextA(layhdc, text, -1, layrect, 0)
        UpdateWindow(layhWnd)
        ReleaseDC(layhWnd, layhdc)
Failed:
    End Sub
    'Sub InitializeOverlay()
    '    curtime = Environment.TickCount
    '    screenwidth = 0
    '    While screenwidth = 0
    '        Toolbox.Proxy.DirectXProxy.GPSI_GetScreenSize(screenwidth, screenheight)
    '        If main.nf.HasExited Or Environment.TickCount - curtime > 5000 Then Exit Sub
    '        Threading.Thread.Sleep(10)
    '    End While
    'End Sub
    'Public blanktext = "                                                                                                                                                                                                                                                                                "
    'Sub ShowText(ByVal text As String, ByVal index As Byte, ByVal height As UShort)
    '    InitializeOverlay()
    '    Toolbox.Proxy.DirectXProxy.GPML_SetTextMultilineData(index, 50, height, blanktext, Color.FromArgb(255, Color.White).ToArgb, False, 32, True, screenwidth - 100, screenheight - 100, 1)
    '    Toolbox.Proxy.DirectXProxy.GPML_SetTextMultilineData(index, 50, height, text, Color.FromArgb(255, Color.White).ToArgb, False, 32, True, screenwidth - 100, screenheight - 100, 1)
    '    Toolbox.Proxy.DirectXProxy.GPML_ShowText(index, True) 'shows the text
    'End Sub
    'Sub HideText(ByVal index As Byte)
    '    Toolbox.Proxy.DirectXProxy.GPML_ShowText(index, False) 'hides the text
    'End Sub
    'Sub ShowPicture(ByVal file As String, ByVal x As UShort, ByVal y As UShort)
    '    InitializeOverlay()
    '    Toolbox.Proxy.DirectXProxy.GPPIC_LoadNewPicture(file)
    '    Toolbox.Proxy.DirectXProxy.GPPIC_ShowPicturePos(True, x, y)
    'End Sub
    'Sub HidePicture(ByVal x As UShort, ByVal y As UShort)
    '    Toolbox.Proxy.DirectXProxy.GPPIC_ShowPicturePos(False, x, y)
    'End Sub


    'Vb.net solution, flickers bad
    'Public deviceContext As Graphics
    'Public drawFormat As New StringFormat()
    'Sub InitializeOverlay()
    '    deviceContext = Graphics.FromHwnd(main.nf.MainWindowHandle)
    '    drawFormat.FormatFlags = StringFormatFlags.NoFontFallback
    'End Sub
    'Sub ShowText(ByVal text As String, ByVal fontname As String, ByVal size As Integer, ByVal color1 As Color, ByVal color2 As Color, ByVal gradienttype As Drawing2D.LinearGradientMode, ByVal style As FontStyle, ByVal x As Integer, ByVal y As Integer)
    '    On Error Resume Next
    '    deviceContext.DrawString(text, New Font(fontname, size),
    '                             New Drawing2D.LinearGradientBrush(deviceContext.ClipBounds, color1, color2, gradienttype),
    '                             x, y, drawFormat
    '                             )
    'End Sub
End Module
