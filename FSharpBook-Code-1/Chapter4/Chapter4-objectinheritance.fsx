// =============================================================
// Дмитрий Сошников: Функциональное программирования на языке F#
//                               http://www.soshnikov.com/fsharp
// -------------------------------------------------------------
// Глава 4: Императивное и объектно-ориентированное программир.
// =============================================================
// Построение классической объектной иерархии с наследованием

open System

type Shape = interface
   abstract Draw : unit -> unit
   abstract Area : float
end

type Point (cx,cy) = 
   let mutable x = cx
   let mutable y = cy
   new() = new Point(0.0,0.0)
   abstract MoveTo : Point -> unit
   default p.MoveTo(dest) = p.Coords <- dest.Coords
   member p.Coords with get() = (x,y) and set(v) = let (x1,y1) = v in x <- x1; y <- y1
   interface Shape with
     override t.Draw() = printfn "Point %A" t.Coords
     override t.Area = 0.0
   static member Zero = new Point()

let p = new Point()
p.Coords
(p :> Shape).Area
let draw (x: #Shape) = x.Draw()
draw p

// Подробный (старый) синтаксис описания класса
type Circle (cx,cy,cr) = 
 class
   inherit Point(cx,cy)
   let mutable r = cr
   new () = new Circle(0.0,0.0,0.0)
   member p.Radius with get()=r and set(v)=r<-v
   interface Shape with
     override t.Draw() = printfn "Circle %A, r=%f" base.Coords r
     override t.Area = Math.PI*r*r/2.0
 end

// Краткий (новый) синтаксис описания класса
type Square (cx,cy,sz) =
   inherit Point(cx,cy)
   let mutable size = sz
   new() = new Square(0.0,0.0,1.0)
   member p.Size with get()=size and set(v)=size<-v
   interface Shape with
     override t.Draw() = printfn "Square %A, sz=%f" base.Coords size
     override t.Area = size*size

// Создаем список точек с помощью явного приведения типов   
let plist = [new Point(); new Square():>Point; new Circle(1.0,1.0,5.0):>Point]

plist |> List.iter (fun p -> (p:>Shape).Draw())
plist |> List.map (fun x -> (x.Coords,(x:>Shape).Area))
plist |> List.iter (fun p -> p.MoveTo(Point.Zero))
plist |> List.iter (fun p -> (p:>Shape).Draw())

// Динамическая проверка того, поддерживает ли объект заданный интерфейс
let area (p:Object) =
  if (p :? Shape) then (p:?>Shape).Area
  else failwith "Not a shape"

let area (p:Object) =
   match p with
    | :? Shape as s -> s.Area
    | _ -> failwith "Not a shape" 
