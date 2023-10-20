using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Sera.Core.Ser;

namespace Sera.Core.Impls;

#region Serialize

#region Sync

#region Generic

public readonly struct IEnumerableSerializeImplWrapper<E, T>
    (IEnumerableSerializeImplBase<E, T> Serialize) : ISerialize<E>, ISeqSerializerReceiver<E>
    where E : IEnumerable<T>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write<S>(S serializer, E value, ISeraOptions options) where S : ISerializer
        => Serialize.Write(serializer, value, options);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Receive<S>(E value, S serializer) where S : ISeqSerializer
        => Serialize.Receive(value, serializer);
}

public abstract class IEnumerableSerializeImplBase<E, T> : ISerialize<E>, ISeqSerializerReceiver<E>
    where E : IEnumerable<T>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public abstract void Write<S>(S serializer, E value, ISeraOptions options) where S : ISerializer;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public abstract void Receive<S>(E value, S serializer) where S : ISeqSerializer;
}

public sealed class IEnumerableSerializeImpl<E, T, ST>(ST Serialize) : IEnumerableSerializeImplBase<E, T>
    where E : IEnumerable<T> where ST : ISerialize<T>
{
    private readonly IEnumerableSerializeReceiveImpl<E, T, ST> ReceiveImpl = new(Serialize);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override void Write<S>(S serializer, E value, ISeraOptions options)
        => serializer.StartSeq<T, E, IEnumerableSerializeReceiveImpl<E, T, ST>>(null, value, ReceiveImpl);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override void Receive<S>(E value, S serializer)
        => ReceiveImpl.Receive(value, serializer);
}

public readonly struct IEnumerableSerializeReceiveImpl<E, T, ST>(ST Serialize) : ISeqSerializerReceiver<E>
    where E : IEnumerable<T> where ST : ISerialize<T>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Receive<S>(E value, S serializer) where S : ISeqSerializer
    {
        foreach (var item in value)
        {
            serializer.WriteElement(item, Serialize);
        }
    }
}

#endregion

#region Legacy

public readonly struct IEnumerableSerializeStaticImpl<E> : ISerialize<E>, ISeqSerializerReceiver<E>
    where E : IEnumerable
{
    public static IEnumerableSerializeStaticImpl<E> Instance { get; } = new();
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write<S>(S serializer, E value, ISeraOptions options) where S : ISerializer
        => serializer.StartSeq(null, value, this);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Receive<S>(E value, S serializer) where S : ISeqSerializer
    {
        foreach (var item in value)
        {
            serializer.WriteElement(item, RawObjectImpl.Instance);
        }
    }
}

public readonly struct IEnumerableSerializeRuntimeImpl<E> : ISerialize<E>
    where E : IEnumerable
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write<S>(S serializer, E value, ISeraOptions options) where S : ISerializer
        => serializer.StartSeq(null, value,
            new IEnumerableSerializeReceiveRuntimeImpl<E>(serializer.RuntimeProvider.GetRuntimeSerialize()));
}

public readonly struct IEnumerableSerializeReceiveRuntimeImpl<E>(ISerialize<object?> Serialize) : ISeqSerializerReceiver<E>
    where E : IEnumerable
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Receive<S>(E value, S serializer) where S : ISeqSerializer
    {
        foreach (var item in value)
        {
            serializer.WriteElement(item, Serialize);
        }
    }
}

#endregion

#endregion

#region Async

public class AsyncIEnumerableSerializeImpl<E, T, ST>(ST Serialize) : IAsyncSerialize<E>, IAsyncSeqSerializerReceiver<E>
    where E : IEnumerable<T> where ST : IAsyncSerialize<T>
{
    public ValueTask WriteAsync<S>(S serializer, E value, ISeraOptions options) where S : IAsyncSerializer
        => serializer.StartSeqAsync<T, E, AsyncIEnumerableSerializeImpl<E, T, ST>>(null, value, this);

    public async ValueTask ReceiveAsync<S>(E value, S serializer) where S : IAsyncSeqSerializer
    {
        foreach (var item in value)
        {
            await serializer.WriteElementAsync(item, Serialize);
        }
    }
}

public class AsyncIAsyncEnumerableSerializeImpl<E, T, ST>(ST Serialize) : IAsyncSerialize<E>,
    IAsyncSeqSerializerReceiver<E>
    where E : IAsyncEnumerable<T> where ST : IAsyncSerialize<T>
{
    public ValueTask WriteAsync<S>(S serializer, E value, ISeraOptions options) where S : IAsyncSerializer
        => serializer.StartSeqAsync<T, E, AsyncIAsyncEnumerableSerializeImpl<E, T, ST>>(null, value, this);

    public async ValueTask ReceiveAsync<S>(E value, S serializer) where S : IAsyncSeqSerializer
    {
        await foreach (var item in value)
        {
            await serializer.WriteElementAsync(item, Serialize);
        }
    }
}

public class AsyncIEnumerableSerializeImpl<E, ST>(ST Serialize) : IAsyncSerialize<E>, IAsyncSeqSerializerReceiver<E>
    where E : IEnumerable where ST : IAsyncSerialize<object>
{
    public ValueTask WriteAsync<S>(S serializer, E value, ISeraOptions options) where S : IAsyncSerializer
        => serializer.StartSeqAsync(null, value, this);

    public async ValueTask ReceiveAsync<S>(E value, S serializer) where S : IAsyncSeqSerializer
    {
        foreach (var item in value)
        {
            await serializer.WriteElementAsync(item, Serialize);
        }
    }
}

#endregion

#endregion
