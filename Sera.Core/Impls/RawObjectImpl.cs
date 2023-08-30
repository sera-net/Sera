using System;
using System.Threading.Tasks;
using Sera.Core.De;
using Sera.Core.Ser;

namespace Sera.Core.Impls;

public record RawObjectImpl : ISerialize<object?>, IDeserialize<object?>
{
    public static RawObjectImpl Instance { get; } = new();

    public void Write<S>(S serializer, in object? value, SeraOptions options) where S : ISerializer
    {
        if (value == null)
        {
            serializer.WriteNull();
        }
        else
        {
            serializer.WriteStructStart<object>(nameof(Object), 0);
            serializer.WriteStructEnd();
        }
    }

    public async ValueTask WriteAsync<S>(S serializer, object? value, SeraOptions options) where S : IAsyncSerializer
    {
        if (value == null)
        {
            await serializer.WriteNullAsync();
        }
        else
        {
            await serializer.WriteStructStartAsync<object>(nameof(Object), 0);
            await serializer.WriteStructEndAsync();
        }
    }

    public void Read<D>(D deserializer, out object? value, SeraOptions options) where D : IDeserializer
    {
        if (deserializer.PeekIsNull())
        {
            deserializer.ReadNull();
            value = null;
        }
        else
        {
            deserializer.ReadStructStart<object>(nameof(Object), 0);
            deserializer.ReadStructEnd();
            value = new();
        }
    }

    public async ValueTask<object?> ReadAsync<D>(D deserializer, SeraOptions options) where D : IAsyncDeserializer
    {
        if (await deserializer.PeekIsNullAsync())
        {
            await deserializer.ReadNullAsync();
            return null;
        }
        else
        {
            await deserializer.ReadStructStartAsync<object>(nameof(Object), 0);
            await deserializer.ReadStructEndAsync();
            return new();
        }
    }
}

public record RawObjectNonNullImpl : ISerialize<object>, IDeserialize<object>
{
    public static RawObjectNonNullImpl Instance { get; } = new();

    public void Write<S>(S serializer, in object value, SeraOptions options) where S : ISerializer
    {
        serializer.WriteStructStart<object>(nameof(Object), 0);
        serializer.WriteStructEnd();
    }

    public async ValueTask WriteAsync<S>(S serializer, object value, SeraOptions options) where S : IAsyncSerializer
    {
        await serializer.WriteStructStartAsync<object>(nameof(Object), 0);
        await serializer.WriteStructEndAsync();
    }

    public void Read<D>(D deserializer, out object value, SeraOptions options) where D : IDeserializer
    {
        deserializer.ReadStructStart<object>(nameof(Object), 0);
        deserializer.ReadStructEnd();
        value = new();
    }

    public async ValueTask<object> ReadAsync<D>(D deserializer, SeraOptions options) where D : IAsyncDeserializer
    {
        await deserializer.ReadStructStartAsync<object>(nameof(Object), 0);
        await deserializer.ReadStructEndAsync();
        return new();
    }
}