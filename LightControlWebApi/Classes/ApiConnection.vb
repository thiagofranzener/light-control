Public Class ApiConnection
    Implements System.IDisposable

#Region "Variáveis"
    Private ReadOnly connection As SqlClient.SqlConnection = New SqlClient.SqlConnection("Server=localhost;Database=LightControl;User Id=thiago;Password=1234;Connection Timeout=30")
#End Region

    Sub New()
        connection.Open()
    End Sub

    Function GetConnection() As SqlClient.SqlConnection
        Return connection
    End Function

    Public Function GetCommand() As SqlClient.SqlCommand
        Dim command As SqlClient.SqlCommand = connection.CreateCommand()
        command.Connection = connection
        Return command
    End Function

    Public Sub Dispose() Implements IDisposable.Dispose
        connection.Close()
        connection.Dispose()
    End Sub

End Class
