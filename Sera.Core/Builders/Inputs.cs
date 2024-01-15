using System.IO;
using System.Threading.Tasks;
using Sera.Utils;

namespace Sera.Core.Builders.Inputs;

public interface IStringInput
{
    public R BuildStringInput<T, R, A>(A accept, InputBuildParam param, CompoundString str)
        where A : AcceptASeraColctor<T, T, R>;
}

public interface IStreamInput
{
    public R BuildStreamInput<T, R, A>(A accept, InputBuildParam param, Stream stream)
        where A : AcceptASeraColctor<T, T, R>;
}

public interface IAsyncStreamInput
{
    public ValueTask<R> BuildAsyncStreamInput<T, R, A>(A accept, InputBuildParam param, Stream stream)
        where A : AcceptASeraColctor<T, ValueTask<T>, ValueTask<R>>;
}

public readonly record struct InputBuildParam(
    IRuntimeProvider<ISeraColion<object?>>? RuntimeProvider
);

public interface AcceptASeraColctor<out T, in R, out RR>
{
    public RR Accept<C>(C colctor) where C : ISeraColctor<T, R>;
}
