using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Sera.Core;
using Sera.Core.Ser;
using Sera.Core.De;

namespace Sera;

/// <summary>
/// Provide serialization implementation for <see cref="T"/>.
/// </summary>
public interface ISerialize<in T>
{
    /// <summary>
    /// Serialize this value into the given serializer.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write<S>(S serializer, T value, SeraOptions options) where S : ISerializer;
}

/// <summary>
/// Provide serialization implementation for <see cref="T"/>.
/// </summary>
public interface IAsyncSerialize<in T>
{
    /// <summary>
    /// Serialize this value into the given serializer.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ValueTask WriteAsync<S>(S serializer, T value, SeraOptions options) where S : IAsyncSerializer;
}

/// <summary>
/// Provides deserialization implementation for <see cref="T"/>.
/// </summary>
public interface IDeserialize<out T>
{
    /// <summary>
    /// Deserialize this value from the given deserializer.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T Read<D>(D deserializer, SeraOptions options) where D : IDeserializer;
}

/// <summary>
/// Provides deserialization implementation for <see cref="T"/>.
/// </summary>
public interface IAsyncDeserialize<T>
{
    /// <summary>
    /// Deserialize this value from the given deserializer.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ValueTask<T> ReadAsync<D>(D deserializer, SeraOptions options) where D : IAsyncDeserializer;
}
