﻿using System;
using System.Runtime.CompilerServices;
using System.Text;
using Sera.Core.Impls.De;
using Sera.Utils;

namespace Sera.Core.Impls.De;

public readonly struct StringImpl :
    ISeraColion<string>, ISeraColion<char[]>,
    ISeraColion<ReadOnlyMemory<char>>, ISeraColion<Memory<char>>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Collect<R, C>(ref C colctor, InType<string>? t = null)
        where C : ISeraColctor<string, R>
        => colctor.CString(new IdentityMapper<string>(), new Type<string>());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Collect<R, C>(ref C colctor, InType<char[]>? t = null)
        where C : ISeraColctor<char[], R>
        => colctor.CString(new IdentityMapper<char[]>(), new Type<char[]>());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Collect<R, C>(ref C colctor, InType<ReadOnlyMemory<char>>? t = null)
        where C : ISeraColctor<ReadOnlyMemory<char>, R>
        => colctor.CString(new IdentityMapper<ReadOnlyMemory<char>>(), new Type<ReadOnlyMemory<char>>());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Collect<R, C>(ref C colctor, InType<Memory<char>>? t = null)
        where C : ISeraColctor<Memory<char>, R>
        => colctor.CString(new IdentityMapper<Memory<char>>(), new Type<Memory<char>>());
}

public readonly struct StringEncodedImpl(Encoding encoding) :
    ISeraColion<byte[]>,
    ISeraColion<ReadOnlyMemory<byte>>, ISeraColion<Memory<byte>>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Collect<R, C>(ref C colctor, InType<byte[]>? t = null)
        where C : ISeraColctor<byte[], R>
        => colctor.CString(new IdentityMapper<byte[]>(), new Type<byte[]>(), encoding);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Collect<R, C>(ref C colctor, InType<ReadOnlyMemory<byte>>? t = null)
        where C : ISeraColctor<ReadOnlyMemory<byte>, R>
        => colctor.CString(new IdentityMapper<ReadOnlyMemory<byte>>(), new Type<ReadOnlyMemory<byte>>(), encoding);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Collect<R, C>(ref C colctor, InType<Memory<byte>>? t = null)
        where C : ISeraColctor<Memory<byte>, R>
        => colctor.CString(new IdentityMapper<Memory<byte>>(), new Type<Memory<byte>>(), encoding);
}
