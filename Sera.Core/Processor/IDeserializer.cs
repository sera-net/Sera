using System;
using System.Buffers;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Sera.Core.Impls;
using Sera.Core.SerDe;

namespace Sera.Core.De;

#region Basic

/// <summary>Hints that the next one might be</summary>
[Flags]
public enum DeserializerHint
{
    Unknown = 0,
    Any = Primitive | String | Bytes | Unit | Option | Seq | Map | Struct | Variant,

    Primitive = 1 << 0,
    String = 1 << 1,
    Bytes = 1 << 2,
    Unit = 1 << 3,
    Option = 1 << 4,
    Seq = 1 << 5,
    Map = 1 << 6,
    Struct = 1 << 7,
    Variant = 1 << 8,
}

public static partial class DeserializerEx
{
    public static DeserializerPrimitiveHint ToHint(this SeraPrimitiveTypes type) => type switch
    {
        SeraPrimitiveTypes.Boolean => DeserializerPrimitiveHint.Boolean,
        SeraPrimitiveTypes.SByte => DeserializerPrimitiveHint.SByte,
        SeraPrimitiveTypes.Int16 => DeserializerPrimitiveHint.Int16,
        SeraPrimitiveTypes.Int32 => DeserializerPrimitiveHint.Int32,
        SeraPrimitiveTypes.Int64 => DeserializerPrimitiveHint.Int64,
        SeraPrimitiveTypes.Int128 => DeserializerPrimitiveHint.Int128,
        SeraPrimitiveTypes.Byte => DeserializerPrimitiveHint.Byte,
        SeraPrimitiveTypes.UInt16 => DeserializerPrimitiveHint.UInt16,
        SeraPrimitiveTypes.UInt32 => DeserializerPrimitiveHint.UInt32,
        SeraPrimitiveTypes.UInt64 => DeserializerPrimitiveHint.UInt64,
        SeraPrimitiveTypes.UInt128 => DeserializerPrimitiveHint.UInt128,
        SeraPrimitiveTypes.IntPtr => DeserializerPrimitiveHint.IntPtr,
        SeraPrimitiveTypes.UIntPtr => DeserializerPrimitiveHint.UIntPtr,
        SeraPrimitiveTypes.Half => DeserializerPrimitiveHint.Half,
        SeraPrimitiveTypes.Single => DeserializerPrimitiveHint.Single,
        SeraPrimitiveTypes.Double => DeserializerPrimitiveHint.Double,
        SeraPrimitiveTypes.Decimal => DeserializerPrimitiveHint.Decimal,
        SeraPrimitiveTypes.BigInteger => DeserializerPrimitiveHint.BigInteger,
        SeraPrimitiveTypes.Complex => DeserializerPrimitiveHint.Complex,
        SeraPrimitiveTypes.TimeSpan => DeserializerPrimitiveHint.TimeSpan,
        SeraPrimitiveTypes.DateOnly => DeserializerPrimitiveHint.DateOnly,
        SeraPrimitiveTypes.TimeOnly => DeserializerPrimitiveHint.TimeOnly,
        SeraPrimitiveTypes.DateTime => DeserializerPrimitiveHint.DateTime,
        SeraPrimitiveTypes.DateTimeOffset => DeserializerPrimitiveHint.DateTimeOffset,
        SeraPrimitiveTypes.Guid => DeserializerPrimitiveHint.Guid,
        SeraPrimitiveTypes.Range => DeserializerPrimitiveHint.Range,
        SeraPrimitiveTypes.Index => DeserializerPrimitiveHint.Index,
        SeraPrimitiveTypes.Char => DeserializerPrimitiveHint.Char,
        SeraPrimitiveTypes.Rune => DeserializerPrimitiveHint.Rune,
        _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
    };

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static DeserializerPrimitiveHint ToHint<T>()
    {
        if (typeof(T) == typeof(bool)) return DeserializerPrimitiveHint.Boolean;
        if (typeof(T) == typeof(sbyte)) return DeserializerPrimitiveHint.SByte;
        if (typeof(T) == typeof(short)) return DeserializerPrimitiveHint.Int16;
        if (typeof(T) == typeof(int)) return DeserializerPrimitiveHint.Int32;
        if (typeof(T) == typeof(long)) return DeserializerPrimitiveHint.Int64;
        if (typeof(T) == typeof(Int128)) return DeserializerPrimitiveHint.Int128;
        if (typeof(T) == typeof(byte)) return DeserializerPrimitiveHint.Byte;
        if (typeof(T) == typeof(ushort)) return DeserializerPrimitiveHint.UInt16;
        if (typeof(T) == typeof(uint)) return DeserializerPrimitiveHint.UInt32;
        if (typeof(T) == typeof(ulong)) return DeserializerPrimitiveHint.UInt64;
        if (typeof(T) == typeof(UInt128)) return DeserializerPrimitiveHint.UInt128;
        if (typeof(T) == typeof(nint)) return DeserializerPrimitiveHint.IntPtr;
        if (typeof(T) == typeof(nuint)) return DeserializerPrimitiveHint.UIntPtr;
        if (typeof(T) == typeof(Half)) return DeserializerPrimitiveHint.Half;
        if (typeof(T) == typeof(float)) return DeserializerPrimitiveHint.Float;
        if (typeof(T) == typeof(double)) return DeserializerPrimitiveHint.Double;
        if (typeof(T) == typeof(decimal)) return DeserializerPrimitiveHint.Decimal;
        if (typeof(T) == typeof(BigInteger)) return DeserializerPrimitiveHint.BigInteger;
        if (typeof(T) == typeof(Complex)) return DeserializerPrimitiveHint.Complex;
        if (typeof(T) == typeof(TimeSpan)) return DeserializerPrimitiveHint.TimeSpan;
        if (typeof(T) == typeof(DateOnly)) return DeserializerPrimitiveHint.DateOnly;
        if (typeof(T) == typeof(TimeOnly)) return DeserializerPrimitiveHint.TimeOnly;
        if (typeof(T) == typeof(DateTime)) return DeserializerPrimitiveHint.DateTime;
        if (typeof(T) == typeof(DateTimeOffset)) return DeserializerPrimitiveHint.DateTimeOffset;
        if (typeof(T) == typeof(Guid)) return DeserializerPrimitiveHint.Guid;
        if (typeof(T) == typeof(Range)) return DeserializerPrimitiveHint.Range;
        if (typeof(T) == typeof(Index)) return DeserializerPrimitiveHint.Index;
        if (typeof(T) == typeof(char)) return DeserializerPrimitiveHint.Char;
        if (typeof(T) == typeof(Rune)) return DeserializerPrimitiveHint.Rune;
        throw new ArgumentOutOfRangeException($"type {typeof(T)} is not a sera primitive");
    }
}

