using System;
using System.Reflection;
using Sera.Runtime.Emit.Ser.Internal;

namespace Sera.Runtime.Emit.Ser.Jobs._Enum;

internal abstract class _Variant : _Base
{
    protected static readonly Type VariantMetaType = typeof(VariantMeta);
    protected static readonly FieldInfo VariantMetaFieldName =
        VariantMetaType.GetField(nameof(VariantMeta.Name), BindingFlags.Public | BindingFlags.Instance)!;
    protected static readonly FieldInfo VariantMetaFieldStyle =
        VariantMetaType.GetField(nameof(VariantMeta.Style), BindingFlags.Public | BindingFlags.Instance)!;
}
