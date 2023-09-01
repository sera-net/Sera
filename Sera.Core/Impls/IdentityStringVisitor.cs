using System.Threading.Tasks;
using Sera.Core.De;

namespace Sera.Core.Impls;

public struct IdentityStringVisitor : IStringDeserializerVisitor<string>, IAsyncStringDeserializerVisitor<string>
{
    public string VisitString<A>(A access) where A : IStringAccess
        => access.ReadString();

    public ValueTask<string> VisitStringAsync<A>(A access) where A : IAsyncStringAccess
        => access.ReadStringAsync();
}
