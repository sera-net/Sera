namespace Sera.Runtime.FSharp

open Sera.Runtime
open Sera.Runtime.FSharp.Emit.Ser

type SeraRuntimeFSharp private () =
    static member Instance = SeraRuntimeFSharp()
    
    interface ISeraRuntimePlugin with
        member this.GetSerEmitProvider() = SerEmitProvider()
        
        member this.GetDeEmitProvider() = failwith "todo"
