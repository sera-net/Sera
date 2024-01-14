using System;
using System.Buffers;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using BetterCollections;
using BetterCollections.Memories;
using Sera.Core;
using Sera.Core.Abstract;
using Sera.Core.Formats;
using Sera.Core.Impls.De;
using Sera.Json.Utils;
using Sera.Utils;

namespace Sera.Json.De;

public class JsonDeserializer(SeraJsonOptions options, AJsonReader reader) : AJsonDeserializer(options)
{
    public JsonDeserializer(AJsonReader reader) : this(reader.Options, reader) { }

    internal readonly AJsonReader reader = reader;

    public JsonDeserializer<T> MakeColctor<T>() => new(this);
}

public readonly struct JsonDeserializer<T>(JsonDeserializer impl) : ISeraColctor<T, T>
{
    #region Ability

    public string FormatName => impl.FormatName;
    public string FormatMIME => impl.FormatMIME;
    public SeraFormatType FormatType => impl.FormatType;
    public ISeraOptions Options => impl.Options;
    public ISeraColion<object?> RuntimeImpl => impl.RuntimeImpl;
    private AJsonReader reader => impl.reader;

    #endregion

    #region Select

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T CSelect<C, M, U>(C colion, M mapper, Type<U> u) where C : ISelectSeraColion<U> where M : ISeraMapper<U, T>
    {
        var token = reader.CurrentToken;
        Unsafe.SkipInit(out T r);
        var mark = SeraKinds.None;
        List<Exception>? ex = null;
        var pos = (long?)reader.Save();
        try
        {
            if (colion.Priorities.HasValue)
                if (CSelect<C, M, U>(colion, mapper, token, ref mark, ref r, ref ex, pos,
                        colion.Priorities.Value))
                    return r;
            if (CSelect<C, M, U>(colion, mapper, token, ref mark, ref r, ref ex, pos,
                    AJsonDeserializer.SelectPriorities))
                return r;
            var err_msg = $"Select failed at {token.Pos}";
            if (ex is null or { Count: 0 }) throw new JsonParseException(err_msg, token.Pos);
            if (ex is { Count: 1 } and [var ex0]) throw new JsonParseException(err_msg, token.Pos, ex0);
            var inner = new AggregateException(ex);
            throw new JsonParseException(err_msg, token.Pos, inner);
        }
        finally
        {
            reader.UnSave(pos.Value);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool CSelect<C, M, U>(in C colion, in M mapper, in JsonToken token,
        ref SeraKinds mark, ref T r, ref List<Exception>? ex, long? pos, ReadOnlyMemory<Any.Kind> kinds)
        where C : ISelectSeraColion<U> where M : ISeraMapper<U, T>
    {
        var c = new JsonDeserializer<U>.SelectSeraColctor(impl);
        foreach (var anyKind in kinds)
        {
            var kind = anyKind.ToKinds();
            if (mark.Has(kind)) continue;
            if (pos.HasValue)
            {
                try
                {
                    if (CSelect(colion, token, kind, ref c))
                    {
                        r = mapper.Map(c.Value);
                        return true;
                    }
                    else reader.Load(pos.Value);
                }
                catch (Exception e)
                {
                    ex ??= new();
                    ex.Add(e);
                    reader.Load(pos.Value);
                }
            }
            else
            {
                if (CSelect(colion, token, kind, ref c))
                {
                    r = mapper.Map(c.Value);
                    return true;
                }
            }
            mark.Set(kind);
        }
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool CSelect<C, U>(in C colion, in JsonToken token, SeraKinds kind,
        ref JsonDeserializer<U>.SelectSeraColctor c)
        where C : ISelectSeraColion<U>
    {
        switch (kind)
        {
            case SeraKinds.Primitive:
                return colion.SelectPrimitive<bool, JsonDeserializer<U>.SelectSeraColctor>(ref c);
            case SeraKinds.String:
                if (token.Kind is not JsonTokenKind.String) return false;
                return colion.SelectString<bool, JsonDeserializer<U>.SelectSeraColctor>(ref c);
            case SeraKinds.Bytes:
                if (token.Kind is not (JsonTokenKind.String or JsonTokenKind.ArrayStart)) return false;
                return colion.SelectBytes<bool, JsonDeserializer<U>.SelectSeraColctor>(ref c);
            case SeraKinds.Array:
                if (token.Kind is not JsonTokenKind.ArrayStart) return false;
                return colion.SelectArray<bool, JsonDeserializer<U>.SelectSeraColctor>(ref c);
            case SeraKinds.Unit:
                if (token.Kind is not JsonTokenKind.Null) return false;
                return colion.SelectUnit<bool, JsonDeserializer<U>.SelectSeraColctor>(ref c);
            case SeraKinds.Option:
                return colion.SelectOption<bool, JsonDeserializer<U>.SelectSeraColctor>(ref c);
            case SeraKinds.Entry:
                if (token.Kind is not JsonTokenKind.ArrayStart) return false;
                return colion.SelectEntry<bool, JsonDeserializer<U>.SelectSeraColctor>(ref c);
            case SeraKinds.Tuple:
                if (token.Kind is not JsonTokenKind.ArrayStart) return false;
                return colion.SelectTuple<bool, JsonDeserializer<U>.SelectSeraColctor>(ref c, null);
            case SeraKinds.Seq:
                if (token.Kind is not JsonTokenKind.ArrayStart) return false;
                return colion.SelectSeq<bool, JsonDeserializer<U>.SelectSeraColctor>(ref c, null);
            case SeraKinds.Map:
                if (token.Kind is not JsonTokenKind.ObjectStart) return false;
                return colion.SelectMap<bool, JsonDeserializer<U>.SelectSeraColctor>(ref c, null);
            case SeraKinds.Struct:
                if (token.Kind is not JsonTokenKind.ObjectStart) return false;
                return colion.SelectStruct<bool, JsonDeserializer<U>.SelectSeraColctor>(ref c, null, null);
            case SeraKinds.Union:
                return colion.SelectUnion<bool, JsonDeserializer<U>.SelectSeraColctor>(ref c, null);
            default:
                throw new ArgumentOutOfRangeException(nameof(kind), kind, null);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T CSelectPrimitive<C, M, U>(C colion, M mapper, Type<U> u) where C : ISelectPrimitiveSeraColion<U>
        where M : ISeraMapper<U, T>
    {
        var token = reader.CurrentToken;
        Unsafe.SkipInit(out T r);
        var mark = SeraPrimitiveKinds.None;
        List<Exception>? ex = null;
        var pos = (long?)reader.Save();
        try
        {
            if (colion.Priorities.HasValue)
                if (CSelectPrimitive<C, M, U>(colion, mapper, token, ref mark, ref r, ref ex, pos,
                        colion.Priorities.Value))
                    return r;
            if (CSelectPrimitive<C, M, U>(colion, mapper, token, ref mark, ref r, ref ex, pos,
                    AJsonDeserializer.SelectPrimitivePriorities))
                return r;
            var err_msg = $"Select Primitive failed at {token.Pos}";
            if (ex is null or { Count: 0 }) throw new JsonParseException(err_msg, token.Pos);
            if (ex is { Count: 1 } and [var ex0]) throw new JsonParseException(err_msg, token.Pos, ex0);
            var inner = new AggregateException(ex);
            throw new JsonParseException(err_msg, token.Pos, inner);
        }
        finally
        {
            reader.UnSave(pos.Value);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool CSelectPrimitive<C, M, U>(in C colion, in M mapper, in JsonToken token,
        ref SeraPrimitiveKinds mark, ref T r, ref List<Exception>? ex, long? pos,
        ReadOnlyMemory<SeraPrimitiveTypes> types)
        where C : ISelectPrimitiveSeraColion<U> where M : ISeraMapper<U, T>
    {
        var c = new JsonDeserializer<U>.SelectSeraColctor(impl);
        foreach (var type in types)
        {
            var kind = type.ToKinds();
            if (mark.Has(kind)) continue;
            if (pos.HasValue)
            {
                try
                {
                    if (CSelectPrimitive(in colion, in token, type, ref c))
                    {
                        r = mapper.Map(c.Value);
                        return true;
                    }
                    else reader.Load(pos.Value);
                }
                catch (Exception e)
                {
                    ex ??= new();
                    ex.Add(e);
                    reader.Load(pos.Value);
                }
            }
            else
            {
                if (CSelectPrimitive(in colion, in token, type, ref c))
                {
                    r = mapper.Map(c.Value);
                    return true;
                }
            }
            mark.Set(kind);
        }
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool CSelectPrimitive<C, U>(in C colion, in JsonToken token, SeraPrimitiveTypes type,
        ref JsonDeserializer<U>.SelectSeraColctor c)
        where C : ISelectPrimitiveSeraColion<U>
    {
        switch (type)
        {
            case SeraPrimitiveTypes.Boolean:
                if (token.Kind is not (JsonTokenKind.True or JsonTokenKind.False)) return false;
                break;
            case SeraPrimitiveTypes.SByte:
            case SeraPrimitiveTypes.Byte:
            case SeraPrimitiveTypes.Int16:
            case SeraPrimitiveTypes.UInt16:
            case SeraPrimitiveTypes.Int32:
            case SeraPrimitiveTypes.UInt32:
            case SeraPrimitiveTypes.Int64:
            case SeraPrimitiveTypes.UInt64:
            case SeraPrimitiveTypes.Int128:
            case SeraPrimitiveTypes.UInt128:
            case SeraPrimitiveTypes.IntPtr:
            case SeraPrimitiveTypes.UIntPtr:
            case SeraPrimitiveTypes.Half:
            case SeraPrimitiveTypes.Single:
            case SeraPrimitiveTypes.Double:
            case SeraPrimitiveTypes.Decimal:
            case SeraPrimitiveTypes.NFloat:
            case SeraPrimitiveTypes.BigInteger:
            case SeraPrimitiveTypes.TimeSpan:
            case SeraPrimitiveTypes.DateOnly:
            case SeraPrimitiveTypes.TimeOnly:
            case SeraPrimitiveTypes.DateTime:
            case SeraPrimitiveTypes.DateTimeOffset:
            case SeraPrimitiveTypes.Index:
                if (token.Kind is not (JsonTokenKind.Number or JsonTokenKind.String)) return false;
                break;
            case SeraPrimitiveTypes.Complex:
                if (token.Kind is not (JsonTokenKind.String or JsonTokenKind.ArrayStart))
                    return false;
                break;
            case SeraPrimitiveTypes.Guid:
            case SeraPrimitiveTypes.Range:
            case SeraPrimitiveTypes.Char:
            case SeraPrimitiveTypes.Rune:
            case SeraPrimitiveTypes.Uri:
            case SeraPrimitiveTypes.Version:
                if (token.Kind is not JsonTokenKind.String) return false;
                break;
        }
        return colion.SelectPrimitiveDetail<bool, JsonDeserializer<U>.SelectSeraColctor>(ref c, type);
    }

    private struct SelectSeraColctor(JsonDeserializer impl) : ISelectSeraColctor<T, bool>
    {
        public T Value = default!;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool CSome<C, M, U>(C colion, M mapper, Type<U> u) where C : ISeraColion<U> where M : ISeraMapper<U, T>
        {
            var c = new JsonDeserializer<U>(impl);
            var r = colion.Collect<U, JsonDeserializer<U>>(ref c);
            Value = mapper.Map(r);
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool CNone() => false;
    }

    #endregion

    #region Primitive

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T CPrimitive<M>(M mapper, Type<bool> t, SeraFormats? formats = null)
        where M : ISeraMapper<bool, T>
    {
        var token = reader.CurrentToken;
        Unsafe.SkipInit(out T r);
        if (token.Kind is JsonTokenKind.True) r = mapper.Map(true);
        else if (token.Kind is JsonTokenKind.False) r = mapper.Map(false);
        else if (token.Kind is JsonTokenKind.Number) goto try_number;
        else if (token.Kind is JsonTokenKind.String)
        {
            if (bool.TryParse(token.AsSpan(), out var b)) r = mapper.Map(b);
            else goto try_number;
        }
        else goto err;
        ret:
        reader.MoveNext();
        return r;
        err:
        throw new JsonParseException($"Expected Bool but found {token.Kind} at {token.Pos}", token.Pos);
        try_number:
        if (decimal.TryParse(token.AsSpan(), NumberStyles.Any, null, out var num))
            r = mapper.Map(num != 0);
        else goto err;
        goto ret;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T CPrimitive<M>(M mapper, Type<sbyte> t, SeraFormats? formats = null) where M : ISeraMapper<sbyte, T>
        => CPrimitiveNumber<sbyte, M>(mapper, formats);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T CPrimitive<M>(M mapper, Type<byte> t, SeraFormats? formats = null) where M : ISeraMapper<byte, T>
        => CPrimitiveNumber<byte, M>(mapper, formats);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T CPrimitive<M>(M mapper, Type<short> t, SeraFormats? formats = null) where M : ISeraMapper<short, T>
        => CPrimitiveNumber<short, M>(mapper, formats);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T CPrimitive<M>(M mapper, Type<ushort> t, SeraFormats? formats = null) where M : ISeraMapper<ushort, T>
        => CPrimitiveNumber<ushort, M>(mapper, formats);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T CPrimitive<M>(M mapper, Type<int> t, SeraFormats? formats = null) where M : ISeraMapper<int, T>
        => CPrimitiveNumber<int, M>(mapper, formats);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T CPrimitive<M>(M mapper, Type<uint> t, SeraFormats? formats = null) where M : ISeraMapper<uint, T>
        => CPrimitiveNumber<uint, M>(mapper, formats);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T CPrimitive<M>(M mapper, Type<long> t, SeraFormats? formats = null) where M : ISeraMapper<long, T>
        => CPrimitiveNumber<long, M>(mapper, formats);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T CPrimitive<M>(M mapper, Type<ulong> t, SeraFormats? formats = null) where M : ISeraMapper<ulong, T>
        => CPrimitiveNumber<ulong, M>(mapper, formats);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T CPrimitive<M>(M mapper, Type<Int128> t, SeraFormats? formats = null) where M : ISeraMapper<Int128, T>
        => CPrimitiveNumber<Int128, M>(mapper, formats);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T CPrimitive<M>(M mapper, Type<UInt128> t, SeraFormats? formats = null) where M : ISeraMapper<UInt128, T>
        => CPrimitiveNumber<UInt128, M>(mapper, formats);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T CPrimitive<M>(M mapper, Type<IntPtr> t, SeraFormats? formats = null) where M : ISeraMapper<IntPtr, T>
        => CPrimitiveNumber<IntPtr, M>(mapper, formats);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T CPrimitive<M>(M mapper, Type<UIntPtr> t, SeraFormats? formats = null) where M : ISeraMapper<UIntPtr, T>
        => CPrimitiveNumber<UIntPtr, M>(mapper, formats);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T CPrimitive<M>(M mapper, Type<Half> t, SeraFormats? formats = null) where M : ISeraMapper<Half, T>
        => CPrimitiveNumber<Half, M>(mapper, formats);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T CPrimitive<M>(M mapper, Type<float> t, SeraFormats? formats = null) where M : ISeraMapper<float, T>
        => CPrimitiveNumber<float, M>(mapper, formats);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T CPrimitive<M>(M mapper, Type<double> t, SeraFormats? formats = null) where M : ISeraMapper<double, T>
        => CPrimitiveNumber<double, M>(mapper, formats);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T CPrimitive<M>(M mapper, Type<decimal> t, SeraFormats? formats = null) where M : ISeraMapper<decimal, T>
        => CPrimitiveNumber<decimal, M>(mapper, formats);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T CPrimitive<M>(M mapper, Type<NFloat> t, SeraFormats? formats = null) where M : ISeraMapper<NFloat, T>
        => CPrimitiveNumber<NFloat, M>(mapper, formats);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T CPrimitive<M>(M mapper, Type<BigInteger> t, SeraFormats? formats = null)
        where M : ISeraMapper<BigInteger, T>
        => CPrimitiveNumber<BigInteger, M>(mapper, formats);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T CPrimitive<M>(M mapper, Type<Complex> t, SeraFormats? formats = null) where M : ISeraMapper<Complex, T>
    {
        var token = reader.CurrentToken;
        Unsafe.SkipInit(out Complex r);
        if (token.Kind is JsonTokenKind.String)
        {
            if (token.AsSpan().TryParseComplex(formats.GetNumberStyles(), out r))
            {
                reader.MoveNext();
                goto ok;
            }
            throw new JsonParseException($"Illegal Complex format at {token.Pos}", token.Pos);
        }
        reader.ReadArrayStart();
        var c = new JsonDeserializer<double>(impl);
        var real = c.CPrimitiveNumber<double, IdentityMapper<double>>(new(), formats);
        reader.ReadComma();
        var imaginary = c.CPrimitiveNumber<double, IdentityMapper<double>>(new(), formats);
        reader.ReadArrayEnd();
        r = new Complex(real, imaginary);
        ok:
        return mapper.Map(r);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T CPrimitive<M>(M mapper, Type<TimeSpan> t, SeraFormats? formats = null) where M : ISeraMapper<TimeSpan, T>
        => CPrimitiveDateTime<TimeSpan, M, DateOrTimeFromNumberMapper>(mapper, nameof(TimeSpan), new());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T CPrimitive<M>(M mapper, Type<DateOnly> t, SeraFormats? formats = null) where M : ISeraMapper<DateOnly, T>
        => CPrimitiveDateTime<DateOnly, M, DateOrTimeFromNumberMapper>(mapper, nameof(DateOnly), new());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T CPrimitive<M>(M mapper, Type<TimeOnly> t, SeraFormats? formats = null) where M : ISeraMapper<TimeOnly, T>
        => CPrimitiveDateTime<TimeOnly, M, DateOrTimeFromNumberMapper>(mapper, nameof(TimeOnly), new());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T CPrimitive<M>(M mapper, Type<DateTime> t, SeraFormats? formats = null) where M : ISeraMapper<DateTime, T>
    {
        var token = reader.CurrentToken;
        var r = token.ParseDateOrTime<DateTime, DateOrTimeFromNumberWithOffsetMapper>(nameof(DateTime),
            new(Options.TimeZone.BaseUtcOffset));
        r = TimeZoneInfo.ConvertTime(r, Options.TimeZone);
        reader.MoveNext();
        return mapper.Map(r);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T CPrimitive<M>(M mapper, Type<DateTimeOffset> t, SeraFormats? formats = null)
        where M : ISeraMapper<DateTimeOffset, T>
    {
        var token = reader.CurrentToken;
        var r = token.ParseDateOrTime<DateTimeOffset, DateOrTimeFromNumberWithOffsetMapper>(nameof(DateTimeOffset),
            new(Options.TimeZone.BaseUtcOffset));
        if (formats?.DateTimeFormat.HasFlag(DateTimeFormatFlags.DateTimeOffsetUseTimeZone) ?? false)
        {
            r = TimeZoneInfo.ConvertTime(r, Options.TimeZone);
        }
        reader.MoveNext();
        return mapper.Map(r);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T CPrimitive<M>(M mapper, Type<Guid> t, SeraFormats? formats = null) where M : ISeraMapper<Guid, T>
    {
        var token = reader.ReadStringToken();
        Guid v;
        try
        {
            var span = token.AsSpan();
            v = Guid.Parse(span);
        }
        catch (Exception e)
        {
            throw new JsonParseException($"Illegal Guid format at {token.Pos}", token.Pos, e);
        }
        return mapper.Map(v);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T CPrimitive<M>(M mapper, Type<Range> t, SeraFormats? formats = null) where M : ISeraMapper<Range, T>
    {
        var token = reader.ReadStringToken();
        var span = token.AsSpan();
        if (!span.TryParseRange(out var range))
            throw new JsonParseException($"Illegal Range format at {token.Pos}", token.Pos);
        return mapper.Map(range);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T CPrimitive<M>(M mapper, Type<Index> t, SeraFormats? formats = null) where M : ISeraMapper<Index, T>
    {
        var token = reader.CurrentToken;
        if (token.Kind is not (JsonTokenKind.Number or JsonTokenKind.String))
        {
            reader.ThrowExpected(JsonTokenKind.Number, JsonTokenKind.String);
        }
        var span = token.AsSpan();
        if (!span.TryParseIndex(out var index))
            throw new JsonParseException($"Illegal Index format at {token.Pos}", token.Pos);
        reader.MoveNext();
        return mapper.Map(index);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T CPrimitive<M>(M mapper, Type<char> t, SeraFormats? formats = null) where M : ISeraMapper<char, T>
    {
        var token = reader.ReadStringToken();
        var span = token.AsSpan();
        if (span.Length != 1)
            throw new JsonParseException($"char must be a string of only one character at {token.Pos}", token.Pos);
        return mapper.Map(span[0]);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T CPrimitive<M>(M mapper, Type<Rune> t, SeraFormats? formats = null) where M : ISeraMapper<Rune, T>
    {
        var token = reader.ReadStringToken();
        var span = token.AsSpan();
        var r = Rune.DecodeFromUtf16(span, out var rune, out var len);
        if (r != OperationStatus.Done)
            throw new JsonParseException($"Decode Rune failed ({r}) at {token.Pos}", token.Pos);
        if (span.Length != len)
            throw new JsonParseException($"String too long for Rune at {token.Pos}", token.Pos);
        return mapper.Map(rune);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T CPrimitive<M>(M mapper, Type<Uri> t, SeraFormats? formats = null) where M : ISeraMapper<Uri, T>
    {
        var token = reader.ReadStringToken();
        var str = token.AsString();
        Uri v;
        try
        {
            v = new Uri(str);
        }
        catch (Exception e)
        {
            throw new JsonParseException($"Illegal Uri format at {token.Pos}", token.Pos, e);
        }
        return mapper.Map(v);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T CPrimitive<M>(M mapper, Type<Version> t, SeraFormats? formats = null) where M : ISeraMapper<Version, T>
    {
        var token = reader.ReadStringToken();
        var span = token.AsSpan();
        Version v;
        try
        {
            v = Version.Parse(span);
        }
        catch (Exception e)
        {
            throw new JsonParseException($"Illegal Version format at {token.Pos}", token.Pos, e);
        }
        return mapper.Map(v);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private T CPrimitiveNumber<N, M>(M mapper, SeraFormats? formats = null)
        where M : ISeraMapper<N, T> where N : INumberBase<N>
    {
        var token = reader.CurrentToken;
        Unsafe.SkipInit(out N r);
        if (token.Kind is JsonTokenKind.Number or JsonTokenKind.String)
        {
            var style = formats.GetNumberStyles();
            r = token.ParseNumber<N>(style);
        }
        else reader.ThrowExpected(JsonTokenKind.Number, JsonTokenKind.String);
        reader.MoveNext();
        return mapper.Map(r);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private T CPrimitiveDateTime<N, M, C>(M mapper, string TypeName, C FromTicks)
        where M : ISeraMapper<N, T> where N : ISpanParsable<N> where C : ISeraMapper<long, N>
    {
        var token = reader.CurrentToken;
        var r = token.ParseDateOrTime<N, C>(TypeName, FromTicks);
        reader.MoveNext();
        return mapper.Map(r);
    }

    #endregion

    #region String

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T CString<M>(M mapper, Type<string> t) where M : ISeraMapper<string, T>
    {
        var token = reader.ReadStringToken();
        return mapper.Map(token.AsString());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T CString<M>(M mapper, Type<char[]> t) where M : ISeraMapper<char[], T>
        => mapper.Map(CStringArray());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T CString<M>(M mapper, Type<Memory<char>> t) where M : ISeraMapper<Memory<char>, T>
        => mapper.Map(CStringArray());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private char[] CStringArray()
    {
        var token = reader.ReadStringToken();
        return token.AsSpan().ToArray();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T CString<M>(M mapper, Type<ReadOnlyMemory<char>> t) where M : ISeraMapper<ReadOnlyMemory<char>, T>
    {
        var token = reader.ReadStringToken();
        return mapper.Map(token.AsMemory());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T CStringSpan<M>(M mapper) where M : ISeraSpanMapper<char, T>
    {
        var token = reader.ReadStringToken();
        return mapper.SpanMap(token.AsSpan());
    }

    #endregion

    #region Bytes

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T CBytes<M>(M mapper, Type<byte[]> t) where M : ISeraMapper<byte[], T>
        => mapper.Map(CBytes().ToArray());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T CBytes<M>(M mapper, Type<Memory<byte>> t) where M : ISeraMapper<Memory<byte>, T>
        => mapper.Map(CBytes());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T CBytes<M>(M mapper, Type<ReadOnlyMemory<byte>> t) where M : ISeraMapper<ReadOnlyMemory<byte>, T>
        => mapper.Map(CBytes());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T CBytesSpan<M>(M mapper) where M : ISeraSpanMapper<byte, T>
        => mapper.SpanMap(CBytes().Span);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private Memory<byte> CBytes()
    {
        var token = reader.CurrentToken;
        if (token.Kind is JsonTokenKind.String)
        {
            var span = token.AsSpan();
            var size = Base64.GetMaxDecodedFromUtf8Length(span.Length);
            var arr = new byte[size];
            if (!Convert.TryFromBase64Chars(span, arr, out var len))
                throw new JsonParseException($"Decoding base64 failed at {token.Pos}", token.Pos);
            reader.MoveNext();
            return arr.AsMemory(0, len);
        }
        var vec = new Vec<byte>();
        reader.ReadArrayStart();
        var first = true;
        var c = new JsonDeserializer<short>(impl);
        for (; reader.CurrentHas;)
        {
            token = reader.CurrentToken;
            if (token.Kind is JsonTokenKind.ArrayEnd) break;
            if (first) first = false;
            else reader.ReadComma();
            var v = (byte)c.CPrimitiveNumber<short, IdentityMapper<short>>(new());
            vec.Add(v);
        }
        reader.ReadArrayEnd();
        return vec.AsMemory;
    }

    #endregion

    #region Array

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T CArray<C, M, I>(C colion, M mapper, Type<I[]> t, Type<I> i)
        where C : ISeraColion<I> where M : ISeraMapper<I[], T>
        => mapper.Map(CArray<C, I>(colion).ToArray());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T CArray<C, M, I>(C colion, M mapper, Type<Memory<I>> t, Type<I> i)
        where C : ISeraColion<I> where M : ISeraMapper<Memory<I>, T>
        => mapper.Map(CArray<C, I>(colion).AsMemory);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T CArray<C, M, I>(C colion, M mapper, Type<ReadOnlyMemory<I>> t, Type<I> i)
        where C : ISeraColion<I> where M : ISeraMapper<ReadOnlyMemory<I>, T>
        => mapper.Map(CArray<C, I>(colion).AsMemory);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T CArraySpan<C, M, I>(C colion, M mapper, Type<I> i) where C : ISeraColion<I> where M : ISeraSpanMapper<I, T>
    {
        var arr = CArray<C, I>(colion);
        return mapper.SpanMap(arr.AsSpan);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private Vec<I> CArray<C, I>(C colion) where C : ISeraColion<I>
    {
        reader.ReadArrayStart();
        var vec = new Vec<I>();
        var first = true;
        for (; reader.CurrentHas;)
        {
            var token = reader.CurrentToken;
            if (token.Kind is JsonTokenKind.ArrayEnd) break;
            if (first) first = false;
            else reader.ReadComma();
            var c = new JsonDeserializer<I>(impl);
            var version = reader.Version;
            var i = colion.Collect<I, JsonDeserializer<I>>(ref c);
            reader.AssertMove(version);
            vec.Add(i);
        }
        reader.ReadArrayEnd();
        return vec;
    }

    #endregion

    #region Unit

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T CUnit<N>(N ctor) where N : ISeraCtor<T>
    {
        reader.ReadNull();
        return ctor.Ctor();
    }

    #endregion

    #region Option

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T COption<C>(C colion) where C : IOptionSeraColion<T>
    {
        var token = reader.CurrentToken;
        if (token.Kind is JsonTokenKind.Null)
        {
            reader.MoveNext();
            return colion.CtorNone();
        }
        var colctor = new SomeSeraColctor(impl);
        return colion.CollectSome<T, SomeSeraColctor>(ref colctor);
    }

    private readonly struct SomeSeraColctor(JsonDeserializer impl) : ISomeSeraColctor<T, T>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T CSome<C, M, U>(C colion, M mapper, Type<U> u) where C : ISeraColion<U> where M : ISeraMapper<U, T>
        {
            var colctor = new JsonDeserializer<U>(impl);
            var r = colion.Collect<U, JsonDeserializer<U>>(ref colctor);
            return mapper.Map(r);
        }
    }

    #endregion

    #region Entry

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T CEntry<C, B, M>(C colion, M mapper, Type<B> b) where C : IEntrySeraColion<B> where M : ISeraMapper<B, T>
    {
        reader.ReadArrayStart();
        var colctor = new EntrySeraColctor<B>(colion.Builder(), impl);
        colion.CollectKey<Unit, EntrySeraColctor<B>>(ref colctor);
        reader.ReadComma();
        colion.CollectValue<Unit, EntrySeraColctor<B>>(ref colctor);
        reader.ReadArrayEnd();
        return mapper.Map(colctor.builder);
    }

    private struct EntrySeraColctor<B>(B builder, JsonDeserializer impl) : IEntrySeraColctor<B, Unit>
    {
        public B builder = builder;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Unit CItem<C, E, I>(C colion, E effector, Type<I> i)
            where C : ISeraColion<I> where E : ISeraEffector<B, I>
        {
            var c = new JsonDeserializer<I>(impl);
            var r = colion.Collect<I, JsonDeserializer<I>>(ref c);
            effector.Effect(ref builder, r);
            return default;
        }
    }

    #endregion

    #region Tuple

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T CTuple<C, B, M>(C colion, M mapper, Type<B> b)
        where C : ITupleSeraColion<B> where M : ISeraMapper<B, T>
    {
        reader.ReadArrayStart();
        var size = colion.Size;
        var colctor = new TupleSeraColctor<B>(colion.Builder(), impl);
        var first = true;
        var i = 0;
        if (size.HasValue)
        {
            for (; i < size; i++)
            {
                if (first) first = false;
                else reader.ReadComma();
                var version = reader.Version;
                var err = colion.CollectItem<bool, TupleSeraColctor<B>>(ref colctor, i);
                if (err) throw new DeserializeException($"Unable to read item {i} of tuple {typeof(T)}");
                reader.AssertMove(version);
            }
        }
        else
        {
            for (; reader.CurrentHas; i++)
            {
                var token = reader.CurrentToken;
                if (token.Kind is JsonTokenKind.ArrayEnd) break;
                if (first) first = false;
                else reader.ReadComma();
                var version = reader.Version;
                var err = colion.CollectItem<bool, TupleSeraColctor<B>>(ref colctor, i);
                if (err) throw new DeserializeException($"Unable to read item {i} of tuple {typeof(T)}");
                reader.AssertMove(version);
            }
        }
        reader.ReadArrayEnd();
        if (!colion.FinishCollect(i)) throw new DeserializeException("Failed to collect tuple");
        return mapper.Map(colctor.builder);
    }

    private struct TupleSeraColctor<B>(B builder, JsonDeserializer impl) : ITupleSeraColctor<B, bool>
    {
        public B builder = builder;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool CItem<C, E, I>(C colion, E effector, Type<I> i)
            where C : ISeraColion<I> where E : ISeraEffector<B, I>
        {
            var c = new JsonDeserializer<I>(impl);
            var r = colion.Collect<I, JsonDeserializer<I>>(ref c);
            effector.Effect(ref builder, r);
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool CNone() => true;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool CRest<C, E, I>(C colion, E effector, Type<I> i) where C : ITupleRestSeraColion<I>
            where E : ISeraEffector<B, I>
        {
            var c = new JsonDeserializer<I>.TupleRestSeraColctor(impl);
            var r = colion.CollectRest<I, JsonDeserializer<I>.TupleRestSeraColctor>(ref c);
            effector.Effect(ref builder, r);
            return false;
        }
    }

    private readonly struct TupleRestSeraColctor(JsonDeserializer impl) : ITupleRestSeraColctor<T, T>
    {
        private AJsonReader reader => impl.reader;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T CTupleRest<C, B, M>(C colion, M mapper, Type<B> b)
            where C : ITupleSeraColion<B> where M : ISeraMapper<B, T>
        {
            var size = colion.Size;
            var colctor = new TupleSeraColctor<B>(colion.Builder(), impl);
            var first = true;
            var i = 0;
            if (size.HasValue)
            {
                for (; i < size; i++)
                {
                    if (first) first = false;
                    else reader.ReadComma();
                    var version = reader.Version;
                    var err = colion.CollectItem<bool, TupleSeraColctor<B>>(ref colctor, i);
                    if (err) throw new DeserializeException($"Unable to read item {i} of tuple {typeof(T)}");
                    reader.AssertMove(version);
                }
            }
            else
            {
                for (; reader.CurrentHas; i++)
                {
                    var token = reader.CurrentToken;
                    if (token.Kind is JsonTokenKind.ArrayEnd) break;
                    if (first) first = false;
                    else reader.ReadComma();
                    var version = reader.Version;
                    var err = colion.CollectItem<bool, TupleSeraColctor<B>>(ref colctor, i);
                    if (err) throw new DeserializeException($"Unable to read item {i} of tuple {typeof(T)}");
                    reader.AssertMove(version);
                }
            }
            if (!colion.FinishCollect(i)) throw new DeserializeException("Failed to collect tuple");
            return mapper.Map(colctor.builder);
        }
    }

    #endregion

    #region Seq

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T CSeq<C, B, M, I>(C colion, M mapper, Type<B> b, Type<I> _i)
        where C : ISeqSeraColion<B, I> where M : ISeraMapper<B, T>
    {
        reader.ReadArrayStart();
        var colctor = new SeqSeraColctor<B, I>(colion.Builder(null), impl);
        var first = true;
        var i = 0;
        for (; reader.CurrentHas; i++)
        {
            var token = reader.CurrentToken;
            if (token.Kind is JsonTokenKind.ArrayEnd) break;
            if (first) first = false;
            else reader.ReadComma();
            var version = reader.Version;
            colion.CollectItem<Unit, SeqSeraColctor<B, I>>(ref colctor);
            reader.AssertMove(version);
        }
        reader.ReadArrayEnd();
        if (!colion.FinishCollect(i)) throw new DeserializeException("Failed to collect seq");
        return mapper.Map(colctor.builder);
    }

    private struct SeqSeraColctor<B, I>(B builder, JsonDeserializer impl) : ISeqSeraColctor<B, I, Unit>
    {
        public B builder = builder;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Unit CItem<C, E>(C colion, E effector)
            where C : ISeraColion<I> where E : ISeraEffector<B, I>
        {
            var c = new JsonDeserializer<I>(impl);
            var r = colion.Collect<I, JsonDeserializer<I>>(ref c);
            effector.Effect(ref builder, r);
            return default;
        }
    }

    #endregion

    #region Map

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T CMap<C, B, M, IK, IV>(C colion, M mapper, Type<B> b, Type<IK> k, Type<IV> v,
        ASeraTypeAbility? keyAbility = null)
        where C : IMapSeraColion<B, IK, IV> where M : ISeraMapper<B, T>
        => typeof(IK) == typeof(string)
            ? CObjectMapStringKey<C, B, M, IK, IV>(colion, mapper, keyAbility)
            : CArrayMap<C, B, M, IK, IV>(colion, mapper, keyAbility);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private T CArrayMap<C, B, M, IK, IV>(C colion, M mapper, ASeraTypeAbility? keyAbility)
        where C : IMapSeraColion<B, IK, IV> where M : ISeraMapper<B, T>
    {
        var token = reader.CurrentToken;
        if (token.Kind is JsonTokenKind.ObjectStart)
        {
            if (keyAbility?.IsAccept(SeraKinds.String) ?? false)
                return CObjectMapAsStringKey<C, B, M, IK, IV>(colion, mapper);
            return CObjectMapSubJsonKey<C, B, M, IK, IV>(colion, mapper);
        }
        reader.ReadArrayStart();
        var colctor = new ArrayMapSeraColctor<B, IK, IV>(colion.Builder(null), impl);
        var first = true;
        var i = 0;
        for (; reader.CurrentHas; i++)
        {
            token = reader.CurrentToken;
            if (token.Kind is JsonTokenKind.ArrayEnd) break;
            if (first) first = false;
            else reader.ReadComma();
            var version = reader.Version;
            colion.CollectItem<Unit, ArrayMapSeraColctor<B, IK, IV>>(ref colctor);
            reader.AssertMove(version);
        }
        reader.ReadArrayEnd();
        if (!colion.FinishCollect(i)) throw new DeserializeException("Failed to collect map");
        return mapper.Map(colctor.builder);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private T CObjectMapStringKey<C, B, M, IK, IV>(C colion, M mapper, ASeraTypeAbility? keyAbility)
        where C : IMapSeraColion<B, IK, IV> where M : ISeraMapper<B, T>
    {
        var token = reader.CurrentToken;
        if (token.Kind is JsonTokenKind.ArrayStart) return CArrayMap<C, B, M, IK, IV>(colion, mapper, keyAbility);
        if (keyAbility?.IsAccept(SeraKinds.String) ?? false)
            return CObjectMapAsStringKey<C, B, M, IK, IV>(colion, mapper);
        reader.ReadObjectStart();
        var colctor = new ObjectMapStringKeySeraColctor<B, IK, IV>(colion.Builder(null), impl);
        var first = true;
        var i = 0;
        for (; reader.CurrentHas; i++)
        {
            token = reader.CurrentToken;
            if (token.Kind is JsonTokenKind.ObjectEnd) break;
            if (first) first = false;
            else reader.ReadComma();
            var version = reader.Version;
            colion.CollectItem<Unit, ObjectMapStringKeySeraColctor<B, IK, IV>>(ref colctor);
            reader.AssertMove(version);
        }
        reader.ReadObjectEnd();
        if (!colion.FinishCollect(i)) throw new DeserializeException("Failed to collect map");
        return mapper.Map(colctor.builder);
    }

    private T CObjectMapSubJsonKey<C, B, M, IK, IV>(C colion, M mapper)
        where C : IMapSeraColion<B, IK, IV> where M : ISeraMapper<B, T>
    {
        reader.ReadObjectStart();
        var colctor = new ObjectMapSubJsonKeySeraColctor<B, IK, IV>(colion.Builder(null), impl);
        var first = true;
        var i = 0;
        for (; reader.CurrentHas; i++)
        {
            var token = reader.CurrentToken;
            if (token.Kind is JsonTokenKind.ObjectEnd) break;
            if (first) first = false;
            else reader.ReadComma();
            var version = reader.Version;
            colion.CollectItem<Unit, ObjectMapSubJsonKeySeraColctor<B, IK, IV>>(ref colctor);
            reader.AssertMove(version);
        }
        reader.ReadObjectEnd();
        if (!colion.FinishCollect(i)) throw new DeserializeException("Failed to collect map");
        return mapper.Map(colctor.builder);
    }

    private T CObjectMapAsStringKey<C, B, M, IK, IV>(C colion, M mapper)
        where C : IMapSeraColion<B, IK, IV> where M : ISeraMapper<B, T>
    {
        reader.ReadObjectStart();
        var colctor = new ObjectMapAsStringKeySeraColctor<B, IK, IV>(colion.Builder(null), impl);
        var first = true;
        var i = 0;
        for (; reader.CurrentHas; i++)
        {
            var token = reader.CurrentToken;
            if (token.Kind is JsonTokenKind.ObjectEnd) break;
            if (first) first = false;
            else reader.ReadComma();
            var version = reader.Version;
            colion.CollectItem<Unit, ObjectMapAsStringKeySeraColctor<B, IK, IV>>(ref colctor);
            reader.AssertMove(version);
        }
        reader.ReadObjectEnd();
        if (!colion.FinishCollect(i)) throw new DeserializeException("Failed to collect map");
        return mapper.Map(colctor.builder);
    }

    private struct ArrayMapSeraColctor<B, IK, IV>(B builder, JsonDeserializer impl) : IMapSeraColctor<B, IK, IV, Unit>
    {
        public B builder = builder;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Unit CItem<CK, CV, E>(CK keyColion, CV valueColion, E effector)
            where CK : ISeraColion<IK> where CV : ISeraColion<IV> where E : ISeraEffector<B, KeyValuePair<IK, IV>>
        {
            impl.reader.ReadArrayStart();
            var ck = new JsonDeserializer<IK>(impl);
            var vk = keyColion.Collect<IK, JsonDeserializer<IK>>(ref ck);
            impl.reader.ReadComma();
            var cv = new JsonDeserializer<IV>(impl);
            var vv = valueColion.Collect<IV, JsonDeserializer<IV>>(ref cv);
            impl.reader.ReadArrayEnd();
            effector.Effect(ref builder, new(vk, vv));
            return default;
        }
    }

    private struct ObjectMapStringKeySeraColctor<B, IK, IV>(B builder, JsonDeserializer impl)
        : IMapSeraColctor<B, IK, IV, Unit>
    {
        public B builder = builder;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Unit CItem<CK, CV, E>(CK keyColion, CV valueColion, E effector)
            where CK : ISeraColion<IK> where CV : ISeraColion<IV> where E : ISeraEffector<B, KeyValuePair<IK, IV>>
        {
            var vk = impl.reader.ReadString();
            impl.reader.ReadColon();
            var cv = new JsonDeserializer<IV>(impl);
            var vv = valueColion.Collect<IV, JsonDeserializer<IV>>(ref cv);
            effector.Effect(ref builder, new((IK)(object)vk, vv));
            return default;
        }
    }

    private struct ObjectMapSubJsonKeySeraColctor<B, IK, IV>(B builder, JsonDeserializer impl)
        : IMapSeraColctor<B, IK, IV, Unit>
    {
        public B builder = builder;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Unit CItem<CK, CV, E>(CK keyColion, CV valueColion, E effector)
            where CK : ISeraColion<IK> where CV : ISeraColion<IV> where E : ISeraEffector<B, KeyValuePair<IK, IV>>
        {
            IK vk;
            var key_token = impl.reader.ReadStringToken();
            try
            {
                vk = SeraJson.Deserializer
                    .Deserialize<IK>()
                    .Use(keyColion)
                    .Static.From.String(key_token.AsSpan());
            }
            catch (Exception e)
            {
                var pos = key_token.Pos;
                throw new JsonParseException($"Parsing object sub json key failed at {pos}", pos, e);
            }
            impl.reader.ReadColon();
            var cv = new JsonDeserializer<IV>(impl);
            var vv = valueColion.Collect<IV, JsonDeserializer<IV>>(ref cv);
            effector.Effect(ref builder, new(vk, vv));
            return default;
        }
    }

    private struct ObjectMapAsStringKeySeraColctor<B, IK, IV>(B builder, JsonDeserializer impl)
        : IMapSeraColctor<B, IK, IV, Unit>
    {
        public B builder = builder;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Unit CItem<CK, CV, E>(CK keyColion, CV valueColion, E effector)
            where CK : ISeraColion<IK> where CV : ISeraColion<IV> where E : ISeraEffector<B, KeyValuePair<IK, IV>>
        {
            var key_token = impl.reader.CurrentToken;
            if (key_token.Kind is not JsonTokenKind.String) impl.reader.ThrowExpected(JsonTokenKind.String);
            var ck = new JsonDeserializer<IK>(impl);
            var vk = keyColion.Collect<IK, JsonDeserializer<IK>>(ref ck);
            impl.reader.ReadColon();
            var cv = new JsonDeserializer<IV>(impl);
            var vv = valueColion.Collect<IV, JsonDeserializer<IV>>(ref cv);
            effector.Effect(ref builder, new(vk, vv));
            return default;
        }
    }

    #endregion

    #region Struct

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T CStruct<C, B, M>(C colion, M mapper, Type<B> b) where C : IStructSeraColion<B> where M : ISeraMapper<B, T>
    {
        reader.ReadObjectStart();
        if (colion.Fields is { } fields)
            return CStaticStruct<C, B, M>(colion, mapper, fields);
        else return CDynamicStruct<C, B, M>(colion, mapper);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private T CStaticStruct<C, B, M>(C colion, M mapper, SeraFieldInfos fields)
        where C : IStructSeraColion<B> where M : ISeraMapper<B, T>
    {
        var c = new StructSeraColctor<B>(colion.Builder(null), impl);
        var first = true;
        var i = 0;
        for (; reader.CurrentHas; i++)
        {
            var token = reader.CurrentToken;
            if (token.Kind is JsonTokenKind.ObjectEnd) break;
            if (first) first = false;
            else reader.ReadComma();
            var field_name = reader.ReadString();
            reader.ReadColon();
            if (!fields.TryGet(field_name, out var info))
            {
                reader.SkipValue();
                continue;
            }
            var key = long.TryParse(field_name, out var key_) ? (long?)key_ : null;
            var version = reader.Version;
            var r = colion.CollectField<StructRes, StructSeraColctor<B>>(ref c, info.index, field_name, key);
            switch (r)
            {
                case StructRes.None:
                    throw new DeserializeException(
                        $"Unable to read field {field_name}({info.index}) of struct {typeof(T)}");
                case StructRes.Skip:
                    reader.SkipValue();
                    break;
                case StructRes.Field:
                    reader.AssertMove(version);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        reader.ReadObjectEnd();
        if (!colion.FinishCollect(i)) throw new DeserializeException("Failed to collect tuple");
        return mapper.Map(c.builder);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private T CDynamicStruct<C, B, M>(C colion, M mapper)
        where C : IStructSeraColion<B> where M : ISeraMapper<B, T>
    {
        var c = new StructSeraColctor<B>(colion.Builder(null), impl);
        var first = true;
        var i = 0;
        for (; reader.CurrentHas; i++)
        {
            var token = reader.CurrentToken;
            if (token.Kind is JsonTokenKind.ObjectEnd) break;
            if (first) first = false;
            else reader.ReadComma();
            var field_name = reader.ReadString();
            reader.ReadColon();
            var key = long.TryParse(field_name, out var key_) ? (long?)key_ : null;
            var r = colion.CollectField<StructRes, StructSeraColctor<B>>(ref c, i, field_name, key);
            switch (r)
            {
                case StructRes.None:
                case StructRes.Skip:
                    reader.SkipValue();
                    break;
                case StructRes.Field:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        reader.ReadObjectEnd();
        if (!colion.FinishCollect(i)) throw new DeserializeException("Failed to collect tuple");
        return mapper.Map(c.builder);
    }

    private enum StructRes : byte
    {
        None,
        Skip,
        Field,
    }

    private struct StructSeraColctor<B>(B builder, JsonDeserializer impl) : IStructSeraColctor<B, StructRes>
    {
        public B builder = builder;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public StructRes CField<C, E, I>(C colion, E effector, Type<I> i)
            where C : ISeraColion<I> where E : ISeraEffector<B, I>
        {
            var c = new JsonDeserializer<I>(impl);
            var r = colion.Collect<I, JsonDeserializer<I>>(ref c);
            effector.Effect(ref builder, r);
            return StructRes.Field;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public StructRes CSkip() => StructRes.Skip;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public StructRes CNone() => StructRes.None;
    }

    #endregion

    #region Union

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T CUnionEmpty<N>(N ctor) where N : ISeraCtor<T>
    {
        var token = reader.CurrentToken;
        if (token.Kind is JsonTokenKind.Null) goto ok;
        if (token.Kind is JsonTokenKind.ObjectStart)
        {
            reader.ReadObjectStart();
            reader.ReadObjectEnd();
            goto ok;
        }
        if (token.Kind is JsonTokenKind.ArrayStart)
        {
            reader.ReadArrayStart();
            reader.ReadArrayEnd();
            // ReSharper disable once RedundantJumpStatement
            goto ok;
        }
        ok:
        return ctor.Ctor();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T CUnionVariants<C, B, M>(C colion, M mapper, Type<B> b, UnionStyle? union_style = null)
        where C : IVariantsSeraColion<B> where M : ISeraMapper<B, T>
    {
        var variants = colion.Variants;
        var token = reader.CurrentToken;
        (Variant variant, int index) info;
        if (token.Kind is JsonTokenKind.String or JsonTokenKind.Number)
        {
            var span = token.AsSpan();
            if (span.TryParseVariantTag(variants.TagKind, union_style?.VariantFormats, out var tag))
            {
                if (variants.TryGet(tag, out info)) goto ok;
            }
            {
                var str = token.AsString();
                if (variants.TryGet(str, out info)) goto ok;
            }
        }
        var format = union_style?.Format ?? UnionFormat.Any;
        return format switch
        {
            UnionFormat.External => CUnionVariantsExternal<C, B, M>(variants, colion, mapper, union_style),
            UnionFormat.Internal => CUnionVariantsInternal<C, B, M>(variants, colion, mapper, union_style),
            UnionFormat.Adjacent => CUnionVariantsAdjacent<C, B, M>(variants, colion, mapper, union_style),
            UnionFormat.Tuple => CUnionVariantsTuple<C, B, M>(variants, colion, mapper, union_style),
            UnionFormat.Untagged => CUnionVariantsUntagged<C, B, M>(colion, mapper),
            _ => CUnionVariantsAny<C, B, M>(variants, in token, colion, mapper, union_style),
        };
        ok:
        reader.MoveNext();
        var c = new JsonDeserializer<B>.JustVariantSeraColctor(impl);
        var r = colion.CollectVariant<B, JsonDeserializer<B>.JustVariantSeraColctor>(ref c, info.index);
        return mapper.Map(r);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T CUnionVariantsAny<C, B, M>(SeraVariantInfos variants, in JsonToken token,
        C colion, M mapper, UnionStyle? union_style)
        where C : IVariantsSeraColion<B> where M : ISeraMapper<B, T>
    {
        if (token.Kind is JsonTokenKind.ArrayStart)
            return CUnionVariantsTuple<C, B, M>(variants, colion, mapper, union_style);
        return CUnionVariantsExternal<C, B, M>(variants, colion, mapper, union_style);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T CUnionVariantsExternal<C, B, M>(SeraVariantInfos variants, C colion, M mapper, UnionStyle? union_style)
        where C : IVariantsSeraColion<B> where M : ISeraMapper<B, T>
    {
        var start = reader.ReadObjectStart();
        var key = reader.ReadStringToken();
        var span = key.AsSpan();
        (Variant variant, int index) info;
        if (span.TryParseVariantTag(variants.TagKind, union_style?.VariantFormats, out var tag))
        {
            if (variants.TryGet(tag, out info)) goto ok;
        }
        {
            var str = key.AsString();
            if (variants.TryGet(str, out info)) goto ok;
        }
        throw new JsonParseException($"Unknown Union Variant at {key.Pos}", key.Pos);
        ok:
        reader.ReadColon();
        var c = new JsonDeserializer<B>.ValueVariantSeraColctor(impl, start.Pos);
        var r = colion.CollectVariant<B, JsonDeserializer<B>.ValueVariantSeraColctor>(ref c, info.index);
        reader.ReadObjectEnd();
        return mapper.Map(r);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T CUnionVariantsTuple<C, B, M>(SeraVariantInfos variants, C colion, M mapper, UnionStyle? union_style)
        where C : IVariantsSeraColion<B> where M : ISeraMapper<B, T>
    {
        var start = reader.ReadArrayStart();
        var key = reader.CurrentToken;
        if (key.Kind is not (JsonTokenKind.Number or JsonTokenKind.String))
            reader.ThrowExpected(JsonTokenKind.Number, JsonTokenKind.String);
        var span = key.AsSpan();
        (Variant variant, int index) info;
        if (span.TryParseVariantTag(variants.TagKind, union_style?.VariantFormats, out var tag))
        {
            if (variants.TryGet(tag, out info)) goto ok;
        }
        {
            var str = key.AsString();
            if (variants.TryGet(str, out info)) goto ok;
        }
        throw new JsonParseException($"Unknown Union Variant at {key.Pos}", key.Pos);
        ok:
        reader.MoveNext();
        reader.ReadComma();
        var c = new JsonDeserializer<B>.ValueVariantSeraColctor(impl, start.Pos);
        var r = colion.CollectVariant<B, JsonDeserializer<B>.ValueVariantSeraColctor>(ref c, info.index);
        reader.ReadArrayEnd();
        return mapper.Map(r);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T CUnionVariantsUntagged<C, B, M>(C colion, M mapper)
        where C : IVariantsSeraColion<B> where M : ISeraMapper<B, T>
    {
        var c = new JsonDeserializer<B>.UntaggedUnionSeraColctor(impl);
        var r = colion.CollectUntagged<B, JsonDeserializer<B>.UntaggedUnionSeraColctor>(ref c);
        return mapper.Map(r);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T CUnionVariantsAdjacent<C, B, M>(SeraVariantInfos variants, C colion, M mapper, UnionStyle? union_style)
        where C : IVariantsSeraColion<B> where M : ISeraMapper<B, T>
    {
        var ast = reader.ReadObject();
        var style = union_style ?? UnionStyle.Default;
        if (!ast.Map.TryGetValue(style.AdjacentTagName, out var keys))
            throw new JsonParseException(
                $"The key \"{style.AdjacentTagName}\" does not exist in the object at {ast.ObjectStart.Pos}",
                ast.ObjectStart.Pos);
        var key = keys.SubLast.Value;

        (Variant variant, int index) variant;
        {
            var token = key.Value.Tag switch
            {
                JsonAst.Tags.String => key.Value.String,
                JsonAst.Tags.Number => key.Value.Number,
                _ => throw new JsonParseException($"Unexpected tag kind {key.Value.Tag} at {key.Value.Pos}",
                    key.Value.Pos)
            };

            var span = token.AsSpan();
            if (span.TryParseVariantTag(variants.TagKind, union_style?.VariantFormats, out var tag))
            {
                if (variants.TryGet(tag, out variant)) goto ok;
            }
            if (!variants.TryGet(span.ToString(), out variant))
                throw new JsonParseException($"Unknown Union Variant \"{span}\" at {key.Value.Pos}", key.Value.Pos);
            ok: ;
        }

        if (!ast.Map.TryGetValue(style.AdjacentValueName, out var values))
            throw new JsonParseException(
                $"The key \"{style.AdjacentValueName}\" does not exist in the object at {ast.ObjectStart.Pos}",
                ast.ObjectStart.Pos);
        var value = values.SubLast.Value;

        var ast_reader = AstJsonReader.Create(reader.Options, value.Value);
        var sub_deserializer = new JsonDeserializer(reader.Options, ast_reader);
        var c = new JsonDeserializer<B>.ValueVariantSeraColctor(sub_deserializer, ast.ObjectStart.Pos);

        var r = colion.CollectVariant<B, JsonDeserializer<B>.ValueVariantSeraColctor>(ref c, variant.index);
        return mapper.Map(r);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T CUnionVariantsInternal<C, B, M>(SeraVariantInfos variants, C colion, M mapper, UnionStyle? union_style)
        where C : IVariantsSeraColion<B> where M : ISeraMapper<B, T>
    {
        var ast = reader.ReadObject();
        var style = union_style ?? UnionStyle.Default;
        if (!ast.Map.TryGetValue(style.InternalTagName, out var keys))
            throw new JsonParseException(
                $"The key \"{style.InternalTagName}\" does not exist in the object at {ast.ObjectStart.Pos}",
                ast.ObjectStart.Pos);
        var keys_last = keys.SubLast;
        var key = keys_last.Value;

        (Variant variant, int index) variant;
        {
            var token = key.Value.Tag switch
            {
                JsonAst.Tags.String => key.Value.String,
                JsonAst.Tags.Number => key.Value.Number,
                _ => throw new JsonParseException($"Unexpected tag kind {key.Value.Tag} at {key.Value.Pos}",
                    key.Value.Pos)
            };

            var span = token.AsSpan();
            if (span.TryParseVariantTag(variants.TagKind, union_style?.VariantFormats, out var tag))
            {
                if (variants.TryGet(tag, out variant)) goto ok;
            }
            if (!variants.TryGet(span.ToString(), out variant))
                throw new JsonParseException($"Unknown Union Variant \"{span}\" at {key.Value.Pos}", key.Value.Pos);
            ok: ;
        }

        var removed_ast = ast with { Map = ast.Map.TryRemove(keys_last)! };
        Debug.Assert(removed_ast.Map != null);
        var c = new JsonDeserializer<B>.InternalVariantSeraColctor(impl, removed_ast, style);

        var r = colion.CollectVariant<B, JsonDeserializer<B>.InternalVariantSeraColctor>(ref c, variant.index);
        return mapper.Map(r);
    }

    private readonly struct JustVariantSeraColctor(JsonDeserializer impl) : IVariantSeraColctor<T, T>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T CVariant<N>(N ctor) where N : ISeraCtor<T>
            => ctor.Ctor();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T CVariantValue<C, M, I>(C colion, M mapper, Type<I> i)
            where C : ISeraColion<I> where M : ISeraMapper<I, T>
            => CNone();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T CVariantTuple<C, M, I>(C colion, M mapper, Type<I> i)
            where C : ITupleSeraColion<I> where M : ISeraMapper<I, T>
            => CNone();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T CVariantStruct<C, M, I>(C colion, M mapper, Type<I> i)
            where C : IStructSeraColion<I> where M : ISeraMapper<I, T>
            => CNone();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T CNone() => throw new DeserializeException($"Unable to read union {typeof(T)}");
    }

    private readonly struct ValueVariantSeraColctor(JsonDeserializer impl, SourcePos pos) : IVariantSeraColctor<T, T>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T CVariant<N>(N ctor) where N : ISeraCtor<T>
        {
            impl.reader.ReadNull();
            return ctor.Ctor();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T CVariantValue<C, M, I>(C colion, M mapper, Type<I> i)
            where C : ISeraColion<I> where M : ISeraMapper<I, T>
        {
            var c = new JsonDeserializer<I>(impl);
            var r = colion.Collect<I, JsonDeserializer<I>>(ref c);
            return mapper.Map(r);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T CVariantTuple<C, M, I>(C colion, M mapper, Type<I> i)
            where C : ITupleSeraColion<I> where M : ISeraMapper<I, T>
        {
            var c = new JsonDeserializer<I>(impl);
            var r = c.CTuple(colion, new IdentityMapper<I>(), new Type<I>());
            return mapper.Map(r);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T CVariantStruct<C, M, I>(C colion, M mapper, Type<I> i)
            where C : IStructSeraColion<I> where M : ISeraMapper<I, T>
        {
            var c = new JsonDeserializer<I>(impl);
            var r = c.CStruct(colion, new IdentityMapper<I>(), new Type<I>());
            return mapper.Map(r);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T CNone() => throw new JsonParseException($"Unable to read union {typeof(T)} at {pos}", pos);
    }

    private readonly struct UntaggedUnionSeraColctor(JsonDeserializer impl) : IUntaggedUnionSeraColctor<T, T>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T CSelect<C, M, U>(C colion, M mapper, Type<U> u)
            where C : ISelectSeraColion<U> where M : ISeraMapper<U, T>
        {
            var c = new JsonDeserializer<T>(impl);
            return c.CSelect(colion, mapper, u);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T CSelectPrimitive<C, M, U>(C colion, M mapper, Type<U> u) where C : ISelectPrimitiveSeraColion<U>
            where M : ISeraMapper<U, T>
        {
            var c = new JsonDeserializer<T>(impl);
            return c.CSelectPrimitive(colion, mapper, u);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T CNone() => throw new DeserializeException($"Unable to read union {typeof(T)}");
    }

    private readonly struct InternalVariantSeraColctor(JsonDeserializer impl, JsonAstObject ast, UnionStyle style)
        : IVariantSeraColctor<T, T>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T CVariant<N>(N ctor) where N : ISeraCtor<T>
        {
            return ctor.Ctor();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private JsonDeserializer GetValue()
        {
            if (!ast.Map.TryGetValue(style.InternalValueName, out var values))
                throw new JsonParseException(
                    $"The key \"{style.InternalValueName}\" does not exist in the object at {ast.ObjectStart.Pos}",
                    ast.ObjectStart.Pos);
            var value = values.SubLast.Value;

            var ast_reader = AstJsonReader.Create(impl.reader.Options, value.Value);
            var sub_deserializer = new JsonDeserializer(impl.reader.Options, ast_reader);

            return sub_deserializer;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T CVariantValue<C, M, I>(C colion, M mapper, Type<I> i)
            where C : ISeraColion<I> where M : ISeraMapper<I, T>
        {
            var sub_deserializer = GetValue();
            var c = new JsonDeserializer<I>(sub_deserializer);

            var r = colion.Collect<I, JsonDeserializer<I>>(ref c);
            return mapper.Map(r);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T CVariantTuple<C, M, I>(C colion, M mapper, Type<I> i)
            where C : ITupleSeraColion<I> where M : ISeraMapper<I, T>
        {
            var sub_deserializer = GetValue();
            var c = new JsonDeserializer<I>(sub_deserializer);

            var r = c.CTuple(colion, new IdentityMapper<I>(), new Type<I>());
            return mapper.Map(r);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T CVariantStruct<C, M, I>(C colion, M mapper, Type<I> i)
            where C : IStructSeraColion<I> where M : ISeraMapper<I, T>
        {
            var sub_ast = JsonAst.MakeObject(ast);
            var ast_reader = AstJsonReader.Create(impl.reader.Options, sub_ast);
            var sub_deserializer = new JsonDeserializer(impl.reader.Options, ast_reader);
            var c = new JsonDeserializer<I>(sub_deserializer);

            var r = c.CStruct(colion, new IdentityMapper<I>(), new Type<I>());
            return mapper.Map(r);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T CNone() => throw new JsonParseException($"Unable to read union {typeof(T)} at {ast.ObjectStart.Pos}",
            ast.ObjectStart.Pos);
    }

    #endregion
}
