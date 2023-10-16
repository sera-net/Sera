using System;
using System.Runtime.CompilerServices;
using Sera.Core;
using Sera.Core.Ser;

namespace Sera.Runtime.Emit.Ser.Internal;

public struct PrivateStructSerializeImpl<T> : ISerialize<T>, IStructSerializerReceiver<T>
{
    private sealed record Data(object meta_key, string name, nuint field_count);

    private readonly Data data;

    internal PrivateStructSerializeImpl(object meta_key, string name, nuint field_count)
        => data = new(meta_key, name, field_count);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write<S>(S serializer, T value, ISeraOptions options) where S : ISerializer
    {
        if (!typeof(T).IsValueType && value == null) throw new NullReferenceException();
        serializer.StartStruct(data.name, data.field_count, value, this);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Receive<S>(T value, S serializer) where S : IStructSerializer
    {
        Jobs._Struct._Private.ReceiveImpl<T, S>.Receive(data.meta_key, value, serializer);
    }
}
