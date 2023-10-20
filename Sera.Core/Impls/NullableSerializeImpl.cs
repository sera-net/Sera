using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Sera.Core.Ser;

namespace Sera.Core.Impls;

#region ValueType

#region Sync

public readonly struct NullableSerializeImplWrapper<T>(NullableSerializeImplBase<T> Serialize) : ISerialize<T?>
    where T : struct
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write<S>(S serializer, T? value, ISeraOptions options) where S : ISerializer
        => Serialize.Write(serializer, value, options);
}

public abstract class NullableSerializeImplBase<T> : ISerialize<T?> where T : struct
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public abstract void Write<S>(S serializer, T? value, ISeraOptions options) where S : ISerializer;
}

public sealed class NullableSerializeImpl<T, ST>(ST Serialize) : NullableSerializeImplBase<T>
    where T : struct where ST : ISerialize<T>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override void Write<S>(S serializer, T? value, ISeraOptions options)
    {
        if (value == null) serializer.WriteNone<T>();
        else serializer.WriteSome(value.Value, Serialize);
    }
}

#endregion

#region Async

public readonly struct AsyncNullableSerializeImpl<T, ST>(ST Serialize) : IAsyncSerialize<T?>
    where T : struct where ST : IAsyncSerialize<T>
{
    public ValueTask WriteAsync<S>(S serializer, T? value, ISeraOptions options) where S : IAsyncSerializer
        => value == null ? serializer.WriteNoneAsync<T>() : serializer.WriteSomeAsync(value.Value, Serialize);
}

#endregion

#endregion

#region ReferenceType

public readonly struct NullableReferenceTypeSerializeImpl<T, ST>(ST Serialize) : ISerialize<T?>
    where T : class where ST : ISerialize<T>
{
    public void Write<S>(S serializer, T? value, ISeraOptions options) where S : ISerializer
    {
        if (value == null) serializer.WriteNone<T>();
        else serializer.WriteSome(value, Serialize);
    }
}

public readonly struct AsyncNullableReferenceTypeSerializeImpl<T, ST>(ST Serialize) : IAsyncSerialize<T?>
    where T : class where ST : IAsyncSerialize<T>
{
    public ValueTask WriteAsync<S>(S serializer, T? value, ISeraOptions options) where S : IAsyncSerializer
        => value == null ? serializer.WriteNoneAsync<T>() : serializer.WriteSomeAsync(value, Serialize);
}

#endregion
