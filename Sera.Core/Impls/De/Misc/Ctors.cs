using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Sera.Core.Impls.De;

public readonly struct ListCtor<I> : ICapSeraCtor<List<I>>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public List<I> Ctor(int? count) => count is { } c ? new(c) : new();
}

public readonly struct DictionaryCtor<K, V> : ICapSeraCtor<Dictionary<K, V>> where K : notnull
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Dictionary<K, V> Ctor(int? count) => count is { } c ? new(c) : new();
}

public readonly struct HashSetCtor<I> : ICapSeraCtor<HashSet<I>>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HashSet<I> Ctor(int? count) => count is { } c ? new(c) : new();
}
