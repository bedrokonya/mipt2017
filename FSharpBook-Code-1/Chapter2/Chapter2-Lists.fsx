// =============================================================
// Дмитрий Сошников: Функциональное программирования на языке F#
//                               http://www.soshnikov.com/fsharp
// -------------------------------------------------------------
// Глава 2: Рекурсивные структуры данных - Списки
// =============================================================

open System

// Как можно смоделировать список самому на основе полиморфного типа

type 't sequence = Nil | Cons of 't*'t sequence

// Cons - конструктор списка (Cons(голова,хвост))
// Nil - пустой список

Cons(1,Cons(2,Cons(3,Nil))) // самодельный список типа int sequence

// Функции отделения головы и хвоста списка
let head (Cons(u,v)) = u
let tail (Cons(u,v)) = v

// Функция вычисления длины "самодельного" списка
let rec len l =
  if l = Nil then 0
  else 1+len(tail l)

len (Cons(1,Cons(2,Cons(3,Nil))))

// Далее рассмотрим стандартные списки F#
// В качестве конструктора списка используется ::
// Пустой список обозначается []

// Как можно вычислить длину F#-списка
let rec len l =
  if l = [] then 0
  else 1+len (List.tail l)

len [1;2;3] // самодельная функция len
List.length [1;2;3] // стандартная функция length

// Более традиционная запись списковой функции
// с помощью сопоставления с образцом
let rec len l =
   match l with
      [] -> 0
    | h::t -> 1+len t

let rec len = function
   [] -> 0
 | _::t -> 1+len t


// Пример: вычисление суммы положит.слагаемых
let rec sum_positive l = 
   match l with
      [] -> 0
    | h::t when h>0 -> h+sum_positive t
    | _::t -> sum_positive t

sum_positive [1;-3;0;2]


// Конкатенация списков
List.append [1;2] [3;4]
[1;2]@[3;4]

let rec append l r = 
  match l with
    [] -> r
  | h::t -> h::(append t r)

append [1;2] [3;4]

// Взятие n-го элемента - разные способы записи
List.nth [1;2;3] 1
[1;2;3].Item(1)
[1;2;3].[1]

// Далее рассмотрим три базовые операции над списками:

// 1. Map - отображение
// --------------------

// map f L применяет функцию f к каждому элементу L и возвращает список результатов
let rec map f = function
   [] -> []
 | h::t -> (f h)::(map f t)

map (fun x -> x*2) [1;2;3]
map ((*)2) [1;2;3]
[ for x in [1;2;3] -> x*2 ] // другой синтаксис map в виде конструктора списка

// Функция http считывает содержимое веб-страницы из интернет
// нам сейчас не важно, как она описывается
open System.Net
open System.IO
let http (url:string) =
   let rq = WebRequest.Create(url)
   use res = rq.GetResponse()
   use rd = new StreamReader(res.GetResponseStream())
   rd.ReadToEnd()

["http://www.bing.com";"http://www.yandex.ru"] |> List.map http

// Функция mapi - такая же, как map, но в кач.первого аргумента передается номер
["Говорить";"Читать";"Писать"] |> List.mapi (fun i x -> (i+1).ToString()+". "+x)

// Переводим число из с-мы счисления с основанием b в десятичную
let conv_to_dec b l = 
   List.rev l |> 
   List.mapi (fun i x -> x*int(float(b)**float(i))) |> 
   List.sum
conv_to_dec 2 [1;0;0;0]

// Второй способ
let conv_to_dec b l =
   [ for i = (List.length l)-1 downto 0 do yield int(float(b)**float(i)) ] |>
   List.map2 (*) l |>
   List.sum

conv_to_dec 2 [1;0;0;0]

// map2 обрабатывает параллельно два списка (элементы - попарно)
List.map2 (fun u v -> u+v) [1;2;3] [4;5;6]
List.map2 (+) [1;2;3] [4;5;6]

// Подход, называемый map-concat. Применение функции f даёт список, и нам
// нужно в результате получить конкатенацию этих списков
// В нашем случае для нескольких сайтов мы считываем странички about и contacts, и
// получаем единый список результатов
[ "http://site1.com"; "http://site2.com"; "http://site3.com" ] |>
  List.map (fun url -> [ http (url+"/about.html"); http (url+"/contact.html")])
  |> List.concat

// Тоже самое, только одним вызовом List.collect
[ "http://site1.com"; "http://site2.com"; "http://site3.com" ] |>
  List.collect (fun url -> [ http (url+"/about.html"); http (url+"/contact.html")])

// 2. Filter - фильтрация
// ----------------------

// filter f L оставляет только те элементы, для которых f истинно
let rec filter f = function
    [] -> []
  | h::t when (f h) -> h::(filter f t)
  | _::t -> filter f t 

[1..10] |> filter (fun x -> x%2=0)

["http://www.bing.com";"http://www.yandex.ru"] |> 
   List.map http |> List.filter (fun s -> s.IndexOf("bing")>0)

// Синтаксис фильтрации в конструкторах списков
let lst = [1;2;3;4;2]
[ for x in lst do if x%2=0 then yield x]

// Пример: вычисление простых чисел методом решета Эратосфена   
let rec primes = function
    [] -> []
  | h::t -> h::primes(filter (fun x->x%h>0) t)

primes [2..100]

// Разбиение списка на два в зависимости от предиката
// Получаем список отрицательных и неотрицательных чисел
List.partition ((>)0) [1;-3;0;4;3]

