using Sera.Core;

namespace Sera.Emit.Template;

public enum Enum2
{
    A,
    B,
    C,
}

public readonly struct Enum2Impl : ISeraVision<Enum2>, IUnionSeraVision<Enum2>
{
    public static UnionStyle? style;
    public static VariantStyle? style2;

    public R Accept<R, V>(V visitor, Enum2 value) where V : ASeraVisitor<R>
        => visitor.VUnion(this, value);

    public string? Name => nameof(Enum2);

    public R AcceptUnion<R, V>(V visitor, ref Enum2 value) where V : AUnionSeraVisitor<R>
        => value switch
        {
            Enum2.A => visitor.VVariant(new Variant(nameof(Enum2.A), VariantTag.Create((int)Enum2.A)), style, style2),
            Enum2.B => visitor.VVariant(new Variant(nameof(Enum2.B), VariantTag.Create((int)Enum2.B)), style, null),
            Enum2.C => visitor.VVariant(new Variant(nameof(Enum2.C), VariantTag.Create((int)Enum2.C)), style, style2),
            _ => visitor.VNone(),
        };
}
