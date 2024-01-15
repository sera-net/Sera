using System.Text;
using Sera.Json;
using Sera.Json.De;

namespace TestJson.TestReader;

public class TestAsyncStreamJsonReader
{
    [Test]
    public async ValueTask Test1()
    {
        using var memory = new MemoryStream();
        memory.Write("[null, \r\ntrue, \rfalse, \n{}]"u8);
        memory.Position = 0;
        var options = SeraJsonOptions.Default with { Encoding = Encoding.UTF8 };
        using var reader = await AsyncStreamJsonReader.Create(options, memory);
        var tokens = new List<JsonToken>();
        for (; reader.CurrentHas; await reader.MoveNext())
        {
            tokens.Add(reader.CurrentToken);
        }
        Console.WriteLine(string.Join("\n", tokens));
        Assert.Multiple(() =>
        {
            Assert.That(tokens.Count, Is.EqualTo(10));

            Assert.That(tokens[0].Kind, Is.EqualTo(JsonTokenKind.ArrayStart));
            Assert.That(tokens[1].Kind, Is.EqualTo(JsonTokenKind.Null));
            Assert.That(tokens[2].Kind, Is.EqualTo(JsonTokenKind.Comma));
            Assert.That(tokens[3].Kind, Is.EqualTo(JsonTokenKind.True));
            Assert.That(tokens[4].Kind, Is.EqualTo(JsonTokenKind.Comma));
            Assert.That(tokens[5].Kind, Is.EqualTo(JsonTokenKind.False));
            Assert.That(tokens[6].Kind, Is.EqualTo(JsonTokenKind.Comma));
            Assert.That(tokens[7].Kind, Is.EqualTo(JsonTokenKind.ObjectStart));
            Assert.That(tokens[8].Kind, Is.EqualTo(JsonTokenKind.ObjectEnd));
            Assert.That(tokens[9].Kind, Is.EqualTo(JsonTokenKind.ArrayEnd));
        });
    }

    [Test]
    public async ValueTask Test2()
    {
        using var memory = new MemoryStream();
        memory.Write(@"{
    ""asd"": 123,
    ""123"": ""zxc""
}"u8);
        memory.Position = 0;
        var options = SeraJsonOptions.Default with { Encoding = Encoding.UTF8 };
        using var reader = await AsyncStreamJsonReader.Create(options, memory);
        var tokens = new List<JsonToken>();
        for (; reader.CurrentHas; await reader.MoveNext())
        {
            tokens.Add(reader.CurrentToken);
        }
        Console.WriteLine(string.Join("\n", tokens));
        Assert.Multiple(() =>
        {
            Assert.That(tokens.Count, Is.EqualTo(9));

            Assert.That(tokens[0].Kind, Is.EqualTo(JsonTokenKind.ObjectStart));
            Assert.That(tokens[1].Kind, Is.EqualTo(JsonTokenKind.String));
            Assert.That(tokens[2].Kind, Is.EqualTo(JsonTokenKind.Colon));
            Assert.That(tokens[3].Kind, Is.EqualTo(JsonTokenKind.Number));
            Assert.That(tokens[4].Kind, Is.EqualTo(JsonTokenKind.Comma));
            Assert.That(tokens[5].Kind, Is.EqualTo(JsonTokenKind.String));
            Assert.That(tokens[6].Kind, Is.EqualTo(JsonTokenKind.Colon));
            Assert.That(tokens[7].Kind, Is.EqualTo(JsonTokenKind.String));
            Assert.That(tokens[8].Kind, Is.EqualTo(JsonTokenKind.ObjectEnd));
        });
    }

