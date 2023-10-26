using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Sera.Core.Ser;

namespace Sera.Core.Impls;

public readonly struct FlagsAsUnderlyingSerializeImpl<T, V, SV>(SV Serialize) : ISerialize<T>
    where T : Enum where V : unmanaged where SV : ISerialize<V>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void Write<S>(S serializer, T value, ISeraOptions options) where S : ISerializer
        => Serialize.Write(serializer, (V)(object)value, options);
}

public readonly struct AsyncFlagsAsUnderlyingSerializeImpl<T, V, SV>(SV Serialize) : IAsyncSerialize<T>
    where T : Enum where V : unmanaged where SV : IAsyncSerialize<V>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public ValueTask WriteAsync<S>(S serializer, T value, ISeraOptions options) where S : IAsyncSerializer
        => Serialize.WriteAsync(serializer, (V)(object)value, options);
}
