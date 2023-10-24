﻿using System;
using System.Reflection;
using Sera.Core.Impls;
using Sera.Runtime.Emit.Deps;

namespace Sera.Runtime.Emit.Ser.Jobs._ICollection._Generic;

internal class _ReadOnly_Private(Type ItemType) : _Private(ItemType)
{
    public override void Init(EmitStub stub, EmitMeta target)
    {
        BaseType = typeof(IReadOnlyCollectionSerializeImplBase<,>).MakeGenericType(target.Type, ItemType);
    }

    public override EmitTransform[] CollectTransforms(EmitStub stub, EmitMeta target) => target.IsValueType
        ? new EmitTransform[]
        {
            new Transforms._IReadOnlyCollectionSerializeImplWrapper(ItemType),
        }
        : new EmitTransform[]
        {
            new Transforms._IReadOnlyCollectionSerializeImplWrapper(ItemType),
            new Transforms._ReferenceTypeWrapperSerializeImpl(),
        };

    public override object CreateInst(EmitStub stub, EmitMeta target, RuntimeDeps deps)
    {
        var dep = deps.Get(0);
        var wrapper = dep.MakeSerializeWrapper(ItemType);
        var inst_type = typeof(IReadOnlyCollectionSerializeImpl<,,>).MakeGenericType(target.Type, ItemType, wrapper);
        var ctor = inst_type.GetConstructor(BindingFlags.Public | BindingFlags.Instance, new[] { wrapper })!;
        var inst = ctor.Invoke(new object?[] { null });
        return inst;
    }
}