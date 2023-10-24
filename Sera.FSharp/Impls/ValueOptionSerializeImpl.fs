namespace Sera.FSharp.Impls

open System.Runtime.CompilerServices
open Microsoft.FSharp.Core
open Sera
open Sera.Core
open Sera.Core.Ser

[<AbstractClass>]
type ValueOptionSerializeImplBase<'T>() =
    interface ISerialize<'T ValueOption> with
        [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
        member this.Write(serializer, value, options) = this.Write serializer value options

    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    abstract member Write<'S when 'S :> ISerializer> :
        serializer: 'S -> value: 'T ValueOption -> options: ISeraOptions -> unit

[<Sealed>]
type ValueOptionSerializeImpl<'T, 'ST when 'ST :> ISerialize<'T>>(ser: 'ST) =
    inherit ValueOptionSerializeImplBase<'T>()

    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    override this.Write serializer value options =
        match value with
        | ValueNone -> serializer.WriteNone()
        | ValueSome v -> serializer.WriteSome(v, ser)

[<IsReadOnly; Struct; NoEquality; NoComparison>]
type ValueOptionSerializeImplWrapper<'T>(ser: 'T ValueOptionSerializeImplBase) =
    interface ISerialize<'T ValueOption> with
        [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
        member this.Write(serializer, value, options) = this.Write serializer value options

    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    member this.Write<'S when 'S :> ISerializer> (serializer: 'S) value options = ser.Write serializer value options
