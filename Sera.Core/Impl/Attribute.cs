using System;

namespace Sera;

#region Generator

/// <summary>Mark auto-generated serialize</summary>
[AttributeUsage(AttributeTargets.All, Inherited = false)]
public sealed class SerializableAttribute : Attribute
{
    public bool NoSync { get; set; } = false;
    public bool NoAsync { get; set; } = false;
}

/// <summary>Mark auto-generated deserialize</summary>
[AttributeUsage(AttributeTargets.All, Inherited = false)]
public sealed class DeserializableAttribute : Attribute
{
    public bool NoSync { get; set; } = false;
    public bool NoAsync { get; set; } = false;
}

#endregion

#region Sync

[AttributeUsage(AttributeTargets.All)]
public class SerializeAttribute : Attribute
{
    public Type ImplType { get; }

    public SerializeAttribute(Type implType)
    {
        ImplType = implType;
    }
}

[AttributeUsage(AttributeTargets.All)]
public class SerializeAttribute<T> : SerializeAttribute
{
    public SerializeAttribute() : base(typeof(T)) { }
}

[AttributeUsage(AttributeTargets.All)]
public class DeserializeAttribute : Attribute
{
    public Type ImplType { get; }

    public DeserializeAttribute(Type implType)
    {
        ImplType = implType;
    }
}

[AttributeUsage(AttributeTargets.All)]
public class DeserializeAttribute<T> : DeserializeAttribute
{
    public DeserializeAttribute() : base(typeof(T)) { }
}

#endregion

#region Async

[AttributeUsage(AttributeTargets.All)]
public class AsyncSerializeAttribute : Attribute
{
    public Type ImplType { get; }

    public AsyncSerializeAttribute(Type implType)
    {
        ImplType = implType;
    }
}

[AttributeUsage(AttributeTargets.All)]
public class AsyncSerializeAttribute<T> : AsyncSerializeAttribute
{
    public AsyncSerializeAttribute() : base(typeof(T)) { }
}

[AttributeUsage(AttributeTargets.All)]
public class AsyncDeserializeAttribute : Attribute
{
    public Type ImplType { get; }

    public AsyncDeserializeAttribute(Type implType)
    {
        ImplType = implType;
    }
}

[AttributeUsage(AttributeTargets.All)]
public class AsyncDeserializeAttribute<T> : AsyncDeserializeAttribute
{
    public AsyncDeserializeAttribute() : base(typeof(T)) { }
}

#endregion
