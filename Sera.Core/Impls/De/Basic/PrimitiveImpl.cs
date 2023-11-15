using System.Runtime.CompilerServices;
using Sera.Utils;

namespace Sera.Core.Impls.De;

public static class PrimitiveImpls
{
    public readonly struct Bool(SeraFormats? formats = null) : ISeraColion<IdentityAsmer<bool>>
    {
        [AssocType]
        public abstract class A(IdentityAsmer<bool> type);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public R Collect<R, C, B>(C colctor, B asmer) where C : ASeraColctor<R> where B : IRef<IdentityAsmer<bool>>
            => colctor.CPrimitive(asmer, new Type<IdentityAsmer<bool>>(), new Type<bool>(), formats);
    }
}
