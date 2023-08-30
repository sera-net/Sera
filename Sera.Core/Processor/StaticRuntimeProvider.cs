using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Sera.Core.Impls;

namespace Sera.Core;

public class StaticRuntimeProvider : IRuntimeProvider
{
    public static StaticRuntimeProvider Instance { get; } = new();

    private static readonly ConditionalWeakTable<Type, object> SerializeCache = new();
    private static readonly ConditionalWeakTable<Type, object> DeserializeCache = new();

    #region Serialize

    public ISerialize<object?> GetRuntimeSerialize()
        => throw new NotSupportedException($"{nameof(StaticRuntimeProvider)} dose not support dynamic create");

    public bool TryGetRuntimeSerialize(out ISerialize<object?> serialize)
    {
        serialize = default!;
        return false;
    }

    public ISerialize<T> GetSerialize<T>()
    {
        var type = typeof(T);
        if (SerializeCache.TryGetValue(type, out var r)) return (ISerialize<T>)r;
        if (BuiltInRuntimeProvider.Instance.TryGetSerialize<T>(out var rr))
        {
            r = rr;
            goto add;
        }
        r = CreateSerialize(type, static () => SkipImpl<T>.Instance);
        add:
        SerializeCache.Add(type, r);
        return (ISerialize<T>)r;
    }

    public bool TryGetSerialize<T>(out ISerialize<T> serialize)
    {
        var type = typeof(T);
        if (SerializeCache.TryGetValue(type, out var r)) goto ret;
        if (BuiltInRuntimeProvider.Instance.TryGetSerialize<T>(out var rr))
        {
            r = rr;
            goto add;
        }
        r = CreateSerialize(type, static () => null!);
        add:
        SerializeCache.Add(type, r);
        ret:
        if (r == null!)
        {
            serialize = null!;
            return false;
        }
        else
        {
            serialize = (ISerialize<T>)r;
            return true;
        }
    }

    private object CreateSerialize(Type type, Func<object> Fallback)
    {
        var info = type.GetTypeInfo();
        var it = info.ImplementedInterfaces.Where(i =>
        {
            if (!i.IsGenericType) return false;
            var def = i.GetGenericTypeDefinition();
            if (def != typeof(ISerializable<,>)) return false;
            var target = i.GenericTypeArguments[0];
            return target.IsAssignableFrom(type);
        }).FirstOrDefault();
        if (it == null) return Fallback();
        var map = type.GetInterfaceMap(it);
        var index = map.InterfaceMethods
            .Select((a, i) => (a, i: (int?)i))
            .Where(a => a.a.HasSameMetadataDefinitionAs(SeraReflectionUtils.ISerializable_GetSerialize))
            .Select(a => a.i)
            .FirstOrDefault() ?? -1;
        if (index < 0) return Fallback();
        var method = map.TargetMethods[index];
        return method.Invoke(null, Array.Empty<object>())!;
    }

    #endregion

    #region Deserialize

    public IDeserialize<object?> GetRuntimeDeserialize()
        => throw new NotSupportedException($"{nameof(StaticRuntimeProvider)} dose not support dynamic create");

    public bool TryGetRuntimeDeserialize(out IDeserialize<object?> deserialize)
    {
        deserialize = default!;
        return false;
    }

    public IDeserialize<T> GetDeserialize<T>()
    {
        var type = typeof(T);
        if (DeserializeCache.TryGetValue(type, out var r)) return (IDeserialize<T>)r;
        if (BuiltInRuntimeProvider.Instance.TryGetDeserialize<T>(out var rr))
        {
            r = rr;
            goto add;
        }
        r = CreateDeserialize(type, static () => SkipImpl<T>.Instance);
        add:
        DeserializeCache.Add(type, r);
        return (IDeserialize<T>)r;
    }

    public bool TryGetDeserialize<T>(out IDeserialize<T> deserialize)
    {
        var type = typeof(T);
        if (DeserializeCache.TryGetValue(type, out var r)) goto ret;
        if (BuiltInRuntimeProvider.Instance.TryGetDeserialize<T>(out var rr))
        {
            r = rr;
            goto add;
        }
        r = CreateDeserialize(type, static () => null!);
        add:
        DeserializeCache.Add(type, r);
        ret:
        if (r == null!)
        {
            deserialize = null!;
            return false;
        }
        else
        {
            deserialize = (IDeserialize<T>)r;
            return true;
        }
    }

    private object CreateDeserialize(Type type, Func<object> Fallback)
    {
        var info = type.GetTypeInfo();
        var it = info.ImplementedInterfaces.Where(i =>
        {
            if (!i.IsGenericType) return false;
            var def = i.GetGenericTypeDefinition();
            if (def != typeof(IDeserializable<,>)) return false;
            var target = i.GenericTypeArguments[0];
            return target.IsAssignableFrom(type);
        }).FirstOrDefault();
        if (it == null) return Fallback();
        var map = type.GetInterfaceMap(it);
        var index = map.InterfaceMethods
            .Select((a, i) => (a, i: (int?)i))
            .Where(a => a.a.HasSameMetadataDefinitionAs(SeraReflectionUtils.IDeserializable_GetDeserialize))
            .Select(a => a.i)
            .FirstOrDefault() ?? -1;
        if (index < 0) return Fallback();
        var method = map.TargetMethods[index];
        return method.Invoke(null, Array.Empty<object>())!;
    }

    #endregion
}
