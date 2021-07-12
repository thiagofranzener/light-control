Imports System.Data
Imports System.Data.SqlClient
Imports System.Text

Public Class SocketRepository
    Inherits Repository

    Private ReadOnly _connection As SqlConnection
    Private ReadOnly _roomRepository As RoomRepository

    Sub New(connection As SqlConnection)
        Me._connection = connection
        Me._roomRepository = New RoomRepository(connection)
    End Sub

    Public Sub Add(socket As Socket)
        Dim command As SqlCommand = _connection.CreateCommand()
        command.CommandText = GetAddQuery()
        PopulateQuery(command, socket)
        command.ExecuteNonQuery()
        socket.Id = GetLastId()
    End Sub

    Public Sub Update(socket As Socket)
        Dim sql As New StringBuilder(GetUpdateQuery(socket.Id))
        Dim command As SqlCommand = _connection.CreateCommand()
        command.CommandText = sql.ToString()
        PopulateQuery(command, socket)
        command.ExecuteNonQuery()
    End Sub

    Public Sub Delete(id As Integer)
        Dim command As SqlCommand = _connection.CreateCommand()
        command.CommandText = $"DELETE FROM Tomada WHERE id={id}"
        command.ExecuteNonQuery()
    End Sub

    Public Function Exist(id As Integer) As Boolean
        Dim command As SqlCommand = _connection.CreateCommand()
        command.CommandText = $"SELECT COUNT(0) FROM Tomada WHERE id={id}"
        If command.ExecuteScalar() > 0 Then Return True
        Return False
    End Function

    Public Function FindById(id As Integer) As Socket
        Dim sql As New StringBuilder(GetFindQuery())
        sql.AppendFormat("WHERE TOM.id={0}", id)
        Dim command = _connection.CreateCommand()
        command.CommandText = sql.ToString()
        Using reader As IDataReader = command.ExecuteReader()
            If reader.Read() Then
                Dim socket As New Socket()
                Populate(reader, socket)
                Return socket
            End If
        End Using
        Return Nothing
    End Function

    Public Function FindByRoomId(roomId) As HashSet(Of Socket)
        Dim result As New HashSet(Of Socket)
        Dim sql As New StringBuilder(GetFindQuery())
        sql.AppendFormat("WHERE TOM.idComodo='{0}'", roomId)
        Dim command = _connection.CreateCommand()
        command.CommandText = sql.ToString
        Using reader As IDataReader = command.ExecuteReader()
            If reader.Read() Then
                Dim socket As New Socket()
                Populate(reader, socket)
                result.Add(socket)
            End If
        End Using
        Return result
    End Function

    Public Function FindByUserId(userId) As HashSet(Of Socket)
        Dim result As New HashSet(Of Socket)
        Dim sql As New StringBuilder(GetFindQuery())
        sql.AppendFormat("WHERE CMD.idUsuario='{0}'", userId)
        Dim command = _connection.CreateCommand()
        command.CommandText = sql.ToString
        Using reader As IDataReader = command.ExecuteReader()
            If reader.Read() Then
                Dim socket As New Socket()
                Populate(reader, socket)
                result.Add(socket)
            End If
        End Using
        Return result
    End Function

    Private Shared Sub PopulateQuery(command As SqlCommand, socket As Socket)
        command.Parameters.AddWithValue("@id", socket.Id)
        command.Parameters.AddWithValue("@idComodo", socket.Room.Id)
        command.Parameters.AddWithValue("@descricao", socket.Description)
    End Sub

    Private Sub Populate(reader As IDataReader, socket As Socket)
        socket.Id = reader("id")
        socket.Description = reader("descricao")
        Dim roomId As Integer = reader("idComodo")
        reader.Close()
        socket.Room = _roomRepository.FindById(roomId)
    End Sub

    Private Function GetLastId() As Integer
        Dim command As SqlCommand = _connection.CreateCommand()
        command.CommandText = "SELECT ISNULL(MAX(id),0) FROM Tomada"
        Return command.ExecuteScalar()
    End Function

    Private Shared Function GetAddQuery() As String
        Dim sql As New StringBuilder()
        sql.AppendLine("INSERT INTO Tomada")
        sql.AppendLine("(idComodo,descricao)")
        sql.AppendLine("VALUES")
        sql.AppendLine("(@idComodo,@descricao)")
        Return sql.ToString()
    End Function

    Private Shared Function GetFindQuery() As String
        Dim sql As New StringBuilder()
        sql.AppendLine("SELECT")
        sql.AppendLine("TOM.id")
        sql.AppendLine(",TOM.idComodo")
        sql.AppendLine(",TOM.descricao")
        sql.AppendLine("FROM Tomada TOM")
        sql.AppendLine("JOIN Comodo CMD ON TOM.idComodo=CMD.id")
        sql.AppendLine("JOIN Usuario USU ON CMD.idUsuario=USU.id")
        Return sql.ToString()
    End Function

    Private Shared Function GetUpdateQuery(id As Integer) As String
        Dim sql As New StringBuilder()
        sql.AppendLine("UPDATE Tomada SET")
        sql.AppendLine("idComodo=@idComodo")
        sql.AppendLine(",descricao=@descricao")
        sql.AppendFormat("WHERE id={0}", id)
        Return sql.ToString()
    End Function

    Public Overrides Sub Dispose()
        _connection.Close()
    End Sub

End Class