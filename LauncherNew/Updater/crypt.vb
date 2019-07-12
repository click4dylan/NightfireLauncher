Imports System.Security.Cryptography
Imports System.IO
Imports System.Text

Public Class crypt
    'Byte vector required for Rijndael.  This is randomly generated and recommended you change it on a per-application basis.
    'It is 16 bytes.
    Private bytIV() As Byte = {183, 203, 48, 202, 196, 170, 201, 169, 60, 138, 184, 186, 84, 184, 171, 84}
    Private enc As New HexEncoding
    'Character to pad keys with to make them at least intMinKeySize.
    Private Const chrKeyFill As Char = "X"c

    'String to display on error for functions that return strings. {0} is Exception.Message.
    Private Const strTextErrorString As String = "#ERROR - {0}"

    'Min size in bytes of randomly generated salt.
    Private Const intMinSalt As Integer = 4

    'Max size in bytes of randomly generated salt.
    Private Const intMaxSalt As Integer = 8

    'Size in bytes of Hash result.  MD5 returns a 128 bit hash.
    Private Const intHashSize As Integer = 16

    'Size in bytes of the key length.  Rijndael takes either a 128, 192, or 256 bit key.  
    'If it is under this, pad with chrKeyFill. If it is over this, truncate to the length.
    Private Const intKeySize As Integer = 32

    'Encrypt a String with Rijndael symmetric encryption.
    Public Function EncryptString128Bit(ByVal strPlainText As String, ByVal strKey As String) As String
        Try
            Dim bytPlainText() As Byte
            Dim bytKey() As Byte
            Dim bytEncoded() As Byte
            Dim objMemoryStream As New MemoryStream
            Dim objRijndaelManaged As New RijndaelManaged

            strPlainText = strPlainText.Replace(vbNullChar, String.Empty)

            bytPlainText = Encoding.UTF8.GetBytes(strPlainText)
            bytKey = ConvertKeyToBytes(strKey)

            Dim objCryptoStream As New CryptoStream(objMemoryStream, _
                objRijndaelManaged.CreateEncryptor(bytKey, bytIV), _
                CryptoStreamMode.Write)

            objCryptoStream.Write(bytPlainText, 0, bytPlainText.Length)
            objCryptoStream.FlushFinalBlock()

            bytEncoded = objMemoryStream.ToArray
            objMemoryStream.Close()
            objCryptoStream.Close()
            Return HexEncoding.GetString(bytEncoded)
        Catch ex As Exception
            Return String.Format(strTextErrorString, ex.Message)
        End Try
    End Function

    'Decrypt a String with Rijndael symmetric encryption.
    Public Function DecryptString128Bit(ByVal strCryptText As String, ByVal strKey As String) As String
        Dim bytCryptText() As Byte
        Dim bytKey() As Byte

        Dim objRijndaelManaged As New RijndaelManaged

        bytCryptText = HexEncoding.GetBytes(strCryptText, 0)
        bytKey = ConvertKeyToBytes(strKey)

        Dim bytTemp(bytCryptText.Length) As Byte
        Dim objMemoryStream As New MemoryStream(bytCryptText)

        Dim objCryptoStream As New CryptoStream(objMemoryStream, _
            objRijndaelManaged.CreateDecryptor(bytKey, bytIV), _
            CryptoStreamMode.Read)

        objCryptoStream.Read(bytTemp, 0, bytTemp.Length)

        objMemoryStream.Close()
        objCryptoStream.Close()

        Return Encoding.UTF8.GetString(bytTemp).Replace(vbNullChar, String.Empty)
    End Function

    'Compute an MD5 hash code from a string and append any salt-bytes used/generated to the end.
    Public Function ComputeMD5Hash(ByVal strPlainText As String, Optional ByVal bytSalt() As Byte = Nothing) As String
        Try
            Dim bytPlainText As Byte() = Encoding.UTF8.GetBytes(strPlainText)
            Dim hash As HashAlgorithm = New MD5CryptoServiceProvider()

            If bytSalt Is Nothing Then
                Dim rand As New Random
                Dim intSaltSize As Integer = rand.Next(intMinSalt, intMaxSalt)

                bytSalt = New Byte(intSaltSize - 1) {}

                Dim rng As New RNGCryptoServiceProvider
                rng.GetNonZeroBytes(bytSalt)
            End If

            Dim bytPlainTextWithSalt() As Byte = New Byte(bytPlainText.Length + bytSalt.Length - 1) {}

            bytPlainTextWithSalt = ConcatBytes(bytPlainText, bytSalt)

            Dim bytHash As Byte() = hash.ComputeHash(bytPlainTextWithSalt)
            Dim bytHashWithSalt() As Byte = New Byte(bytHash.Length + bytSalt.Length - 1) {}

            bytHashWithSalt = ConcatBytes(bytHash, bytSalt)

            Return Convert.ToBase64String(bytHashWithSalt)
        Catch ex As Exception
            Return String.Format(strTextErrorString, ex.Message)
        End Try
    End Function

    'Verify a string against a hash generated with the ComputeMD5Hash function above.
    Public Function VerifyHash(ByVal strPlainText As String, ByVal strHashValue As String) As Boolean
        Try
            Dim bytWithSalt As Byte() = Convert.FromBase64String(strHashValue)

            If bytWithSalt.Length < intHashSize Then Return False

            Dim bytSalt() As Byte = New Byte(bytWithSalt.Length - intHashSize - 1) {}

            Array.Copy(bytWithSalt, intHashSize, bytSalt, 0, bytWithSalt.Length - intHashSize)

            Dim strExpectedHashString As String = ComputeMD5Hash(strPlainText, bytSalt)

            Return strHashValue.Equals(strExpectedHashString)
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    'Simple function to concatenate two byte arrays. 
    Private Function ConcatBytes(ByVal bytA() As Byte, ByVal bytB() As Byte) As Byte()
        Try
            Dim bytX() As Byte = New Byte(((bytA.Length + bytB.Length)) - 1) {}

            Array.Copy(bytA, bytX, bytA.Length)
            Array.Copy(bytB, 0, bytX, bytA.Length, bytB.Length)

            Return bytX
        Catch ex As Exception
            Return Nothing
        End Try

    End Function

    'A function to convert a string into a 32 byte key. 
    Private Function ConvertKeyToBytes(ByVal strKey As String) As Byte()
        Try
            Dim intLength As Integer = strKey.Length

            If intLength < intKeySize Then
                strKey &= Strings.StrDup(intKeySize - intLength, chrKeyFill)
            Else
                strKey = strKey.Substring(0, intKeySize)
            End If

            Return Encoding.UTF8.GetBytes(strKey)
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

