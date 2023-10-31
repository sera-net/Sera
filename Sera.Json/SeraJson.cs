using System.IO;
using System.Text;
using System.Threading.Tasks;
using Sera.Core;
using Sera.Core.Builders;
using Sera.Core.Builders.Outputs;
using Sera.Json.Builders;
using Sera.Json.Ser;

namespace Sera.Json
{

    public static class SeraJson
    {
        public static SerializerBuilder Serializer => new(SeraJsonOptions.Default, CompactJsonFormatter.Default);
    }

}

namespace Sera.Json.Builders
{

    public readonly record struct SerializerBuilder(SeraJsonOptions Options, AJsonFormatter Formatter) :
        ISerBuilder, IStreamOutput, IStringBuilderOutput, IAsyncStreamOutput
    {
        public SerializerBuilder WithOptions(SeraJsonOptions options) =>
            this with { Options = options };

        public SerializerBuilder WithFormatter(AJsonFormatter formatter) =>
            this with { Formatter = formatter };

        public R BuildStreamOutput<R, A>(A accept, OutputBuildParam param, Stream stream)
            where A : AcceptASeraVisitor<Unit, R>
            => accept.Accept(new JsonSerializer(new StreamJsonWriter(Options, Formatter, stream))
                { RuntimeProviderOverride = param.RuntimeProvider });

        public R BuildStringBuilderOutput<R, A>(A accept, OutputBuildParam param,
            StringBuilder stringBuilder)
            where A : AcceptASeraVisitor<Unit, R>
            => accept.Accept(new JsonSerializer(new StringBuilderJsonWriter(Options, Formatter, stringBuilder))
                { RuntimeProviderOverride = param.RuntimeProvider });

        public R BuildAsyncStreamOutput<R, A>(A accept, OutputBuildParam param, Stream stream)
            where A : AcceptASeraVisitor<ValueTask, R>
            => accept.Accept(new AsyncJsonSerializer(new AsyncStreamJsonWriter(Options, Formatter, stream))
                { RuntimeProviderOverride = param.RuntimeProvider });
    }

}
