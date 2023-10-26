using System;
using System.Collections.Generic;
using System.Reflection;

namespace Sera.Runtime.Emit.Ser.Jobs._ICollection._Generic;

internal class _ReadOnly_Public(Type ItemType, InterfaceMapping? mapping, MethodInfo? DirectGetEnumerator)
    : _Public(ItemType, mapping, DirectGetEnumerator)
{
    protected override MethodInfo GetGetCount() => typeof(IReadOnlyCollection<>).MakeGenericType(ItemType)
        .GetProperty(nameof(IReadOnlyCollection<int>.Count))!
        .GetMethod!;
}
