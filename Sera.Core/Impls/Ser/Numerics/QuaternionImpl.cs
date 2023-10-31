using System.Numerics;
using System.Runtime.CompilerServices;

namespace Sera.Core.Impls.Ser;

public readonly struct QuaternionImpl(SeraFormats? formats = null) :
    ISeraVision<Quaternion>,
    ITupleSeraVision<Quaternion>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, Quaternion value) where V : ASeraVisitor<R>
        => visitor.VTuple(this, value);

    public int Size
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => 4;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R AcceptItem<R, V>(V visitor, Quaternion value, int index) where V : ATupleSeraVisitor<R>
        => index switch
        {
            0 => visitor.VItem(new PrimitiveImpl(formats), value.X),
            1 => visitor.VItem(new PrimitiveImpl(formats), value.Y),
            2 => visitor.VItem(new PrimitiveImpl(formats), value.Z),
            3 => visitor.VItem(new PrimitiveImpl(formats), value.W),
            _ => visitor.VNone(),
        };
}
