using System;
using System.Runtime.CompilerServices;
using Sera.Core;
using Sera.Core.Ser;

namespace Sera.Runtime.Emit.Ser.Internal;

public class PrivateStructSerializeImpl<T> : ISerialize<T>, IStructSerializerReceiver<T>
{
    private readonly object meta_key;
    private readonly string name;
    private readonly nuint field_count;

    internal PrivateStructSerializeImpl(object meta_key, string name, nuint field_count)
    {
        this.meta_key = meta_key;
        this.name = name;
        this.field_count = field_count;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write<S>(S serializer, T value, ISeraOptions options) where S : ISerializer
    {
        if (!typeof(T).IsValueType && value == null) throw new NullReferenceException();
        serializer.StartStruct(name, field_count, value, this);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Receive<S>(T value, S serializer) where S : IStructSerializer
    {
        Jobs._Struct_Private.ReceiveImpl<T, S>.Receive(meta_key, value, serializer);
    }
}
