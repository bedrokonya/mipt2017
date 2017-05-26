// =============================================================
// Дмитрий Сошников: Функциональное программирования на языке F#
//                               http://www.soshnikov.com/fsharp
// -------------------------------------------------------------
// Глава 1: Основы функционального программирования
// Построение множества Мандельброта на консоли и в окне
// =============================================================

// Нам нужен FSharp Powerpack для определения комплексного типа
// Доступен на http://fsharppowerpack.codeplex.com
#r @"FSharp.PowerPack.dll"

open Microsoft.FSharp.Math;;
open System;

// Рассмотренная нами ранее функция многократной композиции
let rec rpt n f =
    if n=0 then fun x->x
    else f >> rpt (n-1) f

// Базовая функция Мандельброта z -> z^2+c
let mandelf (c:Complex) (z:Complex) = z*z+c

// Принадлежность точки множеству Мандельброта
let ismandel c = Complex.Abs(rpt 20 (mandelf c) (Complex.zero))<1.0;;

// Масштабирование
let scale (x:float,y:float) (u,v) n = float(n-u)/float(v-u)*(y-x)+x;;

(* Строим на консоли *)

for i=1 to 40 do
 for j=1 to 40 do
   let lscale = scale (-1.2,1.2) (1,40) in
   let t = complex (lscale j) (lscale i) in 
   Console.Write(if ismandel t then "*" else " ")
 Console.WriteLine("")


(* Строим в окне Windows Forms *)

open System.Drawing
open System.Windows.Forms

let form =
   let image = new Bitmap(400, 400)
   let lscale = scale (-1.2,1.2) (0,image.Height-1)
   for i = 0 to (image.Height-1) do
     for j = 0 to (image.Width-1) do
       let t = complex (lscale i) (lscale j) in
       image.SetPixel(i,j,if ismandel t then Color.Black else Color.White)
   let temp = new Form()
   temp.Paint.Add(fun e -> e.Graphics.DrawImage(image, 0, 0))
   temp.Show()
   temp

[<STAThread>]
do Application.Run(form);;
