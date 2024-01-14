using System.Runtime.CompilerServices;
using Sera.Utils;

namespace Sera.Core.Impls.De;

public readonly struct UnitAsNullImpl<T> : ISeraColion<T?> where T : class
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Collect<R, C>(ref C colctor, InType<T?>? t = null) where C : ISeraColctor<T?, R>
        => colctor.CUnit(new NullCtor<T>());
}
