using System;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace Sera.Runtime.Utils;

public static class StructReflectionUtils
{
    public static bool IsSkipped(MemberInfo m, SerOrDe ser_or_de)
    {
        var sera_ignore_attr = m.GetCustomAttribute<SeraIgnoreAttribute>();
        if (ser_or_de is SerOrDe.Ser)
        {
            if (sera_ignore_attr?.Ser ?? false) return true;
        }
        else
        {
            if (sera_ignore_attr?.De ?? false) return true;
        }

        var ignore_data_member_attr = m.GetCustomAttribute<IgnoreDataMemberAttribute>();
        if (ignore_data_member_attr != null) return true;

        var non_serialized_attr = m.GetCustomAttribute<NonSerializedAttribute>();
        return non_serialized_attr != null;
    }

    public static (string name, long? int_key) GetName(
        MemberInfo m,
        SeraAttribute? member_sera_attr,
        SeraAttribute? struct_sera_attr,
        SeraFieldKeyAttribute? key_attr
    )
    {
        // todo auto rename
        var name = member_sera_attr?.Name ?? m.Name;
        var int_key = key_attr?.Key;
        return (name, int_key);
    }

    public static StructMember[] GetStructMembers(Type target, SerOrDe ser_or_de, SeraAttribute? struct_sera_attr, SeraStructAttribute? struct_attr)
    {
        var members = target.GetMembers(
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
        );
        return members.AsParallel()
            .AsOrdered()
            .Select(m =>
            {
                if (m is PropertyInfo p)
                {
                    var get_method = p.GetMethod;
                    if (get_method == null) return null;

                    var args = get_method.GetParameters();
                    if (args.Length > 0) return null;

                    var skip = IsSkipped(p, ser_or_de);
                    var member_sera_attr = p.GetCustomAttribute<SeraAttribute>();
                    var sera_include_attr = p.GetCustomAttribute<SeraIncludeAttribute>();
                    var key_attr = p.GetCustomAttribute<SeraFieldKeyAttribute>();
                    var include = get_method.IsPublic || (sera_include_attr?.Ser ?? false);
                    if (skip || !include) return null;

                    var (name, int_key) = GetName(p, member_sera_attr, struct_sera_attr, key_attr);

                    var type = p.PropertyType;
                    return new StructMember
                    {
                        MemberSeraAttr = member_sera_attr,
                        Name = name,
                        IntKey = int_key,
                        Property = p,
                        Kind = PropertyOrField.Property,
                        Type = type,
                    };
                }
                else if (m is FieldInfo f)
                {
                    var skip = IsSkipped(f, ser_or_de);
                    var member_sera_attr = f.GetCustomAttribute<SeraAttribute>();
                    var sera_include_attr = f.GetCustomAttribute<SeraIncludeAttribute>();
                    var key_attr = f.GetCustomAttribute<SeraFieldKeyAttribute>();
                    var include = ((struct_attr?.IncludeFields ?? false) && f.IsPublic) ||
                                  (sera_include_attr?.Ser ?? false);
                    if (skip || !include) return null;

                    var (name, int_key) = GetName(f, member_sera_attr, struct_sera_attr, key_attr);

                    var type = f.FieldType;
                    return new StructMember
                    {
                        MemberSeraAttr = member_sera_attr,
                        Name = name,
                        IntKey = int_key,
                        Field = f,
                        Kind = PropertyOrField.Field,
                        Type = type,
                    };
                }
                else return null;
            })
            .Where(a => a != null)
            .Where(a => a is not { Type: { IsByRef: true } or { IsByRefLike: true } }) // todo support span
            .ToArray()!;
    }
}

public record StructMember
{
    public SeraAttribute? MemberSeraAttr { get; set; }
    public required string Name { get; set; }
    public long? IntKey { get; set; }
    public required Type Type { get; set; }
    public PropertyInfo? Property { get; set; }
    public FieldInfo? Field { get; set; }
    public PropertyOrField Kind { get; set; }
}

public enum PropertyOrField
{
    Property,
    Field,
}

public enum SerOrDe : sbyte
{
    Ser,
    De = -1,
}
