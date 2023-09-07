using Sera.Core.Impls;
using Sera.Json;
using Sera.Json.Runtime;

namespace TestJson;

public class TestRuntimeX32
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
}
