// =============================================================
// ������� ��������: �������������� ���������������� �� ����� F#
//                               http://www.soshnikov.com/fsharp
// -------------------------------------------------------------
// ����� 4: ������������ � ��������-��������������� ����������.
// =============================================================
// ������������ ����������� � �����������

// ������������ ��������� ������ � ������������ �����
// � ����������� mutable-�����������
let sum_list l =
   let mutable acc = 0
   for x in l do
     acc <- acc+x
   acc

sum_list [1;2;3]

// ������������ ��������� ������ � ������������ �����
// � ref-��������
let sum_list l =
   let acc = ref 0
   for x in l do
      acc := !acc+x
   !acc

sum_list [1;2;3]


open System

// ������ ������������� ����� While � ���������� ��������� ������ .NET
// ResizeArray � F# - ������� List � .NET
let ReadLines() =
   let a = new ResizeArray<string>()
   let mutable s = ""
   while s<>"." do
     s <- Console.ReadLine()
     a.Add(s)
   List.ofSeq a

// �������� �������� ����� �� ����� else-�����, ���� ���������� unit

let print_sign x =
   if x>0 then printfn "Positive"
   elif x=0 then printfn "Zero"
   else printfn "Negative"

let warn_if_negative x =
  if x<0 then printfn "Negative!!!"

// Null-��������

// � �������������� ������� ������ ������������ option type
// �� ������� .NET ����� ���������� �������� null
let getenv s = System.Environment.GetEnvironmentVariable s

getenv "PATH"
getenv "PAX"

// ��� ��� ����� ��������������� � ������������� ����
let getenv s = match System.Environment.GetEnvironmentVariable s with
                | null -> None
                | x -> Some(x)


// ����������

open System.IO
let ReadFile f =
   let fi = File.OpenText(f)
   let s = fi.ReadToEnd()
   fi.Close()
   s

ReadFile "dd\\:/"              // NotSupportedException
ReadFile "c:\nonexistant.txt"  // ArgumentException
ReadFile @"c:\nonexistant.txt" // FileNotFoundException

// ��������� ����������
open System
let ReadFile f =
   try
     let fi = File.OpenText(f)
     let s = fi.ReadToEnd()
     fi.Close()
     Some(s)
   with
     | :? FileNotFoundException -> eprintfn "File not found"; None
     | :? NotSupportedException 
     | :? ArgumentException -> eprintfn "Illegal path"; None
     | _ -> eprintfn "Unknown error"; None

ReadFile "dd\\:/"
ReadFile "c:\nonexistant.txt"
ReadFile @"c:\nonexistant.txt"


let ReadFile f =
   let fi = File.OpenText(f)
   try
     let s = fi.ReadToEnd()
     s
   finally
     fi.Close()

// �������� � ��������� ����������� ����������
exception CannotReadFile of string
let ReadFile f =
   try
     use fi = File.OpenText(f)
     fi.ReadToEnd()
   with
     | :? FileNotFoundException | :? NotSupportedException 
     | :? ArgumentException -> raise (CannotReadFile(f))
     | _ -> failwith "Unknown error"

try
 ReadFile @"c:\nonexistant.txt"
with
 CannotReadFile(f) -> eprintfn "Cannot read file: %s" f; ""

