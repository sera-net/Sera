using System.Threading.Tasks;
using Sera.Core.De;

namespace Sera.Core.Impls;

public class IdentityUnitOrNullVisitor<R> :
    IUnitDeserializerVisitor<R>, IAsyncUnitDeserializerVisitor<R>
{
    public R VisitUnit() => default!;

    public ValueTask<R> VisitUnitAsync() => ValueTask.FromResult<R>(default!);

    public R VisitNull() => default!;

    public ValueTask<R> VisitNullAsync() => ValueTask.FromResult<R>(default!);
}
