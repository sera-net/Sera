using System;
using System.Threading.Tasks;
using Sera.Core.Ser;

namespace Sera.Core.Impls;

public struct FlagsToStringSplitSerializeImpl<T> :
    ISerialize<T>, ISeqSerializerReceiver<string[]>
    where T : Enum
{
    public static FlagsToStringSplitSerializeImpl<T> Instance { get; } = new();

    public void Write<S>(S serializer, T value, ISeraOptions options) where S : ISerializer
    {
        var str = $"{value}".Split(", ");
        serializer.StartSeq<string, string[], FlagsToStringSplitSerializeImpl<T>>((nuint)str.LongLength, str, this);
    }

    public void Receive<S>(string[] value, S serializer) where S : ISeqSerializer
    {
        foreach (var item in value)
        {
            serializer.WriteElement(item, StringImpl.Instance);
        }
    }
}
