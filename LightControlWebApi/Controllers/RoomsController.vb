Imports System.Web.Http
Imports LightControlLib

Namespace Controllers

    <UserAuthorization>
    <RoutePrefix("api/rooms")>
    Public Class RoomsController
        Inherits ApiController

        Private ReadOnly Connection As New ApiConnection()

        Public Sub New()
        End Sub

        <HttpPost>
        Public Function Add(<FromBody()> ByVal room As RoomViewModel) As IHttpActionResult
            Try

                Dim errors = New RoomValidator(Connection.GetConnection(), room).Validate()
                If errors IsNot Nothing Then
                    Connection.Dispose()
                    Return Me.BadRequest(String.Join(vbCrLf, errors))
                End If

                Dim roomRepository As New RoomRepository(Connection.GetConnection())
                Dim roomModel As Room = ViewModelToModel(room)
                roomRepository.Add(roomModel)

                Connection.Dispose()
                Return Me.Ok(roomModel)
            Catch ex As Exception
                Connection.Dispose()
                Return Me.InternalServerError(ex)
            End Try
        End Function

        <HttpGet>
        <Route("{roomId}")>
        Public Function FindById(<FromUri> roomId As Integer) As IHttpActionResult
            If roomId <= 0 Then Return Me.BadRequest("Código do cômodo inválido!")
            Dim roomRepository As New RoomRepository(Connection.GetConnection())
            Dim result As RoomViewModel
            Try
                result = ModelToViewModel(roomRepository.FindById(roomId))
            Catch ex As Exception
                roomRepository.Dispose()
                Return Me.InternalServerError(ex)
            End Try
            roomRepository.Dispose()
            If result Is Nothing Then Return Me.BadRequest("Código do cômodo inválido!")
            Return Me.Ok(result)
        End Function

        <HttpGet>
        Public Function FindAll() As IHttpActionResult
            Dim roomRepository As New RoomRepository(Connection.GetConnection())
            Dim result As New HashSet(Of RoomViewModel)
            Try
                Dim hashRoom As HashSet(Of Room) = roomRepository.FindByUserId(HttpContext.Current.User.Identity.Name)
                For Each room As Room In hashRoom
                    result.Add(ModelToViewModel(room))
                Next
            Catch ex As Exception
                roomRepository.Dispose()
                Return Me.InternalServerError(ex)
            End Try
            roomRepository.Dispose()
            If result Is Nothing Then Return Me.BadRequest("Usuário inválido!")
            Return Me.Ok(result)
        End Function

        Private Function ModelToViewModel(room As Room) As RoomViewModel
            Return New RoomViewModel With {
                .Id = room.Id,
                .UserId = room.User.Id,
                .Description = room.Description
             }
        End Function

        Private Function ViewModelToModel(roomViewModel As RoomViewModel) As Room
            If roomViewModel Is Nothing Then Return Nothing
            Dim userRepository As New UserRepository(Connection.GetConnection())
            Return New Room() With {
                .Id = roomViewModel.Id,
                .User = userRepository.FindById(roomViewModel.UserId),
                .Description = roomViewModel.Description
            }
            Connection.Dispose()
        End Function

    End Class

End Namespace