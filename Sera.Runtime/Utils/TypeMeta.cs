using System;
using System.Collections.Concurrent;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using Sera.Core;

namespace Sera.Runtime.Utils;

internal record struct TypeMeta(Type Type, GenericMeta? Generics, NullabilityMeta? Nullability);

internal record struct GenericMeta(
    Type[] RawTypes, TypeMeta[] Metas, NullabilityMeta[]? Nullabilities, int Length
)
{
    public static GenericMeta Create(params Type[] types) => new(types,
        types.Select(t => TypeMetas.GetTypeMeta(t)).ToArray(), null, types.Length);
}

internal record NullabilityMeta
{
    public static NullabilityMeta Empty { get; } = new((NullabilityInfo?)null);

    private Vector128<byte>[] Data { get; }

    private int Hash { get; }
    public NullabilityInfo? NullabilityInfo { get; }

    public NullabilityMeta(NullabilityInfo? nullabilityInfo)
    {
        NullabilityInfo = nullabilityInfo;
        Data = GenData(nullabilityInfo);
        Hash = GenHash(Data);
    }

    private static Vector128<byte>[] GenData(NullabilityInfo? NullabilityInfo)
    {
        if (NullabilityInfo == null) return Array.Empty<Vector128<byte>>();
        using var mem = new MemoryStream();
        using var zip = new BrotliStream(mem, CompressionLevel.Optimal);
        using var serer = new BytesSerializer(zip, DefaultSeraOptions.Default);
        new NullabilityInfoBinarySerializeImpl().Accept<Unit, BytesSerializer>(serer, NullabilityInfo);
        zip.Flush();
        mem.Position = 0;
        var len = (int)mem.Length;
        if (len == 0) return Array.Empty<Vector128<byte>>();
        var size = (len + 15) & ~15;
        var arr = GC.AllocateUninitializedArray<Vector128<byte>>(size / 16);
        var bytes = MemoryMarshal.AsBytes(arr.AsSpan());
        _ = mem.Read(bytes);
        var last_span = bytes[len..];
        last_span.Clear();
        return arr;
    }

    private static int GenHash(Vector128<byte>[] data)
    {
        var hash = new HashCode();
        var span = data.AsSpan();
        hash.AddBytes(MemoryMarshal.AsBytes(span));
        return hash.ToHashCode();
    }

    public virtual bool Equals(NullabilityMeta? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        var l = Data.AsSpan();
        var r = other.Data.AsSpan();
        return l.SequenceEqual(r);
    }

    public override int GetHashCode() => Hash;
}

internal static class TypeMetas
{
    #region Caches

    #region FieldInfoToNullabilityCache

    private static readonly ConditionalWeakTable<FieldInfo, NullabilityInfo>
        FieldInfoToNullabilityCache = new();

    private static NullabilityInfo GetCache(FieldInfo info)
        => FieldInfoToNullabilityCache.GetValue(info, i => new NullabilityInfoContext().Create(i));

    #endregion

    #region PropertyInfoToNullabilityCache

    private static readonly ConditionalWeakTable<PropertyInfo, NullabilityInfo>
        PropertyInfoToNullabilityCache = new();

    private static NullabilityInfo GetCache(PropertyInfo info)
        => PropertyInfoToNullabilityCache.GetValue(info, i => new NullabilityInfoContext().Create(i));

    #endregion

    #region ParameterInfoToNullabilityCache

    private static readonly ConditionalWeakTable<ParameterInfo, NullabilityInfo>
        ParameterInfoToNullabilityCache = new();

    private static NullabilityInfo GetCache(ParameterInfo info)
        => ParameterInfoToNullabilityCache.GetValue(info, i => new NullabilityInfoContext().Create(i));

    #endregion

    #region EventInfoToNullabilityCache

    private static readonly ConditionalWeakTable<EventInfo, NullabilityInfo>
        EventInfoToNullabilityCache = new();

    private static NullabilityInfo GetCache(EventInfo info)
        => EventInfoToNullabilityCache.GetValue(info, i => new NullabilityInfoContext().Create(i));

    #endregion

    #region NullabilityInfoToNullabilityMetaCache

    private static readonly ConditionalWeakTable<NullabilityInfo, NullabilityMeta>
        NullabilityInfoToNullabilityMetaCache = new();

    private static NullabilityMeta GetCache(NullabilityInfo info)
        => NullabilityInfoToNullabilityMetaCache.GetValue(info, static info => new(info));

    #endregion

    #region TypeMetaCache

    internal readonly record struct TypeMetaIndex(Type Target, NullabilityMeta? Meta);

    private static readonly ConditionalWeakTable<Type, ConcurrentDictionary<TypeMetaIndex, TypeMeta>> cache = new();

    private static ConcurrentDictionary<TypeMetaIndex, TypeMeta> GetCache(Type type)
        => cache.GetValue(type, static _ => new());

    #endregion

    #endregion

    #region GetNullability

    public static NullabilityMeta? GetNullabilityMeta(FieldInfo? info) =>
        info == null ? null : GetNullabilityMeta(GetCache(info));

    public static NullabilityMeta? GetNullabilityMeta(PropertyInfo? info) =>
        info == null ? null : GetNullabilityMeta(GetCache(info));

    public static NullabilityMeta? GetNullabilityMeta(ParameterInfo? info) =>
        info == null ? null : GetNullabilityMeta(GetCache(info));

    public static NullabilityMeta? GetNullabilityMeta(EventInfo? info) =>
        info == null ? null : GetNullabilityMeta(GetCache(info));

    public static NullabilityMeta? GetNullabilityMeta(NullabilityInfo? info) =>
        info == null ? null : GetCache(info);

    #endregion

    #region GetTypeMeta

    public static TypeMeta GetTypeMeta(Type target, NullabilityMeta? nullabilityMeta = null) =>
        GetCache(target).GetOrAdd(new(target, nullabilityMeta), static index =>
        {
            GenericMeta? generic = !index.Target.IsGenericType
                ? null
                : index.Target.GetGenericTypeDefinition() == typeof(Nullable<>)
                    ? null
                    : GetGenericMeta(
                        index.Target.GenericTypeArguments,
                        index.Meta?.NullabilityInfo?.GenericTypeArguments
                            .Select(a => GetNullabilityMeta(a) ?? NullabilityMeta.Empty)
                            .ToArray()
                    );
            return new TypeMeta(index.Target, generic, index.Meta);
        });

    public static GenericMeta GetGenericMeta(Type[] types, NullabilityMeta[]? nullabilityInfos = null)
    {
        if (nullabilityInfos != null)
        {
            if (nullabilityInfos.Length != types.Length)
                throw new ArgumentException($"{nameof(nullabilityInfos)}.Length must == {nameof(types)}.Length");
        }
        var metas = types.AsParallel().AsOrdered()
            .Select((t, i) => GetTypeMeta(t, nullabilityInfos?[i]))
            .ToArray();
        return new(types, metas, nullabilityInfos, types.Length);
    }

    #endregion
}
