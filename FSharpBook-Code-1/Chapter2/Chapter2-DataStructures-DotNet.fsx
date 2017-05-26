// =============================================================
// Дмитрий Сошников: Функциональное программирования на языке F#
//                               http://www.soshnikov.com/fsharp
// -------------------------------------------------------------
// Глава 2: Рекурсивные структуры данных - Структуры данных .NET
// =============================================================

// Множества (Set)
// ---------------

let s1 = set [1;2;5;6]
let s2 = set [4;5;7;9]

s1+s2,s1-s2,Set.intersect s1 s2 // объединение, разность, пересечение

// Строим множество символов, встречающихся в строке
let letters (s:string) = s.ToCharArray() |> Array.fold (fun s c -> s+set[c]) Set.empty

letters "A quick brown fox jumped over the lazy dog"

// Отображения (Map)
// -----------------

//  Строим частотный словарь букв в строке
let letters (s:string) = 
   s.ToCharArray() 
   |> Array.fold (fun mp c -> 
                   if Map.containsKey c mp then Map.add c (mp.[c]+1) mp else Map.add c 1 mp) 
       Map.empty

letters "A quick brown fox jumped over the lazy sleeping dog" |> Map.toList


// Частотный словарь с помощью HashMultiMap
#r "FSharp.PowerPack" 
let letters (s:string) = 
   let ht = new HashMultiMap<char,int>(HashIdentity.Structural)
   s.ToCharArray() 
   |> Array.iter (fun c -> 
                   if ht.ContainsKey c then ht.[c] <- ht.[c]+1 else ht.[c] <- 1)
   ht

// Частотный словарь с помошью стандартного .NET-класса Dictionary
open System.Collections.Generic
let letters (s:string) = 
   let ht = new Dictionary<char,int>()
   s.ToCharArray() 
   |> Array.iter (fun c -> 
                   if ht.ContainsKey c then ht.[c] <- ht.[c]+1 
                   else ht.[c] <- 1)
   ht

letters "A quick brown fox jumped over the lazy sleeping dog"

