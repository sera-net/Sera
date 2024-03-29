﻿using Sera.Json;
using Sera.Json.De;

namespace TestJson.TestReader;

public class TestStringJsonReader
{
    [Test]
    public void Test1()
    {
        var json = "[null, \r\ntrue, \rfalse, \n{}]";
        var reader = StringJsonReader.Create(SeraJsonOptions.Default, json.AsMemory());
        var tokens = new List<JsonToken>();
        for (; reader.CurrentHas; reader.MoveNext())
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
    public void Test2()
    {
        var json = @"{
    ""asd"": 123,
    ""123"": ""zxc""
}";
        var reader = StringJsonReader.Create(SeraJsonOptions.Default, json.AsMemory());
        var tokens = new List<JsonToken>();
        for (; reader.CurrentHas; reader.MoveNext())
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
    public void TestNumber1()
    {
        var json = "123 0 -1 -0 0.123 123.456 1e-5 1e5 1e+5 -123.456E+789 -1.2e3 0e3";
        var reader = StringJsonReader.Create(SeraJsonOptions.Default, json.AsMemory());
        var tokens = new List<JsonToken>();
        for (; reader.CurrentHas; reader.MoveNext())
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
    public void TestString1()
    {
        var json =
            "\"asd\" \"\" \"\\t\" \"123456789\" \"\\u2a5f\" \"123\\t456\" \"asd镍😂\" \"asd\\u954D\\uD83D\\uDE02\"";
        var reader = StringJsonReader.Create(SeraJsonOptions.Default, json.AsMemory());
        var tokens = new List<JsonToken>();
        for (; reader.CurrentHas; reader.MoveNext())
        {
            tokens.Add(reader.CurrentToken);
        }
        Console.WriteLine(string.Join("\n", tokens));
        Assert.Multiple(() =>
        {
            Assert.That(tokens.Count, Is.EqualTo(8));

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
        });
    }

    [Test]
    public void TestString2()
    {
        var json = "\"asd\\\\asd\"";
        var reader = StringJsonReader.Create(SeraJsonOptions.Default, json.AsMemory());
        var tokens = new List<JsonToken>();
        for (; reader.CurrentHas; reader.MoveNext())
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
        var e = Assert.Throws<JsonParseException>(() =>
        {
            var json = "asd";
            _ = StringJsonReader.Create(SeraJsonOptions.Default, json.AsMemory());
        });
        Console.WriteLine(e);
        Console.WriteLine(0);
        Console.WriteLine();

        e = Assert.Throws<JsonParseException>(() =>
        {
            var json = "-";
            _ = StringJsonReader.Create(SeraJsonOptions.Default, json.AsMemory());
        });
        Console.WriteLine(e);
        Console.WriteLine(1);
        Console.WriteLine();

        e = Assert.Throws<JsonParseException>(() =>
        {
            var json = "-a";
            _ = StringJsonReader.Create(SeraJsonOptions.Default, json.AsMemory());
        });
        Console.WriteLine(e);
        Console.WriteLine(2);
        Console.WriteLine();

        e = Assert.Throws<JsonParseException>(() =>
        {
            var json = "0e";
            _ = StringJsonReader.Create(SeraJsonOptions.Default, json.AsMemory());
        });
        Console.WriteLine(e);
        Console.WriteLine(3);
        Console.WriteLine();

        e = Assert.Throws<JsonParseException>(() =>
        {
            var json = "0e-";
            _ = StringJsonReader.Create(SeraJsonOptions.Default, json.AsMemory());
        });
        Console.WriteLine(e);
        Console.WriteLine(4);
        Console.WriteLine();

        e = Assert.Throws<JsonParseException>(() =>
        {
            var json = "\"asd";
            _ = StringJsonReader.Create(SeraJsonOptions.Default, json.AsMemory());
        });
        Console.WriteLine(e);
        Console.WriteLine(5);
        Console.WriteLine();

        e = Assert.Throws<JsonParseException>(() =>
        {
            var json = "\"asd\n";
            _ = StringJsonReader.Create(SeraJsonOptions.Default, json.AsMemory());
        });
        Console.WriteLine(e);
        Console.WriteLine(6);
        Console.WriteLine();

        e = Assert.Throws<JsonParseException>(() =>
        {
            var json = "\"\\n asd";
            _ = StringJsonReader.Create(SeraJsonOptions.Default, json.AsMemory());
        });
        Console.WriteLine(e);
        Console.WriteLine(7);
        Console.WriteLine();

        e = Assert.Throws<JsonParseException>(() =>
        {
            var json = "\"\\x";
            _ = StringJsonReader.Create(SeraJsonOptions.Default, json.AsMemory());
        });
        Console.WriteLine(e);
        Console.WriteLine(8);
        Console.WriteLine();

        e = Assert.Throws<JsonParseException>(() =>
        {
            var json = "\"\\u";
            _ = StringJsonReader.Create(SeraJsonOptions.Default, json.AsMemory());
        });
        Console.WriteLine(e);
        Console.WriteLine(9);
        Console.WriteLine();

        e = Assert.Throws<JsonParseException>(() =>
        {
            var json = "\"\\uzxc";
            _ = StringJsonReader.Create(SeraJsonOptions.Default, json.AsMemory());
        });
        Console.WriteLine(e);
        Console.WriteLine(10);
        Console.WriteLine();

        e = Assert.Throws<JsonParseException>(() =>
        {
            var json = "\"\\uvbnm";
            _ = StringJsonReader.Create(SeraJsonOptions.Default, json.AsMemory());
        });
        Console.WriteLine(e);
        Console.WriteLine(11);
        Console.WriteLine();

        e = Assert.Throws<JsonParseException>(() =>
        {
            var json = "\"\\n asd \n";
            _ = StringJsonReader.Create(SeraJsonOptions.Default, json.AsMemory());
        });
        Console.WriteLine(e);
        Console.WriteLine(12);
        Console.WriteLine();

        e = Assert.Throws<JsonParseException>(() =>
        {
            var json = "nul";
            _ = StringJsonReader.Create(SeraJsonOptions.Default, json.AsMemory());
        });
        Console.WriteLine(e);
        Console.WriteLine(13);
        Console.WriteLine();

        e = Assert.Throws<JsonParseException>(() =>
        {
            var json = "tr";
            _ = StringJsonReader.Create(SeraJsonOptions.Default, json.AsMemory());
        });
        Console.WriteLine(e);
        Console.WriteLine(14);
        Console.WriteLine();

        e = Assert.Throws<JsonParseException>(() =>
        {
            var json = "fa";
            _ = StringJsonReader.Create(SeraJsonOptions.Default, json.AsMemory());
        });
        Console.WriteLine(e);
        Console.WriteLine(15);
        Console.WriteLine();

        e = Assert.Throws<JsonParseException>(() =>
        {
            var json = "nuxx";
            _ = StringJsonReader.Create(SeraJsonOptions.Default, json.AsMemory());
        });
        Console.WriteLine(e);
        Console.WriteLine(16);
        Console.WriteLine();

        e = Assert.Throws<JsonParseException>(() =>
        {
            var json = "trxx";
            _ = StringJsonReader.Create(SeraJsonOptions.Default, json.AsMemory());
        });
        Console.WriteLine(e);
        Console.WriteLine(17);
        Console.WriteLine();

        e = Assert.Throws<JsonParseException>(() =>
        {
            var json = "faxxx";
            _ = StringJsonReader.Create(SeraJsonOptions.Default, json.AsMemory());
        });
        Console.WriteLine(e);
        Console.WriteLine(18);
        Console.WriteLine();
    }
}
