using Sera.Json;
using Sera.Json.De;
using Sera.Utils;

namespace TestJson.TestReader;

public class TestSeek
{
    [Test]
    public void Test1()
    {
        var reader = StringJsonReader.Create(SeraJsonOptions.Default, CompoundString.MakeString("[123,456]"));
        Assert.That(reader.CurrentToken.Kind, Is.EqualTo(JsonTokenKind.ArrayStart));
        var save = reader.Save();
        reader.MoveNext();
        Assert.That(reader.CurrentToken.Kind, Is.EqualTo(JsonTokenKind.Number));
        reader.Load(save);
        Assert.That(reader.CurrentToken.Kind, Is.EqualTo(JsonTokenKind.ArrayStart));
        reader.MoveNext();
        Assert.That(reader.CurrentToken.Kind, Is.EqualTo(JsonTokenKind.Number));
        reader.UnSave(save);
    }

    [Test]
    public void Test2()
    {
        var reader = AstJsonReader.Create(SeraJsonOptions.Default,
            StringJsonReader.Create(SeraJsonOptions.Default, CompoundString.MakeString("[123,456]")).ReadValue());
        Assert.That(reader.CurrentToken.Kind, Is.EqualTo(JsonTokenKind.ArrayStart));
        var save = reader.Save();
        reader.MoveNext();
        Assert.That(reader.CurrentToken.Kind, Is.EqualTo(JsonTokenKind.Number));
        reader.Load(save);
        Assert.That(reader.CurrentToken.Kind, Is.EqualTo(JsonTokenKind.ArrayStart));
        reader.MoveNext();
        Assert.That(reader.CurrentToken.Kind, Is.EqualTo(JsonTokenKind.Number));
        reader.UnSave(save);
    }
}
