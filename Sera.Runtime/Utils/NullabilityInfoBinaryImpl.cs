using System.Reflection;
using Sera.Core;
using Sera.Core.Impls;
using Sera.Core.Ser;

namespace Sera.Runtime.Utils;

internal class NullabilityInfoBinarySerializeImpl : ISerialize<NullabilityInfo>, ISeqSerializerReceiver<NullabilityInfo>
{
    public static NullabilityInfoBinarySerializeImpl Instance { get; } = new();
    public static NullableReferenceTypeImpl<NullabilityInfo, NullabilityInfoBinarySerializeImpl> NullableImpl { get; }
        = new(Instance);
    public static ArraySerializeImpl<NullabilityInfo, NullabilityInfoBinarySerializeImpl> ArrayImpl { get; }
        = new(Instance);

    public void Write<S>(S serializer, NullabilityInfo value, ISeraOptions options) where S : ISerializer
        => serializer.StartSeq(null, value, this);

    public void Receive<S>(NullabilityInfo value, S serializer) where S : ISeqSerializer
    {
        serializer.WriteElement((byte)value.ReadState, PrimitiveImpls.Byte);
        serializer.WriteElement((byte)value.WriteState, PrimitiveImpls.Byte);
        serializer.WriteElement(value.ElementType, NullableImpl);
        serializer.WriteElement(value.GenericTypeArguments, ArrayImpl);
    }
}
