using System;
using System.Runtime.CompilerServices;
using Sera.Utils;

namespace Sera.Core.Impls.De;

public readonly struct BytesImpl : ISeraColion<byte[]>,
    ISeraColion<ReadOnlyMemory<byte>>, ISeraColion<Memory<byte>>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Collect<R, C>(ref C colctor, InType<byte[]>? t = null) where C : ISeraColctor<byte[], R>
        => colctor.CBytes(new IdentityMapper<byte[]>(), new Type<byte[]>());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Collect<R, C>(ref C colctor, InType<ReadOnlyMemory<byte>>? t = null)
        where C : ISeraColctor<ReadOnlyMemory<byte>, R>
        => colctor.CBytes(new IdentityMapper<ReadOnlyMemory<byte>>(), new Type<ReadOnlyMemory<byte>>());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Collect<R, C>(ref C colctor, InType<Memory<byte>>? t = null) where C : ISeraColctor<Memory<byte>, R>
        => colctor.CBytes(new IdentityMapper<Memory<byte>>(), new Type<Memory<byte>>());
}
