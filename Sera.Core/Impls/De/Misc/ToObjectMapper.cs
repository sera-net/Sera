using System.Runtime.CompilerServices;
using Sera.Utils;

namespace Sera.Core.Impls.De;

public readonly struct ToObjectMapper<T> : ISeraMapper<T, object?>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public object? Map(T value, InType<object>? u = null)
        => value;
}

public readonly struct SeraPrimitiveToObjectMapper : ISeraMapper<SeraPrimitive, object>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public object Map(SeraPrimitive value, InType<object>? u = null)
        => value.ToObject();
}
