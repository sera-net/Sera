using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Sera.Core;
using Sera.Core.Builders;
using Sera.Core.Builders.Outputs;
using Sera.Core.Builders.Ser;
using Sera.Runtime;
using Sera.Runtime.Emit.Ser;

namespace Sera;

public static class SeraBuilderForRuntimeSer
{
    #region Accept

    public readonly struct RuntimeAccept<T>(T Value, SeraStyles? styles) : AcceptASeraVisitor<Unit, Unit>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Unit Accept<VI>(VI visitor) where VI : ASeraVisitor<Unit>
        {
            EmitSerImpls.Get<T>(styles).Accept<Unit, VI>(visitor, Value);
            visitor.Flush();
            return default;
        }
    }

    public readonly struct AsyncRuntimeAccept<T>(T Value, SeraStyles? styles) : AcceptASeraVisitor<ValueTask, ValueTask>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public async ValueTask Accept<VI>(VI visitor) where VI : ASeraVisitor<ValueTask>
        {
            await EmitSerImpls.Get<T>(styles).Accept<ValueTask, VI>(visitor, Value);
            await visitor.Flush();
        }
    }

    #endregion

    #region OutputBuildParam

    public static OutputBuildParam RuntimeOutputBuildParam =>
        new(RuntimeProvider: EmitRuntimeProvider.Instance);

    #endregion

    #region Static

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Stream<B, T, V>(this To<SerBuilder<B, T, V>> self, Stream stream)
        where B : ISerBuilder, IStreamOutput where V : ISeraVision<T>
    {
        var (Target, Value, Vision) = self.Target;
        Target.BuildStreamOutput<Unit, SeraBuilderForStaticSer.StaticAccept<T, V>>(
            new SeraBuilderForStaticSer.StaticAccept<T, V>(Value, Vision), RuntimeOutputBuildParam, stream);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void String<B, T, V>(this To<SerBuilder<B, T, V>> self, StringBuilder stringBuilder)
        where B : ISerBuilder, IStringBuilderOutput where V : ISeraVision<T>
    {
        var (Target, Value, Vision) = self.Target;
        Target.BuildStringBuilderOutput<Unit, SeraBuilderForStaticSer.StaticAccept<T, V>>(
            new SeraBuilderForStaticSer.StaticAccept<T, V>(Value, Vision), RuntimeOutputBuildParam, stringBuilder);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string String<B, T, V>(this To<SerBuilder<B, T, V>> self)
        where B : ISerBuilder, IStringBuilderOutput where V : ISeraVision<T>
    {
        var sb = new StringBuilder();
        self.String(sb);
        return sb.ToString();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueTask StreamAsync<B, T, V>(this To<SerBuilder<B, T, V>> self, Stream stream)
        where B : ISerBuilder, IAsyncStreamOutput where V : ISeraVision<T>
    {
        var (Target, Value, Vision) = self.Target;
        return Target.BuildAsyncStreamOutput<ValueTask, SeraBuilderForStaticSer.AsyncStaticAccept<T, V>>(
            new SeraBuilderForStaticSer.AsyncStaticAccept<T, V>(Value, Vision), RuntimeOutputBuildParam, stream);
    }

    #endregion

    #region Runtime

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Stream<B, T>(this To<SerBuilder<B, T>> self, Stream stream, SeraStyles? styles = default)
        where B : ISerBuilder, IStreamOutput
    {
        var (Target, Value) = self.Target;
        Target.BuildStreamOutput<Unit, RuntimeAccept<T>>(
            new RuntimeAccept<T>(Value, styles), RuntimeOutputBuildParam, stream);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void String<B, T>(this To<SerBuilder<B, T>> self, StringBuilder stringBuilder,
        SeraStyles? styles = default)
        where B : ISerBuilder, IStringBuilderOutput
    {
        var (Target, Value) = self.Target;
        Target.BuildStringBuilderOutput<Unit, RuntimeAccept<T>>(
            new RuntimeAccept<T>(Value, styles), RuntimeOutputBuildParam, stringBuilder);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string String<B, T>(this To<SerBuilder<B, T>> self, SeraStyles? styles = default)
        where B : ISerBuilder, IStringBuilderOutput
    {
        var sb = new StringBuilder();
        self.String(sb, styles);
        return sb.ToString();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueTask StreamAsync<B, T>(this To<SerBuilder<B, T>> self, Stream stream,
        SeraStyles? styles = default)
        where B : ISerBuilder, IAsyncStreamOutput
    {
        var (Target, Value) = self.Target;
        return Target.BuildAsyncStreamOutput<ValueTask, AsyncRuntimeAccept<T>>(
            new AsyncRuntimeAccept<T>(Value, styles), RuntimeOutputBuildParam, stream);
    }

    #endregion
}
