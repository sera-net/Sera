using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Sera.Core;
using Sera.Core.Builders;
using Sera.Core.Builders.De;
using Sera.Core.Builders.Inputs;
using Sera.Core.Builders.Outputs;
using Sera.Core.Builders.Ser;
using Sera.Json.Builders;
using Sera.Json.De;
using Sera.Json.Ser;
using Sera.Utils;

namespace Sera.Json
{

    public static class SeraJson
    {
        public static SerBuilder<SerializerBuilder> Serializer
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new(new(SeraJsonOptions.Default, CompactJsonFormatter.Default));
        }
        public static DeBuilder<DeserializerBuilder> Deserializer
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new(new(SeraJsonOptions.Default));
        }
    }

    public static class SerializerBuilderEx
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SerBuilder<SerializerBuilder> WithOptions(this SerBuilder<SerializerBuilder> self,
            SeraJsonOptions options)
            => new(self.Target.WithOptions(options));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SerBuilder<SerializerBuilder> WithFormatter(this SerBuilder<SerializerBuilder> self,
            AJsonFormatter formatter)
            => new(self.Target.WithFormatter(formatter));


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DeBuilder<DeserializerBuilder> WithOptions(this DeBuilder<DeserializerBuilder> self,
            SeraJsonOptions options)
            => new(self.Target.WithOptions(options));
    }

}

namespace Sera.Json.Builders
{

    public readonly record struct SerializerBuilder(SeraJsonOptions Options, AJsonFormatter Formatter) :
        ISerBuilder, IStreamOutput, IStringBuilderOutput, IAsyncStreamOutput
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SerializerBuilder WithOptions(SeraJsonOptions options) =>
            this with { Options = options };

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SerializerBuilder WithFormatter(AJsonFormatter formatter) =>
            this with { Formatter = formatter };

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public R BuildStreamOutput<R, A>(A accept, OutputBuildParam param, Stream stream)
            where A : AcceptASeraVisitor<Unit, R>
            => accept.Accept(new JsonSerializer(new StreamJsonWriter(Options, Formatter, stream))
                { RuntimeProviderOverride = param.RuntimeProvider });

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public R BuildStringBuilderOutput<R, A>(A accept, OutputBuildParam param,
            StringBuilder stringBuilder)
            where A : AcceptASeraVisitor<Unit, R>
            => accept.Accept(new JsonSerializer(new StringBuilderJsonWriter(Options, Formatter, stringBuilder))
                { RuntimeProviderOverride = param.RuntimeProvider });

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public R BuildAsyncStreamOutput<R, A>(A accept, OutputBuildParam param, Stream stream)
            where A : AcceptASeraVisitor<ValueTask, R>
            => accept.Accept(new AsyncJsonSerializer(new AsyncStreamJsonWriter(Options, Formatter, stream))
                { RuntimeProviderOverride = param.RuntimeProvider });
    }

    public readonly record struct DeserializerBuilder(SeraJsonOptions Options) :
        IDeBuilder, IStringInput, IStreamInput, IAsyncStreamInput
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public DeserializerBuilder WithOptions(SeraJsonOptions options) =>
            this with { Options = options };

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public R BuildStringInput<T, R, A>(A accept, InputBuildParam param, CompoundString str)
            where A : AcceptASeraColctor<T, T, R>
            => accept.Accept(new JsonDeserializer(StringJsonReader.Create(Options, str))
                { RuntimeProviderOverride = param.RuntimeProvider }.MakeColctor<T>());

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public R BuildStreamInput<T, R, A>(A accept, InputBuildParam param, Stream stream)
            where A : AcceptASeraColctor<T, T, R>
            => accept.Accept(new JsonDeserializer(StreamJsonReader.Create(Options, stream))
                { RuntimeProviderOverride = param.RuntimeProvider }.MakeColctor<T>());

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public async ValueTask<R> BuildAsyncStreamInput<T, R, A>(A accept, InputBuildParam param, Stream stream)
            where A : AcceptASeraColctor<T, ValueTask<T>, ValueTask<R>>
            => await accept.Accept(new AsyncJsonDeserializer(await AsyncStreamJsonReader.Create(Options, stream))
                { RuntimeProviderOverride = param.RuntimeProvider }.MakeColctor<T>());
    }

}
