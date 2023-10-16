using System.Runtime.CompilerServices;
using Sera.Core.Impls;
using Sera.Runtime.Emit;
using BindingFlags = System.Reflection.BindingFlags;

namespace Sera.Runtime.Utils;

public static class RuntimeUtils
{
    private static readonly ConditionalWeakTable<object, object> ReferenceNullableCache = new();

    public static ISerialize<T> GetMayReferenceNullableSerialize<T>(this EmitRuntimeProvider rt)
    {
        var ser = rt.GetSerialize<T>();
        var target_type = typeof(T);
        if (target_type.IsValueType) return ser;
        return (ISerialize<T>)ReferenceNullableCache.GetValue(ser, _ =>
        {
            var ser_type = ser.GetType();
            var impl_type = typeof(NullableReferenceTypeSerializeImpl<,>).MakeGenericType(target_type, ser.GetType());
            var ctor = impl_type.GetConstructor(BindingFlags.Public | BindingFlags.Instance, new[] { ser_type })!;
            var inst = ctor.Invoke(new object[] { ser });
            return inst;
        });
    }
}
