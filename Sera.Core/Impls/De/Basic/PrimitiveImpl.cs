using System.Runtime.CompilerServices;
using Sera.Core.Impls.De.Misc;
using Sera.Utils;

namespace Sera.Core.Impls.De;

public readonly struct PrimitiveImpl(SeraFormats? formats = null) :
    ISeraColion<bool>,
    ISeraColion<float>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Collect<R, C>(ref C colctor, InType<bool>? t) where C : ISeraColctor<bool, R>
        => colctor.CPrimitive(new IdentityMapper<bool>(), new Type<bool>(), formats);


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Collect<R, C>(ref C colctor, InType<float>? t) where C : ISeraColctor<float, R>
        => colctor.CPrimitive(new IdentityMapper<float>(), new Type<float>(), formats);
}
