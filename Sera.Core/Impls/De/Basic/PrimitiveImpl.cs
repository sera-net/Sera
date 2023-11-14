using System.Runtime.CompilerServices;
using Sera.Utils;

namespace Sera.Core.Impls.De;

public readonly struct PrimitiveImpl(SeraFormats? formats = null) :
    ISeraColion<IdentityAsmer<bool>>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    R ISeraColion<IdentityAsmer<bool>>.Collect<R, C, B>(C colctor, B asmer)
        => colctor.CPrimitive(asmer, new Type<IdentityAsmer<bool>>(), new Type<bool>(), formats);
}
