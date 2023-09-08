using System;

namespace Sera;

#region Generator

/// <summary>Mark auto-generated serialize and deserialize</summary>
[AttributeUsage(AttributeTargets.Struct | AttributeTargets.Class | AttributeTargets.Enum, Inherited = false)]
public sealed class SeraGenAttribute : Attribute
{
    public bool NoSync { get; set; } = false;
    public bool NoAsync { get; set; } = false;
}

/// <summary>Mark auto-generated serialize</summary>
[AttributeUsage(AttributeTargets.Struct | AttributeTargets.Class | AttributeTargets.Enum, Inherited = false)]
public sealed class SeraGenSerAttribute : Attribute
{
    public bool NoSync { get; set; } = false;
    public bool NoAsync { get; set; } = false;
}

/// <summary>Mark auto-generated deserialize</summary>
[AttributeUsage(AttributeTargets.Struct | AttributeTargets.Class | AttributeTargets.Enum, Inherited = false)]
public sealed class SeraGenDeAttribute : Attribute
{
    public bool NoSync { get; set; } = false;
    public bool NoAsync { get; set; } = false;
}

/// <summary>Mark fields should be included when (de)serializing</summary>
[AttributeUsage(AttributeTargets.Struct | AttributeTargets.Class)]
public sealed class SeraIncludeFieldAttribute : Attribute { }

/// <summary>Mark this member should be ignored when (de)serializing</summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public sealed class SeraIgnoreAttribute : Attribute
{
    /// <summary>
    /// Ignore when serializing
    /// </summary>
    public bool Ser { get; set; } = true;
    /// <summary>
    /// Ignore when deserializing
    /// </summary>
    public bool De { get; set; } = true;

    public SeraIgnoreAttribute() { }

    /// <param name="ser">Ignore when serializing</param>
    /// <param name="de">Ignore when deserializing</param>
    public SeraIgnoreAttribute(bool ser, bool de)
    {
        Ser = ser;
        De = de;
    }
}

/// <summary>Mark this member should be included when (de)serialization</summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public sealed class SeraIncludeAttribute : Attribute
{
    /// <summary>
    /// Include when serializing
    /// </summary>
    public bool Ser { get; set; } = true;
    /// <summary>
    /// Include when deserializing
    /// </summary>
    public bool De { get; set; } = true;

    public SeraIncludeAttribute() { }

    /// <param name="ser">Include when serializing</param>
    /// <param name="de">Include when deserializing</param>
    public SeraIncludeAttribute(bool ser, bool de)
    {
        Ser = ser;
        De = de;
    }
}

/// <summary>Mark fields should be included when (de)serializing</summary>
[AttributeUsage(AttributeTargets.Struct | AttributeTargets.Class | AttributeTargets.Enum
                | AttributeTargets.Property | AttributeTargets.Field)]
public sealed class SeraRenameAttribute : Attribute
{
    public string? Name { get; set; }
    public long? IntKey { get; set; }

    public string? SerName { get; set; }
    public long? SerIntKey { get; set; }

    public string? DeName { get; set; }
    public long? DeIntKey { get; set; }

    public SeraRenameAttribute() { }

    public SeraRenameAttribute(string name)
    {
        Name = name;
    }

    public SeraRenameAttribute(long intKey)
    {
        IntKey = intKey;
    }

    public SeraRenameAttribute(string name, long intKey)
    {
        Name = name;
        IntKey = intKey;
    }
}

/// <summary>Mark fields should be included when (de)serializing</summary>
[AttributeUsage(AttributeTargets.Struct | AttributeTargets.Class | AttributeTargets.Enum
                | AttributeTargets.Property | AttributeTargets.Field)]
public sealed class SeraAutoRenameAttribute : Attribute
{
    public SeraRenameMode? Mode { get; set; }

    public SeraRenameMode? SerMode { get; set; }
    public SeraRenameMode? DeMode { get; set; }

    public SeraAutoRenameAttribute() { }

    public SeraAutoRenameAttribute(SeraRenameMode mode)
    {
        Mode = mode;
    }
}

public enum SeraRenameMode
{
    /// <summary>Pascal case <c>PascalCase</c> </summary>
    PascalCase,
    /// <summary> Camel case <c>camelCase</c> </summary>
    camelCase,
    /// <summary> Lower case <c>lowercase</c> </summary>
    lowercase,
    /// <summary> Upper case <c>UPPERCASE</c> </summary>
    UPPERCASE,
    /// <summary> Snake case <c>snake_case</c> </summary>
    snake_case,
    /// <summary> Screaming snake case <c>SCREAMING_SNAKE_CASE</c> </summary>
    SCREAMING_SNAKE_CASE,
    /// <summary> Kebab case <c>kebab-case</c> </summary>
    kebab_case,
    /// <summary> Screaming kebab case <c>SCREAMING-KEBAB-CASE</c> </summary>
    SCREAMING_KEBAB_CASE,
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
