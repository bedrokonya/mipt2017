call fslex -o lex.fs --unicode lex.fsl
call fsyacc -o pars.fs --module Pars pars.fsy
call fsc -g -o interp.exe -r "FSharp.Powerpack.dll" ast.fs pars.fsi pars.fs lex.fs lambdaint.fs main.fs
