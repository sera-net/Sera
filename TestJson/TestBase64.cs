using System.Buffers;
using System.Text;
using Sera;
using Sera.Core.Impls.Ser;
using Sera.Json;

namespace TestJson;

public class TestBase64
{
    [SetUp]
    public void Setup() { }

    [Test]
    public void Test1()
    {
        using var stream = new MemoryStream();

        var seq = new ReadOnlySequence<byte>(new byte[] { 1, 2, 3, 4, 5 });

        SeraJson.Serializer
            .Serialize(seq)
            .Use(new BytesImpl())
            .Static
            .To.Stream(stream);

        stream.Position = 0;
        using var reader = new StreamReader(stream, Encoding.UTF8);
        var str = reader.ReadToEnd();
        Console.WriteLine(str);

        Assert.That(str, Is.EqualTo("\"AQIDBAU=\""));
    }

    [Test]
    public void Test2()
    {
        using var stream = new MemoryStream();

        var seq = new ReadOnlySequence<byte>(new byte[] { 1, 2, 3, 4, 5 });

        SeraJson.Serializer
            .WithOptions(SeraJsonOptions.Default with { Encoding = Encoding.Unicode })
            .Serialize(seq)
            .Use(new BytesImpl())
            .Static
            .To.Stream(stream);

        stream.Position = 0;
        using var reader = new StreamReader(stream, Encoding.Unicode);
        var str = reader.ReadToEnd();
        Console.WriteLine(str);

        Assert.That(str, Is.EqualTo("\"AQIDBAU=\""));
    }

    [Test]
    public void Test3()
    {
        var seq = new ReadOnlySequence<byte>(new byte[] { 1, 2, 3, 4, 5 });

        var str = SeraJson.Serializer
            .Serialize(seq)
            .Use(new BytesImpl())
            .Static
            .To.String();

        Console.WriteLine(str);

        Assert.That(str, Is.EqualTo("\"AQIDBAU=\""));
    }
}
