Imports System.Web.Http
Imports System.Web.Http.Controllers
Imports System.Net.Http
Imports LightControlLib
Imports LightControlUtils.Extensions
Imports System.Security.Principal

Public NotInheritable Class UserAuthorizationAttribute
    Inherits AuthorizeAttribute

    Public Sub New()
    End Sub

    Protected Overrides Sub HandleUnauthorizedRequest(actionContext As HttpActionContext)
        Dim response As New HttpResponseMessage
        response.StatusCode = Net.HttpStatusCode.Unauthorized
        response.ReasonPhrase = "Sem permissão para acessar."
        actionContext.Response = response
        MyBase.HandleUnauthorizedRequest(actionContext)
    End Sub

    Protected Overrides Function IsAuthorized(actionContext As HttpActionContext) As Boolean

        If Not actionContext.Request.Headers.Contains("userId") OrElse
            Not actionContext.Request.Headers.Contains("password") Then

            Return False
        End If

        Dim userId As String = actionContext.Request.Headers.GetValues("userId").FirstOrDefault()
        Dim password As String = actionContext.Request.Headers.GetValues("password").FirstOrDefault()

        If String.IsNullOrEmpty(userId) Then
            Return False
        End If

        Using connection As New ApiConnection
            Dim userRepository As New UserRepository(connection.GetConnection())
            Dim user As User = userRepository.FindById(userId)
            If user IsNot Nothing AndAlso
                (user.Password = password OrElse user.Password = EncryptString(password)) Then

                Dim identity As New GenericIdentity(user.Id)
                HttpContext.Current.User = New GenericPrincipal(identity, Nothing)
                Return True
            End If
        End Using

        Return False
    End Function
End Class