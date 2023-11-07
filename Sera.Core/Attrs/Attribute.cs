using System;
using System.Collections.Generic;
using Sera.Core;
using Sera.Core.Formats;

namespace Sera;

#region Generator

/// <summary>Mark auto-generated serialize and deserialize</summary>
[AttributeUsage(AttributeTargets.Struct | AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Enum,
    Inherited = false)]
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
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface | AttributeTargets.Enum |
                AttributeTargets.Field | AttributeTargets.Property)]
public sealed class SeraAttribute : Attribute
{
    /// <summary>Specify special semantics, only for field | property</summary>
    public SeraAs As { get; set; } = SeraAs.None;
    /// <summary>Rename fields | struct name</summary>
    public string? Name { get; set; }
    /// <summary>Mark auto rename mode</summary>
    public SeraRenameMode Rename { get; set; }
}

/// <summary>Specify special semantics</summary>
public enum SeraAs : byte
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
    /// <summary>Mark ignore like <see cref="IEnumerable{T}"/> or <see cref="IDictionary{K,V}"/></summary>
    Struct,
}

/// <summary>Auto renaming mode</summary>
public enum SeraRenameMode : byte
{
    /// <summary>Ignore</summary>
    None,
    /// <summary>Don't rename</summary>
    Dont,
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

[AttributeUsage(AttributeTargets.Struct | AttributeTargets.Class)]
public sealed class SeraStructAttribute : Attribute
{
    /// <summary>Mark fields should be included when (de)serializing</summary>
    public bool IncludeFields { get; set; }
}

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

/// <summary>Rename fields int key</summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public sealed class SeraFieldKeyAttribute(long key) : Attribute
{
    /// <summary>Rename fields int key</summary>
    public long Key { get; } = key;
}

[AttributeUsage(AttributeTargets.Struct | AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Enum |
                AttributeTargets.Property | AttributeTargets.Field)]
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

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public sealed class SeraVariantAttribute : Attribute
{
    public VariantPriority Priority { get; set; } = VariantPriority.Any;
    public VariantFormat Format { get; set; } = VariantFormat.None;

    public SeraVariantAttribute() { }

    public SeraVariantAttribute(VariantPriority priority)
    {
        Priority = priority;
    }
}

public enum VariantFormat
{
    None,
    /// <summary>
    /// If f# union has only one variant and variant name is Item then this is the default
    /// <code language="fs">type A = A of int</code>
    /// <code language="js">{ "A": 123 }</code>
    /// </summary>
    Value,
    /// <summary>
    /// Only for f# union; If the name of each variant in f# union is ItemN then this is the default
    /// <code language="fs">type A = A of int</code>
    /// <code language="js">{ "A": [123] }</code>
    /// </summary>
    Tuple,
    /// <summary>
    /// Only for f# union; The default of f #union in other cases
    /// <code language="fs">type A = A of a: int</code>
    /// <code language="js">{ "A": { "a": 123 } }</code>
    /// </summary>
    Struct,
}

[AttributeUsage(AttributeTargets.Enum | AttributeTargets.Struct | AttributeTargets.Class | AttributeTargets.Interface)]
public class SeraUnionAttribute : Attribute
{
    public SeraUnionMode Mode { get; set; }
    /// <summary>Variant tag priority</summary>
    public VariantPriority Priority { get; set; } = VariantPriority.Any;
    /// <summary>Union format</summary>
    public UnionFormat Format { get; set; } = UnionFormat.Any;
    /// <summary>Whether to treat the variant as a tag when it has no value when the format is <see cref="UnionFormat.Internal"/> | <see cref="UnionFormat.Adjacent"/> | <see cref="UnionFormat.Tuple"/></summary>
    public bool CompactTag { get; set; } = false;
    /// <summary>The name of the tag when the format is <see cref="UnionFormat.Internal"/></summary>
    public string InternalTagName { get; set; } = "type";
    /// <summary>Field name if the variant value cannot be treated as a structure when the format is <see cref="UnionFormat.Internal"/></summary>
    public string InternalValueName { get; set; } = "value";
    /// <summary>The name of the tag when the format is <see cref="UnionFormat.Adjacent"/></summary>
    public string AdjacentTagName { get; set; } = "t";
    /// <summary>The name of the value when the format is <see cref="UnionFormat.Adjacent"/></summary>
    public string AdjacentValueName { get; set; } = "c";
}

/// <summary>
/// Whether to exhaustively match
/// <para>F# union is always <see cref="Exhaustive"/></para>
/// </summary>
[SeraUnion(Mode = Exhaustive)]
public enum SeraUnionMode : byte
{
    None,
    /// <summary>No more variants will be added in the future</summary>
    Exhaustive,
    /// <summary>More variations may be added in the future <para>C# enum default value</para></summary>
    NonExhaustive,
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

public enum SeraFlagsMode : byte
{
    /// <summary>
    /// As a <see cref="string"/> seq, flags without names will be ignored, default
    /// </summary>
    Seq,
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

[AttributeUsage(AttributeTargets.Struct | AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Enum |
                AttributeTargets.Field | AttributeTargets.Property)]
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
