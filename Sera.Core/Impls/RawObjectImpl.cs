using System;
using System.Threading.Tasks;
using Sera.Core.De;
using Sera.Core.Ser;

namespace Sera.Core.Impls;

public class RawObjectImpl :
    ISerialize<object?>, IAsyncSerialize<object?>,
    IDeserialize<object?>, IAsyncDeserialize<object?>,
    IOptionDeserializerVisitor<object?>, IAsyncOptionDeserializerVisitor<object?>
{
    public static RawObjectImpl Instance { get; } = new();

    public void Write<S>(S serializer, object? value, SeraOptions options) where S : ISerializer
    {
        if (value == null)
        {
            serializer.WriteNone<object>();
        }
        else
        {
            serializer.WriteSome(value, RawObjectNonNullImpl.Instance);
        }
    }

    public ValueTask WriteAsync<S>(S serializer, object? value, SeraOptions options) where S : IAsyncSerializer
        => value == null
            ? serializer.WriteNoneAsync<object>()
            : serializer.WriteSomeAsync(value, RawObjectNonNullImpl.Instance);

    public object? Read<D>(D deserializer, SeraOptions options) where D : IDeserializer
        => deserializer.ReadOption<object?, RawObjectImpl>(this);

    public ValueTask<object?> ReadAsync<D>(D deserializer, SeraOptions options) where D : IAsyncDeserializer
        => deserializer.ReadOptionAsync<object?, RawObjectImpl>(this);

    public object? VisitNone() => null;

    public object? VisitSome<D>(D deserializer, SeraOptions options) where D : IDeserializer
        => RawObjectNonNullImpl.Instance.Read(deserializer, options);

    public ValueTask<object?> VisitNoneAsync()
        => ValueTask.FromResult<object?>(null);

    public ValueTask<object?> VisitSomeAsync<D>(D deserializer, SeraOptions options) where D : IAsyncDeserializer
        => RawObjectNonNullImpl.Instance.ReadAsync(deserializer, options)!;
}

public class RawObjectNonNullImpl :
    ISerialize<object>, IAsyncSerialize<object>,
    IDeserialize<object>, IAsyncDeserialize<object>,
    IStructSerializerReceiver<object>, IAsyncStructSerializerReceiver<object>
{
    public static RawObjectNonNullImpl Instance { get; } = new();

    public void Write<S>(S serializer, object value, SeraOptions options) where S : ISerializer
        => serializer.StartStruct<object, object, RawObjectNonNullImpl>(nameof(Object), 0, value, this);

    public void Receive<S>(object value, S serialize) where S : IStructSerializer { }

    public ValueTask WriteAsync<S>(S serializer, object value, SeraOptions options) where S : IAsyncSerializer
        => serializer.StartStructAsync<object, object, RawObjectNonNullImpl>(nameof(Object), 0, value, this);

    public ValueTask ReceiveAsync<S>(object value, S serialize) where S : IAsyncStructSerializer
        => ValueTask.CompletedTask;

    public object Read<D>(D deserializer, SeraOptions options) where D : IDeserializer
        => deserializer.ReadStruct<object, EmptyStructVisitor<object>>(nameof(Object), 0, Array.Empty<string>(),
            new(new()));

    public ValueTask<object> ReadAsync<D>(D deserializer, SeraOptions options) where D : IAsyncDeserializer
        => deserializer.ReadStructAsync<object, EmptyStructVisitor<object>>(
            nameof(Object), 0, Array.Empty<string>(), new(new())
        );
}
