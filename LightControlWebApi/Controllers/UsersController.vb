Imports System.Web.Http
Imports LightControlLib

Namespace Controllers

    <RoutePrefix("api/users")>
    Public Class UsersController
        Inherits ApiController

        Private ReadOnly Connection As New ApiConnection()

        Public Sub New()
        End Sub

        <HttpPost>
        Public Function Add(<FromBody()> ByVal user As UserViewModel) As IHttpActionResult
            Try

                Dim errors = New UserValidator(Connection.GetConnection(), user).Validate()
                If errors IsNot Nothing Then
                    Connection.Dispose()
                    Return Me.BadRequest(String.Join(vbCrLf, errors))
                End If

                Dim userRepository As New UserRepository(Connection.GetConnection())
                Dim userModel As User = ViewModelToModel(user)
                userRepository.Add(userModel)

                Connection.Dispose()
                Return Me.Ok(userModel)
            Catch ex As Exception
                Connection.Dispose()
                Return Me.InternalServerError(ex)
            End Try
        End Function

        <UserAuthorization>
        <HttpGet>
        Public Function FindUser() As IHttpActionResult
            Dim userRepository As New UserRepository(Connection.GetConnection())
            Dim result As UserViewModel = Nothing
            Try
                result = ModelToViewModel(userRepository.FindById(HttpContext.Current.User.Identity.Name))
            Catch ex As Exception
                userRepository.Dispose()
                Return Me.InternalServerError(ex)
            End Try
            userRepository.Dispose()
            If result Is Nothing Then Return Me.BadRequest("Usuário ou senha incorretos!")
            Return Me.Ok(result)
        End Function

        Private Function ModelToViewModel(user As User) As UserViewModel
            Return New UserViewModel With {
                .Id = user.Id,
                .Name = user.Name,
                .Email = user.Email,
                .PhoneNumber = user.PhoneNumber,
                .Password = user.Password
             }
        End Function

        Private Function ViewModelToModel(userViewModel As UserViewModel) As User
            If userViewModel Is Nothing Then Return Nothing
            Return New User() With {
                .Id = userViewModel.Id,
                .Name = userViewModel.Name,
                .Email = userViewModel.Email,
                .PhoneNumber = userViewModel.PhoneNumber,
                .Password = userViewModel.Password
            }
        End Function

    End Class

End Namespace