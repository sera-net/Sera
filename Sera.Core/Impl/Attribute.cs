using System;
using System.Collections.Generic;
using Sera.Core;
using Sera.Core.Formats;

namespace Sera;

#region Generator

/// <summary>Mark auto-generated serialize and deserialize</summary>
[AttributeUsage(AttributeTargets.Struct | AttributeTargets.Class | AttributeTargets.Enum, Inherited = false)]
public sealed class SeraGenAttribute : Attribute
{
    public bool NoSer { get; set; }
    public bool NoDe { get; set; }
}

/// <summary>Mark auto-generated serialize and deserialize</summary>
[AttributeUsage(AttributeTargets.Assembly)]
public class SeraGenForAttribute : Attribute
{
    public SeraGenForAttribute(Type target)
    {
        Target = target;
    }

    public Type Target { get; set; }

    public bool NoSer { get; set; }
    public bool NoDe { get; set; }
}

/// <summary>Mark auto-generated serialize and deserialize</summary>
[AttributeUsage(AttributeTargets.Assembly)]
public sealed class SeraGenForAttribute<T>() : SeraGenForAttribute(typeof(T)) { }

/// <summary>Misc options</summary>
[AttributeUsage(AttributeTargets.All)]
public sealed class SeraAttribute : Attribute
{
    /// <summary>Specify special semantics, only for field | property</summary>
    public SeraAs As { get; set; } = SeraAs.None;
}

/// <summary>Specify special semantics</summary>
public enum SeraAs
{
    /// <summary>No operation</summary>
    None,
    /// <summary>Mark byte array | list | memory ... is bytes semantics</summary>
    Bytes,
    /// <summary>Mark char array | list | memory ... is string semantics</summary>
    String,
    /// <summary>Mark <see cref="IDictionary{K,V}"/> is seq semantics</summary>
    Seq,
    /// <summary>Mark <see cref="IEnumerable{T}"/> of <see cref="KeyValuePair{K,V}"/> is map semantics</summary>
    Map,
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

[AttributeUsage(AttributeTargets.All)]
public sealed class SeraFormatsAttribute : Attribute
{
    public bool BooleanAsNumber { get; set; }

    public ToUpperOrLower ToUpperOrLower { get; set; } = ToUpperOrLower.None;

    public NumberTextFormat NumberTextFormat { get; set; } = NumberTextFormat.Any;
    public string? CustomNumberTextFormat { get; set; }

    public DateTimeFormatFlags DateTimeFormat { get; set; } = DateTimeFormatFlags.None;

    public GuidTextFormat GuidTextFormat { get; set; } = GuidTextFormat.Any;
    public GuidBinaryFormat GuidBinaryFormat { get; set; } = GuidBinaryFormat.Any;
    public string? CustomGuidTextFormat { get; set; }
}

[AttributeUsage(AttributeTargets.Field)]
public sealed class SeraVariantAttribute : Attribute
{
    public VariantPriority Priority { get; set; } = VariantPriority.Any;

    public SeraVariantAttribute() { }

    public SeraVariantAttribute(VariantPriority priority)
    {
        Priority = priority;
    }
}

[AttributeUsage(AttributeTargets.Enum | AttributeTargets.Struct | AttributeTargets.Class)]
public class SeraUnionAttribute : Attribute
{
    public VariantPriority Priority { get; set; } = VariantPriority.Any;
    public UnionFormat Format { get; set; } = UnionFormat.Any;
    public string InternalTagName { get; set; } = "type";
    public string AdjacentTagName { get; set; } = "t";
    public string AdjacentValueName { get; set; } = "c";
}

[AttributeUsage(AttributeTargets.Enum)]
public sealed class SeraFlagsAttribute : Attribute
{
    public SeraFlagsMode Mode { get; }

    public SeraFlagsAttribute(SeraFlagsMode mode)
    {
        Mode = mode;
    }

    public SeraFlagsAttribute() { }
}

public enum SeraFlagsMode
{
    /// <summary>
    /// As <see cref="string"/> array, flags without names will be ignored， default
    /// </summary>
    Array,
    /// <summary>
    /// As underlying number
    /// </summary>
    Number,
    /// <summary>
    /// Use <see cref="Enum.ToString()"/>, ignore member rename
    /// </summary>
    String,
    /// <summary>
    /// Use <see cref="Enum.ToString()"/>, then split by <c>", "</c>, as <see cref="string"/> array, ignore member rename
    /// </summary>
    StringSplit,
}

#endregion

#region Mark

[AttributeUsage(AttributeTargets.All)]
public class SeraVisionByAttribute : Attribute
{
    public SeraVisionByAttribute(string methodName)
    {
        TargetType = null;
        MethodName = methodName;
    }

    public SeraVisionByAttribute(Type targetType, string methodName)
    {
        TargetType = targetType;
        MethodName = methodName;
    }

    /// <summary>
    /// The type where the static method is located, null is the type where the attribute is located
    /// </summary>
    public Type? TargetType { get; }
    /// <summary>
    /// A static method to return <see cref="ISeraVision{T}"/>, the method allows at most one generic type, passing in the type of the target
    /// </summary>
    public string MethodName { get; }
}

#endregion
