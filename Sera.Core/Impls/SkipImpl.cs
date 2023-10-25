using System;
using System.Threading.Tasks;
using Sera.Core.De;
using Sera.Core.Ser;

namespace Sera.Core.Impls;

public struct SkipImpl<T> : ISerialize<T>, IDeserialize<T>
{
    public static SkipImpl<T> Instance => new();

    public void Write<S>(S serializer, T value, ISeraOptions options) where S : ISerializer
        => serializer.WriteUnit();


    public T Read<D>(D deserializer, ISeraOptions options) where D : IDeserializer
    {
        deserializer.Skip();
        return default!;
    }
}
