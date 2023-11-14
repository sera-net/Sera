using System.Runtime.CompilerServices;

namespace Sera.Core.Impls.De;

public readonly struct DefaultCtor<T> : ISeraCtor<T>, ICapSeraCtor<T> where T : new()
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T Ctor() => new();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T Ctor(int? cap) => new();
}
