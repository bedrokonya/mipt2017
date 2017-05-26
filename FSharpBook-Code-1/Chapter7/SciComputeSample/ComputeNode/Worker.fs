// =============================================================
// Дмитрий Сошников: Функциональное программирования на языке F#
//                               http://www.soshnikov.com/fsharp
// -------------------------------------------------------------
// Глава 7: Решение типовых задач
// =============================================================

namespace ComputeNode

open System
open System.Diagnostics
open System.Linq;
open System.Net;
open System.Threading
open Microsoft.WindowsAzure.Diagnostics
open Microsoft.WindowsAzure.ServiceRuntime
open Microsoft.WindowsAzure.StorageClient
open Microsoft.WindowsAzure

type Worker() =
    inherit RoleEntryPoint() 

    let log message kind = Trace.WriteLine(message, kind)

    let mutable queue_in : CloudQueue = null
    let mutable queue_out : CloudQueue = null
    let mutable num = 10
    let mutable num_slices = 5000
    let mutable func = fun x -> sqrt(x)

    let rec ffor op f (a: float) (b: float) (h: float) =
        if a>=b then f b
        else op (f a) (ffor op f (a+h) b h)

    let integrate f a b =
        let h = (b-a)/float(num_slices) in
        ffor (+) (fun x -> h*f(x)) a b h

    let ProcessMessage (s:string) =
        let a = s.Split(':')
        match a.[0] with
          "I" -> 
             let l = Double.Parse(a.[1])
             let r = Double.Parse(a.[2])
             let h = (r-l)/float(num)
             for i in 1..num do
                let l1 = l+float(i-1)*h
                let r1 = l1+h
                let s = "i:"+l1.ToString()+":"+r1.ToString()
                queue_in.AddMessage(new CloudQueueMessage(s))
             null
        | "i" ->
             log ("Integrating "+s) "Information"
             let l = Double.Parse(a.[1])
             let r = Double.Parse(a.[2])
             let res = integrate func l r    
             "r:"+res.ToString()
        | _ -> null

    override wr.Run() =
        log "Starting computation agent..." "Information"
        while(true) do 
            let msg = queue_in.GetMessage()
            if msg=null 
                then Thread.Sleep(1000)
                else
                    let res = ProcessMessage(msg.AsString)
                    queue_in.DeleteMessage(msg)
                    if res<>null then queue_out.AddMessage(new CloudQueueMessage(res))
            log "Processing queue" "Information"

    override wr.OnStart() = 
        // Set the maximum number of concurrent connections 
        ServicePointManager.DefaultConnectionLimit <- 12

        DiagnosticMonitor.Start("DiagnosticsConnectionString") |> ignore
        
        RoleEnvironment.Changing.Add(fun e -> 
            // If a configuration setting is changing
            if e.Changes |> Seq.exists (fun change -> change :? RoleEnvironmentConfigurationSettingChange) then 
                // Set e.Cancel to true to restart this role instance
                e.Cancel <- true)
        let act = new Action<string,Func<string,bool>>(fun str func -> func.Invoke(RoleEnvironment.GetConfigurationSettingValue(str)) |> ignore)
        CloudStorageAccount.SetConfigurationSettingPublisher(act)
        let storageAccount = CloudStorageAccount.FromConfigurationSetting("DataConnectionString")
        let cloudQueueClient = storageAccount.CreateCloudQueueClient()
        let queue_in' = cloudQueueClient.GetQueueReference("sci-queue-in")
        let queue_out' = cloudQueueClient.GetQueueReference("sci-queue-out")
        queue_in'.CreateIfNotExist() |> ignore
        queue_out'.CreateIfNotExist() |> ignore
        queue_in <- queue_in'
        queue_out <- queue_out'
        base.OnStart()
