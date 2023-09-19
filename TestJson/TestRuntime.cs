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
        [UsedImplicitly]
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
        [UsedImplicitly]
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
        [UsedImplicitly]
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
        [UsedImplicitly]
        private PrivateType Member1 { get; set; } = new();

        private class PrivateType
        {
            [SeraInclude]
            [UsedImplicitly]
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

    #region Enum1

    public enum Enum1
    {
        A,
        [SeraRename("X")]
        B,
        C,
    }

    [Test]
    public void TestEnum1()
    {
        {
            var obj = Enum1.A;

            var str = SeraJson.Serializer
                .ToString()
                .Serialize(obj);

            Console.WriteLine(str);
            Assert.That(str, Is.EqualTo("\"A\""));
        }
        {
            var obj = Enum1.B;

            var str = SeraJson.Serializer
                .ToString()
                .Serialize(obj);

            Console.WriteLine(str);
            Assert.That(str, Is.EqualTo("\"X\""));
        }
        {
            var obj = Enum1.C;

            var str = SeraJson.Serializer
                .ToString()
                .Serialize(obj);

            Console.WriteLine(str);
            Assert.That(str, Is.EqualTo("\"C\""));
        }
        {
            var obj = (Enum1)100;

            var str = SeraJson.Serializer
                .ToString()
                .Serialize(obj);

            Console.WriteLine(str);
            Assert.That(str, Is.EqualTo("100"));
        }
    }

    #endregion

    #region Enum2

    public enum Enum2
    {
        A = -20,
        B = 50,
        C = 500,
    }

    [Test]
    public void TestEnum2()
    {
        {
            var obj = Enum2.A;

            var str = SeraJson.Serializer
                .ToString()
                .Serialize(obj);

            Console.WriteLine(str);
            Assert.That(str, Is.EqualTo("\"A\""));
        }
        {
            var obj = Enum2.B;

            var str = SeraJson.Serializer
                .ToString()
                .Serialize(obj);

            Console.WriteLine(str);
            Assert.That(str, Is.EqualTo("\"B\""));
        }
        {
            var obj = Enum2.C;

            var str = SeraJson.Serializer
                .ToString()
                .Serialize(obj);

            Console.WriteLine(str);
            Assert.That(str, Is.EqualTo("\"C\""));
        }
        {
            var obj = (Enum2)1000;

            var str = SeraJson.Serializer
                .ToString()
                .Serialize(obj);

            Console.WriteLine(str);
            Assert.That(str, Is.EqualTo("1000"));
        }
    }

    #endregion

    #region Enum3

    public enum Enum3 : byte
    {
        A,
        B,
        C,
    }

    [Test]
    public void TestEnum3()
    {
        {
            var obj = Enum3.A;

            var str = SeraJson.Serializer
                .ToString()
                .Serialize(obj);

            Console.WriteLine(str);
            Assert.That(str, Is.EqualTo("\"A\""));
        }
        {
            var obj = Enum3.B;

            var str = SeraJson.Serializer
                .ToString()
                .Serialize(obj);

            Console.WriteLine(str);
            Assert.That(str, Is.EqualTo("\"B\""));
        }
        {
            var obj = Enum3.C;

            var str = SeraJson.Serializer
                .ToString()
                .Serialize(obj);

            Console.WriteLine(str);
            Assert.That(str, Is.EqualTo("\"C\""));
        }
        {
            var obj = (Enum3)100;

            var str = SeraJson.Serializer
                .ToString()
                .Serialize(obj);

            Console.WriteLine(str);
            Assert.That(str, Is.EqualTo("100"));
        }
    }

    #endregion

    #region Enum4

    public enum Enum4 : ulong
    {
        A,
        B,
        C,
    }

    [Test]
    public void TestEnum4()
    {
        {
            var obj = Enum4.A;

            var str = SeraJson.Serializer
                .ToString()
                .Serialize(obj);

            Console.WriteLine(str);
            Assert.That(str, Is.EqualTo("\"A\""));
        }
        {
            var obj = Enum4.B;

            var str = SeraJson.Serializer
                .ToString()
                .Serialize(obj);

            Console.WriteLine(str);
            Assert.That(str, Is.EqualTo("\"B\""));
        }
        {
            var obj = Enum4.C;

            var str = SeraJson.Serializer
                .ToString()
                .Serialize(obj);

            Console.WriteLine(str);
            Assert.That(str, Is.EqualTo("\"C\""));
        }
        {
            var obj = (Enum4)100;

            var str = SeraJson.Serializer
                .ToString()
                .Serialize(obj);

            Console.WriteLine(str);
            Assert.That(str, Is.EqualTo("\"100\""));
        }
    }

    #endregion

    #region Enum5

    public enum Enum5 : short
    {
        A = -20,
        B = 50,
        C = 500,
    }

    [Test]
    public void TestEnum5()
    {
        {
            var obj = Enum5.A;

            var str = SeraJson.Serializer
                .ToString()
                .Serialize(obj);

            Console.WriteLine(str);
            Assert.That(str, Is.EqualTo("\"A\""));
        }
        {
            var obj = Enum5.B;

            var str = SeraJson.Serializer
                .ToString()
                .Serialize(obj);

            Console.WriteLine(str);
            Assert.That(str, Is.EqualTo("\"B\""));
        }
        {
            var obj = Enum5.C;

            var str = SeraJson.Serializer
                .ToString()
                .Serialize(obj);

            Console.WriteLine(str);
            Assert.That(str, Is.EqualTo("\"C\""));
        }
        {
            var obj = (Enum5)1000;

            var str = SeraJson.Serializer
                .ToString()
                .Serialize(obj);

            Console.WriteLine(str);
            Assert.That(str, Is.EqualTo("1000"));
        }
    }

    #endregion

    #region Enum6

    public enum Enum6 : long
    {
        A = -20,
        B = 50,
        C = 500,
    }

    [Test]
    public void TestEnum6()
    {
        {
            var obj = Enum6.A;

            var str = SeraJson.Serializer
                .ToString()
                .Serialize(obj);

            Console.WriteLine(str);
            Assert.That(str, Is.EqualTo("\"A\""));
        }
        {
            var obj = Enum6.B;

            var str = SeraJson.Serializer
                .ToString()
                .Serialize(obj);

            Console.WriteLine(str);
            Assert.That(str, Is.EqualTo("\"B\""));
        }
        {
            var obj = Enum6.C;

            var str = SeraJson.Serializer
                .ToString()
                .Serialize(obj);

            Console.WriteLine(str);
            Assert.That(str, Is.EqualTo("\"C\""));
        }
        {
            var obj = (Enum6)1000;

            var str = SeraJson.Serializer
                .ToString()
                .Serialize(obj);

            Console.WriteLine(str);
            Assert.That(str, Is.EqualTo("\"1000\""));
        }
    }

    #endregion

    #region Enum7

    public enum Enum7
    {
        A1 = 5,
        A2 = 9,
        A3 = 1,
        A4 = 123,
        A5 = 456,
        A6 = -7,
        A7 = -99,
        A8 = 99,
        A9 = 765,
        A10 = 616,
        A11 = 90,
        A12 = 52,
        A13 = 69,
        A14 = 233,
        A15 = 86,
        A16 = 42,
        A17 = 137,
        A18 = 7010,
        A19 = 2077,
        A20 = 65536,
    }

    [Test]
    public void TestEnum7()
    {
        {
            var obj = Enum7.A3;

            var str = SeraJson.Serializer
                .ToString()
                .Serialize(obj);

            Console.WriteLine(str);
            Assert.That(str, Is.EqualTo("\"A3\""));
        }
        {
            var obj = Enum7.A20;

            var str = SeraJson.Serializer
                .ToString()
                .Serialize(obj);

            Console.WriteLine(str);
            Assert.That(str, Is.EqualTo("\"A20\""));
        }
        {
            var obj = Enum7.A15;

            var str = SeraJson.Serializer
                .ToString()
                .Serialize(obj);

            Console.WriteLine(str);
            Assert.That(str, Is.EqualTo("\"A15\""));
        }
        {
            var obj = (Enum7)100;

            var str = SeraJson.Serializer
                .ToString()
                .Serialize(obj);

            Console.WriteLine(str);
            Assert.That(str, Is.EqualTo("100"));
        }
    }

    #endregion

    #region Enum8

    public enum Enum8
    {
        A,
        B,
        C,
        A1 = A,
        B1 = B,
        C1 = C,
    }

    [Test]
    public void TestEnum8()
    {
        {
            var obj = Enum8.A;

            var str = SeraJson.Serializer
                .ToString()
                .Serialize(obj);

            Console.WriteLine(str);
            Assert.That(str, Is.EqualTo("\"A\""));
        }
        {
            var obj = Enum8.B;

            var str = SeraJson.Serializer
                .ToString()
                .Serialize(obj);

            Console.WriteLine(str);
            Assert.That(str, Is.EqualTo("\"B\""));
        }
        {
            var obj = Enum8.C;

            var str = SeraJson.Serializer
                .ToString()
                .Serialize(obj);

            Console.WriteLine(str);
            Assert.That(str, Is.EqualTo("\"C\""));
        }
        {
            var obj = (Enum8)100;

            var str = SeraJson.Serializer
                .ToString()
                .Serialize(obj);

            Console.WriteLine(str);
            Assert.That(str, Is.EqualTo("100"));
        }
    }

    #endregion

    #region Enum9

    private enum Enum9
    {
        A,
        B,
        C,
    }

    [Test]
    public void TestEnum9()
    {
        {
            var obj = Enum9.A;

            var str = SeraJson.Serializer
                .ToString()
                .Serialize(obj);

            Console.WriteLine(str);
            Assert.That(str, Is.EqualTo("\"A\""));
        }
        {
            var obj = Enum9.B;

            var str = SeraJson.Serializer
                .ToString()
                .Serialize(obj);

            Console.WriteLine(str);
            Assert.That(str, Is.EqualTo("\"B\""));
        }
        {
            var obj = Enum9.C;

            var str = SeraJson.Serializer
                .ToString()
                .Serialize(obj);

            Console.WriteLine(str);
            Assert.That(str, Is.EqualTo("\"C\""));
        }
        {
            var obj = (Enum9)100;

            var str = SeraJson.Serializer
                .ToString()
                .Serialize(obj);

            Console.WriteLine(str);
            Assert.That(str, Is.EqualTo("100"));
        }
    }

    #endregion

    #region Enum10

    [SeraEnum(UseNumberTag = true)]
    public enum Enum10
    {
        A,
        B,
        C,
    }

    [Test]
    public void TestEnum10()
    {
        {
            var obj = Enum10.A;

            var str = SeraJson.Serializer
                .ToString()
                .Serialize(obj);

            Console.WriteLine(str);
            Assert.That(str, Is.EqualTo("0"));
        }
        {
            var obj = Enum10.B;

            var str = SeraJson.Serializer
                .ToString()
                .Serialize(obj);

            Console.WriteLine(str);
            Assert.That(str, Is.EqualTo("1"));
        }
        {
            var obj = Enum10.C;

            var str = SeraJson.Serializer
                .ToString()
                .Serialize(obj);

            Console.WriteLine(str);
            Assert.That(str, Is.EqualTo("2"));
        }
        {
            var obj = (Enum10)100;

            var str = SeraJson.Serializer
                .ToString()
                .Serialize(obj);

            Console.WriteLine(str);
            Assert.That(str, Is.EqualTo("100"));
        }
    }

    #endregion

    #region Enum11

    [SeraEnum(UseNumberTag = true)]
    private enum Enum11
    {
        A,
        B,
        C,
    }

    [Test]
    public void TestEnum11()
    {
        {
            var obj = Enum11.A;

            var str = SeraJson.Serializer
                .ToString()
                .Serialize(obj);

            Console.WriteLine(str);
            Assert.That(str, Is.EqualTo("0"));
        }
        {
            var obj = Enum11.B;

            var str = SeraJson.Serializer
                .ToString()
                .Serialize(obj);

            Console.WriteLine(str);
            Assert.That(str, Is.EqualTo("1"));
        }
        {
            var obj = Enum11.C;

            var str = SeraJson.Serializer
                .ToString()
                .Serialize(obj);

            Console.WriteLine(str);
            Assert.That(str, Is.EqualTo("2"));
        }
        {
            var obj = (Enum11)100;

            var str = SeraJson.Serializer
                .ToString()
                .Serialize(obj);

            Console.WriteLine(str);
            Assert.That(str, Is.EqualTo("100"));
        }
    }

    #endregion

    #region Enum12

    [SeraEnum(UseNumberTag = true)]
    public enum Enum12
    {
        [SeraEnum(UseStringTag = true)]
        A,
        [SeraEnum(UseNumberTag = true)]
        B,
        C,
    }

    [Test]
    public void TestEnum12()
    {
        {
            var obj = Enum12.A;

            var str = SeraJson.Serializer
                .ToString()
                .Serialize(obj);

            Console.WriteLine(str);
            Assert.That(str, Is.EqualTo("\"A\""));
        }
        {
            var obj = Enum12.B;

            var str = SeraJson.Serializer
                .ToString()
                .Serialize(obj);

            Console.WriteLine(str);
            Assert.That(str, Is.EqualTo("1"));
        }
        {
            var obj = Enum12.C;

            var str = SeraJson.Serializer
                .ToString()
                .Serialize(obj);

            Console.WriteLine(str);
            Assert.That(str, Is.EqualTo("2"));
        }
        {
            var obj = (Enum12)100;

            var str = SeraJson.Serializer
                .ToString()
                .Serialize(obj);

            Console.WriteLine(str);
            Assert.That(str, Is.EqualTo("100"));
        }
    }

    #endregion

    #region Enum13

    [SeraEnum(UseNumberTag = true)]
    private enum Enum13
    {
        [SeraEnum(UseStringTag = true)]
        A,
        [SeraEnum(UseNumberTag = true)]
        B,
        C,
    }

    [Test]
    public void TestEnum13()
    {
        {
            var obj = Enum13.A;

            var str = SeraJson.Serializer
                .ToString()
                .Serialize(obj);

            Console.WriteLine(str);
            Assert.That(str, Is.EqualTo("\"A\""));
        }
        {
            var obj = Enum13.B;

            var str = SeraJson.Serializer
                .ToString()
                .Serialize(obj);

            Console.WriteLine(str);
            Assert.That(str, Is.EqualTo("1"));
        }
        {
            var obj = Enum13.C;

            var str = SeraJson.Serializer
                .ToString()
                .Serialize(obj);

            Console.WriteLine(str);
            Assert.That(str, Is.EqualTo("2"));
        }
        {
            var obj = (Enum13)100;

            var str = SeraJson.Serializer
                .ToString()
                .Serialize(obj);

            Console.WriteLine(str);
            Assert.That(str, Is.EqualTo("100"));
        }
    }

    #endregion

    #region Flags1

    [Flags, SeraFlags(SeraFlagsMode.String)]
    public enum Flags1
    {
        A = 1 << 0,
        B = 1 << 1,
        C = 1 << 2,
    }

    [Test]
    public void TestFlags1()
    {
        {
            var obj = Flags1.A;

            var str = SeraJson.Serializer
                .ToString()
                .Serialize(obj);

            Console.WriteLine(str);
            Assert.That(str, Is.EqualTo("\"A\""));
        }
        {
            var obj = Flags1.B;

            var str = SeraJson.Serializer
                .ToString()
                .Serialize(obj);

            Console.WriteLine(str);
            Assert.That(str, Is.EqualTo("\"B\""));
        }
        {
            var obj = Flags1.C;

            var str = SeraJson.Serializer
                .ToString()
                .Serialize(obj);

            Console.WriteLine(str);
            Assert.That(str, Is.EqualTo("\"C\""));
        }
        {
            var obj = (Flags1)100;

            var str = SeraJson.Serializer
                .ToString()
                .Serialize(obj);

            Console.WriteLine(str);
            Assert.That(str, Is.EqualTo("\"100\""));
        }
        {
            var obj = Flags1.A | Flags1.B;

            var str = SeraJson.Serializer
                .ToString()
                .Serialize(obj);

            Console.WriteLine(str);
            Assert.That(str, Is.EqualTo("\"A, B\""));
        }
        {
            var obj = Flags1.A | Flags1.B | Flags1.C;

            var str = SeraJson.Serializer
                .ToString()
                .Serialize(obj);

            Console.WriteLine(str);
            Assert.That(str, Is.EqualTo("\"A, B, C\""));
        }
        {
            var obj = Flags1.A | Flags1.B | (Flags1)100;

            var str = SeraJson.Serializer
                .ToString()
                .Serialize(obj);

            Console.WriteLine(str);
            Assert.That(str, Is.EqualTo("\"103\""));
        }
    }

    #endregion

    #region Flags2

    [Flags, SeraFlags(SeraFlagsMode.StringSplit)]
    public enum Flags2
    {
        A = 1 << 0,
        B = 1 << 1,
        C = 1 << 2,
    }

    [Test]
    public void TestFlags2()
    {
        {
            var obj = Flags2.A;

            var str = SeraJson.Serializer
                .ToString()
                .Serialize(obj);

            Console.WriteLine(str);
            Assert.That(str, Is.EqualTo("[\"A\"]"));
        }
        {
            var obj = Flags2.B;

            var str = SeraJson.Serializer
                .ToString()
                .Serialize(obj);

            Console.WriteLine(str);
            Assert.That(str, Is.EqualTo("[\"B\"]"));
        }
        {
            var obj = Flags2.C;

            var str = SeraJson.Serializer
                .ToString()
                .Serialize(obj);

            Console.WriteLine(str);
            Assert.That(str, Is.EqualTo("[\"C\"]"));
        }
        {
            var obj = (Flags2)100;

            var str = SeraJson.Serializer
                .ToString()
                .Serialize(obj);

            Console.WriteLine(str);
            Assert.That(str, Is.EqualTo("[\"100\"]"));
        }
        {
            var obj = Flags2.A | Flags2.B;

            var str = SeraJson.Serializer
                .ToString()
                .Serialize(obj);

            Console.WriteLine(str);
            Assert.That(str, Is.EqualTo("[\"A\",\"B\"]"));
        }
        {
            var obj = Flags2.A | Flags2.B | Flags2.C;

            var str = SeraJson.Serializer
                .ToString()
                .Serialize(obj);

            Console.WriteLine(str);
            Assert.That(str, Is.EqualTo("[\"A\",\"B\",\"C\"]"));
        }
        {
            var obj = Flags2.A | Flags2.B | (Flags2)100;

            var str = SeraJson.Serializer
                .ToString()
                .Serialize(obj);

            Console.WriteLine(str);
            Assert.That(str, Is.EqualTo("[\"103\"]"));
        }
    }

    #endregion

    #region Flags3

    [Flags, SeraFlags(SeraFlagsMode.Number)]
    public enum Flags3
    {
        A = 1 << 0,
        B = 1 << 1,
        C = 1 << 2,
    }

    [Test]
    public void TestFlags3()
    {
        {
            var obj = Flags3.A;

            var str = SeraJson.Serializer
                .ToString()
                .Serialize(obj);

            Console.WriteLine(str);
            Assert.That(str, Is.EqualTo("1"));
        }
        {
            var obj = Flags3.B;

            var str = SeraJson.Serializer
                .ToString()
                .Serialize(obj);

            Console.WriteLine(str);
            Assert.That(str, Is.EqualTo("2"));
        }
        {
            var obj = Flags3.C;

            var str = SeraJson.Serializer
                .ToString()
                .Serialize(obj);

            Console.WriteLine(str);
            Assert.That(str, Is.EqualTo("4"));
        }
        {
            var obj = (Flags3)100;

            var str = SeraJson.Serializer
                .ToString()
                .Serialize(obj);

            Console.WriteLine(str);
            Assert.That(str, Is.EqualTo("100"));
        }
        {
            var obj = Flags3.A | Flags3.B;

            var str = SeraJson.Serializer
                .ToString()
                .Serialize(obj);

            Console.WriteLine(str);
            Assert.That(str, Is.EqualTo("3"));
        }
        {
            var obj = Flags3.A | Flags3.B | Flags3.C;

            var str = SeraJson.Serializer
                .ToString()
                .Serialize(obj);

            Console.WriteLine(str);
            Assert.That(str, Is.EqualTo("7"));
        }
        {
            var obj = Flags3.A | Flags3.B | (Flags3)100;

            var str = SeraJson.Serializer
                .ToString()
                .Serialize(obj);

            Console.WriteLine(str);
            Assert.That(str, Is.EqualTo("103"));
        }
    }

    #endregion
    
    #region Flags4

    [Flags, SeraFlags(SeraFlagsMode.Array)]
    public enum Flags4
    {
        A = 1 << 0,
        [SeraRename("X")]
        B = 1 << 1,
        C = 1 << 2,
    }

    [Test]
    public void TestFlags4()
    {
        {
            var obj = Flags4.A;

            var str = SeraJson.Serializer
                .ToString()
                .Serialize(obj);

            Console.WriteLine(str);
            Assert.That(str, Is.EqualTo("[\"A\"]"));
        }
        {
            var obj = Flags4.B;

            var str = SeraJson.Serializer
                .ToString()
                .Serialize(obj);

            Console.WriteLine(str);
            Assert.That(str, Is.EqualTo("[\"X\"]"));
        }
        {
            var obj = Flags4.C;

            var str = SeraJson.Serializer
                .ToString()
                .Serialize(obj);

            Console.WriteLine(str);
            Assert.That(str, Is.EqualTo("[\"C\"]"));
        }
        {
            var obj = (Flags4)100;

            var str = SeraJson.Serializer
                .ToString()
                .Serialize(obj);

            Console.WriteLine(str);
            Assert.That(str, Is.EqualTo("[\"C\"]"));
        }
        {
            var obj = (Flags4)80;

            var str = SeraJson.Serializer
                .ToString()
                .Serialize(obj);

            Console.WriteLine(str);
            Assert.That(str, Is.EqualTo("[]"));
        }
        {
            var obj = Flags4.A | Flags4.B;

            var str = SeraJson.Serializer
                .ToString()
                .Serialize(obj);

            Console.WriteLine(str);
            Assert.That(str, Is.EqualTo("[\"A\",\"X\"]"));
        }
        {
            var obj = Flags4.A | Flags4.B | Flags4.C;

            var str = SeraJson.Serializer
                .ToString()
                .Serialize(obj);

            Console.WriteLine(str);
            Assert.That(str, Is.EqualTo("[\"A\",\"X\",\"C\"]"));
        }
        {
            var obj = Flags4.A | Flags4.B | (Flags4)80;

            var str = SeraJson.Serializer
                .ToString()
                .Serialize(obj);

            Console.WriteLine(str);
            Assert.That(str, Is.EqualTo("[\"A\",\"X\"]"));
        }
    }

    #endregion
    
    #region ValueTuple1
    
    [Test]
    public void TestValueTuple1()
    {
        var obj = (1, 2, 3);

        var str = SeraJson.Serializer
            .ToString()
            .Serialize(obj);

        Console.WriteLine(str);
        Assert.That(str, Is.EqualTo("[1,2,3]"));
    }

    #endregion

    #region ValueTuple2
    
    [Test]
    public void TestValueTuple2()
    {
        var obj = (1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12);

        var str = SeraJson.Serializer
            .ToString()
            .Serialize(obj);

        Console.WriteLine(str);
        Assert.That(str, Is.EqualTo("[1,2,3,4,5,6,7,8,9,10,11,12]"));
    }

    #endregion

}
