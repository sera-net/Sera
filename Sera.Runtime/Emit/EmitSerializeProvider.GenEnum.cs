using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;
using Sera.Core;
using Sera.Runtime.Utils;

namespace Sera.Runtime.Emit;

internal partial class EmitSerializeProvider
{
    private void GenEnum(Type target, CacheStub stub)
    {
        var underlying_type = target.GetEnumUnderlyingType();
        var items = GetEnumInfo(target, underlying_type);
        var jump_table = TryMakeJumpTable(underlying_type, items);
        var flags = target.GetCustomAttribute<FlagsAttribute>() != null;
        if (flags)
        {
            if (target.IsVisible)
            {
                GenEnumFlagsPublic(target, underlying_type, items, jump_table, stub);
            }
            else
            {
                GenEnumFlagsPrivate(target, underlying_type, items, jump_table, stub);
            }
        }
        else
        {
            if (target.IsVisible)
            {
                GenEnumVariantPublic(target, underlying_type, items, jump_table, stub);
            }
            else
            {
                GenEnumVariantPrivate(target, underlying_type, items, jump_table, stub);
            }
        }
    }

    private EnumInfo[] GetEnumInfo(Type target, Type underlying_type)
    {
        var method = _GetEnumInfo_MethodInfo.MakeGenericMethod(underlying_type);
        return (EnumInfo[])method.Invoke(this, new object?[] { target })!;
    }

    private static readonly MethodInfo _GetEnumInfo_MethodInfo = typeof(EmitSerializeProvider)
        .GetMethod(nameof(_GetEnumInfo), BindingFlags.Instance | BindingFlags.NonPublic)!;

    private EnumInfo[] _GetEnumInfo<V>(Type target)
    {
        var names_set = target.GetEnumNames().ToHashSet();
        var fields = target.GetFields(BindingFlags.Static | BindingFlags.Public)
            .AsParallel().AsOrdered()
            .Where(a => names_set.Contains(a.Name))
            .ToDictionary(a => a.Name);

        return fields.AsParallel().AsOrdered()
            .Select((kv, i) => (name: kv.Key, field: kv.Value))
            .Select(a => (a.name, a.field, value: (V)a.field.GetValue(null)!))
            .Select(a =>
            {
                var enum_attr = a.field.GetCustomAttribute<SeraEnumAttribute>();
                return new EnumInfo(a.name, a.value.MakeVariantTag(), a.field, enum_attr);
            })
            .DistinctBy(a => a.Tag)
            .ToArray();
    }

    private record EnumInfo(string Name, VariantTag Tag, FieldInfo Field, SeraEnumAttribute? enum_attr);

    private EnumJumpTables? TryMakeJumpTable(Type underlying_type, EnumInfo[] items)
    {
        if (items.Length == 0) return null;
        var method = _TryMakeJumpTable_MethodInfo.MakeGenericMethod(underlying_type);
        return (EnumJumpTables?)method.Invoke(this, new object?[] { items });
    }

    private static readonly MethodInfo _TryMakeJumpTable_MethodInfo = typeof(EmitSerializeProvider)
        .GetMethod(nameof(_TryMakeJumpTable), BindingFlags.Instance | BindingFlags.NonPublic)!;

    private const int MaxJumpTableSizeAllowed = 255;

    private EnumJumpTables? _TryMakeJumpTable<V>(EnumInfo[] items)
        where V : unmanaged, INumber<V>
    {
        if (items.Length <= 1) return null;
        var values = items.AsParallel().AsOrdered()
            .Select((e, i) => (i, v: e.Tag.As<V>(), e))
            .OrderBy(a => a.v)
            .ToArray();
        var value_index_map = values.AsParallel()
            .ToDictionary(a => a.v, a => a.i);
        var min = values.First().v;
        var max = values.Last().v;
        var size = max.PrimitiveToInt128() - min.PrimitiveToInt128();
        if (size >= MaxJumpTableSizeAllowed) return null;
        var offset = 0 - min.PrimitiveToInt128();

        var last = values.Length - 1;
        var tables = new List<EnumJumpTable>();
        for (var v = max; v > min; v--)
        {
            last = value_index_map.TryGetValue(v, out var last1) ? last1 : last;
            tables.Add(new EnumJumpTable(last, values[last].e));
        }
        tables.Add(new EnumJumpTable(0, values[0].e));
        tables.Reverse();

        return new EnumJumpTables((int)offset, tables.ToArray());
    }

    private record struct EnumJumpTable(int Index, EnumInfo Info);

    private record EnumJumpTables(int Offset, EnumJumpTable[] Table);
}
