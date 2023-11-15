using System;

namespace Sera.Utils;

/// <summary>
/// <code>
/// // ? is optional
/// [AssocType(name?)]
/// public abstract class Name(TheType type, OfInterface of?)
/// </code>
/// </summary>
[AttributeUsage(AttributeTargets.GenericParameter | AttributeTargets.Class)]
public sealed class AssocTypeAttribute : Attribute
{
    public string? Name { get; }

    public AssocTypeAttribute() { }

    public AssocTypeAttribute(string? name = null)
    {
        Name = name;
    }
}

