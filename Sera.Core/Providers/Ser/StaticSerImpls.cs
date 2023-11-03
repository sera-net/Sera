using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Sera.Core.Impls.Ser;

namespace Sera.Core.Providers.Ser;

public readonly struct StaticSerImpls
{
    #region Builtins

    private static readonly object PrimitiveImpl = new PrimitiveImpl();
    private static readonly object StringImpl = new StringImpl();
    private static readonly object objectImpl =
        new NullableClassImpl<object, ReferenceImpl<object, EmptyStructImpl<object>>>();
    private static readonly object VectorImpl = new VectorImpl();
    private static readonly object MatrixImpl = new MatrixImpl();

    private static readonly FrozenDictionary<Type, object> Builtins = new Dictionary<Type, object>
    {
        { typeof(bool), PrimitiveImpl },
        { typeof(sbyte), PrimitiveImpl },
        { typeof(short), PrimitiveImpl },
        { typeof(int), PrimitiveImpl },
        { typeof(long), PrimitiveImpl },
        { typeof(Int128), PrimitiveImpl },
        { typeof(byte), PrimitiveImpl },
        { typeof(ushort), PrimitiveImpl },
        { typeof(uint), PrimitiveImpl },
        { typeof(ulong), PrimitiveImpl },
        { typeof(UInt128), PrimitiveImpl },
        { typeof(nint), PrimitiveImpl },
        { typeof(nuint), PrimitiveImpl },
        { typeof(Half), PrimitiveImpl },
        { typeof(float), PrimitiveImpl },
        { typeof(double), PrimitiveImpl },
        { typeof(decimal), PrimitiveImpl },
        { typeof(NFloat), PrimitiveImpl },
        { typeof(BigInteger), PrimitiveImpl },
        { typeof(Complex), PrimitiveImpl },
        { typeof(TimeSpan), PrimitiveImpl },
        { typeof(DateOnly), PrimitiveImpl },
        { typeof(TimeOnly), PrimitiveImpl },
        { typeof(DateTime), PrimitiveImpl },
        { typeof(DateTimeOffset), PrimitiveImpl },
        { typeof(Guid), PrimitiveImpl },
        { typeof(Range), PrimitiveImpl },
        { typeof(Index), PrimitiveImpl },
        { typeof(char), PrimitiveImpl },
        { typeof(Rune), PrimitiveImpl },
        { typeof(Uri), PrimitiveImpl },
        { typeof(Version), PrimitiveImpl },

        { typeof(string), StringImpl },

        { typeof(object), objectImpl },

        { typeof(DBNull), new UnitImpl<DBNull>() },
        { typeof(Unit), new UnitImpl<Unit>() },
        { typeof(ValueTuple), new TupleImpl() },
        
        { typeof(Vector2), VectorImpl },
        { typeof(Vector3), VectorImpl },
        { typeof(Vector4), VectorImpl },
        { typeof(Matrix3x2), MatrixImpl },
        { typeof(Matrix4x4), MatrixImpl },
        { typeof(Plane), new PlaneImpl() },
        { typeof(Quaternion), new QuaternionImpl() },
    }.ToFrozenDictionary();

    #endregion

    #region Dynamic impls

    private abstract class DynamicImpl
    {
        // ReSharper disable once MemberHidesStaticFromOuterClass
        public abstract ISeraVision<T> Get<T>();
    }

    private class UnitDynamicImpl : DynamicImpl
    {
        // ReSharper disable once MemberHidesStaticFromOuterClass
        public static UnitDynamicImpl Instance { get; } = new();

        // ReSharper disable once MemberHidesStaticFromOuterClass
        public override ISeraVision<T> Get<T>() => new UnitImpl<T>();
    }

    private static readonly FrozenDictionary<string, DynamicImpl> DynamicImpls =
        new Dictionary<string, DynamicImpl>
        {
            { "Microsoft.FSharp.Core.Unit", UnitDynamicImpl.Instance },
            { "LibSugar.Unit", UnitDynamicImpl.Instance },
        }.ToFrozenDictionary();

    private static readonly ConditionalWeakTable<Type, object?> InstanceCache = new();

    #endregion

    #region Generic Get

    public static bool TryGet<T>(out ISeraVision<T> impl)
    {
        if (Builtins.TryGetValue(typeof(T), out var _impl))
        {
            impl = (ISeraVision<T>)_impl;
            return true;
        }
        impl = (ISeraVision<T>?)InstanceCache.GetValue(typeof(T), CreateInstance<T>)!;
        return impl != null!;
    }

    private static object? CreateInstance<T>(Type type)
    {
        var name = type.FullName;
        if (name != null && DynamicImpls.TryGetValue(name, out var impl)) return impl.Get<T>();
        return null;
    }

    #endregion
}
