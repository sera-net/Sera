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
    public void Write<S>(S serializer, T value, ISeraOptions options) where S : ISerializer;
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
    public T Read<D>(D deserializer, ISeraOptions options) where D : IDeserializer;
}
