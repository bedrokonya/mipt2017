namespace WebSharperProject

open IntelliFactory.WebSharper
open IntelliFactory.WebSharper.Html
open IntelliFactory.WebSharper.Formlet

module Application =
    let rec primes = function
              [] -> []
            | h::t -> h::primes(List.filter (fun x -> x%h>0) t)
    [<Rpc>]
    let GetPrimes() = primes [2..100] |> List.fold (fun s i -> s+" "+i.ToString()) ""

[<JavaScriptType>]
type MainPage() = 
    inherit Web.Control()

    [<JavaScript>]
    override this.Body = 
       let result = P [Text ""]
       Div [
            P [Text "Press to retrieve data"]
            Input [Type "Button"; Value "Get Data"] |>! OnClick (fun e a -> result.Text <- Application.GetPrimes())
            result
           ]



