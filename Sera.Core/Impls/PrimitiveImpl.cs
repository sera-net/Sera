using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Sera.Core.De;
using Sera.Core.Ser;

namespace Sera.Core.Impls;

public record PrimitiveImpl<T>(SeraPrimitiveTypes type) : PrimitiveImpl(typeof(T)),
    ISerialize<T>, IDeserialize<T>, IAsyncSerialize<T>, IAsyncDeserialize<T>
{
    public void Write<S>(S serializer, T value, ISeraOptions options) where S : ISerializer
        => serializer.WritePrimitive(value, null);

    public T Read<D>(D deserializer, ISeraOptions options) where D : IDeserializer
        => deserializer.ReadPrimitive<T>();

    public ValueTask WriteAsync<S>(S serializer, T value, ISeraOptions options) where S : IAsyncSerializer
        => serializer.WritePrimitiveAsync(value, null);

    public ValueTask<T> ReadAsync<D>(D deserializer, ISeraOptions options) where D : IAsyncDeserializer
        => deserializer.ReadPrimitiveAsync<T>();
}

public abstract record PrimitiveImpl(Type Type)
{
    public static PrimitiveImpl<bool> Boolean { get; } = new(SeraPrimitiveTypes.Boolean);
    public static PrimitiveImpl<sbyte> SByte { get; } = new(SeraPrimitiveTypes.SByte);
    public static PrimitiveImpl<short> Int16 { get; } = new(SeraPrimitiveTypes.Int16);
    public static PrimitiveImpl<int> Int32 { get; } = new(SeraPrimitiveTypes.Int32);
    public static PrimitiveImpl<long> Int64 { get; } = new(SeraPrimitiveTypes.Int64);
    public static PrimitiveImpl<Int128> Int128 { get; } = new(SeraPrimitiveTypes.Int128);
    public static PrimitiveImpl<byte> Byte { get; } = new(SeraPrimitiveTypes.Byte);
    public static PrimitiveImpl<ushort> UInt16 { get; } = new(SeraPrimitiveTypes.UInt16);
    public static PrimitiveImpl<uint> UInt32 { get; } = new(SeraPrimitiveTypes.UInt32);
    public static PrimitiveImpl<ulong> UInt64 { get; } = new(SeraPrimitiveTypes.UInt64);
    public static PrimitiveImpl<UInt128> UInt128 { get; } = new(SeraPrimitiveTypes.UInt128);
    public static PrimitiveImpl<nint> IntPtr { get; } = new(SeraPrimitiveTypes.IntPtr);
    public static PrimitiveImpl<nuint> UIntPtr { get; } = new(SeraPrimitiveTypes.UIntPtr);
    public static PrimitiveImpl<Half> Half { get; } = new(SeraPrimitiveTypes.Half);
    public static PrimitiveImpl<float> Single { get; } = new(SeraPrimitiveTypes.Single);
    public static PrimitiveImpl<double> Double { get; } = new(SeraPrimitiveTypes.Double);
    public static PrimitiveImpl<decimal> Decimal { get; } = new(SeraPrimitiveTypes.Decimal);
    public static PrimitiveImpl<BigInteger> BigInteger { get; } = new(SeraPrimitiveTypes.BigInteger);
    public static PrimitiveImpl<Complex> Complex { get; } = new(SeraPrimitiveTypes.Complex);
    public static PrimitiveImpl<TimeSpan> TimeSpan { get; } = new(SeraPrimitiveTypes.TimeSpan);
    public static PrimitiveImpl<DateOnly> DateOnly { get; } = new(SeraPrimitiveTypes.DateOnly);
    public static PrimitiveImpl<TimeOnly> TimeOnly { get; } = new(SeraPrimitiveTypes.TimeOnly);
    public static PrimitiveImpl<DateTime> DateTime { get; } = new(SeraPrimitiveTypes.DateTime);
    public static PrimitiveImpl<DateTimeOffset> DateTimeOffset { get; } = new(SeraPrimitiveTypes.DateTimeOffset);
    public static PrimitiveImpl<Guid> Guid { get; } = new(SeraPrimitiveTypes.Guid);
    public static PrimitiveImpl<Range> Range { get; } = new(SeraPrimitiveTypes.Range);
    public static PrimitiveImpl<Index> Index { get; } = new(SeraPrimitiveTypes.Index);
    public static PrimitiveImpl<char> Char { get; } = new(SeraPrimitiveTypes.Char);
    public static PrimitiveImpl<Rune> Rune { get; } = new(SeraPrimitiveTypes.Rune);

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
