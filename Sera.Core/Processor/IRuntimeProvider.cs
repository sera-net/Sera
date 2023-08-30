using System;

namespace Sera.Core;

public interface IRuntimeProvider
{
    /// <summary>Get the serializer dynamically</summary>
    public ISerialize<object?> GetRuntimeSerialize();
    
    /// <summary>Get the serializer dynamically</summary>
    public bool TryGetRuntimeSerialize(out ISerialize<object?> serialize);

    /// <summary>Get the serializer dynamically</summary>
    public ISerialize<T> GetSerialize<T>();
    
    /// <summary>Get the serializer dynamically</summary>
    public bool TryGetSerialize<T>(out ISerialize<T> serialize);

    /// <summary>Get the deserializer dynamically</summary>
    public IDeserialize<object?> GetRuntimeDeserialize();
    
    /// <summary>Get the deserializer dynamically</summary>
    public bool TryGetRuntimeDeserialize(out IDeserialize<object?> deserialize);
    
    /// <summary>Get the deserializer dynamically</summary>
    public IDeserialize<T> GetDeserialize<T>();
    
    /// <summary>Get the deserializer dynamically</summary>
    public bool TryGetDeserialize<T>(out IDeserialize<T> deserialize);
}
