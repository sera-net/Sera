module TestJsonFs.Runtime.TestUnion

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

// #region TestUnion1

[<SeraUnion(Format = UnionFormat.External)>]
type TypeTestUnion1 =
    | TypeTestUnion1_A_1
    | [<SeraVariant(Priority = VariantPriority.TagFirst)>] TypeTestUnion1_A_2
    | TypeTestUnion1_B_1 of int
    | [<SeraVariant(Format = VariantFormat.Tuple)>] TypeTestUnion1_B_2 of int
    | [<SeraVariant(Format = VariantFormat.Struct)>] TypeTestUnion1_B_3 of int
    | TypeTestUnion1_C_1 of int * int
    | TypeTestUnion1_D_1 of a: int * b: int
    | TypeTestUnion1_E_1 of a: int * int

[<Test>]
let TestUnion1 () =
    let obj = TypeTestUnion1_A_1
    let str = SeraJson.Serializer.Serialize(obj).To.String()
    Console.WriteLine(str)
    Assert.That(str, Is.EqualTo("\"TypeTestUnion1_A_1\""))

    // ==================================================

    let obj = TypeTestUnion1_A_2
    let str = SeraJson.Serializer.Serialize(obj).To.String()
    Console.WriteLine(str)
    Assert.That(str, Is.EqualTo("1"))

    // ==================================================

    let obj = TypeTestUnion1_B_1(123)
    let str = SeraJson.Serializer.Serialize(obj).To.String()
    Console.WriteLine(str)
    Assert.That(str, Is.EqualTo("{\"TypeTestUnion1_B_1\":123}"))

    // ==================================================

    let obj = TypeTestUnion1_B_2(123)
    let str = SeraJson.Serializer.Serialize(obj).To.String()
    Console.WriteLine(str)
    Assert.That(str, Is.EqualTo("{\"TypeTestUnion1_B_2\":[123]}"))

    // ==================================================

    let obj = TypeTestUnion1_B_3(123)
    let str = SeraJson.Serializer.Serialize(obj).To.String()
    Console.WriteLine(str)
    Assert.That(str, Is.EqualTo("{\"TypeTestUnion1_B_3\":{\"Item\":123}}"))

    // ==================================================

    let obj = TypeTestUnion1_C_1(123, 456)
    let str = SeraJson.Serializer.Serialize(obj).To.String()
    Console.WriteLine(str)
    Assert.That(str, Is.EqualTo("{\"TypeTestUnion1_C_1\":[123,456]}"))

    // ==================================================

    let obj = TypeTestUnion1_D_1(123, 456)
    let str = SeraJson.Serializer.Serialize(obj).To.String()
    Console.WriteLine(str)
    Assert.That(str, Is.EqualTo("{\"TypeTestUnion1_D_1\":{\"a\":123,\"b\":456}}"))

    // ==================================================

    let obj = TypeTestUnion1_E_1(123, 456)
    let str = SeraJson.Serializer.Serialize(obj).To.String()
    Console.WriteLine(str)
    Assert.That(str, Is.EqualTo("{\"TypeTestUnion1_E_1\":{\"a\":123,\"Item2\":456}}"))

    // ==================================================
    ()

// #endregion

// #region TestUnion2

[<Struct; SeraUnion(Format = UnionFormat.External)>]
type TypeTestUnion2 =
    | TypeTestUnion2_A_1
    | [<SeraVariant(Priority = VariantPriority.TagFirst)>] TypeTestUnion2_A_2
    | [<SeraVariant(Format = VariantFormat.Value)>] TypeTestUnion2_B_1 of xxx: int
    | [<SeraVariant(Format = VariantFormat.Tuple)>] TypeTestUnion2_B_2 of yyy: int
    | [<SeraVariant(Format = VariantFormat.Struct)>] TypeTestUnion2_B_3 of zzz: int
    | [<SeraVariant(Format = VariantFormat.Tuple)>] TypeTestUnion2_C_1 of u: int * v: int
    | TypeTestUnion2_D_1 of a: int * b: int
    | TypeTestUnion2_E_1 of c: int * int

