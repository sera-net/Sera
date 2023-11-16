using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Sera.Utils;

namespace Sera.Core.Impls.De;

public readonly struct DefaultCtor<T> :
    ISeraCtor<T>,
    ICapSeraCtor<T>
    where T : new()
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T Ctor(InType<T>? t) => new();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T Ctor(int? cap, InType<T>? t) => new();
}

public readonly struct SeqCapCtor<I> :
    ICapSeraCtor<ICollection<I>>,
    ICapSeraCtor<List<I>>,
    ICapSeraCtor<HashSet<I>>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ICollection<I> Ctor(int? cap, InType<ICollection<I>>? t = null)
        => Ctor(cap, (InType<List<I>>?)null);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public List<I> Ctor(int? cap, InType<List<I>>? t) => cap is { } c ? new(c) : new();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HashSet<I> Ctor(int? cap, InType<HashSet<I>>? t) => cap is { } c ? new(c) : new();
}

public readonly struct DictCapCtor<K, V> :
    ICapSeraCtor<IDictionary<K, V>>,
    ICapSeraCtor<Dictionary<K, V>>
    where K : notnull
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IDictionary<K, V> Ctor(int? cap, InType<IDictionary<K, V>>? t = null)
        => Ctor(cap, (InType<Dictionary<K, V>>?)null);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Dictionary<K, V> Ctor(int? count, InType<Dictionary<K, V>>? t) => count is { } c ? new(c) : new();
}
