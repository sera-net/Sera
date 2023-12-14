namespace TestJson.TestDeserializer;

public class TestStaticDeSerializer
{
    [Test]
    public void TestPrimitiveBoolean1()
    {
        using var stream = new MemoryStream();

        var json = "true";

        // var val = true;

        // SeraJson.Serializer
        //     .Serialize(val)
        //     .Use(new PrimitiveImpl())
        //     .Static.To.Stream(stream);
        //
        // stream.Position = 0;
        // using var reader = new StreamReader(stream, Encoding.UTF8);
        // var str = reader.ReadToEnd();
        // Console.WriteLine(str);
        //
        // Assert.That(str, Is.EqualTo("true"));
    }
}