    [Test]
    public async ValueTask TestNumber1()
    {
        using var memory = new MemoryStream();
        memory.Write("123 0 -1 -0 0.123 123.456 1e-5 1e5 1e+5 -123.456E+789 -1.2e3 0e3"u8);
        memory.Position = 0;
        var options = SeraJsonOptions.Default with { Encoding = Encoding.UTF8 };
        using var reader = await AsyncStreamJsonReader.Create(options, memory);
        var tokens = new List<JsonToken>();
        for (; reader.CurrentHas; await reader.MoveNext())
        {
            tokens.Add(reader.CurrentToken);
        }
        Console.WriteLine(string.Join("\n", tokens));
        Assert.Multiple(() =>
        {
            Assert.That(tokens.Count, Is.EqualTo(12));

            foreach (var token in tokens)
            {
                Assert.That(token.Kind, Is.EqualTo(JsonTokenKind.Number));
            }

            Assert.That(tokens[0].Text.AsString(), Is.EqualTo("123"));
            Assert.That(tokens[1].Text.AsString(), Is.EqualTo("0"));
            Assert.That(tokens[2].Text.AsString(), Is.EqualTo("-1"));
            Assert.That(tokens[3].Text.AsString(), Is.EqualTo("-0"));
            Assert.That(tokens[4].Text.AsString(), Is.EqualTo("0.123"));
            Assert.That(tokens[5].Text.AsString(), Is.EqualTo("123.456"));
            Assert.That(tokens[6].Text.AsString(), Is.EqualTo("1e-5"));
            Assert.That(tokens[7].Text.AsString(), Is.EqualTo("1e5"));
            Assert.That(tokens[8].Text.AsString(), Is.EqualTo("1e+5"));
            Assert.That(tokens[9].Text.AsString(), Is.EqualTo("-123.456E+789"));
            Assert.That(tokens[10].Text.AsString(), Is.EqualTo("-1.2e3"));
            Assert.That(tokens[11].Text.AsString(), Is.EqualTo("0e3"));
        });
    }

    [Test]
    public async ValueTask TestString1()
    {
        using var memory = new MemoryStream();
        memory.Write(
            "\"asd\" \"\" \"\\t\" \"123456789\" \"\\u2a5f\" \"123\\t456\" \"asd镍😂\" \"asd\\u954D\\uD83D\\uDE02\" \"\\b\\f\\r\\n\""u8);
        memory.Position = 0;
        var options = SeraJsonOptions.Default with { Encoding = Encoding.UTF8 };
        using var reader = await AsyncStreamJsonReader.Create(options, memory);
        var tokens = new List<JsonToken>();
        for (; reader.CurrentHas; await reader.MoveNext())
        {
            tokens.Add(reader.CurrentToken);
        }
        Console.WriteLine(string.Join("\n", tokens));
        Assert.Multiple(() =>
        {
            Assert.That(tokens.Count, Is.EqualTo(9));

            foreach (var token in tokens)
            {
                Assert.That(token.Kind, Is.EqualTo(JsonTokenKind.String));
            }

            Assert.That(tokens[0].Text.AsString(), Is.EqualTo("asd"));
            Assert.That(tokens[1].Text.AsString(), Is.EqualTo(""));
            Assert.That(tokens[2].Text.AsString(), Is.EqualTo("\t"));
            Assert.That(tokens[3].Text.AsString(), Is.EqualTo("123456789"));
            Assert.That(tokens[4].Text.AsString(), Is.EqualTo("\u2a5f"));
            Assert.That(tokens[5].Text.AsString(), Is.EqualTo("123\t456"));
            Assert.That(tokens[6].Text.AsString(), Is.EqualTo("asd镍😂"));
            Assert.That(tokens[7].Text.AsString(), Is.EqualTo("asd镍😂"));
            Assert.That(tokens[8].Text.AsString(), Is.EqualTo("\b\f\r\n"));
        });
    }

