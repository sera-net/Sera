using Sera.Core;
using Sera.Core.Ser;

namespace Sera.Emit.Template;

public enum Enum3
{
    A = 50,
    B = -20,
    C = 500,
}

public class Enum3Impl : ISerialize<Enum3>
{
    public void Write<S>(S serializer, Enum3 value, ISeraOptions options) where S : ISerializer
    {
        switch (value)
        {
            case Enum3.A:
                serializer.WriteVariantUnit<Enum3>(
                    "Enum3",
                    new Variant("A", VariantTag.Create((long)value)),
                    SerializerVariantHint.Unknown
                );
                break;
            case Enum3.B:
                serializer.WriteVariantUnit<Enum3>(
                    "Enum3",
                    new Variant("B", VariantTag.Create((long)value)),
                    SerializerVariantHint.Unknown
                );
                break;
            case Enum3.C:
                serializer.WriteVariantUnit<Enum3>(
                    "Enum3",
                    new Variant("C", VariantTag.Create((long)value)),
                    SerializerVariantHint.Unknown
                );
                break;
            default:
                serializer.WriteVariantUnit<Enum3>(
                    "Enum3",
                    new Variant(VariantTag.Create((long)value)),
                    SerializerVariantHint.Unknown
                );
                break;
        }
    }
}
