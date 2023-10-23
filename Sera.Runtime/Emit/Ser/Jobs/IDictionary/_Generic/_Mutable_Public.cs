using System;
using System.Collections.Generic;
using System.Reflection;

namespace Sera.Runtime.Emit.Ser.Jobs.IDictionary._Generic;

internal class _Mutable_Public(Type KeyType, Type ValueType, InterfaceMapping? mapping, MethodInfo? DirectGetEnumerator)
    : _Public(KeyType, ValueType, mapping, DirectGetEnumerator)
{
    protected override MethodInfo GetGetCount() => typeof(ICollection<>)
        .MakeGenericType(typeof(KeyValuePair<,>).MakeGenericType(KeyType, ValueType))
        .GetProperty(nameof(ICollection<int>.Count))!
        .GetMethod!;
}
