module TestJsonFs.TestRuntime

open System
open NUnit.Framework
open Sera
open Sera.Json
open Sera.Runtime

[<SetUp>]
let Setup () = ()

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
