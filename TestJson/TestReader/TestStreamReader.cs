using System.Runtime.InteropServices;
using System.Text;
using Sera.Json;
using Sera.Json.De;

namespace TestJson.TestReader;

public class TestStreamReader
{
    [Test]
    public void Test1()
    {
        using var memory = new MemoryStream();
        memory.Write("\"asd\\n\""u8);
        memory.Position = 0;
        var options = SeraJsonOptions.Default with { Encoding = Encoding.UTF8 };
        var reader = StreamJsonReader.Create(options, memory);
        var tokens = new List<JsonToken>();
        for (; reader.CurrentHas; reader.MoveNext())
        {
            tokens.Add(reader.CurrentToken);
        }
        Console.WriteLine(string.Join("\n", tokens));
    }
}
