using System;
using System.Reflection;
using Sera.Core.Impls;
using Sera.Runtime.Emit.Deps;

namespace Sera.Runtime.Emit.Ser.Jobs._IDictionary._Generic;

internal class _ReadOnly_Private(Type KeyType, Type ValueType) :_Private(KeyType, ValueType) {
    public override void Init(EmitStub stub, EmitMeta target)
    {
        BaseType = typeof(IReadOnlyDictionarySerializeImplBase<,,>).MakeGenericType(target.Type, KeyType, ValueType);
    }

    public override EmitTransform[] CollectTransforms(EmitStub stub, EmitMeta target) => target.IsValueType
        ? new EmitTransform[]
        {
            new Transforms._IReadOnlyDictionarySerializeImplWrapper(KeyType, ValueType),
        }
        : new EmitTransform[]
        {
            new Transforms._IReadOnlyDictionarySerializeImplWrapper(KeyType, ValueType),
            new Transforms._ReferenceTypeWrapperSerializeImpl(),
        };
    
    public override object CreateInst(EmitStub stub, EmitMeta target, RuntimeDeps deps)
    {
        var dep_key = deps.Get(0);
        var dep_val = deps.Get(1);
        var wrapper_key = dep_key.MakeSerializeWrapper(KeyType);
        var wrapper_val = dep_val.MakeSerializeWrapper(ValueType);
        var inst_type = typeof(IReadOnlyDictionarySerializeImpl<,,,,>)
            .MakeGenericType(target.Type, KeyType, ValueType, wrapper_key, wrapper_val);
        var ctor = inst_type.GetConstructor(BindingFlags.Public | BindingFlags.Instance,
            new[] { wrapper_key, wrapper_val })!;
        var inst = ctor.Invoke(new object?[] { null, null });
        return inst;
    }
}
