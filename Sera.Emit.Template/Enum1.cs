using Sera.Core;

namespace Sera.Emit.Template;

public enum Enum1 : byte
{
    A,
    B = 6,
    C,
}

public readonly struct Enum1Impl : ISeraVision<Enum1>, IUnionSeraVision<Enum1>
{
    public static UnionStyle? style;
    public static VariantStyle? style2;

    public R Accept<R, V>(V visitor, Enum1 value) where V : ASeraVisitor<R>
        => visitor.VUnion(this, value);

    public string Name => nameof(Enum1);

    public R AcceptUnion<R, V>(V visitor, Enum1 value) where V : AUnionSeraVisitor<R>
        => value switch
        {
            Enum1.A => visitor.VVariant(new Variant(nameof(Enum1.A), VariantTag.Create((int)Enum1.A)), style, style2),
            Enum1.B => visitor.VVariant(new Variant(nameof(Enum1.B), VariantTag.Create((int)Enum1.B)), style, null),
            Enum1.C => visitor.VVariant(new Variant(nameof(Enum1.C), VariantTag.Create((int)Enum1.C)), style, style2),
            _ => visitor.VVariant(new Variant(VariantTag.Create((int)value)), style),
        };
}
