using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using Sera.Core.Impls;

namespace Sera.Core;

public class BuiltInRuntimeProvider : IRuntimeProvider, IAsyncRuntimeProvider
{
    public static BuiltInRuntimeProvider Instance { get; } = new();

    private static readonly Dictionary<Type, object> BuiltIns = new()
    {
        { typeof(bool), PrimitiveImpl.Boolean },
        { typeof(sbyte), PrimitiveImpl.SByte },
        { typeof(short), PrimitiveImpl.Int16 },
        { typeof(int), PrimitiveImpl.Int32 },
        { typeof(long), PrimitiveImpl.Int64 },
        { typeof(Int128), PrimitiveImpl.Int128 },
        { typeof(byte), PrimitiveImpl.Byte },
        { typeof(ushort), PrimitiveImpl.UInt16 },
        { typeof(uint), PrimitiveImpl.UInt32 },
        { typeof(ulong), PrimitiveImpl.UInt64 },
        { typeof(UInt128), PrimitiveImpl.UInt128 },
        { typeof(nint), PrimitiveImpl.IntPtr },
        { typeof(nuint), PrimitiveImpl.UIntPtr },
        { typeof(Half), PrimitiveImpl.Half },
        { typeof(float), PrimitiveImpl.Single },
        { typeof(double), PrimitiveImpl.Double },
        { typeof(decimal), PrimitiveImpl.Decimal },
        { typeof(BigInteger), PrimitiveImpl.BigInteger },
        { typeof(Complex), PrimitiveImpl.Complex },
        { typeof(TimeSpan), PrimitiveImpl.TimeSpan },
        { typeof(DateOnly), PrimitiveImpl.DateOnly },
        { typeof(TimeOnly), PrimitiveImpl.TimeOnly },
        { typeof(DateTime), PrimitiveImpl.DateTime },
        { typeof(DateTimeOffset), PrimitiveImpl.DateTimeOffset },
        { typeof(Guid), PrimitiveImpl.Guid },
        { typeof(Range), PrimitiveImpl.Range },
        { typeof(Index), PrimitiveImpl.Index },
        { typeof(char), PrimitiveImpl.Char },
        { typeof(Rune), PrimitiveImpl.Rune },
        { typeof(string), StringImpl.Instance },
        { typeof(SeraAny), AnyImpl.Instance },
        { typeof(object), RawObjectImpl.Instance },
        { typeof(DBNull), DBNullImpl.Instance },
        { typeof(Unit), UnitImpl<Unit>.Instance },
    };

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
        InstanceCache.Add(type, r!);
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