public partial interface IDeserializer : ISeraAbility
{
    /// <summary>Peek the next hint</summary>
    public DeserializerHint PeekNext(DeserializerHint? need);

    /// <summary>Skip once</summary>
    public void Skip();

    public R ReadAny<R, V>(DeserializerHint? need, DeserializerPrimitiveHint? need2, V visitor)
        where V : IAnyDeserializerVisitor<R>;
}

public interface IAnyDeserializerVisitor<out R> :
    IPrimitiveDeserializerVisitor<R>, IStringDeserializerVisitor<R>, IBytesDeserializerVisitor<R>,
    IUnitDeserializerVisitor<R>, IOptionDeserializerVisitor<R>, ISeqDeserializerVisitor<R>,
    IMapDeserializerVisitor<R>, IStructDeserializerVisitor<R>, IVariantDeserializerVisitor<R> { }

public partial interface IAsyncDeserializer : ISeraAbility
{
    /// <summary>Peek the next hint</summary>
    public ValueTask<DeserializerHint> PeekNextAsync(DeserializerHint? need);

    /// <summary>Skip once</summary>
    public ValueTask SkipAsync();

    public ValueTask<R> ReadAnyAsync<R, V>(DeserializerHint? need, DeserializerPrimitiveHint? need2, V visitor)
        where V : IAsyncAnyDeserializerVisitor<R>;
}

public interface IAsyncAnyDeserializerVisitor<R> :
    IAsyncPrimitiveDeserializerVisitor<R>, IAsyncStringDeserializerVisitor<R>, IAsyncBytesDeserializerVisitor<R>,
    IAsyncUnitDeserializerVisitor<R>, IAsyncOptionDeserializerVisitor<R>, IAsyncSeqDeserializerVisitor<R>,
    IAsyncMapDeserializerVisitor<R>, IAsyncStructDeserializerVisitor<R>, IAsyncVariantDeserializerVisitor<R> { }

#endregion

#region Primitive

/// <summary>Hints that the primitive might be</summary>
[Flags]
public enum DeserializerPrimitiveHint : ulong
{
    Unknown = 0,
    Any = Boolean | Number | Date | Guid | Range | Index | Char | Rune,

    Boolean = 1 << 0,
    SByte = 1 << 1,
    Int16 = 1 << 2,
    Int32 = 1 << 3,
    Int64 = 1 << 4,
    Int128 = 1 << 5,
    Byte = 1 << 6,
    UInt16 = 1 << 7,
    UInt32 = 1 << 8,
    UInt64 = 1 << 9,
    UInt128 = 1 << 10,
    IntPtr = 1 << 11,
    UIntPtr = 1 << 12,
    Half = 1 << 13,
    Single = 1 << 14,
    Double = 1 << 15,
    Decimal = 1 << 16,
    BigInteger = 1 << 17,
    Complex = 1 << 18,
    TimeSpan = 1 << 19,
    DateOnly = 1 << 20,
    TimeOnly = 1 << 21,
    DateTime = 1 << 22,
    DateTimeOffset = 1 << 23,
    Guid = 1 << 24,
    Range = 1 << 25,
    Index = 1 << 26,
    Char = 1 << 27,
    Rune = 1 << 28,

