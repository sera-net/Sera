using System.Text;
using Sera;
using Sera.Core.Impls.De;
using Sera.Json;

namespace TestJson.TestDeserializer;

public class TestStaticDeSerializerAsync
{
    [Test]
    public async ValueTask TestPrimitiveBoolean1()
    {
        using var memory = new MemoryStream();
        memory.Write("true"u8);
        memory.Position = 0;

        var val = await SeraJson.Deserializer
            .WithOptions(SeraJsonOptions.Default with { Encoding = Encoding.UTF8 })
            .Deserialize<bool>()
            .Use(new PrimitiveImpl())
            .Static.From.StreamAsync(memory);

        Console.WriteLine(val);

        Assert.That(val, Is.EqualTo(true));
    }
    
    [Test]
    public async ValueTask TestPrimitiveBoolean2()
    {
        using var memory = new MemoryStream();
        memory.Write("false"u8);
        memory.Position = 0;

        var val = await SeraJson.Deserializer
            .WithOptions(SeraJsonOptions.Default with { Encoding = Encoding.UTF8 })
            .Deserialize<bool>()
            .Use(new PrimitiveImpl())
            .Static.From.StreamAsync(memory);

        Console.WriteLine(val);

        Assert.That(val, Is.EqualTo(false));
    }
}
