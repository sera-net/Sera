using System.Numerics;
using System.Text;
using Sera.Core;
using Sera.Core.Impls;
using Sera.Core.Ser;
using Sera.Json;
using Sera.Json.Ser;

namespace TestJson;

public class Tests
{
    [SetUp]
    public void Setup() { }

    [Test]
    public void TestPrimitiveBoolean1()
    {
        using var stream = new MemoryStream();

        SeraJson.Serializer
            .ToStream(stream)
            .SerializeStatic(true, PrimitiveImpl.Boolean);

        stream.Position = 0;
        using var reader = new StreamReader(stream, Encoding.UTF8);
        var str = reader.ReadToEnd();
        Console.WriteLine(str);

        Assert.That(str, Is.EqualTo("true"));
    }

    [Test]
    public void TestPrimitiveBoolean2()
    {
        using var stream = new MemoryStream();

        SeraJson.Serializer
            .ToStream(stream)
            .SerializeStatic(false, PrimitiveImpl.Boolean);

        stream.Position = 0;
        using var reader = new StreamReader(stream, Encoding.UTF8);
        var str = reader.ReadToEnd();
        Console.WriteLine(str);

        Assert.That(str, Is.EqualTo("false"));
    }

    [Test]
    public void TestPrimitiveNumber1()
    {
        using var stream = new MemoryStream();

        SeraJson.Serializer
            .ToStream(stream)
            .SerializeStatic(123, PrimitiveImpl.Int32);

        stream.Position = 0;
        using var reader = new StreamReader(stream, Encoding.UTF8);
        var str = reader.ReadToEnd();
        Console.WriteLine(str);

        Assert.That(str, Is.EqualTo("123"));
    }

    [Test]
    public void TestPrimitiveNumber2()
    {
        using var stream = new MemoryStream();

        SeraJson.Serializer
            .ToStream(stream)
            .SerializeStatic(123.456, PrimitiveImpl.Double);

        stream.Position = 0;
        using var reader = new StreamReader(stream, Encoding.UTF8);
        var str = reader.ReadToEnd();
        Console.WriteLine(str);

        Assert.That(str, Is.EqualTo("123.456"));
    }

    [Test]
    public void TestPrimitiveNumber3()
    {
        using var stream = new MemoryStream();

        SeraJson.Serializer
            .ToStream(stream)
            .SerializeStatic(123UL, PrimitiveImpl.UInt64);

        stream.Position = 0;
        using var reader = new StreamReader(stream, Encoding.UTF8);
        var str = reader.ReadToEnd();
        Console.WriteLine(str);

        Assert.That(str, Is.EqualTo("\"123\""));
    }

    [Test]
    public void TestPrimitiveNumber4()
    {
        using var stream = new MemoryStream();

        SeraJson.Serializer
            .ToStream(stream)
            .WithFormatter(CompactJsonFormatter.Default with { LargeNumberUseString = false })
            .SerializeStatic(123UL, PrimitiveImpl.UInt64);

        stream.Position = 0;
        using var reader = new StreamReader(stream, Encoding.UTF8);
        var str = reader.ReadToEnd();
        Console.WriteLine(str);

        Assert.That(str, Is.EqualTo("123"));
    }

    [Test]
    public void TestPrimitiveNumber5()
    {
        using var stream = new MemoryStream();

        SeraJson.Serializer
            .ToStream(stream)
            .WithFormatter(CompactJsonFormatter.Default)
            .SerializeStatic(new Complex(1, 2), PrimitiveImpl.Complex);

        stream.Position = 0;
        using var reader = new StreamReader(stream, Encoding.UTF8);
        var str = reader.ReadToEnd();
        Console.WriteLine(str);

        Assert.That(str, Is.EqualTo("\"<1; 2>\""));
    }

    [Test]
    public void TestPrimitiveNumber6()
    {
        using var stream = new MemoryStream();

        SeraJson.Serializer
            .ToStream(stream)
            .WithFormatter(CompactJsonFormatter.Default)
            .SerializeStatic(new BigInteger(12345678), PrimitiveImpl.BigInteger);

        stream.Position = 0;
        using var reader = new StreamReader(stream, Encoding.UTF8);
        var str = reader.ReadToEnd();
        Console.WriteLine(str);

        Assert.That(str, Is.EqualTo("\"12345678\""));
    }

