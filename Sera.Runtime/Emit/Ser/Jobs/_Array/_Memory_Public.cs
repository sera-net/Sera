using System;
using System.Reflection;
using System.Reflection.Emit;
using Sera.Runtime.Emit.Deps;
using Sera.Runtime.Utils;

namespace Sera.Runtime.Emit.Ser.Jobs._Array;

internal class _Memory_Public(Type ItemType) : _ReadOnlyMemory_Public(ItemType)
{
    protected override NullabilityInfo? GetElementNullabilityInfo(EmitMeta target)
        => target.TypeMeta.Nullability?.NullabilityInfo?.GenericTypeArguments[0];

    protected override void ConvertValue(EmitMeta target, DepPlace dep, ILGenerator ilg)
    {
        var cast = ReflectionUtils.Get_Memory_to_ReadOnlyMemory(ItemType);
        ilg.Emit(OpCodes.Call, cast);
    }
}
