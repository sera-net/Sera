namespace Sera

open System.IO
open System.Runtime.CompilerServices
open System.Text
open Sera.Core
open Sera.Core.Builders
open Sera.Core.Builders.Outputs
open Sera.Core.Builders.Ser
open Sera.FSharp.Builders

[<Extension; Sealed; AbstractClass>]
type SeraBuilderForSerFs =
    [<Extension>]
    static member inline Zero(__: #ISerBuilder SerBuilder) = __

    [<Extension>]
    static member inline Yield(__: #ISerBuilder SerBuilder, _: unit) = __

    [<Extension; CustomOperation("Serialize")>]
    static member inline Serialize(__: #ISerBuilder SerBuilder, b: #ISerBuilder SerBuilder, v: 'a) = b, v

    [<Extension; CustomOperation("Use")>]
    static member inline Use(__: #ISerBuilder SerBuilder, (b, v): #ISerBuilder SerBuilder * 'a, i: #ISeraVision<'a>) =
        (b, v, i, FsUse)

    [<Extension; CustomOperation("Static")>]
    static member inline Static(__: #ISerBuilder SerBuilder, (b, v, i, _): #ISerBuilder SerBuilder * 'a * 'i * FsUse) =
        (b, v, i, FsUse, FsStatic)

module StaticSerFs =
    [<NoCompilerInlining>]
    let SerOutputToStream<'b, 'a, 'i when 'b :> ISerBuilder and 'b :> IStreamOutput and 'i :> ISeraVision<'a>>
        (b: 'b)
        (v: 'a)
        (i: 'i)
        stream
        =
        let a = SeraBuilderForStaticSer.StaticAccept(v, i)
        b.BuildStreamOutput(a, Unchecked.defaultof<OutputBuildParam>, stream)

    [<NoCompilerInlining>]
    let SerOutputToString<'b, 'a, 'i when 'b :> ISerBuilder and 'b :> IStringBuilderOutput and 'i :> ISeraVision<'a>>
        (b: 'b)
        (v: 'a)
        (i: 'i)
        sb
        =
        let a = SeraBuilderForStaticSer.StaticAccept(v, i)
        b.BuildStringBuilderOutput(a, Unchecked.defaultof<OutputBuildParam>, sb)

    [<NoCompilerInlining>]
    let SerOutputToStreamAsync<'b, 'a, 'i when 'b :> ISerBuilder and 'b :> IAsyncStreamOutput and 'i :> ISeraVision<'a>>
        (b: 'b)
        (v: 'a)
        (i: 'i)
        stream
        =
        let a = SeraBuilderForStaticSer.AsyncStaticAccept(v, i)
        b.BuildAsyncStreamOutput(a, Unchecked.defaultof<OutputBuildParam>, stream)

[<Extension; Sealed; AbstractClass>]
type SeraBuilderForStaticSerFs =
    [<Extension; CustomOperation("ToStream")>]
    static member inline OutputToStream<'s, 'b, 'a, 'i
        when 's :> ISerBuilder and 'b :> ISerBuilder and 'b :> IStreamOutput and 'i :> ISeraVision<'a>>
        (
            __: 's SerBuilder,
            (b, v, i, _, _): 'b SerBuilder * 'a * 'i * FsUse * FsStatic,
            stream: Stream
        ) =
        StaticSerFs.SerOutputToStream b.Target v i stream

    [<Extension; CustomOperation("ToString")>]
    static member inline OutputToString<'s, 'b, 'a, 'i
        when 's :> ISerBuilder and 'b :> ISerBuilder and 'b :> IStringBuilderOutput and 'i :> ISeraVision<'a>>
        (
            __: 's SerBuilder,
            (b, v, i, _, _): 'b SerBuilder * 'a * 'i * FsUse * FsStatic,
            sb: StringBuilder
        ) =
        StaticSerFs.SerOutputToString b.Target v i sb

    [<Extension; CustomOperation("ToString")>]
    static member inline OutputToString(__: #ISerBuilder SerBuilder, b: 'b SerBuilder * 'a * 'i * FsUse * FsStatic) =
        let sb = StringBuilder()
        __.OutputToString(b, sb) |> ignore
        sb.ToString()

    [<Extension; CustomOperation("ToStreamAsync")>]
    static member inline OutputToStreamAsync<'s, 'b, 'a, 'i
        when 's :> ISerBuilder and 'b :> ISerBuilder and 'b :> IAsyncStreamOutput and 'i :> ISeraVision<'a>>
        (
            __: 's SerBuilder,
            (b, v, i, _, _): 'b SerBuilder * 'a * 'i * FsUse * FsStatic,
            stream: Stream
        ) =
        StaticSerFs.SerOutputToStreamAsync b.Target v i stream
