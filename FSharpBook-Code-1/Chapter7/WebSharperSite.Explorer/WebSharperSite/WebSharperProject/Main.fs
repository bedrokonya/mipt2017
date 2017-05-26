namespace WebSharperProject

open IntelliFactory.WebSharper
open IntelliFactory.WebSharper.Html
open IntelliFactory.WebSharper.Formlet

module WebExplore =
    open System.Net
    open System.IO
    let http (url:string) =
       try
           let rq = WebRequest.Create(url)
           use res = rq.GetResponse()
           use rd = new StreamReader(res.GetResponseStream())
           rd.ReadToEnd()
       with _ -> ""
    open System.Text.RegularExpressions
    let links txt = 
      let mtch = Regex.Matches(txt,"href=\s*\"[^\"h]*(http://[^&\"]*)\"")
      [ for x in mtch -> x.Groups.[1].Value ]
    [<Rpc>]
    let explore url n = http url |> links |> Seq.take n |> Seq.toList


[<JavaScriptType>]
type MainPage() = 
    inherit Web.Control()

    [<JavaScript>]
    override this.Body =
        let res = Div []
        Div [
            Formlet.Yield(fun s n -> s,n|>int)
                <*> (Controls.Input "http://www.yandex.ru" 
                    |> Validator.IsNotEmpty "Must be entered" 
                    |> Enhance.WithTextLabel "Enter URL:")
                <*> (Controls.Input "100" 
                    |> Validator.IsInt "Must be int" 
                    |> Enhance.WithTextLabel "Enter max number:")
                |> Enhance.WithValidationIcon                                 
                |> Enhance.WithSubmitAndResetButtons
                |> Enhance.WithFormContainer
                |> Formlet.Run(fun (s,n) -> 
                                res.Append(UL[ 
                                             for i in (WebExplore.explore s n) -> LI[Text i]
                                             ]))
            res ]

