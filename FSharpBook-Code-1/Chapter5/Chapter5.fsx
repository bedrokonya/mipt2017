// =============================================================
// Дмитрий Сошников: Функциональное программирования на языке F#
//                               http://www.soshnikov.com/fsharp
// -------------------------------------------------------------
// Глава 5: Метапрограммирование
// =============================================================

#r "FSharp.Powerpack.dll"

(* Активные шаблоны - Active Patterns *)

// Как сделать проверку без активных шаблонов
let test x = if x%2=0 then printfn "%d is even" x
                      else printfn "%d is odd" x

// Как сделать проверку через сопоставление с образцом
let test x =
    match x with
      | x when x%2=0 -> printfn "%d is even" x
      | _ -> printfn "%d is odd" x

// Активный шаблон
let (|Even|Odd|) x = if x%2=0 then Even else Odd

// Использование активного шаблона
let test x = 
    match x with
      | Even -> printfn "%d is even" x
      | Odd -> printfn "%d is odd" x  

// Сложный активный шаблон регулярных выражений с параметром 
open System
open System.Text.RegularExpressions

let (|Match|_|) (pat : string) (inp : string) = 
    let m = Regex.Match(inp, pat)
    if m.Success then Some (List.tail [ for g in m.Groups -> g.Value ]) 
                 else None

match "My daughter is 16 years old" with
  | Match "(\d+)" x -> printfn "found %A" x
  | _ -> printfn "Not found"


(* Квотирование - Quotations *)
open Microsoft.FSharp.Quotations
open Microsoft.FSharp.Quotations.Patterns

printf "%A" (<@@ (1+2)*3 @@>)
<@@ (1+2)*3 @@>

printf "%A" (<@ (1+2)*3 @>)
<@ (1+2)*3 @>

printf "%A" (<@ (+)1 @>)

// Преобразование арифметического выражение к командам стекового калькулятора 
type Command = Push of int
             | Add
             | Sub
             | Mult
             | Div
             | Print
            ;;
            
type Prog = Command list;;

let rec compile l E = 
 match E with
   Call(_,Op,Args) -> compileop (compilel l Args) Op
 | Value(x,_) -> Push(int(x.ToString()))::l
and compileop l Op = 
  if Op.Name="op_Addition" then Add::l
  else if Op.Name="op_Multiply" then Mult::l
  else if Op.Name="op_Subtraction" then Sub::l
  else if Op.Name="op_Division" then Div::l
  else l
and compilel l = function
    [] -> l
  | h::t -> compilel (compile l h) t;;

let comp : (Expr->Command list) = compile [];;

comp <@ (1+2)*3-4/2 @>
comp <@@ (1+2)*3 @@>
comp <@@ 1+2*3 @@>

// Генерация дерева выражений

let test x = <@ %x+1 @>

let rec power n x =
  if n=0 then 1
  else x*power (n-1) x;;
let pow5 = power 5;;
let pow5 x = x*x*x*x*x;;

// Моделирование суперкомпиляции через квотирование и создание выражений

#r "FSharp.Powerpack.Linq"

// Raising x to the power of n
// Returns quotation expression that calculates the result
// metapower 5 x returns <@ x*x*x*x*x @>
let rec metapower n x =
  if n=0 then <@ 1 @>
  else (fun z -> <@ %x * %z @>) (metapower(n-1) x);;

// Defining short synonim for quotation evaluation
let qeval = Microsoft.FSharp.Linq.QuotationEvaluator.Evaluate

// Raising arbitrary number to the power of 5
// Gets the quotation with expression x*x*x*x*x and evaluates it
let pow5 x = metapower 5 <@ x @> |> qeval

printfn "%d" (pow5 10)

(* Монады *)

// Монада вывода
type State = string list;;

let print_int (x:int) = fun l -> x.ToString() :: l
let print_str (x:string) = fun l -> x::l

print_str "!" (print_int (3*5) (print_str "The result is" []));;

