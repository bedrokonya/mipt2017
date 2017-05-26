// =============================================================
// Дмитрий Сошников: Функциональное программирования на языке F#
//                               http://www.soshnikov.com/fsharp
// -------------------------------------------------------------
// Глава 7: Решение типовых задач
// =============================================================

namespace FSharpMVC2.Web.Models

open System.ComponentModel

type InputModel() =
    let mutable url=""
    [<DisplayName("Enter Url:")>]
    member x.Url with get()=url and set(v) = url <- v

