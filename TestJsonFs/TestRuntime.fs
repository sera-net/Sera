module TestJsonFs.TestRuntime

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

// #region TestRecord1

type Record1 = { a: string }

[<Test>]
let TestRecord1 () =
    let obj: Record1 = { a = "asd" }

    let str = SeraJson.Serializer.Serialize(obj).To.String()

    Console.WriteLine(str)

    Assert.That(str, Is.EqualTo("{\"a\":\"asd\"}"))

    ()

// #endregion

// #region TestRecord1

[<Test>]
let TestRecord2 () =
    let obj = {| a = 123 |}

    let str = SeraJson.Serializer.Serialize(obj).To.String()

    Console.WriteLine(str)

    Assert.That(str, Is.EqualTo("{\"a\":123}"))

    ()

// #endregion

// #region TestTuple1

[<Test>]
let TestTuple1 () =
    //==================================================

    let obj = Tuple.Create(1)
    let str = SeraJson.Serializer.Serialize(obj).To.String()

    Console.WriteLine(str)

    Assert.That(str, Is.EqualTo("[1]"))

    //==================================================

    let obj = (1, 2)
    let str = SeraJson.Serializer.Serialize(obj).To.String()

    Console.WriteLine(str)

    Assert.That(str, Is.EqualTo("[1,2]"))

    //==================================================

    let obj = (1, 2, 3)
    let str = SeraJson.Serializer.Serialize(obj).To.String()

    Console.WriteLine(str)

    Assert.That(str, Is.EqualTo("[1,2,3]"))

    //==================================================

    let obj = (1, 2, 3, 4)
    let str = SeraJson.Serializer.Serialize(obj).To.String()

    Console.WriteLine(str)

    Assert.That(str, Is.EqualTo("[1,2,3,4]"))

    //==================================================

    let obj = (1, 2, 3, 4, 5)
    let str = SeraJson.Serializer.Serialize(obj).To.String()

    Console.WriteLine(str)

    Assert.That(str, Is.EqualTo("[1,2,3,4,5]"))

    //==================================================

    let obj = (1, 2, 3, 4, 5, 6)
    let str = SeraJson.Serializer.Serialize(obj).To.String()

    Console.WriteLine(str)

    Assert.That(str, Is.EqualTo("[1,2,3,4,5,6]"))

    //==================================================

    let obj = (1, 2, 3, 4, 5, 6, 7)
    let str = SeraJson.Serializer.Serialize(obj).To.String()

    Console.WriteLine(str)

    Assert.That(str, Is.EqualTo("[1,2,3,4,5,6,7]"))

    //==================================================

    let obj = (1, 2, 3, 4, 5, 6, 7, 8)
    let str = SeraJson.Serializer.Serialize(obj).To.String()

    Console.WriteLine(str)

    Assert.That(str, Is.EqualTo("[1,2,3,4,5,6,7,8]"))

    //==================================================

    ()

// #endregion

// #region TestTuple1

[<Test>]
let TestValueTuple1 () =
    //==================================================

    let obj = ValueTuple.Create()
    let str = SeraJson.Serializer.Serialize(obj).To.String()

    Console.WriteLine(str)

    Assert.That(str, Is.EqualTo("[]"))

    //==================================================

    let obj = ValueTuple.Create(1)
    let str = SeraJson.Serializer.Serialize(obj).To.String()

    Console.WriteLine(str)

    Assert.That(str, Is.EqualTo("[1]"))

    //==================================================

    let obj = struct (1, 2)
    let str = SeraJson.Serializer.Serialize(obj).To.String()

    Console.WriteLine(str)

    Assert.That(str, Is.EqualTo("[1,2]"))

    //==================================================

    let obj = struct (1, 2, 3)
    let str = SeraJson.Serializer.Serialize(obj).To.String()

    Console.WriteLine(str)

    Assert.That(str, Is.EqualTo("[1,2,3]"))

    //==================================================

    let obj = struct (1, 2, 3, 4)
    let str = SeraJson.Serializer.Serialize(obj).To.String()

    Console.WriteLine(str)

    Assert.That(str, Is.EqualTo("[1,2,3,4]"))

    //==================================================

    let obj = struct (1, 2, 3, 4, 5)
    let str = SeraJson.Serializer.Serialize(obj).To.String()

    Console.WriteLine(str)

    Assert.That(str, Is.EqualTo("[1,2,3,4,5]"))

    //==================================================

    let obj = struct (1, 2, 3, 4, 5, 6)
    let str = SeraJson.Serializer.Serialize(obj).To.String()

    Console.WriteLine(str)

    Assert.That(str, Is.EqualTo("[1,2,3,4,5,6]"))

    //==================================================

    let obj = struct (1, 2, 3, 4, 5, 6, 7)
    let str = SeraJson.Serializer.Serialize(obj).To.String()

    Console.WriteLine(str)

    Assert.That(str, Is.EqualTo("[1,2,3,4,5,6,7]"))

    //==================================================

    let obj = struct (1, 2, 3, 4, 5, 6, 7, 8)
    let str = SeraJson.Serializer.Serialize(obj).To.String()

    Console.WriteLine(str)

    Assert.That(str, Is.EqualTo("[1,2,3,4,5,6,7,8]"))

    //==================================================

    ()

// #endregion

// #region TestList1

[<Test>]
let TestList1 () =
    let obj = [ 1; 2; 3 ]

    let str = SeraJson.Serializer.Serialize(obj).To.String()

    Console.WriteLine(str)

    Assert.That(str, Is.EqualTo("[1,2,3]"))

    ()

// #endregion

// #region TestSet1

[<Test>]
let TestSet1 () =
    let obj = Set [ 1; 2; 3 ]

    let str = SeraJson.Serializer.Serialize(obj).To.String()

    Console.WriteLine(str)

    Assert.That(str, Is.EqualTo("[1,2,3]"))

    ()

// #endregion

// #region TestOption1

[<Test>]
let TestOption1 () =
    let obj: int option = None

    let str = SeraJson.Serializer.Serialize(obj).To.String()

    Console.WriteLine(str)

    Assert.That(str, Is.EqualTo("null"))

    ()

// #endregion

// #region TestOption2

[<Test>]
let TestOption2 () =
    let obj: int option = Some 1

    let str = SeraJson.Serializer.Serialize(obj).To.String()

    Console.WriteLine(str)

    Assert.That(str, Is.EqualTo("1"))

    ()

// #endregion

// #region TestPrivateOption1

type private TypeTestPrivateOption1 =
    new() = TypeTestPrivateOption1()

[<Test>]
let TestPrivateOption1 () =
    let obj: TypeTestPrivateOption1 option = None

    let str = SeraJson.Serializer.Serialize(obj).To.String()

    Console.WriteLine(str)

    Assert.That(str, Is.EqualTo("null"))

    ()

// #endregion

// #region TestPrivateOption2

type private TypeTestPrivateOption2 =
    new() = TypeTestPrivateOption2()

[<Test>]
let TestPrivateOption2 () =
    let obj = Some(TypeTestPrivateOption2)

    let str = SeraJson.Serializer.Serialize(obj).To.String()

    Console.WriteLine(str)

    Assert.That(str, Is.EqualTo("{}"))

    ()

// #endregion

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
let TestTypeTestUnion1 () =
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
let TestTypeTestUnion2 () =
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
let TestTypeTestUnion3 () =
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
let TestTypeTestUnion4 () =
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
let TestTypeTestUnion5 () =
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
let TestTypeTestUnion6 () =
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
