using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Sera.Core.De;
using Sera.Core.Ser;

namespace Sera.Core.Impls;

public record PrimitiveImpl<T>(SerializerPrimitiveHint? Hint = null) : PrimitiveImpl(typeof(T)),
    ISerialize<T>, IDeserialize<T>, IAsyncSerialize<T>, IAsyncDeserialize<T>
{
    public static PrimitiveImpl<T> Default { get; } = new();
    
    public void Write<S>(S serializer, T value, ISeraOptions options) where S : ISerializer
        => serializer.WritePrimitive(value, Hint);

    public T Read<D>(D deserializer, ISeraOptions options) where D : IDeserializer
        => deserializer.ReadPrimitive<T>();

    public ValueTask WriteAsync<S>(S serializer, T value, ISeraOptions options) where S : IAsyncSerializer
        => serializer.WritePrimitiveAsync(value, Hint);

    public ValueTask<T> ReadAsync<D>(D deserializer, ISeraOptions options) where D : IAsyncDeserializer
        => deserializer.ReadPrimitiveAsync<T>();
}

public abstract record PrimitiveImpl(Type Type)
{
    public static PrimitiveImpl<bool> Boolean { get; } = new();
    public static PrimitiveImpl<sbyte> SByte { get; } = new();
    public static PrimitiveImpl<short> Int16 { get; } = new();
    public static PrimitiveImpl<int> Int32 { get; } = new();
    public static PrimitiveImpl<long> Int64 { get; } = new();
    public static PrimitiveImpl<Int128> Int128 { get; } = new();
    public static PrimitiveImpl<byte> Byte { get; } = new();
    public static PrimitiveImpl<ushort> UInt16 { get; } = new();
    public static PrimitiveImpl<uint> UInt32 { get; } = new();
    public static PrimitiveImpl<ulong> UInt64 { get; } = new();
    public static PrimitiveImpl<UInt128> UInt128 { get; } = new();
    public static PrimitiveImpl<nint> IntPtr { get; } = new();
    public static PrimitiveImpl<nuint> UIntPtr { get; } = new();
    public static PrimitiveImpl<Half> Half { get; } = new();
    public static PrimitiveImpl<float> Single { get; } = new();
    public static PrimitiveImpl<double> Double { get; } = new();
    public static PrimitiveImpl<decimal> Decimal { get; } = new();
    public static PrimitiveImpl<BigInteger> BigInteger { get; } = new();
    public static PrimitiveImpl<Complex> Complex { get; } = new();
    public static PrimitiveImpl<TimeSpan> TimeSpan { get; } = new();
    public static PrimitiveImpl<DateOnly> DateOnly { get; } = new();
    public static PrimitiveImpl<TimeOnly> TimeOnly { get; } = new();
    public static PrimitiveImpl<DateTime> DateTime { get; } = new();
    public static PrimitiveImpl<DateTimeOffset> DateTimeOffset { get; } = new();
    public static PrimitiveImpl<Guid> Guid { get; } = new();
    public static PrimitiveImpl<Range> Range { get; } = new();
    public static PrimitiveImpl<Index> Index { get; } = new();
    public static PrimitiveImpl<char> Char { get; } = new();
    public static PrimitiveImpl<Rune> Rune { get; } = new();

    public static ReadOnlyDictionary<SeraPrimitiveTypes, PrimitiveImpl> Instances { get; } = new(
        new Dictionary<SeraPrimitiveTypes, PrimitiveImpl>
        {
            { SeraPrimitiveTypes.Boolean, Boolean },
            { SeraPrimitiveTypes.SByte, SByte },
            { SeraPrimitiveTypes.Int16, Int16 },
            { SeraPrimitiveTypes.Int32, Int32 },
            { SeraPrimitiveTypes.Int64, Int64 },
            { SeraPrimitiveTypes.Int128, Int128 },
            { SeraPrimitiveTypes.Byte, Byte },
            { SeraPrimitiveTypes.UInt16, UInt16 },
            { SeraPrimitiveTypes.UInt32, UInt32 },
            { SeraPrimitiveTypes.UInt64, UInt64 },
            { SeraPrimitiveTypes.UInt128, UInt128 },
            { SeraPrimitiveTypes.IntPtr, IntPtr },
            { SeraPrimitiveTypes.UIntPtr, UIntPtr },
            { SeraPrimitiveTypes.Half, Half },
            { SeraPrimitiveTypes.Single, Single },
            { SeraPrimitiveTypes.Double, Double },
            { SeraPrimitiveTypes.Decimal, Decimal },
            { SeraPrimitiveTypes.BigInteger, BigInteger },
            { SeraPrimitiveTypes.Complex, Complex },
            { SeraPrimitiveTypes.TimeSpan, TimeSpan },
            { SeraPrimitiveTypes.DateOnly, DateOnly },
            { SeraPrimitiveTypes.TimeOnly, TimeOnly },
            { SeraPrimitiveTypes.DateTime, DateTime },
            { SeraPrimitiveTypes.DateTimeOffset, DateTimeOffset },
            { SeraPrimitiveTypes.Guid, Guid },
            { SeraPrimitiveTypes.Range, Range },
            { SeraPrimitiveTypes.Index, Index },
            { SeraPrimitiveTypes.Char, Char },
            { SeraPrimitiveTypes.Rune, Rune },
        });
}
