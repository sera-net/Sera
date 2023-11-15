using Sera.Utils;

namespace Sera.Core;

public interface ISeraCtor<out T>
{
    public T Ctor();
}

public interface ISeraColion<[AssocType] A>
{
    public R Collect<R, C, B>(C colctor, B asmer)
        where C : ASeraColctor<R> where B : IRef<A>;
}

public interface ISeraAsmable<[AssocType] out A>
{
    public A Asmer();
}

public interface ISeraAsmer<out T>
{
    public T Asm();
}

public interface IValueSeraAsmer<in T>
{
    public void Provide(T value);
}

public interface ITupleSeraColion<[AssocType] A>
{
    public int Size { get; }

    public R CollectItem<R, C, B>(ref C colctor, B asmer, int index)
        where C : ITupleSeraColctor<R> where B : IRef<A>;
}

public interface ICapSeraCtor<out T>
{
    public T Ctor(int? cap);
}

public interface ISeqSeraAsmer
{
    public void Init(int? count);
    public void Add();
}

public interface ISeqSeraColion<[AssocType] A>
{
    public R CollectItem<R, C, B>(ref C colctor, B asmer)
        where C : ISeqSeraColctor<R> where B : IRef<A>;
}