// Быстрая сортировка Хоара на списках
let rec qsort = function
   [] -> []
 | h::t ->
     qsort(List.filter ((>)h) t) @ [h] @ 
     qsort(List.filter ((<=)h) t)

// Более понятная запись через конструкторы списков
let rec qsort = function
   [] -> []
 | h::t ->
     qsort([for x in t do if x<=h then yield x]) @ [h] @ qsort([for x in t do if x>h then yield x])

// Более эффективная реализация с однопроходным разбиением списка через partition
let rec qsort = function
   [] -> []
 | h::t -> 
     let (a,b) = List.partition ((>)h) t
     qsort(a) @ [h] @ qsort(b)

qsort([4;5;5;5;5;2;3;1;6;5])

// Хитрая функция choose сочетает в себе filter и map
// Они могут быть определены через choose
let filter p = List.choose (fun x -> if p x then Some(x) else None)
let map f = List.choose (fun x -> Some(f x))

// 3. Fold - свёртка
// -----------------

// Сумма и произведение элементов списка через свёртку
let sum L = List.fold (fun s x -> s+x) 0 L
let sum = List.fold (+) 0
let product = List.fold (*) 1

// Поиск минимального и максимального элемента в один проход
// В качестве аккумулятора выступает пара значений
let minmax L = 
   let a0 = List.head L in
   List.fold (fun (mi,ma) x -> 
               ((if mi>x then x else mi),
                (if ma<x then x else ma))) 
             (a0,a0) L
let min L = fst (minmax L)
let max L = snd (minmax L)

// foldBack - это обратная свёртка
let minmax L = 
   let a0 = List.head L in
   List.foldBack (fun x (mi,ma) -> 
               ((if mi>x then x else mi),
                (if ma<x then x else ma))) 
             L (a0,a0)

minmax [1;0;3;-2;12]
min [1;0;3;-2;12]

// Если нужен только min или max, определения намного короче
// Reduce - это как свёртка, но в кач.аккумулятора выступает первый элемент списка
let min : int list -> int = List.reduce (fun a b -> Math.Min(a,b))

min [4;6;1;3;3]

let minimum a b = if a>b then b else a
let min L = List.reduce minimum L

// ------------------------------------
// Другие полезные библиотечные функции
// ------------------------------------

// простое итерирование 
List.iter (fun x -> printf "%s\n" x) ["One";"Two";"Three"];; 

// простое итерирование с номером
List.iteri (fun n x -> printf "%d. %s\n" (n+1) x) ["One";"Two";"Three"];; 

// простое итерирование по двум спискам
List.iter2 (fun n x -> printf "%d. %s\n" n x) [1;2;3] ["One";"Two";"Three"];;

// проверка существования элемента и выполнения свойства для всех элементов - через свёртку
let exists p = List.fold (fun a x -> a || (p x)) false;;
let for_all p = List.fold (fun a x -> a && (p x)) true;;

// Различные встроенные сортировки списков
["One";"Two";"Three";"Four"] |> List.sort // использует функцию сравнения по умолчанию
["One";"Two";"Three";"Four"] |> List.sortBy(String.length) // применяет к элементам списка заданную функцию и сортирует полученные значения (в данном случае - по длинам строк)
["One";"Two";"Three";"Four"] |> List.sortWith(fun a b -> a.Length.CompareTo(b.Length)) // явно заданная функция сравнения элементов

// нахождение максимального элемента (в данном случае - макс.длины строки)
["One";"Two";"Three";"Four"] |> List.maxBy(String.length)
// нахождение среднего значения (длины строки)
["One";"Two";"Three";"Four"] |> List.averageBy(fun s -> float(s.Length))

// Перестановка элементов списка
List.permute (function 0->0 | 1->2 | 2->1) [1;2;3]

// ------------------
// Хвостовая рекурсия
// ------------------

// Обычная функция длины списка
let rec len = function
   [] -> 0
 | h::t -> 1+len t

// Длина списка через хвостовую рекурсию с лишним аргументом-счётчиком
let rec len a = function
   [] -> a
 | _::t -> len (a+1) t

len 0 [1;2;3]

// Правильное описание функции len, где хвостовая рекурсия обёрнута в локальную функцию
let len l =
  let rec len_tail a = function
     [] -> a
   | _::t -> len_tail (a+1) t
  len_tail 0 l

// Переворачивание (реверсирование) списка со сложностью O(n^2)
let rec rev = function
  [] -> []
| h::t -> (rev t)@[h];; 

// Переворачивание с хвостовой рекурсией со сложностью O(n)
let rev L =
   let rec rev_tail s = function
     [] -> s
   | h::t -> rev_tail (h::s) t in
   rev_tail [] L

// ------------------------------------
// Синтаксис конструкторов списков
// ------------------------------------

[1..10]
(..) 1 10 // Оператор (..) - это каррированный оператор конструирования последовательности целых чисел
let integers n = (..) 1
[1.1..0.1..1.9]
[ for x in 0..8 -> 2.**float(x) ]
List.init 9 (fun x -> 2.0**float(x))
[0..8] |> List.map (fun x -> 2.0**float(x))

[ for a in -3.0..3.0 do 
    for b in -3.0..3.0 do
       for c in -3.0..3.0 do
         let d = b*b-4.*a*c
         if a<>0.0 then
           if d<0.0 then yield (a,b,c,None,None)
           else yield (a,b,c,Some((-b-Math.Sqrt(d))/2./a),Some((-b+Math.Sqrt(d))/2./a)) ]
