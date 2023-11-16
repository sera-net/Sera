using Sera.Utils;

namespace Sera.Core;

public interface ISeraFunctor<in T, out U>
{
    public U Map(T value, InType<U>? u = null);
}

public interface ISeraEffector<T, in I>
{
    public void Effect(ref T target, I value);
}

public interface ISeraColion<out T>
{
    public R Collect<R, C>(ref C colctor, InType<T>? t = null) where C : ISeraColctor<T, R>;
}

public interface ITupleSeraColion<B>
{
    public int Size { get; }

    public B Builder(Type<B> b = default);

    public R CollectItem<R, C>(ref C colctor, int index, Type<B> b = default)
        where C : ITupleSeraColctor<B, R>;
}

public interface ISeqSeraColion<B>
{
    public B Builder(int? cap, Type<B> b = default);

    public R CollectItem<R, C>(ref C colctor, Type<B> b = default)
        where C : ISeqSeraColctor<B, R>;
}
