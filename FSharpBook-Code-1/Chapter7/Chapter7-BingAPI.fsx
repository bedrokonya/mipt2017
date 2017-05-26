// =============================================================
// Дмитрий Сошников: Функциональное программирования на языке F#
//                               http://www.soshnikov.com/fsharp
// -------------------------------------------------------------
// Глава 7: Решение типовых задач
// =============================================================
// Использование Bing API

#r "System.Xml"
#r "System.Xml.Linq"

open System.Xml
open System.Net
open System.Xml.Linq

let AppID = "put your Bing App ID here"

let url = sprintf "http://api.search.live.net/xml.aspx?Appid=%s&sources=web&query=%s" AppID "Hello"
let req = WebRequest.Create url
let resp = req.GetResponse()
let stream = resp.GetResponseStream()
let xdoc = XDocument.Load(stream)
let xn s = XName.op_Implicit s
let webns = System.Xml.Linq.XNamespace.op_Implicit "http://schemas.microsoft.com/LiveSearch/2008/04/XML/web"
let sx = xdoc.Descendants(webns.GetName("Total"))
let cnt = Seq.head(sx).Value
cnt

printfn "Result=%s" (search "Hello")
