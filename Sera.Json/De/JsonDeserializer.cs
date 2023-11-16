using System;
using System.Runtime.CompilerServices;
using Sera.Core;
using Sera.Utils;

namespace Sera.Json.De;

public class JsonDeserializer(SeraJsonOptions options, AJsonReader reader) : SeraBase<ISeraColion<object?>>
{
    public override string FormatName => "json";
    public override string FormatMIME => "application/json";
    public override SeraFormatType FormatType => SeraFormatType.HumanReadableText;
    public override ISeraOptions Options => options;
    internal readonly AJsonReader reader = reader;

    public override IRuntimeProvider<ISeraColion<object?>> RuntimeProvider =>
        RuntimeProviderOverride ?? throw new NotImplementedException("todo"); // EmptyDeRuntimeProvider.Instance

    public IRuntimeProvider<ISeraColion<object?>>? RuntimeProviderOverride { get; set; }

    public T Collect<C, T>(C colion) where C : ISeraColion<T>
    {
        var c = new JsonDeserializer<T>(this);
        return colion.Collect<T, JsonDeserializer<T>>(ref c);
    }
}

public readonly struct JsonDeserializer<T>(JsonDeserializer impl) : ISeraColctor<T, T>
{
    [AssocType]
    public abstract class R(T type);

    public string FormatName => impl.FormatName;
    public string FormatMIME => impl.FormatMIME;
    public SeraFormatType FormatType => impl.FormatType;
    public ISeraOptions Options => impl.Options;
    private readonly AJsonReader reader = impl.reader;

    public ISeraColion<object?> RuntimeImpl => impl.RuntimeImpl;

    #region Primitive

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T CPrimitive<F>(F functor, Type<bool> t, SeraFormats? formats = null)
        where F : ISeraFunctor<bool, T>
    {
        T r;
        var token = reader.CurrentToken();
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T CTuple<C, B, F>(C colion, F functor, Type<B> b)
        where C : ITupleSeraColion<B> where F : ISeraFunctor<B, T>
    {
        reader.ReadArrayStart();
        var size = colion.Size;
        var colctor = new TupleSeraColctor<B>(colion.Builder(), impl);
        var first = true;
        for (var i = 0; i < size; i++)
        {
            if (first) first = false;
            else reader.ReadComma();
            var err = colion.CollectItem<bool, TupleSeraColctor<B>>(ref colctor, i);
            if (err) throw new SerializeException($"Unable to write item {i} of tuple {typeof(T)}");
        }
        reader.ReadArrayEnd();
        return functor.Map(colctor.builder);
    }

    private struct TupleSeraColctor<B>(B builder, JsonDeserializer impl) : ITupleSeraColctor<B, bool>
    {
        [AssocType("R")]
        public abstract class _R(bool type);

        public B builder = builder;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool CItem<C, E, I>(C colion, E effector, Type<I> i)
            where C : ISeraColion<I> where E : ISeraEffector<B, I>
        {
            var c = new JsonDeserializer<I>(impl);
            var r = colion.Collect<I, JsonDeserializer<I>>(ref c);
            effector.Effect(ref builder, r);
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool CNone() => false;
    }

    #endregion

    #region Seq

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T CSeq<C, B, F>(C colion, F functor, Type<B> b) where C : ISeqSeraColion<B> where F : ISeraFunctor<B, T>
    {
        reader.ReadArrayStart();
        var colctor = new SeqSeraColctor<B>(colion.Builder(null), impl);
        var first = true;
        for (; reader.Has(); reader.MoveNext())
        {
            var token = reader.CurrentToken();
            if (token.Kind is JsonTokenKind.ArrayEnd) break;
            if (first) first = false;
            else if (token.Kind is not JsonTokenKind.Comma) throw new NotImplementedException(); // todo error
            colion.CollectItem<Unit, SeqSeraColctor<B>>(ref colctor);
        }
        reader.ReadArrayEnd();
        return functor.Map(colctor.builder);
    }
    
    private struct SeqSeraColctor<B>(B builder, JsonDeserializer impl) : ISeqSeraColctor<B, Unit>
    {
        [AssocType("R")]
        public abstract class _R(Unit type);

        public B builder = builder;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Unit CItem<C, E, I>(C colion, E effector, Type<I> i)
            where C : ISeraColion<I> where E : ISeraEffector<B, I>
        {
            var c = new JsonDeserializer<I>(impl);
            var r = colion.Collect<I, JsonDeserializer<I>>(ref c);
            effector.Effect(ref builder, r);
            return default;
        }
    }

    #endregion
}
