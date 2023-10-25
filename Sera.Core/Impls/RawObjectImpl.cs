using System;
using System.Threading.Tasks;
using Sera.Core.De;
using Sera.Core.Ser;

namespace Sera.Core.Impls;

public readonly struct RawObjectImpl :
    ISerialize<object>,
    IStructSerializerReceiver<object>
{
    public static RawObjectImpl Instance { get; } = new();

    public void Write<S>(S serializer, object value, ISeraOptions options) where S : ISerializer
    {
        if (value == null!) throw new NullReferenceException();
        serializer.StartStruct<object, object, RawObjectImpl>(nameof(Object), 0, value, this);
    }

    public void Receive<S>(object value, S serialize) where S : IStructSerializer { }
}
