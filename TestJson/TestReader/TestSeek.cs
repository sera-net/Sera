using System.Text;
using Sera.Json;
using Sera.Json.De;
using Sera.Utils;

namespace TestJson.TestReader;

public class TestSeek
{
    [Test]
    public void Test1()
    {
        var reader = StringJsonReader.Create(SeraJsonOptions.Default, CompoundString.MakeString("[123,true,null]"));
        Assert.That(reader.CurrentToken.Kind, Is.EqualTo(JsonTokenKind.ArrayStart));
        var save = reader.Save();
        reader.MoveNext();
        Assert.That(reader.CurrentToken.Kind, Is.EqualTo(JsonTokenKind.Number));
        reader.Load(save);
        Assert.That(reader.CurrentToken.Kind, Is.EqualTo(JsonTokenKind.ArrayStart));
        reader.MoveNext();
        Assert.That(reader.CurrentToken.Kind, Is.EqualTo(JsonTokenKind.Number));
        reader.UnSave(save);
        save = reader.Save();
        Assert.That(reader.CurrentToken.Kind, Is.EqualTo(JsonTokenKind.Number));
        reader.MoveNext();
        Assert.That(reader.CurrentToken.Kind, Is.EqualTo(JsonTokenKind.Comma));
        reader.MoveNext();
        Assert.That(reader.CurrentToken.Kind, Is.EqualTo(JsonTokenKind.True));
        reader.MoveNext();
        Assert.That(reader.CurrentToken.Kind, Is.EqualTo(JsonTokenKind.Comma));
        reader.Load(save);
        Assert.That(reader.CurrentToken.Kind, Is.EqualTo(JsonTokenKind.Number));
        reader.UnSave(save);
        Assert.That(reader.CurrentToken.Kind, Is.EqualTo(JsonTokenKind.Number));
        reader.MoveNext();
        Assert.That(reader.CurrentToken.Kind, Is.EqualTo(JsonTokenKind.Comma));
        reader.MoveNext();
        Assert.That(reader.CurrentToken.Kind, Is.EqualTo(JsonTokenKind.True));
        reader.MoveNext();
        Assert.That(reader.CurrentToken.Kind, Is.EqualTo(JsonTokenKind.Comma));
        reader.MoveNext();
        Assert.That(reader.CurrentToken.Kind, Is.EqualTo(JsonTokenKind.Null));
        reader.MoveNext();
        Assert.That(reader.CurrentToken.Kind, Is.EqualTo(JsonTokenKind.ArrayEnd));
    }

    [Test]
    public void Test2()
    {
        var reader = AstJsonReader.Create(SeraJsonOptions.Default,
            StringJsonReader.Create(SeraJsonOptions.Default, CompoundString.MakeString("[123,true,null]")).ReadValue());
        Assert.That(reader.CurrentToken.Kind, Is.EqualTo(JsonTokenKind.ArrayStart));
        var save = reader.Save();
        reader.MoveNext();
        Assert.That(reader.CurrentToken.Kind, Is.EqualTo(JsonTokenKind.Number));
        reader.Load(save);
        Assert.That(reader.CurrentToken.Kind, Is.EqualTo(JsonTokenKind.ArrayStart));
        reader.MoveNext();
        Assert.That(reader.CurrentToken.Kind, Is.EqualTo(JsonTokenKind.Number));
        reader.UnSave(save);
        save = reader.Save();
        Assert.That(reader.CurrentToken.Kind, Is.EqualTo(JsonTokenKind.Number));
        reader.MoveNext();
        Assert.That(reader.CurrentToken.Kind, Is.EqualTo(JsonTokenKind.Comma));
        reader.MoveNext();
        Assert.That(reader.CurrentToken.Kind, Is.EqualTo(JsonTokenKind.True));
        reader.MoveNext();
        Assert.That(reader.CurrentToken.Kind, Is.EqualTo(JsonTokenKind.Comma));
        reader.Load(save);
        Assert.That(reader.CurrentToken.Kind, Is.EqualTo(JsonTokenKind.Number));
        reader.UnSave(save);
        Assert.That(reader.CurrentToken.Kind, Is.EqualTo(JsonTokenKind.Number));
        reader.MoveNext();
        Assert.That(reader.CurrentToken.Kind, Is.EqualTo(JsonTokenKind.Comma));
        reader.MoveNext();
        Assert.That(reader.CurrentToken.Kind, Is.EqualTo(JsonTokenKind.True));
        reader.MoveNext();
        Assert.That(reader.CurrentToken.Kind, Is.EqualTo(JsonTokenKind.Comma));
        reader.MoveNext();
        Assert.That(reader.CurrentToken.Kind, Is.EqualTo(JsonTokenKind.Null));
        reader.MoveNext();
        Assert.That(reader.CurrentToken.Kind, Is.EqualTo(JsonTokenKind.ArrayEnd));
    }

    [Test]
    public void Test3()
    {
        using var memory = new MemoryStream();
        memory.Write("[123,true,null]"u8);
        memory.Position = 0;
        var options = SeraJsonOptions.Default with { Encoding = Encoding.UTF8 };
        using var reader = StreamJsonReader.Create(options, memory);
        Assert.That(reader.CurrentToken.Kind, Is.EqualTo(JsonTokenKind.ArrayStart));
        var save = reader.Save();
        reader.MoveNext();
        Assert.That(reader.CurrentToken.Kind, Is.EqualTo(JsonTokenKind.Number));
        reader.Load(save);
        Assert.That(reader.CurrentToken.Kind, Is.EqualTo(JsonTokenKind.ArrayStart));
        reader.MoveNext();
        Assert.That(reader.CurrentToken.Kind, Is.EqualTo(JsonTokenKind.Number));
        reader.UnSave(save);
        save = reader.Save();
        Assert.That(reader.CurrentToken.Kind, Is.EqualTo(JsonTokenKind.Number));
        reader.MoveNext();
        Assert.That(reader.CurrentToken.Kind, Is.EqualTo(JsonTokenKind.Comma));
        reader.MoveNext();
        Assert.That(reader.CurrentToken.Kind, Is.EqualTo(JsonTokenKind.True));
        reader.MoveNext();
        Assert.That(reader.CurrentToken.Kind, Is.EqualTo(JsonTokenKind.Comma));
        reader.Load(save);
        Assert.That(reader.CurrentToken.Kind, Is.EqualTo(JsonTokenKind.Number));
        reader.UnSave(save);
        Assert.That(reader.CurrentToken.Kind, Is.EqualTo(JsonTokenKind.Number));
        reader.MoveNext();
        Assert.That(reader.CurrentToken.Kind, Is.EqualTo(JsonTokenKind.Comma));
        reader.MoveNext();
        Assert.That(reader.CurrentToken.Kind, Is.EqualTo(JsonTokenKind.True));
        reader.MoveNext();
        Assert.That(reader.CurrentToken.Kind, Is.EqualTo(JsonTokenKind.Comma));
        reader.MoveNext();
        Assert.That(reader.CurrentToken.Kind, Is.EqualTo(JsonTokenKind.Null));
        reader.MoveNext();
        Assert.That(reader.CurrentToken.Kind, Is.EqualTo(JsonTokenKind.ArrayEnd));
    }
}
