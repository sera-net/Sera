namespace Sera.Runtime.FSharp.Emit.Ser

open System
open System.Reflection
open Sera
open Sera.Core
open Sera.Runtime
open Sera.Runtime.Emit
open Sera.Runtime.FSharp.Utils
open Sera.Runtime.FSharp.Utils.Cont

module internal SerEmitProviderModule =
    let option_type = typedefof<_ option>
    let ValueOption_type = typedefof<_ ValueOption>

    let is_option t = t = option_type || t = ValueOption_type

open SerEmitProviderModule

type internal SerEmitProvider() =
    interface ISeraEmitProvider with
        member this.TryCreateJob(target) =
            cont<EmitJob> {
                if target.Type.IsGenericType then
                    let def = target.Type.GetGenericTypeDefinition()

                    if is_option def then
                        let item_type = target.Type.GetGenericArguments()[0]
                        return! Jobs._Option item_type

                if UnionReflectionUtils.IsUnion(target.Type) then
                    return! this.CreateUnion(target)

                return null
            }

    member private this.CreateUnion(target: EmitMeta) : EmitJob =
        let name = target.Type.Name // todo rename
        let union_attr = target.Type.GetCustomAttribute<SeraUnionAttribute>()
        let format_attr = target.Type.GetCustomAttribute<SeraFormatsAttribute>()
        let style = UnionStyle.FromAttr(union_attr, format_attr)
        let info = UnionReflectionUtils.GetUnionInfo(target.Type)

        if target.Type.IsVisible then
            Jobs._Union._Public (name, info, style)
        else
            Jobs._Union._Private (name, info, style)