    SInt = SByte | Int16 | Int32 | Int64 | Int128 | IntPtr,
    UInt = Byte | UInt16 | UInt32 | UInt64 | UInt128 | UIntPtr,
    Int = SInt | UInt,
    Float = Half | Single | Double,
    BinaryNumber = Int | Float,
    Number = BinaryNumber | Decimal | BigInteger | Complex,
    Date = TimeSpan | DateOnly | TimeOnly | DateTime | DateTimeOffset,
}

public partial interface IDeserializer
{
    /// <summary>
    /// Peek the next primitive hint.<br/>
    /// <para>If not a primitive return <see cref="DeserializerPrimitiveHint.Unknown"/></para>
    /// </summary>
    public DeserializerPrimitiveHint PeekPrimitive(DeserializerPrimitiveHint? need);

    public R ReadPrimitive<R, V>(DeserializerPrimitiveHint? need, V visitor) where V : IPrimitiveDeserializerVisitor<R>;

    public T ReadPrimitive<T>()
        => ReadPrimitive<T, IdentityPrimitiveVisitor<T>>(DeserializerEx.ToHint<T>(), new());
}

public interface IPrimitiveDeserializerVisitor<out R>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public R VisitPrimitive<T>(T value) => value switch
    {
        bool v => Visit(v),
        sbyte v => Visit(v),
        short v => Visit(v),
        int v => Visit(v),
        long v => Visit(v),
        Int128 v => Visit(v),
        byte v => Visit(v),
        ushort v => Visit(v),
        uint v => Visit(v),
        ulong v => Visit(v),
        UInt128 v => Visit(v),
        nint v => Visit(v),
        nuint v => Visit(v),
        Half v => Visit(v),
        float v => Visit(v),
        double v => Visit(v),
        decimal v => Visit(v),
        BigInteger v => Visit(v),
        Complex v => Visit(v),
        TimeSpan v => Visit(v),
        DateOnly v => Visit(v),
        TimeOnly v => Visit(v),
        DateTime v => Visit(v),
        DateTimeOffset v => Visit(v),
        Guid v => Visit(v),
        Range v => Visit(v),
        Index v => Visit(v),
        char v => Visit(v),
        Rune v => Visit(v),
        _ => throw DeserializeInvalidTypeException.Unexpected(value),
    };

    protected R Visit(bool value) => throw DeserializeInvalidTypeException.Unexpected(value);
    protected R Visit(sbyte value) => throw DeserializeInvalidTypeException.Unexpected(value);
    protected R Visit(short value) => throw DeserializeInvalidTypeException.Unexpected(value);
    protected R Visit(int value) => throw DeserializeInvalidTypeException.Unexpected(value);
    protected R Visit(long value) => throw DeserializeInvalidTypeException.Unexpected(value);
    protected R Visit(Int128 value) => throw DeserializeInvalidTypeException.Unexpected(value);
    protected R Visit(byte value) => throw DeserializeInvalidTypeException.Unexpected(value);
    protected R Visit(ushort value) => throw DeserializeInvalidTypeException.Unexpected(value);
    protected R Visit(uint value) => throw DeserializeInvalidTypeException.Unexpected(value);
    protected R Visit(ulong value) => throw DeserializeInvalidTypeException.Unexpected(value);
    protected R Visit(UInt128 value) => throw DeserializeInvalidTypeException.Unexpected(value);
    protected R Visit(nint value) => throw DeserializeInvalidTypeException.Unexpected(value);
    protected R Visit(nuint value) => throw DeserializeInvalidTypeException.Unexpected(value);
    protected R Visit(Half value) => throw DeserializeInvalidTypeException.Unexpected(value);
    protected R Visit(float value) => throw DeserializeInvalidTypeException.Unexpected(value);
    protected R Visit(double value) => throw DeserializeInvalidTypeException.Unexpected(value);
    protected R Visit(decimal value) => throw DeserializeInvalidTypeException.Unexpected(value);
    protected R Visit(BigInteger value) => throw DeserializeInvalidTypeException.Unexpected(value);
    protected R Visit(Complex value) => throw DeserializeInvalidTypeException.Unexpected(value);
    protected R Visit(TimeSpan value) => throw DeserializeInvalidTypeException.Unexpected(value);
    protected R Visit(DateOnly value) => throw DeserializeInvalidTypeException.Unexpected(value);
    protected R Visit(TimeOnly value) => throw DeserializeInvalidTypeException.Unexpected(value);
    protected R Visit(DateTime value) => throw DeserializeInvalidTypeException.Unexpected(value);
    protected R Visit(DateTimeOffset value) => throw DeserializeInvalidTypeException.Unexpected(value);
    protected R Visit(Guid value) => throw DeserializeInvalidTypeException.Unexpected(value);
    protected R Visit(Range value) => throw DeserializeInvalidTypeException.Unexpected(value);
    protected R Visit(Index value) => throw DeserializeInvalidTypeException.Unexpected(value);
    protected R Visit(char value) => throw DeserializeInvalidTypeException.Unexpected(value);
    protected R Visit(Rune value) => throw DeserializeInvalidTypeException.Unexpected(value);
}

