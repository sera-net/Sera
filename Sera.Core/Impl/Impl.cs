using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Sera.Core;
using Sera.Core.Ser;
using Sera.Core.De;

namespace Sera;

/// <summary>
/// Provide serialization implementation for <see cref="T"/>.
/// </summary>
public interface ISerialize<T>
{
    /// <summary>
    /// Serialize this value into the given serializer.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write<S>(S serializer, in T value, SeraOptions options) where S : ISerializer;

    /// <summary>
    /// Serialize this value into the given serializer.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ValueTask WriteAsync<S>(S serializer, T value, SeraOptions options) where S : IAsyncSerializer
    {
        Write(serializer, in value, options);
        return ValueTask.CompletedTask;
    }
}

/// <summary>
/// Provides deserialization implementation for <see cref="T"/>.
/// </summary>
public interface IDeserialize<T>
{
    /// <summary>
    /// Deserialize this value from the given deserializer.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Read<D>(D deserializer, out T value, SeraOptions options) where D : IDeserializer;

    /// <summary>
    /// Deserialize this value from the given deserializer.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ValueTask<T> ReadAsync<D>(D deserializer, SeraOptions options) where D : IAsyncDeserializer
    {
        Read(deserializer, out var r, options);
        return ValueTask.FromResult(r);
    }
}
