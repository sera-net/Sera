using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Sera.Core;
using Sera.Core.Builders;
using Sera.Core.Builders.Outputs;
using Sera.Core.Builders.Ser;

namespace Sera;

public static class SeraBuilderForSer
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SerBuilder<B> Make<B>(this B builder)
        where B : ISerBuilder => new(builder);
}

public static class SeraBuilderForStaticSer
{
    #region Accept

    public readonly struct StaticAccept<T, V>(T Value, V Vision) : AcceptASeraVisitor<Unit, Unit>
        where V : ISeraVision<T>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Unit Accept<VI>(VI visitor) where VI : ASeraVisitor<Unit>
        {
            Vision.Accept<Unit, VI>(visitor, Value);
            visitor.Flush();
            return default;
        }
    }

    public readonly struct AsyncStaticAccept<T, V>(T Value, V Vision) : AcceptASeraVisitor<ValueTask, ValueTask>
        where V : ISeraVision<T>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public async ValueTask Accept<VI>(VI visitor) where VI : ASeraVisitor<ValueTask>
        {
            await Vision.Accept<ValueTask, VI>(visitor, Value);
            await visitor.Flush();
        }
    }

    #endregion

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Stream<B, T, V>(this To<Static<SerBuilder<B, T, V>>> self, Stream stream)
        where B : ISerBuilder, IStreamOutput where V : ISeraVision<T>
    {
        var (Target, Value, Vision) = self.Target.Target;
        Target.BuildStreamOutput<Unit, StaticAccept<T, V>>(
            new StaticAccept<T, V>(Value, Vision), default, stream);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void String<B, T, V>(this To<Static<SerBuilder<B, T, V>>> self, StringBuilder stringBuilder)
        where B : ISerBuilder, IStringBuilderOutput where V : ISeraVision<T>
    {
        var (Target, Value, Vision) = self.Target.Target;
        Target.BuildStringBuilderOutput<Unit, StaticAccept<T, V>>(
            new StaticAccept<T, V>(Value, Vision), default, stringBuilder);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string String<B, T, V>(this To<Static<SerBuilder<B, T, V>>> self)
        where B : ISerBuilder, IStringBuilderOutput where V : ISeraVision<T>
    {
        var sb = new StringBuilder();
        self.String(sb);
        return sb.ToString();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueTask StreamAsync<B, T, V>(this To<Static<SerBuilder<B, T, V>>> self, Stream stream)
        where B : ISerBuilder, IAsyncStreamOutput where V : ISeraVision<T>
    {
        var (Target, Value, Vision) = self.Target.Target;
        return Target.BuildAsyncStreamOutput<ValueTask, AsyncStaticAccept<T, V>>(
            new AsyncStaticAccept<T, V>(Value, Vision), default, stream);
    }
}
