using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using Sera.Utils;

namespace Sera.Core.Impls.De.Numerics;

public readonly struct VectorImpl(SeraFormats? formats = null)
    : ISeraColion<Vector2>, ISeraColion<Vector3>, ISeraColion<Vector4>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Collect<R, C>(ref C colctor, InType<Vector2>? t = null) where C : ISeraColctor<Vector2, R>
        => colctor.CTuple(new Vector2Impl(formats), new IdentityMapper<Vector2>(), new Type<Vector2>());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Collect<R, C>(ref C colctor, InType<Vector3>? t = null) where C : ISeraColctor<Vector3, R>
        => colctor.CTuple(new Vector3Impl(formats), new IdentityMapper<Vector3>(), new Type<Vector3>());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Collect<R, C>(ref C colctor, InType<Vector4>? t = null) where C : ISeraColctor<Vector4, R>
        => colctor.CTuple(new Vector4Impl(formats), new IdentityMapper<Vector4>(), new Type<Vector4>());

    public readonly struct Vector2Impl(SeraFormats? formats = null) : ITupleSeraColion<Vector2>
    {
        public int? Size
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => 2;
        }
        
        public int? TotalSize
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Size;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector2 Builder(Type<Vector2> b = default) => default;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public R CollectItem<R, C>(ref C colctor, int index, Type<Vector2> b = default)
            where C : ITupleSeraColctor<Vector2, R>
            => colctor.CItem(new PrimitiveImpl(formats), new Vector2Effector(index), new Type<float>());
    }

    public readonly struct Vector3Impl(SeraFormats? formats = null) : ITupleSeraColion<Vector3>
    {
        public int? Size
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => 3;
        }
        
        public int? TotalSize
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Size;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector3 Builder(Type<Vector3> b = default) => default;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public R CollectItem<R, C>(ref C colctor, int index, Type<Vector3> b = default)
            where C : ITupleSeraColctor<Vector3, R>
            => colctor.CItem(new PrimitiveImpl(formats), new Vector3Effector(index), new Type<float>());
    }

    public readonly struct Vector4Impl(SeraFormats? formats = null) : ITupleSeraColion<Vector4>
    {
        public int? Size
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => 4;
        }
        
        public int? TotalSize
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Size;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector4 Builder(Type<Vector4> b = default) => default;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public R CollectItem<R, C>(ref C colctor, int index, Type<Vector4> b = default)
            where C : ITupleSeraColctor<Vector4, R>
            => colctor.CItem(new PrimitiveImpl(formats), new Vector4Effector(index), new Type<float>());
    }

    public readonly struct Vector2Effector(int index) : ISeraEffector<Vector2, float>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Effect(ref Vector2 target, float value)
            => target[index] = value;
    }

    public readonly struct Vector3Effector(int index) : ISeraEffector<Vector3, float>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Effect(ref Vector3 target, float value)
            => target[index] = value;
    }

    public readonly struct Vector4Effector(int index) : ISeraEffector<Vector4, float>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Effect(ref Vector4 target, float value)
            => target[index] = value;
    }
}

