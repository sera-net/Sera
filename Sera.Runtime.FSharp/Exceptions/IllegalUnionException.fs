namespace Sera.Runtime.FSharp.Exceptions

open System
open Sera.Core

type IllegalUnionException =
    inherit SeraSerFailureException

    val Type: Type

    new(typ: Type, msg: string, inner: Exception) =
        { inherit SeraSerFailureException(msg, inner)
          Type = typ }

    new(typ: Type, msg: string) =
        { inherit SeraSerFailureException(msg)
          Type = typ }

    new(typ: Type) =
        { inherit SeraSerFailureException()
          Type = typ }
