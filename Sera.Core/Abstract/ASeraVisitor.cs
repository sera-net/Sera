using System;
using System.Buffers;
using System.Numerics;
using System.Text;

using SeraBase = Sera.Core.SeraBase<Sera.Core.ISeraVision<object?>>;
using SeraBaseForward = Sera.Core.SeraBaseForward<Sera.Core.ISeraVision<object?>>;

namespace Sera.Core;

public abstract class ASeraVisitor<R> : SeraBase
{
    #region Flush

    public abstract R Flush();

    #endregion
    
    #region Reference

    public abstract R VReference<V, T>(V vision, T value) where V : ISeraVision<T> where T : class;

    #endregion

    #region Primitive

    public abstract R VPrimitive(bool value, SeraFormats? formats = null);

    public abstract R VPrimitive(sbyte value, SeraFormats? formats = null);
    public abstract R VPrimitive(byte value, SeraFormats? formats = null);

    public abstract R VPrimitive(short value, SeraFormats? formats = null);
    public abstract R VPrimitive(ushort value, SeraFormats? formats = null);

    public abstract R VPrimitive(int value, SeraFormats? formats = null);
    public abstract R VPrimitive(uint value, SeraFormats? formats = null);

    public abstract R VPrimitive(long value, SeraFormats? formats = null);
    public abstract R VPrimitive(ulong value, SeraFormats? formats = null);

    public abstract R VPrimitive(Int128 value, SeraFormats? formats = null);
    public abstract R VPrimitive(UInt128 value, SeraFormats? formats = null);

    public abstract R VPrimitive(nint value, SeraFormats? formats = null);
    public abstract R VPrimitive(nuint value, SeraFormats? formats = null);

    public abstract R VPrimitive(Half value, SeraFormats? formats = null);
    public abstract R VPrimitive(float value, SeraFormats? formats = null);
    public abstract R VPrimitive(double value, SeraFormats? formats = null);
    public abstract R VPrimitive(decimal value, SeraFormats? formats = null);

    public abstract R VPrimitive(BigInteger value, SeraFormats? formats = null);
    public abstract R VPrimitive(Complex value, SeraFormats? formats = null);

    public abstract R VPrimitive(TimeSpan value, SeraFormats? formats = null);
    public abstract R VPrimitive(DateOnly value, SeraFormats? formats = null);
    public abstract R VPrimitive(TimeOnly value, SeraFormats? formats = null);
    public abstract R VPrimitive(DateTime value, SeraFormats? formats = null);
    public abstract R VPrimitive(DateTimeOffset value, SeraFormats? formats = null);

    public abstract R VPrimitive(Guid value, SeraFormats? formats = null);

    public abstract R VPrimitive(Range value, SeraFormats? formats = null);
    public abstract R VPrimitive(Index value, SeraFormats? formats = null);

    public abstract R VPrimitive(char value, SeraFormats? formats = null);
    public abstract R VPrimitive(Rune value, SeraFormats? formats = null);

    public abstract R VPrimitive(Uri value, SeraFormats? formats = null);

    public abstract R VPrimitive(Version value, SeraFormats? formats = null);

    #endregion

    #region String

    public virtual R VString(string value) => VString(value.AsMemory());
    public virtual R VString(char[] value) => VString(value.AsMemory());
    public abstract R VString(ReadOnlyMemory<char> value);

    #endregion

    #region String Encoded

    public virtual R VString(byte[] value, Encoding encoding) =>
        VString(value.AsMemory(), encoding);

    public abstract R VString(ReadOnlyMemory<byte> value, Encoding encoding);

    #endregion

    #region Bytes

    public virtual R VBytes(byte[] value) => VBytes(value.AsMemory());
    public abstract R VBytes(ReadOnlyMemory<byte> value);
    public abstract R VBytes(ReadOnlySequence<byte> value);

    #endregion

    #region Array

    public virtual R VArray<V, T>(V vision, T[] value) where V : ISeraVision<T> =>
        VArray<V, T>(vision, value.AsMemory());
    
    public abstract R VArray<V, T>(V vision, ReadOnlyMemory<T> value) where V : ISeraVision<T>;

    public abstract R VArray<V, T>(V vision, ReadOnlySequence<T> value) where V : ISeraVision<T>;

    #endregion

    #region Unit

    public abstract R VUnit();

    #endregion

    #region Option

    public abstract R VNone();

    public virtual R VNone<T>() => VNone();

    public abstract R VSome<V, T>(V vision, T value) where V : ISeraVision<T>;

    #endregion

    #region Entry

    public abstract R VEntry<KV, VV, IK, IV>(KV keyVision, VV valueVision, IK key, IV value)
        where KV : ISeraVision<IK>
        where VV : ISeraVision<IV>;

    #endregion

    #region Tuple

    public abstract R VTuple<V, T>(V vision, T value) where V : ITupleSeraVision<T>;

    #endregion

    #region Seq

    public abstract R VSeq<V>(V vision) where V : ISeqSeraVision;

    public virtual R VSeq<V, T>(V vision) where V : ISeqSeraVision
        => VSeq(vision);

    public virtual R VSeq<V, T, I>(V vision) where V : ISeqSeraVision
        => VSeq<V, T>(vision);

    #endregion

    #region Map

    public abstract R VMap<V>(V vision) where V : IMapSeraVision;

    public virtual R VMap<V, T>(V vision) where V : IMapSeraVision
        => VMap(vision);

    public virtual R VMap<V, T, IK, IV>(V vision) where V : IMapSeraVision
        => VMap<V, T>(vision);

    #endregion

    #region Struct

    public abstract R VStruct<V, T>(V vision, T value) where V : IStructSeraVision<T>;

    #endregion

    #region Union

    public abstract R VUnion<V, T>(V vision, T value) where V : IUnionSeraVision<T>;

    #endregion
}

#region Tuple

public abstract class ATupleSeraVisitor<R>(SeraBase Base) : SeraBaseForward(Base)
{
    public abstract R VItem<T, V>(V vision, T value)
        where V : ISeraVision<T>;

    public abstract R VNone();
}

#endregion

#region Seq

public abstract class ASeqSeraVisitor<R>(SeraBase Base) : SeraBaseForward(Base)
{
    public abstract R VItem<T, V>(V vision, T value)
        where V : ISeraVision<T>;

    public abstract R VEnd();
}

#endregion

#region Map

public abstract class AMapSeraVisitor<R>(SeraBase Base) : SeraBaseForward(Base)
{
    public abstract R VEntry<KV, VV, IK, IV>(KV keyVision, VV valueVision, IK key, IV value)
        where KV : ISeraVision<IK>
        where VV : ISeraVision<IV>;

    public abstract R VEnd();
}

#endregion

#region Struct

public abstract class AStructSeraVisitor<R>(SeraBase Base) : SeraBaseForward(Base)
{
    public abstract R VField<V, T>(V vision, T value, string name, long key) where V : ISeraVision<T>;

    public abstract R VNone();
}

#endregion

#region Union

public abstract class AUnionSeraVisitor<R>(SeraBase Base) : SeraBaseForward(Base)
{
    public abstract R VEmpty();

    public abstract R VVariant(Variant variant, UnionStyle? style = null);

    public abstract R VVariant<V, T>(V vision, T value, Variant variant, UnionStyle? style = null)
        where V : ISeraVision<T>;

    public abstract R VVariantStruct<V, T>(V vision, T value, Variant variant, UnionStyle? style = null)
        where V : IStructSeraVision<T>;
}

#endregion
