Imports System.Data.SqlClient
Imports LightControlLib
Imports LightControlUtils.Extensions
Imports LightControlUtils.Validators

Public Class SocketValidator

    Private ReadOnly _socketViewModel As SocketViewModel
    Private ReadOnly _roomRepository As RoomRepository

    Sub New(connection As SqlConnection,
            socketViewModel As SocketViewModel)

        Me._socketViewModel = socketViewModel
        Me._roomRepository = New RoomRepository(connection)
    End Sub

    Public Function Validate() As List(Of String)
        Dim errors As New List(Of String)
        If ObjectSerializationIsInvalid(Me._socketViewModel, errors) Then Return errors
        ValidateFields(errors)
        If errors.Count > 0 Then Return errors
        Return Nothing
    End Function

    Private Sub ValidateFields(errors As List(Of String))
        If Not Validators.ValidateInteger(Me._socketViewModel.RoomId) Then errors.Add("Cômodo inválido.")
        If Not Validators.ValidateString(Me._socketViewModel.Description) Then errors.Add("Descrição inválida.")
        If Me._socketViewModel.Description.Length > 100 Then errors.Add("Descrição muito longa.")
        If Not Me._roomRepository.Exist(Me._socketViewModel.RoomId) Then errors.Add("Cômodo não existe.")
    End Sub

End Class