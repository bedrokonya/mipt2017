// =============================================================
// Дмитрий Сошников: Функциональное программирования на языке F#
//                               http://www.soshnikov.com/fsharp
// -------------------------------------------------------------
// Глава 6: Параллельные и асинхронные вычисления
// =============================================================
// Асинхронный ввод-вывод

// I. Синхронная реализация пакетной обработки файлов

open System.IO

let ReadFile fn =
   use f = File.OpenRead fn
   let len = (int)f.Length
   let cont = Array.zeroCreate len
   let cont' = f.Read(cont,0,len)
   let converter = System.Text.Encoding.UTF8
   converter.GetString(cont)

let WriteFile (str:string) fn =
    use f = File.OpenWrite fn
    let conv = System.Text.Encoding.UTF8
    f.Write(conv.GetBytes(str),0,conv.GetByteCount(str))

let FreqDict S =
   Seq.fold(
     fun (ht:Map<string,int>) v ->
      if v="" then ht else
       if Map.containsKey v ht then Map.add v ((Map.find v ht)+1) ht
       else Map.add v 1 ht)
     (Map.empty) S;;
          
let WriteDict dict fn = 
    let str = Seq.fold (fun s (k,v) -> s+(sprintf "%s: %d\r\n" k v)) "" dict
    WriteFile str fn

let ToWords (s:string) = s.Split([|'"';',';' ';':';'!';'.';'\n';'\r'|]);;

let ProcessFile f =
    let dict = 
      ReadFile f |> 
      ToWords |> 
      FreqDict |> 
      Map.toSeq|> 
      Seq.filter (fun (k,v) -> v>10 && k.Length>3)
    WriteDict dict (f+".res")

let GetFiles path =
    seq {
      for f in Directory.GetFiles(path) -> f
    }
    
let GetTextFiles p = GetFiles p |> Seq.filter (fun s -> s.EndsWith(".txt"));;

let time f =
    let sw = new System.Diagnostics.Stopwatch()
    sw.Start()
    let res = f()
    sw.Stop()
    printf "Time elapsed: %d ticks (%d milliseconds)\n" sw.ElapsedTicks sw.ElapsedMilliseconds
    res

time(
 fun () -> [ for f in GetTextFiles(@"c:\books") -> ProcessFile f ]
 );;


// II. Асинхронная реализация

let ReadFileAsync fn =
  async {
   use f = File.OpenRead fn
   let len = (int)f.Length
   let cont = Array.zeroCreate len
   let! cont' = f.AsyncRead(len)
   let converter = System.Text.Encoding.UTF8
   return converter.GetString(cont)
  }

let WriteFileAsync (str:string) fn =
   async {
    use f = File.OpenWrite fn
    let conv = System.Text.Encoding.UTF8
    do! f.AsyncWrite(conv.GetBytes(str),0,conv.GetByteCount(str))
  }

let ProcessFileAsync f =
   async {
    let! str = ReadFileAsync f
    let dict = 
      ToWords str |> 
      FreqDict |> 
      Map.toSeq|> 
      Seq.filter (fun (k,v) -> v>10 && k.Length>3)
    let st = Seq.fold (fun s (k,v) -> s+(sprintf "%s: %d\r\n" k v)) "" dict
    do! WriteFileAsync st (f+".res")
   }
   
let ProcessFilesAsync ()=
  Async.Parallel 
     [ for f in GetTextFiles(@"c:\books") -> ProcessFileAsync f ] |> 
  Async.RunSynchronously
   
time(
 fun () -> ProcessFilesAsync()
 );;


// III. Реализация с использованием агентного паттерна

let ProcessAgent =
    MailboxProcessor.Start(fun inbox ->
       let rec loop() = async {
        let! msg = inbox.Receive()
        printf "Processing %s\n" msg
        do! ProcessFileAsync msg
        printf "Done processing %s\n" msg
        return! loop()
       }
       loop()
     )  

ProcessAgent.Post(@"c:\books\prince.txt");;

for f in GetTextFiles(@"c:\books") do ProcessAgent.Post f

// IV. Параллельная реализация с агентом-диспетчером задач

let MailboxDispatcher n f =
    MailboxProcessor.Start(fun inbox ->
        let queue = Array.init n (fun i -> MailboxProcessor.Start(f))
        let rec loop i = async {
            let! msg = inbox.Receive()
            queue.[i].Post(msg)
            return! loop((i+1)%n)
        }
        loop 0
    )
    
let ParallelProcessAgent =
       MailboxDispatcher 2 (fun inbox ->
       let rec loop() = async {
        let! msg = inbox.Receive()
        printf "Processing %s\n" msg
        do! ProcessFileAsync msg
        printf "Done processing %s\n" msg
        return! loop()
       }
       loop()
     )
     
for f in GetTextFiles(@"c:\books") do ParallelProcessAgent.Post f
     
