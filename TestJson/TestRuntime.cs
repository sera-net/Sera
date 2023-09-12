using JetBrains.Annotations;
using Sera;
using Sera.Json;
using Sera.Json.Runtime;

namespace TestJson;

public class TestRuntime
{
    [SetUp]
    public void Setup() { }

    #region EmptyStruct1

    public class EmptyStruct1 { }

    [Test]
    public void TestEmptyStruct1()
    {
        var obj = new EmptyStruct1();

        var str = SeraJson.Serializer
            .ToString()
            .Serialize(obj);

        Console.WriteLine(str);
        Assert.That(str, Is.EqualTo("{}"));
    }

    #endregion

    #region Struct1

    [SeraIncludeField]
    public class Struct1
    {
        public int Member1 { get; set; } = 123456;
        public int Member2 = 654321;
    }

    [Test]
    public void TestStruct1()
    {
        var obj = new Struct1();

        var str = SeraJson.Serializer
            .ToString()
            .Serialize(obj);

        Console.WriteLine(str);
        Assert.That(str, Is.EqualTo("{\"Member1\":123456,\"Member2\":654321}"));
    }

    #endregion

    #region Struct2

    public class Struct2
    {
        public Inner Member1 { get; set; } = new();

        public class Inner
        {
            public int Member1 { get; set; } = 123456;
        }
    }

    [Test]
    public void TestStruct2()
    {
        var obj = new Struct2();

        var str = SeraJson.Serializer
            .ToString()
            .Serialize(obj);

        Console.WriteLine(str);
        Assert.That(str, Is.EqualTo("{\"Member1\":{\"Member1\":123456}}"));
    }

    #endregion

    #region Struct3

    public class Struct3
    {
        [SeraRename(1)]
        public int Member1 { get; set; } = 123456;
        [SeraRename(2), SeraInclude]
        public int Member2 = 654321;
    }

    [Test]
    public void TestStruct3()
    {
        var obj = new Struct3();

        var str = SeraJson.Serializer
            .ToString()
            .Serialize(obj);

        Console.WriteLine(str);
        Assert.That(str, Is.EqualTo("{\"Member1\":123456,\"Member2\":654321}"));
    }

    #endregion

    #region Struct4

    [SeraIncludeField]
    public struct Struct4
    {
        public int Member1 { get; set; } = 123456;
        public int Member2 = 654321;

        public Struct4() { }
    }

    [Test]
    public void TestStruct4()
    {
        var obj = new Struct4();

        var str = SeraJson.Serializer
            .ToString()
            .Serialize(obj);

        Console.WriteLine(str);
        Assert.That(str, Is.EqualTo("{\"Member1\":123456,\"Member2\":654321}"));
    }

    #endregion

    #region StructCircularReference1

    public class StructCircularReference1
    {
        public StructCircularReference1? Member1 { get; set; }
    }

    [Test]
    public void TestStructCircularReference1()
    {
        var obj = new StructCircularReference1 { Member1 = new() };

        var str = SeraJson.Serializer
            .ToString()
            .Serialize(obj);

        Console.WriteLine(str);
        Assert.That(str, Is.EqualTo("{\"Member1\":{\"Member1\":null}}"));
    }

    #endregion

    #region StructCircularReference2

    public class StructCircularReference2
    {
        public StructCircularReference2_A? Member1 { get; set; }
    }

    public class StructCircularReference2_A
    {
        public StructCircularReference2? Member1 { get; set; }
    }

    [Test]
    public void TestStructCircularReference2()
    {
        var obj = new StructCircularReference2 { Member1 = new() { Member1 = new() } };

        var str = SeraJson.Serializer
            .ToString()
            .Serialize(obj);

        Console.WriteLine(str);
        Assert.That(str, Is.EqualTo("{\"Member1\":{\"Member1\":{\"Member1\":null}}}"));
    }

    #endregion

    #region StructPrivateField1

    public class StructPrivateField1
    {
        [SeraInclude]
        private int Member1 { get; set; } = 123456;
        [SeraInclude, UsedImplicitly]
        private int Member2 = 654321;
    }

    [Test]
    public void TestStructPrivateField1()
    {
        var obj = new StructPrivateField1();

        var str = SeraJson.Serializer
            .ToString()
            .Serialize(obj);

        Console.WriteLine(str);
        Assert.That(str, Is.EqualTo("{\"Member1\":123456,\"Member2\":654321}"));
    }

    #endregion

    #region StructPrivateType1

    private class StructPrivateType1
    {
        public int Member1 { get; set; } = 123;
    }

    [Test]
    public void TestStructPrivateType1()
    {
        var obj = new StructPrivateType1();

        var str = SeraJson.Serializer
            .ToString()
            .Serialize(obj);

        Console.WriteLine(str);
        Assert.That(str, Is.EqualTo("{\"Member1\":123}"));
    }

    #endregion

    #region StructPrivateTypeMember1

    public class StructPrivateTypeMember1
    {
        [SeraInclude]
        private PrivateType Member1 { get; set; } = new();

        private class PrivateType { }
    }

    [Test]
    public void TestStructPrivateTypeMember1()
    {
        var obj = new StructPrivateTypeMember1();

        var str = SeraJson.Serializer
            .ToString()
            .Serialize(obj);

        Console.WriteLine(str);
        Assert.That(str, Is.EqualTo("{\"Member1\":{}}"));
    }

    #endregion

    #region StructPrivateTypeMember2

    public class StructPrivateTypeMember2
    {
        [SeraInclude]
        private PrivateType Member1 { get; set; } = new();

        private class PrivateType
        {
            [SeraInclude]
            private StructPrivateTypeMember2? Member1 { get; set; }
        }
    }

    [Test]
    public void TestStructPrivateTypeMember2()
    {
        var obj = new StructPrivateTypeMember2();

        var str = SeraJson.Serializer
            .ToString()
            .Serialize(obj);

        Console.WriteLine(str);
        Assert.That(str, Is.EqualTo("{\"Member1\":{\"Member1\":null}}"));
    }

    #endregion
    
    #region StructNullableField1

    public class StructNullableField1
    {
        public object? Member1 { get; set; } = null;
    }

    [Test]
    public void TestStructNullableField1()
    {
        var obj = new StructNullableField1();

        var str = SeraJson.Serializer
            .ToString()
            .Serialize(obj);

        Console.WriteLine(str);
        Assert.That(str, Is.EqualTo("{\"Member1\":null}"));
    }

    #endregion
}
