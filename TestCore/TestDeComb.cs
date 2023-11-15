using Sera.Core.Impls.De;

namespace TestCore;

public class TestDeComb
{
    [Test]
    public void Test()
    {
        ICollectionSeqImpl<List<bool>, bool, ListCtor<bool>, PrimitiveImpls.Bool, IdentityAsmer<bool>>
            a = new(new());
        ICollectionSeqImpl<ICollection<bool>, bool, ListCtor<bool>, PrimitiveImpls.Bool, IdentityAsmer<bool>>
            b = new(new());

        ICollectionSeqAsmable<List<bool>, bool, ListCtor<bool>, IdentityAsmer<bool>.Asmable, IdentityAsmer<bool>>
            aa = new(new(), new());
        ICollectionSeqAsmable<ICollection<bool>, bool, ListCtor<bool>, IdentityAsmer<bool>.Asmable, IdentityAsmer<bool>>
            ab = new(new(), new());
    }
}