public partial interface IAsyncDeserializer
{
    /// <inheritdoc cref="IDeserializer.PeekPrimitive(DeserializerPrimitiveHint?)"/>
    public ValueTask<DeserializerPrimitiveHint> PeekPrimitiveAsync(DeserializerPrimitiveHint? need);

    public ValueTask<R> ReadPrimitiveAsync<R, V>(DeserializerPrimitiveHint? need, V visitor)
        where V : IAsyncPrimitiveDeserializerVisitor<R>;

    public ValueTask<T> ReadPrimitiveAsync<T>()
        => ReadPrimitiveAsync<T, AsyncIdentityPrimitiveVisitor<T>>(DeserializerEx.ToHint<T>(), new());
}

public interface IAsyncPrimitiveDeserializerVisitor<R>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public ValueTask<R> VisitPrimitiveAsync<T>(T value) => value switch
    {
        bool v => VisitAsync(v),
        sbyte v => VisitAsync(v),
        short v => VisitAsync(v),
        int v => VisitAsync(v),
        long v => VisitAsync(v),
        Int128 v => VisitAsync(v),
        byte v => VisitAsync(v),
        ushort v => VisitAsync(v),
        uint v => VisitAsync(v),
        ulong v => VisitAsync(v),
        UInt128 v => VisitAsync(v),
        nint v => VisitAsync(v),
        nuint v => VisitAsync(v),
        Half v => VisitAsync(v),
        float v => VisitAsync(v),
        double v => VisitAsync(v),
        decimal v => VisitAsync(v),
        BigInteger v => VisitAsync(v),
        Complex v => VisitAsync(v),
        TimeSpan v => VisitAsync(v),
        DateOnly v => VisitAsync(v),
        TimeOnly v => VisitAsync(v),
        DateTime v => VisitAsync(v),
        DateTimeOffset v => VisitAsync(v),
        Guid v => VisitAsync(v),
        Range v => VisitAsync(v),
        Index v => VisitAsync(v),
        char v => VisitAsync(v),
        Rune v => VisitAsync(v),
        _ => throw DeserializeInvalidTypeException.Unexpected(value),
    };

    protected ValueTask<R> VisitAsync(bool value) => throw DeserializeInvalidTypeException.Unexpected(value);
    protected ValueTask<R> VisitAsync(sbyte value) => throw DeserializeInvalidTypeException.Unexpected(value);
    protected ValueTask<R> VisitAsync(short value) => throw DeserializeInvalidTypeException.Unexpected(value);
    protected ValueTask<R> VisitAsync(int value) => throw DeserializeInvalidTypeException.Unexpected(value);
    protected ValueTask<R> VisitAsync(long value) => throw DeserializeInvalidTypeException.Unexpected(value);
    protected ValueTask<R> VisitAsync(Int128 value) => throw DeserializeInvalidTypeException.Unexpected(value);
    protected ValueTask<R> VisitAsync(byte value) => throw DeserializeInvalidTypeException.Unexpected(value);
    protected ValueTask<R> VisitAsync(ushort value) => throw DeserializeInvalidTypeException.Unexpected(value);
    protected ValueTask<R> VisitAsync(uint value) => throw DeserializeInvalidTypeException.Unexpected(value);
    protected ValueTask<R> VisitAsync(ulong value) => throw DeserializeInvalidTypeException.Unexpected(value);
    protected ValueTask<R> VisitAsync(UInt128 value) => throw DeserializeInvalidTypeException.Unexpected(value);
    protected ValueTask<R> VisitAsync(nint value) => throw DeserializeInvalidTypeException.Unexpected(value);
    protected ValueTask<R> VisitAsync(nuint value) => throw DeserializeInvalidTypeException.Unexpected(value);
    protected ValueTask<R> VisitAsync(Half value) => throw DeserializeInvalidTypeException.Unexpected(value);
    protected ValueTask<R> VisitAsync(float value) => throw DeserializeInvalidTypeException.Unexpected(value);
    protected ValueTask<R> VisitAsync(double value) => throw DeserializeInvalidTypeException.Unexpected(value);
    protected ValueTask<R> VisitAsync(decimal value) => throw DeserializeInvalidTypeException.Unexpected(value);
    protected ValueTask<R> VisitAsync(BigInteger value) => throw DeserializeInvalidTypeException.Unexpected(value);
    protected ValueTask<R> VisitAsync(Complex value) => throw DeserializeInvalidTypeException.Unexpected(value);
    protected ValueTask<R> VisitAsync(TimeSpan value) => throw DeserializeInvalidTypeException.Unexpected(value);
    protected ValueTask<R> VisitAsync(DateOnly value) => throw DeserializeInvalidTypeException.Unexpected(value);
    protected ValueTask<R> VisitAsync(TimeOnly value) => throw DeserializeInvalidTypeException.Unexpected(value);
    protected ValueTask<R> VisitAsync(DateTime value) => throw DeserializeInvalidTypeException.Unexpected(value);
    protected ValueTask<R> VisitAsync(DateTimeOffset value) => throw DeserializeInvalidTypeException.Unexpected(value);
    protected ValueTask<R> VisitAsync(Guid value) => throw DeserializeInvalidTypeException.Unexpected(value);
    protected ValueTask<R> VisitAsync(Range value) => throw DeserializeInvalidTypeException.Unexpected(value);
    protected ValueTask<R> VisitAsync(Index value) => throw DeserializeInvalidTypeException.Unexpected(value);
    protected ValueTask<R> VisitAsync(char value) => throw DeserializeInvalidTypeException.Unexpected(value);
    protected ValueTask<R> VisitAsync(Rune value) => throw DeserializeInvalidTypeException.Unexpected(value);
}

