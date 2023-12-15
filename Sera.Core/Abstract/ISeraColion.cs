using System;
using System.Text;
using Sera.TaggedUnion;
using Sera.Utils;

namespace Sera.Core;

public interface ISeraMapper<in T, out U>
{
    public U Map(T value, InType<U>? u = null);
}

public interface ISeraSpanMapper<T, out U>
{
    public U SpanMap(ReadOnlySpan<T> value, InType<U>? u = null);
}

public interface ISeraEffector<T, in I>
{
    public void Effect(ref T target, I value);
}

public interface ISeraColion<out T>
{
    public R Collect<R, C>(ref C colctor, InType<T>? t = null) where C : ISeraColctor<T, R>;
}

public interface IOptionSeraColion<out T>
{
    public T CtorNone();
    public R CollectSome<R, C>(ref C colctor, InType<T>? t = null) where C : ISomeSeraColctor<T, R>;
}

public interface IEntrySeraColion<B>
{
    public B Builder(Type<B> b = default);

    public R CollectKey<R, C>(ref C colctor, Type<B> b = default)
        where C : IEntrySeraColctor<B, R>;

    public R CollectValue<R, C>(ref C colctor, Type<B> b = default)
        where C : IEntrySeraColctor<B, R>;
}

public interface ITupleSeraColion<B>
{
    public int Size { get; }

    public B Builder(Type<B> b = default);

    public R CollectItem<R, C>(ref C colctor, int index, Type<B> b = default)
        where C : ITupleSeraColctor<B, R>;
}

public interface ISeqSeraColion<B, I>
{
    public B Builder(int? cap, Type<B> b = default);

    public R CollectItem<R, C>(ref C colctor, Type<B> b = default)
        where C : ISeqSeraColctor<B, I, R>;
}

public interface IMapSeraColion<B, IK, IV>
{
    public B Builder(int? cap, Type<B> b = default);

    public R CollectItem<R, C>(ref C colctor, Type<B> b = default)
        where C : IMapSeraColctor<B, IK, IV, R>;
}

public interface IStructSeraColion<B>
{
    public SeraFieldInfos? Fields { get; }

    public B Builder(string? name, Type<B> b = default);

    public R CollectField<R, C>(ref C colctor, int field, string? name, long? key, Type<B> b = default)
        where C : IStructSeraColctor<B, R>;
}

public interface IUnionSeraColion<out T>
{
    public SeraVariantInfos Variants { get; }

    public T CollectEmpty();

    public R CollectVariant<R, C>(ref C colctor, int variant, InType<T>? t = null)
        where C : IUnionSeraColctor<T, R>;
}

public interface ISelectSeraColion<out T>
{
    public ReadOnlyMemory<Any.Kind>? Priorities { get; }

    public R SelectPrimitive<R, C>(ref C colctor, InType<T>? t = null)
        where C : ISelectSeraColctor<T, R>
        => colctor.CNone();

    public R SelectString<R, C>(ref C colctor, Encoding encoding, InType<T>? t = null)
        where C : ISelectSeraColctor<T, R>
        => colctor.CNone();

    public R SelectBytes<R, C>(ref C colctor, InType<T>? t = null)
        where C : ISelectSeraColctor<T, R>
        => colctor.CNone();

    public R SelectArray<R, C>(ref C colctor, InType<T>? t = null)
        where C : ISelectSeraColctor<T, R>
        => colctor.CNone();

    public R SelectUnit<R, C>(ref C colctor, InType<T>? t = null)
        where C : ISelectSeraColctor<T, R>
        => colctor.CNone();

    public R SelectOption<R, C>(ref C colctor, InType<T>? t = null)
        where C : ISelectSeraColctor<T, R>
        => colctor.CNone();

    public R SelectEntry<R, C>(ref C colctor, InType<T>? t = null)
        where C : ISelectSeraColctor<T, R>
        => colctor.CNone();

    public R SelectTuple<R, C>(ref C colctor, int size, InType<T>? t = null)
        where C : ISelectSeraColctor<T, R>
        => colctor.CNone();

    public R SelectSeq<R, C>(ref C colctor, int? size, InType<T>? t = null)
        where C : ISelectSeraColctor<T, R>
        => colctor.CNone();

    public R SelectMap<R, C>(ref C colctor, int? size, InType<T>? t = null)
        where C : ISelectSeraColctor<T, R>
        => colctor.CNone();

    public R SelectStruct<R, C>(ref C colctor, string? name, int? size, InType<T>? t = null)
        where C : ISelectSeraColctor<T, R>
        => colctor.CNone();

    public R SelectUnion<R, C>(ref C colctor, string? name, InType<T>? t = null)
        where C : ISelectSeraColctor<T, R>
        => colctor.CNone();
}