    [Test]
    public void TestPrimitiveDate1()
    {
        using var stream = new MemoryStream();

        SeraJson.Serializer
            .ToStream(stream)
            .WithOptions(SeraJsonOptions.Default with { TimeZone = TimeZoneInfo.Utc })
            .SerializeStatic(new DateTime(2077, 7, 7, 7, 7, 7, DateTimeKind.Utc), PrimitiveImpl.DateTime);

        stream.Position = 0;
        using var reader = new StreamReader(stream, Encoding.UTF8);
        var str = reader.ReadToEnd();
        Console.WriteLine(str);

        Assert.That(str, Is.EqualTo("\"2077-07-07T07:07:07.0000000Z\""));
    }

    [Test]
    public void TestPrimitiveDate2()
    {
        using var stream = new MemoryStream();

        SeraJson.Serializer
            .ToStream(stream)
            .WithOptions(SeraJsonOptions.Default with { TimeZone = TimeZoneInfo.Utc })
            .SerializeStatic(new DateTimeOffset(2077, 7, 7, 7, 7, 7, TimeSpan.Zero), PrimitiveImpl.DateTimeOffset);

        stream.Position = 0;
        using var reader = new StreamReader(stream, Encoding.UTF8);
        var str = reader.ReadToEnd();
        Console.WriteLine(str);

        Assert.That(str, Is.EqualTo("\"2077-07-07T07:07:07.0000000+00:00\""));
    }

    [Test]
    public void TestPrimitiveDate3()
    {
        using var stream = new MemoryStream();

        SeraJson.Serializer
            .ToStream(stream)
            .WithOptions(SeraJsonOptions.Default with { TimeZone = TimeZoneInfo.Utc })
            .SerializeStatic(new DateOnly(2077, 7, 7), PrimitiveImpl.DateOnly);

        stream.Position = 0;
        using var reader = new StreamReader(stream, Encoding.UTF8);
        var str = reader.ReadToEnd();
        Console.WriteLine(str);

        Assert.That(str, Is.EqualTo("\"2077-07-07\""));
    }

    [Test]
    public void TestPrimitiveDate4()
    {
        using var stream = new MemoryStream();

        SeraJson.Serializer
            .ToStream(stream)
            .WithOptions(SeraJsonOptions.Default with { TimeZone = TimeZoneInfo.Utc })
            .SerializeStatic(new TimeOnly(7, 7, 7), PrimitiveImpl.TimeOnly);

        stream.Position = 0;
        using var reader = new StreamReader(stream, Encoding.UTF8);
        var str = reader.ReadToEnd();
        Console.WriteLine(str);

        Assert.That(str, Is.EqualTo("\"07:07:07.0000000\""));
    }

    [Test]
    public void TestPrimitiveDate5()
    {
        using var stream = new MemoryStream();

        SeraJson.Serializer
            .ToStream(stream)
            .WithOptions(SeraJsonOptions.Default with { TimeZone = TimeZoneInfo.Utc })
            .SerializeStatic(new TimeSpan(7, 7, 7, 7), PrimitiveImpl.TimeSpan);

        stream.Position = 0;
        using var reader = new StreamReader(stream, Encoding.UTF8);
        var str = reader.ReadToEnd();
        Console.WriteLine(str);

        Assert.That(str, Is.EqualTo("\"7:07:07:07.0000000\""));
    }

    [Test]
    public void TestPrimitiveGuid1()
    {
        using var stream = new MemoryStream();

        SeraJson.Serializer
            .ToStream(stream)
            .WithFormatter(CompactJsonFormatter.Default)
            .SerializeStatic(new Guid("189819f1-1db6-4b57-be54-1821339b85f7"), PrimitiveImpl.Guid);

        stream.Position = 0;
        using var reader = new StreamReader(stream, Encoding.UTF8);
        var str = reader.ReadToEnd();
        Console.WriteLine(str);

        Assert.That(str, Is.EqualTo("\"189819f1-1db6-4b57-be54-1821339b85f7\""));
    }

