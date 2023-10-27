namespace Sera.FSharp.Impls.Ser

open System.Runtime.CompilerServices
open Sera.Core

[<IsReadOnly; Struct>]
type OptionImpl<'T, 'D when 'D :> ISeraVision<'T>>(dep: 'D) =
    interface ISeraVision<'T option> with
        [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
        member this.Accept(visitor, value) = this.Accept visitor value

    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    member this.Accept (visitor: #ASeraVisitor<'a>) value =
        match value with
        | None -> visitor.VNone()
        | Some v -> visitor.VSome(dep, v)

[<IsReadOnly; Struct>]
type ValueOptionImpl<'T, 'D when 'D :> ISeraVision<'T>>(dep: 'D) =
    interface ISeraVision<'T ValueOption> with
        [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
        member this.Accept(visitor, value) = this.Accept visitor value

    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    member this.Accept (visitor: #ASeraVisitor<'a>) value =
        match value with
        | ValueNone -> visitor.VNone()
        | ValueSome v -> visitor.VSome(dep, v)
