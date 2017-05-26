// =============================================================
// Дмитрий Сошников: Функциональное программирования на языке F#
//                               http://www.soshnikov.com/fsharp
// -------------------------------------------------------------
// Глава 1: Основы функционального программирования
// =============================================================

// Построение простейших L-систем

open System
open System.Drawing
open System.Drawing.Imaging
open System.Drawing.Drawing2D

// Визуализация команд черепашей графики в графический файл
let TurtleBitmapVisualizer n delta cmd =
    let W,H = 1600,1600
    let b = new Bitmap(W,H)
    let g = Graphics.FromImage(b)
    let pen = new Pen(Color.Black)
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
     | _ -> (x,y,phi)     
    let rec draw x y phi = function
     | [] -> ()
     | h::t ->
         let (nx,ny,nphi) = ProcessCommand x y phi h
         draw nx ny nphi t
    draw (float(W)/2.0) (float(H)/2.0) 0. cmd
    b

// Реализация процессора грамматики 
type Rule = char * char list
type Grammar = Rule list

let FindSubst c (gr:Grammar) = 
   match List.tryFind (fun (x,S) -> x=c) gr with
     | Some(x,S) -> S
     | None -> [c]

let Apply (gr:Grammar) L =
   List.collect (fun c -> FindSubst c gr) L

let rec NApply n gr L = 
   if n>0 then Apply gr (NApply (n-1) gr L)
   else L

let str (s:string) = s.ToCharArray() |> List.ofArray

let gr = [('F',str "F+FF-FF-F-F+F+FF-F-F+F+FF+FF-F")]
let lsys = NApply 3 gr (str "F-F-F-F")

let B = TurtleBitmapVisualizer 2.0 (Math.PI/2.0) lsys
B.Save(@"c:\pictures\bitmap.jpg")
