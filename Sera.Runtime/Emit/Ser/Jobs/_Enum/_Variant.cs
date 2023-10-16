using System;
using System.Reflection;
using Sera.Core.Ser;
using Sera.Runtime.Utils;

namespace Sera.Runtime.Emit.Ser.Jobs._Enum;

internal abstract class _Variant(Type UnderlyingType, EnumInfo[] Items, SeraEnumAttribute? EnumAttr) : _Base
{
    public SeraEnumAttribute? EnumAttr { get; private set; } = EnumAttr;
    protected SerializerVariantHint? RootHint { get; } = EnumAttr?.SerHint;

    protected static readonly Type VariantMetaType = typeof((string name, SerializerVariantHint? hint));
    protected static readonly FieldInfo VariantMetaFieldName =
        VariantMetaType.GetField(nameof(ValueTuple<int, int>.Item1), BindingFlags.Public | BindingFlags.Instance)!;
    protected static readonly FieldInfo VariantMetaFieldHint =
        VariantMetaType.GetField(nameof(ValueTuple<int, int>.Item2), BindingFlags.Public | BindingFlags.Instance)!;
}
