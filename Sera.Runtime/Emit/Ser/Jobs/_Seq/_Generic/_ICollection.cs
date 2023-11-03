using System;
using System.Reflection;
using Sera.Core.Impls.Ser;
using Sera.Runtime.Emit.Deps;

namespace Sera.Runtime.Emit.Ser.Jobs._Seq._Generic;

internal class _ICollection(Type ItemType) : _Generic(ItemType)
{
    private Type EmitType { get; set; } = null!;
    private Type RuntimeType { get; set; } = null!;

    public override void Init(EmitStub stub, EmitMeta target) { }

    public override Type GetEmitType(EmitStub stub, EmitMeta target, EmitDeps deps)
        => EmitType =
            typeof(SeqICollectionImpl<,,>).MakeGenericType(target.Type, ItemType, deps.Get(0).MakeSerWrapper(ItemType));

    public override Type GetRuntimeType(EmitStub stub, EmitMeta target, RuntimeDeps deps)
        => RuntimeType =
            typeof(SeqICollectionImpl<,,>).MakeGenericType(target.Type, ItemType, deps.Get(0).MakeSerWrapper(ItemType));

    public override void Emit(EmitStub stub, EmitMeta target, EmitDeps deps) { }

    public override object CreateInst(EmitStub stub, EmitMeta target, RuntimeDeps deps)
    {
        var dep = deps.Get(0);
        var wrapper = dep.MakeSerWrapper(ItemType);
        var ctor = RuntimeType.GetConstructor(BindingFlags.Public | BindingFlags.Instance,
            new[] { wrapper })!;
        var inst = ctor.Invoke(new object[] { null! });
        return inst;
    }
}
