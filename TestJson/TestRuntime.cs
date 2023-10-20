using System.Buffers;
using System.Collections;
using JetBrains.Annotations;
using Sera;
using Sera.Json;
using Sera.Json.Runtime;
using Sera.Runtime;
using Sera.Runtime.Emit;

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

    #region Struct5

    [SeraIncludeField]
    public struct Struct5
    {
        public Struct5B Member1 { get; set; }
    }

    [SeraIncludeField]
    public struct Struct5B { }

    [Test]
    public void TestStruct5()
    {
        var obj = new Struct5();

        var str = SeraJson.Serializer
            .ToString()
            .Serialize(obj);

        Console.WriteLine(str);
        Assert.That(str, Is.EqualTo("{\"Member1\":{}}"));
    }

    #endregion

    #region Struct6

    [SeraIncludeField]
    public class Struct6
    {
        public Struct6B? Member1 { get; set; } = new();
    }

    [SeraIncludeField]
    public class Struct6B
    {
        public Struct6? Member1 { get; set; } = null;
    }

    [Test]
    public void TestStruct6()
    {
        var obj = new Struct6();

        var str = SeraJson.Serializer
            .ToString()
            .Serialize(obj);

        Console.WriteLine(str);
        Assert.That(str, Is.EqualTo("{\"Member1\":{\"Member1\":null}}"));
    }

    #endregion

    #region Struct7

    public class Struct7Base
    {
        public int A { get; set; } = 123456;
    }

    public class Struct7 : Struct7Base { }

    [Test]
    public void TestStruct7()
    {
        var obj = new Struct7();

        var str = SeraJson.Serializer
            .ToString()
            .Serialize(obj);

        Console.WriteLine(str);
        Assert.That(str, Is.EqualTo("{\"A\":123456}"));
    }

    #endregion

    #region Struct8

    public interface IStruct8
    {
        public int A { get; set; }
    }

    public class Struct8 : IStruct8
    {
        public int A { get; set; } = 123456;
    }

    [Test]
    public void TestStruct8()
    {
        var obj = new Struct8();

        var str = SeraJson.Serializer
            .ToString()
            .Serialize(obj);

        Console.WriteLine(str);
        Assert.That(str, Is.EqualTo("{\"A\":123456}"));
    }

    #endregion

    #region Struct9

    public interface IStruct9
    {
        public int A { get; set; }
    }

    public class Struct9 : IStruct9
    {
        public int A { get; set; } = 123456;
    }

    [Test]
    public void TestStruct9()
    {
        IStruct9 obj = new Struct9();

        var str = SeraJson.Serializer
            .ToString()
            .Serialize(obj);

        Console.WriteLine(str);
        Assert.That(str, Is.EqualTo("{\"A\":123456}"));
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

    #region StructNullableField2

    public class StructNullableField2
    {
        public object Member1 { get; set; } = null!;
    }

    [Test]
    public void TestStructNullableField2()
    {
        var obj = new StructNullableField2();

        Assert.Throws<NullReferenceException>(() => SeraJson.Serializer
            .ToString()
            .Serialize(obj));
    }

    #endregion

    #region StructNullableField3

    public class StructNullableField3
    {
        public StructNullableField3A Member1 { get; set; } = null!;
    }

    public class StructNullableField3A { }

    [Test]
    public void TestStructNullableField3()
    {
        var obj = new StructNullableField3();

        Assert.Throws<NullReferenceException>(() => SeraJson.Serializer
            .ToString()
            .Serialize(obj));
    }

    #endregion

    #region StructNullableField4

    public class StructNullableField4
    {
        public StructNullableField4A Member1 { get; set; } = null!;
    }

    public class StructNullableField4A
    {
        public int A { get; set; }
    }

    [Test]
    public void TestStructNullableField4()
    {
        var obj = new StructNullableField4();

        Assert.Throws<NullReferenceException>(() => SeraJson.Serializer
            .ToString()
            .Serialize(obj));
    }

    #endregion

    #region StructNullableField5

    private class StructNullableField5
    {
        public StructNullableField5 Member1 { get; set; } = null!;
    }

    private class StructNullableField5A { }

    [Test]
    public void TestStructNullableField5()
    {
        var obj = new StructNullableField5();

        Assert.Throws<NullReferenceException>(() => SeraJson.Serializer
            .ToString()
            .Serialize(obj));
    }

    #endregion

    #region StructNullableField6

    private class StructNullableField6
    {
        public StructNullableField6 Member1 { get; set; } = null!;
    }

    private class StructNullableField6A
    {
        public int A { get; set; }
    }

    [Test]
    public void TestStructNullableField6()
    {
        var obj = new StructNullableField6();

        Assert.Throws<NullReferenceException>(() => SeraJson.Serializer
            .ToString()
            .Serialize(obj));
    }

    #endregion

    #region StructGenericNullable1

    public class StructGenericNullable1
    {
        [UsedImplicitly]
        public StructGenericNullable1<StructGenericNullable1?> Member1 { get; set; } = new(null);
    }

    public record StructGenericNullable1<A>([UsedImplicitly] A Member1);

    [Test]
    public void TestStructGenericNullable1()
    {
        var obj = new StructGenericNullable1();

        var str = SeraJson.Serializer
            .ToString()
            .Serialize(obj);

        Console.WriteLine(str);
        Assert.That(str, Is.EqualTo("{\"Member1\":{\"Member1\":null}}"));
    }

    #endregion

    #region StructGenericNullable2

    public class StructGenericNullable2
    {
        [UsedImplicitly]
        public StructGenericNullable2<StructGenericNullable2>? Member1 { get; set; }
    }

    public record StructGenericNullable2<A>([UsedImplicitly] A Member1);

    [Test]
    public void TestStructGenericNullable2()
    {
        var obj = new StructGenericNullable2() { Member1 = new(new()) };

        var str = SeraJson.Serializer
            .ToString()
            .Serialize(obj);

        Console.WriteLine(str);
        Assert.That(str, Is.EqualTo("{\"Member1\":{\"Member1\":{\"Member1\":null}}}"));
    }

    #endregion

    #region StructGenericNullable3

    public class StructGenericNullable3
    {
        [UsedImplicitly]
        public StructGenericNullable3<StructGenericNullable3> Member1 { get; set; } = new(null!);
    }

    public record StructGenericNullable3<A>([UsedImplicitly] A Member1) where A : notnull;

    [Test]
    public void TestStructGenericNullable3()
    {
        var obj = new StructGenericNullable3();

        Assert.Throws<NullReferenceException>(() => SeraJson.Serializer
            .ToString()
            .Serialize(obj));
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

    #region ValueTuple3

    [Test]
    public void TestValueTuple3()
    {
        var obj = (1, 2, 3, 4, 5, 6, 7, 8);

        var str = SeraJson.Serializer
            .ToString()
            .Serialize(obj);

        Console.WriteLine(str);
        Assert.That(str, Is.EqualTo("[1,2,3,4,5,6,7,8]"));
    }

    #endregion

    #region ValueTuple4

    [Test]
    public void TestValueTuple4()
    {
        {
            var obj = ValueTuple.Create();

            var str = SeraJson.Serializer
                .ToString()
                .Serialize(obj);

            Console.WriteLine(str);
            Assert.That(str, Is.EqualTo("[]"));
        }
        {
            var obj = ValueTuple.Create(1);

            var str = SeraJson.Serializer
                .ToString()
                .Serialize(obj);

            Console.WriteLine(str);
            Assert.That(str, Is.EqualTo("[1]"));
        }
        {
            var obj = ValueTuple.Create(1, 2);

            var str = SeraJson.Serializer
                .ToString()
                .Serialize(obj);

            Console.WriteLine(str);
            Assert.That(str, Is.EqualTo("[1,2]"));
        }
        {
            var obj = ValueTuple.Create(1, 2, 3);

            var str = SeraJson.Serializer
                .ToString()
                .Serialize(obj);

            Console.WriteLine(str);
            Assert.That(str, Is.EqualTo("[1,2,3]"));
        }
        {
            var obj = ValueTuple.Create(1, 2, 3, 4);

            var str = SeraJson.Serializer
                .ToString()
                .Serialize(obj);

            Console.WriteLine(str);
            Assert.That(str, Is.EqualTo("[1,2,3,4]"));
        }
        {
            var obj = ValueTuple.Create(1, 2, 3, 4, 5);

            var str = SeraJson.Serializer
                .ToString()
                .Serialize(obj);

            Console.WriteLine(str);
            Assert.That(str, Is.EqualTo("[1,2,3,4,5]"));
        }
        {
            var obj = ValueTuple.Create(1, 2, 3, 4, 5, 6);

            var str = SeraJson.Serializer
                .ToString()
                .Serialize(obj);

            Console.WriteLine(str);
            Assert.That(str, Is.EqualTo("[1,2,3,4,5,6]"));
        }
        {
            var obj = ValueTuple.Create(1, 2, 3, 4, 5, 6, 7);

            var str = SeraJson.Serializer
                .ToString()
                .Serialize(obj);

            Console.WriteLine(str);
            Assert.That(str, Is.EqualTo("[1,2,3,4,5,6,7]"));
        }
        {
            var obj = ValueTuple.Create(1, 2, 3, 4, 5, 6, 7, 8);

            var str = SeraJson.Serializer
                .ToString()
                .Serialize(obj);

            Console.WriteLine(str);
            Assert.That(str, Is.EqualTo("[1,2,3,4,5,6,7,8]"));
        }
        {
            var obj = (1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11);

            var str = SeraJson.Serializer
                .ToString()
                .Serialize(obj);

            Console.WriteLine(str);
            Assert.That(str, Is.EqualTo("[1,2,3,4,5,6,7,8,9,10,11]"));
        }
    }

    #endregion

    #region Tuple1

    [Test]
    public void TestTuple1()
    {
        var obj = Tuple.Create(1, 2, 3);

        var str = SeraJson.Serializer
            .ToString()
            .Serialize(obj);

        Console.WriteLine(str);
        Assert.That(str, Is.EqualTo("[1,2,3]"));
    }

    #endregion

    #region Tuple2

    [Test]
    public void TestTuple2()
    {
        var obj = new Tuple<int, int, int, int, int, int, int, Tuple<int, int, int, int, int>>(
            1, 2, 3, 4, 5, 6, 7,
            new Tuple<int, int, int, int, int>(8, 9, 10, 11, 12)
        );

        var str = SeraJson.Serializer
            .ToString()
            .Serialize(obj);

        Console.WriteLine(str);
        Assert.That(str, Is.EqualTo("[1,2,3,4,5,6,7,8,9,10,11,12]"));
    }

    #endregion

    #region Tuple3

    [Test]
    public void TestTuple3()
    {
        var obj = Tuple.Create(1, 2, 3, 4, 5, 6, 7, 8);

        var str = SeraJson.Serializer
            .ToString()
            .Serialize(obj);

        Console.WriteLine(str);
        Assert.That(str, Is.EqualTo("[1,2,3,4,5,6,7,8]"));
    }

    #endregion

    #region Tuple4

    [Test]
    public void TestTuple4()
    {
        {
            var obj = Tuple.Create(1);

            var str = SeraJson.Serializer
                .ToString()
                .Serialize(obj);

            Console.WriteLine(str);
            Assert.That(str, Is.EqualTo("[1]"));
        }
        {
            var obj = Tuple.Create(1, 2);

            var str = SeraJson.Serializer
                .ToString()
                .Serialize(obj);

            Console.WriteLine(str);
            Assert.That(str, Is.EqualTo("[1,2]"));
        }
        {
            var obj = Tuple.Create(1, 2, 3);

            var str = SeraJson.Serializer
                .ToString()
                .Serialize(obj);

            Console.WriteLine(str);
            Assert.That(str, Is.EqualTo("[1,2,3]"));
        }
        {
            var obj = Tuple.Create(1, 2, 3, 4);

            var str = SeraJson.Serializer
                .ToString()
                .Serialize(obj);

            Console.WriteLine(str);
            Assert.That(str, Is.EqualTo("[1,2,3,4]"));
        }
        {
            var obj = Tuple.Create(1, 2, 3, 4, 5);

            var str = SeraJson.Serializer
                .ToString()
                .Serialize(obj);

            Console.WriteLine(str);
            Assert.That(str, Is.EqualTo("[1,2,3,4,5]"));
        }
        {
            var obj = Tuple.Create(1, 2, 3, 4, 5, 6);

            var str = SeraJson.Serializer
                .ToString()
                .Serialize(obj);

            Console.WriteLine(str);
            Assert.That(str, Is.EqualTo("[1,2,3,4,5,6]"));
        }
        {
            var obj = Tuple.Create(1, 2, 3, 4, 5, 6, 7);

            var str = SeraJson.Serializer
                .ToString()
                .Serialize(obj);

            Console.WriteLine(str);
            Assert.That(str, Is.EqualTo("[1,2,3,4,5,6,7]"));
        }
        {
            var obj = Tuple.Create(1, 2, 3, 4, 5, 6, 7, 8);

            var str = SeraJson.Serializer
                .ToString()
                .Serialize(obj);

            Console.WriteLine(str);
            Assert.That(str, Is.EqualTo("[1,2,3,4,5,6,7,8]"));
        }
        {
            var obj = new Tuple<int, int, int, int, int, int, int, Tuple<int, int, int, int>>(1, 2, 3, 4, 5, 6, 7,
                new(8, 9, 10, 11));

            var str = SeraJson.Serializer
                .ToString()
                .Serialize(obj);

            Console.WriteLine(str);
            Assert.That(str, Is.EqualTo("[1,2,3,4,5,6,7,8,9,10,11]"));
        }
    }

    #endregion

    #region ValueTupleNullable1

    public class ValueTupleNullable1A
    {
        public (int, ValueTupleNullable1A?, int) Member1 { get; } = (1, null, 3);
    }

    [Test]
    public void TestValueTupleNullable1()
    {
        var obj = new ValueTupleNullable1A();

        var str = SeraJson.Serializer
            .ToString()
            .Serialize(obj);

        Console.WriteLine(str);
        Assert.That(str, Is.EqualTo("{\"Member1\":[1,null,3]}"));
    }

    #endregion

    #region ValueTupleNullable2

    public class ValueTupleNullable2A
    {
        public (int, ValueTupleNullable2A, int) Member1 { get; } = (1, null!, 3);
    }

    [Test]
    public void TestValueTupleNullable2()
    {
        var obj = new ValueTupleNullable2A();

        Assert.Throws<NullReferenceException>(() => SeraJson.Serializer
            .ToString()
            .Serialize(obj));
    }

    #endregion

    #region ValueTupleNullable3

    public class ValueTupleNullable3A { }

    [Test]
    public void TestValueTupleNullable3()
    {
        // Reflection cannot get nullable info in this case

        (int, ValueTupleNullable3A, int) obj = (1, null!, 3);

        var str = SeraJson.Serializer
            .ToString()
            .Serialize(obj);

        Console.WriteLine(str);
        Assert.That(str, Is.EqualTo("[1,null,3]"));
    }

    #endregion

    #region ValueTupleNullable4

    public class ValueTupleNullable4A
    {
        public (int, ValueTupleNullable4B?, int) Member1 { get; } = (1, new(), 3);
    }

    public class ValueTupleNullable4B
    {
        public (int, ValueTupleNullable1A?, int) Member1 { get; } = (1, null, 3);
    }

    [Test]
    public void TestValueTupleNullable4()
    {
        var obj = new ValueTupleNullable4A();

        var str = SeraJson.Serializer
            .ToString()
            .Serialize(obj);

        Console.WriteLine(str);
        Assert.That(str, Is.EqualTo("{\"Member1\":[1,{\"Member1\":[1,null,3]},3]}"));
    }

    #endregion

    #region PrivateValueTuple1

    private class PrivateValueTuple1 { }

    [Test]
    public void TestPrivateValueTuple1()
    {
        var a = new PrivateValueTuple1();
        {
            var obj = ValueTuple.Create(a);

            var str = SeraJson.Serializer
                .ToString()
                .Serialize(obj);

            Console.WriteLine(str);
            Assert.That(str, Is.EqualTo("[{}]"));
        }
        {
            var obj = ValueTuple.Create(a, a);

            var str = SeraJson.Serializer
                .ToString()
                .Serialize(obj);

            Console.WriteLine(str);
            Assert.That(str, Is.EqualTo("[{},{}]"));
        }
        {
            var obj = ValueTuple.Create(a, a, a);

            var str = SeraJson.Serializer
                .ToString()
                .Serialize(obj);

            Console.WriteLine(str);
            Assert.That(str, Is.EqualTo("[{},{},{}]"));
        }
        {
            var obj = ValueTuple.Create(a, a, a, a);

            var str = SeraJson.Serializer
                .ToString()
                .Serialize(obj);

            Console.WriteLine(str);
            Assert.That(str, Is.EqualTo("[{},{},{},{}]"));
        }
        {
            var obj = ValueTuple.Create(a, a, a, a, a);

            var str = SeraJson.Serializer
                .ToString()
                .Serialize(obj);

            Console.WriteLine(str);
            Assert.That(str, Is.EqualTo("[{},{},{},{},{}]"));
        }
        {
            var obj = ValueTuple.Create(a, a, a, a, a, a);

            var str = SeraJson.Serializer
                .ToString()
                .Serialize(obj);

            Console.WriteLine(str);
            Assert.That(str, Is.EqualTo("[{},{},{},{},{},{}]"));
        }
        {
            var obj = ValueTuple.Create(a, a, a, a, a, a, a);

            var str = SeraJson.Serializer
                .ToString()
                .Serialize(obj);

            Console.WriteLine(str);
            Assert.That(str, Is.EqualTo("[{},{},{},{},{},{},{}]"));
        }
        {
            var obj = ValueTuple.Create(a, a, a, a, a, a, a, a);

            var str = SeraJson.Serializer
                .ToString()
                .Serialize(obj);

            Console.WriteLine(str);
            Assert.That(str, Is.EqualTo("[{},{},{},{},{},{},{},{}]"));
        }
        {
            var obj =
                new Tuple<PrivateValueTuple1, PrivateValueTuple1, PrivateValueTuple1, PrivateValueTuple1,
                    PrivateValueTuple1, PrivateValueTuple1, PrivateValueTuple1, Tuple<PrivateValueTuple1,
                        PrivateValueTuple1, PrivateValueTuple1, PrivateValueTuple1>>(
                    a, a, a, a, a, a, a,
                    new(a, a, a, a));

            var str = SeraJson.Serializer
                .ToString()
                .Serialize(obj);

            Console.WriteLine(str);
            Assert.That(str, Is.EqualTo("[{},{},{},{},{},{},{},{},{},{},{}]"));
        }
    }

    #endregion

    #region PrivateTuple1

    private class PrivateTuple1 { }

    [Test]
    public void TestPrivateTuple1()
    {
        var a = new PrivateTuple1();
        {
            var obj = Tuple.Create(a);

            var str = SeraJson.Serializer
                .ToString()
                .Serialize(obj);

            Console.WriteLine(str);
            Assert.That(str, Is.EqualTo("[{}]"));
        }
        {
            var obj = Tuple.Create(a, a);

            var str = SeraJson.Serializer
                .ToString()
                .Serialize(obj);

            Console.WriteLine(str);
            Assert.That(str, Is.EqualTo("[{},{}]"));
        }
        {
            var obj = Tuple.Create(a, a, a);

            var str = SeraJson.Serializer
                .ToString()
                .Serialize(obj);

            Console.WriteLine(str);
            Assert.That(str, Is.EqualTo("[{},{},{}]"));
        }
        {
            var obj = Tuple.Create(a, a, a, a);

            var str = SeraJson.Serializer
                .ToString()
                .Serialize(obj);

            Console.WriteLine(str);
            Assert.That(str, Is.EqualTo("[{},{},{},{}]"));
        }
        {
            var obj = Tuple.Create(a, a, a, a, a);

            var str = SeraJson.Serializer
                .ToString()
                .Serialize(obj);

            Console.WriteLine(str);
            Assert.That(str, Is.EqualTo("[{},{},{},{},{}]"));
        }
        {
            var obj = Tuple.Create(a, a, a, a, a, a);

            var str = SeraJson.Serializer
                .ToString()
                .Serialize(obj);

            Console.WriteLine(str);
            Assert.That(str, Is.EqualTo("[{},{},{},{},{},{}]"));
        }
        {
            var obj = Tuple.Create(a, a, a, a, a, a, a);

            var str = SeraJson.Serializer
                .ToString()
                .Serialize(obj);

            Console.WriteLine(str);
            Assert.That(str, Is.EqualTo("[{},{},{},{},{},{},{}]"));
        }
        {
            var obj = Tuple.Create(a, a, a, a, a, a, a, a);

            var str = SeraJson.Serializer
                .ToString()
                .Serialize(obj);

            Console.WriteLine(str);
            Assert.That(str, Is.EqualTo("[{},{},{},{},{},{},{},{}]"));
        }
        {
            var obj = (a, a, a, a, a, a, a, a, a, a, a);

            var str = SeraJson.Serializer
                .ToString()
                .Serialize(obj);

            Console.WriteLine(str);
            Assert.That(str, Is.EqualTo("[{},{},{},{},{},{},{},{},{},{},{}]"));
        }
    }

    #endregion

    #region PrivateTuple2

    private class PrivateTuple2
    {
        public (PrivateTuple2?, PrivateTuple2?) A { get; set; } = (null, null);
    }

    [Test]
    public void TestPrivateTuple2()
    {
        var obj = new PrivateTuple2();

        var str = SeraJson.Serializer
            .ToString()
            .Serialize(obj);

        Console.WriteLine(str);
        Assert.That(str, Is.EqualTo("{\"A\":[null,null]}"));
    }

    #endregion

    #region PrivateTuple3

    private class PrivateTuple3
    {
        public Tuple<PrivateTuple3?, PrivateTuple3?> A { get; set; } =
            Tuple.Create<PrivateTuple3?, PrivateTuple3?>(null, null);
    }

    [Test]
    public void TestPrivateTuple3()
    {
        var obj = new PrivateTuple3();

        var str = SeraJson.Serializer
            .ToString()
            .Serialize(obj);

        Console.WriteLine(str);
        Assert.That(str, Is.EqualTo("{\"A\":[null,null]}"));
    }

    #endregion

    #region Array1

    [Test]
    public void TestArray1()
    {
        var obj = new[] { 1, 2, 3 };

        var str = SeraJson.Serializer
            .ToString()
            .Serialize(obj);

        Console.WriteLine(str);
        Assert.That(str, Is.EqualTo("[1,2,3]"));
    }

    #endregion

    #region Array2

    public class StructTestArray2
    {
        public StructTestArray2?[] A { get; set; } = { null };
    }

    [Test]
    public void TestArray2()
    {
        var obj = new[] { new StructTestArray2() };

        var str = SeraJson.Serializer
            .ToString()
            .Serialize(obj);

        Console.WriteLine(str);
        Assert.That(str, Is.EqualTo("[{\"A\":[null]}]"));
    }

    #endregion

    #region Array3

    private class StructTestArray3 { }

    [Test]
    public void TestArray3()
    {
        var obj = new[] { new StructTestArray3() };

        var str = SeraJson.Serializer
            .ToString()
            .Serialize(obj);

        Console.WriteLine(str);
        Assert.That(str, Is.EqualTo("[{}]"));
    }

    #endregion

    #region PrivateArray1

    private class StructPrivateArray1
    {
        public StructPrivateArray1[]? A { get; set; } = null;
    }

    [Test]
    public void TestPrivateArray1()
    {
        var obj = new[] { new StructPrivateArray1() };

        var str = SeraJson.Serializer
            .ToString()
            .Serialize(obj);

        Console.WriteLine(str);
        Assert.That(str, Is.EqualTo("[{\"A\":null}]"));
    }

    #endregion

    #region PrivateArray2

    private class StructPrivateArray2
    {
        public StructPrivateArray2?[] A { get; set; } = { null };
    }

    [Test]
    public void TestPrivateArray2()
    {
        var obj = new[] { new StructPrivateArray2() };

        var str = SeraJson.Serializer
            .ToString()
            .Serialize(obj);

        Console.WriteLine(str);
        Assert.That(str, Is.EqualTo("[{\"A\":[null]}]"));
    }

    #endregion

    #region List1

    [Test]
    public void TestList1()
    {
        var obj = new List<int> { 1, 2, 3 };

        var str = SeraJson.Serializer
            .ToString()
            .Serialize(obj);

        Console.WriteLine(str);
        Assert.That(str, Is.EqualTo("[1,2,3]"));
    }

    #endregion

    #region PrivateList1

    private class PrivateList1
    {
        public List<PrivateList1?> A { get; set; } = new() { null };
    }

    [Test]
    public void TestPrivateList1()
    {
        var a = new PrivateList1();
        var obj = new List<PrivateList1> { a, a, a };

        var str = SeraJson.Serializer
            .ToString()
            .Serialize(obj);

        Console.WriteLine(str);
        Assert.That(str, Is.EqualTo("[{\"A\":[null]},{\"A\":[null]},{\"A\":[null]}]"));
    }

    #endregion

    #region ListBase1

    public class ListBase1 : List<int> { }

    [Test]
    public void TestListBase1()
    {
        var obj = new ListBase1 { 1, 2, 3 };

        var str = SeraJson.Serializer
            .ToString()
            .Serialize(obj);

        Console.WriteLine(str);
        Assert.That(str, Is.EqualTo("[1,2,3]"));
    }

    #endregion

    #region PrivateListBase1

    private class PrivateListBase1 : List<int> { }

    [Test]
    public void TestPrivateListBase1()
    {
        var obj = new PrivateListBase1 { 1, 2, 3 };

        var str = SeraJson.Serializer
            .ToString()
            .Serialize(obj);

        Console.WriteLine(str);
        Assert.That(str, Is.EqualTo("[1,2,3]"));
    }

    #endregion

    #region PrivateListBase2

    public class PrivateListBase2<A> : List<int> { }

    private class PrivateListBase2A { }

    [Test]
    public void TestPrivateListBase2()
    {
        var obj = new PrivateListBase2<PrivateListBase2A> { 1, 2, 3 };

        var str = SeraJson.Serializer
            .ToString()
            .Serialize(obj);

        Console.WriteLine(str);
        Assert.That(str, Is.EqualTo("[1,2,3]"));
    }

    #endregion

    #region PrivateListBase3

    private class PrivateListBase3 : List<int> { }

    private class PrivateListBase3A
    {
        public PrivateListBase3 A { get; set; } = new() { 1, 2, 3 };
    }

    [Test]
    public void TestPrivateListBase3()
    {
        var obj = new PrivateListBase3A();

        var str = SeraJson.Serializer
            .ToString()
            .Serialize(obj);

        Console.WriteLine(str);
        Assert.That(str, Is.EqualTo("{\"A\":[1,2,3]}"));
    }

    #endregion

    #region ReadOnlySequence1

    [Test]
    public void TestReadOnlySequence1()
    {
        var obj = new ReadOnlySequence<int>(new[] { 1, 2, 3 });

        var str = SeraJson.Serializer
            .ToString()
            .Serialize(obj);

        Console.WriteLine(str);
        Assert.That(str, Is.EqualTo("[1,2,3]"));
    }

    #endregion

    #region ReadOnlySequence2

    public class ReadOnlySequence2
    {
        public ReadOnlySequence<ReadOnlySequence2?> A { get; set; } =
            new(new ReadOnlySequence2?[] { null });
    }

    [Test]
    public void TestReadOnlySequence2()
    {
        var a = new ReadOnlySequence2();
        var obj = new ReadOnlySequence<ReadOnlySequence2>(new[] { a, a, a });

        var str = SeraJson.Serializer
            .ToString()
            .Serialize(obj);

        Console.WriteLine(str);
        Assert.That(str, Is.EqualTo("[{\"A\":[null]},{\"A\":[null]},{\"A\":[null]}]"));
    }

    #endregion

    #region PrivateReadOnlySequence1

    private class PrivateReadOnlySequence1
    {
        public ReadOnlySequence<PrivateReadOnlySequence1?> A { get; set; } =
            new(new PrivateReadOnlySequence1?[] { null });
    }

    [Test]
    public void TestPrivateReadOnlySequence1()
    {
        var a = new PrivateReadOnlySequence1();
        var obj = new ReadOnlySequence<PrivateReadOnlySequence1>(new[] { a, a, a });

        var str = SeraJson.Serializer
            .ToString()
            .Serialize(obj);

        Console.WriteLine(str);
        Assert.That(str, Is.EqualTo("[{\"A\":[null]},{\"A\":[null]},{\"A\":[null]}]"));
    }

    #endregion

    #region ReadOnlyMemory1

    [Test]
    public void TestReadOnlyMemory1()
    {
        var obj = new ReadOnlyMemory<int>(new[] { 1, 2, 3 });

        var str = SeraJson.Serializer
            .ToString()
            .Serialize(obj);

        Console.WriteLine(str);
        Assert.That(str, Is.EqualTo("[1,2,3]"));
    }

    #endregion

    #region ReadOnlyMemory2

    public class ReadOnlyMemory2
    {
        public ReadOnlyMemory<ReadOnlyMemory2?> A { get; set; } =
            new(new ReadOnlyMemory2?[] { null });
    }

    [Test]
    public void TestReadOnlyMemory2()
    {
        var a = new ReadOnlyMemory2();
        var obj = new ReadOnlyMemory<ReadOnlyMemory2>(new[] { a, a, a });

        var str = SeraJson.Serializer
            .ToString()
            .Serialize(obj);

        Console.WriteLine(str);
        Assert.That(str, Is.EqualTo("[{\"A\":[null]},{\"A\":[null]},{\"A\":[null]}]"));
    }

    #endregion

    #region PrivateReadOnlyMemory1

    private class PrivateReadOnlyMemory1
    {
        public ReadOnlyMemory<PrivateReadOnlyMemory1?> A { get; set; } =
            new(new PrivateReadOnlyMemory1?[] { null });
    }

    [Test]
    public void TestPrivateReadOnlyMemory1()
    {
        var a = new PrivateReadOnlyMemory1();
        var obj = new ReadOnlyMemory<PrivateReadOnlyMemory1>(new[] { a, a, a });

        var str = SeraJson.Serializer
            .ToString()
            .Serialize(obj);

        Console.WriteLine(str);
        Assert.That(str, Is.EqualTo("[{\"A\":[null]},{\"A\":[null]},{\"A\":[null]}]"));
    }

    #endregion

    #region Memory1

    [Test]
    public void TestMemory1()
    {
        var obj = new Memory<int>(new[] { 1, 2, 3 });

        var str = SeraJson.Serializer
            .ToString()
            .Serialize(obj);

        Console.WriteLine(str);
        Assert.That(str, Is.EqualTo("[1,2,3]"));
    }

    #endregion

    #region Memory2

    public class Memory2
    {
        public Memory<Memory2?> A { get; set; } =
            new(new Memory2?[] { null });
    }

    [Test]
    public void TestMemory2()
    {
        var a = new Memory2();
        var obj = new Memory<Memory2>(new[] { a, a, a });

        var str = SeraJson.Serializer
            .ToString()
            .Serialize(obj);

        Console.WriteLine(str);
        Assert.That(str, Is.EqualTo("[{\"A\":[null]},{\"A\":[null]},{\"A\":[null]}]"));
    }

    #endregion

    #region PrivateMemory1

    private class PrivateMemory1
    {
        public Memory<PrivateMemory1?> A { get; set; } =
            new(new PrivateMemory1?[] { null });
    }

    [Test]
    public void TestPrivateMemory1()
    {
        var a = new PrivateMemory1();
        var obj = new Memory<PrivateMemory1>(new[] { a, a, a });

        var str = SeraJson.Serializer
            .ToString()
            .Serialize(obj);

        Console.WriteLine(str);
        Assert.That(str, Is.EqualTo("[{\"A\":[null]},{\"A\":[null]},{\"A\":[null]}]"));
    }

    #endregion

    #region Bytes1

    public class Bytes1
    {
        [Sera(As = SeraAs.Bytes)]
        public byte[] A { get; set; } = { 1, 2, 3 };
    }

    [Test]
    public void TestBytes1()
    {
        var obj = new Bytes1();

        var str = SeraJson.Serializer
            .ToString()
            .Serialize(obj);

        Console.WriteLine(str);
        Assert.That(str, Is.EqualTo("{\"A\":\"AQID\"}"));
    }

    #endregion

    #region Bytes2

    [Test]
    public void TestBytes2()
    {
        var obj = new byte[] { 1, 2, 3 };

        var str = SeraJson.Serializer
            .ToString()
            .Serialize(obj, new SeraHints(As: SeraAs.Bytes));

        Console.WriteLine(str);
        Assert.That(str, Is.EqualTo("\"AQID\""));
    }

    #endregion

    #region BytesBase1

    public class BytesBase1 : List<byte> { }

    [Test]
    public void TestBytesBase1()
    {
        var obj = new BytesBase1 { 1, 2, 3 };

        var str = SeraJson.Serializer
            .ToString()
            .Serialize(obj, new SeraHints(As: SeraAs.Bytes));

        Console.WriteLine(str);
        Assert.That(str, Is.EqualTo("\"AQID\""));
    }

    #endregion

    #region PrivateBytesBase1

    private class PrivateBytesBase1 : List<byte> { }

    [Test]
    public void TestPrivateBytesBase1()
    {
        var obj = new PrivateBytesBase1 { 1, 2, 3 };

        var str = SeraJson.Serializer
            .ToString()
            .Serialize(obj, new SeraHints(As: SeraAs.Bytes));

        Console.WriteLine(str);
        Assert.That(str, Is.EqualTo("\"AQID\""));
    }

    #endregion

    #region PrivateBytesBase2

    public class PrivateBytesBase2<A> : List<byte> { }

    private class PrivateBytesBase2A { }

    [Test]
    public void TestPrivateBytesBase2()
    {
        var obj = new PrivateBytesBase2<PrivateBytesBase2A> { 1, 2, 3 };

        var str = SeraJson.Serializer
            .ToString()
            .Serialize(obj, new SeraHints(As: SeraAs.Bytes));

        Console.WriteLine(str);
        Assert.That(str, Is.EqualTo("\"AQID\""));
    }

    #endregion

    #region PrivateBytesBase3

    private class PrivateBytesBase3 : List<byte> { }

    private class PrivateBytesBase3A
    {
        [Sera(As = SeraAs.Bytes)]
        public PrivateBytesBase3 A { get; set; } = new() { 1, 2, 3 };
    }

    [Test]
    public void TestPrivateBytesBase3()
    {
        var obj = new PrivateBytesBase3A();

        var str = SeraJson.Serializer
            .ToString()
            .Serialize(obj);

        Console.WriteLine(str);
        Assert.That(str, Is.EqualTo("{\"A\":\"AQID\"}"));
    }

    #endregion

    #region String1

    [Test]
    public void TestString1()
    {
        var obj = "abc";

        var str = SeraJson.Serializer
            .ToString()
            .Serialize(obj);

        Console.WriteLine(str);
        Assert.That(str, Is.EqualTo("\"abc\""));
    }

    #endregion

    #region String2

    public class String2
    {
        [Sera(As = SeraAs.String)]
        public char[] A { get; set; } = { 'a', 'b', 'c' };
    }

    [Test]
    public void TestString2()
    {
        var obj = new String2();

        var str = SeraJson.Serializer
            .ToString()
            .Serialize(obj);

        Console.WriteLine(str);
        Assert.That(str, Is.EqualTo("{\"A\":\"abc\"}"));
    }

    #endregion

    #region String3

    [Test]
    public void TestString3()
    {
        var obj = new[] { 'a', 'b', 'c' };

        var str = SeraJson.Serializer
            .ToString()
            .Serialize(obj, new SeraHints(As: SeraAs.String));

        Console.WriteLine(str);
        Assert.That(str, Is.EqualTo("\"abc\""));
    }

    #endregion

    #region StringBase1

    public class StringBase1 : List<char> { }

    [Test]
    public void TestStringBase1()
    {
        var obj = new StringBase1 { 'a', 'b', 'c' };

        var str = SeraJson.Serializer
            .ToString()
            .Serialize(obj, new SeraHints(As: SeraAs.String));

        Console.WriteLine(str);
        Assert.That(str, Is.EqualTo("\"abc\""));
    }

    #endregion

    #region PrivateStringBase1

    private class PrivateStringBase1 : List<char> { }

    [Test]
    public void TestPrivateStringBase1()
    {
        var obj = new PrivateStringBase1 { 'a', 'b', 'c' };

        var str = SeraJson.Serializer
            .ToString()
            .Serialize(obj, new SeraHints(As: SeraAs.String));

        Console.WriteLine(str);
        Assert.That(str, Is.EqualTo("\"abc\""));
    }

    #endregion

    #region PrivateStringBase2

    public class PrivateStringBase2<A> : List<char> { }

    private class PrivateStringBase2A;

    [Test]
    public void TestPrivateStringBase2()
    {
        var obj = new PrivateStringBase2<PrivateStringBase2A> { 'a', 'b', 'c' };

        var str = SeraJson.Serializer
            .ToString()
            .Serialize(obj, new SeraHints(As: SeraAs.String));

        Console.WriteLine(str);
        Assert.That(str, Is.EqualTo("\"abc\""));
    }

    #endregion

    #region PrivateStringBase3

    private class PrivateStringBase3 : List<char> { }

    private class PrivateStringBase3A
    {
        [Sera(As = SeraAs.String)]
        public PrivateStringBase3 A { get; set; } = new() { 'a', 'b', 'c' };
    }

    [Test]
    public void TestPrivateStringBase3()
    {
        var obj = new PrivateStringBase3A();

        var str = SeraJson.Serializer
            .ToString()
            .Serialize(obj);

        Console.WriteLine(str);
        Assert.That(str, Is.EqualTo("{\"A\":\"abc\"}"));
    }

    #endregion

    #region Nullable1

    [Test]
    public void TestNullable1()
    {
        int? obj = 1;

        var str = SeraJson.Serializer
            .ToString()
            .Serialize(obj);

        Console.WriteLine(str);
        Assert.That(str, Is.EqualTo("1"));
    }

    #endregion

    #region Nullable2

    [Test]
    public void TestNullable2()
    {
        int? obj = null;

        var str = SeraJson.Serializer
            .ToString()
            .Serialize(obj);

        Console.WriteLine(str);
        Assert.That(str, Is.EqualTo("null"));
    }

    #endregion

    #region Nullable3

    public struct Nullable3
    {
        public Nullable3A? A { get; set; } = new Nullable3A();

        public Nullable3() { }
    }

    public struct Nullable3A;

    [Test]
    public void TestNullable3()
    {
        var obj = new Nullable3();

        var str = SeraJson.Serializer
            .ToString()
            .Serialize(obj);

        Console.WriteLine(str);
        Assert.That(str, Is.EqualTo("{\"A\":{}}"));
    }

    #endregion

    #region Nullable4

    public struct Nullable4
    {
        public Nullable4A A { get; set; } = new();

        public Nullable4() { }
    }

    public class Nullable4A
    {
        public Nullable4? A { get; set; } = null;
    }

    [Test]
    public void TestNullable4()
    {
        var obj = new Nullable4();

        var str = SeraJson.Serializer
            .ToString()
            .Serialize(obj);

        Console.WriteLine(str);
        Assert.That(str, Is.EqualTo("{\"A\":{\"A\":null}}"));
    }

    #endregion

    #region PrivateNullable1

    private struct PrivateNullable1 { }

    [Test]
    public void TestPrivateNullable1()
    {
        PrivateNullable1? obj = new PrivateNullable1();

        var str = SeraJson.Serializer
            .ToString()
            .Serialize(obj);

        Console.WriteLine(str);
        Assert.That(str, Is.EqualTo("{}"));
    }

    #endregion

    #region PrivateNullable2

    private struct PrivateNullable2 { }

    [Test]
    public void TestPrivateNullable2()
    {
        PrivateNullable1? obj = null;

        var str = SeraJson.Serializer
            .ToString()
            .Serialize(obj);

        Console.WriteLine(str);
        Assert.That(str, Is.EqualTo("null"));
    }

    #endregion

    #region PrivateNullable3

    private struct PrivateNullable3
    {
        public PrivateNullable3A? A { get; set; } = new PrivateNullable3A();

        public PrivateNullable3() { }
    }

    private struct PrivateNullable3A;

    [Test]
    public void TestPrivateNullable3()
    {
        var obj = new PrivateNullable3();

        var str = SeraJson.Serializer
            .ToString()
            .Serialize(obj);

        Console.WriteLine(str);
        Assert.That(str, Is.EqualTo("{\"A\":{}}"));
    }

    #endregion

    #region PrivateNullable4

    private struct PrivateNullable4
    {
        public PrivateNullable4A A { get; set; } = new();

        public PrivateNullable4() { }
    }

    private class PrivateNullable4A
    {
        public PrivateNullable4? A { get; set; } = null;
    }

    [Test]
    public void TestPrivateNullable4()
    {
        var obj = new PrivateNullable4();

        var str = SeraJson.Serializer
            .ToString()
            .Serialize(obj);

        Console.WriteLine(str);
        Assert.That(str, Is.EqualTo("{\"A\":{\"A\":null}}"));
    }

    #endregion

    #region IEnumerable1

    public class ClassIEnumerable1 : IEnumerable<int>
    {
        public IEnumerator<int> GetEnumerator()
        {
            yield return 1;
            yield return 2;
            yield return 3;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    [Test]
    public void TestIEnumerable1()
    {
        var obj = new ClassIEnumerable1();

        var str = SeraJson.Serializer
            .ToString()
            .Serialize(obj);

        Console.WriteLine(str);
        Assert.That(str, Is.EqualTo("[1,2,3]"));
    }

    #endregion

    #region IEnumerable2

    [Test]
    public void TestIEnumerable2()
    {
        IEnumerable<int> obj = new[] { 1, 2, 3, };

        var str = SeraJson.Serializer
            .ToString()
            .Serialize(obj);

        Console.WriteLine(str);
        Assert.That(str, Is.EqualTo("[1,2,3]"));
    }

    #endregion

    #region IEnumerable3

    public struct ClassIEnumerable3 : IEnumerable<int>
    {
        public IEnumerator<int> GetEnumerator()
        {
            yield return 1;
            yield return 2;
            yield return 3;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    [Test]
    public void TestIEnumerable3()
    {
        var obj = new ClassIEnumerable3();

        var str = SeraJson.Serializer
            .ToString()
            .Serialize(obj);

        Console.WriteLine(str);
        Assert.That(str, Is.EqualTo("[1,2,3]"));
    }

    #endregion

    #region IEnumerable4

    public class ClassIEnumerable4 : IEnumerable<int>
    {
        IEnumerator<int> IEnumerable<int>.GetEnumerator()
        {
            yield return 1;
            yield return 2;
            yield return 3;
        }

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<int>)this).GetEnumerator();
    }

    [Test]
    public void TestIEnumerable4()
    {
        var obj = new ClassIEnumerable4();

        var str = SeraJson.Serializer
            .ToString()
            .Serialize(obj);

        Console.WriteLine(str);
        Assert.That(str, Is.EqualTo("[1,2,3]"));
    }

    #endregion

    #region IEnumerable5

    public struct ClassIEnumerable5 : IEnumerable<int>
    {
        IEnumerator<int> IEnumerable<int>.GetEnumerator()
        {
            yield return 1;
            yield return 2;
            yield return 3;
        }

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<int>)this).GetEnumerator();
    }

    [Test]
    public void TestIEnumerable5()
    {
        var obj = new ClassIEnumerable5();

        var str = SeraJson.Serializer
            .ToString()
            .Serialize(obj);

        Console.WriteLine(str);
        Assert.That(str, Is.EqualTo("[1,2,3]"));
    }

    #endregion

    #region IEnumerable6

    public class ClassIEnumerable6 : IEnumerable<int>
    {
        public Enumerator GetEnumerator() => new();

        IEnumerator<int> IEnumerable<int>.GetEnumerator() => GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public struct Enumerator : IEnumerator<int>
        {
            private bool end = false;

            public Enumerator() { }

            public void Dispose() { }

            public bool MoveNext()
            {
                if (end) return false;
                end = true;
                return true;
            }

            public void Reset()
            {
                end = false;
            }

            public int Current => 1;
            object IEnumerator.Current => Current;
        }
    }

    [Test]
    public void TestIEnumerable6()
    {
        var obj = new ClassIEnumerable6();

        var str = SeraJson.Serializer
            .ToString()
            .Serialize(obj);

        Console.WriteLine(str);
        Assert.That(str, Is.EqualTo("[1]"));
    }

    #endregion

    #region IEnumerable7

    public class IEnumerable7
    {
        public IEnumerable<IEnumerable7?> A { get; set; } = new IEnumerable7?[] { null };
    }

    [Test]
    public void TestIEnumerable7()
    {
        var obj = new IEnumerable7();

        var str = SeraJson.Serializer
            .ToString()
            .Serialize(obj);

        Console.WriteLine(str);
        Assert.That(str, Is.EqualTo("{\"A\":[null]}"));
    }

    #endregion

    #region PrivateIEnumerable1

    private class ClassPrivateIEnumerable1 : IEnumerable<int>
    {
        public IEnumerator<int> GetEnumerator()
        {
            yield return 1;
            yield return 2;
            yield return 3;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    [Test]
    public void TestPrivateIEnumerable1()
    {
        var obj = new ClassPrivateIEnumerable1();

        var str = SeraJson.Serializer
            .ToString()
            .Serialize(obj);

        Console.WriteLine(str);
        Assert.That(str, Is.EqualTo("[1,2,3]"));
    }

    #endregion

    #region PrivateIEnumerable2

    private class PrivateIEnumerable2 { }

    [Test]
    public void TestPrivateIEnumerable2()
    {
        IEnumerable<PrivateIEnumerable2> obj = new[] { new PrivateIEnumerable2() };

        var str = SeraJson.Serializer
            .ToString()
            .Serialize(obj);

        Console.WriteLine(str);
        Assert.That(str, Is.EqualTo("[{}]"));
    }

    #endregion

    #region PrivateIEnumerable3

    private class PrivateIEnumerable3
    {
        public IEnumerable<PrivateIEnumerable3?> A { get; set; } = new PrivateIEnumerable3?[] { null };
    }

    [Test]
    public void TestPrivateIEnumerable3()
    {
        var obj = new PrivateIEnumerable3();

        var str = SeraJson.Serializer
            .ToString()
            .Serialize(obj);

        Console.WriteLine(str);
        Assert.That(str, Is.EqualTo("{\"A\":[null]}"));
    }

    #endregion

    #region Runtime1

    [SeraIncludeField]
    public class Runtime1
    {
        public int A { get; set; } = 123456;
        public int B = 654321;
    }

    [Test]
    public void TestRuntime1()
    {
        var obj = new Runtime1();

        var impl = EmitRuntimeProvider.Instance.GetRuntimeSerialize();

        var str = SeraJson.Serializer
            .ToString()
            .Serialize(obj, impl);

        Console.WriteLine(str);
        Assert.That(str, Is.EqualTo("{\"A\":123456,\"B\":654321}"));
    }

    #endregion

    #region Runtime2

    [SeraIncludeField]
    public struct Runtime2
    {
        public int A { get; set; } = 123456;
        public int B = 654321;

        public Runtime2() { }
    }

    [Test]
    public void TestRuntime2()
    {
        var obj = new Runtime2();

        var impl = EmitRuntimeProvider.Instance.GetRuntimeSerialize();

        var str = SeraJson.Serializer
            .ToString()
            .Serialize((object)obj, impl);

        Console.WriteLine(str);
        Assert.That(str, Is.EqualTo("{\"A\":123456,\"B\":654321}"));
    }

    #endregion
}