    [Test]
    public void TestPrimitiveGuid2()
    {
        using var stream = new MemoryStream();

        SeraJson.Serializer
            .ToStream(stream)
            .WithFormatter(CompactJsonFormatter.Default)
            .SerializeStatic(new Guid("189819f1-1db6-4b57-be54-1821339b85f7"),
                PrimitiveImpl.Guid with { Hint = SerializerPrimitiveHint.GuidFormatShort });

        stream.Position = 0;
        using var reader = new StreamReader(stream, Encoding.UTF8);
        var str = reader.ReadToEnd();
        Console.WriteLine(str);

        Assert.That(str, Is.EqualTo("\"189819f11db64b57be541821339b85f7\""));
    }

    [Test]
    public void TestPrimitiveGuid3()
    {
        using var stream = new MemoryStream();

        SeraJson.Serializer
            .ToStream(stream)
            .WithFormatter(CompactJsonFormatter.Default)
            .SerializeStatic(new Guid("189819f1-1db6-4b57-be54-1821339b85f7"),
                PrimitiveImpl.Guid with { Hint = SerializerPrimitiveHint.GuidFormatBraces });

        stream.Position = 0;
        using var reader = new StreamReader(stream, Encoding.UTF8);
        var str = reader.ReadToEnd();
        Console.WriteLine(str);

        Assert.That(str, Is.EqualTo("\"{189819f1-1db6-4b57-be54-1821339b85f7}\""));
    }

    [Test]
    public void TestPrimitiveGuid4()
    {
        using var stream = new MemoryStream();

        SeraJson.Serializer
            .ToStream(stream)
            .WithFormatter(CompactJsonFormatter.Default)
            .SerializeStatic(new Guid("189819f1-1db6-4b57-be54-1821339b85f7"),
                PrimitiveImpl.Guid with { Hint = SerializerPrimitiveHint.GuidFormatHex });

        stream.Position = 0;
        using var reader = new StreamReader(stream, Encoding.UTF8);
        var str = reader.ReadToEnd();
        Console.WriteLine(str);

        Assert.That(str, Is.EqualTo("\"{0x189819f1,0x1db6,0x4b57,{0xbe,0x54,0x18,0x21,0x33,0x9b,0x85,0xf7}}\""));
    }

    [Test]
    public void TestPrimitiveGuid5()
    {
        using var stream = new MemoryStream();

        SeraJson.Serializer
            .ToStream(stream)
            .WithFormatter(CompactJsonFormatter.Default)
            .SerializeStatic(new Guid("189819f1-1db6-4b57-be54-1821339b85f7"),
                PrimitiveImpl.Guid with { Hint = SerializerPrimitiveHint.GuidFormatGuid });

        stream.Position = 0;
        using var reader = new StreamReader(stream, Encoding.UTF8);
        var str = reader.ReadToEnd();
        Console.WriteLine(str);

        Assert.That(str, Is.EqualTo("\"189819f1-1db6-4b57-be54-1821339b85f7\""));
    }


    [Test]
    public void TestPrimitiveRange1()
    {
        using var stream = new MemoryStream();

        SeraJson.Serializer
            .ToStream(stream)
            .WithFormatter(CompactJsonFormatter.Default)
            .SerializeStatic(new Range(1, 2), PrimitiveImpl.Range);

        stream.Position = 0;
        using var reader = new StreamReader(stream, Encoding.UTF8);
        var str = reader.ReadToEnd();
        Console.WriteLine(str);

        Assert.That(str, Is.EqualTo("\"1..2\""));
    }

    [Test]
    public void TestPrimitiveRange2()
    {
        using var stream = new MemoryStream();

        SeraJson.Serializer
            .ToStream(stream)
            .WithFormatter(CompactJsonFormatter.Default)
            .SerializeStatic(new Range(1, ^2), PrimitiveImpl.Range);

        stream.Position = 0;
        using var reader = new StreamReader(stream, Encoding.UTF8);
        var str = reader.ReadToEnd();
        Console.WriteLine(str);

        Assert.That(str, Is.EqualTo("\"1..^2\""));
    }

