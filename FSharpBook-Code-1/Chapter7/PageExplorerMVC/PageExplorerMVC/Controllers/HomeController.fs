namespace FSharpMVC2.Web.Controllers

open  FSharpMVC2.Web.Models
open System.Web
open System.Web.Mvc

[<HandleError>]
type HomeController() =
    inherit Controller()
    member x.Index (m:InputModel) : ActionResult =
        x.ViewData.["Message"] <- "Welcome to ASP.NET MVC!"
        x.ViewData.["Urls"] <- WebExplore.explore m.Url
        x.View() :> ActionResult
    member x.About () =
        x.View() :> ActionResult