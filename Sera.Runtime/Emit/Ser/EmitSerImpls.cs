using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using Sera.Core;
using Sera.Core.Impls.Ser;
using Sera.Core.Providers.Ser;

namespace Sera.Runtime.Emit.Ser;

public static class EmitSerImpls
{
    private static readonly ConditionalWeakTable<object, object> NullableClassCache = new();
    
    private static readonly SerializeEmitProvider provider = new();

    public static ISeraVision<T> Get<T>(SeraStyles? styles)
    {
        if (StaticSerImpls.TryGet<T>(out var impl)) return impl;
        impl = provider.Get<T>(styles ?? SeraStyles.Default);
        var target_type = typeof(T);
        if (target_type.IsValueType) return impl;
        return (ISeraVision<T>)NullableClassCache.GetValue(impl, impl =>
        {
            var ser_type = impl.GetType();
            var impl_type = typeof(NullableClassImpl<,>).MakeGenericType(target_type, impl.GetType());
            var ctor = impl_type.GetConstructor(BindingFlags.Public | BindingFlags.Instance, new[] { ser_type })!;
            var inst = ctor.Invoke(new object[] { impl });
            return inst;
        });
    }

    private static readonly MethodInfo GenericGet =
        typeof(EmitSerImpls).GetMethod(nameof(Get), BindingFlags.Public | BindingFlags.Static)!;

    private static readonly ConditionalWeakTable<Type, ISeraVision<object>> RuntimeGetCache = new();

    public static ISeraVision<object> RuntimeGet(Type type) => RuntimeGetCache.GetValue(type, static type =>
    {
        var get = GenericGet.MakeGenericMethod(type);
        var impl = get.Invoke(null, new object?[] { null })!;
        var impl_type = impl.GetType();
        var forward_impl = typeof(CastForwardImpl<,>).MakeGenericType(type, impl_type)!;
        var ctor = forward_impl.GetConstructor(BindingFlags.Public | BindingFlags.Instance, new[] { impl_type })!;
        return (ISeraVision<object>)ctor.Invoke(new[] { impl });
    });
}
