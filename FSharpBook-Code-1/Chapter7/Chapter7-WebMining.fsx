// =============================================================
// Дмитрий Сошников: Функциональное программирования на языке F#
//                               http://www.soshnikov.com/fsharp
// -------------------------------------------------------------
// Глава 7: Решение типовых задач
// =============================================================

(* Чтение RSS-потока из Интернет *)

#r "System.Xml.Linq.dll"
open System.Xml
open System.Xml.Linq

let xdoc = XDocument.Load(@"http://blogs.msdn.com/b/sos/rss.aspx")

let xn s = XName.op_Implicit s

let titles = seq{ for t in xdoc.Descendants(xn "item") -> t.Element(xn "title").Value }
titles |> Seq.toList

(* Использование Bing API для сравнения популярности *)

open System.Net
open System 

let SearchCountAsync s =
    let AppID = "0000000000000000000000000000000000000000" // сюда поместите свой App ID
    let url = sprintf "http://api.search.live.net/xml.aspx?Appid=%s&sources=web&query=%s" AppID s
    async {
        let req = WebRequest.Create url
        use! resp = Async.FromBeginEnd(req.BeginGetResponse,req.EndGetResponse)
        use stream = resp.GetResponseStream()
        let xdoc = XDocument.Load(stream)
        let webns = System.Xml.Linq.XNamespace.op_Implicit "http://schemas.microsoft.com/LiveSearch/2008/04/XML/web"
        let sx = xdoc.Descendants(webns.GetName("Total"))
        let cnt = Seq.head(sx).Value
        return Int32.Parse(cnt)
    }

let Compare L =
     L |> List.map SearchCountAsync |> Async.Parallel |> Async.RunSynchronously

(* Чтение и обработка текста из интернет *)

open System.Net
open System.IO
let http (url:string) =
   let rq = WebRequest.Create(url)
   use res = rq.GetResponse()
   use rd = new StreamReader(res.GetResponseStream())
   rd.ReadToEnd()

http "http://www.yandex.ru"

open System.Text.RegularExpressions
let links txt = 
  let mtch = Regex.Matches(txt,"href=\s*\"[^\"h]*(http://[^&\"]*)\"")
  [ for x in mtch -> x.Groups.[1].Value ]

http "http://www.yandex.ru" |> links

(* Простейший Web Crawler *)

open System.Collections.Generic
let internet = new Dictionary<string,string>()
let queue = new Queue<string>()

let rec crawl n =
   if n>0 then
       let url = queue.Dequeue()
       if not (internet.ContainsKey(url)) then
          printf "%d. Processing %s..." n url
          let page = try http url with _ -> printfn "Error"; ""
          if page<>"" then
              internet.Add(url,page)
              let linx = page |> links 
              linx |> Seq.iter(fun l -> queue.Enqueue(l))
              printf "%d bytes, %d links..." page.Length (Seq.length linx)
              printfn "Done"
       crawl (n-1)

let engine url n =
   queue.Clear()
   queue.Enqueue(url)
   crawl n

engine "http://www.yandex.ru" 100

(* Асинхронный web crawler *)
open System.Collections.Generic
open System.Net
open System.IO

let process' (url:string) =
   async {
     let! html =
      async {
         try
             let req = WebRequest.Create(url)
             use! resp = req.AsyncGetResponse()
             use reader = new StreamReader(resp.GetResponseStream())
             return reader.ReadToEnd()
         with _ -> return ""
         }
     return links html
     }

let crawler = 
  MailboxProcessor.Start(fun inbox ->
    let rec loop n (inet:Set<string>) =
       async {
         if n>0 then
           let! url = inbox.Receive()
           if not (Set.contains url inet) then
             printfn "Processing %d -> %s" n url
             do Async.Start(
                        async { 
                          let! links = process' url
                          for l in links do inbox.Post(l)
                          })
             printfn "Done %d" n
           return! loop (n-1) (Set.add url inet)}
    loop 100 Set.empty)

crawler.Post "http://www.yandex.ru"

(* Параллельный web crawler *)
let MailboxDispatcher n f =
    MailboxProcessor.Start(fun inbox ->
        let queue = Array.init n (fun i -> MailboxProcessor.Start(f))
        let rec loop i = async {
            let! msg = inbox.Receive()
            queue.[i].Post(msg)
            return! loop((i+1)%n)
        }
        loop 0
    )

let crawler = 
  MailboxDispatcher 8 (fun inbox ->
    let rec loop n (inet:Set<string>) =
       async {
         if n>0 then
           let! url = inbox.Receive()
           if not (Set.contains url inet) then
             printfn "Processing %d -> %s" n url
             do Async.Start(
                        async { 
                          let! links = process' url
                          for l in links do crawler.Post(l)
                          })
             printfn "Done %d" n
           return! loop (n-1) (Set.add url inet)}
    loop 10 Set.empty)

crawler.Post "http://www.yandex.ru"
