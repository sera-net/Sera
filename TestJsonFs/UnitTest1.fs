module TestJsonFs.UnitTest1

open System
open NUnit.Framework
open Sera
open Sera.Core
open Sera.Core.Impls
open Sera.Core.Ser
open Sera.Json
open Sera.Runtime

[<SetUp>]
let Setup () = ()

[<SeraGen; SeraGenDe; SeraGenSer; SeraIncludeField>]
type UnionForAttrs1 = UnionForAttrs1

[<Test>]
let Test1 () =
    let str =
        SeraJson.Serializer {
            ToString
            Use PrimitiveImpls.Int32
            Static
            Serialize 123
        }

    Console.WriteLine(str)

    Assert.That(str, Is.EqualTo("123"))

    ()

[<Test>]
let Test2 () =

    let str =
        SeraJson.Serializer {
            ToString
            Hints(SeraHints(As = SeraAs.Bytes))
            Serialize [| 1uy; 2uy; 3uy |]
        }

    Console.WriteLine(str)

    Assert.That(str, Is.EqualTo("\"AQID\""))

    ()

[<Test>]
let Test3 () =
    let str =
        SeraJson.Serializer {
            ToString
            BySelf
            Static
            Serialize(SeraAny.MakeUnit())
        }

    Console.WriteLine(str)

    Assert.That(str, Is.EqualTo("null"))

    ()
