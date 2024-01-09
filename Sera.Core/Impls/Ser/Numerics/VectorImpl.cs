using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;

namespace Sera.Core.Impls.Ser;

public readonly struct VectorImpl(SeraFormats? formats = null)
    : ISeraVision<Vector2>, ISeraVision<Vector3>, ISeraVision<Vector4>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, Vector2 value) where V : ASeraVisitor<R>
        => visitor.VTuple(new Vector2Impl(formats), value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, Vector3 value) where V : ASeraVisitor<R>
        => visitor.VTuple(new Vector3Impl(formats), value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, Vector4 value) where V : ASeraVisitor<R>
        => visitor.VTuple(new Vector4Impl(formats), value);

    public readonly struct Vector2Impl(SeraFormats? formats = null) : ITupleSeraVision<Vector2>
    {
        public int Size
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => 2;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public R AcceptItem<R, V>(V visitor, ref Vector2 value, int index) where V : ATupleSeraVisitor<R>
            => visitor.VItem(new PrimitiveImpl(formats), value[index]);
    }

    public readonly struct Vector3Impl(SeraFormats? formats = null) : ITupleSeraVision<Vector3>
    {
        public int Size
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => 3;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public R AcceptItem<R, V>(V visitor, ref Vector3 value, int index) where V : ATupleSeraVisitor<R>
            => visitor.VItem(new PrimitiveImpl(formats), value[index]);
    }

    public readonly struct Vector4Impl(SeraFormats? formats = null) : ITupleSeraVision<Vector4>
    {
        public int Size
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => 4;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public R AcceptItem<R, V>(V visitor, ref Vector4 value, int index) where V : ATupleSeraVisitor<R>
            => visitor.VItem(new PrimitiveImpl(formats), value[index]);
    }
}

public readonly struct VectorImpl<T, D>(D dep) :
    ISeraVision<Vector<T>>,
    ISeraVision<Vector64<T>>,
    ISeraVision<Vector128<T>>,
    ISeraVision<Vector256<T>>,
    ISeraVision<Vector512<T>>
    where D : ISeraVision<T>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, Vector<T> value) where V : ASeraVisitor<R>
        => visitor.VTuple(new VectorVarImpl(dep), value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, Vector64<T> value) where V : ASeraVisitor<R>
        => visitor.VTuple(new Vector64Impl(dep), value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, Vector128<T> value) where V : ASeraVisitor<R>
        => visitor.VTuple(new Vector128Impl(dep), value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, Vector256<T> value) where V : ASeraVisitor<R>
        => visitor.VTuple(new Vector256Impl(dep), value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, Vector512<T> value) where V : ASeraVisitor<R>
        => visitor.VTuple(new Vector512Impl(dep), value);

    public readonly struct VectorVarImpl(D dep) : ITupleSeraVision<Vector<T>>
    {
        public int Size
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Vector<T>.Count;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public R AcceptItem<R, V>(V visitor, ref Vector<T> value, int index) where V : ATupleSeraVisitor<R>
            => visitor.VItem(dep, value[index]);
    }

    public readonly struct Vector64Impl(D dep) : ITupleSeraVision<Vector64<T>>
    {
        public int Size
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Vector64<T>.Count;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public R AcceptItem<R, V>(V visitor, ref Vector64<T> value, int index) where V : ATupleSeraVisitor<R>
            => visitor.VItem(dep, value[index]);
    }

    public readonly struct Vector128Impl(D dep) : ITupleSeraVision<Vector128<T>>
    {
        public int Size
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Vector128<T>.Count;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public R AcceptItem<R, V>(V visitor, ref Vector128<T> value, int index) where V : ATupleSeraVisitor<R>
            => visitor.VItem(dep, value[index]);
    }

    public readonly struct Vector256Impl(D dep) : ITupleSeraVision<Vector256<T>>
    {
        public int Size
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Vector256<T>.Count;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public R AcceptItem<R, V>(V visitor, ref Vector256<T> value, int index) where V : ATupleSeraVisitor<R>
            => visitor.VItem(dep, value[index]);
    }

    public readonly struct Vector512Impl(D dep) : ITupleSeraVision<Vector512<T>>
    {
        public int Size
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Vector512<T>.Count;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public R AcceptItem<R, V>(V visitor, ref Vector512<T> value, int index) where V : ATupleSeraVisitor<R>
            => visitor.VItem(dep, value[index]);
    }
}
