// =============================================================
// Дмитрий Сошников: Функциональное программирования на языке F#
//                               http://www.soshnikov.com/fsharp
// -------------------------------------------------------------
// Глава 7: Решение типовых задач
// =============================================================

// Интерпретатор простейшего функционального языка программирования

#light

module Interp
open System.IO
open Ast
open Printf
open LambdaInterpreter

open Microsoft.FSharp.Text.Lexing

[<EntryPoint>]
let main(argv) = 

    if argv.Length <> 1 then begin 
        printf "usage: interp.exe <file>\n";
        exit 1;
    end;
    
    let stream = new StreamReader(argv.[0]) 
    let myProg = 

        // Create the lexer, presenting the bytes to the lexer as ASCII regardless of the original
        // encoding of the stream (the lexer specification 
        // is designed to consume ASCII)
        let lexbuf = Microsoft.FSharp.Text.Lexing.LexBuffer<_>.FromTextReader stream 

        // Call the parser 
        try 
            Pars.start Lex.token lexbuf
        with e -> 
            let pos = lexbuf.EndPos
            printf "error near line %d, character %d\n%s\n" pos.Line pos.Column (e.ToString()); 
            exit 1
      
    // Now look at the resulting AST, e.g. count the number of top-level 
    // statements, and then the overall number of nodes. 
    printf "Execution Begins...\n%A\n" (E myProg)
    0
