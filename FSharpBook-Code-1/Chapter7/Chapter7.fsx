// =============================================================
// Дмитрий Сошников: Функциональное программирования на языке F#
//                               http://www.soshnikov.com/fsharp
// -------------------------------------------------------------
// Глава 7: Решение типовых задач
// =============================================================

(* Вычисления с произвольной точностью *)

let rec fact = function
    1 -> 1
  | n -> n*fact (n-1)

fact 17

#r "FSharp.Powerpack"

open Microsoft.FSharp.Math
open System.Numerics


let rec fact = function
    1 -> 1I
  | n -> BigInteger(n)*fact (n-1)

let n = 1N/10N

let nth n x = BigNum.PowN(x,n) / BigNum.FromBigInt(fact n)
let exp x = 1N+Seq.fold(fun s n -> s+(nth n x)) 0N [1..50];;

BigNum.ToDouble(exp 1N);;
System.Math.Exp(1.0);;

(* Units of Measure *)

#r "FSharp.Powerpack"

open Microsoft.FSharp.Math

[<Measure>]
type m

[<Measure>]
type s

let g = 9.8<m/s^2>

sin(float g)

let double x = x*2.0<_>

let rec go (vx,vy) (x,y) (time:float<s>) =
  printf "Time: %A Pos: (%A,%A), Speed: (%A,%A)\n" time x y vx vy
  if y >= 0.0<m> then
    let h = 0.1<s>
    let dx = h*vx
    let dy = h*vy
    let dspy = -h*g
    go (vx,vy+dspy) (x+dx,y+dy) (time+h)

go (3.0<m/s>,3.0<m/s>) (0.0<m>,0.0<m>) 0.0<s>

(* Использование Math.NET.Numerics *)

#r "c:\\winapp\\Math.NET.Numerics\MathNet.Numerics.dll"
// #r "c:\\winapp\\Math.NET.Numerics\MathNet.Numerics.FSharp.dll"

open MathNet.Numerics.Distributions
let d = new Normal()
d.Variance <- 0.5
let seq = d.Samples()

open MathNet.Numerics.Statistics
let H = new Histogram(Seq.take 1000 seq,10)
for i in 0..H.BucketCount-1 do 
  let bk = H.[i]
  for j in 0 .. int(bk.Count/10.0) do printf "*"
  printfn ""


(* Работа с XML с помощью классического XmlDocument API *)

let xfile = @"blog.xml"

open System.Xml
let xdoc = new XmlDocument()
xdoc.Load(xfile)

let titles = seq{ for t in xdoc.SelectNodes("//item/title") -> t.InnerText }
titles |> Seq.toList                         

type BlogRecord = { title : string; desc : string; categories : string[] }
let records = 
  let node (t:XmlNode) x = t.SelectSingleNode(x).InnerText
  seq{
    for t in xdoc.SelectNodes("//item") -> 
       { title=node t "title"; desc = node t "description" ; 
         categories = (node t "category").Split(';') }}

records |> Seq.toList

records |> Seq.collect (fun x -> x.categories) | FreqDict

(* Работа с XML с помощью XLinq *)
 
#r "System.Xml.Linq.dll"
open System.Xml
open System.Xml.Linq

let xfile = @"blog.xml"

let xdoc = XDocument.Load(xfile)

let xn s = XName.op_Implicit s

let titles = seq{ for t in xdoc.Descendants(xn "item") -> t.Element(xn "title").Value }
titles |> Seq.toList

type BlogRecord = { title : string; desc : string; categories : string[] }
let records =
  let xv (t:#XElement) n = t.Element(xn n).Value
  seq{ for t in xdoc.Descendants(xn "item") -> 
         { title=xv t "title"; desc=xv t "description"; categories=(xv t "category").Split(';') }}

records |> Seq.map (fun x -> x.title) |> Seq.toList

let xd = new XDocument(
          new XElement(xn "root",
           seq { for x in 1..3 ->
                  new XElement(xn "item", x.ToString()) 

let html = new XDocument(
            new XElement(xn "html",
             new XElement(xn "body",
              new XElement(xn "h1", "Blog Content"),
              new XElement(xn "ul",
               seq { for x in xdoc.Descendants(xn "item") ->
                       new XElement(xn "li",x.Element(xn "title").Value) }))))

(* Взаимодействие с Excel *)

#r "FSharp.Powerpack"
#r "Microsoft.Office.Interop.Excel"
#r "Office"

open System
open Microsoft.Office.Interop.Excel

let app = new ApplicationClass(Visible = true) // установите Visible=false, чтобы скрыть работающее приложение
app.Workbooks.Open(@"c:\temp\Chapter7.xlsx")

let cell x y =
  let range = sprintf "%c%d" (char (x + int 'A')) (y+1)
  // printf "%A\n" range
  (app.ActiveSheet :?> _Worksheet).Range(range)

let get x y = (cell x y).Value(System.Reflection.Missing.Value)
let set x y z = (cell x y).Value(System.Reflection.Missing.Value) <- z

let col x = 
   Seq.unfold
    (fun i -> 
       let v = get x i
       if v=null then None
       else Some(v,i+1)) 0

col 0

let fcol n =
  (Seq.skip 1 (col n)) |> Seq.map (fun o -> o :?> float)

Seq.zip (fcol 0) (fcol 1)

(* Интерполяция методом наименьших квадратов *)

let interpolate (s: (float*float) seq) =
   let (sx,sy) = Seq.fold (fun (sx,sy) (x,y) -> (sx+x,sy+y)) (0.0,0.0) s
   let (sx2,sy2) = Seq.fold (fun (sx,sy) (x,y) -> (sx+x*x,sy+y*y)) (0.0,0.0) s
   let sxy = Seq.fold (fun s (x,y) -> s+x*y) 0.0 s
   let n = Seq.length s
   let b = (sy*sx2-sxy*sx)/(float(n)*sx2-sx*sx)
   let a = (sy-float(n)*b)/sx
   fun x -> a*x+b

let coords x = Seq.unfold
                      (fun i ->
                         let u = get x i
                         let v = get (x+1) i
                         if u=null then None
                         else Some((u:?>float,if v=null then None else Some(v:?>float)),i+1)) 20

coords 0 |> Seq.toArray
coords 0 |> Seq.filter (fun (u,v) -> Option.isSome(v)) |> Seq.toArray

open MathNet.Numerics.Interpolation
let interpolate (s: (float*float) seq) =
   let xs = Seq.map fst s |> Seq.toArray
   let ys = Seq.map snd s |> Seq.toArray
   let i = MathNet.Numerics.Interpolation.Interpolate.Common(xs,ys)
   fun x -> i.Interpolate(x)

let f = coords 0 |> Seq.filter (fun (u,v) -> Option.isSome(v)) 
         |> Seq.map (fun (u,Some(v))->(u,v)) |> interpolate

coords 0 |> Seq.iteri (fun i (x,y) -> set 2 (20+i) (f x))

app.Quit()
