using Sera.Core;
using Sera.Core.Ser;

namespace Sera.Emit.Template;

public enum Enum1
{
    A,
    B,
    C
}

public class Enum1Impl : ISerialize<Enum1>
{
    public void Write<S>(S serializer, Enum1 value, ISeraOptions options) where S : ISerializer
    {
        switch (value)
        {
            case Enum1.A:
                serializer.WriteVariantUnit<Enum1>(
                    "Enum1",
                    new Variant("A", VariantTag.Create((int)value)),
                    SerializerVariantHint.Unknown
                );
                break;
            case Enum1.B:
                serializer.WriteVariantUnit<Enum1>(
                    "Enum1",
                    new Variant("B", VariantTag.Create((int)value)),
                    SerializerVariantHint.Unknown
                );
                break;
            case Enum1.C:
                serializer.WriteVariantUnit<Enum1>(
                    "Enum1",
                    new Variant("C", VariantTag.Create((int)value)),
                    SerializerVariantHint.Unknown
                );
                break;
            default:
                serializer.WriteVariantUnit<Enum1>(
                    "Enum1",
                    new Variant(VariantTag.Create((int)value)),
                    SerializerVariantHint.Unknown
                );
                break;
        }
    }
}
