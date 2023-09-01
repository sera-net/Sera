using Sera;
using Sera.Core;
using Sera.Core.De;
using Sera.Core.Impls;
using Unit = Microsoft.FSharp.Core.Unit;

namespace TestCore;

public class TestStaticRuntimeProvider
{
    [SetUp]
    public void Setup() { }

    class Foo : ISerializable<Foo>, IDeserializable<Foo>, IAsyncSerializable<Foo>, IAsyncDeserializable<Foo>
    {
        public static ISerialize<Foo> GetSerialize()
        {
            return UnitImpl<Foo>.Instance;
        }

        public static IDeserialize<Foo> GetDeserialize()
        {
            return UnitImpl<Foo>.Instance;
        }

        public static IAsyncSerialize<Foo> GetAsyncSerialize()
        {
            return UnitImpl<Foo>.Instance;
        }

        public static IAsyncDeserialize<Foo> GetAsyncDeserialize()
        {
            return UnitImpl<Foo>.Instance;
        }
    }

    [Test]
    public void Test0()
    {
        var inst = StaticRuntimeProvider.Instance;

        var a = inst.GetSerialize<int>();
        Assert.That(a, Is.Not.Null);

        var b = inst.GetDeserialize<int>();
        Assert.That(b, Is.Not.Null);
    }

    [Test]
    public void Test1()
    {
        var inst = StaticRuntimeProvider.Instance;

        var a = inst.GetSerialize<Foo>();
        Assert.That(a, Is.Not.Null);

        var b = inst.GetDeserialize<Foo>();
        Assert.That(b, Is.Not.Null);
    }

    [Test]
    public void Test2()
    {
        var inst = StaticRuntimeProvider.Instance;

        var a = inst.GetSerialize<Unit>();
        Assert.That(a, Is.Not.Null);

        var b = inst.GetDeserialize<Unit>();
        Assert.That(b, Is.Not.Null);
    }

    [Test]
    public void TestAsync0()
    {
        var inst = StaticRuntimeProvider.Instance;

        var a = inst.GetAsyncSerialize<int>();
        Assert.That(a, Is.Not.Null);

        var b = inst.GetAsyncDeserialize<int>();
        Assert.That(b, Is.Not.Null);
    }

    [Test]
    public void TestAsync1()
    {
        var inst = StaticRuntimeProvider.Instance;

        var a = inst.GetAsyncSerialize<Foo>();
        Assert.That(a, Is.Not.Null);

        var b = inst.GetAsyncDeserialize<Foo>();
        Assert.That(b, Is.Not.Null);
    }

    [Test]
    public void TestAsync2()
    {
        var inst = StaticRuntimeProvider.Instance;

        var a = inst.GetAsyncSerialize<Unit>();
        Assert.That(a, Is.Not.Null);

        var b = inst.GetAsyncDeserialize<Unit>();
        Assert.That(b, Is.Not.Null);
    }
}
