// =============================================================
// Дмитрий Сошников: Функциональное программирования на языке F#
//                               http://www.soshnikov.com/fsharp
// -------------------------------------------------------------
// Глава 2: Рекурсивные структуры данных - Массивы
// =============================================================

// Описание .NET-массивов (Array)
[| 1;2;3 |]

// К ним применимы все те же функции, что и к спискам
// А также функции из модуля Seq
Array.map ((*)2) [|1;2;3|]
Seq.map ((*)2) [|1;2;3|]

// Доступ к элементам массива - по номеру с помощью операции .[]
// Суммируем простым обходом в рекурсивном цикле
let sum (a : int []) =
   let rec sumrec i s =
      if i<a.Length then sumrec (i+1) (s+a.[i])
      else s
   sumrec 0 0

sum [|1;2;3|]

// Создание массива программно
let intarray n =
   let a = Array.create n 0
   Array.iteri (fun i _ -> a.[i] <- (i+1)) a
   a

let r = intarray 10

// Операции "вырезания" фрагментов массива
r.[1..5] <- [| 11..15 |]
r.[5..]

// Конструкторы массивов и эквивалентный вызов функции init
let intarray n = Array.init n (fun i -> i+1)
let intarray n = [|1..n|]

// ResizeArray - это синоним List<> в .NET
// Используем для формирования массива переменной длины
let ReadLines() =
  let inp = new ResizeArray<string>()
  let rec recread() =
    let s = Console.ReadLine()
    if s<>"." then
      inp.Add(s)
      recread()
  recread()
  List.ofSeq inp

ReadLines()

// "Неровные" многомерные массивы 
// ------------------------------

// Строим треугольник Паскаля
let pascal n = 
  let rec pas L n =
    let A::t = L in
    if n = 0 then L
    else
      pas (((1::[for i in 1..(List.length A-1) -> A.[i-1]+A.[i]])@[1])::L) (n-1)
  pas [[1;1]] n

pascal 10;;


// Строим треугольник Паскаля - второй вариант
let pascal n = 
  let rec pas L n =
    if n = 0 then L
    else
      let A::t = L in
      pas (((1::[for i in 1..(List.length A-1) -> A.[i-1]+A.[i]])@[1])::L) (n-1)
  pas [[1;1]] n

pascal 10;;

