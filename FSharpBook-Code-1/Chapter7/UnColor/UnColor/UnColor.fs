// =============================================================
// Дмитрий Сошников: Функциональное программирования на языке F#
//                               http://www.soshnikov.com/fsharp
// -------------------------------------------------------------
// Глава 7: Решение типовых задач
// =============================================================

// Преобразование изображения в оттенки серого

open System
open System.Drawing

let uncolor (c:Color) =
    let x = (int)(c.R + c.G + c.B)/3
    Color.FromArgb(x,x,x)

let pixprocess func fin fout =
   let bin = new Bitmap(fin:string)
   let bout = new Bitmap(width=bin.Width,height=bin.Height)
   for i in 0..(bin.Width-1) do
      for j in 0..(bin.Height-1) do
        bout.SetPixel(i,j,bin.GetPixel(i,j)|>func)
   bout.Save fout

[<EntryPoint>]
let main(args: string array) =
   Console.WriteLine("Image Uncolorifier")
   if args.Length<>2 then
     Console.WriteLine("Format: uncolor <input file> <output file>"); 1
   else 
     try
       pixprocess uncolor args.[0] args.[1]; 0
     with e -> Console.WriteLine("Error: " + e.Message); 2

