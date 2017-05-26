// =============================================================
// Дмитрий Сошников: Функциональное программирования на языке F#
//                               http://www.soshnikov.com/fsharp
// -------------------------------------------------------------
// Глава 6: Параллельные и асинхронные вычисления
// =============================================================

#r "FSharp.Powerpack.dll"

(* Простейшие конструкции async для параллельных вычислений *)
let t1 = async { return 1+2 }
let t2 = async { return 3+4 }
Async.RunSynchronously(Async.Parallel [t1;t2]);;

(* Параллельный map *)

let map' func items =
     let tasks =
         seq {
             for i in items -> async {
                 return (func i)
             }
          }
     Async.RunSynchronously (Async.Parallel tasks)
;;

let time f =
   let st = new System.Diagnostics.Stopwatch()
   st.Start()
   let _ = f()
   printf "Time: %A\n" st.ElapsedTicks
;;

let rec fib x = if x<1 then 1 else fib(x-1)+fib(x-2);;

time (fun () -> List.map (fun x -> fib(x)) [30..35]);;
time (fun () -> map' (fun x -> fib(x)) [30..35]);;

