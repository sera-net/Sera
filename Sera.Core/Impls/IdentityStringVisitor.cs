using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sera.Core.De;

namespace Sera.Core.Impls;

public readonly struct IdentityStringVisitor
    : IStringDeserializerVisitor<string>
{
    public string VisitString<A>(A access) where A : IStringAccess
        => access.ReadString();
}

public readonly struct IdentityStringArrayVisitor
    : IStringDeserializerVisitor<char[]>
{
    public char[] VisitString<A>(A access) where A : IStringAccess
        => access.ReadStringAsArray();
}

public readonly struct IdentityStringListVisitor
    : IStringDeserializerVisitor<List<char>>
{
    public List<char> VisitString<A>(A access) where A : IStringAccess
        => access.ReadStringAsList();
}

public readonly struct IdentityStringReadOnlyMemoryVisitor
    : IStringDeserializerVisitor<ReadOnlyMemory<char>>
{
    public ReadOnlyMemory<char> VisitString<A>(A access) where A : IStringAccess
        => access.ReadStringAsReadOnlyMemory();
}

public readonly struct IdentityStringMemoryVisitor
    : IStringDeserializerVisitor<Memory<char>>
{
    public Memory<char> VisitString<A>(A access) where A : IStringAccess
        => access.ReadStringAsMemory();
}
