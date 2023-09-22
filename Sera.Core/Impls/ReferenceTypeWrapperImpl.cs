using System.Threading.Tasks;
using Sera.Core.Ser;

namespace Sera.Core.Impls;

public record struct ReferenceTypeWrapperSerializeImpl<T, ST>(ST Serialize) : ISerialize<T>
    where T : class where ST : ISerialize<T>
{
    public void Write<S>(S serializer, T value, ISeraOptions options) where S : ISerializer
    {
        if (serializer.MarkReference(value, Serialize)) return;
        Serialize.Write(serializer, value, options);
    }
}

public record struct AsyncReferenceTypeWrapperSerializeImpl<T, ST>(ST Serialize) : IAsyncSerialize<T>
    where T : class where ST : IAsyncSerialize<T>
{
    public async ValueTask WriteAsync<S>(S serializer, T value, ISeraOptions options) where S : IAsyncSerializer
    {
        if (await serializer.MarkReferenceAsync(value, Serialize)) return;
        await Serialize.WriteAsync(serializer, value, options);
    }
}
