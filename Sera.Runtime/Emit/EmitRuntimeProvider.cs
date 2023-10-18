using System;
using Sera.Core;
using Sera.Runtime.Emit.Ser;

namespace Sera.Runtime.Emit;

public class EmitRuntimeProvider : IRuntimeProvider
{
    public static EmitRuntimeProvider Instance { get; } = new();

    #region Serialize

    internal readonly SerializeEmitProvider serializeEmitProvider = new();

    public ISerialize<object?> GetRuntimeSerialize()
    {
        throw new NotImplementedException("todo");
    }

    public bool TryGetRuntimeSerialize(out ISerialize<object?> serialize)
    {
        serialize = GetRuntimeSerialize();
        return true;
    }

    public ISerialize<T> GetSerialize<T>() => GetSerialize<T>(SeraHints.Default);

    public ISerialize<T> GetSerialize<T>(SeraHints hints)
    {
        if (StaticRuntimeProvider.Instance.TryGetSerialize<T>(out var ser)) return ser;
        return serializeEmitProvider.GetSerialize<T>(hints);
    }

    public bool TryGetSerialize<T>(out ISerialize<T> serialize) =>
        TryGetSerialize(SeraHints.Default, out serialize);

    public bool TryGetSerialize<T>(SeraHints hints, out ISerialize<T> serialize)
    {
        serialize = GetSerialize<T>(hints);
        return true;
    }

    #endregion

    #region Deserialize

    public IDeserialize<object?> GetRuntimeDeserialize()
    {
        throw new System.NotImplementedException();
    }

    public bool TryGetRuntimeDeserialize(out IDeserialize<object?> deserialize)
    {
        throw new System.NotImplementedException();
    }

    public IDeserialize<T> GetDeserialize<T>()
    {
        throw new System.NotImplementedException();
    }

    public bool TryGetDeserialize<T>(out IDeserialize<T> deserialize)
    {
        throw new System.NotImplementedException();
    }

    #endregion
}
