using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Sera.Core.Impls.De;
using Sera.Utils;

namespace Sera.Core.Impls.De.Numerics;

public readonly struct MatrixImpl(SeraFormats? formats = null) :
    ISeraColion<Matrix3x2>, ISeraColion<Matrix4x4>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Collect<R, C>(ref C colctor, InType<Matrix3x2>? t = null) where C : ISeraColctor<Matrix3x2, R>
        => colctor.CTuple(new Matrix3x2Impl(formats), new IdentityMapper<Matrix3x2>(), new Type<Matrix3x2>());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Collect<R, C>(ref C colctor, InType<Matrix4x4>? t = null) where C : ISeraColctor<Matrix4x4, R>
        => colctor.CTuple(new Matrix4x4Impl(formats), new IdentityMapper<Matrix4x4>(), new Type<Matrix4x4>());

    public readonly struct Matrix3x2Impl(SeraFormats? formats = null) : ITupleSeraColion<Matrix3x2>
    {
        public int? Size
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => 3 * 2;
        }

        public int? TotalSize
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Size;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Matrix3x2 Builder(Type<Matrix3x2> b = default) => default;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public R CollectItem<R, C>(ref C colctor, int index, Type<Matrix3x2> b = default)
            where C : ITupleSeraColctor<Matrix3x2, R>
            => colctor.CItem(new PrimitiveImpl(formats), new Matrix3x2Effector(index), new Type<float>());
    }

    public readonly struct Matrix4x4Impl(SeraFormats? formats = null) : ITupleSeraColion<Matrix4x4>
    {
        public int? Size
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => 4 * 4;
        }

        public int? TotalSize
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Size;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Matrix4x4 Builder(Type<Matrix4x4> b = default) => default;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public R CollectItem<R, C>(ref C colctor, int index, Type<Matrix4x4> b = default)
            where C : ITupleSeraColctor<Matrix4x4, R>
            => colctor.CItem(new PrimitiveImpl(formats), new Matrix4x4Effector(index), new Type<float>());
    }

    public readonly struct Matrix3x2Effector(int index) : ISeraEffector<Matrix3x2, float>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Effect(ref Matrix3x2 target, float value)
        {
            var span = MemoryMarshal.Cast<Matrix3x2, float>(new(ref target));
            span[index] = value;
        }
    }

    public readonly struct Matrix4x4Effector(int index) : ISeraEffector<Matrix4x4, float>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Effect(ref Matrix4x4 target, float value)
        {
            var span = MemoryMarshal.Cast<Matrix4x4, float>(new(ref target));
            span[index] = value;
        }
    }
}
