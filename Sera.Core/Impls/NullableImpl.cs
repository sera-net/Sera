using System.Threading.Tasks;
using Sera.Core.Ser;

namespace Sera.Core.Impls;

public record NullableObjectImpl<T, ST>(ST Serialize) : ISerialize<T?> where T : class where ST : ISerialize<T>
{
    public void Write<S>(S serializer, T? value, ISeraOptions options) where S : ISerializer
    {
        if (value == null) serializer.WriteNone<T>();
        else serializer.WriteSome(value, Serialize);
    }
}

public record AsyncNullableObjectImpl<T, ST>(ST Serialize) : IAsyncSerialize<T?> where T : class where ST : IAsyncSerialize<T>
{
    public ValueTask WriteAsync<S>(S serializer, T? value, ISeraOptions options) where S : IAsyncSerializer
        => value == null ? serializer.WriteNoneAsync<T>() : serializer.WriteSomeAsync(value, Serialize);
}

public record NullableStructImpl<T, ST>(ST Serialize) : ISerialize<T?> where T : struct where ST : ISerialize<T>
{
    public void Write<S>(S serializer, T? value, ISeraOptions options) where S : ISerializer
    {
        if (value == null) serializer.WriteNone<T>();
        else serializer.WriteSome(value.Value, Serialize);
    }
}

public record AsyncNullableStructImpl<T, ST>(ST Serialize) : IAsyncSerialize<T?> where T : struct where ST : IAsyncSerialize<T>
{
    public ValueTask WriteAsync<S>(S serializer, T? value, ISeraOptions options) where S : IAsyncSerializer
        => value == null ? serializer.WriteNoneAsync<T>() : serializer.WriteSomeAsync(value.Value, Serialize);
}