public static partial class DeserializerEx
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool PeekIsPrimitive<S>(this S self) where S : IDeserializer
        => (self.PeekNext(DeserializerHint.Primitive) & DeserializerHint.Primitive) != 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static async ValueTask<bool> PeekIsPrimitiveAsync<S>(this S self) where S : IAsyncDeserializer
        => (await self.PeekNextAsync(DeserializerHint.Primitive) & DeserializerHint.Primitive) != 0;
}

#endregion

#region String

public partial interface IDeserializer
{
    public R ReadString<R, V>(V visitor) where V : IStringDeserializerVisitor<R>;

    public string ReadString() => ReadString<string, IdentityStringVisitor>(new());
}

public interface IStringDeserializerVisitor<out R>
{
    public R VisitString<A>(A access) where A : IStringAccess;
}

public interface IStringAccess
{
    /// <summary>
    /// Get string length in utf-16.
    /// </summary>
    public int GetLength() => GetLength(Encoding.Unicode);

    /// <summary>Get the encoding of string</summary>
    public Encoding GetEncoding();

    /// <summary>
    /// Get string length in the encoding.
    /// </summary>
    public int GetLength(Encoding encoding);

    /// <summary>Read string as utf-16</summary>
    public string ReadString();

    /// <summary>Read string as utf-16</summary>
    public void ReadString(Memory<char> value) => ReadString(value.Span);

    /// <summary>Read string as utf-16</summary>
    public void ReadString(Span<char> value);

    /// <summary>Read string as utf-16</summary>
    public ReadOnlyMemory<char> ReadStringAsMemory() => ReadString().AsMemory();

    /// <summary>Read string as utf-16</summary>
    public Memory<char> ReadStringAsMutableMemory() => ReadString().ToCharArray();

    /// <summary>Read string as encoding</summary>
    public byte[] ReadStringEncoded(Encoding encoding);

    /// <summary>Read string as encoding</summary>
    public void ReadStringEncoded(Memory<byte> value, Encoding encoding) => ReadStringEncoded(value.Span, encoding);

    /// <summary>Read string as encoding</summary>
    public void ReadStringEncoded(Span<byte> value, Encoding encoding);

    /// <summary>Read string as encoding</summary>
    public ReadOnlyMemory<byte> ReadStringEncodedAsMemory(Encoding encoding) => ReadStringEncoded(encoding);

    /// <summary>Read string as encoding</summary>
    public Memory<byte> ReadStringEncodedAsMutableMemory(Encoding encoding) => ReadStringEncoded(encoding);
}

public partial interface IAsyncDeserializer
{
    public ValueTask<R> ReadStringAsync<R, V>(V visitor) where V : IAsyncStringDeserializerVisitor<R>;

    public ValueTask<string> ReadStringAsync() => ReadStringAsync<string, IdentityStringVisitor>(new());
}

public interface IAsyncStringDeserializerVisitor<R>
{
    public ValueTask<R> VisitStringAsync<A>(A access) where A : IAsyncStringAccess;
}

public interface IAsyncStringAccess
{
    /// <summary>
    /// Get string length in utf-16.
    /// </summary>
    public ValueTask<int> GetLengthAsync() => GetLengthAsync(Encoding.Unicode);

    /// <summary>Get the encoding of string</summary>
    public ValueTask<Encoding> GetEncodingAsync();

    /// <summary>
    /// Get string length in the encoding.
    /// </summary>
    public ValueTask<int> GetLengthAsync(Encoding encoding);

    /// <summary>Read string as utf-16</summary>
    public ValueTask<string> ReadStringAsync();

    /// <summary>Read string as utf-16</summary>
    public ValueTask ReadStringAsync(Memory<char> value);

    /// <summary>Read string as utf-16</summary>
    public async ValueTask<ReadOnlyMemory<char>> ReadStringAsMemoryAsync() => (await ReadStringAsync()).AsMemory();

