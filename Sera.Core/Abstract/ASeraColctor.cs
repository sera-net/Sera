using System;
using Sera.Utils;
using SeraBase = Sera.Core.SeraBase<Sera.Core.ISeraColion<object?, Sera.Core.ISeraAsmer<object?>>>;
using SeraBaseForward = Sera.Core.SeraBaseForward<Sera.Core.ISeraColion<object?, Sera.Core.ISeraAsmer<object?>>>;

namespace Sera.Core;

public abstract class ASeraColctor<R> : SeraBase
{
    #region Primitive

    #region bool

    public abstract R CPrimitive<A, B>(B asmer, Type<A> a, Type<bool> t, SeraFormats? formats = null)
        where A : ISeraValueAsmer<bool> where B : IRef<A>;

    #endregion

    // todo other Primitive

    #endregion

    #region Tuple

    public abstract R CTuple<C, A, B, T>(C colion, B asmer, Type<A> a, Type<T> t)
        where C : ISeraTupleColion<T, A> where A : ISeraAsmer<T> where B : IRef<A>;

    #endregion
}

public interface ITupleSeraColctor<out R>
{
    public abstract R CItem<C, A, B, I>(C colion, B asmer, Type<A> a, Type<I> i)
        where C : ISeraColion<I, A> where A : ISeraAsmer<I> where B : IRef<A>;

    public abstract R CNone();
}
