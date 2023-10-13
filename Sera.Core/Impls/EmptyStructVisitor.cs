using System.Threading.Tasks;
using Sera.Core.De;

namespace Sera.Core.Impls;

public readonly struct EmptyStructVisitor<R>(R v) : IStructDeserializerVisitor<R>, IAsyncStructDeserializerVisitor<R>
{
    public R VisitStructSeq<A>(A access) where A : IStructSeqAccess
        => v;

    public R VisitStructMap<A>(A access) where A : IStructMapAccess
        => v;

    public ValueTask<R> VisitStructSeqAsync<A>(A access) where A : IAsyncStructSeqAccess
        => ValueTask.FromResult(v);

    public ValueTask<R> VisitStructMapAsync<A>(A access) where A : IAsyncStructMapAccess
        => ValueTask.FromResult(v);
}