public readonly struct VectorImpl<T, D>(D dep) : ISeraColion<Vector<T>>,
    ISeraColion<Vector64<T>>,
    ISeraColion<Vector128<T>>,
    ISeraColion<Vector256<T>>,
    ISeraColion<Vector512<T>>
    where D : ISeraColion<T>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Collect<R, C>(ref C colctor, InType<Vector<T>>? t = null) where C : ISeraColctor<Vector<T>, R>
        => colctor.CTuple(new VectorVarImpl(dep), new IdentityMapper<Vector<T>>(), new Type<Vector<T>>());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Collect<R, C>(ref C colctor, InType<Vector64<T>>? t = null) where C : ISeraColctor<Vector64<T>, R>
        => colctor.CTuple(new Vector64Impl(dep), new IdentityMapper<Vector64<T>>(), new Type<Vector64<T>>());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Collect<R, C>(ref C colctor, InType<Vector128<T>>? t = null) where C : ISeraColctor<Vector128<T>, R>
        => colctor.CTuple(new Vector128Impl(dep), new IdentityMapper<Vector128<T>>(), new Type<Vector128<T>>());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Collect<R, C>(ref C colctor, InType<Vector256<T>>? t = null) where C : ISeraColctor<Vector256<T>, R>
        => colctor.CTuple(new Vector256Impl(dep), new IdentityMapper<Vector256<T>>(), new Type<Vector256<T>>());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Collect<R, C>(ref C colctor, InType<Vector512<T>>? t = null) where C : ISeraColctor<Vector512<T>, R>
        => colctor.CTuple(new Vector512Impl(dep), new IdentityMapper<Vector512<T>>(), new Type<Vector512<T>>());

    public readonly struct VectorVarImpl(D dep) : ITupleSeraColion<Vector<T>>
    {
        public int? Size
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Vector<T>.Count;
        }
        
        public int? TotalSize
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Size;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector<T> Builder(Type<Vector<T>> b = default) => default;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public R CollectItem<R, C>(ref C colctor, int index, Type<Vector<T>> b = default)
            where C : ITupleSeraColctor<Vector<T>, R>
            => colctor.CItem(dep, new VectorVarEffector<T>(), new Type<T>());
    }

    public readonly struct Vector64Impl(D dep) : ITupleSeraColion<Vector64<T>>
    {
        public int? Size
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Vector64<T>.Count;
        }
        
        public int? TotalSize
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Size;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector64<T> Builder(Type<Vector64<T>> b = default) => default;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public R CollectItem<R, C>(ref C colctor, int index, Type<Vector64<T>> b = default)
            where C : ITupleSeraColctor<Vector64<T>, R>
            => colctor.CItem(dep, new Vector64Effector<T>(), new Type<T>());
    }

    public readonly struct Vector128Impl(D dep) : ITupleSeraColion<Vector128<T>>
    {
        public int? Size
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Vector128<T>.Count;
        }
        
        public int? TotalSize
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Size;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector128<T> Builder(Type<Vector128<T>> b = default) => default;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public R CollectItem<R, C>(ref C colctor, int index, Type<Vector128<T>> b = default)
            where C : ITupleSeraColctor<Vector128<T>, R>
            => colctor.CItem(dep, new Vector128Effector<T>(), new Type<T>());
    }

    public readonly struct Vector256Impl(D dep) : ITupleSeraColion<Vector256<T>>
    {
        public int? Size
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Vector256<T>.Count;
        }

        public int? TotalSize
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Size;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector256<T> Builder(Type<Vector256<T>> b = default) => default;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public R CollectItem<R, C>(ref C colctor, int index, Type<Vector256<T>> b = default)
            where C : ITupleSeraColctor<Vector256<T>, R>
            => colctor.CItem(dep, new Vector256Effector<T>(), new Type<T>());
    }

    public readonly struct Vector512Impl(D dep) : ITupleSeraColion<Vector512<T>>
    {
        public int? Size
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Vector512<T>.Count;
        }
        
        public int? TotalSize
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Size;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector512<T> Builder(Type<Vector512<T>> b = default) => default;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public R CollectItem<R, C>(ref C colctor, int index, Type<Vector512<T>> b = default)
            where C : ITupleSeraColctor<Vector512<T>, R>
            => colctor.CItem(dep, new Vector512Effector<T>(), new Type<T>());
    }
}

public readonly struct VectorVarEffector<T>(int index) : ISeraEffector<Vector<T>, T>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Effect(ref Vector<T> target, T value)
        => target = target.WithElement(index, value);
}

public readonly struct Vector64Effector<T>(int index) : ISeraEffector<Vector64<T>, T>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Effect(ref Vector64<T> target, T value)
        => target = target.WithElement(index, value);
}

public readonly struct Vector128Effector<T>(int index) : ISeraEffector<Vector128<T>, T>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Effect(ref Vector128<T> target, T value)
        => target = target.WithElement(index, value);
}

public readonly struct Vector256Effector<T>(int index) : ISeraEffector<Vector256<T>, T>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Effect(ref Vector256<T> target, T value)
        => target = target.WithElement(index, value);
}

public readonly struct Vector512Effector<T>(int index) : ISeraEffector<Vector512<T>, T>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Effect(ref Vector512<T> target, T value)
        => target = target.WithElement(index, value);
}
