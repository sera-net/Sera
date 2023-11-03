using System;
using System.Linq;
using System.Reflection;
using Sera.Core.Impls.Ser;
using Sera.Runtime.Emit.Deps;
using Sera.Runtime.Utils;

namespace Sera.Runtime.Emit.Ser.Jobs;

internal class _KeyValuePair(Type KeyType, Type ValueType) : _Base
{
    private Type EmitType { get; set; } = null!;
    private Type RuntimeType { get; set; } = null!;

    public override void Init(EmitStub stub, EmitMeta target) { }

    public override EmitTransform[] CollectTransforms(EmitStub stub, EmitMeta target)
        => EmitTransform.EmptyTransforms;

    public override DepMeta[] CollectDeps(EmitStub stub, EmitMeta target)
    {
        var nullables = target.TypeMeta.Nullability?.NullabilityInfo?.GenericTypeArguments;
        var types = new[] { KeyType, ValueType };
        return types.Select((t, i) =>
        {
            var nullable = nullables?[i];
            var transforms = !t.IsValueType && nullable is not
                { ReadState: NullabilityState.NotNull }
                ? SerializeEmitProvider.NullableClassImplTransforms
                : EmitTransform.EmptyTransforms;
            var meta = new DepMeta(
                new(TypeMetas.GetTypeMeta(t, new NullabilityMeta(nullable)), target.Styles.TakeFormats()),
                transforms);
            return meta;
        }).ToArray();
    }

    public override Type GetEmitType(EmitStub stub, EmitMeta target, EmitDeps deps)
    {
        var dep_key = deps.Get(0);
        var dep_value = deps.Get(1);
        return EmitType =
            typeof(EntryImpl<,,,>).MakeGenericType(
                KeyType, ValueType,
                dep_key.MakeSerWrapper(KeyType),
                dep_value.MakeSerWrapper(ValueType)
            );
    }

    public override Type GetRuntimeType(EmitStub stub, EmitMeta target, RuntimeDeps deps)
    {
        var dep_key = deps.Get(0);
        var dep_value = deps.Get(1);
        return EmitType =
            typeof(EntryImpl<,,,>).MakeGenericType(
                KeyType, ValueType,
                dep_key.MakeSerWrapper(KeyType),
                dep_value.MakeSerWrapper(ValueType)
            );
    }

    public override void Emit(EmitStub stub, EmitMeta target, EmitDeps deps) { }

    public override object CreateInst(EmitStub stub, EmitMeta target, RuntimeDeps deps)
    {
        var dep_key = deps.Get(0);
        var dep_value = deps.Get(1);
        var wrapper_key = dep_key.MakeSerWrapper(KeyType);
        var wrapper_value = dep_value.MakeSerWrapper(ValueType);
        var ctor = RuntimeType.GetConstructor(BindingFlags
                .Public | BindingFlags.Instance,
            new[] { wrapper_key, wrapper_value })!;
        var inst = ctor.Invoke(new object[] { null!, null! });
        return inst;
    }
}
