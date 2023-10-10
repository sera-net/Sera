using System;
using System.Runtime.CompilerServices;
using Sera.Core;
using Sera.Core.Impls;
using Sera.Core.Ser;

namespace Sera.Runtime.Emit.Ser.Internal;

public record struct FlagInfo<T>(string name, T value);

public readonly struct FlagsArrayImpl<T>(FlagInfo<T>[] Items) : ISerialize<T>, ISeqSerializerReceiver<T>
    where T : Enum
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write<S>(S serializer, T value, ISeraOptions options) where S : ISerializer
    {
        serializer.StartSeq<string, T, FlagsArrayImpl<T>>(null, value, this);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Receive<S>(T value, S serializer) where S : ISeqSerializer
    {
        foreach (var item in Items)
        {
            if (value.HasFlag(item.value))
            {
                serializer.WriteElement(item.name, StringImpl.Instance);
            }
        }
    }
}
