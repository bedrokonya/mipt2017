// =============================================================
// Дмитрий Сошников: Функциональное программирования на языке F#
//                               http://www.soshnikov.com/fsharp
// -------------------------------------------------------------
// Глава 1: Основы функционального программирования
// =============================================================

// Построение L-систем со стеком откатов

open System
open System.Drawing
open System.Collections.Generic

type TurtleState = float * float * float

let TurtleBitmapVisualizer n delta cmd =
    let W,H = 1600,1600
    let b = new Bitmap(W,H)
    let g = Graphics.FromImage(b)
    let pen = new Pen(Color.Black)
    let stk = new Stack<TurtleState>()
    let NewCoord (x:float) (y:float) phi =
       let nx = x+n*cos(phi)
       let ny = y+n*sin(phi)
       (nx,ny,phi)
    let ProcessCommand x y phi = function
     | 'f' -> NewCoord x y phi
     | '+' -> (x,y,phi+delta)
     | '-' -> (x,y,phi-delta)
     | 'F' -> 
         let (nx,ny,phi) = NewCoord x y phi
         g.DrawLine(pen,(float32)x,(float32)y,(float32)nx,(float32)ny)
         // printfn "Drawing (%A,%A) -> (%A,%A) [phi=%A]" x y nx ny phi
         (nx,ny,phi)
     | '[' -> stk.Push((x,y,phi)); (x,y,phi)
     | ']' -> stk.Pop()
     | _ -> (x,y,phi)     
    let rec draw x y phi = function
     | [] -> ()
     | h::t ->
         let (nx,ny,nphi) = ProcessCommand x y phi h
         draw nx ny nphi t
    draw (float(W)/2.0) (float(H)/2.0) (-System.Math.PI/2.) cmd
    b

type Rule = char * char list
type Grammar = Rule list

let FindSubst c (gr:Grammar) = 
   match List.tryFind (fun (x,S) -> x=c) gr with
     | Some(x,S) -> S
     | None -> [c]

let Apply gr= List.collect (fun c -> FindSubst c gr)

let rec rpt n f = if n=0 then (fun x->x) else f>>rpt (n-1) f

let NApply n gr = rpt n (Apply gr)

let str (s:string) = s.ToCharArray() |> List.ofArray

let gr = [('F',str "F+FF-FF-F-F+F+FF-F-F+F+FF+FF-F")]
let lsys = NApply 3 gr (str "F-F-F-F")

let B = TurtleBitmapVisualizer 2.0 (Math.PI/2.0) lsys
B.Save(@"c:\pictures\bitmap.jpg")

let gr = [('F',str "F[+F]F[-F][F]")]
let lsys = NApply 5 gr (['F'])
let B = TurtleBitmapVisualizer 10.0 (Math.PI/180.0*20.0) lsys
B.Save(@"c:\pictures\bitmap.jpg")

let gr = [('F',str "FF-[-F+F+F]+[+F-F-F]")]
let lsys = NApply 4 gr (['F'])
let B = TurtleBitmapVisualizer 10.0 (Math.PI/180.0*22.5) lsys
B.Save(@"c:\pictures\bitmap.jpg")

let tostr L = List.fold (fun s (c:char) -> s+c.ToString()) "" L

(* Снежинка Коха *)
let gr = [('F',str "F-F++F-F")]
let lsys = NApply 2 gr (str "F++F++F++")
lsys |> tostr
let B = TurtleBitmapVisualizer 40.0 (Math.PI/180.0*60.0) lsys
B.Save(@"c:\pictures\bitmap.jpg")

(* Построение серии кривых с разничным числом замен *)
let rec DrawSeries name n factor len phi gr str =
   if n>0 then
     printfn "Drawing for n=%d" n
     let B = TurtleBitmapVisualizer len phi str
     B.Save(String.Format("{0}-{1}.jpg",name,n))
     DrawSeries name (n-1) factor (len/factor) phi gr (Apply gr str)

(* Разные кривые Коха*)
DrawSeries @"c:\pictures\Coch" 4 3.0 300.0 (Math.PI/180.0*60.0) [('F',str "F-F++F-F")] (str "F++F++F++")

DrawSeries @"c:\pictures\Square" 3 6.0 400.0 (Math.PI/2.) [('F',str "F+FF-FF-F-F+F+FF-F-F+F+FF+FF-F")] (str "F-F-F-F")
DrawSeries @"c:\pictures\SquareA" 3 6.0 400.0 (Math.PI/2.) [('F',str "F+f-FF+F+FF+Ff+FF-f+FF-F-FF-Ff-FFF");('f',str "ffffff")] (str "F+F+F+F")
DrawSeries @"c:\pictures\Tree" 6 2.0 200.0 (Math.PI/180.0*22.5) [('F',str "FF-[-F+F+F]+[+F-F-F]")] ['F']
DrawSeries @"c:\pictures\TreeA" 8 2.0 200.0 (Math.PI/180.0*22.5) [('X',str "F-[[X]+X]+F[+FX]-X");('F',str "FF")] ['X']