let (>>=) A B = B A;;

[] >>= print_str "The result is" >>= print_int 15 >>= print_str "!";;


// Монада ввода-вывода

type State = string list * string list;;
type IO <'t> = (State->'t*State);;

let print (t:string) = fun (I,O) -> ((),(I,t::O));;
let read = fun (I,O) -> (List.head I,(List.tail I,O));;
let ret (x:string) = fun S -> (x,S);;

let (>>=) a b =
  fun s0 ->
    let (ra,s1) = a s0 in
    b ra s1

let (>>) f g = 
  fun s0 ->
    let (_,s1) = f s0 in 
    g s1
 
let (>>) f g = f >>= (fun _ -> g)

(read >>= print) (["hello"],[]);;
(read >>= fun t -> ret ("I like "+t) >>= print) (["Vasya"],[]);;
(read >>= fun t -> print ("I like "+t)) (["Vasya"],[]);;
(read >>= (fun t -> print ("I like "+t))>> print "there")(["hello"],[]);;
(print "You name:" >> 
 read >>= 
 (fun t -> print ("I like "+t)) >> 
 print "Goodbye")
   (["Mike"],[]);;

(print "Hello" >> print "There" >> print "!")([],[]);;

// Монада недетерминированных вычислений
type Nondet<'a> = 'a list;;
let ret x = [x];;
let ret' x = x;;
let fail = [];;

let (>>=) mA (b:'b->Nondet<'b>) = List.collect b mA;;

ret' [1;2;3] >>= 
  fun x -> [x*4;x*5];;

ret' [0..9] >>= fun x->
 List.map (fun z -> z*10+x) [0..9];;

ret 10 >>= fun x -> ret (x*2);;
  
// Монадическое выражение для недетерминированных вычислений
type NondetBuilder() =
  member m.Return(x) = ret x
  member m.Bind(mA,b) = mA >>= b
  member m.Zero() = fail
  member m.Combine(a,b) = a@b
  member m.Delay(x) = x()
  member m.ReturnFrom(x) = x

let nondet = new NondetBuilder()

nondet { let x = 10 in return! [x;x+1] } ;;
nondet { let! x = [1;2;3] in return! [x*2;x*3] } ;;
nondet { let! x = [1;2;3] in if x>2 then return! [x;x+1] else fail } ;;

// Недетерминированное удаление элемента из списка
let rec remove x l =
  nondet {
    if l=[] then return []
    else
     if (List.head l) = x then return (List.tail l)
     return! List.map (fun x -> (List.head l)::x) (remove x (List.tail l))
  }
;;

remove 2 [1;2;3;4;3;2;1];;

let rec premove l =
  nondet {
    if l=[] then return (0,[])
    else
     return (List.head l,List.tail l)
     return! List.map (fun (z,r) -> (z,(List.head l)::r)) (premove (List.tail l))
  }
;;

premove [1;2;3];;


(*let rec permute l =
  nondet {
    if l=[] then return []
    else
      let! (z,r) = premove l in
        return! (map (fun t -> z::t) (permute r))
  };;

nondet { let! z,r = (premove [1;2;3]) in return r }

*)

 
// Решение логической задачки
let lev = [false;false;false;true;true;true;true];;
let edi = [true;true;true;false;false;false;true];;
let days = ["mon";"tue";"wed";"thu";"fri";"sat";"sun"];;

let data = List.zip3 lev edi days;;

let rec prev last hit l =
  match l with
    [] -> last
  | h::t -> if hit h then last else prev h hit t;;

prev (true,true,"sun") (fun (_,_,x) -> x="sun") data;;

let realday state said = 
  if state then said else not(said);;

let res = 
 nondet {
   let! (l,e,d) = data in
    let (l1,e1,d1) = prev (true,true,"sun") (fun (_,_,x) -> x=d) data in
    if (realday l false) = l1 && (realday e false) = e1 then return (l1,e1,d1)
  };;
  