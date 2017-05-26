#r "c:\\winapp\\Math.NET.Numerics\MathNet.Numerics.dll"
#r "c:\\winapp\\Math.NET.Numerics\MathNet.Numerics.FSharp.dll"

open MathNet.Numerics.LinearAlgebra
open MathNet.Numerics.LinearAlgebra.Double

let interpolate (s: (float*float) seq) =
   let xs = Seq.map fst s |> Seq.toArray
   let ys = Seq.map snd s |> Seq.toArray
   let n = Seq.length s
   let M = MathNet.Numerics.LinearAlgebra.Double.V
   (n,3,
                              fun i j -> match j with
                                          0 -> xs.[i]
                                        | 1 -> 1.0
                                        | 2 -> ys.[i])
   fun x -> 1.0


