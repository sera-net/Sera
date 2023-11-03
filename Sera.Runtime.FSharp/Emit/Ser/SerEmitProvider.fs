namespace Sera.Runtime.FSharp.Emit.Ser

open Sera.Runtime

type internal SerEmitProvider() =
    interface ISeraEmitProvider with
        member this.TryCreateJob(target) =
            if target.Type.IsGenericType then
                let del = target.Type.GetGenericTypeDefinition()

                if del = typedefof<_ option> || del = typedefof<_ ValueOption> then
                    let item_type = target.Type.GetGenericArguments()[0]
                    Jobs._Option item_type
                else
                    null
            else
                null
