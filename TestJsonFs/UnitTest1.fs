module TestJsonFs.UnitTest1

open System
open System.IO
open System.Text
open System.Threading.Tasks
open NUnit.Framework
open Sera
open Sera.Core.Impls.Ser
open Sera.Json
open Sera.Runtime
open Sera.Runtime.FSharp

[<SetUp>]
let Setup () =
    SeraRuntime.Reg(SeraRuntimeFSharp.Instance)
    ()

[<SeraGen; SeraStruct(IncludeFields = true)>]
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

[<Test>]
let Test3 () =
    task {
        use stream = new MemoryStream()

        do!
            SeraJson.Serializer {
                Serialize 123
                Use(PrimitiveImpl())
                Static
                ToStreamAsync stream
            }

        stream.Position <- 0
        use reader = new StreamReader(stream, Encoding.UTF8)
        let str = reader.ReadToEnd()

        Console.WriteLine(str)

        Assert.That(str, Is.EqualTo("123"))
    }
    :> Task
