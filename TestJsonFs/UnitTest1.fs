module TestJsonFs.UnitTest1

open System
open NUnit.Framework
open Sera
open Sera.Core.Impls.Ser
open Sera.Json
open Sera.Runtime

[<SetUp>]
let Setup () = ()

[<SeraGen; SeraIncludeField>]
type UnionForAttrs1 = UnionForAttrs1

[<Test>]
let Test1 () =
    let str =
        SeraJson.Serializer {
            Serialize 123
            Use(PrimitiveImpl())
            Static
            ToString
        }

    Console.WriteLine(str)

    Assert.That(str, Is.EqualTo("123"))

    ()

[<Test>]
let Test2 () =

    let str =
        SeraJson.Serializer {
            Serialize [| 1uy; 2uy; 3uy |]
            Styles(SeraStyles(As = SeraAs.Bytes))
            ToString
        }

    Console.WriteLine(str)

    Assert.That(str, Is.EqualTo("\"AQID\""))

    ()
