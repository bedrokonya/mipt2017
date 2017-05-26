// =============================================================
// Дмитрий Сошников: Функциональное программирования на языке F#
//                               http://www.soshnikov.com/fsharp
// -------------------------------------------------------------
// Глава 7: Решение типовых задач
// =============================================================

module WebExplore
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
      [| for x in mtch -> x.Groups.[1].Value |]

    let explore url = http url |> links 
