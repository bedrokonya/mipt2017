// =============================================================
// Дмитрий Сошников: Функциональное программирования на языке F#
//                               http://www.soshnikov.com/fsharp
// -------------------------------------------------------------
// Глава 2: Рекурсивные структуры данных - Матрицы
// =============================================================

// Многомерные массивы
// -------------------

// Создание массива функцией инициализации
let A = Array2D.init 3 4 (fun i j -> i*3+j)

// Синтаксис вырезания частей массива
A.[0..,1..2]

// Пример: свёртка по столбцам
let fold_cols f i (A : 't[,]) =
   let n = Array2D.length2 A
   let res = Array.create n i
   Array2D.iteri (fun i j x -> res.[j] <- f res.[j] x) A
   res

fold_cols (+) 0 A
fold_cols (+) 0 (Array2D.init 3 4 (fun i j -> i*3+j))
fold_cols (+) 0.0 (Array2D.init 3 4 (fun i j -> float(i)*3.0+float(j)))


// Вектора и матрицы
// -----------------

#r "FSharp.Powerpack.dll"

open Microsoft.FSharp.Math

let v = vector [1.;2.;3.]
let rv = rowvec [1.;2.;3.]
let m : Matrix<float> = matrix [ [ 1.;2.;3.];[4.;5.;6.];[7.;8.;9.]]
rv*v,v*rv,m*v,rv*m

// Матрицы могут быть разреженными
let sparse = Matrix.initSparse 100 100 [for i in 0..99 -> (i,i,1.0)]
let dense = Matrix.init 100 100 (fun i j -> float(i)*0.1+float(j)/2.0)
dense.IsSparse, sparse.IsSparse
(dense * sparse).IsSparse, (sparse * dense).IsSparse
(2.0*sparse).IsSparse
sparse.Row(1)

// Пример: диагонализация матрицы методом Гаусса
let diagonalize (m:Matrix<float>) =
   let nrows = m.NumRows-1
   let ncols = m.NumCols-1
   let norm j =
       Seq.iteri (fun i x -> m.[j,i] <- x / m.[j,j]) (m.Row j)
   let swaprow i j = 
       let r = m.[i..i,0..ncols]
       m.[i..i,0..ncols] <- m.[j..j,0..ncols]
       m.[j..j,0..ncols] <- r
   let rec swapnz i j =
       if j<=nrows then
         if m.[j,i]<>0. then swaprow i j
         else swapnz i (j+1)
   for i = 0 to nrows do
       if m.[i,i]=0. then swapnz i (i+1)
       if m.[i,i]<>0. then
           norm i
           for j = i+1 to nrows do
              let c = m.[j,i]
              for k=i to ncols do m.[j,k] <- m.[j,k]-m.[i,k]*c
   m

let m : Matrix<float> = matrix [ [ 1.;2.;3.;4.];[2.;4.;6.;89.];[4.;8.;6.;11.]]
diagonalize m

