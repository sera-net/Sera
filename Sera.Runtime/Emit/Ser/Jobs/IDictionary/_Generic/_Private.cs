using System;

namespace Sera.Runtime.Emit.Ser.Jobs.IDictionary._Generic;

internal abstract class _Private(Type KeyType, Type ValueType) : _Generic(KeyType, ValueType)
{
    
    public Type BaseType { get; set; } = null!;
}
