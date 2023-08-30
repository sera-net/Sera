using System;

namespace Sera;

/// <summary>Mark auto-generated serialize</summary>
[AttributeUsage(AttributeTargets.All, Inherited = false)]
public sealed class SerializableAttribute : Attribute { }

/// <summary>Mark auto-generated deserialize</summary>
[AttributeUsage(AttributeTargets.All, Inherited = false)]
public sealed class DeserializableAttribute : Attribute { }

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
