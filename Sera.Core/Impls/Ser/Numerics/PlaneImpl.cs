using System.Numerics;
using System.Runtime.CompilerServices;

namespace Sera.Core.Impls.Ser;

public readonly struct PlaneImpl(SeraFormats? formats = null) : ISeraVision<Plane>, ITupleSeraVision<Plane>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, Plane value) where V : ASeraVisitor<R>
        => visitor.VTuple(this, value);

    public int Size
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => 4;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R AcceptItem<R, V>(V visitor, Plane value, int index) where V : ATupleSeraVisitor<R>
        => index switch
        {
            0 => visitor.VItem(new PrimitiveImpl(formats), value.Normal.X),
            1 => visitor.VItem(new PrimitiveImpl(formats), value.Normal.Y),
            2 => visitor.VItem(new PrimitiveImpl(formats), value.Normal.Z),
            3 => visitor.VItem(new PrimitiveImpl(formats), value.D),
            _ => visitor.VNone(),
        };
}
