using System;
using System.Buffers;

namespace Sera.Utils;

public readonly struct RAIIArrayPool<T>(T[] array, int size) : IDisposable
{
    public T[] Array => array;
    public int Size => size;
    public Memory<T> Memory => new(array, 0, size);
    public Span<T> Span => new(array, 0, size);

    public static RAIIArrayPool<T> Get(int size)
    {
        var arr = ArrayPool<T>.Shared.Rent(size);
        return new(arr, size);
    }

    public void Dispose()
    {
        ArrayPool<T>.Shared.Return(array);
    }

    public ref T this[int i] => ref Array[i];
}
