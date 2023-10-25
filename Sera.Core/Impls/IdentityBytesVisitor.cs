using System;
using System.Buffers;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Sera.Core.De;

namespace Sera.Core.Impls;

public struct IdentityBytesVisitor : IBytesDeserializerVisitor<byte[]>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public byte[] VisitBytes<A>(A access) where A : IBytesAccess
        => access.ReadBytes();
}

public struct IdentityBytesListVisitor :
    IBytesDeserializerVisitor<List<byte>>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public List<byte> VisitBytes<A>(A access) where A : IBytesAccess
        => access.ReadBytesAsList();
}

public struct IdentityBytesMemoryVisitor :
    IBytesDeserializerVisitor<Memory<byte>>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Memory<byte> VisitBytes<A>(A access) where A : IBytesAccess
        => access.ReadBytesAsMutableMemory();
}

public struct IdentityBytesReadOnlyMemoryVisitor :
    IBytesDeserializerVisitor<ReadOnlyMemory<byte>>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlyMemory<byte> VisitBytes<A>(A access) where A : IBytesAccess
        => access.ReadBytesAsMemory();
}

public struct IdentityBytesReadOnlySequenceVisitor :
    IBytesDeserializerVisitor<ReadOnlySequence<byte>>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlySequence<byte> VisitBytes<A>(A access) where A : IBytesAccess
        => access.ReadBytesAsSequence();
}
