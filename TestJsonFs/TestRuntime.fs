module TestJsonFs.TestRuntime

open System
open NUnit.Framework
open Sera.Json
open Sera.Json.Runtime

[<SetUp>]
let Setup () = ()

// #region TestTuple1

[<Test>]
let TestTuple1 () =
    //==================================================
    
    let obj = Tuple.Create(1)
    let str = SeraJson.Serializer.ToString().Serialize(obj)

    Console.WriteLine(str)

    Assert.That(str, Is.EqualTo("[1]"))
    
    //==================================================

    let obj = (1, 2)
    let str = SeraJson.Serializer.ToString().Serialize(obj)

    Console.WriteLine(str)

    Assert.That(str, Is.EqualTo("[1,2]"))

    //==================================================
    
    let obj = (1, 2, 3)
    let str = SeraJson.Serializer.ToString().Serialize(obj)

    Console.WriteLine(str)

    Assert.That(str, Is.EqualTo("[1,2,3]"))
    
    //==================================================

    let obj = (1, 2, 3, 4)
    let str = SeraJson.Serializer.ToString().Serialize(obj)

    Console.WriteLine(str)

    Assert.That(str, Is.EqualTo("[1,2,3,4]"))
    
    //==================================================
    
    let obj = (1, 2, 3, 4, 5)
    let str = SeraJson.Serializer.ToString().Serialize(obj)

    Console.WriteLine(str)

    Assert.That(str, Is.EqualTo("[1,2,3,4,5]"))
    
    //==================================================
    
    let obj = (1, 2, 3, 4, 5, 6)
    let str = SeraJson.Serializer.ToString().Serialize(obj)

    Console.WriteLine(str)

    Assert.That(str, Is.EqualTo("[1,2,3,4,5,6]"))
    
    //==================================================
    
    let obj = (1, 2, 3, 4, 5, 6, 7)
    let str = SeraJson.Serializer.ToString().Serialize(obj)

    Console.WriteLine(str)

    Assert.That(str, Is.EqualTo("[1,2,3,4,5,6,7]"))
    
    //==================================================
        
    let obj = (1, 2, 3, 4, 5, 6, 7, 8)
    let str = SeraJson.Serializer.ToString().Serialize(obj)

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
    let str = SeraJson.Serializer.ToString().Serialize(obj)

    Console.WriteLine(str)

    Assert.That(str, Is.EqualTo("[]"))
    
    //==================================================
    
    let obj = ValueTuple.Create(1)
    let str = SeraJson.Serializer.ToString().Serialize(obj)

    Console.WriteLine(str)

    Assert.That(str, Is.EqualTo("[1]"))
    
    //==================================================

    let obj = struct(1, 2)
    let str = SeraJson.Serializer.ToString().Serialize(obj)

    Console.WriteLine(str)

    Assert.That(str, Is.EqualTo("[1,2]"))

    //==================================================
    
    let obj = struct(1, 2, 3)
    let str = SeraJson.Serializer.ToString().Serialize(obj)

    Console.WriteLine(str)

    Assert.That(str, Is.EqualTo("[1,2,3]"))
    
    //==================================================

    let obj = struct(1, 2, 3, 4)
    let str = SeraJson.Serializer.ToString().Serialize(obj)

    Console.WriteLine(str)

    Assert.That(str, Is.EqualTo("[1,2,3,4]"))
    
    //==================================================
    
    let obj = struct(1, 2, 3, 4, 5)
    let str = SeraJson.Serializer.ToString().Serialize(obj)

    Console.WriteLine(str)

    Assert.That(str, Is.EqualTo("[1,2,3,4,5]"))
    
    //==================================================
    
    let obj = struct(1, 2, 3, 4, 5, 6)
    let str = SeraJson.Serializer.ToString().Serialize(obj)

    Console.WriteLine(str)

    Assert.That(str, Is.EqualTo("[1,2,3,4,5,6]"))
    
    //==================================================
    
    let obj = struct(1, 2, 3, 4, 5, 6, 7)
    let str = SeraJson.Serializer.ToString().Serialize(obj)

    Console.WriteLine(str)

    Assert.That(str, Is.EqualTo("[1,2,3,4,5,6,7]"))
    
    //==================================================
        
    let obj = struct(1, 2, 3, 4, 5, 6, 7, 8)
    let str = SeraJson.Serializer.ToString().Serialize(obj)

    Console.WriteLine(str)

    Assert.That(str, Is.EqualTo("[1,2,3,4,5,6,7,8]"))
    
    //==================================================
    
    ()

// #endregion
