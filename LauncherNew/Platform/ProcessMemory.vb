Imports System
Imports System.Diagnostics
Imports System.Runtime.InteropServices
Imports System.Text

Public Class ProcessMemory
    Public Enum ProcessAccess As UInteger
        All = &H1F0FFF
        Terminate = &H1
        CreateThread = &H2
        VMOperation = &H8
        VMRead = &H10
        VMWrite = &H20
        DupHandle = &H40
        SetInformation = &H200
        QueryInformation = &H400
        Synchronize = &H100000
    End Enum
    Declare Function GetAsyncKeyState Lib "user32.dll" (ByVal vKey As Int32) As UShort
    <DllImport("kernel32.dll")> _
    Private Shared Function OpenProcess(ByVal dwDesiredAccess As ProcessAccess, <MarshalAs(UnmanagedType.Bool)> ByVal bInheritHandle As Boolean, ByVal dwProcessId As Integer) As Integer
    End Function
    <DllImport("kernel32.dll", SetLastError:=True)> _
    Public Shared Function CloseHandle(ByVal hObject As Integer) As <MarshalAs(UnmanagedType.Bool)> Boolean
    End Function
    <DllImport("kernel32.dll", SetLastError:=True)> _
    Public Shared Function ReadProcessMemory( _
    ByVal hProcess As Integer, _
    ByVal lpBaseAddress As Integer, _
    <Out()> ByVal lpBuffer() As Byte, _
    ByVal dwSize As Integer, _
    ByRef lpNumberOfBytesRead As Integer
  ) As Boolean
    End Function
    <DllImport("kernel32.dll", SetLastError:=True)> _
    Public Shared Function ReadProcessMemory( _
    ByVal hProcess As Integer, _
        ByVal lpBaseAddress As Integer, _
       <Out(), MarshalAs(UnmanagedType.AsAny)> ByVal lpBuffer As Object, _
    ByVal dwSize As Integer, _
    ByRef lpNumberOfBytesRead As Integer _
   ) As Boolean
    End Function
    <DllImport("kernel32.dll", SetLastError:=True)> _
    Public Shared Function ReadProcessMemory( _
    ByVal hProcess As Integer, _
    ByVal lpBaseAddress As Integer, _
    ByVal lpBuffer As Integer, _
    ByVal iSize As Integer, _
    ByRef lpNumberOfBytesRead As Integer) As Boolean
    End Function
    <DllImport("kernel32.dll", SetLastError:=True)> _
    Public Shared Function WriteProcessMemory(ByVal hProcess As Integer, ByVal lpBaseAddress As Integer, ByVal lpBuffer As Byte(), ByVal nSize As System.UInt32, <Out()> ByRef lpNumberOfBytesWritten As Int32) As Boolean
    End Function
    Private _Process As Process = Nothing
    Public ReadOnly Property Process() As Process
        Get
            Return _Process
        End Get
    End Property
    Private _Access As UInteger
    Public ReadOnly Property Access() As UInteger
        Get
            Return _Access
        End Get
    End Property
    Private _Handle As Integer = 0
    Public ReadOnly Property Handle() As Integer
        Get
            Return _Handle
        End Get
    End Property
    Private _Position As Integer = 0
    Public Property Position() As Long
        Get
            Return _Position
        End Get
        Set(ByVal value As Long)
            _Position = value
        End Set
    End Property
    ''' <summary>
    ''' This class allows you to read and write to another processes memory.
    ''' </summary>
    ''' <param name="process">The process to attach to.</param>
    ''' <param name="access">Use ProcessAccess.  Reading requires PROCESS_VM_READ while writing requires both PROCESS_VM_WRITE and PROCESS_VM_OPERATION.</param>
    Public Sub New(ByVal process As Process, ByVal access As UInteger)
        _Process = process
        _Access = access
    End Sub
    Public Sub New(ByVal process As Process, ByVal access As ProcessAccess)
        _Process = process
        _Access = CUInt(access)
    End Sub
    Public Sub Open()
        _Handle = OpenProcess(_Access, False, CUInt(_Process.Id))
    End Sub
    Public Sub Close()
            Dim iRetValue As Integer
            iRetValue = CloseHandle(_Handle)
            If iRetValue = 0 Then
                Throw New Exception("CloseHandle failed")
            End If
    End Sub
#Region " Write Methods "
    Public Function Write(ByVal buffer As Byte()) As Integer
        Return Write(_Position, buffer)
    End Function
    Public Function Write(ByVal address As Integer, ByVal buffer As Byte()) As Integer
        Dim BytesWritten As Integer
        WriteProcessMemory(_Handle, address, buffer, CUInt(buffer.Length), BytesWritten)
        _Position = address + BytesWritten
        Dim a = Marshal.GetLastWin32Error.ToString
        Return BytesWritten
    End Function
    Public Function Write(ByVal address As Integer, ByVal value As Byte) As Integer
        Return Write(address, New Byte() {value})
    End Function
    Public Function Write(ByVal address As Integer, ByVal value As Single) As Integer
        Return Write(address, BitConverter.GetBytes(value))
    End Function
    Public Function Write(ByVal address As Integer, ByVal value As Double) As Integer
        Return Write(address, BitConverter.GetBytes(value))
    End Function
    Public Function Write(ByVal address As Integer, ByVal value As Boolean) As Integer
        Return Write(address, BitConverter.GetBytes(value))
    End Function
    Public Function Write(ByVal address As Integer, ByVal value As Char) As Integer
        Return Write(address, BitConverter.GetBytes(value))
    End Function
    Public Function Write(ByVal address As Integer, ByVal value As Short) As Integer
        Return Write(address, BitConverter.GetBytes(value))
    End Function
    Public Function Write(ByVal address As Integer, ByVal value As UShort) As Integer
        Return Write(address, BitConverter.GetBytes(value))
    End Function
    Public Function Write(ByVal address As Integer, ByVal value As Integer) As Integer
        Return Write(address, BitConverter.GetBytes(value))
    End Function
    Public Function Write(ByVal address As Integer, ByVal value As UInteger) As Integer
        Return Write(address, BitConverter.GetBytes(value))
    End Function
    Public Function Write(ByVal address As Integer, ByVal value As Long) As Integer
        Return Write(address, BitConverter.GetBytes(value))
    End Function
    Public Function Write(ByVal address As Integer, ByVal value As ULong) As Integer
        Return Write(address, BitConverter.GetBytes(value))
    End Function
    Public Function Write(ByVal address As Integer, ByVal value As String, ByVal encoding As Encoding) As Integer
        Return Write(address, encoding.GetBytes(value))
    End Function
#End Region
#Region " Read Methods "
    Public Function Read(ByVal length As UInteger) As Byte()
        Return Read(_Position, length)
    End Function
    Public Function Read(ByVal address As Integer, ByVal length As Integer) As Byte()
        Dim buffer As Byte() = New Byte(length - 1) {}

        Dim _read As Integer
        Dim _read2 As Integer
        ReadProcessMemory(_Handle, address, buffer, length, _read2)
        _read = _read2
        _Position = address + _read

        Return buffer
    End Function
    Public Function ReadUInt8(ByVal address As Integer) As Byte
        Dim temp As Byte() = Read(address, 1)
        Return temp(0)
    End Function
    Public Function ReadSingle(ByVal address As Integer) As Single
        Dim read__1 As Single() = New Single(0) {}
        Dim gch As GCHandle = GCHandle.Alloc(Read(address, 4), GCHandleType.Pinned)
        Marshal.Copy(gch.AddrOfPinnedObject(), read__1, 0, 1)
        gch.Free()
        Return read__1(0)
    End Function
    Public Function ReadDouble(ByVal address As Integer) As Double
        Dim read__1 As Double() = New Double(0) {}
        Dim gch As GCHandle = GCHandle.Alloc(Read(address, 8), GCHandleType.Pinned)
        Marshal.Copy(gch.AddrOfPinnedObject(), read__1, 0, 1)
        gch.Free()
        Return read__1(0)
    End Function
    Public Function ReadChar(ByVal address As Integer) As Char
        Dim read__1 As Char() = New Char(0) {}
        Dim gch As GCHandle = GCHandle.Alloc(Read(address, 2), GCHandleType.Pinned)
        Marshal.Copy(gch.AddrOfPinnedObject(), read__1, 0, 1)
        gch.Free()
        Return read__1(0)
    End Function
    Public Function ReadInt16(ByVal address As Integer) As Short
        Dim read__1 As Short() = New Short(0) {}
        Dim gch As GCHandle = GCHandle.Alloc(Read(address, 2), GCHandleType.Pinned)
        Marshal.Copy(gch.AddrOfPinnedObject(), read__1, 0, 1)
        gch.Free()
        Return read__1(1)
    End Function
    Public Function ReadInt32(ByVal address As Integer) As Integer
        Dim read__1 As Integer() = New Integer(0) {}
        Dim gch As GCHandle = GCHandle.Alloc(Read(address, 4), GCHandleType.Pinned)
        Marshal.Copy(gch.AddrOfPinnedObject(), read__1, 0, 1)
        gch.Free()
        Return read__1(0)
    End Function
    Public Function ReadUInt32(ByVal address As Integer) As UInteger
        Return BitConverter.ToUInt32(Read(address, 4), 0)
    End Function
    Public Function ReadInt64(ByVal address As Integer) As Long
        Dim read__1 As Long() = New Long(0) {}
        Dim gch As GCHandle = GCHandle.Alloc(Read(address, 8), GCHandleType.Pinned)
        Marshal.Copy(gch.AddrOfPinnedObject(), read__1, 0, 1)
        gch.Free()
        Return read__1(0)
    End Function
    Public Function ReadString(ByVal address As Integer, ByVal length As UInteger, ByVal encoding As Encoding) As String
        Return encoding.GetString(Read(address, length))
    End Function
#End Region

End Class

