using System.Threading.Tasks;
using Sera.Core.Ser;

namespace Sera.Core.Impls;

public readonly struct ReferenceTypeWrapperSerializeImpl<T, ST>(ST Serialize) : ISerialize<T>
    where T : class where ST : ISerialize<T>
{
    public void Write<S>(S serializer, T value, ISeraOptions options) where S : ISerializer
    {
        if (serializer.MarkReference(value, Serialize)) return;
        Serialize.Write(serializer, value, options);
    }
}
