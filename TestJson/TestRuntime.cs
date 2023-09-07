using Sera.Json;
using Sera.Json.Runtime;

namespace TestJson;

public class TestRuntime
{
    [SetUp]
    public void Setup() { }

    public class EmptyStruct1 { }

    [Test]
    public void TestEmptyStruct1()
    {
        var obj = new EmptyStruct1();

        var str = SeraJson.Serializer
            .ToString()
            .Serialize(obj);

        Console.WriteLine(str);
        Assert.That(str, Is.EqualTo("{}"));
    }

    public class Struct1
    {
        public int Member1 { get; set; } = 123456;
        public int Member2 = 654321;
    }
    
    [Test]
    public void TestStruct1()
    {
        var obj = new Struct1();

        var str = SeraJson.Serializer
            .ToString()
            .Serialize(obj);

        Console.WriteLine(str);
        Assert.That(str, Is.EqualTo("{\"Member1\":123456,\"Member2\":654321}"));
    }
}
