using System.Threading.Tasks;
using Sera.Core.De;

namespace Sera.Core.Impls;

public struct IdentityUnitOrNullVisitor<R> :
    IUnitDeserializerVisitor<R>, IAsyncUnitDeserializerVisitor<R>
{
    public R VisitUnit() => default!;

    public ValueTask<R> VisitUnitAsync() => ValueTask.FromResult<R>(default!);

    public R VisitNull() => default!;

    public ValueTask<R> VisitNullAsync() => ValueTask.FromResult<R>(default!);
}
