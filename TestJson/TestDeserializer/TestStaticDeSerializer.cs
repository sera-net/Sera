﻿using System.Numerics;
using System.Text;
using Sera;
using Sera.Core;
using Sera.Core.Formats;
using Sera.Core.Impls.De;
using Sera.Json;
using Sera.Utils;

namespace TestJson.TestDeserializer;

public class TestStaticDeSerializer
{
    [Test]
    public void TestPrimitiveBoolean1()
    {
        var json = "true";

        var val = SeraJson.Deserializer
            .Deserialize<bool>()
            .Use(new PrimitiveImpl())
            .Static.From.String(json);

        Console.WriteLine(val);

        Assert.That(val, Is.EqualTo(true));
    }

    [Test]
    public void TestPrimitiveBoolean2()
    {
        var json = "false";

        var val = SeraJson.Deserializer
            .Deserialize<bool>()
            .Use(new PrimitiveImpl())
            .Static.From.String(json);

        Console.WriteLine(val);

        Assert.That(val, Is.EqualTo(false));
    }

    [Test]
    public void TestPrimitiveNumber1()
    {
        var json = "123";

        var val = SeraJson.Deserializer
            .Deserialize<int>()
            .Use(new PrimitiveImpl())
            .Static.From.String(json);

        Console.WriteLine(val);

        Assert.That(val, Is.EqualTo(123));
    }

    [Test]
    public void TestPrimitiveNumber2()
    {
        var json = "123";

        var val = SeraJson.Deserializer
            .Deserialize<float>()
            .Use(new PrimitiveImpl())
            .Static.From.String(json);

        Console.WriteLine(val);

        Assert.That(val, Is.EqualTo(123f));
    }

    [Test]
    public void TestPrimitiveNumber3()
    {
        var json = "123.456";

        var val = SeraJson.Deserializer
            .Deserialize<float>()
            .Use(new PrimitiveImpl())
            .Static.From.String(json);

        Console.WriteLine(val);

        Assert.That(val, Is.EqualTo(123.456f));
    }

    [Test]
    public void TestPrimitiveNumber4()
    {
        var json = "\"123\"";

        var val = SeraJson.Deserializer
            .Deserialize<ulong>()
            .Use(new PrimitiveImpl())
            .Static.From.String(json);

        Console.WriteLine(val);

        Assert.That(val, Is.EqualTo(123UL));
    }

    [Test]
    public void TestPrimitiveNumber5()
    {
        var json = "[1,2]";

        var val = SeraJson.Deserializer
            .Deserialize<Complex>()
            .Use(new PrimitiveImpl())
            .Static.From.String(json);

        Console.WriteLine(val);

        Assert.That(val, Is.EqualTo(new Complex(1, 2)));
    }

    [Test]
    public void TestPrimitiveNumber6()
    {
        var json = "\"<1; 2>\"";

        var val = SeraJson.Deserializer
            .Deserialize<Complex>()
            .Use(new PrimitiveImpl(SeraFormats.Default with { ComplexAsString = true }))
            .Static.From.String(json);

        Console.WriteLine(val);

        Assert.That(val, Is.EqualTo(new Complex(1, 2)));
    }

    [Test]
    public void TestPrimitiveNumber7()
    {
        var json = "123";

        var val = SeraJson.Deserializer
            .Deserialize<BigInteger>()
            .Use(new PrimitiveImpl())
            .Static.From.String(json);

        Console.WriteLine(val);

        Assert.That(val, Is.EqualTo(new BigInteger(123)));
    }

    [Test]
    public void TestPrimitiveDate1()
    {
        var json = "\"2077-07-07T07:07:07.0000000Z\"";

        var val = SeraJson.Deserializer
            .Deserialize<DateTime>()
            .Use(new PrimitiveImpl())
            .Static.From.String(json);

        Console.WriteLine(val);

        Assert.That(val.ToUniversalTime(), Is.EqualTo(new DateTime(2077, 7, 7, 7, 7, 7, DateTimeKind.Utc)));
    }

    [Test]
    public void TestPrimitiveDate2()
    {
        var json = "\"2077-07-07T07:07:07.0000000Z\"";

        var val = SeraJson.Deserializer
            .WithOptions(SeraJsonOptions.Default with { TimeZone = TimeZoneInfo.Utc })
            .Deserialize<DateTime>()
            .Use(new PrimitiveImpl())
            .Static.From.String(json);

        Console.WriteLine(val);

        Assert.That(val, Is.EqualTo(new DateTime(2077, 7, 7, 7, 7, 7, DateTimeKind.Utc)));
    }

