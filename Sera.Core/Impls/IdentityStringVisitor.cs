using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sera.Core.De;

namespace Sera.Core.Impls;

public readonly struct IdentityStringVisitor
    : IStringDeserializerVisitor<string>, IAsyncStringDeserializerVisitor<string>
{
    public string VisitString<A>(A access) where A : IStringAccess
        => access.ReadString();

    public ValueTask<string> VisitStringAsync<A>(A access) where A : IAsyncStringAccess
        => access.ReadStringAsync();
}

public readonly struct IdentityStringArrayVisitor
    : IStringDeserializerVisitor<char[]>, IAsyncStringDeserializerVisitor<char[]>
{
    public char[] VisitString<A>(A access) where A : IStringAccess
        => access.ReadStringAsArray();

    public ValueTask<char[]> VisitStringAsync<A>(A access) where A : IAsyncStringAccess
        => access.ReadStringAsArrayAsync();
}

public readonly struct IdentityStringListVisitor
    : IStringDeserializerVisitor<List<char>>, IAsyncStringDeserializerVisitor<List<char>>
{
    public List<char> VisitString<A>(A access) where A : IStringAccess
        => access.ReadStringAsList();

    public ValueTask<List<char>> VisitStringAsync<A>(A access) where A : IAsyncStringAccess
        => access.ReadStringAsListAsync();
}

public readonly struct IdentityStringReadOnlyMemoryVisitor
    : IStringDeserializerVisitor<ReadOnlyMemory<char>>, IAsyncStringDeserializerVisitor<ReadOnlyMemory<char>>
{
    public ReadOnlyMemory<char> VisitString<A>(A access) where A : IStringAccess
        => access.ReadStringAsReadOnlyMemory();

    public ValueTask<ReadOnlyMemory<char>> VisitStringAsync<A>(A access) where A : IAsyncStringAccess
        => access.ReadStringAsReadOnlyMemoryAsync();
}

public readonly struct IdentityStringMemoryVisitor
    : IStringDeserializerVisitor<Memory<char>>, IAsyncStringDeserializerVisitor<Memory<char>>
{
    public Memory<char> VisitString<A>(A access) where A : IStringAccess
        => access.ReadStringAsMemory();

    public ValueTask<Memory<char>> VisitStringAsync<A>(A access) where A : IAsyncStringAccess
        => access.ReadStringAsMemoryAsync();
}