    [Test]
    public async ValueTask TestString2()
    {
        using var memory = new MemoryStream();
        memory.Write(
            "\"asd\\\\asd\""u8);
        memory.Position = 0;
        var options = SeraJsonOptions.Default with { Encoding = Encoding.UTF8 };
        using var reader = await AsyncStreamJsonReader.Create(options, memory);
        var tokens = new List<JsonToken>();
        for (; reader.CurrentHas; await reader.MoveNext())
        {
            tokens.Add(reader.CurrentToken);
        }
        Console.WriteLine(string.Join("\n", tokens));
        Assert.Multiple(() =>
        {
            Assert.That(tokens.Count, Is.EqualTo(1));

            foreach (var token in tokens)
            {
                Assert.That(token.Kind, Is.EqualTo(JsonTokenKind.String));
            }

            Assert.That(tokens[0].Text.AsString(), Is.EqualTo("asd\\asd"));
        });
    }

    [Test]
    public void TestExceptions()
    {
        var e = Assert.ThrowsAsync<JsonParseException>(async () =>
        {
            using var memory = new MemoryStream();
            memory.Write(
                "asd"u8);
            memory.Position = 0;
            var options = SeraJsonOptions.Default with { Encoding = Encoding.UTF8 };
            await AsyncStreamJsonReader.Create(options, memory);
        });
        Console.WriteLine(e);
        Console.WriteLine(0);
        Console.WriteLine();

        e = Assert.ThrowsAsync<JsonParseException>(async () =>
        {
            using var memory = new MemoryStream();
            memory.Write(
                "-"u8);
            memory.Position = 0;
            var options = SeraJsonOptions.Default with { Encoding = Encoding.UTF8 };
            await AsyncStreamJsonReader.Create(options, memory);
        });
        Console.WriteLine(e);
        Console.WriteLine(1);
        Console.WriteLine();

        e = Assert.ThrowsAsync<JsonParseException>(async () =>
        {
            using var memory = new MemoryStream();
            memory.Write(
                "-a"u8);
            memory.Position = 0;
            var options = SeraJsonOptions.Default with { Encoding = Encoding.UTF8 };
            await AsyncStreamJsonReader.Create(options, memory);
        });
        Console.WriteLine(e);
        Console.WriteLine(2);
        Console.WriteLine();

        e = Assert.ThrowsAsync<JsonParseException>(async () =>
        {
            using var memory = new MemoryStream();
            memory.Write(
                "0e"u8);
            memory.Position = 0;
            var options = SeraJsonOptions.Default with { Encoding = Encoding.UTF8 };
            await AsyncStreamJsonReader.Create(options, memory);
        });
        Console.WriteLine(e);
        Console.WriteLine(3);
        Console.WriteLine();

        e = Assert.ThrowsAsync<JsonParseException>(async () =>
        {
            using var memory = new MemoryStream();
            memory.Write(
                "oe-"u8);
            memory.Position = 0;
            var options = SeraJsonOptions.Default with { Encoding = Encoding.UTF8 };
            await AsyncStreamJsonReader.Create(options, memory);
        });
        Console.WriteLine(e);
        Console.WriteLine(4);
        Console.WriteLine();

        e = Assert.ThrowsAsync<JsonParseException>(async () =>
        {
            using var memory = new MemoryStream();
            memory.Write(
                "\"asd"u8);
            memory.Position = 0;
            var options = SeraJsonOptions.Default with { Encoding = Encoding.UTF8 };
            await AsyncStreamJsonReader.Create(options, memory);
        });
        Console.WriteLine(e);
        Console.WriteLine(5);
        Console.WriteLine();

        e = Assert.ThrowsAsync<JsonParseException>(async () =>
        {
            using var memory = new MemoryStream();
            memory.Write(
                "\"asd\n"u8);
            memory.Position = 0;
            var options = SeraJsonOptions.Default with { Encoding = Encoding.UTF8 };
            await AsyncStreamJsonReader.Create(options, memory);
        });
        Console.WriteLine(e);
        Console.WriteLine(6);
        Console.WriteLine();

        e = Assert.ThrowsAsync<JsonParseException>(async () =>
        {
            using var memory = new MemoryStream();
            memory.Write(
                "\"\\n asd"u8);
            memory.Position = 0;
            var options = SeraJsonOptions.Default with { Encoding = Encoding.UTF8 };
            await AsyncStreamJsonReader.Create(options, memory);
        });
        Console.WriteLine(e);
        Console.WriteLine(7);
        Console.WriteLine();

        e = Assert.ThrowsAsync<JsonParseException>(async () =>
        {
            using var memory = new MemoryStream();
            memory.Write(
                "\"\\x"u8);
            memory.Position = 0;
            var options = SeraJsonOptions.Default with { Encoding = Encoding.UTF8 };
            await AsyncStreamJsonReader.Create(options, memory);
        });
        Console.WriteLine(e);
        Console.WriteLine(8);
        Console.WriteLine();

        e = Assert.ThrowsAsync<JsonParseException>(async () =>
        {
            using var memory = new MemoryStream();
            memory.Write(
                "\"\\u"u8);
            memory.Position = 0;
            var options = SeraJsonOptions.Default with { Encoding = Encoding.UTF8 };
            await AsyncStreamJsonReader.Create(options, memory);
        });
        Console.WriteLine(e);
        Console.WriteLine(9);
        Console.WriteLine();

        e = Assert.ThrowsAsync<JsonParseException>(async () =>
        {
            using var memory = new MemoryStream();
            memory.Write(
                "\"\\uzxc"u8);
            memory.Position = 0;
            var options = SeraJsonOptions.Default with { Encoding = Encoding.UTF8 };
            await AsyncStreamJsonReader.Create(options, memory);
        });
        Console.WriteLine(e);
        Console.WriteLine(10);
        Console.WriteLine();

        e = Assert.ThrowsAsync<JsonParseException>(async () =>
        {
            using var memory = new MemoryStream();
            memory.Write(
                "\"\\uvbnm"u8);
            memory.Position = 0;
            var options = SeraJsonOptions.Default with { Encoding = Encoding.UTF8 };
            await AsyncStreamJsonReader.Create(options, memory);
        });
        Console.WriteLine(e);
        Console.WriteLine(11);
        Console.WriteLine();

        e = Assert.ThrowsAsync<JsonParseException>(async () =>
        {
            using var memory = new MemoryStream();
            memory.Write(
                "\"\\n asd \n"u8);
            memory.Position = 0;
            var options = SeraJsonOptions.Default with { Encoding = Encoding.UTF8 };
            await AsyncStreamJsonReader.Create(options, memory);
        });
        Console.WriteLine(e);
        Console.WriteLine(12);
        Console.WriteLine();

        e = Assert.ThrowsAsync<JsonParseException>(async () =>
        {
            using var memory = new MemoryStream();
            memory.Write(
                "nul"u8);
            memory.Position = 0;
            var options = SeraJsonOptions.Default with { Encoding = Encoding.UTF8 };
            await AsyncStreamJsonReader.Create(options, memory);
        });
        Console.WriteLine(e);
        Console.WriteLine(13);
        Console.WriteLine();

        e = Assert.ThrowsAsync<JsonParseException>(async () =>
        {
            using var memory = new MemoryStream();
            memory.Write(
                "tr"u8);
            memory.Position = 0;
            var options = SeraJsonOptions.Default with { Encoding = Encoding.UTF8 };
            await AsyncStreamJsonReader.Create(options, memory);
        });
        Console.WriteLine(e);
        Console.WriteLine(14);
        Console.WriteLine();

        e = Assert.ThrowsAsync<JsonParseException>(async () =>
        {
            using var memory = new MemoryStream();
            memory.Write(
                "fa"u8);
            memory.Position = 0;
            var options = SeraJsonOptions.Default with { Encoding = Encoding.UTF8 };
            await AsyncStreamJsonReader.Create(options, memory);
        });
        Console.WriteLine(e);
        Console.WriteLine(15);
        Console.WriteLine();

        e = Assert.ThrowsAsync<JsonParseException>(async () =>
        {
            using var memory = new MemoryStream();
            memory.Write(
                "nuxx"u8);
            memory.Position = 0;
            var options = SeraJsonOptions.Default with { Encoding = Encoding.UTF8 };
            await AsyncStreamJsonReader.Create(options, memory);
        });
        Console.WriteLine(e);
        Console.WriteLine(16);
        Console.WriteLine();

        e = Assert.ThrowsAsync<JsonParseException>(async () =>
        {
            using var memory = new MemoryStream();
            memory.Write(
                "trxx"u8);
            memory.Position = 0;
            var options = SeraJsonOptions.Default with { Encoding = Encoding.UTF8 };
            await AsyncStreamJsonReader.Create(options, memory);
        });
        Console.WriteLine(e);
        Console.WriteLine(17);
        Console.WriteLine();

        e = Assert.ThrowsAsync<JsonParseException>(async () =>
        {
            using var memory = new MemoryStream();
            memory.Write(
                "faxxx"u8);
            memory.Position = 0;
            var options = SeraJsonOptions.Default with { Encoding = Encoding.UTF8 };
            await AsyncStreamJsonReader.Create(options, memory);
        });
        Console.WriteLine(e);
        Console.WriteLine(18);
        Console.WriteLine();
    }