    /// <summary>Read string as utf-16</summary>
    public async ValueTask<Memory<char>> ReadStringAsMutableMemoryAsync() => (await ReadStringAsync()).ToCharArray();

    /// <summary>Read string as encoding</summary>
    public ValueTask<byte[]> ReadStringEncodedAsync(Encoding encoding);

    /// <summary>Read string as encoding</summary>
    public ValueTask ReadStringEncodedAsync(Memory<byte> value, Encoding encoding);

    /// <summary>Read string as encoding</summary>
    public async ValueTask<ReadOnlyMemory<byte>> ReadStringEncodedAsMemoryAsync(Encoding encoding) =>
        await ReadStringEncodedAsync(encoding);

    /// <summary>Read string as encoding</summary>
    public async ValueTask<Memory<byte>> ReadStringEncodedAsMutableMemoryAsync(Encoding encoding) =>
        await ReadStringEncodedAsync(encoding);
}

public static partial class DeserializerEx
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool PeekIsString<S>(this S self) where S : IDeserializer
        => (self.PeekNext(DeserializerHint.String) & DeserializerHint.String) != 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static async ValueTask<bool> PeekIsStringAsync<S>(this S self) where S : IAsyncDeserializer
        => (await self.PeekNextAsync(DeserializerHint.String) & DeserializerHint.String) != 0;
}

#endregion

#region Bytes

public partial interface IDeserializer
{
    public R ReadBytes<R, V>(V visitor) where V : IBytesDeserializerVisitor<R>;

    public byte[] ReadBytes() => ReadBytes<byte[], IdentityBytesVisitor>(new());
}

public interface IBytesDeserializerVisitor<out R>
{
    public R VisitBytes<A>(A access) where A : IBytesAccess;
}

public interface IBytesAccess
{
    public int GetLength();
    public byte[] ReadBytes();
    public List<byte> ReadBytesAsList();
    public void ReadBytes(Memory<byte> value) => ReadBytes(value.Span);
    public void ReadBytes(Span<byte> value);
    public ReadOnlyMemory<byte> ReadBytesAsMemory();
    public Memory<byte> ReadBytesAsMutableMemory();
    public ReadOnlySequence<byte> ReadBytesAsSequence();
}

public partial interface IAsyncDeserializer
{
    public ValueTask<R> ReadBytesAsync<R, V>(V visitor) where V : IAsyncBytesDeserializerVisitor<R>;

    public ValueTask<byte[]> ReadBytesAsync() => ReadBytesAsync<byte[], IdentityBytesVisitor>(new());
}

public interface IAsyncBytesDeserializerVisitor<R>
{
    public ValueTask<R> VisitBytesAsync<A>(A access) where A : IAsyncBytesAccess;
}

public interface IAsyncBytesAccess
{
    public ValueTask<int> GetLengthAsync();
    public ValueTask<byte[]> ReadBytesAsync();
    public ValueTask<List<byte>> ReadBytesAsListAsync();
    public ValueTask ReadBytesAsync(Memory<byte> value);
    public ValueTask<ReadOnlyMemory<byte>> ReadBytesAsMemoryAsync();
    public ValueTask<Memory<byte>> ReadBytesAsMutableMemoryAsync();
    public ValueTask<ReadOnlySequence<byte>> ReadBytesAsSequenceAsync();
}

public static partial class DeserializerEx
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool PeekIsBytes<S>(this S self) where S : IDeserializer
        => (self.PeekNext(DeserializerHint.Bytes) & DeserializerHint.Bytes) != 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static async ValueTask<bool> PeekIsBytesAsync<S>(this S self) where S : IAsyncDeserializer
        => (await self.PeekNextAsync(DeserializerHint.Bytes) & DeserializerHint.Bytes) != 0;
}

#endregion

#region Unit

public partial interface IDeserializer
{
    public R ReadUnit<R, V>(V visitor) where V : IUnitDeserializerVisitor<R>;

    public void ReadUnit() => ReadUnit<Unit, IdentityUnitOrNullVisitor<Unit>>(new());
}

public interface IUnitDeserializerVisitor<out R>
{
    public R VisitUnit();
}

public partial interface IAsyncDeserializer
{
    public ValueTask<R> ReadUnitAsync<R, V>(V visitor) where V : IAsyncUnitDeserializerVisitor<R>;

    public async ValueTask ReadUnitAsync() => await ReadUnitAsync<Unit, IdentityUnitOrNullVisitor<Unit>>(new());
}

public interface IAsyncUnitDeserializerVisitor<R>
{
    public ValueTask<R> VisitUnitAsync();
}

public static partial class DeserializerEx
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool PeekIsUnit<S>(this S self) where S : IDeserializer
        => (self.PeekNext(DeserializerHint.Unit) & DeserializerHint.Unit) != 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static async ValueTask<bool> PeekIsUnitAsync<S>(this S self) where S : IAsyncDeserializer
        => (await self.PeekNextAsync(DeserializerHint.Unit) & DeserializerHint.Unit) != 0;
}

