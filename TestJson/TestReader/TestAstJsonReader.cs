using Sera.Json;
using Sera.Json.De;
using Sera.Json.Utils;
using Sera.Utils;

namespace TestJson.TestReader;

public class TestAstJsonReader
{
    [Test]
    public void Test1()
    {
        var ast = new StringJsonReader(SeraJsonOptions.Default, CompoundString.MakeString("123")).ReadValue();
        Console.WriteLine(ast);
        var reader = new AstJsonReader(SeraJsonOptions.Default, ast);
        var tokens = new List<JsonToken>();
        for (; reader.Has; reader.MoveNext())
        {
            tokens.Add(reader.CurrentToken);
        }
        Console.WriteLine();
        Console.WriteLine(string.Join("\n", tokens));
        Assert.Multiple(() =>
        {
            Assert.That(tokens.Count, Is.EqualTo(1));

            Assert.That(tokens[0].Kind, Is.EqualTo(JsonTokenKind.Number));
        });
    }

    [Test]
    public void TestArray1()
    {
        var ast = new StringJsonReader(SeraJsonOptions.Default, CompoundString.MakeString("[1,[2],3]")).ReadValue();
        Console.WriteLine(ast);
        var reader = new AstJsonReader(SeraJsonOptions.Default, ast);
        var tokens = new List<JsonToken>();
        for (; reader.Has; reader.MoveNext())
        {
            tokens.Add(reader.CurrentToken);
        }
        Console.WriteLine();
        Console.WriteLine(string.Join("\n", tokens));
        Assert.Multiple(() =>
        {
            Assert.That(tokens.Count, Is.EqualTo(9));

            Assert.That(tokens[0].Kind, Is.EqualTo(JsonTokenKind.ArrayStart));
            Assert.That(tokens[1].Kind, Is.EqualTo(JsonTokenKind.Number));
            Assert.That(tokens[2].Kind, Is.EqualTo(JsonTokenKind.Comma));
            Assert.That(tokens[3].Kind, Is.EqualTo(JsonTokenKind.ArrayStart));
            Assert.That(tokens[4].Kind, Is.EqualTo(JsonTokenKind.Number));
            Assert.That(tokens[5].Kind, Is.EqualTo(JsonTokenKind.ArrayEnd));
            Assert.That(tokens[6].Kind, Is.EqualTo(JsonTokenKind.Comma));
            Assert.That(tokens[7].Kind, Is.EqualTo(JsonTokenKind.Number));
            Assert.That(tokens[8].Kind, Is.EqualTo(JsonTokenKind.ArrayEnd));
        });
    }

    [Test]
    public void TestObject1()
    {
        var ast = new StringJsonReader(SeraJsonOptions.Default, CompoundString.MakeString("{\"a\":1,\"b\":{\"c\":2}}"))
            .ReadValue();
        Console.WriteLine(ast);
        var reader = new AstJsonReader(SeraJsonOptions.Default, ast);
        var tokens = new List<JsonToken>();
        for (; reader.Has; reader.MoveNext())
        {
            tokens.Add(reader.CurrentToken);
        }
        Console.WriteLine();
        Console.WriteLine(string.Join("\n", tokens));
        Assert.Multiple(() =>
        {
            Assert.That(tokens.Count, Is.EqualTo(13));

            Assert.That(tokens[0].Kind, Is.EqualTo(JsonTokenKind.ObjectStart));
            Assert.That(tokens[1].Kind, Is.EqualTo(JsonTokenKind.String));
            Assert.That(tokens[2].Kind, Is.EqualTo(JsonTokenKind.Colon));
            Assert.That(tokens[3].Kind, Is.EqualTo(JsonTokenKind.Number));
            Assert.That(tokens[4].Kind, Is.EqualTo(JsonTokenKind.Comma));
            Assert.That(tokens[5].Kind, Is.EqualTo(JsonTokenKind.String));
            Assert.That(tokens[6].Kind, Is.EqualTo(JsonTokenKind.Colon));
            Assert.That(tokens[7].Kind, Is.EqualTo(JsonTokenKind.ObjectStart));
            Assert.That(tokens[8].Kind, Is.EqualTo(JsonTokenKind.String));
            Assert.That(tokens[9].Kind, Is.EqualTo(JsonTokenKind.Colon));
            Assert.That(tokens[10].Kind, Is.EqualTo(JsonTokenKind.Number));
            Assert.That(tokens[11].Kind, Is.EqualTo(JsonTokenKind.ObjectEnd));
            Assert.That(tokens[12].Kind, Is.EqualTo(JsonTokenKind.ObjectEnd));
        });
    }
}
