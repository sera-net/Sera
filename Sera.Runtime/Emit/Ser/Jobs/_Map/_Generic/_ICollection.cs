using System;
using System.Reflection;
using Sera.Core.Impls.Ser;
using Sera.Runtime.Emit.Deps;

namespace Sera.Runtime.Emit.Ser.Jobs._Map._Generic;

internal class _ICollection(Type KeyType, Type ValType) : _Generic(KeyType, ValType)
{
    private Type EmitType { get; set; } = null!;
    private Type RuntimeType { get; set; } = null!;

    public override void Init(EmitStub stub, EmitMeta target) { }

    public override Type GetEmitType(EmitStub stub, EmitMeta target, EmitDeps deps)
        => EmitType =
            typeof(MapICollectionImpl<,,,,>).MakeGenericType(target.Type, KeyType, ValType,
                deps.Get(0).MakeSerWrapper(KeyType), deps.Get(1).MakeSerWrapper(ValType));

    public override Type GetRuntimeType(EmitStub stub, EmitMeta target, RuntimeDeps deps)
        => RuntimeType =
            typeof(MapICollectionImpl<,,,,>).MakeGenericType(target.Type, KeyType, ValType,
                deps.Get(0).MakeSerWrapper(KeyType), deps.Get(1).MakeSerWrapper(ValType));

    public override void Emit(EmitStub stub, EmitMeta target, EmitDeps deps) { }

    public override object CreateInst(EmitStub stub, EmitMeta target, RuntimeDeps deps)
    {
        var dep_key = deps.Get(0);
        var dep_val = deps.Get(1);
        var wrapper_key = dep_key.MakeSerWrapper(KeyType);
        var wrapper_val = dep_val.MakeSerWrapper(ValType);
        var ctor = RuntimeType.GetConstructor(BindingFlags.Public | BindingFlags.Instance,
            new[] { wrapper_key, wrapper_val })!;
        var inst = ctor.Invoke(new object[] { null!, null! });
        return inst;
    }
}
