Imports LightControlUtils.Extensions

Namespace Validators

    Public Class Validators

        Public Shared Function ValidateString(value As String) As Boolean
            If value Is Nothing OrElse String.IsNullOrEmpty(value) OrElse String.IsNullOrWhiteSpace(value) Then Return False
            Return True
        End Function

        Public Shared Function ValidateDecimal(value As Decimal) As Boolean
            If Not IsNumber(value) OrElse value <= 0 Then Return False
            Return True
        End Function

        Public Shared Function ValidateInteger(value As Integer) As Boolean
            If value <= 0 Then Return False
            Return True
        End Function

        Public Shared Function ValidateEmail(value As String) As Boolean
            If value Is Nothing OrElse Not value.Contains("@") OrElse Not value.Contains(".") Then Return False
            Return True
        End Function

        Public Shared Function ValidatePhoneNumber(value As String) As Boolean
            If value Is Nothing OrElse (value.Length <> 10 AndAlso value.Length <> 11) Then Return False
            Return True
        End Function

    End Class

End Namespace