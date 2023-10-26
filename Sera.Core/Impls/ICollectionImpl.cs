using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Sera.Core.Ser;

namespace Sera.Core.Impls;

#region Serialize

#region Sync

#region Generic

#region Mutable

public readonly struct ICollectionSerializeImplWrapper<C, T>
    (ICollectionSerializeImplBase<C, T> Serialize) : ISerialize<C>, ISeqSerializerReceiver<C>
    where C : ICollection<T>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write<S>(S serializer, C value, ISeraOptions options) where S : ISerializer
        => Serialize.Write(serializer, value, options);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Receive<S>(C value, S serializer) where S : ISeqSerializer
        => Serialize.Receive(value, serializer);
}

public abstract class ICollectionSerializeImplBase<C, T> : ISerialize<C>, ISeqSerializerReceiver<C>
    where C : ICollection<T>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public abstract void Write<S>(S serializer, C value, ISeraOptions options) where S : ISerializer;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public abstract void Receive<S>(C value, S serializer) where S : ISeqSerializer;
}

public sealed class ICollectionSerializeImpl<C, T, ST>(ST Serialize) : ICollectionSerializeImplBase<C, T>
    where C : ICollection<T> where ST : ISerialize<T>
{
    private readonly IEnumerableSerializeReceiveImpl<C, T, ST> ReceiveImpl = new(Serialize);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override void Write<S>(S serializer, C value, ISeraOptions options)
        => serializer.StartSeq<T, C, IEnumerableSerializeReceiveImpl<C, T, ST>>((nuint)value.Count, value, ReceiveImpl);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override void Receive<S>(C value, S serializer)
        => ReceiveImpl.Receive(value, serializer);
}

#endregion

#region ReadOnly

public readonly struct IReadOnlyCollectionSerializeImplWrapper<C, T>
    (IReadOnlyCollectionSerializeImplBase<C, T> Serialize) : ISerialize<C>, ISeqSerializerReceiver<C>
    where C : IReadOnlyCollection<T>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write<S>(S serializer, C value, ISeraOptions options) where S : ISerializer
        => Serialize.Write(serializer, value, options);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Receive<S>(C value, S serializer) where S : ISeqSerializer
        => Serialize.Receive(value, serializer);
}

public abstract class IReadOnlyCollectionSerializeImplBase<C, T> : ISerialize<C>, ISeqSerializerReceiver<C>
    where C : IReadOnlyCollection<T>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public abstract void Write<S>(S serializer, C value, ISeraOptions options) where S : ISerializer;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public abstract void Receive<S>(C value, S serializer) where S : ISeqSerializer;
}

public sealed class IReadOnlyCollectionSerializeImpl<C, T, ST>
    (ST Serialize) : IReadOnlyCollectionSerializeImplBase<C, T>
    where C : IReadOnlyCollection<T> where ST : ISerialize<T>
{
    private readonly IEnumerableSerializeReceiveImpl<C, T, ST> ReceiveImpl = new(Serialize);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override void Write<S>(S serializer, C value, ISeraOptions options)
        => serializer.StartSeq<T, C, IEnumerableSerializeReceiveImpl<C, T, ST>>((nuint)value.Count, value, ReceiveImpl);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override void Receive<S>(C value, S serializer)
        => ReceiveImpl.Receive(value, serializer);
}

#endregion

#endregion

#region Legacy

public readonly struct ICollectionSerializeStaticImpl<C> : ISerialize<C>, ISeqSerializerReceiver<C>
    where C : ICollection
{
    public static ICollectionSerializeStaticImpl<C> Instance { get; } = new();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write<S>(S serializer, C value, ISeraOptions options) where S : ISerializer
        => serializer.StartSeq((nuint)value.Count, value, this);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Receive<S>(C value, S serializer) where S : ISeqSerializer
    {
        var impl = new NullableReferenceTypeSerializeImpl<object, RawObjectImpl>();
        foreach (var item in value)
        {
            serializer.WriteElement(item, impl);
        }
    }
}

public readonly struct ICollectionSerializeRuntimeImpl<C> : ISerialize<C>
    where C : ICollection
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write<S>(S serializer, C value, ISeraOptions options) where S : ISerializer
        => serializer.StartSeq((nuint)value.Count, value,
            new IEnumerableSerializeReceiveRuntimeImpl<C>(serializer.RuntimeProvider.GetRuntimeSerialize()));
}

#endregion

#endregion

#region Async

public class AsyncICollectionSerializeImpl<C, T, ST>(ST Serialize) : IAsyncSerialize<C>, IAsyncSeqSerializerReceiver<C>
    where C : ICollection<T> where ST : IAsyncSerialize<T>
{
    public ValueTask WriteAsync<S>(S serializer, C value, ISeraOptions options) where S : IAsyncSerializer
        => serializer.StartSeqAsync<T, C, AsyncICollectionSerializeImpl<C, T, ST>>((nuint)value.Count, value, this);

    public async ValueTask ReceiveAsync<S>(C value, S serializer) where S : IAsyncSeqSerializer
    {
        foreach (var item in value)
        {
            await serializer.WriteElementAsync(item, Serialize);
        }
    }
}

public class AsyncICollectionSerializeImpl<C, ST>(ST Serialize) : IAsyncSerialize<C>, IAsyncSeqSerializerReceiver<C>
    where C : ICollection where ST : IAsyncSerialize<object?>
{
    public ValueTask WriteAsync<S>(S serializer, C value, ISeraOptions options) where S : IAsyncSerializer
        => serializer.StartSeqAsync((nuint)value.Count, value, this);

    public async ValueTask ReceiveAsync<S>(C value, S serializer) where S : IAsyncSeqSerializer
    {
        foreach (var item in value)
        {
            await serializer.WriteElementAsync(item, Serialize);
        }
    }
}

#endregion

#endregion
