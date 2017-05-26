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
let size = W.Size
let n = Seq.zip (rand 1.0 (W.Rank*7+1)) (rand 1.0 (W.Rank*3+2))
         |> Seq.take N
         |> Seq.filter (fun (x,y) -> x*x+y*y<1.0)
         |> Seq.length
if W.Rank=0 then
   let res = [1..(size-1)] 
              |> List.map(fun _ -> W.Receive<int>(Communicator.anySource,Communicator.anyTag))
              |> List.sum
   let pi = 4.0*float(n+res)/float(N)/float(W.Size)
   Console.WriteLine("Pi={0}",pi)
else
   W.Send<int>(n,0,0)
env.Dispose()


