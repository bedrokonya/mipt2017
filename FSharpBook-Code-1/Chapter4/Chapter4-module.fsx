// =============================================================
// Дмитрий Сошников: Функциональное программирования на языке F#
//                               http://www.soshnikov.com/fsharp
// -------------------------------------------------------------
// Глава 4: Императивное и объектно-ориентированное программир.
// =============================================================
// Модульное программирование на F#

// Описание очереди из книги Chris Okasaki: Purely Functional Data Structures
module Queue =
    type 'a queue = 'a list * 'a list
    let empty = [],[]
    let tail (L,R) = 
      match L with
       [x] -> (List.rev R, [])
      | h::t -> (t,R)
    let head (h::_,_) = h
    let put x (L,R) =
      match L with
       [] -> ([x],R)
      | _ -> (L,x::R)

open Queue

let q = Queue.empty
let q1 = Queue.put 5 (Queue.put 10 q)
Queue.head q1
let q2 = Queue.tail q1
