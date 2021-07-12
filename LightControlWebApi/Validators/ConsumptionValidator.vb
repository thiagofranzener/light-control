Imports System.Data.SqlClient
Imports LightControlLib
Imports LightControlUtils.Extensions
Imports LightControlUtils.Validators

Public Class ConsumptionValidator

    Private ReadOnly _consumptionViewModel As ConsumptionViewModel
    Private ReadOnly _socketRepository As SocketRepository

    Sub New(connection As SqlConnection,
            consumptionViewModel As ConsumptionViewModel)

        Me._consumptionViewModel = consumptionViewModel
        Me._socketRepository = New SocketRepository(connection)
    End Sub

    Public Function Validate() As List(Of String)
        Dim errors As New List(Of String)
        If ObjectSerializationIsInvalid(Me._consumptionViewModel, errors) Then Return errors
        ValidateFields(errors)
        If errors.Count > 0 Then Return errors
        Return Nothing
    End Function

    Private Sub ValidateFields(errors As List(Of String))
        If Not Validators.ValidateInteger(Me._consumptionViewModel.SocketId) Then errors.Add("Tomada inválida.")
        If Not Validators.ValidateDecimal(Me._consumptionViewModel.KWhAmount) Then errors.Add("Quantidade de kWh inválida.")
        If Not Validators.ValidateInteger(Me._consumptionViewModel.Voltage) Then errors.Add("Tensão inválida.")
        If Not Me._socketRepository.Exist(Me._consumptionViewModel.SocketId) Then errors.Add("Tomada não existe.")
    End Sub

End Class