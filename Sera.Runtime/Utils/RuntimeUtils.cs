using System.Runtime.CompilerServices;
using Sera.Core;
using Sera.Core.Impls;
using Sera.Core.Impls.Ser;
using Sera.Runtime.Emit;
using Sera.Runtime.Emit.Ser;
using BindingFlags = System.Reflection.BindingFlags;

namespace Sera.Runtime.Utils;

public static class RuntimeUtils
{
    private static readonly ConditionalWeakTable<object, object> ReferenceNullableCache = new();

    public static ISeraVision<T> Wrap<T>(SeraStyles? styles)
    {
        var ser = EmitSerImpls.Get<T>(styles);
        var target_type = typeof(T);
        if (target_type.IsValueType) return ser;
        return (ISeraVision<T>)ReferenceNullableCache.GetValue(ser, _ =>
        {
            var ser_type = ser.GetType();
            var impl_type = typeof(NullableClassImpl<,>).MakeGenericType(target_type, ser.GetType());
            var ctor = impl_type.GetConstructor(BindingFlags.Public | BindingFlags.Instance, new[] { ser_type })!;
            var inst = ctor.Invoke(new object[] { ser });
            return inst;
        });
    }
}