    [Test]
    public async ValueTask TestStringLarge1()
    {
        using var memory = new MemoryStream();
        memory.Write(
            "\""u8);
        var arr = new byte[1024];
        arr.AsSpan().Fill(" "u8[0]);
        memory.Write(arr);
        memory.Write(
            "\""u8);
        memory.Position = 0;
        var str = Encoding.UTF8.GetString(memory.ToArray());
        memory.Position = 0;
        var options = SeraJsonOptions.Default with { Encoding = Encoding.UTF8 };
        using var reader = await AsyncStreamJsonReader.Create(options, memory);
        var tokens = new List<JsonToken>();
        for (; reader.CurrentHas; await reader.MoveNext())
        {
            tokens.Add(reader.CurrentToken);
        }
        Console.WriteLine(string.Join("\n", tokens));
        Assert.Multiple(() =>
        {
            Assert.That(tokens.Count, Is.EqualTo(1));

            foreach (var token in tokens)
            {
                Assert.That(token.Kind, Is.EqualTo(JsonTokenKind.String));
            }

            Assert.That(tokens[0].Text.AsString(), Is.EqualTo(str[1..^1]));
        });
    }

    [Test]
    public async ValueTask TestStringLarge2()
    {
        using var memory = new MemoryStream();
        memory.Write(
            "\"\\t"u8);
        var arr = new byte[1024];
        arr.AsSpan().Fill(" "u8[0]);
        memory.Write(arr);
        memory.Write(
            "\""u8);
        memory.Position = 0;
        var str = Encoding.UTF8.GetString(memory.ToArray()).Replace(@"\t", "\t");
        memory.Position = 0;
        var options = SeraJsonOptions.Default with { Encoding = Encoding.UTF8 };
        using var reader = await AsyncStreamJsonReader.Create(options, memory);
        var tokens = new List<JsonToken>();
        for (; reader.CurrentHas; await reader.MoveNext())
        {
            tokens.Add(reader.CurrentToken);
        }
        Console.WriteLine(string.Join("\n", tokens));
        Assert.Multiple(() =>
        {
            Assert.That(tokens.Count, Is.EqualTo(1));

            foreach (var token in tokens)
            {
                Assert.That(token.Kind, Is.EqualTo(JsonTokenKind.String));
            }

            Assert.That(tokens[0].Text.AsString(), Is.EqualTo(str[1..^1]));
        });
    }

