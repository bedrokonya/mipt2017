// =============================================================
// Дмитрий Сошников: Функциональное программирования на языке F#
//                               http://www.soshnikov.com/fsharp
// -------------------------------------------------------------
// Глава 6: Параллельные и асинхронные вычисления
// =============================================================

// Параллельная программа для вычисления числа Пи

open System
open MPI

// Вычисляющие функции

let rand max n = 
    seq {
        let r = new System.Random(n)
        while true do yield r.NextDouble()*max
    }

// Основная программа

let args = Environment.GetCommandLineArgs()
let env = new Environment(ref args)
let W = Communicator.world
let N = 10000
Console.WriteLine("Running on node {0} out of {1}",W.Rank+1,W.Size)
let n = Seq.zip (rand 1.0 (W.Rank*7+1)) (rand 1.0 (W.Rank*3+2))
         |> Seq.take N
         |> Seq.filter (fun (x,y) -> x*x+y*y<1.0)
         |> Seq.length
Console.WriteLine("Computed # is {0}",n)
if W.Rank=0 then
   let res = W.Reduce<int>(n,Operation<int>.Add,0)
   Console.WriteLine("Total # is {0}",res)
   let pi = 4.0*float(res)/float(N)/float(W.Size)
   Console.WriteLine("Pi={0}",pi)
else
   W.Reduce<int>(n,Operation<int>.Add,0) |> ignore
env.Dispose()

