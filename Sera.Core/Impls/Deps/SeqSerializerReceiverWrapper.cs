using System.Runtime.CompilerServices;
using Sera.Core.Ser;

namespace Sera.Core.Impls.Deps;

public readonly struct DepsSeqSerializerReceiverWrapper1<T, ST, D> : ISeqSerializerReceiver<T>
    where ST : ISeqSerializerReceiver<T>
    where D : IDepsContainer<ST>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Receive<S>(T value, S serializer) where S : ISeqSerializer
        => D.Impl1!.Receive(value, serializer);
}

public readonly struct DepsSeqSerializerReceiverWrapper2<T, ST, D> : ISeqSerializerReceiver<T>
    where ST : ISeqSerializerReceiver<T>
    where D : IDepsContainer2<ST>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Receive<S>(T value, S serializer) where S : ISeqSerializer
        => D.Impl2!.Receive(value, serializer);
}

public readonly struct DepsSeqSerializerReceiverWrapper3<T, ST, D> : ISeqSerializerReceiver<T>
    where ST : ISeqSerializerReceiver<T>
    where D : IDepsContainer3<ST>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Receive<S>(T value, S serializer) where S : ISeqSerializer
        => D.Impl3!.Receive(value, serializer);
}

public readonly struct DepsSeqSerializerReceiverWrapper4<T, ST, D> : ISeqSerializerReceiver<T>
    where ST : ISeqSerializerReceiver<T>
    where D : IDepsContainer4<ST>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Receive<S>(T value, S serializer) where S : ISeqSerializer
        => D.Impl4!.Receive(value, serializer);
}

public readonly struct DepsSeqSerializerReceiverWrapper5<T, ST, D> : ISeqSerializerReceiver<T>
    where ST : ISeqSerializerReceiver<T>
    where D : IDepsContainer5<ST>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Receive<S>(T value, S serializer) where S : ISeqSerializer
        => D.Impl5!.Receive(value, serializer);
}

public readonly struct DepsSeqSerializerReceiverWrapper6<T, ST, D> : ISeqSerializerReceiver<T>
    where ST : ISeqSerializerReceiver<T>
    where D : IDepsContainer6<ST>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Receive<S>(T value, S serializer) where S : ISeqSerializer
        => D.Impl6!.Receive(value, serializer);
}

public readonly struct DepsSeqSerializerReceiverWrapper7<T, ST, D> : ISeqSerializerReceiver<T>
    where ST : ISeqSerializerReceiver<T>
    where D : IDepsContainer7<ST>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Receive<S>(T value, S serializer) where S : ISeqSerializer
        => D.Impl7!.Receive(value, serializer);
}

public readonly struct DepsSeqSerializerReceiverWrapper8<T, ST, D> : ISeqSerializerReceiver<T>
    where ST : ISeqSerializerReceiver<T>
    where D : IDepsContainer8<ST>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Receive<S>(T value, S serializer) where S : ISeqSerializer
        => D.Impl8!.Receive(value, serializer);
}
