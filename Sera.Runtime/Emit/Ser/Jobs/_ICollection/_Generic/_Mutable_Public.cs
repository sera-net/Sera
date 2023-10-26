using System;
using System.Collections.Generic;
using System.Reflection;

namespace Sera.Runtime.Emit.Ser.Jobs._ICollection._Generic;

internal class _Mutable_Public(Type ItemType, InterfaceMapping? mapping, MethodInfo? DirectGetEnumerator)
    : _Public(ItemType, mapping, DirectGetEnumerator)
{
    protected override MethodInfo GetGetCount() => typeof(ICollection<>).MakeGenericType(ItemType)
        .GetProperty(nameof(ICollection<int>.Count))!
        .GetMethod!;
}
