using System.Threading.Tasks;
using Sera.Core.De;

namespace Sera.Core.Impls;

public readonly struct IdentityStringVisitor : IStringDeserializerVisitor<string>, IAsyncStringDeserializerVisitor<string>
{
    public string VisitString<A>(A access) where A : IStringAccess
        => access.ReadString();

    public ValueTask<string> VisitStringAsync<A>(A access) where A : IAsyncStringAccess
        => access.ReadStringAsync();
}

public readonly struct IdentityStringCharArrayVisitor : IStringDeserializerVisitor<char[]>, IAsyncStringDeserializerVisitor<char[]>
{
    public char[] VisitString<A>(A access) where A : IStringAccess
        => access.ReadStringAsCharArray();

    public ValueTask<char[]> VisitStringAsync<A>(A access) where A : IAsyncStringAccess
        => access.ReadStringAsCharArrayAsync();
}
