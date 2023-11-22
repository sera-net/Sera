using System.Numerics;
using System.Runtime.CompilerServices;
using Sera.Core.Impls.De;
using Sera.Utils;

namespace Sera.Core.Impls.De.Numerics;

public readonly struct PlaneImpl(SeraFormats? formats = null)
    : ISeraColion<Plane>, ITupleSeraColion<Plane>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Collect<R, C>(ref C colctor, InType<Plane>? t = null) where C : ISeraColctor<Plane, R>
        => colctor.CTuple(this, new IdentityMapper<Plane>(), new Type<Plane>());

    public int Size
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => 4;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Plane Builder(Type<Plane> b = default) => default;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R CollectItem<R, C>(ref C colctor, int index, Type<Plane> b = default)
        where C : ITupleSeraColctor<Plane, R>
        => colctor.CItem(new PrimitiveImpl(formats), new QuaternionEffector(index), new Type<float>());

    public readonly struct QuaternionEffector(int index) : ISeraEffector<Plane, float>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Effect(ref Plane target, float value)
        {
            switch (index)
            {
                case 0:
                    target.Normal.X = value;
                    break;
                case 1:
                    target.Normal.Y = value;
                    break;
                case 2:
                    target.Normal.Z = value;
                    break;
                case 3:
                    target.D = value;
                    break;
            }
        }
    }
}
