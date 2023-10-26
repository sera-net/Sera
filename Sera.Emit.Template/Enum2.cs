using Sera.Core;
using Sera.Core.Ser;

namespace Sera.Emit.Template;

public enum Enum2 : long
{
    A,
    B,
    C
}

public class Enum2Impl : ISerialize<Enum2>
{
    public void Write<S>(S serializer, Enum2 value, ISeraOptions options) where S : ISerializer
    {
        switch (value)
        {
            case Enum2.A:
                serializer.WriteVariantUnit<Enum2>(
                    "Enum2",
                    new Variant("A", VariantTag.Create((long)value)),
                    SerializerVariantHint.Unknown
                );
                break;
            case Enum2.B:
                serializer.WriteVariantUnit<Enum2>(
                    "Enum2",
                    new Variant("B", VariantTag.Create((long)value)),
                    SerializerVariantHint.Unknown
                );
                break;
            case Enum2.C:
                serializer.WriteVariantUnit<Enum2>(
                    "Enum2",
                    new Variant("C", VariantTag.Create((long)value)),
                    SerializerVariantHint.Unknown
                );
                break;
            default:
                serializer.WriteVariantUnit<Enum2>(
                    "Enum2",
                    new Variant(VariantTag.Create((long)value)),
                    SerializerVariantHint.Unknown
                );
                break;
        }
    }
}
