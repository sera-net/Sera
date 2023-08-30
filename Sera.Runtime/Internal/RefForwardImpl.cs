using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Sera.Core;
using Sera.Core.Ser;

namespace Sera.Runtime.Internal;

/// <summary>Since runtime emit cannot implement interface methods with in parameters, so make such a class and forward it</summary>
internal abstract class RefForwardSerializeImpl<T> : ISerialize<T>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    protected abstract void Write<S>(S serializer, ref T value, SeraOptions options) where S : ISerializer;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    protected virtual ValueTask WriteAsync<S>(S serializer, T value, SeraOptions options) where S : IAsyncSerializer
    {
        Write(serializer, ref value, options);
        return ValueTask.CompletedTask;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    void ISerialize<T>.Write<S>(S serializer, in T value, SeraOptions options)
        => Write(serializer, ref Unsafe.AsRef(in value), options);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    ValueTask ISerialize<T>.WriteAsync<S>(S serializer, T value, SeraOptions options)
        => WriteAsync(serializer, value, options);
}
