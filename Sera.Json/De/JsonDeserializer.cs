using System;
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

    public override IRuntimeProvider<ISeraColion<object?, ISeraAsmer<object?>>> RuntimeProvider =>
        RuntimeProviderOverride ?? throw new NotImplementedException("todo"); // EmptyDeRuntimeProvider.Instance

    public IRuntimeProvider<ISeraColion<object?, ISeraAsmer<object?>>>? RuntimeProviderOverride { get; set; }

    public T Enter<C, T, A>(C colion) where C : ISeraColion<T, A> where A : ISeraAsmer<T>
    {
        var asmer = new Box<A>(colion.Asmer(new Type<A>()));
        colion.Collect<Unit, JsonDeserializer, Box<A>>(this, asmer, new());
        return asmer.Value.Asm(new());
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
            var err = colion.CollectItem<bool, TupleSeraColctor, B>(ref tuple_colctor, asmer, i, new Type<T>());
            if (err) throw new SerializeException($"Unable to write item {i} of tuple {typeof(T)}");
        }
        reader.ReadArrayEnd();
        return default;
    }

    private readonly struct TupleSeraColctor(JsonDeserializer Base) : ITupleSeraColctor<bool>
    {
        public bool CItem<C, A, B, I>(C colion, B asmer, Type<A> a, Type<I> i) where C : ISeraColion<I, A>
            where A : ISeraAsmer<I>
            where B : IRef<A>
        {
            colion.Collect<Unit, JsonDeserializer, B>(Base, asmer, i);
            return false;
        }

        public bool CNone() => true;
    }

    #endregion
}
