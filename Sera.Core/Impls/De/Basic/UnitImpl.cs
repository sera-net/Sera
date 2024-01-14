using System.Runtime.CompilerServices;
using Sera.Utils;

namespace Sera.Core.Impls.De;

public readonly struct UnitImpl<T> : ISeraColion<T>, ISeraCtor<T>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Collect<R, C>(ref C colctor, InType<T>? t = null) where C : ISeraColctor<T, R>
        => colctor.CUnit(this);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T Ctor(InType<T>? t) => default!;
}
