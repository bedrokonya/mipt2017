// =============================================================
// ������� ��������: �������������� ���������������� �� ����� F#
//                               http://www.soshnikov.com/fsharp
// -------------------------------------------------------------
// ����� 3: ����� ��������������� ���������������� - ����������
// =============================================================

// ����� ����������
// ----------------

// ������� ������� ������� ���������
let rec fib n =
  if n<2 then 1 else fib (n-1) + fib(n-2);;

// ������������� ������� - �� ���� ���������� ���������� ����������
open System.Collections.Generic
let mfib =
   let d = new Dictionary<int,int>()
   let rec fib n =
     if d.ContainsKey(n) then d.[n]
     else
       let res = if n<2 then 1 else fib (n-1) + fib(n-2)
       d.Add(n,res)
       res
   fun n -> fib n;;

// ������� ��� ��������� ������� ������
let time f =
  let st = System.DateTime.Now
  let res = f()
  let elaps = System.DateTime.Now - st
  printfn "Execution took %A" elaps
  res

time (fun() -> fib 30);;
time (fun() -> mfib 30);;

// ��������� ���������������� ���������� � ��������� �������

let memoize (f: 'a -> 'b) =
   let t = new System.Collections.Generic.Dictionary<'a,'b>()
   fun n ->
     if t.ContainsKey(n) then t.[n]
     else let res = f n
          t.Add(n,res)
          res;;

// ������������� ������� ��������� � �������������� memoize
let rec fibFast =
       memoize (
         fun n -> if n < 2 then 1 else fibFast(n-1) + fibFast(n-2));;

// ��� ��������� - ������� ������� ���������
let fibLazy n =
   let rec fl n = 
        lazy(
             if n < 2 then 1 else fl(n-1).Force() + fl(n-2).Force()
        ) in
   (fl n).Force();;

time (fun() -> fib 30);;
time (fun() -> mfib 30);;
time (fun()->fibFast 30);;
time (fun()->fibLazy 30);;

let mtime f =
  let st = new System.Diagnostics.Stopwatch()
  st.Start()
  let res = f()
  st.ElapsedTicks;;

for i in 1..30 do 
 printfn "%d,%d,%d,%d,%d" i
  (mtime (fun() -> fib i))
  (mtime (fun() -> mfib i))
  (mtime (fun()->fibFast i))
  (mtime (fun()->fibLazy i));;
