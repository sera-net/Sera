using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Sera.Utils;

public class Vec<T> : IList<T>, IReadOnlyList<T>, IDisposable
{
    private readonly ArrayPool<T> pool;
    private T[] array;
    private int size;

    private const int DefaultCapacity = 4;

    public Vec(ArrayPool<T> pool)
    {
        this.pool = pool;
        array = pool.Rent(0);
        size = 0;
    }

    public Vec(int cap, ArrayPool<T> pool)
    {
        this.pool = pool;
        array = pool.Rent(cap);
        size = 0;
    }

    public Vec() : this(DirectAllocationArrayPool<T>.Instance) { }
    public Vec(int cap) : this(cap, DirectAllocationArrayPool<T>.Instance) { }


    public int Count => size;

    public bool IsReadOnly => false;

    public bool IsEmpty => size == 0;

    public Memory<T> AsMemory => array.AsMemory(0, size);
    public Span<T> AsSpan => array.AsSpan(0, size);

    public T[] UnsafeArray => array;

    public T this[int index]
    {
        get => AsSpan[index];
        set => AsSpan[index] = value;
    }

    #region Enumerator

    public RefMemoryEnumerator<T> GetEnumerator() => AsMemory.GetEnumerator();

    IEnumerator<T> IEnumerable<T>.GetEnumerator() => new MemoryEnumeratorClass<T>(AsMemory);

    IEnumerator IEnumerable.GetEnumerator() => new MemoryEnumeratorClass<T>(AsMemory);

    #endregion

    public void Add(T item)
    {
        if (size >= array.Length) Grow();
        array[size] = item;
        size += 1;
    }

    private void Grow()
    {
        var old_array = array;
        if (array.Length == 0)
        {
            array = pool.Rent(DefaultCapacity);
            pool.Return(old_array);
        }
        else
        {
            var new_array = NewGrown();
            try
            {
                AsSpan.CopyTo(new_array);
            }
            catch
            {
                pool.Return(new_array);
                throw;
            }
            array = new_array;
            pool.Return(old_array);
        }
    }

    private T[] NewGrown() => pool.Rent(array.Length * 2);

    public void Clear()
    {
        if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
        {
            if (size > 0)
            {
                Array.Clear(array, 0, size);
            }
        }
        size = 0;
    }

    public bool Contains(T item) => !IsEmpty && IndexOf(item) >= 0;

    public void CopyTo(T[] array, int arrayIndex) => AsSpan.CopyTo(array.AsSpan(arrayIndex));

    public bool Remove(T item)
    {
        var index = IndexOf(item);
        if (index < 0) return false;
        RemoveAt(index);
        return true;
    }

    public int IndexOf(T item) => Array.IndexOf(array, item, 0, size);

    public void Insert(int index, T item)
    {
        if (index < 0 || index > size) throw new ArgumentOutOfRangeException(nameof(index));
        if (index == size)
        {
            Add(item);
            return;
        }
        if (size >= array.Length)
        {
            var old_array = array;
            var new_array = NewGrown();
            try
            {
                var old_span = old_array.AsSpan(0, size);
                var new_span = new_array.AsSpan(0, size + 1);
                old_span[..index].CopyTo(new_span);
                var i1 = index + 1;
                old_span[index..].CopyTo(new_span[i1..]);
                new_span[index] = item;
            }
            catch
            {
                pool.Return(new_array);
                throw;
            }

            array = new_array;
            size += 1;
            pool.Return(old_array);
        }
        else
        {
            size += 1;
            var span = AsSpan;
            var i1 = index + 1;
            span[index..^1].CopyTo(span[i1..]);
            span[index] = item;
        }
    }

    public void RemoveAt(int index)
    {
        if (index < 0 || index >= size) throw new ArgumentOutOfRangeException(nameof(index));
        var span = AsSpan;
        size -= 1;
        var i1 = index + 1;
        span[i1..].CopyTo(span[index..]);
        if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
        {
            span[size] = default!;
        }
    }

    public T[] ToArray() => AsSpan.ToArray();

    public override string ToString()
    {
        if (typeof(T) == typeof(char)) return AsSpan.ToString();
        return $"Vec<{typeof(T).Name}>[{size}]";
    }

    #region Dispose

    protected virtual void Dispose(bool disposing) { }

    public void Dispose()
    {
        if (array != null!)
        {
            pool.Return(array);
            array = null!;
        }
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~Vec() => Dispose(false);

    #endregion
}
