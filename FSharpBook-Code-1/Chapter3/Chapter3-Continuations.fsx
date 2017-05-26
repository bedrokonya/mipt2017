// =============================================================
// ������� ��������: �������������� ���������������� �� ����� F#
//                               http://www.soshnikov.com/fsharp
// -------------------------------------------------------------
// ����� 3: ����� ��������������� ���������������� - �����������
// =============================================================

// ������� ������� �������������� ������
let rec rev L =
  match L with
    [] -> []
  | h::t -> rev t @ [h]

rev [1;2;3]

// ������� �������������� � �������������� �����������
// l - ������
// f - �������, ������� ����� ��������� ��� ��������� ���������� (�����������)
let rec rv l f =
    match l with
    [] -> (f [])
    | h::t -> rv t (f>>(fun x -> h::x))
let rev l = rv l (fun x -> x)

// �������������� � ������������� � ���������� �������������
let rev L = 
  let rec rv l f =
     match l with
       [] -> (f [])
     | h::t -> rv t (f>>(fun x -> h::x))
  rv L (fun x -> x)

rev [1;2;3]


// ����������� ����� �������������� ��� �������� �������� �������� � ���������!
// ���������� �������� ������
type 't tree = Nil | Node of 't*('t tree)*('t tree)

// ������� ������ (���������� �����) ������
let rec size = function
   Nil -> 0
 | Node(_,L,R) ->
     1+(size L)+(size R);;

// ������� � �������������� �����������
let size t =
  let rec size' t cont =
    match t with
      Nil -> cont 0
    | Node(_,L,R) ->
       size' L (fun x1 ->
         size' R (fun x2 -> 
           cont(x1+x2+1))) in
  size' t (fun x->x);;

// ���������� ������� - ���������� ������������� ��������� �������� � ������������� ��� ������
// �� ������ ����������� � ����������� ��� ������ �� �������
let size t =
  let rec size' acc t cont =
    match t with
      Nil -> cont acc
    | Node(_,L,R) ->
       size' (1+acc) L (fun x ->
         size' x R cont) in
  size' 0 t (fun x->x);;