    [Test]
    public void TestPrimitiveIndex1()
    {
        using var stream = new MemoryStream();

        SeraJson.Serializer
            .ToStream(stream)
            .WithFormatter(CompactJsonFormatter.Default)
            .SerializeStatic(new Index(1), PrimitiveImpl.Index);

        stream.Position = 0;
        using var reader = new StreamReader(stream, Encoding.UTF8);
        var str = reader.ReadToEnd();
        Console.WriteLine(str);

        Assert.That(str, Is.EqualTo("\"1\""));
    }

    [Test]
    public void TestPrimitiveIndex2()
    {
        using var stream = new MemoryStream();

        SeraJson.Serializer
            .ToStream(stream)
            .WithFormatter(CompactJsonFormatter.Default)
            .SerializeStatic(^1, PrimitiveImpl.Index);

        stream.Position = 0;
        using var reader = new StreamReader(stream, Encoding.UTF8);
        var str = reader.ReadToEnd();
        Console.WriteLine(str);

        Assert.That(str, Is.EqualTo("\"^1\""));
    }

    [Test]
    public void TestPrimitiveChar1()
    {
        using var stream = new MemoryStream();

        SeraJson.Serializer
            .ToStream(stream)
            .WithFormatter(CompactJsonFormatter.Default)
            .SerializeStatic('a', PrimitiveImpl.Char);

        stream.Position = 0;
        using var reader = new StreamReader(stream, Encoding.UTF8);
        var str = reader.ReadToEnd();
        Console.WriteLine(str);

        Assert.That(str, Is.EqualTo("\"a\""));
    }

    [Test]
    public void TestPrimitiveChar2()
    {
        using var stream = new MemoryStream();

        SeraJson.Serializer
            .ToStream(stream)
            .WithFormatter(CompactJsonFormatter.Default)
            .SerializeStatic('\u2a5f', PrimitiveImpl.Char);

        stream.Position = 0;
        using var reader = new StreamReader(stream, Encoding.UTF8);
        var str = reader.ReadToEnd();
        Console.WriteLine(str);

        Assert.That(str, Is.EqualTo("\"⩟\""));
    }

    [Test]
    public void TestPrimitiveChar3()
    {
        using var stream = new MemoryStream();

        SeraJson.Serializer
            .ToStream(stream)
            .WithFormatter(CompactJsonFormatter.Default with { EscapeAllNonAsciiChar = true })
            .SerializeStatic('\u2a5f', PrimitiveImpl.Char);

        stream.Position = 0;
        using var reader = new StreamReader(stream, Encoding.UTF8);
        var str = reader.ReadToEnd();
        Console.WriteLine(str);

        Assert.That(str, Is.EqualTo("\"\\u2A5F\""));
    }

    [Test]
    public void TestPrimitiveChar4()
    {
        using var stream = new MemoryStream();

        SeraJson.Serializer
            .ToStream(stream)
            .WithFormatter(CompactJsonFormatter.Default with { EscapeAllNonAsciiChar = false })
            .SerializeStatic('\n', PrimitiveImpl.Char);

        stream.Position = 0;
        using var reader = new StreamReader(stream, Encoding.UTF8);
        var str = reader.ReadToEnd();
        Console.WriteLine(str);

        Assert.That(str, Is.EqualTo("\"\\n\""));
    }

    [Test]
    public void TestPrimitiveRune1()
    {
        using var stream = new MemoryStream();

        SeraJson.Serializer
            .ToStream(stream)
            .WithFormatter(CompactJsonFormatter.Default with { EscapeAllNonAsciiChar = true })
            .SerializeStatic(new Rune('a'), PrimitiveImpl.Rune);

        stream.Position = 0;
        using var reader = new StreamReader(stream, Encoding.UTF8);
        var str = reader.ReadToEnd();
        Console.WriteLine(str);

        Assert.That(str, Is.EqualTo("\"a\""));
    }

    [Test]
    public void TestPrimitiveRune2()
    {
        using var stream = new MemoryStream();

        SeraJson.Serializer
            .ToStream(stream)
            .WithFormatter(CompactJsonFormatter.Default with { EscapeAllNonAsciiChar = false })
            .SerializeStatic(new Rune(0x1F602), PrimitiveImpl.Rune);

        stream.Position = 0;
        using var reader = new StreamReader(stream, Encoding.UTF8);
        var str = reader.ReadToEnd();
        Console.WriteLine(str);

        Assert.That(str, Is.EqualTo("\"😂\""));
    }

