using Sera.Utils;

namespace Sera.Core;

public interface ISeraColion<T, A> where A : ISeraAsmer<T>
{
    public A Asmer(Type<A> a);

    public R Collect<R, C, B>(C colctor, B asmer, Type<T> t)
        where C : ASeraColctor<R> where B : IRef<A>;
}

public interface ISeraAsmer<T>
{
    public T Asm(Type<T> t);
}

public interface ISeraValueAsmer<in T>
{
    public void Provide(T value);
}

public interface ISeraTupleColion<T, A> where A : ISeraAsmer<T>
{
    public int Size { get; }

    public R CollectItem<R, C, B>(ref C colctor, B asmer, int index, Type<T> t)
        where C : ITupleSeraColctor<R> where B : IRef<A>;
}
