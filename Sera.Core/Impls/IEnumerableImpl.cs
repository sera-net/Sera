using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sera.Core.Ser;

namespace Sera.Core.Impls;

#region Serialize

public record IEnumerableSerializeImpl<E, T, ST>(ST Serialize) : ISerialize<E>
    where E : IEnumerable<T> where ST : ISerialize<T>
{
    public void Write<S>(S serializer, in E value, SeraOptions options) where S : ISerializer
    {
        serializer.WriteSeqStart<T>(null);
        foreach (var item in value)
        {
            serializer.WriteSeqElement(in item, Serialize);
        }
        serializer.WriteSeqEnd();
    }

    public async ValueTask WriteAsync<S>(S serializer, E value, SeraOptions options) where S : IAsyncSerializer
    {
        await serializer.WriteSeqStartAsync<T>(null);
        foreach (var item in value)
        {
            await serializer.WriteSeqElementAsync(item, Serialize);
        }
        await serializer.WriteSeqEndAsync();
    }
}

public record IEnumerableSerializeImpl<E, ST>(ST Serialize) : ISerialize<E>
    where E : IEnumerable where ST : ISerialize<object>
{
    public void Write<S>(S serializer, in E value, SeraOptions options) where S : ISerializer
    {
        serializer.WriteSeqStart(null);
        foreach (var item in value)
        {
            serializer.WriteSeqElement(in item, Serialize);
        }
        serializer.WriteSeqEnd();
    }

    public async ValueTask WriteAsync<S>(S serializer, E value, SeraOptions options) where S : IAsyncSerializer
    {
        await serializer.WriteSeqStartAsync(null);
        foreach (var item in value)
        {
            await serializer.WriteSeqElementAsync(item, Serialize);
        }
        await serializer.WriteSeqEndAsync();
    }
}

public record IEnumerableSerializeCastImpl<E> : ISerialize<E> where E : IEnumerable
{
    public void Write<S>(S serializer, in E value, SeraOptions options) where S : ISerializer
    {
        var dyn_serializer = options.RuntimeProvider.GetRuntimeSerialize();
        serializer.WriteSeqStart(null);
        foreach (var item in value)
        {
            serializer.WriteSeqElement<object?, ISerialize<object?>>(in item, dyn_serializer);
        }
        serializer.WriteSeqEnd();
    }

    public async ValueTask WriteAsync<S>(S serializer, E value, SeraOptions options) where S : IAsyncSerializer
    {
        var dyn_serializer = options.RuntimeProvider.GetRuntimeSerialize();
        await serializer.WriteSeqStartAsync(null);
        foreach (var item in value)
        {
            await serializer.WriteSeqElementAsync<object?, ISerialize<object?>>(item, dyn_serializer);
        }
        await serializer.WriteSeqEndAsync();
    }
}

#endregion
