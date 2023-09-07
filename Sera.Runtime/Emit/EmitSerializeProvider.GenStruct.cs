using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using Sera.Core;
using Sera.Runtime.Utils;

namespace Sera.Runtime.Emit;

internal partial class EmitSerializeProvider
{
    public StructMember[] GetStructMembers(Type target)
    {
        var members = target.GetMembers(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic |
                                        BindingFlags.GetProperty | BindingFlags.SetProperty);
        return members.AsParallel()
            .AsOrdered()
            .Select(m =>
            {
                if (m is PropertyInfo p)
                {
                    var get_method = p.GetMethod;
                    if (get_method == null) return null;
                    if (!get_method.IsPublic) return null; // todo pass private by attr
                    var name = p.Name; // todo change name by attr
                    var type = p.PropertyType;
                    return new StructMember
                    {
                        Name = name,
                        Property = p,
                        Kind = PropertyOrField.Property,
                        Type = type,
                    };
                }
                else if (m is FieldInfo f)
                {
                    if (!f.IsPublic) return null; // todo pass private by attr
                    var name = f.Name; // todo change name by attr
                    var type = f.FieldType;
                    return new StructMember
                    {
                        Name = name,
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
        public string Name { get; set; }
        public Type Type { get; set; }
        public PropertyInfo? Property { get; set; }
        public FieldInfo? Field { get; set; }
        public PropertyOrField Kind { get; set; }
    }

    internal enum PropertyOrField
    {
        Property,
        Field,
    }

    private (Type impl_type, CacheCell? cell, object? impl) GetImpl(Type target, Thread thread)
    {
        Type impl_type;
        CacheCell? cell;
        if (TryGetStaticImpl(target, out var impl))
        {
            impl_type = impl!.GetType();
            cell = null;
        }
        else
        {
            cell = GetSerialize(target, thread);
            if (cell.CreateThread != thread)
            {
                cell.WaitType.WaitOne();
            }
            impl_type = cell.ser_type!;
        }
        return (impl_type, cell, impl);
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
