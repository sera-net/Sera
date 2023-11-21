using System.Runtime.CompilerServices;
using Sera.Utils;

namespace Sera.Core.Impls.De.Misc;

public readonly struct BoxedImpl<T, D>(Box<D> d) : ISeraColion<T>
    where D : ISeraColion<T>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Collect<R, C>(ref C colctor, InType<T>? t = null) where C : ISeraColctor<T, R>
        => d.Value.Collect<R, C>(ref colctor);
}
