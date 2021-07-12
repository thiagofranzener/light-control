Imports System.Web.Http
Imports LightControlLib

Namespace Controllers

    <UserAuthorization>
    <RoutePrefix("api/consumptions")>
    Public Class ConsumptionsController
        Inherits ApiController

        Private ReadOnly Connection As New ApiConnection()

        Public Sub New()
        End Sub

        <HttpPost>
        Public Function Add(<FromBody()> ByVal consumption As ConsumptionViewModel) As IHttpActionResult
            Try

                Dim errors = New ConsumptionValidator(Connection.GetConnection(), consumption).Validate()
                If errors IsNot Nothing Then
                    Connection.Dispose()
                    Return Me.BadRequest(String.Join(vbCrLf, errors))
                End If

                Dim consumptionRepository As New ConsumptionRepository(Connection.GetConnection())
                Dim consumptionModel As Consumption = ViewModelToModel(consumption)
                consumptionRepository.Add(consumptionModel)

                Connection.Dispose()
                Return Me.Ok(consumptionModel)
            Catch ex As Exception
                Connection.Dispose()
                Return Me.InternalServerError(ex)
            End Try
        End Function

        <HttpGet>
        <Route("{consumptionId}")>
        Public Function FindById(<FromUri> consumptionId As Integer) As IHttpActionResult
            If consumptionId <= 0 Then Return Me.BadRequest("Código da tomada inválida!")
            Dim consumptionRepository As New ConsumptionRepository(Connection.GetConnection())
            Dim result As ConsumptionViewModel
            Try
                result = ModelToViewModel(consumptionRepository.FindById(consumptionId))
            Catch ex As Exception
                consumptionRepository.Dispose()
                Return Me.InternalServerError(ex)
            End Try
            consumptionRepository.Dispose()
            If result Is Nothing Then Return Me.BadRequest("Código da tomada inválida!")
            Return Me.Ok(result)
        End Function

        <HttpGet>
        Public Function FindAll() As IHttpActionResult
            Dim consumptionRepository As New ConsumptionRepository(Connection.GetConnection())
            Dim result As New HashSet(Of ConsumptionViewModel)
            Try
                Dim hashconsumption As HashSet(Of Consumption) = consumptionRepository.FindByUserId(HttpContext.Current.User.Identity.Name)
                For Each consumption As Consumption In hashconsumption
                    result.Add(ModelToViewModel(consumption))
                Next
            Catch ex As Exception
                consumptionRepository.Dispose()
                Return Me.InternalServerError(ex)
            End Try
            consumptionRepository.Dispose()
            If result Is Nothing Then Return Me.BadRequest("Usuário inválido!")
            Return Me.Ok(result)
        End Function

        Private Function ModelToViewModel(consumption As Consumption) As ConsumptionViewModel
            Return New ConsumptionViewModel With {
                .Id = consumption.Id,
                .SocketId = consumption.Socket.Id,
                .DateTime = consumption.DateTime,
                .KWhAmount = consumption.KWhAmount,
                .Voltage = consumption.Voltage,
                .Value = consumption.Value
             }
        End Function

        Private Function ViewModelToModel(consumptionViewModel As ConsumptionViewModel) As Consumption
            If consumptionViewModel Is Nothing Then Return Nothing
            Dim socketRepository As New SocketRepository(Connection.GetConnection())
            Return New Consumption() With {
                .Id = consumptionViewModel.Id,
                .Socket = socketRepository.FindById(consumptionViewModel.SocketId),
                .DateTime = consumptionViewModel.DateTime,
                .KWhAmount = consumptionViewModel.KWhAmount,
                .Voltage = consumptionViewModel.Voltage,
                .Value = consumptionViewModel.Value
            }
            Connection.Dispose()
        End Function

    End Class

End Namespace