    [Test]
    public void TestPrimitiveDate3()
    {
        var json = "\"2077-07-07T07:07:07.0000000Z\"";

        var val = SeraJson.Deserializer
            .Deserialize<DateTimeOffset>()
            .Use(new PrimitiveImpl())
            .Static.From.String(json);

        Console.WriteLine(val);

        Assert.That(val, Is.EqualTo(new DateTimeOffset(2077, 7, 7, 7, 7, 7, TimeSpan.Zero)));
    }

    [Test]
    public void TestPrimitiveDate4()
    {
        var json = "\"2077-07-07T07:07:07.0000000Z\"";

        var val = SeraJson.Deserializer
            .WithOptions(SeraJsonOptions.Default with { TimeZone = TimeZoneInfo.Local })
            .Deserialize<DateTimeOffset>()
            .Use(new PrimitiveImpl(SeraFormats.Default with
            {
                DateTimeFormat = DateTimeFormatFlags.DateTimeOffsetUseTimeZone
            }))
            .Static.From.String(json);

        Console.WriteLine(val);

        var local = new DateTimeOffset(2077, 7, 7, 7, 7, 7, TimeSpan.Zero).ToLocalTime();
        Console.WriteLine(local);

        Assert.That(val, Is.EqualTo(local));
        Assert.That(val.ToString(), Is.EqualTo(local.ToString()));
    }

    [Test]
    public void TestPrimitiveDate5()
    {
        var json = "\"07:07:07.0000000\"";

        var val = SeraJson.Deserializer
            .Deserialize<TimeOnly>()
            .Use(new PrimitiveImpl())
            .Static.From.String(json);

        Console.WriteLine(val);

        Assert.That(val, Is.EqualTo(new TimeOnly(7, 7, 7)));
    }

    [Test]
    public void TestPrimitiveDate6()
    {
        var json = "\"2077-07-07\"";

        var val = SeraJson.Deserializer
            .Deserialize<DateOnly>()
            .Use(new PrimitiveImpl())
            .Static.From.String(json);

        Console.WriteLine(val);

        Assert.That(val, Is.EqualTo(new DateOnly(2077, 7, 7)));
    }

    [Test]
    public void TestPrimitiveDate7()
    {
        var json = "\"7:07:07:07.0000000\"";

        var val = SeraJson.Deserializer
            .Deserialize<TimeSpan>()
            .Use(new PrimitiveImpl())
            .Static.From.String(json);

        Console.WriteLine(val);

        Assert.That(val, Is.EqualTo(new TimeSpan(7, 7, 7, 7)));
    }

    [Test]
    public void TestPrimitiveGuid1()
    {
        var json = "\"189819f1-1db6-4b57-be54-1821339b85f7\"";

        var val = SeraJson.Deserializer
            .Deserialize<Guid>()
            .Use(new PrimitiveImpl())
            .Static.From.String(json);

        Console.WriteLine(val);

        Assert.That(val, Is.EqualTo(new Guid("189819f1-1db6-4b57-be54-1821339b85f7")));
    }

    [Test]
    public void TestPrimitiveGuid2()
    {
        var json = "\"189819f11db64b57be541821339b85f7\"";

        var val = SeraJson.Deserializer
            .Deserialize<Guid>()
            .Use(new PrimitiveImpl())
            .Static.From.String(json);

        Console.WriteLine(val);

        Assert.That(val, Is.EqualTo(new Guid("189819f1-1db6-4b57-be54-1821339b85f7")));
    }

    [Test]
    public void TestPrimitiveGuid3()
    {
        var json = "\"{189819f1-1db6-4b57-be54-1821339b85f7}\"";

        var val = SeraJson.Deserializer
            .Deserialize<Guid>()
            .Use(new PrimitiveImpl())
            .Static.From.String(json);

        Console.WriteLine(val);

        Assert.That(val, Is.EqualTo(new Guid("189819f1-1db6-4b57-be54-1821339b85f7")));
    }

