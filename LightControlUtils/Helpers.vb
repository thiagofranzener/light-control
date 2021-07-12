
Imports System.Runtime.CompilerServices

Namespace Extensions

    Public Module Extensions

        <Extension()>
        Function ValNumber(value As String) As Double
            Dim result As Double
            Try
                result = Double.Parse(value)
            Catch ex As Exception
                Return 0
            End Try
            Return result
        End Function

        Function ValString(value As String) As Decimal
            If String.IsNullOrWhiteSpace(value) Then Return ""
            Return value
        End Function

        Function IsNumber(value As String) As Boolean
            Try
                Integer.Parse(value)
            Catch ex As Exception
                Return False
            End Try
            Return True
        End Function

        Function ObjectSerializationIsInvalid(value As Object, errors As List(Of String)) As Boolean
            If value Is Nothing Then
                errors.Add("Erro ao serializar objeto")
                Return True
            End If
            Return False
        End Function

        Function IsDateValid([date] As Date) As Boolean
            If [date] = Nothing Then Return False
            If [date] < New Date(1900, 1, 1) Then Return False
            If [date] > New Date(2400, 1, 1) Then Return False
            Return True
        End Function

        Function EncryptString(value As String) As String
            Dim result As String = ""
            Dim cryptoService As New Security.Cryptography.MD5CryptoServiceProvider
            Dim bytesToHash() As Byte = Text.Encoding.ASCII.GetBytes(value)
            bytesToHash = cryptoService.ComputeHash(bytesToHash)
            For Each streamByte As Byte In bytesToHash
                result += streamByte.ToString("x2")
            Next
            Return result.ToUpper()
        End Function

    End Module

End Namespace