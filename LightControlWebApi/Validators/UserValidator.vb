Imports System.Data.SqlClient
Imports LightControlLib
Imports LightControlUtils.Extensions
Imports LightControlUtils.Validators

Public Class UserValidator

    Private ReadOnly _userViewModel As UserViewModel
    Private ReadOnly _userRepository As UserRepository

    Sub New(connection As SqlConnection,
            userViewModel As UserViewModel)

        Me._userViewModel = userViewModel
        Me._userRepository = New UserRepository(connection)
    End Sub

    Public Function Validate() As List(Of String)
        Dim errors As New List(Of String)
        If ObjectSerializationIsInvalid(Me._userViewModel, errors) Then Return errors
        ValidateFields(errors)
        If errors.Count > 0 Then Return errors
        Return Nothing
    End Function

    Private Sub ValidateFields(errors As List(Of String))
        If Not Validators.ValidateString(Me._userViewModel.Id) Then errors.Add("Usuário inválido.")
        If Not Validators.ValidateString(Me._userViewModel.Name) Then errors.Add("Nome inválido.")
        If Not Validators.ValidateEmail(Me._userViewModel.Email) Then errors.Add("E-mail inválido.")
        If Not Validators.ValidateString(Me._userViewModel.Password) Then errors.Add("Senha inválida.")
        If Not Validators.ValidatePhoneNumber(Me._userViewModel.PhoneNumber) Then errors.Add("Telefone/Celular inválido.")
        If Me._userViewModel.Id.Length > 100 Then errors.Add("Usuário muito longo.")
        If Me._userViewModel.Name.Length > 100 Then errors.Add("Nome muito longo.")
        If Me._userViewModel.Email.Length > 100 Then errors.Add("E-mail muito longo.")
        If Me._userRepository.Exist(Me._userViewModel.Id) Then errors.Add("Usuário já existe.")
    End Sub

End Class