End Class

''' <summary>
''' Summary description for HexEncoding.
''' </summary>
Public Class HexEncoding
    Public Sub New()
        '
        ' TODO: Add constructor logic here
        ''
    End Sub
    Public Shared Function GetByteCount(ByVal hexString As String) As Integer
        Dim numHexChars As Integer = 0
        Dim c As Char
        ' remove all none A-F, 0-9, characters
        For i As Integer = 0 To hexString.Length - 1
            c = hexString(i)
            If IsHexDigit(c) Then
                numHexChars += 1
            End If
        Next
        ' if odd number of characters, discard last character
        If numHexChars Mod 2 <> 0 Then
            numHexChars -= 1
        End If
        Return numHexChars / 2
        ' 2 characters per byte
    End Function
    ''' <summary>
    ''' Creates a byte array from the hexadecimal string. Each two characters are combined
    ''' to create one byte. First two hexadecimal characters become first byte in returned array.
    ''' Non-hexadecimal characters are ignored. 
    ''' </summary>
    ''' <param name="hexString">string to convert to byte array</param>
    ''' <param name="discarded">number of characters in string ignored</param>
    ''' <returns>byte array, in the same left-to-right order as the hexString</returns>
    Public Shared Function GetBytes(ByVal hexString As String, ByRef discarded As Integer) As Byte()
        discarded = 0
        Dim newString As String = ""
        Dim c As Char
        ' remove all none A-F, 0-9, characters
        For i As Integer = 0 To hexString.Length - 1
            c = hexString(i)
            If IsHexDigit(c) Then
                newString += c
            Else
                discarded += 1
            End If
        Next
        ' if odd number of characters, discard last character
        If newString.Length Mod 2 <> 0 Then
            discarded += 1
            newString = newString.Substring(0, newString.Length - 1)
        End If

        Dim byteLength As Integer = newString.Length / 2
        Dim bytes As Byte() = New Byte(byteLength - 1) {}
        Dim hex As String
        Dim j As Integer = 0
        For i As Integer = 0 To bytes.Length - 1
            hex = New String(New Char() {newString(j), newString(j + 1)})
            bytes(i) = HexToByte(hex)
            j = j + 2
        Next
        Return bytes
    End Function
    Public Shared Function GetString(ByVal bytes As Byte()) As String
        Dim hexString As String = ""
        For i As Integer = 0 To bytes.Length - 1
            hexString += bytes(i).ToString("X2")
        Next
        Return hexString
    End Function
    ''' <summary>
    ''' Determines if given string is in proper hexadecimal string format
    ''' </summary>
    ''' <param name="hexString"></param>
    ''' <returns></returns>
    Public Shared Function InHexFormat(ByVal hexString As String) As Boolean
        Dim hexFormat As Boolean = True

        For Each digit As Char In hexString
            If Not IsHexDigit(digit) Then
                hexFormat = False
                Exit For
            End If
        Next
        Return hexFormat
    End Function

    ''' <summary>
    ''' Returns true is c is a hexadecimal digit (A-F, a-f, 0-9)
    ''' </summary>
    ''' <param name="c">Character to test</param>
    ''' <returns>true if hex digit, false if not</returns>
    Public Shared Function IsHexDigit(ByVal c As Char) As Boolean
        Dim numChar As Integer
        Dim numA As Integer = Convert.ToInt32("A"c)
        Dim num1 As Integer = Convert.ToInt32("0"c)
        c = [Char].ToUpper(c)
        numChar = Convert.ToInt32(c)
        If numChar >= numA AndAlso numChar < (numA + 6) Then
            Return True
        End If
        If numChar >= num1 AndAlso numChar < (num1 + 10) Then
            Return True
        End If
        Return False
    End Function
    ''' <summary>
    ''' Converts 1 or 2 character string into equivalant byte value
    ''' </summary>
    ''' <param name="hex">1 or 2 character string</param>
    ''' <returns>byte</returns>
    Private Shared Function HexToByte(ByVal hex As String) As Byte
        If hex.Length > 2 OrElse hex.Length <= 0 Then
            Throw New ArgumentException("hex must be 1 or 2 characters in length")
        End If
        Dim newByte As Byte = Byte.Parse(hex, System.Globalization.NumberStyles.HexNumber)
        Return newByte
    End Function
End Class