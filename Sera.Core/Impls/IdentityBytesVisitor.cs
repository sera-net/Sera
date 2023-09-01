using System.Threading.Tasks;
using Sera.Core.De;

namespace Sera.Core.Impls;

public struct IdentityBytesVisitor : IBytesDeserializerVisitor<byte[]>, IAsyncBytesDeserializerVisitor<byte[]>
{
    public byte[] VisitBytes<A>(A access) where A : IBytesAccess
        => access.ReadBytes();

    public ValueTask<byte[]> VisitBytesAsync<A>(A access) where A : IAsyncBytesAccess
        => access.ReadBytesAsync();
}
