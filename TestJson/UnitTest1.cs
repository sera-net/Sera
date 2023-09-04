using System.Text;
using Sera.Core.Impls;
using Sera.Json;

namespace TestJson;

public class Tests
{
    [SetUp]
    public void Setup() { }

    [Test]
    public void Test1()
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
}
