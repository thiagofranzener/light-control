Imports System.Data
Imports System.Data.SqlClient
Imports System.Text

Public Class RoomRepository
    Inherits Repository

    Private ReadOnly _connection As SqlConnection
    Private ReadOnly _userRepository As UserRepository

    Sub New(connection As SqlConnection)
        Me._connection = connection
        Me._userRepository = New UserRepository(connection)
    End Sub

    Public Sub Add(room As Room)
        Dim command As SqlCommand = _connection.CreateCommand()
        command.CommandText = GetAddQuery()
        PopulateQuery(command, room)
        command.ExecuteNonQuery()
        room.Id = GetLastId()
    End Sub

    Public Sub Update(room As Room)
        Dim command As SqlCommand = _connection.CreateCommand()
        command.CommandText = GetUpdateQuery()
        PopulateQuery(command, room)
        command.ExecuteNonQuery()
    End Sub

    Public Sub Delete(id)
        Dim command = _connection.CreateCommand()
        command.CommandText = GetDeleteQuery(id)
        command.ExecuteNonQuery()
    End Sub

    Public Function Exist(id As Integer) As Boolean
        Dim command As SqlCommand = _connection.CreateCommand()
        command.CommandText = $"SELECT COUNT(0) FROM Comodo WHERE id=@id"
        command.Parameters.AddWithValue("@id", SqlDbType.Int).Value = id
        If command.ExecuteScalar() > 0 Then Return True
        Return False
    End Function

    Public Function FindByUserId(userId As String) As HashSet(Of Room)
        Dim sql As New StringBuilder(GetFindQuery())
        sql.AppendFormat("WHERE USU.id='{0}'", userId)
        Dim command = _connection.CreateCommand()
        command.CommandText = sql.ToString
        Dim result As New HashSet(Of Room)
        Using reader As IDataReader = command.ExecuteReader()
            If reader.Read() Then
                Dim room As New Room
                Populate(reader, room)
                result.Add(room)
            End If
        End Using
        Return result
    End Function

    Public Function FindById(id As Integer) As Room
        Dim sql As New StringBuilder(GetFindQuery())
        sql.AppendFormat("WHERE CMD.id={0}", id)
        Dim command = _connection.CreateCommand()
        command.CommandText = sql.ToString()
        Using reader As IDataReader = command.ExecuteReader()
            If reader.Read() Then
                Dim room As New Room()
                Populate(reader, room)
                Return room
            End If
        End Using
        Return Nothing
    End Function

    Private Sub Populate(reader As IDataReader, room As Room)
        room.Id = reader("id")
        room.Description = reader("descricao")
        Dim userId As String = reader("idUsuario")
        reader.Close()
        room.User = _userRepository.FindById(userId)
    End Sub

    Private Function GetLastId() As Integer
        Dim command As SqlCommand = _connection.CreateCommand()
        command.CommandText = "SELECT ISNULL(MAX(id),0) FROM Comodo"
        Return command.ExecuteScalar()
    End Function

    Private Shared Function GetAddQuery() As String
        Dim sql As New StringBuilder()
        sql.AppendLine("INSERT INTO Comodo")
        sql.AppendLine("(idUsuario,descricao)")
        sql.AppendLine("VALUES")
        sql.AppendLine("(@idUsuario,@descricao)")
        Return sql.ToString()
    End Function

    Private Shared Function GetUpdateQuery() As String
        Dim sql As New StringBuilder()
        sql.AppendLine("UPDATE Comodo SET")
        sql.AppendLine("descricao=@descricao")
        sql.AppendLine("WHERE id=@id")
        Return sql.ToString()
    End Function

    Private Shared Function GetDeleteQuery(id As Integer) As String
        Dim sql As New StringBuilder()
        sql.AppendLine("DELETE FROM Usuario")
        sql.AppendFormat("WHERE id={0}", id).AppendLine()
        Return sql.ToString()
    End Function

    Private Shared Function GetFindQuery() As String
        Dim sql As New StringBuilder()
        sql.AppendLine("SELECT")
        sql.AppendLine("CMD.id")
        sql.AppendLine(",CMD.idUsuario")
        sql.AppendLine(",CMD.descricao")
        sql.AppendLine("FROM Comodo CMD")
        sql.AppendLine("JOIN Usuario USU ON CMD.idUsuario=USU.id")
        Return sql.ToString()
    End Function

    Private Shared Sub PopulateQuery(command As SqlCommand, room As Room)
        command.Parameters.AddWithValue("@id", room.Id)
        command.Parameters.AddWithValue("@idUsuario", room.User.Id)
        command.Parameters.AddWithValue("@descricao", room.Description)
    End Sub

    Public Overrides Sub Dispose()
        _connection.Close()
    End Sub

End Class
