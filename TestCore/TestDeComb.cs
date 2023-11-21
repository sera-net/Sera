using Sera.Core.Impls.De;
using Sera.Core.Impls.De.Misc;

namespace TestCore;

public class TestDeComb
{
    [Test]
    public void Test()
    {
        SeqICollectionImpl<List<bool>, bool, PrimitiveImpl, SeqCapCtor<bool>>
            a = new(new(), new());
        SeqICollectionImpl<ICollection<bool>, bool, PrimitiveImpl, SeqCapCtor<bool>>
            b = new(new(), new());
    }
}
