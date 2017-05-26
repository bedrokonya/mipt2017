namespace WebSharperProject

open IntelliFactory.WebSharper
open IntelliFactory.WebSharper.Html
open IntelliFactory.WebSharper.Formlet

module Application =
    let rec primes = function
              [] -> []
            | h::t -> h::primes(List.filter (fun x -> x%h>0) t)
    [<Rpc>]
    let GetPrimes(n) = primes [2..n] |> List.fold (fun s i -> s+" "+i.ToString()) ""

[<JavaScriptType>]
type MainPage() = 
    inherit Web.Control()

    [<JavaScript>]
    override this.Body =
                        let res = P [Text ""]
                        Div [
                            formlet {
                               let! max = Controls.Input "100" |> Validator.IsInt "Must be int"
                               return max |> int
                            } 
                            |> Enhance.WithTextLabel "Enter max number:"
                            |> Enhance.WithValidationIcon
                            |> Enhance.WithSubmitAndResetButtons
                            |> Enhance.WithFormContainer
                            |> Formlet.Run(fun n -> res.Text <- Application.GetPrimes(n))
                            res ]

