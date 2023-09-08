using System;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Threading;
using Sera.Core;
using Sera.Runtime.Utils;

namespace Sera.Runtime.Emit;

internal partial class EmitSerializeProvider
{
    public static bool IsSkipped(MemberInfo m, bool ser)
    {
        var sera_ignore_attr = m.GetCustomAttribute<SeraIgnoreAttribute>();
        if (ser)
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

    public static (string name, long? int_key) GetName(MemberInfo m, bool ser)
    {
        // todo auto rename
        var sera_rename_attr = m.GetCustomAttribute<SeraRenameAttribute>();
        var name = (ser ? sera_rename_attr?.SerName : sera_rename_attr?.DeName) ?? sera_rename_attr?.Name ?? m.Name;
        var int_key = (ser ? sera_rename_attr?.SerIntKey : sera_rename_attr?.DeIntKey) ?? sera_rename_attr?.IntKey;
        return (name, int_key);
    }

    public StructMember[] GetStructMembers(Type target, bool ser)
    {
        var include_field_attr = target.GetCustomAttribute<SeraIncludeFieldAttribute>();

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

                    var skip = IsSkipped(p, ser);
                    var sera_include_attr = p.GetCustomAttribute<SeraIncludeAttribute>();
                    var include = get_method.IsPublic || (sera_include_attr?.Ser ?? false);
                    if (skip || !include) return null;

                    var (name, int_key) = GetName(p, ser);

                    var type = p.PropertyType;
                    return new StructMember
                    {
                        Name = name,
                        IntKey = int_key,
                        Property = p,
                        Kind = PropertyOrField.Property,
                        Type = type,
                    };
                }
                else if (m is FieldInfo f)
                {
                    var skip = IsSkipped(f, ser);
                    var sera_include_attr = f.GetCustomAttribute<SeraIncludeAttribute>();
                    var include = (include_field_attr != null && f.IsPublic) || (sera_include_attr?.Ser ?? false);
                    if (skip || !include) return null;

                    var (name, int_key) = GetName(f, ser);

                    var type = f.FieldType;
                    return new StructMember
                    {
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
            .ToArray()!;
    }

    internal record StructMember
    {
        public required string Name { get; set; }
        public long? IntKey { get; set; }
        public required Type Type { get; set; }
        public PropertyInfo? Property { get; set; }
        public FieldInfo? Field { get; set; }
        public PropertyOrField Kind { get; set; }
    }

    internal enum PropertyOrField
    {
        Property,
        Field,
    }

    /// <returns>Actual type: (Type, CacheStub | object)</returns>
    private (Type impl_type, CacheStub? stub, object? impl) GetImpl(Type target, Thread thread)
    {
        Type impl_type;
        CacheStub? stub;
        if (TryGetStaticImpl(target, out var impl))
        {
            impl_type = impl!.GetType();
            stub = null;
        }
        else
        {
            stub = GetSerializeStub(target, thread);
            if (stub.CreateThread != thread)
            {
                stub.WaitType.WaitOne();
            }
            impl_type = stub.ser_type!;
        }
        return (impl_type, stub, impl);
    }

    private bool TryGetStaticImpl(Type type, out object? impl)
    {
        var method = ReflectionUtils.StaticRuntimeProvider_TryGetSerialize.MakeGenericMethod(type);
        var args = new object?[] { null };
        var r = (bool)method.Invoke(StaticRuntimeProvider.Instance, args)!;
        impl = args[0];
        return r;
    }
}
