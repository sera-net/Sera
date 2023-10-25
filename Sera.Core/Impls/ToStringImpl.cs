using Sera.Core.Ser;

namespace Sera.Core.Impls;

public struct ToStringSerializeImpl<T> : ISerialize<T>
{
    public static ToStringSerializeImpl<T> Instance { get; } = new();

    public void Write<S>(S serializer, T value, ISeraOptions options) where S : ISerializer
    {
        var str = $"{value}";
        serializer.WriteString(str);
    }
}
