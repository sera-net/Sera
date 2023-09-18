using System;
using System.Threading.Tasks;
using Sera.Core.Ser;

namespace Sera.Core.Impls;

public class FlagsToStringSplitSerializeImpl<T> :
    ISerialize<T>, ISeqSerializerReceiver<string[]>, IAsyncSerialize<T>, IAsyncSeqSerializerReceiver<string[]>
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

    public ValueTask WriteAsync<S>(S serializer, T value, ISeraOptions options) where S : IAsyncSerializer
    {
        var str = $"{value}".Split(", ");
        return serializer.StartSeqAsync<string, string[], FlagsToStringSplitSerializeImpl<T>>((nuint)str.LongLength,
            str, this);
    }

    public async ValueTask ReceiveAsync<S>(string[] value, S serializer) where S : IAsyncSeqSerializer
    {
        foreach (var item in value)
        {
            await serializer.WriteElementAsync(item, StringImpl.Instance);
        }
    }
}
