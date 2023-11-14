using System;
using System.Runtime.CompilerServices;
using Sera.Core;
using Sera.Utils;

namespace Sera.Json.De;

public class JsonDeserializer(SeraJsonOptions options, AJsonReader reader) : ASeraColctor<Unit>
{
    public override string FormatName => "json";
    public override string FormatMIME => "application/json";
    public override SeraFormatType FormatType => SeraFormatType.HumanReadableText;
    public override ISeraOptions Options => options;
    private readonly AJsonReader reader = reader;

    public override IRuntimeProvider<ISeraColion<ISeraAsmer<object?>>> RuntimeProvider =>
        RuntimeProviderOverride ?? throw new NotImplementedException("todo"); // EmptyDeRuntimeProvider.Instance

    public IRuntimeProvider<ISeraColion<ISeraAsmer<object?>>>? RuntimeProviderOverride { get; set; }

    public T Enter<C, B, A, T>(C colion, B asmable)
        where C : ISeraColion<A>
        where B : ISeraAsmable<A>
        where A : ISeraAsmer<T>
    {
        var asmer = new Box<A>(asmable.Asmer());
        colion.Collect<Unit, JsonDeserializer, Box<A>>(this, asmer);
        return asmer.Value.Asm();
    }

    #region Primitive

    public override Unit CPrimitive<A, B>(B asmer, Type<A> a, Type<bool> t, SeraFormats? formats = null)
    {
        var token = reader.CurrentToken();
        if (token.Kind is JsonTokenKind.True)
        {
            asmer.GetRef().Provide(true);
        }
        else if (token.Kind is JsonTokenKind.False)
        {
            asmer.GetRef().Provide(false);
        }
        else throw new NotImplementedException(); // todo
        return default;
    }

    #endregion

    #region tuple

    public override Unit CTuple<C, A, B, T>(C colion, B asmer, Type<A> a, Type<T> t)
    {
        var size = colion.Size;
        var first = true;
        reader.ReadArrayStart();
        var tuple_colctor = new TupleSeraColctor(this);
        for (var i = 0; i < size; i++)
        {
            if (first) first = false;
            else reader.ReadComma();
            var err = colion.CollectItem<bool, TupleSeraColctor, B>(ref tuple_colctor, asmer, i);
            if (err) throw new SerializeException($"Unable to write item {i} of tuple {typeof(T)}");
        }
        reader.ReadArrayEnd();
        return default;
    }

    private readonly struct TupleSeraColctor(JsonDeserializer Base) : ITupleSeraColctor<bool>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool CItem<C, A, B, I>(C colion, B asmer, Type<A> a, Type<I> i)
            where C : ISeraColion<A> where B : IRef<A>
        {
            colion.Collect<Unit, JsonDeserializer, B>(Base, asmer);
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool CNone() => true;
    }

    #endregion

    #region Seq

    public override Unit CSeq<C, A, B, T, I>(C colion, B asmer, Type<A> a, Type<T> t, Type<I> i)
    {
        var first = true;
        reader.ReadArrayStart();
        var seq_colctor = new SeqSeraColctor(this);
        asmer.GetRef().Init(null);
        for (; reader.Has(); reader.MoveNext())
        {
            var token = reader.CurrentToken();
            if (token.Kind is JsonTokenKind.ArrayEnd) break;
            if (first) first = false;
            else if (token.Kind is not JsonTokenKind.Comma) throw new NotImplementedException(); // todo error
            colion.CollectItem<Unit, SeqSeraColctor, B>(ref seq_colctor, asmer);
            asmer.GetRef().Add();
        }
        return default;
    }

    private readonly struct SeqSeraColctor(JsonDeserializer Base) : ISeqSeraColctor<Unit>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Unit CItem<C, A, B, I>(C colion, B asmer, Type<A> a, Type<I> i)
            where C : ISeraColion<A> where B : IRef<A>
        {
            colion.Collect<Unit, JsonDeserializer, B>(Base, asmer);
            return default;
        }
    }

    #endregion
}
