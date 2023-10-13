using System.Runtime.CompilerServices;
using Sera.Core;
using Sera.Core.Impls.Deps;
using Sera.Core.Ser;
using Sera.Runtime.Utils;

namespace Sera.Runtime.Emit.Ser.Internal;

public readonly struct DepsSerializeWrapper<T, ST, D> : ISerialize<T>
    where ST : ISerialize<T>
    where D : IDepsContainer<ST>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write<S>(S serializer, T value, ISeraOptions options) where S : ISerializer
        => D.Impl1!.Write(serializer, value, options);
}

public readonly struct BoxedDepsSerializeWrapper<T, ST, D> : ISerialize<T>
    where ST : ISerialize<T>
    where D : IDepsContainer<Box<ST>>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write<S>(S serializer, T value, ISeraOptions options) where S : ISerializer
        => D.Impl1!.Value.Write(serializer, value, options);
}

public readonly struct DepsSeqSerializerReceiverWrapper<T, ST, D> : ISeqSerializerReceiver<T>
    where ST : ISeqSerializerReceiver<T>
    where D : IDepsContainer<ST>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Receive<S>(T value, S serializer) where S : ISeqSerializer
        => D.Impl1!.Receive(value, serializer);
}

public readonly struct BoxedDepsSeqSerializerReceiverWrapper<T, ST, D> : ISeqSerializerReceiver<T>
    where ST : ISeqSerializerReceiver<T>
    where D : IDepsContainer<Box<ST>>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Receive<S>(T value, S serializer) where S : ISeqSerializer
        => D.Impl1!.Value.Receive(value, serializer);
}
