using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using Sera.Core.Impls;

namespace Sera.Core;

public class BuiltInRuntimeProvider : IRuntimeProvider, IAsyncRuntimeProvider
{
    public static BuiltInRuntimeProvider Instance { get; } = new();

    private static readonly FrozenDictionary<Type, object> BuiltIns = new Dictionary<Type, object>
    {
        { typeof(bool), PrimitiveImpls.Boolean },
        { typeof(sbyte), PrimitiveImpls.SByte },
        { typeof(short), PrimitiveImpls.Int16 },
        { typeof(int), PrimitiveImpls.Int32 },
        { typeof(long), PrimitiveImpls.Int64 },
        { typeof(Int128), PrimitiveImpls.Int128 },
        { typeof(byte), PrimitiveImpls.Byte },
        { typeof(ushort), PrimitiveImpls.UInt16 },
        { typeof(uint), PrimitiveImpls.UInt32 },
        { typeof(ulong), PrimitiveImpls.UInt64 },
        { typeof(UInt128), PrimitiveImpls.UInt128 },
        { typeof(nint), PrimitiveImpls.IntPtr },
        { typeof(nuint), PrimitiveImpls.UIntPtr },
        { typeof(Half), PrimitiveImpls.Half },
        { typeof(float), PrimitiveImpls.Single },
        { typeof(double), PrimitiveImpls.Double },
        { typeof(decimal), PrimitiveImpls.Decimal },
        { typeof(BigInteger), PrimitiveImpls.BigInteger },
        { typeof(Complex), PrimitiveImpls.Complex },
        { typeof(TimeSpan), PrimitiveImpls.TimeSpan },
        { typeof(DateOnly), PrimitiveImpls.DateOnly },
        { typeof(TimeOnly), PrimitiveImpls.TimeOnly },
        { typeof(DateTime), PrimitiveImpls.DateTime },
        { typeof(DateTimeOffset), PrimitiveImpls.DateTimeOffset },
        { typeof(Guid), PrimitiveImpls.Guid },
        { typeof(Range), PrimitiveImpls.Range },
        { typeof(Index), PrimitiveImpls.Index },
        { typeof(char), PrimitiveImpls.Char },
        { typeof(Rune), PrimitiveImpls.Rune },
        { typeof(string), StringImpl.Instance },
        { typeof(SeraAny), AnyImpl.Instance },
        { typeof(object), RawObjectImpl.Instance },
        { typeof(DBNull), DBNullImpl.Instance },
        { typeof(Unit), UnitImpl<Unit>.Instance },
        { typeof(ValueTuple), EmptyTupleImpl<ValueTuple>.Instance },
    }.ToFrozenDictionary();

    private static readonly ConditionalWeakTable<Type, object> InstanceCache = new();

    public ISerialize<object?> GetRuntimeSerialize()
        => throw new NotSupportedException($"{nameof(BuiltInRuntimeProvider)} dose not support dynamic create");

    public bool TryGetRuntimeSerialize(out ISerialize<object?> serialize)
    {
        serialize = default!;
        return false;
    }

    public ISerialize<T> GetSerialize<T>()
        => TryGetSerialize<T>(out var serialize)
            ? serialize
            : throw new ArgumentException($"Type {typeof(T)} is not BuiltIn");

    public bool TryGetSerialize<T>(out ISerialize<T> serialize)
    {
        var type = typeof(T);
        if (BuiltIns.TryGetValue(type, out var r))
        {
            serialize = (ISerialize<T>)r;
            return true;
        }
        else if (CheckCache<T, ISerialize<T>>(type, out serialize)) return true;
        else
        {
            serialize = null!;
            return false;
        }
    }

    public IDeserialize<object?> GetRuntimeDeserialize()
        => throw new NotSupportedException($"{nameof(BuiltInRuntimeProvider)} dose not support dynamic create");

    public bool TryGetRuntimeDeserialize(out IDeserialize<object?> deserialize)
    {
        deserialize = default!;
        return false;
    }

    public IDeserialize<T> GetDeserialize<T>()
        => TryGetDeserialize<T>(out var deserialize)
            ? deserialize
            : throw new ArgumentException($"Type {typeof(T)} is not BuiltIn");

    public bool TryGetDeserialize<T>(out IDeserialize<T> deserialize)
    {
        var type = typeof(T);
        if (BuiltIns.TryGetValue(type, out var r))
        {
            deserialize = (IDeserialize<T>)r;
            return true;
        }
        else if (CheckCache<T, IDeserialize<T>>(type, out deserialize)) return true;
        else
        {
            deserialize = null!;
            return false;
        }
    }

    private static bool CheckCache<T, R>(Type type, out R r)
    {
        if (InstanceCache.TryGetValue(type, out var o))
        {
            if (o == null!)
            {
                r = default!;
                return false;
            }
            else
            {
                r = (R)o;
                return true;
            }
        }
        r = CreateInstance<T, R>(type);
        InstanceCache.TryAdd(type, r!);
        return r is not null;
    }

    private static R CreateInstance<T, R>(Type type)
    {
        if (type.FullName == "Microsoft.FSharp.Core.Unit") return (R)(object)UnitImpl<T>.Instance;
        if (type.FullName == "LibSugar.Unit") return (R)(object)UnitImpl<T>.Instance;
        return default!;
    }

    public IAsyncSerialize<object?> GetRuntimeAsyncSerialize()
        => throw new NotSupportedException($"{nameof(BuiltInRuntimeProvider)} dose not support dynamic create");

    public bool TryGetRuntimeAsyncSerialize(out IAsyncSerialize<object?> serialize)
    {
        serialize = default!;
        return false;
    }

    public IAsyncSerialize<T> GetAsyncSerialize<T>()
        => TryGetAsyncSerialize<T>(out var serialize)
            ? serialize
            : throw new ArgumentException($"Type {typeof(T)} is not BuiltIn");

    public bool TryGetAsyncSerialize<T>(out IAsyncSerialize<T> serialize)
    {
        var type = typeof(T);
        if (BuiltIns.TryGetValue(type, out var r))
        {
            serialize = (IAsyncSerialize<T>)r;
            return true;
        }
        else if (CheckCache<T, IAsyncSerialize<T>>(type, out serialize)) return true;
        else
        {
            serialize = null!;
            return false;
        }
    }

    public IAsyncDeserialize<object?> GetRuntimeAsyncDeserialize()
        => throw new NotSupportedException($"{nameof(BuiltInRuntimeProvider)} dose not support dynamic create");

    public bool TryGetRuntimeAsyncDeserialize(out IAsyncDeserialize<object?> deserialize)
    {
        deserialize = default!;
        return false;
    }

    public IAsyncDeserialize<T> GetAsyncDeserialize<T>()
        => TryGetAsyncDeserialize<T>(out var deserialize)
            ? deserialize
            : throw new ArgumentException($"Type {typeof(T)} is not BuiltIn");

    public bool TryGetAsyncDeserialize<T>(out IAsyncDeserialize<T> deserialize)
    {
        var type = typeof(T);
        if (BuiltIns.TryGetValue(type, out var r))
        {
            deserialize = (IAsyncDeserialize<T>)r;
            return true;
        }
        else if (CheckCache<T, IAsyncDeserialize<T>>(type, out deserialize)) return true;
        else
        {
            deserialize = null!;
            return false;
        }
    }
}
