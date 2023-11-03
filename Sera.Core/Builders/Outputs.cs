using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Sera.Core.Builders.Outputs;

public interface IStreamOutput
{
    public R BuildStreamOutput<R, A>(A accept, OutputBuildParam param, Stream stream)
        where A : AcceptASeraVisitor<Unit, R>;
}

public interface IStringBuilderOutput
{
    public R BuildStringBuilderOutput<R, A>(A accept, OutputBuildParam param,
        StringBuilder stringBuilder) where A : AcceptASeraVisitor<Unit, R>;
}

public interface IAsyncStreamOutput
{
    public R BuildAsyncStreamOutput<R, A>(A accept, OutputBuildParam param, Stream stream)
        where A : AcceptASeraVisitor<ValueTask, R>;
}

public readonly record struct OutputBuildParam(
    IRuntimeProvider<ISeraVision<object?>>? RuntimeProvider
);

public interface AcceptASeraVisitor<R, out RR>
{
    public RR Accept<V>(V visitor) where V : ASeraVisitor<R>;
}
