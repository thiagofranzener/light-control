Imports System.Data.SqlClient
Imports LightControlLib
Imports LightControlUtils.Extensions
Imports LightControlUtils.Validators

Public Class RoomValidator

    Private ReadOnly _roomViewModel As RoomViewModel
    Private ReadOnly _userRepository As UserRepository

    Sub New(connection As SqlConnection,
            roomViewModel As RoomViewModel)

        Me._roomViewModel = roomViewModel
        Me._userRepository = New UserRepository(connection)
    End Sub

    Public Function Validate() As List(Of String)
        Dim errors As New List(Of String)
        If ObjectSerializationIsInvalid(Me._roomViewModel, errors) Then Return errors
        ValidateFields(errors)
        If errors.Count > 0 Then Return errors
        Return Nothing
    End Function

    Private Sub ValidateFields(errors As List(Of String))
        If Not Validators.ValidateString(Me._roomViewModel.UserId) Then errors.Add("Usuário inválido.")
        If Not Validators.ValidateString(Me._roomViewModel.Description) Then errors.Add("Descrição inválida.")
        If Me._roomViewModel.UserId.Length > 100 Then errors.Add("Usuário muito longo.")
        If Me._roomViewModel.Description.Length > 100 Then errors.Add("Descrição muito longa.")
        If Not Me._userRepository.Exist(Me._roomViewModel.UserId) Then errors.Add("Usuário não existe.")
    End Sub

End Class