    [Test]
    public void TestPrimitiveGuid4()
    {
        var json = "\"{0x189819f1,0x1db6,0x4b57,{0xbe,0x54,0x18,0x21,0x33,0x9b,0x85,0xf7}}\"";

        var val = SeraJson.Deserializer
            .Deserialize<Guid>()
            .Use(new PrimitiveImpl())
            .Static.From.String(json);

        Console.WriteLine(val);

        Assert.That(val, Is.EqualTo(new Guid("189819f1-1db6-4b57-be54-1821339b85f7")));
    }

    [Test]
    public void TestPrimitiveGuid5()
    {
        var json = "\"(189819f1-1db6-4b57-be54-1821339b85f7)\"";

        var val = SeraJson.Deserializer
            .Deserialize<Guid>()
            .Use(new PrimitiveImpl())
            .Static.From.String(json);

        Console.WriteLine(val);

        Assert.That(val, Is.EqualTo(new Guid("189819f1-1db6-4b57-be54-1821339b85f7")));
    }

    [Test]
    public void TestPrimitiveIndex1()
    {
        var json = "123";

        var val = SeraJson.Deserializer
            .Deserialize<Index>()
            .Use(new PrimitiveImpl())
            .Static.From.String(json);

        Console.WriteLine(val);

        Assert.That(val, Is.EqualTo(new Index(123)));
    }

    [Test]
    public void TestPrimitiveIndex2()
    {
        var json = "\"^123\"";

        var val = SeraJson.Deserializer
            .Deserialize<Index>()
            .Use(new PrimitiveImpl())
            .Static.From.String(json);

        Console.WriteLine(val);

        Assert.That(val, Is.EqualTo(^123));
    }

    [Test]
    public void TestPrimitiveRange1()
    {
        var json = "\"123..456\"";

        var val = SeraJson.Deserializer
            .Deserialize<Range>()
            .Use(new PrimitiveImpl())
            .Static.From.String(json);

        Console.WriteLine(val);

        Assert.That(val, Is.EqualTo(123..456));
    }

    [Test]
    public void TestPrimitiveRange2()
    {
        var json = "\"^123..^456\"";

        var val = SeraJson.Deserializer
            .Deserialize<Range>()
            .Use(new PrimitiveImpl())
            .Static.From.String(json);

        Console.WriteLine(val);

        Assert.That(val, Is.EqualTo(^123..^456));
    }

    [Test]
    public void TestPrimitiveChar1()
    {
        var json = "\"a\"";

        var val = SeraJson.Deserializer
            .Deserialize<char>()
            .Use(new PrimitiveImpl())
            .Static.From.String(json);

        Console.WriteLine(val);

        Assert.That(val, Is.EqualTo('a'));
    }

    [Test]
    public void TestPrimitiveChar2()
    {
        var json = "\"⩟\"";

        var val = SeraJson.Deserializer
            .Deserialize<char>()
            .Use(new PrimitiveImpl())
            .Static.From.String(json);

        Console.WriteLine(val);

        Assert.That(val, Is.EqualTo('⩟'));
    }

    [Test]
    public void TestPrimitiveChar3()
    {
        var json = "\"\\u2a5f\"";

        var val = SeraJson.Deserializer
            .Deserialize<char>()
            .Use(new PrimitiveImpl())
            .Static.From.String(json);

        Console.WriteLine(val);

        Assert.That(val, Is.EqualTo('⩟'));
    }

    [Test]
    public void TestPrimitiveChar4()
    {
        var json = "\"\\n\"";

        var val = SeraJson.Deserializer
            .Deserialize<char>()
            .Use(new PrimitiveImpl())
            .Static.From.String(json);

        Console.WriteLine(val);

        Assert.That(val, Is.EqualTo('\n'));
    }

    [Test]
    public void TestPrimitiveRune1()
    {
        var json = "\"a\"";

        var val = SeraJson.Deserializer
            .Deserialize<Rune>()
            .Use(new PrimitiveImpl())
            .Static.From.String(json);

        Console.WriteLine(val);

        Assert.That(val, Is.EqualTo(new Rune('a')));
    }

    [Test]
    public void TestPrimitiveRune2()
    {
        var json = "\"😂\"";

        var val = SeraJson.Deserializer
            .Deserialize<Rune>()
            .Use(new PrimitiveImpl())
            .Static.From.String(json);

        Console.WriteLine(val);

        Assert.That(val, Is.EqualTo(new Rune(0x1F602)));
    }

