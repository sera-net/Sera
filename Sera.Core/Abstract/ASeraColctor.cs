using Sera.Core.SerDe;
using Sera.Utils;
using SeraBase = Sera.Core.SeraBase<Sera.Core.ISeraColion<object?>>;
using SeraBaseForward = Sera.Core.SeraBaseForward<Sera.Core.ISeraColion<object?>>;

namespace Sera.Core;

public interface ISeraColctor<in T, [AssocType] out R> : ISeraAbility<ISeraColion<object?>>
{
    #region Primitive

    public R CPrimitive<F>(F functor, Type<bool> t, SeraFormats? formats = null)
        where F : ISeraFunctor<bool, T>;

    // todo other Primitive

    #endregion

    #region Tuple

    public R CTuple<C, B, F>(C colion, F functor, Type<B> b)
        where C : ITupleSeraColion<B> where F : ISeraFunctor<B, T>;

    #endregion

    #region Seq

    public R CSeq<C, B, F>(C colion, F functor, Type<B> b)
        where C : ISeqSeraColion<B> where F : ISeraFunctor<B, T>;

    #endregion
}

public interface ITupleSeraColctor<B, [AssocType] out R>
{
    public R CItem<C, E, I>(C colion, E effector, Type<I> i)
        where C : ISeraColion<I> where E : ISeraEffector<B, I>;

    public R CNone();
}

public interface ISeqSeraColctor<B, [AssocType] out R>
{
    public R CItem<C, E, I>(C colion, E effector, Type<I> i)
        where C : ISeraColion<I> where E : ISeraEffector<B, I>;
}
