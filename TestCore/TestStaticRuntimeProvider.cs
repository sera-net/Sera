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

    class Foo : ISerializable<Foo>, IDeserializable<Foo>
    {
        public static ISerialize<Foo> GetSerialize()
        {
            return UnitImpl<Foo>.Instance;
        }
        
        public static IDeserialize<Foo> GetDeserialize()
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
}

