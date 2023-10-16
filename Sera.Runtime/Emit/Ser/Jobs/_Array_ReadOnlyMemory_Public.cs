using System;
using System.Reflection;
using Sera.Runtime.Utils;

namespace Sera.Runtime.Emit.Ser.Jobs;

internal class _Array_ReadOnlyMemory_Public(Type ItemType) : _Array_Public(ItemType)
{
    protected override MethodInfo WriteArrayMethod => ReflectionUtils.ISerializer_WriteArray_2generic_ReadOnlyMemory;
}
