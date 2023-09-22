using System;
using System.Buffers;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Sera.Core.De;

namespace Sera.Core.Impls;

public struct IdentityBytesVisitor : IBytesDeserializerVisitor<byte[]>, IAsyncBytesDeserializerVisitor<byte[]>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public byte[] VisitBytes<A>(A access) where A : IBytesAccess
        => access.ReadBytes();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ValueTask<byte[]> VisitBytesAsync<A>(A access) where A : IAsyncBytesAccess
        => access.ReadBytesAsync();
}

public struct IdentityBytesListVisitor :
    IBytesDeserializerVisitor<List<byte>>,
    IAsyncBytesDeserializerVisitor<List<byte>>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public List<byte> VisitBytes<A>(A access) where A : IBytesAccess
        => access.ReadBytesAsList();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ValueTask<List<byte>> VisitBytesAsync<A>(A access) where A : IAsyncBytesAccess
        => access.ReadBytesAsListAsync();
}

public struct IdentityBytesMemoryVisitor :
    IBytesDeserializerVisitor<Memory<byte>>,
    IAsyncBytesDeserializerVisitor<Memory<byte>>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Memory<byte> VisitBytes<A>(A access) where A : IBytesAccess
        => access.ReadBytesAsMutableMemory();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ValueTask<Memory<byte>> VisitBytesAsync<A>(A access) where A : IAsyncBytesAccess
        => access.ReadBytesAsMutableMemoryAsync();
}

public struct IdentityBytesReadOnlyMemoryVisitor :
    IBytesDeserializerVisitor<ReadOnlyMemory<byte>>,
    IAsyncBytesDeserializerVisitor<ReadOnlyMemory<byte>>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlyMemory<byte> VisitBytes<A>(A access) where A : IBytesAccess
        => access.ReadBytesAsMemory();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ValueTask<ReadOnlyMemory<byte>> VisitBytesAsync<A>(A access) where A : IAsyncBytesAccess
        => access.ReadBytesAsMemoryAsync();
}

public struct IdentityBytesReadOnlySequenceVisitor :
    IBytesDeserializerVisitor<ReadOnlySequence<byte>>,
    IAsyncBytesDeserializerVisitor<ReadOnlySequence<byte>>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlySequence<byte> VisitBytes<A>(A access) where A : IBytesAccess
        => access.ReadBytesAsSequence();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ValueTask<ReadOnlySequence<byte>> VisitBytesAsync<A>(A access) where A : IAsyncBytesAccess
        => access.ReadBytesAsSequenceAsync();
}
