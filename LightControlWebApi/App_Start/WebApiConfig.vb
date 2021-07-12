Imports System.Web.Http
Imports System.Web.Http.Cors
Imports Newtonsoft.Json.Serialization

Public Module WebApiConfig
    Public Sub Register(ByVal config As HttpConfiguration)
        ' Web API configuration and services
        config.EnableCors(New EnableCorsAttribute("*", "*", "*"))
        config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = New CamelCasePropertyNamesContractResolver()

        ' Web API routes
        config.MapHttpAttributeRoutes()

        config.Routes.MapHttpRoute(
            name:="DefaultApi",
            routeTemplate:="api/{controller}",
            defaults:=Nothing
        )
    End Sub
End Module
