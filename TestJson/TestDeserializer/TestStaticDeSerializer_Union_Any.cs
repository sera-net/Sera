using BetterCollections.Memories;
using Sera;
using Sera.Core;
using Sera.Core.Impls.De;
using Sera.Json;

namespace TestJson.TestDeserializer;

public class TestStaticDeSerializer_Union_Any
{
    [Test]
    public void Test1()
    {
        var json = "{\"A\":123}";

        var val = SeraJson.Deserializer
            .Deserialize<Any>()
            .Use(new AnyUnionImpl(new()))
            .Static.From.String(json);

        Console.WriteLine(val);

        var target = Any.MakeUnion(new AnyUnion(null,
            (new Variant("A"), AnyVariantValue.MakeValue(Any.MakePrimitive(SeraPrimitive.MakeDouble(123).Box())))));

        Assert.That(val, Is.EqualTo(target));
    }

    [Test]
    public void Test2()
    {
        var json = "{\"t\":\"A\",\"c\":123}";

        var val = SeraJson.Deserializer
            .Deserialize<Any>()
            .Use(new AnyUnionImpl(new(), UnionStyle.Default with { Format = UnionFormat.Adjacent }))
            .Static.From.String(json);

        Console.WriteLine(val);

        var target = Any.MakeUnion(new AnyUnion(null,
            (new Variant("A"), AnyVariantValue.MakeValue(Any.MakePrimitive(SeraPrimitive.MakeDouble(123).Box())))));

        Assert.That(val, Is.EqualTo(target));
    }

    [Test]
    public void Test3()
    {
        var json = "null";

        var val = SeraJson.Deserializer
            .Deserialize<Any>()
            .Use(new AnyUnionImpl(new()))
            .Static.From.String(json);

        Console.WriteLine(val);

        var target = Any.MakeUnion(new AnyUnion(null, null));

        Assert.That(val, Is.EqualTo(target));
    }

    [Test]
    public void Test4()
    {
        var json = "{\"a\":123}";

        var val = SeraJson.Deserializer
            .Deserialize<Any>()
            .Use(new AnyUnionImpl(new(), UnionStyle.Default with { Format = UnionFormat.Untagged }))
            .Static.From.String(json);

        Console.WriteLine(val);

        var target = Any.MakeMap(new()
            { { Any.MakeString("a"), Any.MakePrimitive(SeraPrimitive.MakeDouble(123).Box()) } });

        Assert.That(val, Is.EqualTo(target));
    }

    [Test]
    public void Test5()
    {
        var json = "{\"type\":\"A\",\"c\":123}";

        var val = SeraJson.Deserializer
            .Deserialize<Any>()
            .Use(new AnyUnionImpl(new(), UnionStyle.Default with { Format = UnionFormat.Internal }))
            .Static.From.String(json);

        Console.WriteLine(val);

        var target = Any.MakeUnion(new AnyUnion(null,
            (new Variant("A"),
                AnyVariantValue.MakeStruct(new AnyStruct.Builder(null)
                    .Add("c", null, Any.MakePrimitive(SeraPrimitive.MakeDouble(123).Box())).Build()))));

        Assert.That(val, Is.EqualTo(target));
    }

    [Test]
    public void Test6()
    {
        var json = "[\"A\", 123]";

        var val = SeraJson.Deserializer
            .Deserialize<Any>()
            .Use(new AnyUnionImpl(new(), UnionStyle.Default with { Format = UnionFormat.Tuple }))
            .Static.From.String(json);

        Console.WriteLine(val);

        var target = Any.MakeUnion(new AnyUnion(null,
            (new Variant("A"), AnyVariantValue.MakeValue(Any.MakePrimitive(SeraPrimitive.MakeDouble(123).Box())))));

        Assert.That(val, Is.EqualTo(target));
    }
}