[<Test>]
let TestUnion2 () =
    let obj = TypeTestUnion2_A_1
    let str = SeraJson.Serializer.Serialize(obj).To.String()
    Console.WriteLine(str)
    Assert.That(str, Is.EqualTo("\"TypeTestUnion2_A_1\""))

    // ==================================================

    let obj = TypeTestUnion2_A_2
    let str = SeraJson.Serializer.Serialize(obj).To.String()
    Console.WriteLine(str)
    Assert.That(str, Is.EqualTo("1"))

    // ==================================================

    let obj = TypeTestUnion2_B_1(123)
    let str = SeraJson.Serializer.Serialize(obj).To.String()
    Console.WriteLine(str)
    Assert.That(str, Is.EqualTo("{\"TypeTestUnion2_B_1\":123}"))

    // ==================================================

    let obj = TypeTestUnion2_B_2(123)
    let str = SeraJson.Serializer.Serialize(obj).To.String()
    Console.WriteLine(str)
    Assert.That(str, Is.EqualTo("{\"TypeTestUnion2_B_2\":[123]}"))

    // ==================================================

    let obj = TypeTestUnion2_B_3(123)
    let str = SeraJson.Serializer.Serialize(obj).To.String()
    Console.WriteLine(str)
    Assert.That(str, Is.EqualTo("{\"TypeTestUnion2_B_3\":{\"zzz\":123}}"))

    // ==================================================

    let obj = TypeTestUnion2_C_1(123, 456)
    let str = SeraJson.Serializer.Serialize(obj).To.String()
    Console.WriteLine(str)
    Assert.That(str, Is.EqualTo("{\"TypeTestUnion2_C_1\":[123,456]}"))

    // ==================================================

    let obj = TypeTestUnion2_D_1(123, 456)
    let str = SeraJson.Serializer.Serialize(obj).To.String()
    Console.WriteLine(str)
    Assert.That(str, Is.EqualTo("{\"TypeTestUnion2_D_1\":{\"a\":123,\"b\":456}}"))

    // ==================================================

    let obj = TypeTestUnion2_E_1(123, 456)
    let str = SeraJson.Serializer.Serialize(obj).To.String()
    Console.WriteLine(str)
    Assert.That(str, Is.EqualTo("{\"TypeTestUnion2_E_1\":{\"c\":123,\"Item2\":456}}"))

    // ==================================================
    ()

// #endregion

// #region TestUnion3

[<SeraUnion(Format = UnionFormat.Internal)>]
type TypeTestUnion3 =
    | TypeTestUnion3_A_1
    | [<SeraVariant(Priority = VariantPriority.TagFirst)>] TypeTestUnion3_A_2
    | TypeTestUnion3_B_1 of int
    | TypeTestUnion3_C_1 of int * int
    | TypeTestUnion3_D_1 of a: int * b: int
    | TypeTestUnion3_E_1 of a: int * int

[<Test>]
let TestUnion3 () =
    let obj = TypeTestUnion3_A_1
    let str = SeraJson.Serializer.Serialize(obj).To.String()
    Console.WriteLine(str)
    Assert.That(str, Is.EqualTo("{\"type\":\"TypeTestUnion3_A_1\"}"))

    // ==================================================

    let obj = TypeTestUnion3_A_2
    let str = SeraJson.Serializer.Serialize(obj).To.String()
    Console.WriteLine(str)
    Assert.That(str, Is.EqualTo("{\"type\":1}"))

    // ==================================================

    let obj = TypeTestUnion3_B_1(123)
    let str = SeraJson.Serializer.Serialize(obj).To.String()
    Console.WriteLine(str)
    Assert.That(str, Is.EqualTo("{\"type\":\"TypeTestUnion3_B_1\",\"value\":123}"))

    // ==================================================

    let obj = TypeTestUnion3_C_1(123, 456)
    let str = SeraJson.Serializer.Serialize(obj).To.String()
    Console.WriteLine(str)
    Assert.That(str, Is.EqualTo("{\"type\":\"TypeTestUnion3_C_1\",\"value\":[123,456]}"))

    // ==================================================

    let obj = TypeTestUnion3_D_1(123, 456)
    let str = SeraJson.Serializer.Serialize(obj).To.String()
    Console.WriteLine(str)
    Assert.That(str, Is.EqualTo("{\"type\":\"TypeTestUnion3_D_1\",\"a\":123,\"b\":456}"))

    // ==================================================
    ()

// #endregion

// #region TestUnion4

[<SeraUnion(Format = UnionFormat.Adjacent)>]
type TypeTestUnion4 =
    | TypeTestUnion4_A_1
    | [<SeraVariant(Priority = VariantPriority.TagFirst)>] TypeTestUnion4_A_2
    | TypeTestUnion4_B_1 of int
    | TypeTestUnion4_C_1 of int * int
    | TypeTestUnion4_D_1 of a: int * b: int
    | TypeTestUnion4_E_1 of a: int * int

