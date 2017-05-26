// =============================================================
// ������� ��������: �������������� ���������������� �� ����� F#
//                               http://www.soshnikov.com/fsharp
// -------------------------------------------------------------
// ����� 7: ������� ������� �����
// =============================================================

// ����������� �������������� ������ ����������� ��������������� ����� ����������������

module Ast

type id = string;;
type expr = 
   Var of id
 | Lam of id*expr
 | App of expr*expr
 | Int of int
 | Cond of expr*expr*expr
 | Let of id*expr*expr
 | LetRec of id*expr*expr
 | PFunc of id
 | Op of id*int*expr list
 | Closure of expr*env
 | RClosure of expr*env*id
and
 env = Map<id,expr>;;
;;

type prog = expr;;
