using System.Runtime.CompilerServices;
using Sera.Core;
using Sera.Core.Impls.Deps;
using Sera.Core.Ser;
using Sera.Runtime.Utils;

namespace Sera.Runtime.Emit.Ser.Internal;

public readonly struct DepsWrapper<T, ST, D> : ISerialize<T>
    where ST : ISerialize<T>
    where D : IDepsContainer<ST>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write<S>(S serializer, T value, ISeraOptions options) where S : ISerializer
        => D.Impl1!.Write(serializer, value, options);
}

public readonly struct BoxedDepsWrapper<T, ST, D> : ISerialize<T>
    where ST : ISerialize<T>
    where D : IDepsContainer<Box<ST>>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write<S>(S serializer, T value, ISeraOptions options) where S : ISerializer
        => D.Impl1!.Value.Write(serializer, value, options);
}