[<Test>]
let TestUnion4 () =
    let obj = TypeTestUnion4_A_1
    let str = SeraJson.Serializer.Serialize(obj).To.String()
    Console.WriteLine(str)
    Assert.That(str, Is.EqualTo("{\"t\":\"TypeTestUnion4_A_1\"}"))

    // ==================================================

    let obj = TypeTestUnion4_A_2
    let str = SeraJson.Serializer.Serialize(obj).To.String()
    Console.WriteLine(str)
    Assert.That(str, Is.EqualTo("{\"t\":1}"))

    // ==================================================

    let obj = TypeTestUnion4_B_1(123)
    let str = SeraJson.Serializer.Serialize(obj).To.String()
    Console.WriteLine(str)
    Assert.That(str, Is.EqualTo("{\"t\":\"TypeTestUnion4_B_1\",\"c\":123}"))

    // ==================================================

    let obj = TypeTestUnion4_C_1(123, 456)
    let str = SeraJson.Serializer.Serialize(obj).To.String()
    Console.WriteLine(str)
    Assert.That(str, Is.EqualTo("{\"t\":\"TypeTestUnion4_C_1\",\"c\":[123,456]}"))

    // ==================================================

    let obj = TypeTestUnion4_D_1(123, 456)
    let str = SeraJson.Serializer.Serialize(obj).To.String()
    Console.WriteLine(str)
    Assert.That(str, Is.EqualTo("{\"t\":\"TypeTestUnion4_D_1\",\"c\":{\"a\":123,\"b\":456}}"))

    // ==================================================
    ()

// #endregion

// #region TestUnion5

[<SeraUnion(Format = UnionFormat.Tuple)>]
type TypeTestUnion5 =
    | TypeTestUnion5_A_1
    | [<SeraVariant(Priority = VariantPriority.TagFirst)>] TypeTestUnion5_A_2
    | TypeTestUnion5_B_1 of int
    | TypeTestUnion5_C_1 of int * int
    | TypeTestUnion5_D_1 of a: int * b: int
    | TypeTestUnion5_E_1 of a: int * int

[<Test>]
let TestUnion5 () =
    let obj = TypeTestUnion5_A_1
    let str = SeraJson.Serializer.Serialize(obj).To.String()
    Console.WriteLine(str)
    Assert.That(str, Is.EqualTo("[\"TypeTestUnion5_A_1\"]"))

    // ==================================================

    let obj = TypeTestUnion5_A_2
    let str = SeraJson.Serializer.Serialize(obj).To.String()
    Console.WriteLine(str)
    Assert.That(str, Is.EqualTo("[1]"))

    // ==================================================

    let obj = TypeTestUnion5_B_1(123)
    let str = SeraJson.Serializer.Serialize(obj).To.String()
    Console.WriteLine(str)
    Assert.That(str, Is.EqualTo("[\"TypeTestUnion5_B_1\",123]"))

    // ==================================================

    let obj = TypeTestUnion5_C_1(123, 456)
    let str = SeraJson.Serializer.Serialize(obj).To.String()
    Console.WriteLine(str)
    Assert.That(str, Is.EqualTo("[\"TypeTestUnion5_C_1\",[123,456]]"))

    // ==================================================

    let obj = TypeTestUnion5_D_1(123, 456)
    let str = SeraJson.Serializer.Serialize(obj).To.String()
    Console.WriteLine(str)
    Assert.That(str, Is.EqualTo("[\"TypeTestUnion5_D_1\",{\"a\":123,\"b\":456}]"))

    // ==================================================
    ()

// #endregion

// #region TestUnion6

[<SeraUnion(Format = UnionFormat.Untagged)>]
type TypeTestUnion6 =
    | TypeTestUnion6_A_1
    | [<SeraVariant(Priority = VariantPriority.TagFirst)>] TypeTestUnion6_A_2
    | TypeTestUnion6_B_1 of int
    | TypeTestUnion6_C_1 of int * int
    | TypeTestUnion6_D_1 of a: int * b: int
    | TypeTestUnion6_E_1 of a: int * int

[<Test>]
let TestUnion6 () =
    let obj = TypeTestUnion6_A_1
    let str = SeraJson.Serializer.Serialize(obj).To.String()
    Console.WriteLine(str)
    Assert.That(str, Is.EqualTo("\"TypeTestUnion6_A_1\""))

    // ==================================================

    let obj = TypeTestUnion6_A_2
    let str = SeraJson.Serializer.Serialize(obj).To.String()
    Console.WriteLine(str)
    Assert.That(str, Is.EqualTo("1"))

    // ==================================================

    let obj = TypeTestUnion6_B_1(123)
    let str = SeraJson.Serializer.Serialize(obj).To.String()
    Console.WriteLine(str)
    Assert.That(str, Is.EqualTo("123"))

    // ==================================================

    let obj = TypeTestUnion6_C_1(123, 456)
    let str = SeraJson.Serializer.Serialize(obj).To.String()
    Console.WriteLine(str)
    Assert.That(str, Is.EqualTo("[123,456]"))

    // ==================================================

    let obj = TypeTestUnion6_D_1(123, 456)
    let str = SeraJson.Serializer.Serialize(obj).To.String()
    Console.WriteLine(str)
    Assert.That(str, Is.EqualTo("{\"a\":123,\"b\":456}"))

    // ==================================================
    ()

// #endregion
