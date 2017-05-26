// =============================================================
// Дмитрий Сошников: Функциональное программирования на языке F#
//                               http://www.soshnikov.com/fsharp
// -------------------------------------------------------------
// Глава 7: Решение типовых задач
// =============================================================

open System
open AdventureWorks
open Microsoft.FSharp.Linq.Query

let db = new AdventureWorksDataContext()

db.Log <- Console.Out

let display T = for x in T do printfn "%A" x

let prods0 = query <@ seq { for p in db.Products -> (p.Name,p.ListPrice) } @>
display prods0

let prods1 = 
  query <@ seq { for p in db.Products do 
                    for c in db.ProductCategories do
                      if p.ProductCategoryID.Value = c.ProductCategoryID 
                       && c.Name = "Tires and Tubes"
                       then yield p}
            |> Seq.sortBy (fun p -> p.Name)
            |> Seq.map (fun p -> (p.Name,p.ListPrice)) @>

display prods1

let prods2 = 
  query <@ join db.Products db.ProductCategories
            (fun p -> p.ProductCategoryID.Value)
            (fun pc -> pc.ProductCategoryID)
            (fun p pc -> (p.Name,pc.Name)) @>

display prods2


let prods3 = query <@ seq { for p in db.Products -> (p.Name,p.ProductCategory) } @>
display prods3


type Company = { Name : string; Address : string; City : string }
let custs = 
 query <@ seq { for c in db.Customers do
                  for ca in db.CustomerAddresses do 
                    for a in db.Addresses do
                      if c.CustomerID = ca.CustomerID
                      && ca.AddressID = a.AddressID then
                         yield { Name=c.CompanyName;Address=a.AddressLine1;City=a.City } }
            @>

display custs
                 

printf "Inserting..."
let pc = new ProductCategory()
pc.Name <- "Child Clothing"
pc.ParentProductCategoryID <- new Nullable<int>(3)
pc.rowguid <- Guid.NewGuid()
pc.ModifiedDate <- DateTime.Now
db.ProductCategories.InsertOnSubmit(pc)
db.SubmitChanges()
printfn "Done.."

Console.ReadKey() |> ignore
