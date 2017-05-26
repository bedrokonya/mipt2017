// =============================================================
// ������� ��������: �������������� ���������������� �� ����� F#
//                               http://www.soshnikov.com/fsharp
// -------------------------------------------------------------
// ����� 2: ����������� ��������� ������ - �������
// =============================================================

// �������� .NET-�������� (Array)
[| 1;2;3 |]

// � ��� ��������� ��� �� �� �������, ��� � � �������
// � ����� ������� �� ������ Seq
Array.map ((*)2) [|1;2;3|]
Seq.map ((*)2) [|1;2;3|]

// ������ � ��������� ������� - �� ������ � ������� �������� .[]
// ��������� ������� ������� � ����������� �����
let sum (a : int []) =
   let rec sumrec i s =
      if i<a.Length then sumrec (i+1) (s+a.[i])
      else s
   sumrec 0 0

sum [|1;2;3|]

// �������� ������� ����������
let intarray n =
   let a = Array.create n 0
   Array.iteri (fun i _ -> a.[i] <- (i+1)) a
   a

let r = intarray 10

// �������� "���������" ���������� �������
r.[1..5] <- [| 11..15 |]
r.[5..]

// ������������ �������� � ������������� ����� ������� init
let intarray n = Array.init n (fun i -> i+1)
let intarray n = [|1..n|]

// ResizeArray - ��� ������� List<> � .NET
// ���������� ��� ������������ ������� ���������� �����
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

// "��������" ����������� ������� 
// ------------------------------

// ������ ����������� �������
let pascal n = 
  let rec pas L n =
    let A::t = L in
    if n = 0 then L
    else
      pas (((1::[for i in 1..(List.length A-1) -> A.[i-1]+A.[i]])@[1])::L) (n-1)
  pas [[1;1]] n

pascal 10;;


// ������ ����������� ������� - ������ �������
let pascal n = 
  let rec pas L n =
    if n = 0 then L
    else
      let A::t = L in
      pas (((1::[for i in 1..(List.length A-1) -> A.[i-1]+A.[i]])@[1])::L) (n-1)
  pas [[1;1]] n

pascal 10;;

