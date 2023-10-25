using System.Runtime.CompilerServices;
using Sera.Core.Ser;

namespace Sera.Core.Impls;

#region Serialize

#region Sync

public readonly struct ArraySerializeImplWrapper<T>(ArraySerializeImplBase<T> Serialize) : ISerialize<T[]>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write<S>(S serializer, T[] value, ISeraOptions options) where S : ISerializer
        => Serialize.Write(serializer, value, options);
}

public abstract class ArraySerializeImplBase<T> : ISerialize<T[]>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public abstract void Write<S>(S serializer, T[] value, ISeraOptions options) where S : ISerializer;
}

public sealed class ArraySerializeImpl<T, ST>(ST Serialize) : ArraySerializeImplBase<T>
    where ST : ISerialize<T>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override void Write<S>(S serializer, T[] value, ISeraOptions options)
        => serializer.WriteArray(value, Serialize);
}

#endregion

#endregion
