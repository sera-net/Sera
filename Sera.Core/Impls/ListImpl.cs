using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Sera.Core.De;
using Sera.Core.Ser;

namespace Sera.Core.Impls;

#region Serialize

public readonly struct ListSerializeImplWrapper<L, T>(ListSerializeImplBase<L, T> Serialize) : ISerialize<L>
    where L : List<T>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write<S>(S serializer, L value, ISeraOptions options) where S : ISerializer
        => Serialize.Write(serializer, value, options);
}

public abstract class ListSerializeImplBase<L, T> : ISerialize<L> where L : List<T>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public abstract void Write<S>(S serializer, L value, ISeraOptions options) where S : ISerializer;
}

public sealed class ListSerializeImpl<L, T, ST>(ST Serialize) : ListSerializeImplBase<L, T>
    where L : List<T> where ST : ISerialize<T>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override void Write<S>(S serializer, L value, ISeraOptions options)
        => serializer.WriteArray(value, Serialize);
}

#endregion
