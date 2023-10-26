using System.Runtime.CompilerServices;
using Sera.Core;
using Sera.Core.Ser;

namespace Sera.Runtime.Emit.Ser.Internal;

public readonly struct RuntimeSerializeImpl : ISerialize<object>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write<S>(S serializer, object value, ISeraOptions options) where S : ISerializer
    {
        var type = value.GetType();
        var ser = EmitRuntimeProvider.Instance.serializeEmitProvider.GetRuntimeSerialize(type);
        ser.Write(serializer, value, options);
    }
}
