using System;
using System.Buffers;
using System.Text;
using Sera.Core.SerDe;
using Sera.Utils;
using SeraBase = Sera.Core.SeraBase<Sera.Core.ISeraColion<object?>>;
using SeraBaseForward = Sera.Core.SeraBaseForward<Sera.Core.ISeraColion<object?>>;

namespace Sera.Core;

public interface ISeraColctor<in T, [AssocType] out R> : ISeraAbility<ISeraColion<object?>>
{
    #region Primitive

    public R CPrimitive<M>(M mapper, Type<bool> t, SeraFormats? formats = null)
        where M : ISeraMapper<bool, T>;

    // todo other Primitive

    #endregion

    #region String

    public R CString<M>(M mapper, Type<string> t)
        where M : ISeraMapper<string, T>;

    public R CString<M>(M mapper, Type<char[]> t)
        where M : ISeraMapper<char[], T>;

    public R CString<M>(M mapper, Type<Memory<char>> t)
        where M : ISeraMapper<Memory<char>, T>;

    public R CString<M>(M mapper, Type<ReadOnlyMemory<char>> t)
        where M : ISeraMapper<ReadOnlyMemory<char>, T>;

    #endregion

    #region String Encoded

    public R CString<M>(M mapper, Type<byte[]> t, Encoding encoding)
        where M : ISeraMapper<byte[], T>;

    public R CString<M>(M mapper, Type<Memory<byte>> t, Encoding encoding)
        where M : ISeraMapper<Memory<byte>, T>;

    public R CString<M>(M mapper, Type<ReadOnlyMemory<byte>> t, Encoding encoding)
        where M : ISeraMapper<ReadOnlyMemory<byte>, T>;

    #endregion

    #region Bytes

    // todo

    #endregion

    #region Array

    public R CArray<C, M, I>(C colion, M mapper, Type<I[]> t, Type<I> i)
        where C : ISeraColion<I> where M : ISeraMapper<I[], T>;

    public R CArray<C, M, I>(C colion, M mapper, Type<Memory<I>> t, Type<I> i)
        where C : ISeraColion<I> where M : ISeraMapper<Memory<I>, T>;

    public R CArray<C, M, I>(C colion, M mapper, Type<ReadOnlyMemory<I>> t, Type<I> i)
        where C : ISeraColion<I> where M : ISeraMapper<ReadOnlyMemory<I>, T>;

    public R CArray<C, M, I>(C colion, M mapper, Type<ReadOnlySequence<I>> t, Type<I> i)
        where C : ISeraColion<I> where M : ISeraMapper<ReadOnlySequence<I>, T>;

    #endregion

    #region Unit

    public R CUnit<N>(N ctor) where N : ISeraCtor<T>;

    #endregion

    #region Option

    public R COption<C>(C colion)
        where C : IOptionSeraColion<T>;

    #endregion

    #region Tuple

    public R CTuple<C, B, M>(C colion, M mapper, Type<B> b)
        where C : ITupleSeraColion<B> where M : ISeraMapper<B, T>;

    #endregion

    #region Seq

    public R CSeq<C, B, M>(C colion, M mapper, Type<B> b)
        where C : ISeqSeraColion<B> where M : ISeraMapper<B, T>;

    #endregion
}

public interface ISomeSeraColctor<in T, [AssocType] out R>
{
    public R CSome<C, M, U>(C colion, M mapper, Type<U> u)
        where C : ISeraColion<U> where M : ISeraMapper<U, T>;
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
