using System.Reflection;
using System.Runtime.CompilerServices;
using Sera.Core;
using Sera.Core.Impls.Ser;

namespace Sera.Runtime.Utils;

internal readonly struct NullabilityInfoBinarySerializeImpl : ISeraVision<NullabilityInfo>,
    ITupleSeraVision<NullabilityInfo>
{
    public static NullabilityInfoBinarySerializeImpl Instance { get; } = new();
    public static NullableClassImpl<NullabilityInfo, NullabilityInfoBinarySerializeImpl> NullableImpl { get; }
        = new(Instance);
    public static ArrayImpl<NullabilityInfo, NullabilityInfoBinarySerializeImpl> ArrayImpl { get; }
        = new(Instance);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, NullabilityInfo value) where V : ASeraVisitor<R>
        => visitor.VTuple(this, value);

    public int Size => 4;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R AcceptItem<R, V>(V visitor, NullabilityInfo value, int index) where V : ATupleSeraVisitor<R>
        => index switch
        {
            0 => visitor.VItem(new PrimitiveImpl(), (byte)value.ReadState),
            1 => visitor.VItem(new PrimitiveImpl(), (byte)value.WriteState),
            2 => visitor.VItem(NullableImpl, value.ElementType),
            3 => visitor.VItem(ArrayImpl, value.GenericTypeArguments),
            _ => visitor.VNone(),
        };
}
