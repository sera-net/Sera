﻿using System;
using System.Buffers;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using BetterCollections;
using BetterCollections.Memories;
using Sera.Core;
using Sera.Core.Formats;
using Sera.Core.Impls.De;
using Sera.Json.Utils;
using Sera.Utils;

namespace Sera.Json.De;

public class JsonDeserializer(SeraJsonOptions options, AJsonReader reader) : AJsonDeserializer(options)
{
    public JsonDeserializer(AJsonReader reader) : this(reader.Options, reader) { }

    internal readonly AJsonReader reader = reader;

    public T Collect<C, T>(C colion) where C : ISeraColion<T>
    {
        var c = new JsonDeserializer<T>(this);
        return colion.Collect<T, JsonDeserializer<T>>(ref c);
    }

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

    public T CSelect<C, M, U>(C colion, M mapper, Type<U> u) where C : ISelectSeraColion<U> where M : ISeraMapper<U, T>
    {
        var token = reader.CurrentToken;
        Unsafe.SkipInit(out T r);
        var mark = SeraKinds.None;
        List<Exception>? ex = null;
        var pos = reader.CanSeek ? (long?)reader.Save() : null;
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
            if (ex is not { Count: > 0 }) throw new JsonParseException(err_msg, token.Pos);
            var inner = new AggregateException(ex);
            throw new JsonParseException(err_msg, token.Pos, inner);
        }
        finally
        {
            if (pos.HasValue) reader.UnSave(pos.Value);
        }
    }

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
                return colion.SelectString<bool, JsonDeserializer<U>.SelectSeraColctor>(ref c, reader.Encoding);
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

    private struct SelectSeraColctor(JsonDeserializer impl) : ISelectSeraColctor<T, bool>
    {
        public T Value = default!;

        public bool CSome<C, M, U>(C colion, M mapper, Type<U> u) where C : ISeraColion<U> where M : ISeraMapper<U, T>
        {
            var c = new JsonDeserializer<U>(impl);
            var r = colion.Collect<U, JsonDeserializer<U>>(ref c);
            Value = mapper.Map(r);
            return true;
        }

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
            reader.ThrowExpected(JsonTokenKind.String);
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
        else reader.ThrowExpected(JsonTokenKind.Number);
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

    public T CBytes<M>(M mapper, Type<byte[]> t) where M : ISeraMapper<byte[], T>
        => mapper.Map(CBytes().ToArray());

    public T CBytes<M>(M mapper, Type<Memory<byte>> t) where M : ISeraMapper<Memory<byte>, T>
        => mapper.Map(CBytes());

    public T CBytes<M>(M mapper, Type<ReadOnlyMemory<byte>> t) where M : ISeraMapper<ReadOnlyMemory<byte>, T>
        => mapper.Map(CBytes());

    public T CBytesSpan<M>(M mapper) where M : ISeraSpanMapper<byte, T>
        => mapper.SpanMap(CBytes().Span);

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
        for (; reader.Has;)
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

    private Vec<I> CArray<C, I>(C colion) where C : ISeraColion<I>
    {
        reader.ReadArrayStart();
        var vec = new Vec<I>();
        var first = true;
        for (; reader.Has;)
        {
            var token = reader.CurrentToken;
            if (token.Kind is JsonTokenKind.ArrayEnd) break;
            if (first) first = false;
            else reader.ReadComma();
            var c = new JsonDeserializer<I>(impl);
            var cursor = reader.Cursor;
            var i = colion.Collect<I, JsonDeserializer<I>>(ref c);
            reader.AssertMove(cursor);
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
                var cursor = reader.Cursor;
                var err = colion.CollectItem<bool, TupleSeraColctor<B>>(ref colctor, i);
                if (err) throw new DeserializeException($"Unable to read item {i} of tuple {typeof(T)}");
                reader.AssertMove(cursor);
            }
        }
        else
        {
            for (; reader.Has; i++)
            {
                var token = reader.CurrentToken;
                if (token.Kind is JsonTokenKind.ArrayEnd) break;
                if (first) first = false;
                else reader.ReadComma();
                var cursor = reader.Cursor;
                var err = colion.CollectItem<bool, TupleSeraColctor<B>>(ref colctor, i);
                if (err) throw new DeserializeException($"Unable to read item {i} of tuple {typeof(T)}");
                reader.AssertMove(cursor);
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
                    var cursor = reader.Cursor;
                    var err = colion.CollectItem<bool, TupleSeraColctor<B>>(ref colctor, i);
                    if (err) throw new DeserializeException($"Unable to read item {i} of tuple {typeof(T)}");
                    reader.AssertMove(cursor);
                }
            }
            else
            {
                for (; reader.Has; i++)
                {
                    var token = reader.CurrentToken;
                    if (token.Kind is JsonTokenKind.ArrayEnd) break;
                    if (first) first = false;
                    else reader.ReadComma();
                    var cursor = reader.Cursor;
                    var err = colion.CollectItem<bool, TupleSeraColctor<B>>(ref colctor, i);
                    if (err) throw new DeserializeException($"Unable to read item {i} of tuple {typeof(T)}");
                    reader.AssertMove(cursor);
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
        for (; reader.Has; i++)
        {
            var token = reader.CurrentToken;
            if (token.Kind is JsonTokenKind.ArrayEnd) break;
            if (first) first = false;
            else reader.ReadComma();
            var cursor = reader.Cursor;
            colion.CollectItem<Unit, SeqSeraColctor<B, I>>(ref colctor);
            reader.AssertMove(cursor);
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
    public T CMap<C, B, M, IK, IV>(C colion, M mapper, Type<B> b, Type<IK> k, Type<IV> v)
        where C : IMapSeraColion<B, IK, IV> where M : ISeraMapper<B, T>
        => typeof(IK) == typeof(string)
            ? CObjectMapStringKey<C, B, M, IK, IV>(colion, mapper)
            : CArrayMap<C, B, M, IK, IV>(colion, mapper);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private T CArrayMap<C, B, M, IK, IV>(C colion, M mapper)
        where C : IMapSeraColion<B, IK, IV> where M : ISeraMapper<B, T>
    {
        var token = reader.CurrentToken;
        if (token.Kind is JsonTokenKind.ObjectStart) return CObjectMapSubJsonKey<C, B, M, IK, IV>(colion, mapper);
        reader.ReadArrayStart();
        var colctor = new ArrayMapSeraColctor<B, IK, IV>(colion.Builder(null), impl);
        var first = true;
        var i = 0;
        for (; reader.Has; i++)
        {
            token = reader.CurrentToken;
            if (token.Kind is JsonTokenKind.ArrayEnd) break;
            if (first) first = false;
            else reader.ReadComma();
            var cursor = reader.Cursor;
            colion.CollectItem<Unit, ArrayMapSeraColctor<B, IK, IV>>(ref colctor);
            reader.AssertMove(cursor);
        }
        reader.ReadArrayEnd();
        if (!colion.FinishCollect(i)) throw new DeserializeException("Failed to collect map");
        return mapper.Map(colctor.builder);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private T CObjectMapStringKey<C, B, M, IK, IV>(C colion, M mapper)
        where C : IMapSeraColion<B, IK, IV> where M : ISeraMapper<B, T>
    {
        var token = reader.CurrentToken;
        if (token.Kind is JsonTokenKind.ArrayStart) return CArrayMap<C, B, M, IK, IV>(colion, mapper);
        reader.ReadObjectStart();
        var colctor = new ObjectMapStringKeySeraColctor<B, IK, IV>(colion.Builder(null), impl);
        var first = true;
        var i = 0;
        for (; reader.Has; i++)
        {
            token = reader.CurrentToken;
            if (token.Kind is JsonTokenKind.ObjectEnd) break;
            if (first) first = false;
            else reader.ReadComma();
            var cursor = reader.Cursor;
            colion.CollectItem<Unit, ObjectMapStringKeySeraColctor<B, IK, IV>>(ref colctor);
            reader.AssertMove(cursor);
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
        for (; reader.Has; i++)
        {
            var token = reader.CurrentToken;
            if (token.Kind is JsonTokenKind.ObjectEnd) break;
            if (first) first = false;
            else reader.ReadComma();
            var cursor = reader.Cursor;
            colion.CollectItem<Unit, ObjectMapSubJsonKeySeraColctor<B, IK, IV>>(ref colctor);
            reader.AssertMove(cursor);
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
            var key_token = impl.reader.ReadStringToken();
            IK vk;
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

    #endregion

    #region Struct

    public T CStruct<C, B, M>(C colion, M mapper, Type<B> b) where C : IStructSeraColion<B> where M : ISeraMapper<B, T>
    {
        reader.ReadObjectStart();
        if (colion.Fields is { } fields)
            return CStaticStruct<C, B, M>(colion, mapper, fields);
        else return CDynamicStruct<C, B, M>(colion, mapper);
    }

    private T CStaticStruct<C, B, M>(C colion, M mapper, SeraFieldInfos fields)
        where C : IStructSeraColion<B> where M : ISeraMapper<B, T>
    {
        var c = new StructSeraColctor<B>(colion.Builder(null), impl);
        var first = true;
        var i = 0;
        for (; reader.Has; reader.MoveNext(), i++)
        {
            var token = reader.CurrentToken;
            if (token.Kind is JsonTokenKind.ObjectEnd) break;
            if (first) first = false;
            else reader.ReadComma();
            if (token.Kind is not JsonTokenKind.String) reader.ThrowExpected(JsonTokenKind.String);
            var field_name = token.AsString();
            reader.ReadColon();
            if (!fields.TryGet(field_name, out var info))
            {
                reader.SkipValue();
                continue;
            }
            var key = long.TryParse(field_name, out var key_) ? (long?)key_ : null;
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
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        reader.ReadObjectEnd();
        if (!colion.FinishCollect(i)) throw new DeserializeException("Failed to collect tuple");
        return mapper.Map(c.builder);
    }

    private T CDynamicStruct<C, B, M>(C colion, M mapper)
        where C : IStructSeraColion<B> where M : ISeraMapper<B, T>
    {
        var c = new StructSeraColctor<B>(colion.Builder(null), impl);
        var first = true;
        var i = 0;
        for (; reader.Has; reader.MoveNext(), i++)
        {
            var token = reader.CurrentToken;
            if (token.Kind is JsonTokenKind.ObjectEnd) break;
            if (first) first = false;
            else reader.ReadComma();
            if (token.Kind is not JsonTokenKind.String) reader.ThrowExpected(JsonTokenKind.String);
            var field_name = token.AsString();
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

        public StructRes CField<C, E, I>(C colion, E effector, Type<I> i)
            where C : ISeraColion<I> where E : ISeraEffector<B, I>
        {
            var c = new JsonDeserializer<I>(impl);
            var r = colion.Collect<I, JsonDeserializer<I>>(ref c);
            effector.Effect(ref builder, r);
            return StructRes.Field;
        }

        public StructRes CSkip() => StructRes.Skip;

        public StructRes CNone() => StructRes.None;
    }

    #endregion

    #region Union

    public T CUnion<C, B, M>(C colion, M mapper, Type<B> b, UnionStyle? union_style = null)
        where C : IUnionSeraColion<B> where M : ISeraMapper<B, T>
    {
        throw new NotImplementedException();
    }

    private struct UnionSeraColctor(JsonDeserializer impl) : IUnionSeraColctor<T, T>
    {
        public T CVariant<N>(N ctor, VariantStyle? variant_style = null) where N : ISeraCtor<T> => ctor.Ctor();

        public T CVariantValue<C, M, I>(C colion, M mapper, Type<I> i, VariantStyle? variant_style = null)
            where C : ISeraColion<I> where M : ISeraMapper<I, T>
        {
            throw new NotImplementedException();
        }

        public T CVariantTuple<C, M, I>(C colion, M mapper, Type<I> i, VariantStyle? variant_style = null)
            where C : ITupleSeraColion<I> where M : ISeraMapper<I, T>
        {
            throw new NotImplementedException();
        }

        public T CVariantStruct<C, M, I>(C colion, M mapper, Type<I> i, VariantStyle? variant_style = null)
            where C : IStructSeraColion<I> where M : ISeraMapper<I, T>
        {
            throw new NotImplementedException();
        }

        public T CNone() => throw new DeserializeException(
            $"Unable to read union {typeof(T)}");
    }

    #endregion
}
