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

public interface IAsyncRuntimeProvider
{
    /// <summary>Get the serializer dynamically</summary>
    public IAsyncSerialize<object?> GetRuntimeAsyncSerialize();
    
    /// <summary>Get the serializer dynamically</summary>
    public bool TryGetRuntimeAsyncSerialize(out IAsyncSerialize<object?> serialize);

    /// <summary>Get the serializer dynamically</summary>
    public IAsyncSerialize<T> GetAsyncSerialize<T>();
    
    /// <summary>Get the serializer dynamically</summary>
    public bool TryGetAsyncSerialize<T>(out IAsyncSerialize<T> serialize);

    /// <summary>Get the deserializer dynamically</summary>
    public IAsyncDeserialize<object?> GetRuntimeAsyncDeserialize();
    
    /// <summary>Get the deserializer dynamically</summary>
    public bool TryGetRuntimeAsyncDeserialize(out IAsyncDeserialize<object?> deserialize);
    
    /// <summary>Get the deserializer dynamically</summary>
    public IAsyncDeserialize<T> GetAsyncDeserialize<T>();
    
    /// <summary>Get the deserializer dynamically</summary>
    public bool TryGetAsyncDeserialize<T>(out IAsyncDeserialize<T> deserialize);
}