    [Test]
    public void TestPrimitiveRune3()
    {
        using var stream = new MemoryStream();

        SeraJson.Serializer
            .ToStream(stream)
            .WithFormatter(CompactJsonFormatter.Default with { EscapeAllNonAsciiChar = true })
            .SerializeStatic(new Rune(0x1F602), PrimitiveImpl.Rune);

        stream.Position = 0;
        using var reader = new StreamReader(stream, Encoding.UTF8);
        var str = reader.ReadToEnd();
        Console.WriteLine(str);

        Assert.That(str, Is.EqualTo("\"\\uD83D\\uDE02\""));
    }

    [Test]
    public void TestString1()
    {
        using var stream = new MemoryStream();

        SeraJson.Serializer
            .ToStream(stream)
            .WithFormatter(CompactJsonFormatter.Default with { EscapeAllNonAsciiChar = false })
            .SerializeStatic("asd", StringImpl.Instance);

        stream.Position = 0;
        using var reader = new StreamReader(stream, Encoding.UTF8);
        var str = reader.ReadToEnd();
        Console.WriteLine(str);

        Assert.That(str, Is.EqualTo("\"asd\""));
    }

    [Test]
    public void TestString2()
    {
        using var stream = new MemoryStream();

        SeraJson.Serializer
            .ToStream(stream)
            .WithFormatter(CompactJsonFormatter.Default with { EscapeAllNonAsciiChar = false })
            .SerializeStatic("asd镍😂\n", StringImpl.Instance);

        stream.Position = 0;
        using var reader = new StreamReader(stream, Encoding.UTF8);
        var str = reader.ReadToEnd();
        Console.WriteLine(str);

        Assert.That(str, Is.EqualTo("\"asd镍😂\\n\""));
    }

    [Test]
    public void TestString3()
    {
        using var stream = new MemoryStream();

        SeraJson.Serializer
            .ToStream(stream)
            .WithFormatter(CompactJsonFormatter.Default with { EscapeAllNonAsciiChar = true })
            .SerializeStatic("asd镍😂\n", StringImpl.Instance);

        stream.Position = 0;
        using var reader = new StreamReader(stream, Encoding.UTF8);
        var str = reader.ReadToEnd();
        Console.WriteLine(str);

        Assert.That(str, Is.EqualTo("\"asd\\u954D\\uD83D\\uDE02\\n\""));
    }

    [Test]
    public void TestBytes1()
    {
        using var stream = new MemoryStream();

        SeraJson.Serializer
            .ToStream(stream)
            .WithFormatter(CompactJsonFormatter.Default with { Base64Bytes = true })
            .SerializeStatic(new byte[] { 1, 2, 3, 4, 5 }, BytesImpl.Instance);

        stream.Position = 0;
        using var reader = new StreamReader(stream, Encoding.UTF8);
        var str = reader.ReadToEnd();
        Console.WriteLine(str);

        // ReSharper disable once StringLiteralTypo
        Assert.That(str, Is.EqualTo("\"AQIDBAU=\""));
    }

    [Test]
    public void TestBytes2()
    {
        using var stream = new MemoryStream();

        SeraJson.Serializer
            .ToStream(stream)
            .WithFormatter(CompactJsonFormatter.Default with { Base64Bytes = false })
            .SerializeStatic(new byte[] { 1, 2, 3, 4, 5 }, BytesImpl.Instance);

        stream.Position = 0;
        using var reader = new StreamReader(stream, Encoding.UTF8);
        var str = reader.ReadToEnd();
        Console.WriteLine(str);

        Assert.That(str, Is.EqualTo("[1,2,3,4,5]"));
    }

    [Test]
    public void TestUnit1()
    {
        using var stream = new MemoryStream();

        SeraJson.Serializer
            .ToStream(stream)
            .WithFormatter(CompactJsonFormatter.Default)
            .SerializeStatic(Unit.New, UnitImpl<Unit>.Instance);

        stream.Position = 0;
        using var reader = new StreamReader(stream, Encoding.UTF8);
        var str = reader.ReadToEnd();
        Console.WriteLine(str);

        Assert.That(str, Is.EqualTo("null"));
    }
}
