﻿using System;
using System.Collections.Generic;
using System.Reflection;

namespace Sera.Runtime.Emit.Ser.Jobs._IDictionary._Generic;

internal class _ReadOnly_Public
    (Type KeyType, Type ValueType, InterfaceMapping? mapping, MethodInfo? DirectGetEnumerator)
    : _Public(KeyType, ValueType, mapping, DirectGetEnumerator)
{
    protected override MethodInfo GetGetCount() => typeof(IReadOnlyCollection<>)
        .MakeGenericType(typeof(KeyValuePair<,>).MakeGenericType(KeyType, ValueType))
        .GetProperty(nameof(IReadOnlyCollection<int>.Count))!
        .GetMethod!;
}
