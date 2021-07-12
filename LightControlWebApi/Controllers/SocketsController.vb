Imports System.Web.Http
Imports LightControlLib

Namespace Controllers

    <UserAuthorization>
    <RoutePrefix("api/sockets")>
    Public Class SocketsController
        Inherits ApiController

        Private ReadOnly Connection As New ApiConnection()

        Public Sub New()
        End Sub

        <HttpPost>
        Public Function Add(<FromBody()> ByVal socket As SocketViewModel) As IHttpActionResult
            Try

                Dim errors = New SocketValidator(Connection.GetConnection(), socket).Validate()
                If errors IsNot Nothing Then
                    Connection.Dispose()
                    Return Me.BadRequest(String.Join(vbCrLf, errors))
                End If

                Dim socketRepository As New SocketRepository(Connection.GetConnection())
                Dim socketModel As Socket = ViewModelToModel(socket)
                socketRepository.Add(socketModel)

                Connection.Dispose()
                Return Me.Ok(socketModel)
            Catch ex As Exception
                Connection.Dispose()
                Return Me.InternalServerError(ex)
            End Try
        End Function

        <HttpGet>
        <Route("{socketId}")>
        Public Function FindById(<FromUri> socketId As Integer) As IHttpActionResult
            If socketId <= 0 Then Return Me.BadRequest("Código da tomada inválida!")
            Dim socketRepository As New SocketRepository(Connection.GetConnection())
            Dim result As SocketViewModel
            Try
                result = ModelToViewModel(socketRepository.FindById(socketId))
            Catch ex As Exception
                socketRepository.Dispose()
                Return Me.InternalServerError(ex)
            End Try
            socketRepository.Dispose()
            If result Is Nothing Then Return Me.BadRequest("Código da tomada inválida!")
            Return Me.Ok(result)
        End Function

        <HttpGet>
        Public Function FindAll() As IHttpActionResult
            Dim socketRepository As New SocketRepository(Connection.GetConnection())
            Dim result As New HashSet(Of SocketViewModel)
            Try
                Dim hashsocket As HashSet(Of Socket) = socketRepository.FindByUserId(HttpContext.Current.User.Identity.Name)
                For Each socket As Socket In hashsocket
                    result.Add(ModelToViewModel(socket))
                Next
            Catch ex As Exception
                socketRepository.Dispose()
                Return Me.InternalServerError(ex)
            End Try
            socketRepository.Dispose()
            If result Is Nothing Then Return Me.BadRequest("Usuário inválido!")
            Return Me.Ok(result)
        End Function

        Private Function ModelToViewModel(socket As Socket) As SocketViewModel
            Return New SocketViewModel With {
                .Id = socket.Id,
                .RoomId = socket.Room.Id,
                .Description = socket.Description
             }
        End Function

        Private Function ViewModelToModel(socketViewModel As SocketViewModel) As Socket
            If socketViewModel Is Nothing Then Return Nothing
            Dim roomRepository As New RoomRepository(Connection.GetConnection())
            Return New Socket() With {
                .Id = socketViewModel.Id,
                .Room = roomRepository.FindById(socketViewModel.RoomId),
                .Description = socketViewModel.Description
            }
            Connection.Dispose()
        End Function

    End Class

End Namespace