// =============================================================
// Дмитрий Сошников: Функциональное программирования на языке F#
//                               http://www.soshnikov.com/fsharp
// -------------------------------------------------------------
// Глава 7: Решение типовых задач
// =============================================================

// Нахождение популярности слова в интернет
// при помощи Bing API
// для телефона Windows Phone 7

namespace Shwarsico

open System
open System.Net
open System.Xml.Linq
open WindowsPhonePanoramaApp.Utilities

type SearchResultEventArgs(n : int) =
   inherit EventArgs()
   member x.Count with get()=n

type BingSearcher (s:string) =
  let AppID = "put your bing AppID here"
  let url = sprintf "http://api.search.live.net/xml.aspx?Appid=%s&sources=web&query=%s" AppID s
  let resultEvent = new Event<SearchResultEventArgs>()
  let AsyncResult () = 
    async {
      let req = WebRequest.Create url
      use! resp = Async.FromBeginEnd(req.BeginGetResponse,req.EndGetResponse)
      use stream = resp.GetResponseStream()
      let xdoc = XDocument.Load(stream)
      let xn s = XName.op_Implicit s
      let webns = System.Xml.Linq.XNamespace.op_Implicit "http://schemas.microsoft.com/LiveSearch/2008/04/XML/web"
      let sx = xdoc.Descendants(webns.GetName("Total"))
      let cnt = Seq.head(sx).Value
      return Int32.Parse(cnt)
    }
  
  member x.Pull() =
     let res = AsyncResult()
     let wrk = new AsyncWorker<_>(res)
     wrk.JobCompleted.Add(fun args ->
        resultEvent.Trigger(new SearchResultEventArgs(args.Result)))
     wrk.Start()

  [<CLIEvent>]
  member x.ResultAvailable = resultEvent.Publish
