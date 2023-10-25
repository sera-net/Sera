using System.Threading.Tasks;
using Sera.Core.De;

namespace Sera.Core.Impls;

public readonly struct EmptyStructVisitor<R>(R v) : IStructDeserializerVisitor<R>
{
    public R VisitStructSeq<A>(A access) where A : IStructSeqAccess
        => v;

    public R VisitStructMap<A>(A access) where A : IStructMapAccess
        => v;
}