#endregion

#region Option

public partial interface IDeserializer
{
    public R ReadOption<R, V>(V visitor) where V : IOptionDeserializerVisitor<R>;

    public T? ReadNullable<T, D>(D deserialize) where T : struct where D : IDeserialize<T>
        => ReadOption<T?, NullableOptionVisitor<T, D>>(new(deserialize));
}

public interface IOptionDeserializerVisitor<out R>
{
    public R VisitNone();
    public R VisitSome<D>(D deserializer, ISeraOptions options) where D : IDeserializer;
}

public partial interface IAsyncDeserializer
{
    public ValueTask<R> ReadOptionAsync<R, V>(V visitor) where V : IAsyncOptionDeserializerVisitor<R>;

    public ValueTask<T?> ReadNullableAsync<T, D>(D deserialize) where T : struct where D : IAsyncDeserialize<T>
        => ReadOptionAsync<T?, AsyncNullableOptionVisitor<T, D>>(new(deserialize));
}

public interface IAsyncOptionDeserializerVisitor<R>
{
    public ValueTask<R> VisitNoneAsync();
    public ValueTask<R> VisitSomeAsync<D>(D deserializer, ISeraOptions options) where D : IAsyncDeserializer;
}

public static partial class DeserializerEx
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool PeekIsOption<S>(this S self) where S : IDeserializer
        => (self.PeekNext(DeserializerHint.Option) & DeserializerHint.Option) != 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static async ValueTask<bool> PeekIsOptionAsync<S>(this S self) where S : IAsyncDeserializer
        => (await self.PeekNextAsync(DeserializerHint.Option) & DeserializerHint.Option) != 0;
}

#endregion

#region Seq

public partial interface IDeserializer
{
    public R ReadSeq<R, V>(nuint? len, V visitor) where V : ISeqDeserializerVisitor<R>;
}

public interface ISeqDeserializerVisitor<out R>
{
    public R VisitSeq<A>(A access) where A : ISeqAccess;
}

public interface ISeqAccess
{
    public nuint? GetLength();

    public bool HasNext();

    public T ReadElement<T, D>(out T result, D deserialize) where D : IDeserialize<T>;
}

public partial interface IAsyncDeserializer
{
    public ValueTask<R> ReadSeqAsync<R, V>(nuint? len, V visitor) where V : IAsyncSeqDeserializerVisitor<R>;
}

public interface IAsyncSeqDeserializerVisitor<R>
{
    public ValueTask<R> VisitSeqAsync<A>(A access) where A : IAsyncSeqAccess;
}

public interface IAsyncSeqAccess
{
    public ValueTask<nuint?> GetLengthAsync();

    public ValueTask<bool> HasNextAsync();

    public ValueTask<T> ReadElementAsync<T, D>(D deserialize) where D : IAsyncDeserialize<T>;
}

public static partial class DeserializerEx
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool PeekIsSeq<S>(this S self) where S : IDeserializer
        => (self.PeekNext(DeserializerHint.Seq) & DeserializerHint.Seq) != 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static async ValueTask<bool> PeekIsSeqAsync<S>(this S self) where S : IAsyncDeserializer
        => (await self.PeekNextAsync(DeserializerHint.Seq) & DeserializerHint.Seq) != 0;
}

#endregion

#region Map

public partial interface IDeserializer
{
    public R ReadMap<R, V>(nuint? len, V visitor) where V : IMapDeserializerVisitor<R>;
}

public interface IMapDeserializerVisitor<out R>
{
    public R VisitMap<A>(A access) where A : IMapAccess;
}

public interface IMapAccess
{
    public nuint? GetLength();

    public bool HasNext();

    public void ReadEntry<K, V, DK, DV>(out K key, out V value, DK key_deserialize, DV value_deserialize)
        where DK : IDeserialize<K> where DV : IDeserialize<V>;
}

public partial interface IAsyncDeserializer
{
    public ValueTask<R> ReadMapAsync<R, V>(nuint? len, V visitor) where V : IAsyncMapDeserializerVisitor<R>;
}

public interface IAsyncMapDeserializerVisitor<R>
{
    public ValueTask<R> VisitMapAsync<A>(A access) where A : IAsyncMapAccess;
}

public interface IAsyncMapAccess
{
    public ValueTask<nuint?> GetLengthAsync();

    public ValueTask<bool> HasNextAsync();

    public ValueTask<KeyValuePair<K, V>> ReadEntryAsync<K, V, DK, DV>(DK key_deserialize, DV value_deserialize)
        where DK : IAsyncDeserialize<K> where DV : IAsyncDeserialize<V>;
}

