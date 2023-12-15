using Sera;
using Sera.Core.Impls.De;
using Sera.Json;

namespace TestJson.TestDeserializer;

public class TestStaticDeSerializer
{
    [Test]
    public void TestPrimitiveBoolean1()
    {
        using var stream = new MemoryStream();

        var json = "true";

        var val = SeraJson.Deserializer
            .Deserialize<bool>()
            .Use(new PrimitiveImpl())
            .Static.From.String(json);
        
        Console.WriteLine(val);
    }
}
