using System.Runtime.CompilerServices;
using Sera.Utils;

namespace Sera.Core.Impls.De;

public readonly struct PrimitiveImpl(SeraFormats? formats = null) : ISeraColion<bool, IdentityAsmer<bool>>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IdentityAsmer<bool> Asmer(Type<IdentityAsmer<bool>> a) => new();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Collect<R, C, B>(C colctor, B asmer, Type<bool> t)
        where C : ASeraColctor<R> where B : IRef<IdentityAsmer<bool>>
        => colctor.CPrimitive(asmer, new Type<IdentityAsmer<bool>>(), t, formats);
}
