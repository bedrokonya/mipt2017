// =============================================================
// Дмитрий Сошников: Функциональное программирования на языке F#
//                               http://www.soshnikov.com/fsharp
// -------------------------------------------------------------
// Глава 3: Приёмы функционального программирования - ленивые вычисления
// =============================================================

(* 1. Идеальный код - не пробуйте выполнять *)
open System

let Read s = 
    Console.Write(s:string)
    let inp = Console.ReadLine()
    Double.Parse(inp)

let Print x = printf "%A" x

let Solve =
   let a = Read "Введите a"
   let b = Read "Ввадите b"
   let c = Read "Введите c"
   let d = b*b-4.*a*c
   Print ((-b+sqrt(d))/2./a,(-b-sqrt(d))/2./a)

(* 2. Энергичный код. Ввод с клавиатуры эмулируется числом 1.0 *)
open System 

let Read s = 
    Console.Write(s:string)
    1.0

let Print x = printf "%A" x

let Solve =
   let a = Read "Введите a"
   let b = Read "Ввадите b"
   let c = Read "Введите c"
   let d = b*b-4.*a*c
   Print ((-b+sqrt(d))/2./a,(-b-sqrt(d))/2./a)

(* 3. Ленивый код. Ввод эмулируется *)
let Read s = 
    Console.Write(s:string)
    1.0

let Print x = printf "%A" x

let Solve =
   let a = lazy(Read "Введите a")
   let b = lazy(Read "Ввадите b")
   let c = lazy(Read "Введите c")
   let d = lazy(b.Force()*b.Force()-4.*a.Force()*c.Force())
   Print ((-b.Force()+sqrt(d.Force()))/2./a.Force(),(-b.Force()-sqrt(d.Force()))/2./a.Force())

(* 4. Ещё пример с ленивыми вычислениями и вводом *)
// Псевдо-функция ввода - выводит приглашение, потом возвращает введенный пользователем результат (в нашем случае - всегда 1)
let Read s = 
    System.Console.Write(s:string)
    1

let sqr x = x*x // обычное возведение в квадрат
let lsqr (x: Lazy<int>) = lazy(x.Force()*x.Force()) // ленивое возведение в квадрат

lsqr(lazy (Read "Enter number:"))
lsqr(lazy (Read "Enter number:")).Force()


(* Ленивый факториал *)
let rec lazy_fact n = 
 let _ = printfn "Apply %d" n in
 match n with
   1 -> lazy 1
 | n -> lazy (n*Lazy.force (lazy_fact(n-1)));;
 
 (* Другой способ записи ленивого факториала *)
 let rec lazy_fact n = 
 lazy(
 let _ = printfn "Apply %d" n in
 match n with
   1 -> 1
 | n -> n*lazy_fact(n-1).Force());;


// Ленивые последовательности
// --------------------------
// Аналог ленивых последовательностей seq в F# можно получить с использованием явного механизма ленивости Lazy/Force

#r "FSharp.Powerpack.dll";;

open Lazy;;
open Microsoft.FSharp.Control;;

type 'a SeqCell = Nil | Cons of 'a * 'a Stream
and  'a Stream = Lazy<'a SeqCell>;;

let sample1 = lazy(Cons(1,lazy(Cons(2,lazy(Nil)))));;
let sample2 = lazy(Cons(3,lazy(Cons(4,lazy(Nil)))));;

let rec concat (s:'a Stream) (t: 'a Stream) = 
    match s.Force() with
      Nil -> t
    | Cons(x,y) -> lazy(Cons(x,(concat y t)));;

concat sample1 sample2;;
