using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sera.Core.Ser;

namespace Sera.Core.Impls;

#region Serialize

public record ICollectionSerializeImpl<C, T, ST>(ST Serialize) : ISerialize<C>
    where C : ICollection<T> where ST : ISerialize<T>
{
    public void Write<S>(S serializer, in C value, SeraOptions options) where S : ISerializer
    {
        serializer.WriteSeqStart<T>((nuint)value.Count);
        foreach (var item in value)
        {
            serializer.WriteSeqElement(in item, Serialize);
        }
        serializer.WriteSeqEnd();
    }

    public async ValueTask WriteAsync<S>(S serializer, C value, SeraOptions options) where S : IAsyncSerializer
    {
        await serializer.WriteSeqStartAsync<T>((nuint)value.Count);
        foreach (var item in value)
        {
            await serializer.WriteSeqElementAsync(item, Serialize);
        }
        await serializer.WriteSeqEndAsync();
    }
}

public record ICollectionSerializeImpl<C, ST>(ST Serialize) : ISerialize<C>
    where C : ICollection where ST : ISerialize<object?>
{
    public void Write<S>(S serializer, in C value, SeraOptions options) where S : ISerializer
    {
        serializer.WriteSeqStart((nuint)value.Count);
        foreach (var item in value)
        {
            serializer.WriteSeqElement<object?, ST>(in item, Serialize);
        }
        serializer.WriteSeqEnd();
    }

    public async ValueTask WriteAsync<S>(S serializer, C value, SeraOptions options) where S : IAsyncSerializer
    {
        await serializer.WriteSeqStartAsync((nuint)value.Count);
        foreach (var item in value)
        {
            await serializer.WriteSeqElementAsync<object?, ST>(item, Serialize);
        }
        await serializer.WriteSeqEndAsync();
    }
}

public record ICollectionSerializeCastImpl<C> : ISerialize<C> where C : ICollection
{
    public void Write<S>(S serializer, in C value, SeraOptions options) where S : ISerializer
    {
        var dyn_serializer = options.RuntimeProvider.GetRuntimeSerialize();
        serializer.WriteSeqStart((nuint)value.Count);
        foreach (var item in value)
        {
            serializer.WriteSeqElement<object?, ISerialize<object?>>(in item, dyn_serializer);
        }
        serializer.WriteSeqEnd();
    }

    public async ValueTask WriteAsync<S>(S serializer, C value, SeraOptions options) where S : IAsyncSerializer
    {
        var dyn_serializer = options.RuntimeProvider.GetRuntimeSerialize();
        await serializer.WriteSeqStartAsync((nuint)value.Count);
        foreach (var item in value)
        {
            await serializer.WriteSeqElementAsync<object?, ISerialize<object?>>(item, dyn_serializer);
        }
        await serializer.WriteSeqEndAsync();
    }
}

#endregion
