// =============================================================
// Дмитрий Сошников: Функциональное программирования на языке F#
//                               http://www.soshnikov.com/fsharp
// -------------------------------------------------------------
// Глава 7: Решение типовых задач
// =============================================================

(* Синтаксический анализ выражений *)

type Expr = 
    | Add of Expr*Expr
    | Sub of Expr*Expr
    | Mul of Expr*Expr
    | Div of Expr*Expr
    | Value of int


type State = WhiteSpace | IntSpace of int
type Lexem = Int of int | Op of char

let ToInt (c:char) = int(c)-int('0')

let rec lex (s:string) state =
     if s="" then
         match state with
                     WhiteSpace -> []
                   | IntSpace(n) -> [Int(n)]
     else
         match s.[0] with
            ' ' -> 
              match state with
                 WhiteSpace -> lex (s.Substring(1)) WhiteSpace
               | IntSpace(n) -> Int(n)::(lex (s.Substring(1)) WhiteSpace)
          | x when x>='0'&&x<='9' ->
               match state with
                 WhiteSpace -> lex (s.Substring(1)) (IntSpace(ToInt(x)))
               | IntSpace(n) -> lex (s.Substring(1)) (IntSpace(n*10+ToInt(x)))
          | '+' | '-' | '*' | '/' ->
                 match state with
                     WhiteSpace -> Op(s.[0])::(lex (s.Substring(1)) WhiteSpace)
                   | IntSpace(n) -> Op(s.[0])::Int(n)::(lex (s.Substring(1)) WhiteSpace)

let parse x =
    let rec parse' = function 
          [] -> failwith "Error"
        | Op(c)::l ->
            let (t1,l1) = parse' l
            let (t2,l2) = parse' l1
            match c with
              '+' -> (Add(t1,t2),l2)
            | '-' -> (Sub(t1,t2),l2)
            | '*' -> (Mul(t1,t2),l2)
            | '/' -> (Div(t1,t2),l2)
        | Int(n)::l -> (Value(n),l)
    let (t,l) = parse' x
    if List.isEmpty l then t
    else failwith "Error"

let process text = parse(lex text WhiteSpace)

process "*+1 2 3"