    [Test]
    public void TestPrimitiveRune3()
    {
        var json = "\"\\uD83D\\uDE02\"";

        var val = SeraJson.Deserializer
            .Deserialize<Rune>()
            .Use(new PrimitiveImpl())
            .Static.From.String(json);

        Console.WriteLine(val);

        Assert.That(val, Is.EqualTo(new Rune(0x1F602)));
    }

    [Test]
    public void TestString1()
    {
        var json = "\"asd\"";

        var val = SeraJson.Deserializer
            .Deserialize<string>()
            .Use(new StringImpl())
            .Static.From.String(json);

        Console.WriteLine(val);

        Assert.That(val, Is.EqualTo("asd"));
    }

    [Test]
    public void TestString2()
    {
        var json = "\"asd镍😂\\n\"";

        var val = SeraJson.Deserializer
            .Deserialize<string>()
            .Use(new StringImpl())
            .Static.From.String(json);

        Console.WriteLine(val);

        Assert.That(val, Is.EqualTo("asd镍😂\n"));
    }

    [Test]
    public void TestString3()
    {
        var json = "\"asd\\u954D\\uD83D\\uDE02\\n\"";

        var val = SeraJson.Deserializer
            .Deserialize<string>()
            .Use(new StringImpl())
            .Static.From.String(json);

        Console.WriteLine(val);

        Assert.That(val, Is.EqualTo("asd镍😂\n"));
    }

    [Test]
    public void TestBytes1()
    {
        // ReSharper disable once StringLiteralTypo
        var json = "\"AQIDBAU=\"";

        var val = SeraJson.Deserializer
            .Deserialize<byte[]>()
            .Use(new BytesImpl())
            .Static.From.String(json);

        var bytes = string.Join(",", val);
        Console.WriteLine(bytes);

        Assert.That(bytes, Is.EqualTo("1,2,3,4,5"));
    }

    [Test]
    public void TestBytes2()
    {
        var json = "[1,2,3,4,5]";

        var val = SeraJson.Deserializer
            .Deserialize<byte[]>()
            .Use(new BytesImpl())
            .Static.From.String(json);

        var bytes = string.Join(",", val);
        Console.WriteLine(bytes);

        Assert.That(bytes, Is.EqualTo("1,2,3,4,5"));
    }

    [Test]
    public void TestArray1()
    {
        var json = "[1,2,3]";

        var val = SeraJson.Deserializer
            .Deserialize<int[]>()
            .Use(new ArrayImpl<int, PrimitiveImpl>(new()))
            .Static.From.String(json);

        var str = string.Join(",", val);
        Console.WriteLine(str);

        Assert.That(str, Is.EqualTo("1,2,3"));
    }

    [Test]
    public void TestUnit1()
    {
        var json = "null";

        var val = SeraJson.Deserializer
            .Deserialize<Unit>()
            .Use(new UnitImpl<Unit>())
            .Static.From.String(json);

        Console.WriteLine(val);

        Assert.That(val, Is.EqualTo(new Unit()));
    }

    [Test]
    public void TestOption1()
    {
        var json = "null";

        var val = SeraJson.Deserializer
            .Deserialize<int?>()
            .Use(new NullableImpl<int, PrimitiveImpl>(new()))
            .Static.From.String(json);

        Console.WriteLine(val);

        Assert.That(val, Is.EqualTo(null));
    }

    [Test]
    public void TestOption2()
    {
        var json = "123";

        var val = SeraJson.Deserializer
            .Deserialize<int?>()
            .Use(new NullableImpl<int, PrimitiveImpl>(new()))
            .Static.From.String(json);

        Console.WriteLine(val);

        Assert.That(val, Is.EqualTo((int?)123));
    }

    [Test]
    public void TestEntry1()
    {
        var json = "[1,2]";

        var val = SeraJson.Deserializer
            .Deserialize<KeyValuePair<int, int>>()
            .Use(new EntryImpl<int, int, PrimitiveImpl, PrimitiveImpl>(new(), new()))
            .Static.From.String(json);

        Console.WriteLine(val);

        Assert.That(val, Is.EqualTo(new KeyValuePair<int, int>(1, 2)));
    }

    [Test]
    public void TestTuple1()
    {
        var json = "[1,2]";

        var val = SeraJson.Deserializer
            .Deserialize<(int, int)>()
            .Use(new TupleImpl<int, int, PrimitiveImpl, PrimitiveImpl>(new(), new()))
            .Static.From.String(json);

        Console.WriteLine(val);

        Assert.That(val, Is.EqualTo((1, 2)));
    }

