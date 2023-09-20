module TestJsonFs.UnitTest1

open NUnit.Framework
open Sera

[<SetUp>]
let Setup () = ()

[<SeraGen; SeraGenDe; SeraGenSer; SeraIncludeField>]
type UnionForAttrs1 = UnionForAttrs1

[<Test>]
let Test1 () = ()
