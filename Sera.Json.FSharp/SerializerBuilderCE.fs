namespace Sera.Json

open System.IO
open System.Runtime.CompilerServices
open System.Text
open Sera.Json.Builders
open Sera.Json.Builders.Ser
open Sera.Json.Runtime
open Sera.Runtime
open Sera.Json.Marks

[<Extension>]
type SerializerBuilderEx =
    [<Extension>]
    static member inline Zero(__: SerializerBuilder) = __

    [<Extension>]
    static member inline Yield(__: SerializerBuilder, _: unit) = ()

    [<Extension; CustomOperation("ToStream")>]
    static member inline MarkToString(__: SerializerBuilder, stream: Stream) = __.ToStream(stream)

    [<Extension; CustomOperation("ToString")>]
    static member inline MarkToString(__: SerializerBuilder, _: unit) = __.ToString()

    [<Extension; CustomOperation("ToString")>]
    static member inline MarkToString(__: SerializerBuilder, sb: StringBuilder) = __.ToString(sb)

    [<Extension; CustomOperation("Use")>]
    static member inline MarkUseSerialize(__: SerializerBuilder, b: Builder<'B>, serialize) =
        (b, serialize, MarkWithSerialize)

    [<Extension; CustomOperation("BySelf")>]
    static member inline MarkSerializeBySelf(__: SerializerBuilder, b: Builder<'B>) = (b, MarkBySelf)

    [<Extension; CustomOperation("Static")>]
    static member inline MarkStaticSerialize(__: SerializerBuilder, b: Builder<'B>) = (b, MarkStatic)

    [<Extension; CustomOperation("Static")>]
    static member inline MarkStaticSerialize(__: SerializerBuilder, (b, s, _): Builder<'B> * 'S * MarkWithSerialize) =
        (b, s, MarkWithSerialize, MarkStatic)


    [<Extension; CustomOperation("Static")>]
    static member inline MarkStaticSerialize(__: SerializerBuilder, (b, _): Builder<'B> * MarkBySelf) =
        (b, MarkBySelf, MarkStatic)

[<Extension>]
type SerializerBuilderStaticEx =

    [<Extension; CustomOperation("SerializeStatic")>]
    static member inline DoSerializeStatic(__: SerializerBuilder, builder: Builder<ToStream>, serialize, value) =
        builder.SerializeStatic(value, serialize)

    [<Extension; CustomOperation("SerializeStatic")>]
    static member inline DoSerializeStatic(__: SerializerBuilder, builder: Builder<ToString>, serialize, value) =
        builder.SerializeStatic(value, serialize)

    [<Extension; CustomOperation("SerializeStatic")>]
    static member inline DoSerializeStatic(__: SerializerBuilder, builder: Builder<ToStringBuilder>, serialize, value) =
        builder.SerializeStatic(value, serialize)

    [<Extension; CustomOperation("Serialize")>]
    static member inline DoSerialize
        (
            __: SerializerBuilder,
            (b, s, _, _): Builder<ToStream> * 'S * MarkWithSerialize * MarkStatic,
            MarkStatic,
            value: 'V
        ) =
        b.SerializeStatic(value, s)

    [<Extension; CustomOperation("Serialize")>]
    static member inline DoSerialize
        (
            __: SerializerBuilder,
            (b, s, _, _): Builder<ToString> * 'S * MarkWithSerialize * MarkStatic,
            value: 'V
        ) =
        b.SerializeStatic(value, s)

    [<Extension; CustomOperation("Serialize")>]
    static member inline DoSerialize
        (
            __: SerializerBuilder,
            (b, s, _, _): Builder<ToStringBuilder> * 'S * MarkWithSerialize * MarkStatic,
            value: 'V
        ) =
        b.SerializeStatic(value, s)

    [<Extension; CustomOperation("Serialize")>]
    static member inline DoSerialize
        (
            __: SerializerBuilder,
            (b, _, _): Builder<ToStream> * MarkBySelf * MarkStatic,
            value
        ) =
        b.SerializeStatic<'T>(value)

    [<Extension; CustomOperation("Serialize")>]
    static member inline DoSerialize
        (
            __: SerializerBuilder,
            (b, _, _): Builder<ToStream> * MarkBySelf * MarkStatic,
            value
        ) =
        b.SerializeStatic<'T, 'S>(value)

    [<Extension; CustomOperation("Serialize")>]
    static member inline DoSerialize
        (
            __: SerializerBuilder,
            (b, _, _): Builder<ToString> * MarkBySelf * MarkStatic,
            value
        ) =
        b.SerializeStatic<'T>(value)

    [<Extension; CustomOperation("Serialize")>]
    static member inline DoSerialize
        (
            __: SerializerBuilder,
            (b, _, _): Builder<ToString> * MarkBySelf * MarkStatic,
            value
        ) =
        b.SerializeStatic<'T, 'S>(value)

    [<Extension; CustomOperation("Serialize")>]
    static member inline DoSerialize
        (
            __: SerializerBuilder,
            (b, _, _): Builder<ToStringBuilder> * MarkBySelf * MarkStatic,
            value
        ) =
        b.SerializeStatic<'T>(value)

    [<Extension; CustomOperation("Serialize")>]
    static member inline DoSerialize
        (
            __: SerializerBuilder,
            (b, _, _): Builder<ToStringBuilder> * MarkBySelf * MarkStatic,
            value
        ) =
        b.SerializeStatic<'T, 'S>(value)

    [<Extension; CustomOperation("Serialize")>]
    static member inline DoSerialize
        (
            __: SerializerBuilder,
            (b, s, _): Builder<ToStream> * 'S * MarkWithSerialize,
            MarkStatic,
            value: 'V
        ) =
        b.Serialize(value, s)

    [<Extension; CustomOperation("Serialize")>]
    static member inline DoSerialize
        (
            __: SerializerBuilder,
            (b, s, _): Builder<ToString> * 'S * MarkWithSerialize,
            value: 'V
        ) =
        b.Serialize(value, s)

    [<Extension; CustomOperation("Serialize")>]
    static member inline DoSerialize
        (
            __: SerializerBuilder,
            (b, s, _): Builder<ToStringBuilder> * 'S * MarkWithSerialize,
            value: 'V
        ) =
        b.Serialize(value, s)

[<Extension>]
type SerializerBuilderRuntimeEx =
    [<Extension; CustomOperation("Hints")>]
    static member inline MarkUseSerialize(__: SerializerBuilder, builder: Builder<'B>, hint: SeraHints) =
        (builder, hint, MarkWithHints)

    [<Extension; CustomOperation("Serialize")>]
    static member inline DoSerialize(__: SerializerBuilder, builder: Builder<ToStream>, value: 'V) =
        builder.Serialize(value)

    [<Extension; CustomOperation("Serialize")>]
    static member inline DoSerialize(__: SerializerBuilder, builder: Builder<ToString>, value: 'V) =
        builder.Serialize(value)

    [<Extension; CustomOperation("Serialize")>]
    static member inline DoSerialize(__: SerializerBuilder, builder: Builder<ToStringBuilder>, value: 'V) =
        builder.Serialize(value)

    [<Extension; CustomOperation("Serialize")>]
    static member inline DoSerialize(__: SerializerBuilder, builder: Builder<ToStream>, hint: SeraHints, value: 'V) =
        builder.Serialize(value, hint)

    [<Extension; CustomOperation("Serialize")>]
    static member inline DoSerialize(__: SerializerBuilder, builder: Builder<ToString>, hint: SeraHints, value: 'V) =
        builder.Serialize(value, hint)

    [<Extension; CustomOperation("Serialize")>]
    static member inline DoSerialize
        (
            __: SerializerBuilder,
            builder: Builder<ToStringBuilder>,
            hint: SeraHints,
            value: 'V
        ) =
        builder.Serialize(value, hint)

    [<Extension; CustomOperation("Serialize")>]
    static member inline DoSerialize
        (
            __: SerializerBuilder,
            (b, h, _): Builder<ToStream> * SeraHints * MarkWithHints,
            value: 'V
        ) =
        b.Serialize(value, h)

    [<Extension; CustomOperation("Serialize")>]
    static member inline DoSerialize
        (
            __: SerializerBuilder,
            (b, h, _): Builder<ToString> * SeraHints * MarkWithHints,
            value: 'V
        ) =
        b.Serialize(value, h)

    [<Extension; CustomOperation("Serialize")>]
    static member inline DoSerialize
        (
            __: SerializerBuilder,
            (b, h, _): Builder<ToStringBuilder> * SeraHints * MarkWithHints,
            value: 'V
        ) =
        b.Serialize(value, h)
