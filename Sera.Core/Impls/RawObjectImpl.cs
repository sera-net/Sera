using System;
using System.Threading.Tasks;
using Sera.Core.De;
using Sera.Core.Ser;

namespace Sera.Core.Impls;

public class RawObjectImpl :
    ISerialize<object>, IAsyncSerialize<object>,
    IDeserialize<object>, IAsyncDeserialize<object>,
    IStructSerializerReceiver<object>, IAsyncStructSerializerReceiver<object>
{
    public static RawObjectImpl Instance { get; } = new();

    public void Write<S>(S serializer, object value, ISeraOptions options) where S : ISerializer
    {
        if (value == null!) throw new NullReferenceException();
        serializer.StartStruct<object, object, RawObjectImpl>(nameof(Object), 0, value, this);
    }

    public void Receive<S>(object value, S serialize) where S : IStructSerializer { }

    public ValueTask WriteAsync<S>(S serializer, object value, ISeraOptions options) where S : IAsyncSerializer
    {
        if (value == null!) throw new NullReferenceException();
        return serializer.StartStructAsync<object, object, RawObjectImpl>(nameof(Object), 0, value, this);
    }

    public ValueTask ReceiveAsync<S>(object value, S serialize) where S : IAsyncStructSerializer
        => ValueTask.CompletedTask;

    public object Read<D>(D deserializer, ISeraOptions options) where D : IDeserializer
        => deserializer.ReadStruct<object, EmptyStructVisitor<object>>(nameof(Object), 0,
            Array.Empty<(string, nuint?)>(),
            new(new()));

    public ValueTask<object> ReadAsync<D>(D deserializer, ISeraOptions options) where D : IAsyncDeserializer
        => deserializer.ReadStructAsync<object, EmptyStructVisitor<object>>(
            nameof(Object), 0, Array.Empty<(string, nuint?)>(), new(new())
        );
}
