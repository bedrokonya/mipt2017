// =============================================================
// Дмитрий Сошников: Функциональное программирования на языке F#
//                               http://www.soshnikov.com/fsharp
// -------------------------------------------------------------
// Глава 7: Решение типовых задач
// =============================================================

(* Интерпретатор лямбда-выражений *)

module LambdaInterpreter
open Ast

let arity = function
| "sin" -> 1
| otherwise -> 2


let funof = function
  "+" -> (function [Int(a);Int(b)] -> Int(a+b))
| "-" -> (function [Int(a);Int(b)] -> Int(a-b))
| "*" -> (function [Int(a);Int(b)] -> Int(a*b))
| "/" -> (function [Int(a);Int(b)] -> Int(a/b))
| "=" -> (function [Int(a);Int(b)] -> if a=b then Int(1) else Int(0))
| ">" -> (function [Int(a);Int(b)] -> if a>b then Int(1) else Int(0))
| "<" -> (function [Int(a);Int(b)] -> if a<b then Int(1) else Int(0))
| "<=" -> (function [Int(a);Int(b)] -> if a<=b then Int(1) else Int(0))

let rec eval exp env =
  match exp with
    App(e1,e2) -> apply (eval e1 env) (eval e2 env)
  | Int(n) -> Int(n)
  | Var(x) -> Map.find x env
  | PFunc(f) -> Op(f,arity f,[])
  | Op(id,n,el) -> Op(id,n,el)
  | Cond(e0,e1,e2) -> 
     if Int(1)=eval e0 env then eval e1 env else eval e2 env
  | Let(id,e1,e2) -> 
    let r = eval e1 env in
      eval e2 (Map.add id r env)
  | LetRec(id,e1,e2) ->
      eval e2 (Map.add id (RClosure(e1,env,id)) env)
  | Lam(id,ex) -> Closure(exp,env)
  | Closure(exp,env) -> exp
and apply e1 e2 =
  match e1 with
     Closure(Lam(v,e),env) -> eval e (Map.add v e2 env)
   | RClosure(Lam(v,e),env,id) -> eval e (Map.add v e2 (Map.add id e1 env))
   | Op(id,n,args) ->
      if n=1 then (funof id)(args@[e2])
      else Op(id,n-1,args@[e2])


let E exp = eval exp Map.empty;;
