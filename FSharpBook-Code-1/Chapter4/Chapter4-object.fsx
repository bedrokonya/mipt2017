// =============================================================
// Дмитрий Сошников: Функциональное программирования на языке F#
//                               http://www.soshnikov.com/fsharp
// -------------------------------------------------------------
// Глава 4: Императивное и объектно-ориентированное программир.
// =============================================================

// Описание простейших объектов в виде записей
type Point = { x : float; y : float }

let p1 = { x=10.0; y=10.0 }
let p2 = { new Point with x=10.0 and y=0.0 }

// type Circle = { x : float; y : float; r : float }

open System

// Функции над записями. Обратите внимание, вывод типов работает!
let distance a b =
   let sqr x = x*x
   Math.Sqrt(sqr(a.x-b.x)+sqr(a.y-b.y)) 

distance p1 p2

let quadrant p = 
  match p with
     { x=0.0; y=0.0 } -> 0
   |  { x=u; y=v } when u>=0.0 && v>=0.0 -> 1
   |  { x=u; y=v } when u>=0.0 && v<0.0 -> 2
   |  { x=u; y=v } when u<0.0 && v<0.0 -> 3
   |  { x=u; y=v } when u<0.0 && v>=0.0 -> 4


// Аналог объекта-интерфейса - запись с полями-методами
// Мы не используем ОО-синтаксис F#, моделируем все сами
type Shape = { Draw : unit -> unit; Area : unit -> float }

let circle c r =
    let cent,rad = c,r
    { Draw = fun () -> printfn "Circle @(%f,%f), r=%f" cent.x cent.y rad ;
      Area = fun () -> Math.PI*rad*rad/2.0 }

let square c x =
    let cent,len = c,x
    { Draw = fun () -> printfn "Square @(%f,%f), size=%f" cent.x cent.y len;
      Area = fun () -> len*len }

let shapes = [ circle { x=1.0; y=2.0 } 10.0 ; square { x=10.0; y=3.0 } 2.0 ]

shapes |> List.iter (fun shape -> shape.Draw())
shapes |> List.map (fun shape -> shape.Area())

// Синтаксис F# позволяет определять методы для объектов-записей
type Point = { x : float; y : float }
   with
     member P.Draw() = printfn "Point @(%f,%f)" P.x P.y
     static member Zero = { x=0.0; y=0.0 }
     static member Distance (P1,P2) = 
       let sqr x = x*x
       Math.Sqrt(sqr(P1.x-P2.x)+sqr(P1.y-P2.y))
     static member (+) (P1 : Point, P2 : Point) =
       { x=P1.x+P2.x ; y = P1.y+P2.y }
     member P1.Distance(P2) = Point.Distance(P1,P2)
     override P.ToString() = sprintf "Point @(%f,%f)" P.x P.y
   end

printfn "%A" ({ x = 10.0; y=20.0 }.ToString())
{ x = 10.0; y=20.0 }.Distance(Point.Zero)
{ x=1.0; y=2.0 } + { x=2.0;y=1.0 }

// Теперь строим иерархию объектов
// Краткий синтаксис объявления интерфейса
type Shape = 
   abstract Draw : unit -> unit
   abstract Area : float

// Более подробный (старый) синтаксис
type Shape = interface
   abstract Draw : unit -> unit
   abstract Area : float
end

// Конструктор возвращает экзмепляр интерфейса с переопределёнными полями
let circle cent rad =
  { new Shape with
      member x.Draw() = printfn "Circle @(%f,%f), r=%f" cent.x cent.y rad 
      member x.Area = Math.PI*rad*rad/2.0 }

let square cent size =
 { new Shape with
      member x.Draw() = printfn "Square @(%f,%f), size=%f" cent.x cent.y size
      member x.Area = size*size }

let shapes = [ circle { x=1.0; y=2.0 } 10.0 ; square { x=10.0; y=3.0 } 2.0 ]

shapes |> List.iter (fun shape -> shape.Draw())
shapes |> List.map (fun shape -> shape.Area)

// Определение перегруженного метода для существующего класса string
let SoryByLen (x : ResizeArray<string>) =
      x.Sort({ new IComparer<string> with member this.Compare(s1,s2) = s1.Length.CompareTo(s2.Length) })

// Дополнительные Extension Methods к существующему классу int
type System.Int32 with 
   member x.isOdd = x%2=1
   member x.isEven = x%2=0

(12).isEven

// Идея построение двух ортогональных объектных иерархий...

open System.Text

type Drawer = float*float -> unit

let circle draw cent rad =
  { new Shape with
      member x.Draw() = 
        for phi in 0.0..0.1..(2.0*Math.PI) do draw (cent.x+rad*Math.Cos(phi),cent.y+rad*Math.Sin(phi))
      member x.Area = Math.PI*rad*rad/2.0 }

let ConsoleCircle cent rad = 
  circle (fun (x,y) -> ... Console.Write("*") ...) cent rad

let BitmapCircle cent rad =
  circle (fun (x,y) -> ... Bitmap.Setpixel(x,y) ...) cent rad
