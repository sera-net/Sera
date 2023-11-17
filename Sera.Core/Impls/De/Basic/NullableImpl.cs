using System.Runtime.CompilerServices;
using Sera.Core.Impls.De.Misc;
using Sera.Utils;

namespace Sera.Core.Impls.De;

public readonly struct NullableImpl<T, D>(D dep) : ISeraColion<T?>, IOptionSeraColion<T?>
    where T : struct where D : ISeraColion<T>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Collect<R, C>(ref C colctor, InType<T?>? t = null) where C : ISeraColctor<T?, R>
        => colctor.COption(this);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T? CtorNone() => null;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R CollectSome<R, C>(ref C colctor, InType<T?>? t = null) where C : ISomeSeraColctor<T?, R>
        => colctor.CSome(dep, new NullableMapper(), new Type<T>());

    private readonly struct NullableMapper : ISeraMapper<T, T?>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T? Map(T value, InType<T?>? u = null) => value;
    }
}

public readonly struct NullableClassImpl<T, D>(D dep) : ISeraColion<T?>, IOptionSeraColion<T?>
    where T : class where D : ISeraColion<T>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Collect<R, C>(ref C colctor, InType<T?>? t = null) where C : ISeraColctor<T?, R>
        => colctor.COption(this);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T? CtorNone() => null;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R CollectSome<R, C>(ref C colctor, InType<T?>? t = null) where C : ISomeSeraColctor<T?, R>
        => colctor.CSome(dep, new IdentityMapper<T>(), new Type<T>());
}
