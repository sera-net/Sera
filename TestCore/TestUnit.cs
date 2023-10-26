using System.Runtime.CompilerServices;
using Sera.Core;

namespace TestCore;

public class TestUnit
{
    [Test]
    public void Test1()
    {
        var a = Unit.New;
        
        var size = Unsafe.SizeOf<Unit>();
        
        Console.WriteLine(a);
        Console.WriteLine(size);
    }
}