    [Test]
    public void TestTuple2()
    {
        var json = "[1,2]";

        var val = SeraJson.Deserializer
            .Deserialize<List<int>>()
            .Use(new TupleListImpl<int, PrimitiveImpl>(new(), null))
            .Static.From.String(json);

        Console.WriteLine(val);

        Assert.That(string.Join(",", val), Is.EqualTo("1,2"));
    }

    [Test]
    public void TestTuple3()
    {
        var json = "[1,2,3,4,5,6,7,8]";

        var impl =
            new TupleRestValueImpl<int, int, int, int, int, int, int, ValueTuple<int>,
                    PrimitiveImpl, PrimitiveImpl, PrimitiveImpl, PrimitiveImpl,
                    PrimitiveImpl, PrimitiveImpl, PrimitiveImpl, TupleImpl<int, PrimitiveImpl>>
                (new(), new(), new(), new(), new(), new(), new(), new(new()), 8);

        var val = SeraJson.Deserializer
            .Deserialize<(int, int, int, int, int, int, int, int)>()
            .Use(impl)
            .Static.From.String(json);

        Console.WriteLine(val);

        Assert.That(val, Is.EqualTo((1, 2, 3, 4, 5, 6, 7, 8)));
    }

    [Test]
    public void TestTuple4()
    {
        var json = "[1,2,3,4,5,6,7,8,9]";

        var impl =
            new TupleRestValueImpl<int, int, int, int, int, int, int, (int, int),
                    PrimitiveImpl, PrimitiveImpl, PrimitiveImpl, PrimitiveImpl,
                    PrimitiveImpl, PrimitiveImpl, PrimitiveImpl, TupleImpl<int, int, PrimitiveImpl, PrimitiveImpl>>
                (new(), new(), new(), new(), new(), new(), new(), new(new(), new()), 9);

        var val = SeraJson.Deserializer
            .Deserialize<(int, int, int, int, int, int, int, int, int)>()
            .Use(impl)
            .Static.From.String(json);

        Console.WriteLine(val);

        Assert.That(val, Is.EqualTo((1, 2, 3, 4, 5, 6, 7, 8, 9)));
    }

    [Test]
    public void TestSeq1()
    {
        var json = "[1,2,3,4,5]";

        var val = SeraJson.Deserializer
            .Deserialize<List<int>>()
            .Use(new SeqListImpl<int, PrimitiveImpl>(new()))
            .Static.From.String(json);

        var str = string.Join(",", val);
        Console.WriteLine(str);

        Assert.That(str, Is.EqualTo("1,2,3,4,5"));
    }

    [Test]
    public void TestMap1()
    {
        var json = "{\"a\":1,\"b\":2}";

        var val = SeraJson.Deserializer
            .Deserialize<Dictionary<string, int>>()
            .Use(new MapDictionaryImpl<string, int, StringImpl, PrimitiveImpl>(new(), new()))
            .Static.From.String(json);

        var str = string.Join(",", val);
        Console.WriteLine(str);

        Assert.That(str, Is.EqualTo("[a, 1],[b, 2]"));
    }

    [Test]
    public void TestMap2()
    {
        var json = "[[\"a\",1],[\"b\",2]]";

        var val = SeraJson.Deserializer
            .Deserialize<Dictionary<string, int>>()
            .Use(new MapDictionaryImpl<string, int, StringImpl, PrimitiveImpl>(new(), new()))
            .Static.From.String(json);

        var str = string.Join(",", val);
        Console.WriteLine(str);

        Assert.That(str, Is.EqualTo("[a, 1],[b, 2]"));
    }

    [Test]
    public void TestMap3()
    {
        var json = "[[1,2],[3,4]]";

        var val = SeraJson.Deserializer
            .Deserialize<Dictionary<int, int>>()
            .Use(new MapDictionaryImpl<int, int, PrimitiveImpl, PrimitiveImpl>(new(), new()))
            .Static.From.String(json);

        var str = string.Join(",", val);
        Console.WriteLine(str);

        Assert.That(str, Is.EqualTo("[1, 2],[3, 4]"));
    }

