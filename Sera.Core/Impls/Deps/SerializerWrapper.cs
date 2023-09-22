using System.Runtime.CompilerServices;
using Sera.Core.Ser;

namespace Sera.Core.Impls.Deps;

public readonly struct DepsSerializerWrapper1<T, ST, D> : ISerialize<T>
    where ST : ISerialize<T>
    where D : IDepsContainer<ST>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write<S>(S serializer, T value, ISeraOptions options) where S : ISerializer
        => D.Impl1!.Write(serializer, value, options);
}

public readonly struct DepsSerializerWrapper2<T, ST, D> : ISerialize<T>
    where ST : ISerialize<T>
    where D : IDepsContainer2<ST>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write<S>(S serializer, T value, ISeraOptions options) where S : ISerializer
        => D.Impl2!.Write(serializer, value, options);
}

public readonly struct DepsSerializerWrapper3<T, ST, D> : ISerialize<T>
    where ST : ISerialize<T>
    where D : IDepsContainer3<ST>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write<S>(S serializer, T value, ISeraOptions options) where S : ISerializer
        => D.Impl3!.Write(serializer, value, options);
}

public readonly struct DepsSerializerWrapper4<T, ST, D> : ISerialize<T>
    where ST : ISerialize<T>
    where D : IDepsContainer4<ST>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write<S>(S serializer, T value, ISeraOptions options) where S : ISerializer
        => D.Impl4!.Write(serializer, value, options);
}

public readonly struct DepsSerializerWrapper5<T, ST, D> : ISerialize<T>
    where ST : ISerialize<T>
    where D : IDepsContainer5<ST>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write<S>(S serializer, T value, ISeraOptions options) where S : ISerializer
        => D.Impl5!.Write(serializer, value, options);
}

public readonly struct DepsSerializerWrapper6<T, ST, D> : ISerialize<T>
    where ST : ISerialize<T>
    where D : IDepsContainer6<ST>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write<S>(S serializer, T value, ISeraOptions options) where S : ISerializer
        => D.Impl6!.Write(serializer, value, options);
}

public readonly struct DepsSerializerWrapper7<T, ST, D> : ISerialize<T>
    where ST : ISerialize<T>
    where D : IDepsContainer7<ST>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write<S>(S serializer, T value, ISeraOptions options) where S : ISerializer
        => D.Impl7!.Write(serializer, value, options);
}

public readonly struct DepsSerializerWrapper8<T, ST, D> : ISerialize<T>
    where ST : ISerialize<T>
    where D : IDepsContainer8<ST>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write<S>(S serializer, T value, ISeraOptions options) where S : ISerializer
        => D.Impl8!.Write(serializer, value, options);
}
