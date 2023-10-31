using System.Numerics;
using System.Runtime.CompilerServices;

namespace Sera.Core.Impls.Ser;

public readonly struct MatrixImpl(SeraFormats? formats = null) : ISeraVision<Matrix3x2>, ISeraVision<Matrix4x4>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, Matrix3x2 value) where V : ASeraVisitor<R>
        => visitor.VTuple(new Matrix3x2Impl(formats), value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, Matrix4x4 value) where V : ASeraVisitor<R>
        => visitor.VTuple(new Matrix4x4Impl(formats), value);

    public readonly struct Matrix3x2Impl(SeraFormats? formats = null) : ITupleSeraVision<Matrix3x2>
    {
        public int Size
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => 3 * 2;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public R AcceptItem<R, V>(V visitor, Matrix3x2 value, int index) where V : ATupleSeraVisitor<R>
            => index switch
            {
                0 => visitor.VItem(new PrimitiveImpl(formats), value.M11),
                1 => visitor.VItem(new PrimitiveImpl(formats), value.M12),
                2 => visitor.VItem(new PrimitiveImpl(formats), value.M21),
                3 => visitor.VItem(new PrimitiveImpl(formats), value.M22),
                4 => visitor.VItem(new PrimitiveImpl(formats), value.M31),
                5 => visitor.VItem(new PrimitiveImpl(formats), value.M32),
                _ => visitor.VNone(),
            };
    }

    public readonly struct Matrix4x4Impl(SeraFormats? formats = null) : ITupleSeraVision<Matrix4x4>
    {
        public int Size
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => 4 * 4;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public R AcceptItem<R, V>(V visitor, Matrix4x4 value, int index) where V : ATupleSeraVisitor<R>
            => index switch
            {
                0 => visitor.VItem(new PrimitiveImpl(formats), value.M11),
                1 => visitor.VItem(new PrimitiveImpl(formats), value.M12),
                2 => visitor.VItem(new PrimitiveImpl(formats), value.M13),
                3 => visitor.VItem(new PrimitiveImpl(formats), value.M14),
                4 => visitor.VItem(new PrimitiveImpl(formats), value.M21),
                5 => visitor.VItem(new PrimitiveImpl(formats), value.M22),
                6 => visitor.VItem(new PrimitiveImpl(formats), value.M23),
                7 => visitor.VItem(new PrimitiveImpl(formats), value.M24),
                8 => visitor.VItem(new PrimitiveImpl(formats), value.M31),
                9 => visitor.VItem(new PrimitiveImpl(formats), value.M32),
                10 => visitor.VItem(new PrimitiveImpl(formats), value.M33),
                11 => visitor.VItem(new PrimitiveImpl(formats), value.M34),
                12 => visitor.VItem(new PrimitiveImpl(formats), value.M41),
                13 => visitor.VItem(new PrimitiveImpl(formats), value.M42),
                14 => visitor.VItem(new PrimitiveImpl(formats), value.M43),
                15 => visitor.VItem(new PrimitiveImpl(formats), value.M44),
                _ => visitor.VNone(),
            };
    }
}
