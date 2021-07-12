Imports System.Web.Http
Imports LightControlLib

Namespace Controllers

    <RoutePrefix("api/expense-category")>
    Public Class ExpenseCategoryController
        Inherits ApiController

        Private ReadOnly Connection As New ApiConnection()

        <UserAuthenthicator123>
        <Route("~/api/company/{companyId:int}/expense-category")>
        <HttpPost>
        Public Function Add(<FromUri> companyId As Integer, <FromBody()> ByVal expenseCategory As ExpenseCategoryViewModel) As IHttpActionResult
            Dim errors = New ExpenseCategoryValidator(expenseCategory).Validate()
            If errors IsNot Nothing Then
                Connection.Dispose()
                Return Me.BadRequest(String.Join(", ", errors))
            End If

            Dim companyRepository As New SocketRepository(Connection.GetConnection())
            If Not companyRepository.Exist(companyId) Then
                Connection.Dispose()
                Return Me.BadRequest("Empresa inexistente")
            End If

            Dim userCompany As Company = companyRepository.FindByUsername(HttpContext.Current.User.Identity.Name)
            If userCompany Is Nothing OrElse userCompany.Id <> companyId Then
                Connection.Dispose()
                Return Me.BadRequest($"Empresa inválida para o usuário {HttpContext.Current.User.Identity.Name}")
            End If


            Try
                Dim expenseCategoryRepository As New RoomRepository(Connection.GetConnection())
                Dim nextId As Integer = expenseCategoryRepository.Add(companyId, expenseCategory.Description)
                expenseCategoryRepository.Dispose()
                Return Me.Ok(nextId)
            Catch ex As Exception
                Connection.Dispose()
                Return Me.InternalServerError(ex)
            End Try
        End Function


        <UserAuthenthicator123>
        <Route("~/api/company/{companyId:int}/expense-category")>
        <HttpGet>
        Public Function Find(<FromUri> companyId As Integer) As IHttpActionResult
            Try
                Dim companyRepository As New SocketRepository(Connection.GetConnection())
                If Not companyRepository.Exist(companyId) Then
                    Connection.Dispose()
                    Return Me.BadRequest("Empresa inexistente")
                End If

                Dim userCompany As Company = companyRepository.FindByUsername(HttpContext.Current.User.Identity.Name)
                If userCompany Is Nothing OrElse userCompany.Id <> companyId Then
                    Connection.Dispose()
                    Return Me.BadRequest($"Empresa inválida para o usuário {HttpContext.Current.User.Identity.Name}")
                End If
                Dim expenseCategoryRepository As New RoomRepository(Connection.GetConnection())
                Dim result As HashSet(Of ExpenseCategory) = expenseCategoryRepository.GetAll(companyId)
                Connection.Dispose()
                Return Me.Ok(result)
            Catch ex As Exception
                Connection.Dispose()
                Return Me.InternalServerError(ex)
            End Try

        End Function

        <UserAuthenthicator123>
        <HttpPut>
        <Route("~/api/company/{companyId:int}/expense-category/{expenseCategoryId:int}")>
        Public Function Update(<FromUri> companyId As Integer, <FromUri> expenseCategoryId As Integer, <FromBody> expenseCategory As ExpenseCategoryViewModel) As IHttpActionResult
            Dim errors = New ExpenseCategoryValidator(expenseCategory).Validate()
            If errors IsNot Nothing Then
                Connection.Dispose()
                Return Me.BadRequest(String.Join(", ", errors))
            End If

            Dim companyRepository As New SocketRepository(Connection.GetConnection())
            If Not companyRepository.Exist(companyId) Then
                Connection.Dispose()
                Return Me.BadRequest("Empresa inexistente")
            End If

            Dim userCompany As Company = companyRepository.FindByUsername(HttpContext.Current.User.Identity.Name)
            If userCompany Is Nothing OrElse userCompany.Id <> companyId Then
                Connection.Dispose()
                Return Me.BadRequest($"Empresa inválido para o usuário {HttpContext.Current.User.Identity.Name}")
            End If


            Dim expenseCategoryRepository As New RoomRepository(Connection.GetConnection())
            If Not expenseCategoryRepository.Exist(companyId, expenseCategoryId) Then
                Connection.Dispose()
                Return Me.BadRequest("Categoria inexistente")
            End If

            Try
                expenseCategoryRepository.Update(CreateModel(New ExpenseCategoryViewModel() With {
                .CompanyId = companyId,
                .CategoryId = expenseCategoryId,
                .Description = expenseCategory.Description
                }))
            Catch ex As Exception
                expenseCategoryRepository.Dispose()
                Return Me.InternalServerError(ex)
            End Try
            expenseCategoryRepository.Dispose()
            Return Me.StatusCode(204)
        End Function

        <UserAuthenthicator123>
        <HttpDelete>
        <Route("~/api/company/{companyId:int}/expense-category/{expenseCategoryId:int}")>
        Public Function Delete(<FromUri> companyId As Integer, <FromUri> expenseCategoryId As Integer) As IHttpActionResult
            Dim companyRepository As New SocketRepository(Connection.GetConnection())
            If Not companyRepository.Exist(companyId) Then
                Connection.Dispose()
                Return Me.BadRequest("Empresa inexistente")
            End If

            Dim userCompany As Company = companyRepository.FindByUsername(HttpContext.Current.User.Identity.Name)
            If userCompany Is Nothing OrElse userCompany.Id <> companyId Then
                Connection.Dispose()
                Return Me.BadRequest($"Empresa inválido para o usuário {HttpContext.Current.User.Identity.Name}")
            End If

            Dim expenseCategoryRepository As New RoomRepository(Connection.GetConnection())
            If Not expenseCategoryRepository.Exist(companyId, expenseCategoryId) Then
                Connection.Dispose()
                Return Me.BadRequest("Categoria inexistente")
            End If

            Try
                expenseCategoryRepository.Delete(companyId, expenseCategoryId)
            Catch ex As Exception
                expenseCategoryRepository.Dispose()
                Return Me.InternalServerError(ex)
            End Try
            expenseCategoryRepository.Dispose()
            Return Me.Ok()
        End Function


        <UserAuthenthicator123>
        <HttpGet>
        <Route("~/api/company/{companyId:int}/expense-category/{expenseCategoryId:int}")>
        Public Function FindById(<FromUri> companyId As Integer, <FromUri> expenseCategoryId As Integer) As IHttpActionResult
            Dim companyRepository As New SocketRepository(Connection.GetConnection())
            If Not companyRepository.Exist(companyId) Then
                Connection.Dispose()
                Return Me.BadRequest("Empresa inexistente")
            End If

            Dim userCompany As Company = companyRepository.FindByUsername(HttpContext.Current.User.Identity.Name)
            If userCompany Is Nothing OrElse userCompany.Id <> companyId Then
                Connection.Dispose()
                Return Me.BadRequest($"Empresa inválido para o usuário {HttpContext.Current.User.Identity.Name}")
            End If

            Dim expenseCategoryRepository As New RoomRepository(Connection.GetConnection())
            If Not expenseCategoryRepository.Exist(companyId, expenseCategoryId) Then
                Connection.Dispose()
                Return Me.BadRequest("Categoria inexistente")
            End If

            Try
                Dim result = expenseCategoryRepository.GetById(companyId, expenseCategoryId)
                expenseCategoryRepository.Dispose()
                Return Me.Ok(result)
            Catch ex As Exception
                expenseCategoryRepository.Dispose()
                Return Me.InternalServerError(ex)
            End Try
        End Function

        Private Function CreateModel(expenseCategory As ExpenseCategoryViewModel) As ExpenseCategory
            Return New ExpenseCategory With {
                .CompanyId = expenseCategory.CompanyId,
                .CategoryId = expenseCategory.CategoryId,
                .Description = expenseCategory.Description
             }
        End Function

    End Class

End Namespace