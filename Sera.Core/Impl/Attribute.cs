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

/// <summary>Mark fields should be included when (de)serializing</summary>
[AttributeUsage(AttributeTargets.All)]
public sealed class SeraIncludeFieldAttribute : Attribute { }

/// <summary>Mark this member should be ignored when (de)serializing</summary>
[AttributeUsage(AttributeTargets.All)]
public sealed class SeraIgnoreAttribute : Attribute
{
    public bool Ser { get; set; } = true;
    public bool De { get; set; } = true;

    public SeraIgnoreAttribute() { }

    public SeraIgnoreAttribute(bool ser, bool de)
    {
        Ser = ser;
        De = de;
    }
}

/// <summary>Prompts when generating implementation</summary>
[AttributeUsage(AttributeTargets.All)]
public sealed class SeraAttribute : Attribute
{
    /// <summary>
    /// Specify the field name
    /// </summary>
    public string? Name { get; set; }
    /// <summary>
    /// Specify the field int id
    /// </summary>
    public long? IntKey { get; set; }

    public SeraAttribute() { }

    public SeraAttribute(string name)
    {
        Name = name;
    }

    public SeraAttribute(long intKey)
    {
        IntKey = intKey;
    }

    public SeraAttribute(string name, long intKey)
    {
        Name = name;
        IntKey = intKey;
    }
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
