using System;
using Sera.Utils;
using SeraBase = Sera.Core.SeraBase<Sera.Core.ISeraColion<Sera.Core.ISeraAsmer<object?>>>;
using SeraBaseForward = Sera.Core.SeraBaseForward<Sera.Core.ISeraColion<Sera.Core.ISeraAsmer<object?>>>;

namespace Sera.Core;

public abstract class ASeraColctor<[AssocType] R> : SeraBase
{
    #region Primitive

    public abstract R CPrimitive<A, B>(B asmer, Type<A> a, Type<bool> t, SeraFormats? formats = null)
        where A : IValueSeraAsmer<bool> where B : IRef<A>;

    // todo other Primitive

    #endregion

    #region Tuple

    public abstract R CTuple<C, A, B, T>(C colion, B asmer, Type<A> a, Type<T> t)
        where C : ITupleSeraColion<A> where A : ISeraAsmer<T> where B : IRef<A>;

    #endregion

    #region Seq

    public abstract R CSeq<C, A, B, T, I>(C colion, B asmer, Type<A> a, Type<T> t, Type<I> i)
        where C : ISeqSeraColion<A> where A : ISeqSeraAsmer where B : IRef<A>;

    #endregion
}

public interface ITupleSeraColctor<[AssocType] out R>
{
    public abstract R CItem<C, A, B, I>(C colion, B asmer, Type<A> a, Type<I> i)
        where C : ISeraColion<A> where B : IRef<A>;

    public abstract R CNone();
}

public interface ISeqSeraColctor<[AssocType] out R>
{
    public abstract R CItem<C, A, B, I>(C colion, B asmer, Type<A> a, Type<I> i)
        where C : ISeraColion<A> where B : IRef<A>;
}
