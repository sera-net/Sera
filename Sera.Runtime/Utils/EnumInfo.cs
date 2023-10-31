using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using Sera.Core;

namespace Sera.Runtime.Utils;

internal record EnumInfo(string Name, VariantTag Tag, FieldInfo Field, SeraUnionAttribute? EnumAttr);

internal record struct EnumJumpTable(int Index, EnumInfo Info);

internal record EnumJumpTables(int Offset, EnumJumpTable[] Table);

internal static class EnumUtils
{
    public static EnumInfo[] GetEnumInfo(Type target, Type underlying_type, bool distinct)
    {
        var method = _GetEnumInfo_MethodInfo.MakeGenericMethod(underlying_type);
        return (EnumInfo[])method.Invoke(null, new object?[] { target, distinct })!;
    }

    private static readonly MethodInfo _GetEnumInfo_MethodInfo = typeof(EnumUtils)
        .GetMethod(nameof(_GetEnumInfo), BindingFlags.Static | BindingFlags.NonPublic)!;

    private static EnumInfo[] _GetEnumInfo<V>(Type target, bool distinct)
    {
        var names_set = target.GetEnumNames().ToHashSet();
        var fields = target.GetFields(BindingFlags.Static | BindingFlags.Public)
            .AsParallel().AsOrdered()
            .Where(a => names_set.Contains(a.Name))
            .ToDictionary(a => a.Name);

        var q = fields.AsParallel().AsOrdered()
            .Select((kv, i) => (name: kv.Key, field: kv.Value))
            .Select(a => (a.name, a.field, value: (V)a.field.GetValue(null)!))
            .Select(a =>
            {
                var enum_attr = a.field.GetCustomAttribute<SeraUnionAttribute>();
                var rename_attr = a.field.GetCustomAttribute<SeraRenameAttribute>();
                var name = rename_attr?.SerName ?? rename_attr?.Name ?? a.name; // todo auto rename
                return new EnumInfo(name, a.value.MakeVariantTag(), a.field, enum_attr);
            });
        if (distinct)
            return q
                .DistinctBy(a => a.Tag)
                .ToArray();
        else return q.ToArray();
    }
    
    public static EnumJumpTables? TryMakeJumpTable(Type underlying_type, EnumInfo[] items)
    {
        if (items.Length == 0) return null;
        var method = _TryMakeJumpTable_MethodInfo.MakeGenericMethod(underlying_type);
        return (EnumJumpTables?)method.Invoke(null, new object?[] { items });
    }

    private static readonly MethodInfo _TryMakeJumpTable_MethodInfo = typeof(EnumUtils)
        .GetMethod(nameof(_TryMakeJumpTable), BindingFlags.Static | BindingFlags.NonPublic)!;

    private const int MaxJumpTableSizeAllowed = 255;

    private static EnumJumpTables? _TryMakeJumpTable<V>(EnumInfo[] items)
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
}