public static partial class DeserializerEx
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool PeekIsMap<S>(this S self) where S : IDeserializer
        => (self.PeekNext(DeserializerHint.Map) & DeserializerHint.Map) != 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static async ValueTask<bool> PeekIsMapAsync<S>(this S self) where S : IAsyncDeserializer
        => (await self.PeekNextAsync(DeserializerHint.Map) & DeserializerHint.Map) != 0;
}

#endregion

#region Struct

public partial interface IDeserializer
{
    public R ReadStruct<R, V>(string? name, nuint? len, Memory<(string key, nuint? int_key)>? fields, V visitor)
        where V : IStructDeserializerVisitor<R>;
}

public interface IStructDeserializerVisitor<out R>
{
    public R VisitStructSeq<A>(A access) where A : IStructSeqAccess;
    public R VisitStructMap<A>(A access) where A : IStructMapAccess;
}

public interface IStructAccess
{
    public nuint? GetLength();

    public string? ViewStructName();
}

public interface IStructSeqAccess : IStructAccess
{
    public bool HasNext();

    public void ReadField<T, D>(out T result, D deserialize)
        where D : IDeserialize<T>;
}

public interface IStructMapAccess : IStructAccess
{
    public bool HasNext();

    /// <summary>
    /// At least one of string key and int key must be provided
    /// </summary>
    public void ReadField<T, D>(out string? key, out long? int_key, out T result, D deserialize)
        where D : IDeserialize<T>;
}

public partial interface IAsyncDeserializer
{
    public ValueTask<R> ReadStructAsync<R, V>(string? name, nuint? len, Memory<(string key, nuint? int_key)>? fields,
        V visitor)
        where V : IAsyncStructDeserializerVisitor<R>;
}

public interface IAsyncStructDeserializerVisitor<R>
{
    public ValueTask<R> VisitStructSeqAsync<A>(A access) where A : IAsyncStructSeqAccess;
    public ValueTask<R> VisitStructMapAsync<A>(A access) where A : IAsyncStructMapAccess;
}

public interface IAsyncStructAccess
{
    public ValueTask<nuint?> GetLengthAsync();

    public ValueTask<string?> ViewStructNameAsync();
}

public interface IAsyncStructSeqAccess : IAsyncStructAccess
{
    public ValueTask<bool> HasNextAsync();

    public ValueTask<T> ReadFieldAsync<T, D>(D deserialize)
        where D : IAsyncDeserialize<T>;
}

public interface IAsyncStructMapAccess : IAsyncStructAccess
{
    public ValueTask<bool> HasNextAsync();

    /// <summary>
    /// At least one of string key and int key must be provided
    /// </summary>
    public ValueTask<(string? key, long? int_key, T result)> ReadFieldAsync<T, D>(D deserialize)
        where D : IAsyncDeserialize<T>;
}

public static partial class DeserializerEx
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool PeekIsStruct<S>(this S self) where S : IDeserializer
        => (self.PeekNext(DeserializerHint.Struct) & DeserializerHint.Struct) != 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static async ValueTask<bool> PeekIsStructAsync<S>(this S self) where S : IAsyncDeserializer
        => (await self.PeekNextAsync(DeserializerHint.Struct) & DeserializerHint.Struct) != 0;
}

#endregion

#region Variant

public partial interface IDeserializer
{
    public R ReadVariant<R, V>(string? union_name, Memory<Variant>? variants, V visitor)
        where V : IVariantDeserializerVisitor<R>;
}

public interface IVariantDeserializerVisitor<out R>
{
    public R VisitEmptyUnion();
    public R VisitVariantUnit<A>(Variant variant, A access) where A : IVariantAccess;

    public R VisitVariant<A, D>(Variant variant, A access, D deserializer, ISeraOptions options)
        where A : IVariantAccess where D : IDeserializer;
}

public interface IVariantAccess
{
    public string? ViewUnionName();
}

public partial interface IAsyncDeserializer
{
    public ValueTask<R> ReadVariantAsync<R, V>(string? union_name, Memory<Variant>? variants, V visitor)
        where V : IAsyncVariantDeserializerVisitor<R>;
}

public interface IAsyncVariantDeserializerVisitor<R>
{
    public ValueTask<R> VisitEmptyUnionAsync();
    public ValueTask<R> VisitVariantUnitAsync<A>(Variant variant, A access) where A : IAsyncVariantAccess;

    public ValueTask<R> VisitVariantAsync<A, D>(Variant variant, A access, D deserializer, ISeraOptions options)
        where A : IAsyncVariantAccess where D : IAsyncDeserializer;
}

public interface IAsyncVariantAccess
{
    public ValueTask<string?> ViewUnionNameAsync();
}

public static partial class DeserializerEx
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool PeekIsVariant<S>(this S self) where S : IDeserializer
        => (self.PeekNext(DeserializerHint.Variant) & DeserializerHint.Variant) != 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static async ValueTask<bool> PeekIsVariantAsync<S>(this S self) where S : IAsyncDeserializer
        => (await self.PeekNextAsync(DeserializerHint.Variant) & DeserializerHint.Variant) != 0;
}

#endregion
