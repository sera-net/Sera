using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Sera.Utils;

public sealed class DynamicMap : IDictionary<object?, object?>
{
    private readonly record struct Key(object? value);

    private readonly Dictionary<Key, object?> inner = new();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IEnumerator<KeyValuePair<object?, object?>> GetEnumerator()
    {
        foreach (var (k, v) in inner)
        {
            yield return new KeyValuePair<object?, object?>(k.value, v);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Add(KeyValuePair<object?, object?> item)
        => inner.Add(new(item.Key), item.Value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Clear()
        => inner.Clear();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Contains(KeyValuePair<object?, object?> item)
        => inner.ContainsKey(new(item.Key));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void CopyTo(KeyValuePair<object?, object?>[] array, int arrayIndex)
    {
        var i = 0;
        foreach (var (k, v) in inner)
        {
            array[i + arrayIndex] = new(k.value, v);
            i++;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Remove(KeyValuePair<object?, object?> item)
        => inner.Remove(new(item.Key));

    public int Count
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => inner.Count;
    }
    public bool IsReadOnly
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Add(object? key, object? value)
        => inner.Add(new(key), value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool ContainsKey(object? key)
        => inner.ContainsKey(new(key));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Remove(object? key)
        => inner.Remove(new(key));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryGetValue(object? key, out object? value)
        => inner.TryGetValue(new(key), out value);

    public object? this[object? key]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => inner[new(key)];
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => inner[new(key)] = value;
    }

    private KeysCollection? _keys;
    public ICollection<object?> Keys
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _keys ??= new(this);
    }
    public ICollection<object?> Values
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => inner.Values;
    }

    private sealed class KeysCollection(DynamicMap self) : ICollection<object?>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerator<object?> GetEnumerator()
        {
            foreach (var (k, _) in self.inner)
            {
                yield return k.value;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(object? item)
            => throw new NotSupportedException();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear()
            => throw new NotSupportedException();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(object? item)
            => self.ContainsKey(item);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CopyTo(object?[] array, int arrayIndex)
        {
            var i = 0;
            foreach (var k in this)
            {
                array[arrayIndex + i] = k;
                i++;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Remove(object? item)
            => throw new NotSupportedException();

        public int Count
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => self.Count;
        }
        public bool IsReadOnly
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => true;
        }
    }
}
