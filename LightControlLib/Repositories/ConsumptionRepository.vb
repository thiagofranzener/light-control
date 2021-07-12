Imports System.Data
Imports System.Data.SqlClient
Imports System.Text

Public Class ConsumptionRepository
    Inherits Repository

    Private ReadOnly _connection As SqlConnection
    Private ReadOnly _socketRepository As SocketRepository

    Sub New(connection As SqlConnection)
        Me._connection = connection
        Me._socketRepository = New SocketRepository(connection)
    End Sub

    Public Sub Add(consumption As Consumption)
        consumption.DateTime = Date.Now
        consumption.Value = (consumption.KWhAmount * 0.676)
        Dim command As SqlCommand = _connection.CreateCommand()
        command.CommandText = GetAddQuery()
        PopulateQuery(command, consumption)
        command.ExecuteNonQuery()
        consumption.Id = GetLastId()
    End Sub

    Public Sub Update(consumption As Consumption)
        consumption.Value = (consumption.KWhAmount * 0.676)
        Dim command As SqlCommand = _connection.CreateCommand()
        command.CommandText = GetUpdateQuery()
        PopulateQuery(command, consumption)
        command.ExecuteNonQuery()
    End Sub

    Public Function Exist(id As Integer) As Boolean
        Dim command As SqlCommand = _connection.CreateCommand()
        command.CommandText = $"SELECT COUNT(0) FROM Consumo WHERE id={id}"
        If command.ExecuteScalar() > 0 Then Return True
        Return False
    End Function

    Public Function FindByUserId(userId As String) As HashSet(Of Consumption)
        Dim result As New HashSet(Of Consumption)
        Dim sql As New StringBuilder(GetFindQuery())
        sql.AppendFormat("WHERE USU.id='{0}'", userId).AppendLine()
        Dim command = _connection.CreateCommand()
        command.CommandText = sql.ToString()
        Using reader As IDataReader = command.ExecuteReader()
            If reader.Read() Then
                Dim consumption As New Consumption()
                Populate(reader, consumption)
                result.Add(consumption)
            End If
        End Using
        Return result
    End Function

    Public Function FindById(id As Integer) As Consumption
        Dim result As Consumption = Nothing
        Dim sql As New StringBuilder(GetFindQuery())
        sql.AppendLine().AppendFormat("WHERE CSM.id={0}", id)
        Dim command = _connection.CreateCommand()
        command.CommandText = sql.ToString()
        Using reader As IDataReader = command.ExecuteReader()
            If reader.Read() Then
                Dim consumption As New Consumption()
                Populate(reader, consumption)
                result = consumption
            End If
        End Using
        Return result
    End Function

    Private Shared Function GetAddQuery() As String
        Dim sql As New StringBuilder()
        sql.AppendLine("INSERT INTO Consumo")
        sql.AppendLine("(idTomada,dataHora,quantidadekWh,tensao,valor)")
        sql.AppendLine("VALUES").AppendLine()
        sql.AppendLine("(@idTomada,@dataHora,@quantidadekWh,@tensao,@valor)")
        Return sql.ToString()
    End Function

    Private Shared Function GetUpdateQuery() As String
        Dim sql As New StringBuilder()
        sql.AppendLine("UPDATE Consumo")
        sql.AppendLine("SET idTomada=@idTomada")
        sql.AppendLine(",dataHora=@dataHora")
        sql.AppendLine(",quantidadekWh=@quantidadekWh")
        sql.AppendLine(",tensao=@tensao")
        sql.AppendLine(",valor=@valor")
        sql.AppendLine("WHERE id=@id")
        Return sql.ToString()
    End Function

    Private Shared Sub PopulateQuery(command As SqlCommand, consumption As Consumption)
        command.Parameters.AddWithValue("@id", consumption.Id)
        command.Parameters.AddWithValue("@idTomada", consumption.Socket.Id)
        command.Parameters.AddWithValue("@dataHora", consumption.DateTime)
        command.Parameters.AddWithValue("@quantidadekWh", consumption.KWhAmount)
        command.Parameters.AddWithValue("@tensao", consumption.Voltage)
        command.Parameters.AddWithValue("@valor", consumption.Value)
    End Sub

    Private Function GetLastId() As Integer
        Dim command As SqlCommand = _connection.CreateCommand()
        command.CommandText = "SELECT ISNULL(MAX(id),0) FROM Consumo"
        Return command.ExecuteScalar()
    End Function

    Private Shared Function GetFindQuery() As String
        Dim sql As New StringBuilder()
        sql.AppendLine("SELECT")
        sql.AppendLine("CSM.id")
        sql.AppendLine(",CSM.idTomada")
        sql.AppendLine(",CSM.dataHora")
        sql.AppendLine(",CSM.quantidadekWh")
        sql.AppendLine(",CSM.tensao")
        sql.AppendLine(",CSM.valor")
        sql.AppendLine("FROM Consumo CSM")
        sql.AppendLine("JOIN Tomada TOM ON CSM.idTomada=TOM.id")
        sql.AppendLine("JOIN Comodo CMD ON TOM.idComodo=CMD.id")
        sql.AppendLine("JOIN Usuario USU ON CMD.idUsuario=USU.id")
        Return sql.ToString()
    End Function

    Private Sub Populate(reader As IDataReader, consumption As Consumption)
        consumption.Id = reader("id")
        consumption.DateTime = reader("dataHora")
        consumption.KWhAmount = reader("quantidadekWh")
        consumption.Voltage = reader("tensao")
        consumption.Value = reader("valor")
        Dim socketId As Integer = reader("idTomada")
        reader.Close()
        consumption.Socket = _socketRepository.FindById(socketId)
    End Sub

    Public Overrides Sub Dispose()
        _connection.Close()
    End Sub

End Class