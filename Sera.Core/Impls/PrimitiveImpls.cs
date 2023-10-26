using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Sera.Core.De;
using Sera.Core.Ser;

namespace Sera.Core.Impls;

public readonly record struct PrimitiveImpl<T>(SerializerPrimitiveHint? Hint = null) :
    ISerialize<T>, IDeserialize<T>, IAsyncSerialize<T>, IAsyncDeserialize<T>
{
    public static PrimitiveImpl<T> Default { get; } = new(new());

    public void Write<S>(S serializer, T value, ISeraOptions options) where S : ISerializer
        => serializer.WritePrimitive(value, Hint);

    public T Read<D>(D deserializer, ISeraOptions options) where D : IDeserializer
        => deserializer.ReadPrimitive<T>();

    public ValueTask WriteAsync<S>(S serializer, T value, ISeraOptions options) where S : IAsyncSerializer
        => serializer.WritePrimitiveAsync(value, Hint);

    public ValueTask<T> ReadAsync<D>(D deserializer, ISeraOptions options) where D : IAsyncDeserializer
        => deserializer.ReadPrimitiveAsync<T>();
}

public static class PrimitiveImpls
{
    public static PrimitiveImpl<bool> Boolean => PrimitiveImpl<bool>.Default;
    public static PrimitiveImpl<sbyte> SByte => PrimitiveImpl<sbyte>.Default;
    public static PrimitiveImpl<short> Int16 => PrimitiveImpl<short>.Default;
    public static PrimitiveImpl<int> Int32 => PrimitiveImpl<int>.Default;
    public static PrimitiveImpl<long> Int64 => PrimitiveImpl<long>.Default;
    public static PrimitiveImpl<Int128> Int128 => PrimitiveImpl<Int128>.Default;
    public static PrimitiveImpl<byte> Byte => PrimitiveImpl<byte>.Default;
    public static PrimitiveImpl<ushort> UInt16 => PrimitiveImpl<ushort>.Default;
    public static PrimitiveImpl<uint> UInt32 => PrimitiveImpl<uint>.Default;
    public static PrimitiveImpl<ulong> UInt64 => PrimitiveImpl<ulong>.Default;
    public static PrimitiveImpl<UInt128> UInt128 => PrimitiveImpl<UInt128>.Default;
    public static PrimitiveImpl<nint> IntPtr => PrimitiveImpl<nint>.Default;
    public static PrimitiveImpl<nuint> UIntPtr => PrimitiveImpl<nuint>.Default;
    public static PrimitiveImpl<Half> Half => PrimitiveImpl<Half>.Default;
    public static PrimitiveImpl<float> Single => PrimitiveImpl<float>.Default;
    public static PrimitiveImpl<double> Double => PrimitiveImpl<double>.Default;
    public static PrimitiveImpl<decimal> Decimal => PrimitiveImpl<decimal>.Default;
    public static PrimitiveImpl<BigInteger> BigInteger => PrimitiveImpl<BigInteger>.Default;
    public static PrimitiveImpl<Complex> Complex => PrimitiveImpl<Complex>.Default;
    public static PrimitiveImpl<TimeSpan> TimeSpan => PrimitiveImpl<TimeSpan>.Default;
    public static PrimitiveImpl<DateOnly> DateOnly => PrimitiveImpl<DateOnly>.Default;
    public static PrimitiveImpl<TimeOnly> TimeOnly => PrimitiveImpl<TimeOnly>.Default;
    public static PrimitiveImpl<DateTime> DateTime => PrimitiveImpl<DateTime>.Default;
    public static PrimitiveImpl<DateTimeOffset> DateTimeOffset => PrimitiveImpl<DateTimeOffset>.Default;
    public static PrimitiveImpl<Guid> Guid => PrimitiveImpl<Guid>.Default;
    public static PrimitiveImpl<Range> Range => PrimitiveImpl<Range>.Default;
    public static PrimitiveImpl<Index> Index => PrimitiveImpl<Index>.Default;
    public static PrimitiveImpl<char> Char => PrimitiveImpl<char>.Default;
    public static PrimitiveImpl<Rune> Rune => PrimitiveImpl<Rune>.Default;

    private static readonly HashSet<Type> PrimitiveTypes = new()
    {
        typeof(bool),
        typeof(sbyte),
        typeof(short),
        typeof(int),
        typeof(long),
        typeof(Int128),
        typeof(byte),
        typeof(ushort),
        typeof(uint),
        typeof(ulong),
        typeof(UInt128),
        typeof(nint),
        typeof(nuint),
        typeof(Half),
        typeof(float),
        typeof(double),
        typeof(decimal),
        typeof(BigInteger),
        typeof(Complex),
        typeof(TimeSpan),
        typeof(DateOnly),
        typeof(TimeOnly),
        typeof(DateTime),
        typeof(DateTimeOffset),
        typeof(Guid),
        typeof(Range),
        typeof(Index),
        typeof(char),
        typeof(Rune),
    };

    public static bool IsPrimitiveType(Type type) => PrimitiveTypes.Contains(type);
}
