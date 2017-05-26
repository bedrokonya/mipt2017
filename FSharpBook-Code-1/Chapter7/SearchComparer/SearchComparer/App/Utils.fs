namespace WindowsPhonePanoramaApp

open System
open System.Windows
open System.Windows.Controls
open System.Threading
open System.IO

[<AutoOpen>]
module Utilities = 
                                                                                      
    /// This is an implementation of the dynamic lookup operator for binding
    /// Xaml objects by name.
    let (?) (source:obj) (s:string) =
        match source with 
        | :? ResourceDictionary as r ->  r.[s] :?> 'T
        | :? Control as source -> 
            match source.FindName(s) with 
            | null -> invalidOp (sprintf "dynamic lookup of Xaml component %s failed" s)
            | :? 'T as x -> x
            | _ -> invalidOp (sprintf "dynamic lookup of Xaml component %s failed because the component found was of type %A instead of type %A"  s (s.GetType()) typeof<'T>)
        | _ -> invalidOp (sprintf "dynamic lookup of Xaml component %s failed because the source object was of type %A. It must be a control of a resource dictionary" s (source.GetType()))

    type SynchronizationContext with
        /// A standard helper extension method to raise an event on the GUI thread
        member syncContext.RaiseEvent (event: Event<_>) args =
            //let mutable syncContext : SynchronizationContext = null
            syncContext.Post((fun _ -> event.Trigger args),state=null)
     
        /// A standard helper extension method to capture the current synchronization context.
        /// If none is present, use a context that executes work in the thread pool.
        static member CaptureCurrent () =
            match SynchronizationContext.Current with
            | null -> new SynchronizationContext()
            | ctxt -> ctxt

    type System.String with
        
        member x.GetBytes() =            
            let utf8Enc = System.Text.Encoding.UTF8
            utf8Enc.GetBytes(x)

        member x.ToDouble() =
            let mutable d = Double.MinValue
            Double.TryParse(x, ref d) |> ignore
            d

    type JobCompletedEventArgs<'T>(result:'T) =
        inherit EventArgs()
        member x.Result with get() = result

    type AsyncWorker<'T>(job: Async<'T>) =
     
        // This declares an F# event that we can raise
        let allCompleted  = new Event<'T>()
        let error         = new Event<System.Exception>()
#if WPF
        let canceled      = new Event<System.OperationCanceledException>()
#else
        let canceled      = new Event<OperationCanceledException>()
#endif
        let jobCompleted  = new Event<JobCompletedEventArgs<'T>>()
        let cancellationCapability = new CancellationTokenSource()

        /// Start an instance of the work
        member x.Start()    =
            // Capture the synchronization context to allow us to raise events back on the GUI thread
            let syncContext = SynchronizationContext.CaptureCurrent()
     
            // Mark up the jobs with numbers
            let raiseEventOnGuiThread(evt, args) = syncContext.RaiseEvent evt args
            let work = async { let! result = job
                               syncContext.RaiseEvent jobCompleted (new JobCompletedEventArgs<'T>(result))
                               return result }
            Async.StartWithContinuations
                ( work,
                  (fun res -> raiseEventOnGuiThread(allCompleted, res)),
                  (fun exn -> raiseEventOnGuiThread(error, exn)),
                  (fun exn -> raiseEventOnGuiThread(canceled, exn)),
                    cancellationCapability.Token)

        /// Raised when a particular job completes
        [<CLIEvent>]
        member x.JobCompleted = jobCompleted.Publish
        /// Raised when all jobs complete
        [<CLIEvent>]
        member x.AllCompleted = allCompleted.Publish
        /// Raised when the composition is cancelled successfully
        [<CLIEvent>]
        member x.Canceled = canceled.Publish
        /// Raised when the composition exhibits an error
        [<CLIEvent>]
        member x.Error = error.Publish

    type System.Net.WebRequest with
 
    /// An extension member to read the content from a response to a WebRequest.
    /// The read of the content is synchronous once the response has been received.
    member req.AsyncReadResponse () =
        async { use! response = Async.FromBeginEnd(req.BeginGetResponse, req.EndGetResponse)
                use responseStream = response.GetResponseStream()
                use reader = new StreamReader(responseStream)
                return reader.ReadToEnd() }

