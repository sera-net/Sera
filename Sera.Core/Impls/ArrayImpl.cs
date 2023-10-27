using System.Runtime.CompilerServices;
using Sera.Core.Ser;

namespace Sera.Core.Impls;

#region Serialize

#region Sync

public readonly struct ArraySerializeImpl<T, ST>(ST Serialize) : ISerialize<T[]>
    where ST : ISerialize<T>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write<S>(S serializer, T[] value, ISeraOptions options) where S : ISerializer
        => serializer.WriteArray(value, Serialize);
}

#endregion

#endregion
