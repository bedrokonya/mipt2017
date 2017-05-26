// =============================================================
// Дмитрий Сошников: Функциональное программирования на языке F#
//                               http://www.soshnikov.com/fsharp
// -------------------------------------------------------------
// Глава 2: Рекурсивные структуры данных - Деревья
// =============================================================

// Деревья общего вида
// -------------------

type 'T tree = 
  Leaf of 'T
| Node of 'T*('T tree list)

let tr = Node(1,[Node(2,[Leaf(5)]);Node(3,[Leaf(6);Leaf(7)]);Leaf(4)])

// Обход всех узлов дерева
let rec iter f = function
  Leaf(T) -> f T
| Node(T,L) -> (f T; for t in L do iter f t done)

iter (fun x -> printf "%A\n" x) tr

// Обход узлов дерева с указанием глубины
let iterh f = 
  let rec itr n = function
    Leaf(T) -> f n T
  | Node(T,L) -> (f n T; for t in L do itr (n+1) t done) in
  itr 0;;

let spaces n = List.fold (fun s _ -> s+" ") "" [0..n]

let print_tree T = iterh (fun h x -> printf "%s%A\n" (spaces (h*3)) x) T

print_tree tr

// Отображение map для деревьев
let rec map f = function
   Leaf(T) -> Leaf(f T)
 | Node(T,L) -> Node(f T,List.map (fun t -> map f t) L)


// Деревья общего вида, индуцируемые генератором
// ---------------------------------------------

// Работа с деревом каталогов
// Печать дерева каталогов
open System.IO
let print_dir_tree path =
    let rec tree path ind =
       Directory.GetDirectories path |>
       Array.iter(fun dir ->
         printfn "%s%s" (spaces (ind*3)) dir;
         tree dir (ind+1))
    tree path 0

print_dir_tree "c:\winapp"

// Возвращаем последовательность директорий (с глубиной) путём обхода дерева
let dir_tree path =
    let rec tree path ind =
       seq {
           for dir in Directory.GetDirectories path do
             yield (dir,ind)
             yield! (tree dir (ind+1))
           }
    tree path 0

dir_tree @"c:\winapp" |> Seq.iter (fun (s,h) -> printf "%s%s\n" (spaces (h*3)) s)

// Пример: подсчёт размера директорий (аналогично UNIX-команде du)
let rec du path =
   Directory.GetDirectories path |>
   Array.iter(fun dir ->
     let sz = Directory.GetFiles dir |>
                Array.sumBy (fun f -> (new FileInfo(f)).Length)
     printfn "%10d %s" sz dir;
     du dir)

du @"c:\winapp"

// Двоичные деревья
// ----------------

type 't btree = 
   Node of 't * 't btree * 't btree
 | Nil ;;

let tr = Node(6,
            Node(3,
               Node(1,Nil,Nil),
            Node(4,Nil,Nil)),
            Node(7,Nil,Nil))

// Различные обходы дерева - префиксный, инфиксный, постфиксный

let prefix root left right = (root(); left(); right());;
let infix root left right = (left(); root(); right());;
let postfix root left right = (left(); right(); root());;

let iterh trav f t = 
  let rec tr t h =
    match t with
     Node (x,L,R) -> trav 
                      (fun () -> (f x h)) 
                      (fun () -> tr L (h+1)) 
                      (fun () -> tr R (h+1));
     | Nil -> ()
  tr t 0

let print_tree T = iterh infix (fun x h -> printf "%s%A\n" (spaces h) x) T;;

print_tree tr

// Свёртка дерева в инфиксном порядке обхода
let fold_infix f init t = 
   let rec tr t x =
     match t with
       Node (z,L,R) -> tr L (f z (tr R x))
     | Nil -> x
   tr t init

// Преобразование дерева в список
let tree_to_list T = fold_infix (fun x t -> x::t) [] T

tree_to_list tr

// Двоичные деревья поиска
// -----------------------

// Вставка элемента в дерево поиска
let rec insert x t = 
  match t with
    Nil -> Node(x,Nil,Nil)
  | Node(z,L,R) -> if z=x then t 
                   else if x<z then Node(z,insert x L,R) 
                   else Node(z,L,insert x R)

// Преобразование списка элементов (с отношением порядка) в дерево
// Используем свёртку, в которой аккумулятором выступает дерево!
let list_to_tree L = List.fold (fun t x -> insert x t) Nil L

// Сортировка списка с помощью дерева
let tree_sort : int list -> int list = list_to_tree >> tree_to_list;;

let tree_sort L = (list_to_tree >> tree_to_list) L;;

tree_sort [4;3;1;6;7]

// Деревья выражений
// -----------------

type Operation = Add | Sub | Mul | Div
type ExprNode = Op of Operation | Value of int
type ExprTree = ExprNode btree

let ex = Node(Op(Add),Node(Op(Mul),Node(Value(1),Nil,Nil),Node(Value(2),Nil,Nil)),Node(Value(3),Nil,Nil))

type Expr = 
  Add of Expr * Expr
  | Sub of Expr * Expr
  | Mul of Expr * Expr
  | Div of Expr * Expr
  | Value of int

let ex = Add(Mul(Value(1),Value(2)),Value(3))

// Вычисление выражения по дереву
let rec compute = function
   Value(n) -> n
 | Add(e1,e2) -> compute e1 + compute e2
 | Sub(e1,e2) -> compute e1 - compute e2
 | Mul(e1,e2) -> compute e1 * compute e2
 | Div(e1,e2) -> compute e1 / compute e2

compute ex
