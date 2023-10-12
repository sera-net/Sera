using Sera.Core.Impls.Deps;
using Sera.Runtime.Emit.Deps;

namespace TestCore;

public class TestEmitDepContainer
{
    [SetUp]
    public void Setup() { }

    [Test]
    public void Test1()
    {
        var type = EmitDepContainer.CreateDepContainer();
        var rt = type.MakeGenericType(typeof(int));
        Assert.That(typeof(IDepsContainer<int>).IsAssignableFrom(rt), Is.True);
    }
}
