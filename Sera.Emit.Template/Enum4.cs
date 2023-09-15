using Sera.Core;
using Sera.Core.Ser;

namespace Sera.Emit.Template;

public enum Enum4
{
    A,
    B,
    C,
}

public class Enum4Impl : ISerialize<Enum4>
{
    public static Dictionary<int, (string, SerializerVariantHint?)> _metas = new()
    {
        { (int)Enum4.A, ("A", null) },
        { (int)Enum4.B, ("B", null) },
        { (int)Enum4.C, ("C", null) },
    };

    public void Write<S>(S serializer, Enum4 value, ISeraOptions options) where S : ISerializer
    {
        if (_metas.TryGetValue((int)value, out var meta))
        {
            serializer.WriteVariantUnit<Enum4>(
                "Enum4",
                new Variant(meta.Item1, VariantTag.Create((int)value)),
                meta.Item2
            );
        }
        else
        {
            serializer.WriteVariantUnit<Enum4>(
                "Enum4",
                new Variant(VariantTag.Create((int)value)),
                SerializerVariantHint.Unknown
            );
        }
    }
}
