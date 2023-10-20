using System.Runtime.CompilerServices;
using Sera.Core;
using Sera.Core.Ser;

namespace Sera.Runtime.Emit.Ser.Internal;

public readonly struct RuntimeSerializeImplWrapper<T, ST>(ST Serialize) : ISerialize<object>
    where ST : ISerialize<T>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write<S>(S serializer, object value, ISeraOptions options) where S : ISerializer
        => Serialize.Write(serializer, (T)value, options);
}
