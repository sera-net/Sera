using System.Runtime.CompilerServices;
using Sera.Utils;

namespace Sera.Core.Impls.De;

public readonly struct NullCtor<T> : ISeraCtor<T?> where T : class
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T? Ctor(InType<T?>? t = null)
        => null;
}
