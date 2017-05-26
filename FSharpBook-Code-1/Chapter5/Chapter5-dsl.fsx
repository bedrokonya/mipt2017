// =============================================================
// Дмитрий Сошников: Функциональное программирования на языке F#
//                               http://www.soshnikov.com/fsharp
// -------------------------------------------------------------
// Глава 5: Метапрограммирование
// =============================================================
// Проблемно-ориентированный язык

open System
open System.Collections.Generic

type Person (n : string) =
        let mutable name = n
        let mutable father : Person option = None
        let mutable mother : Person option = None
        let mutable birthdate = DateTime.MinValue
        member x.Name with get()=name and set(v) = name<-v
        member x.Father with get()=father and set(v) = father<-v
        member x.Mother with get()=mother and set(v) = mother<-v
        member x.Birthdate with get()=birthdate and set(v) = birthdate<-v

let People = new Dictionary<string,Person>()

let born str_date =
     DateTime.Parse(str_date)
let unknown_birth=DateTime.MinValue

let father s = People.[s]
let mother s = People.[s]
let child s = People.[s]

let person name bdate =
    let P = new Person(name)
    P.Birthdate <- bdate
    People.Add(name,P)
    P

let rec family F M = function
    [] -> ()
  | (h:Person)::t ->
      h.Father <- Some(F)
      h.Mother <- Some(M)
      family F M t

person "Aaron" (born "21.03.1974")
person "Mary" unknown_birth
person "John" (born "30.12.1940")
person "Vickie" (born "14.05.2004")
person "Julia" unknown_birth
person "Justin" unknown_birth

family 
  (father "Aaron")
  (mother "Julia")
  [child "Vickie"]

family
  (father "John")
  (mother "Mary")
  [child "Aaron";child "Justin"]


let parent = function
   Some(x:Person) -> x.Name
 | None -> "None"

for x in People.Values do printfn "%s <- (%s,%s)" x.Name (parent x.Father) (parent x.Mother)

(* Additional Typing *)

type tperson = Father of Person | Mother of Person | Child of Person

let father s = Father(People.[s])
let mother s = Mother(People.[s])
let child s = Child(People.[s])

let family P1 P2 L =
    let rec rfamily F M = function
            [] -> ()
          | Child(h)::t ->
              h.Father <- Some(F)
              h.Mother <- Some(M)
              rfamily F M t
    match P1,P2 with
       Father(F),Mother(M) -> rfamily F M L
     | Mother(M),Father(F) -> rfamily F M L
     | _ -> failwith "Wrong # of parents"
    