    [Test]
    public async ValueTask TestSpaceLarge1()
    {
        using var memory = new MemoryStream();
        var arr = new byte[1024];
        arr.AsSpan().Fill(" "u8[0]);
        memory.Write(arr);
        memory.Position = 0;
        var options = SeraJsonOptions.Default with { Encoding = Encoding.UTF8 };
        using var reader = await AsyncStreamJsonReader.Create(options, memory);
        var tokens = new List<JsonToken>();
        for (; reader.CurrentHas; await reader.MoveNext())
        {
            tokens.Add(reader.CurrentToken);
        }
        Console.WriteLine(string.Join("\n", tokens));
        Assert.Multiple(() => { Assert.That(tokens.Count, Is.EqualTo(0)); });
    }

    [Test]
    public async ValueTask TestSpaceLarge2()
    {
        using var memory = new MemoryStream();
        memory.Write("true"u8);
        var arr = new byte[1024];
        arr.AsSpan().Fill(" "u8[0]);
        memory.Write(arr);
        memory.Write("true"u8);
        memory.Position = 0;
        var options = SeraJsonOptions.Default with { Encoding = Encoding.UTF8 };
        using var reader = await AsyncStreamJsonReader.Create(options, memory);
        var tokens = new List<JsonToken>();
        for (; reader.CurrentHas; await reader.MoveNext())
        {
            tokens.Add(reader.CurrentToken);
        }
        Console.WriteLine(string.Join("\n", tokens));
        Assert.Multiple(() =>
        {
            Assert.That(tokens.Count, Is.EqualTo(2));

            Assert.That(tokens[0].Kind, Is.EqualTo(JsonTokenKind.True));
            Assert.That(tokens[1].Kind, Is.EqualTo(JsonTokenKind.True));
        });
    }

