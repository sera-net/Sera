using System;
using System.Reflection;
using Sera.Runtime.Utils;

namespace Sera.Runtime.Emit.Ser.Jobs._Array;

internal class _List_Public(Type ItemType) : _Public(ItemType)
{
    protected override MethodInfo WriteArrayMethod => ReflectionUtils.ISerializer_WriteArray_2generic_list;
}
