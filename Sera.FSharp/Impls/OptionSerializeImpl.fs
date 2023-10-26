namespace Sera.FSharp.Impls

open System.Runtime.CompilerServices
open Sera
open Sera.Core
open Sera.Core.Ser

[<AbstractClass>]
type OptionSerializeImplBase<'T>() =
    interface ISerialize<'T option> with
        [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
        member this.Write(serializer, value, options) = this.Write serializer value options

    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    abstract member Write<'S when 'S :> ISerializer> :
        serializer: 'S -> value: 'T option -> options: ISeraOptions -> unit

[<Sealed>]
type OptionSerializeImpl<'T, 'ST when 'ST :> ISerialize<'T>>(ser: 'ST) =
    inherit OptionSerializeImplBase<'T>()

    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    override this.Write serializer value options =
        match value with
        | None -> serializer.WriteNone()
        | Some v -> serializer.WriteSome(v, ser)

[<IsReadOnly; Struct; NoEquality; NoComparison>]
type OptionSerializeImplWrapper<'T>(ser: 'T OptionSerializeImplBase) =
    interface ISerialize<'T option> with
        [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
        member this.Write(serializer, value, options) = this.Write serializer value options

    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    member this.Write<'S when 'S :> ISerializer> (serializer: 'S) value options = ser.Write serializer value options