    [Test]
    public void TestMap4()
    {
        var json = "{\"1\":2,\"3\":4}";

        var val = SeraJson.Deserializer
            .Deserialize<Dictionary<int, int>>()
            .Use(new MapDictionaryImpl<int, int, PrimitiveImpl, PrimitiveImpl>(new(), new()))
            .Static.From.String(json);

        var str = string.Join(",", val);
        Console.WriteLine(str);

        Assert.That(str, Is.EqualTo("[1, 2],[3, 4]"));
    }

    [Test]
    public void TestMap5()
    {
        var json = "{\"[1,2]\":3,\"[4,5]\":6}";

        var val = SeraJson.Deserializer
            .Deserialize<Dictionary<(int, int), int>>()
            .Use(new MapDictionaryImpl<(int, int), int, TupleImpl<int, int, PrimitiveImpl, PrimitiveImpl>,
                PrimitiveImpl>(new(new(), new()), new()))
            .Static.From.String(json);

        var str = string.Join(",", val);
        Console.WriteLine(str);

        Assert.That(str, Is.EqualTo("[(1, 2), 3],[(4, 5), 6]"));
    }

    #region TestStruct1

    [Test]
    public void TestStruct1()
    {
        var json = "{\"A\":123,\"B\":\"asd\"}";

        var val = SeraJson.Deserializer
            .Deserialize<ClassTestStruct1>()
            .Use(new ClassTestStruct1Impl())
            .Static.From.String(json);

        Console.WriteLine(val);

        Assert.That(val, Is.EqualTo(new ClassTestStruct1 { A = 123, B = "asd" }));
    }

    private record ClassTestStruct1
    {
        public int A { get; set; }
        public string? B { get; set; }
    }

    private readonly struct ClassTestStruct1Impl : ISeraColion<ClassTestStruct1>, IStructSeraColion<ClassTestStruct1>
    {
        public R Collect<R, C>(ref C colctor, InType<ClassTestStruct1>? t = null)
            where C : ISeraColctor<ClassTestStruct1, R>
            => colctor.CStruct(this, new IdentityMapper<ClassTestStruct1>(), new Type<ClassTestStruct1>());

        public SeraFieldInfos? Fields => _fields;

        private static readonly SeraFieldInfos? _fields = new SeraFieldInfos([
            new(nameof(ClassTestStruct1.A), 0),
            new(nameof(ClassTestStruct1.B), 1),
        ]);

        public ClassTestStruct1 Builder(string? name, Type<ClassTestStruct1> b = default)
            => new();

        public R CollectField<R, C>(ref C colctor, int field, string? name, long? key,
            Type<ClassTestStruct1> b = default) where C : IStructSeraColctor<ClassTestStruct1, R>
            => field switch
            {
                0 => colctor.CField(new PrimitiveImpl(), new FieldEffectorA(), new Type<int>()),
                1 => colctor.CField(new StringImpl(), new FieldEffectorB(), new Type<string>()),
                _ => colctor.CNone(),
            };

        private readonly struct FieldEffectorA : ISeraEffector<ClassTestStruct1, int>
        {
            public void Effect(ref ClassTestStruct1 target, int value)
                => target.A = value;
        }

        private readonly struct FieldEffectorB : ISeraEffector<ClassTestStruct1, string>
        {
            public void Effect(ref ClassTestStruct1 target, string value)
                => target.B = value;
        }
    }

    #endregion

    [Test]
    public void TestStruct2()
    {
        var json = "{\"A\":123,\"B\":\"asd\"}";

        var val = SeraJson.Deserializer
            .Deserialize<AnyStruct>()
            .Use(new AnyStructImpl())
            .Static.From.String(json);

        var str = SeraJson.Serializer
            .Serialize(val)
            .Use(new Sera.Core.Impls.Ser.AnyStructImpl(new(), val))
            .Static.To.String();

        Console.WriteLine(str);

        Assert.That(str, Is.EqualTo(json));
    }
    
    [Test]
    public void TestAny1()
    {
        var json = "{\"A\":123,\"B\":\"asd\"}";

        var val = SeraJson.Deserializer
            .Deserialize<Any>()
            .Use(new AnyImpl())
            .Static.From.String(json);

        var str = SeraJson.Serializer
            .Serialize(val)
            .Use(new Sera.Core.Impls.Ser.AnyImpl())
            .Static.To.String();

        Console.WriteLine(str);

        Assert.That(str, Is.EqualTo("[[\"A\",123],[\"B\",\"asd\"]]"));
    }
}