    [Test]
    public async ValueTask TestNumberLarge1()
    {
        using var memory = new MemoryStream();
        var arr = new byte[1024];
        arr.AsSpan().Fill("1"u8[0]);
        memory.Write(arr);
        memory.Position = 0;
        var options = SeraJsonOptions.Default with { Encoding = Encoding.UTF8 };
        using var reader = await AsyncStreamJsonReader.Create(options, memory);
        var tokens = new List<JsonToken>();
        for (; reader.CurrentHas; await reader.MoveNext())
        {
            tokens.Add(reader.CurrentToken);
        }
        Console.WriteLine(string.Join("\n", tokens));
        Assert.Multiple(() =>
        {
            Assert.That(tokens.Count, Is.EqualTo(1));

            Assert.That(tokens[0].Kind, Is.EqualTo(JsonTokenKind.Number));
        });
    }

    [Test]
    public async ValueTask TestNumberLarge2()
    {
        using var memory = new MemoryStream();
        memory.Write("123."u8);
        var arr = new byte[1024];
        arr.AsSpan().Fill("1"u8[0]);
        memory.Write(arr);
        memory.Position = 0;
        var options = SeraJsonOptions.Default with { Encoding = Encoding.UTF8 };
        using var reader = await AsyncStreamJsonReader.Create(options, memory);
        var tokens = new List<JsonToken>();
        for (; reader.CurrentHas; await reader.MoveNext())
        {
            tokens.Add(reader.CurrentToken);
        }
        Console.WriteLine(string.Join("\n", tokens));
        Assert.Multiple(() =>
        {
            Assert.That(tokens.Count, Is.EqualTo(1));

            Assert.That(tokens[0].Kind, Is.EqualTo(JsonTokenKind.Number));
        });
    }

    [Test]
    public async ValueTask TestNumberLarge3()
    {
        using var memory = new MemoryStream();
        memory.Write("123.456e"u8);
        var arr = new byte[1024];
        arr.AsSpan().Fill("1"u8[0]);
        memory.Write(arr);
        memory.Position = 0;
        var options = SeraJsonOptions.Default with { Encoding = Encoding.UTF8 };
        using var reader = await AsyncStreamJsonReader.Create(options, memory);
        var tokens = new List<JsonToken>();
        for (; reader.CurrentHas; await reader.MoveNext())
        {
            tokens.Add(reader.CurrentToken);
        }
        Console.WriteLine(string.Join("\n", tokens));
        Assert.Multiple(() =>
        {
            Assert.That(tokens.Count, Is.EqualTo(1));

            Assert.That(tokens[0].Kind, Is.EqualTo(JsonTokenKind.Number));
        });
    }
}
