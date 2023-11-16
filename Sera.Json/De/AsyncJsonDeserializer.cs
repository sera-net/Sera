using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Sera.Core;
using Sera.Utils;

namespace Sera.Json.De;

public class AsyncJsonDeserializer(SeraJsonOptions options, AAsyncJsonReader reader) : SeraBase<ISeraColion<object?>>
{
    public override string FormatName => "json";
    public override string FormatMIME => "application/json";
    public override SeraFormatType FormatType => SeraFormatType.HumanReadableText;
    public override ISeraOptions Options => options;
    internal readonly AAsyncJsonReader reader = reader;

    public override IRuntimeProvider<ISeraColion<object?>> RuntimeProvider =>
        RuntimeProviderOverride ?? throw new NotImplementedException("todo"); // EmptyDeRuntimeProvider.Instance

    public IRuntimeProvider<ISeraColion<object?>>? RuntimeProviderOverride { get; set; }

    public ValueTask<T> Collect<C, T>(C colion) where C : ISeraColion<T>
    {
        var c = new AsyncJsonDeserializer<T>(this);
        return colion.Collect<ValueTask<T>, AsyncJsonDeserializer<T>>(ref c);
    }
}

public readonly struct AsyncJsonDeserializer<T>(AsyncJsonDeserializer impl) : ISeraColctor<T, ValueTask<T>>
{
    [AssocType]
    public abstract class R(T type);

    public string FormatName => impl.FormatName;
    public string FormatMIME => impl.FormatMIME;
    public SeraFormatType FormatType => impl.FormatType;
    public ISeraOptions Options => impl.Options;
    private readonly AAsyncJsonReader reader = impl.reader;

    public ISeraColion<object?> RuntimeImpl => impl.RuntimeImpl;

    #region Primitive

    public async ValueTask<T> CPrimitive<F>(F functor, Type<bool> t, SeraFormats? formats = null)
        where F : ISeraFunctor<bool, T>
    {
        T r;
        var token = await reader.CurrentToken();
        if (token.Kind is JsonTokenKind.True)
        {
            r = functor.Map(true);
        }
        else if (token.Kind is JsonTokenKind.False)
        {
            r = functor.Map(false);
        }
        else throw new NotImplementedException(); // todo
        return r;
    }

    #endregion

    #region tuple

    public async ValueTask<T> CTuple<C, B, F>(C colion, F functor, Type<B> b)
        where C : ITupleSeraColion<B> where F : ISeraFunctor<B, T>
    {
        await reader.ReadArrayStart();
        var size = colion.Size;
        var colctor = new TupleSeraColctor<B>(new(colion.Builder()), impl);
        var first = true;
        for (var i = 0; i < size; i++)
        {
            if (first) first = false;
            else await reader.ReadComma();
            var err = await colion.CollectItem<ValueTask<bool>, TupleSeraColctor<B>>(ref colctor, i);
            if (err) throw new SerializeException($"Unable to write item {i} of tuple {typeof(T)}");
        }
        await reader.ReadArrayEnd();
        return functor.Map(colctor.builder.Value);
    }

    private readonly struct TupleSeraColctor<B>(Box<B> builder, AsyncJsonDeserializer impl)
        : ITupleSeraColctor<B, ValueTask<bool>>
    {
        public readonly Box<B> builder = builder;

        [AssocType("R")]
        public abstract class _R(bool type);

        public async ValueTask<bool> CItem<C, E, I>(C colion, E effector, Type<I> i)
            where C : ISeraColion<I> where E : ISeraEffector<B, I>
        {
            var c = new AsyncJsonDeserializer<I>(impl);
            var r = await colion.Collect<ValueTask<I>, AsyncJsonDeserializer<I>>(ref c);
            effector.Effect(ref builder.Value, r);
            return true;
        }

        public ValueTask<bool> CNone() => ValueTask.FromResult(false);
    }

    #endregion

    #region Seq

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public async ValueTask<T> CSeq<C, B, F>(C colion, F functor, Type<B> b)
        where C : ISeqSeraColion<B> where F : ISeraFunctor<B, T>
    {
        await reader.ReadArrayStart();
        var colctor = new SeqSeraColctor<B>(new(colion.Builder(null)), impl);
        var first = true;
        for (; await reader.Has(); await reader.MoveNext())
        {
            var token = await reader.CurrentToken();
            if (token.Kind is JsonTokenKind.ArrayEnd) break;
            if (first) first = false;
            else if (token.Kind is not JsonTokenKind.Comma) throw new NotImplementedException(); // todo error
            await colion.CollectItem<ValueTask, SeqSeraColctor<B>>(ref colctor);
        }
        await reader.ReadArrayEnd();
        return functor.Map(colctor.builder.Value);
    }

    private readonly struct SeqSeraColctor<B>(Box<B> builder, AsyncJsonDeserializer impl)
        : ISeqSeraColctor<B, ValueTask>
    {
        [AssocType("R")]
        public abstract class _R(Unit type);

        public readonly Box<B> builder = builder;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public async ValueTask CItem<C, E, I>(C colion, E effector, Type<I> i)
            where C : ISeraColion<I> where E : ISeraEffector<B, I>
        {
            var c = new AsyncJsonDeserializer<I>(impl);
            var r = await colion.Collect<ValueTask<I>, AsyncJsonDeserializer<I>>(ref c);
            effector.Effect(ref builder.Value, r);
        }
    }

    #endregion
}
