using System.Numerics;
using System.Runtime.CompilerServices;
using Sera.Core.Impls.De;
using Sera.Utils;

namespace Sera.Core.Impls.De.Numerics;

public readonly struct QuaternionImpl(SeraFormats? formats = null)
    : ISeraColion<Quaternion>, ITupleSeraColion<Quaternion>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Collect<R, C>(ref C colctor, InType<Quaternion>? t = null) where C : ISeraColctor<Quaternion, R>
        => colctor.CTuple(this, new IdentityMapper<Quaternion>(), new Type<Quaternion>());

    public int Size
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => 4;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Quaternion Builder(Type<Quaternion> b = default) => default;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R CollectItem<R, C>(ref C colctor, int index, Type<Quaternion> b = default)
        where C : ITupleSeraColctor<Quaternion, R>
        => colctor.CItem(new PrimitiveImpl(formats), new QuaternionEffector(index), new Type<float>());

    public readonly struct QuaternionEffector(int index) : ISeraEffector<Quaternion, float>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Effect(ref Quaternion target, float value) => target[index] = value;
    }
}
