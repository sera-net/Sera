using System.Numerics;
using System.Text;
using Sera;
using Sera.Core;
using Sera.Core.Formats;
using Sera.Core.Impls.Ser;
using Sera.Json;
using Sera.Json.Ser;

namespace TestJson.TestSerializer;

public class TestStaticSerializer
{
    [Test]
    public void TestPrimitiveBoolean1()
    {
        using var stream = new MemoryStream();

        var val = true;

        SeraJson.Serializer
            .Serialize(val)
            .Use(new PrimitiveImpl())
            .Static.To.Stream(stream);

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

        var val = false;

        SeraJson.Serializer
            .Serialize(val)
            .Use(new PrimitiveImpl())
            .Static.To.Stream(stream);

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

        var val = 123;

        SeraJson.Serializer
            .Serialize(val)
            .Use(new PrimitiveImpl())
            .Static.To.Stream(stream);

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

        var val = 123.456;

        SeraJson.Serializer
            .Serialize(val)
            .Use(new PrimitiveImpl())
            .Static.To.Stream(stream);

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

        var val = 123UL;

        SeraJson.Serializer
            .Serialize(val)
            .Use(new PrimitiveImpl())
            .Static.To.Stream(stream);

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

        var val = 123UL;

        SeraJson.Serializer
            .WithFormatter(CompactJsonFormatter.Default with { LargeNumberUseString = false })
            .Serialize(val)
            .Use(new PrimitiveImpl())
            .Static.To.Stream(stream);

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

        var val = new Complex(1, 2);

        SeraJson.Serializer
            .Serialize(val)
            .Use(new PrimitiveImpl())
            .Static.To.Stream(stream);

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

        var val = new BigInteger(12345678);

        SeraJson.Serializer
            .Serialize(val)
            .Use(new PrimitiveImpl())
            .Static.To.Stream(stream);

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

        var val = new DateTime(2077, 7, 7, 7, 7, 7, DateTimeKind.Utc);

        SeraJson.Serializer
            .WithOptions(SeraJsonOptions.Default with { TimeZone = TimeZoneInfo.Utc })
            .Serialize(val)
            .Use(new PrimitiveImpl())
            .Static.To.Stream(stream);

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

        var val = new DateTimeOffset(2077, 7, 7, 7, 7, 7, TimeSpan.Zero);

        SeraJson.Serializer
            .WithOptions(SeraJsonOptions.Default with { TimeZone = TimeZoneInfo.Utc })
            .Serialize(val)
            .Use(new PrimitiveImpl())
            .Static.To.Stream(stream);

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

        var val = new DateOnly(2077, 7, 7);

        SeraJson.Serializer
            .WithOptions(SeraJsonOptions.Default with { TimeZone = TimeZoneInfo.Utc })
            .Serialize(val)
            .Use(new PrimitiveImpl())
            .Static.To.Stream(stream);

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

        var val = new TimeOnly(7, 7, 7);

        SeraJson.Serializer
            .WithOptions(SeraJsonOptions.Default with { TimeZone = TimeZoneInfo.Utc })
            .Serialize(val)
            .Use(new PrimitiveImpl())
            .Static.To.Stream(stream);

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

        var val = new TimeSpan(7, 7, 7, 7);

        SeraJson.Serializer
            .WithOptions(SeraJsonOptions.Default with { TimeZone = TimeZoneInfo.Utc })
            .Serialize(val)
            .Use(new PrimitiveImpl())
            .Static.To.Stream(stream);

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

        var val = new Guid("189819f1-1db6-4b57-be54-1821339b85f7");

        SeraJson.Serializer
            .Serialize(val)
            .Use(new PrimitiveImpl())
            .Static.To.Stream(stream);

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

        var val = new Guid("189819f1-1db6-4b57-be54-1821339b85f7");

        SeraJson.Serializer
            .Serialize(val)
            .Use(new PrimitiveImpl(SeraFormats.Default with { GuidTextFormat = GuidTextFormat.GuidTextShort }))
            .Static.To.Stream(stream);

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

        var val = new Guid("189819f1-1db6-4b57-be54-1821339b85f7");

        SeraJson.Serializer
            .Serialize(val)
            .Use(new PrimitiveImpl(SeraFormats.Default with { GuidTextFormat = GuidTextFormat.GuidTextBraces }))
            .Static.To.Stream(stream);

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

        var val = new Guid("189819f1-1db6-4b57-be54-1821339b85f7");

        SeraJson.Serializer
            .Serialize(val)
            .Use(new PrimitiveImpl(SeraFormats.Default with { GuidTextFormat = GuidTextFormat.GuidTextHex }))
            .Static.To.Stream(stream);

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

        var val = new Guid("189819f1-1db6-4b57-be54-1821339b85f7");

        SeraJson.Serializer
            .Serialize(val)
            .Use(new PrimitiveImpl(SeraFormats.Default with { GuidTextFormat = GuidTextFormat.GuidTextGuid }))
            .Static.To.Stream(stream);

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

        var val = new Range(1, 2);

        SeraJson.Serializer
            .Serialize(val)
            .Use(new PrimitiveImpl())
            .Static.To.Stream(stream);

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

        var val = new Range(1, ^2);

        SeraJson.Serializer
            .Serialize(val)
            .Use(new PrimitiveImpl())
            .Static.To.Stream(stream);

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

        var val = new Index(1);

        SeraJson.Serializer
            .Serialize(val)
            .Use(new PrimitiveImpl())
            .Static.To.Stream(stream);

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

        var val = ^1;

        SeraJson.Serializer
            .Serialize(val)
            .Use(new PrimitiveImpl())
            .Static.To.Stream(stream);

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

        var val = 'a';

        SeraJson.Serializer
            .Serialize(val)
            .Use(new PrimitiveImpl())
            .Static.To.Stream(stream);

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

        var val = '\u2a5f';

        SeraJson.Serializer
            .Serialize(val)
            .Use(new PrimitiveImpl())
            .Static.To.Stream(stream);

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

        var val = '\u2a5f';

        SeraJson.Serializer
            .WithFormatter(CompactJsonFormatter.Default with { EscapeAllNonAsciiChar = true })
            .Serialize(val)
            .Use(new PrimitiveImpl())
            .Static.To.Stream(stream);

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

        var val = '\n';

        SeraJson.Serializer
            .WithFormatter(CompactJsonFormatter.Default with { EscapeAllNonAsciiChar = false })
            .Serialize(val)
            .Use(new PrimitiveImpl())
            .Static.To.Stream(stream);

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

        var val = new Rune('a');

        SeraJson.Serializer
            .WithFormatter(CompactJsonFormatter.Default with { EscapeAllNonAsciiChar = true })
            .Serialize(val)
            .Use(new PrimitiveImpl())
            .Static.To.Stream(stream);

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

        var val = new Rune(0x1F602);

        SeraJson.Serializer
            .WithFormatter(CompactJsonFormatter.Default with { EscapeAllNonAsciiChar = false })
            .Serialize(val)
            .Use(new PrimitiveImpl())
            .Static.To.Stream(stream);

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

        var val = new Rune(0x1F602);

        SeraJson.Serializer
            .WithFormatter(CompactJsonFormatter.Default with { EscapeAllNonAsciiChar = true })
            .Serialize(val)
            .Use(new PrimitiveImpl())
            .Static.To.Stream(stream);

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

        var val = "asd";

        SeraJson.Serializer
            .WithFormatter(CompactJsonFormatter.Default with { EscapeAllNonAsciiChar = false })
            .Serialize(val)
            .Use(new StringImpl())
            .Static.To.Stream(stream);

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

        var val = "asd镍😂\n";

        SeraJson.Serializer
            .WithFormatter(CompactJsonFormatter.Default with { EscapeAllNonAsciiChar = false })
            .Serialize(val)
            .Use(new StringImpl())
            .Static.To.Stream(stream);

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

        var val = "asd镍😂\n";

        SeraJson.Serializer
            .WithFormatter(CompactJsonFormatter.Default with { EscapeAllNonAsciiChar = true })
            .Serialize(val)
            .Use(new StringImpl())
            .Static.To.Stream(stream);

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

        var val = new byte[] { 1, 2, 3, 4, 5 };

        SeraJson.Serializer
            .WithFormatter(CompactJsonFormatter.Default with { Base64Bytes = true })
            .Serialize(val)
            .Use(new BytesImpl())
            .Static.To.Stream(stream);

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

        var val = new byte[] { 1, 2, 3, 4, 5 };

        SeraJson.Serializer
            .WithFormatter(CompactJsonFormatter.Default with { Base64Bytes = false })
            .Serialize(val)
            .Use(new BytesImpl())
            .Static.To.Stream(stream);

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

        var val = Unit.New;

        SeraJson.Serializer
            .WithFormatter(CompactJsonFormatter.Default)
            .Serialize(val)
            .Use(new UnitImpl<Unit>())
            .Static.To.Stream(stream);

        stream.Position = 0;
        using var reader = new StreamReader(stream, Encoding.UTF8);
        var str = reader.ReadToEnd();
        Console.WriteLine(str);

        Assert.That(str, Is.EqualTo("null"));
    }
}
