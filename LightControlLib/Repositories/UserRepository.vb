Imports System.Data
Imports System.Data.SqlClient
Imports System.Text
Imports LightControlUtils.Extensions

Public Class UserRepository
    Inherits Repository

    Private ReadOnly connection As SqlConnection
    Sub New(connection As SqlConnection)
        Me.connection = connection
    End Sub

    Public Sub Add(user As User)
        Dim command As SqlCommand = connection.CreateCommand()
        command.CommandText = GetAddQuery()
        PopulateQuery(command, user)
        command.ExecuteNonQuery()
    End Sub

    Public Sub Update(user As User)
        Dim sql As New StringBuilder(GetUpdateQuery(user.Id))
        Dim command As SqlCommand = connection.CreateCommand()
        command.CommandText = sql.ToString()
        PopulateQuery(command, user)
        command.ExecuteNonQuery()
    End Sub

    Public Sub Delete(id As String)
        Dim command As SqlCommand = connection.CreateCommand()
        command.CommandText = $"DELETE FROM User WHERE id='{id}'"
        command.ExecuteNonQuery()
    End Sub

    Public Function Exist(id As String) As Boolean
        Dim command As SqlCommand = connection.CreateCommand()
        command.CommandText = $"SELECT COUNT(0) FROM Usuario WHERE id=@id"
        command.Parameters.AddWithValue("@id", SqlDbType.VarChar).Value = id
        If command.ExecuteScalar() > 0 Then Return True
        Return False
    End Function

    Public Function FindById(id As String) As User
        Dim sql As New StringBuilder(GetFindQuery())
        sql.AppendLine($"WHERE USU.id='{id}'")
        Dim command As SqlCommand = connection.CreateCommand()
        command.CommandText = sql.ToString()
        Using reader As IDataReader = command.ExecuteReader()
            If reader.Read() Then
                Dim user As New User()
                PopulateUser(user, reader)
                Return user
            End If
        End Using
        Return Nothing
    End Function

    Private Shared Function GetAddQuery() As String
        Dim sql As New StringBuilder()
        sql.AppendLine("INSERT INTO Usuario")
        sql.AppendLine("(id,nome,email,telefone,senha)")
        sql.AppendLine("VALUES")
        sql.AppendLine("(@id,@nome,@email,@telefone,@senha)")
        Return sql.ToString()
    End Function

    Private Shared Function GetUpdateQuery(id As Integer) As String
        Dim sql As New StringBuilder()
        sql.AppendLine("UPDATE Usuario SET")
        sql.AppendLine("nome=@nome")
        sql.AppendLine(",email=@email")
        sql.AppendLine(",telefone=@telefone")
        sql.AppendLine(",senha=@senha")
        sql.AppendFormat("WHERE id='{0}'", id)
        Return sql.ToString()
    End Function

    Private Shared Function GetFindQuery() As String
        Dim sql As New StringBuilder()
        sql.AppendLine("SELECT")
        sql.AppendLine("USU.id")
        sql.AppendLine(",USU.nome")
        sql.AppendLine(",USU.email")
        sql.AppendLine(",USU.telefone")
        sql.AppendLine(",USU.senha")
        sql.AppendLine("FROM Usuario USU")
        Return sql.ToString()
    End Function

    Private Shared Sub PopulateUser(user As User, reader As IDataReader)
        user.Id = reader("id")
        user.Name = reader("nome")
        user.Email = reader("email")
        user.PhoneNumber = reader("telefone")
        user.Password = reader("senha")
    End Sub

    Private Shared Sub PopulateQuery(command As SqlCommand, user As User)
        command.Parameters.AddWithValue("@id", user.Id)
        command.Parameters.AddWithValue("@nome", user.Name)
        command.Parameters.AddWithValue("@email", user.Email)
        command.Parameters.AddWithValue("@telefone", user.PhoneNumber)
        command.Parameters.AddWithValue("@senha", EncryptString(user.Password))
    End Sub

    Public Overrides Sub Dispose()
        connection.Close()
    End Sub

End Class
