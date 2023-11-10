module TestJsonFs.Runtime.TestUnionRename

open System
open NUnit.Framework
open Sera
open Sera.Core
open Sera.Json
open Sera.Runtime
open Sera.Runtime.FSharp

[<SetUp>]
let Setup () =
    SeraRuntime.Reg(SeraRuntimeFSharp.Instance)
    ()

[<SeraUnion(Format = UnionFormat.External); Sera(Rename = SeraRenameMode.camelCase)>]
type TypeTestUnion1 =
    | TypeTestUnion1_A_1
    | [<Sera(Rename = SeraRenameMode.kebab_case)>] TypeTestUnion1_B_1

[<Test>]
let TestUnion1 () =
    let obj = TypeTestUnion1_A_1
    let str = SeraJson.Serializer.Serialize(obj).To.String()
    Console.WriteLine(str)
    Assert.That(str, Is.EqualTo("\"typeTestUnion1A1\""))
    
    // ==================================================
    
    let obj = TypeTestUnion1_B_1
    let str = SeraJson.Serializer.Serialize(obj).To.String()
    Console.WriteLine(str)
    Assert.That(str, Is.EqualTo("\"type-test-union1-b-1\""))

    // ==================================================
    ()
