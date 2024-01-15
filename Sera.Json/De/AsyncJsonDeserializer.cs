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
using System.Threading.Tasks;
using BetterCollections;
using BetterCollections.Memories;
using Sera.Core;
using Sera.Core.Abstract;
using Sera.Core.Formats;
using Sera.Core.Impls.De;
using Sera.Json.Utils;
using Sera.Utils;

namespace Sera.Json.De;

public class AsyncJsonDeserializer(SeraJsonOptions options, AAsyncJsonReader reader) : AJsonDeserializer(options)
{
    public AsyncJsonDeserializer(AAsyncJsonReader reader) : this(reader.Options, reader) { }

    internal readonly AAsyncJsonReader reader = reader;

    public AsyncJsonDeserializer<T> MakeColctor<T>() => new(this);
}

public readonly struct AsyncJsonDeserializer<T>(AsyncJsonDeserializer impl) : ISeraColctor<T, ValueTask<T>>
{
    #region Ability

    public string FormatName => impl.FormatName;
    public string FormatMIME => impl.FormatMIME;
    public SeraFormatType FormatType => impl.FormatType;
    public ISeraOptions Options => impl.Options;
    public ISeraColion<object?> RuntimeImpl => impl.RuntimeImpl;
    private AAsyncJsonReader reader => impl.reader;

    #endregion

    #region Select

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public async ValueTask<T> CSelect<C, M, U>(C colion, M mapper, Type<U> u)
        where C : ISelectSeraColion<U> where M : ISeraMapper<U, T>
    {
        var token = reader.CurrentToken;
        Unsafe.SkipInit(out CSelectCtx ctx);
        ctx.mark = SeraKinds.None;
        ctx.ex = null;
        var pos = await reader.Save();
        try
        {
            bool b;
            if (colion.Priorities.HasValue)
            {
                (b, ctx) = await CSelect<C, M, U>(colion, mapper, token, ctx, pos,
                    colion.Priorities.Value);
                if (b) return ctx.r;
            }
            (b, ctx) = await CSelect<C, M, U>(colion, mapper, token, ctx, pos,
                AJsonDeserializer.SelectPriorities);
            if (b) return ctx.r;
            var err_msg = $"Select failed at {token.Pos}";
            if (ctx.ex is null or { Count: 0 }) throw new JsonParseException(err_msg, token.Pos);
            if (ctx.ex is { Count: 1 } and [var ex0]) throw new JsonParseException(err_msg, token.Pos, ex0);
            var inner = new AggregateException(ctx.ex);
            throw new JsonParseException(err_msg, token.Pos, inner);
        }
        finally
        {
            await reader.UnSave(pos);
        }
    }

    private record struct CSelectCtx(SeraKinds mark, T r, List<Exception>? ex)
    {
        public SeraKinds mark = mark;
        public T r = r;
        public List<Exception>? ex = ex;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private async ValueTask<(bool, CSelectCtx)> CSelect<C, M, U>(C colion, M mapper, JsonToken token,
        CSelectCtx ctx, long? pos, ReadOnlyMemory<Any.Kind> kinds)
        where C : ISelectSeraColion<U> where M : ISeraMapper<U, T>
    {
        var c = new AsyncJsonDeserializer<U>.SelectSeraColctor(impl);
        foreach (var anyKind in kinds.Iter())
        {
            var kind = anyKind.ToKinds();
            if (ctx.mark.Has(kind)) continue;
            if (pos.HasValue)
            {
                try
                {
                    if (await CSelect(colion, token, kind, ref c))
                    {
                        ctx.r = mapper.Map(c.Value);
                        return (true, ctx);
                    }
                    else await reader.Load(pos.Value);
                }
                catch (Exception e)
                {
                    ctx.ex ??= new();
                    ctx.ex.Add(e);
                    await reader.Load(pos.Value);
                }
            }
            else
            {
                if (await CSelect(colion, token, kind, ref c))
                {
                    ctx.r = mapper.Map(c.Value);
                    return (true, ctx);
                }
            }
            ctx.mark.Set(kind);
        }
        return (false, ctx);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private ValueTask<bool> CSelect<C, U>(in C colion, in JsonToken token, SeraKinds kind,
        ref AsyncJsonDeserializer<U>.SelectSeraColctor c)
        where C : ISelectSeraColion<U>
    {
        switch (kind)
        {
            case SeraKinds.Primitive:
                return colion.SelectPrimitive<ValueTask<bool>, AsyncJsonDeserializer<U>.SelectSeraColctor>(ref c);
            case SeraKinds.String:
                if (token.Kind is not JsonTokenKind.String) return ValueTask.FromResult(false);
                return colion.SelectString<ValueTask<bool>, AsyncJsonDeserializer<U>.SelectSeraColctor>(ref c);
            case SeraKinds.Bytes:
                if (token.Kind is not (JsonTokenKind.String or JsonTokenKind.ArrayStart))
                    return ValueTask.FromResult(false);
                return colion.SelectBytes<ValueTask<bool>, AsyncJsonDeserializer<U>.SelectSeraColctor>(ref c);
            case SeraKinds.Array:
                if (token.Kind is not JsonTokenKind.ArrayStart) return ValueTask.FromResult(false);
                return colion.SelectArray<ValueTask<bool>, AsyncJsonDeserializer<U>.SelectSeraColctor>(ref c);
            case SeraKinds.Unit:
                if (token.Kind is not JsonTokenKind.Null) return ValueTask.FromResult(false);
                return colion.SelectUnit<ValueTask<bool>, AsyncJsonDeserializer<U>.SelectSeraColctor>(ref c);
            case SeraKinds.Option:
                return colion.SelectOption<ValueTask<bool>, AsyncJsonDeserializer<U>.SelectSeraColctor>(ref c);
            case SeraKinds.Entry:
                if (token.Kind is not JsonTokenKind.ArrayStart) return ValueTask.FromResult(false);
                return colion.SelectEntry<ValueTask<bool>, AsyncJsonDeserializer<U>.SelectSeraColctor>(ref c);
            case SeraKinds.Tuple:
                if (token.Kind is not JsonTokenKind.ArrayStart) return ValueTask.FromResult(false);
                return colion.SelectTuple<ValueTask<bool>, AsyncJsonDeserializer<U>.SelectSeraColctor>(ref c, null);
            case SeraKinds.Seq:
                if (token.Kind is not JsonTokenKind.ArrayStart) return ValueTask.FromResult(false);
                return colion.SelectSeq<ValueTask<bool>, AsyncJsonDeserializer<U>.SelectSeraColctor>(ref c, null);
            case SeraKinds.Map:
                if (token.Kind is not JsonTokenKind.ObjectStart) return ValueTask.FromResult(false);
                return colion.SelectMap<ValueTask<bool>, AsyncJsonDeserializer<U>.SelectSeraColctor>(ref c, null);
            case SeraKinds.Struct:
                if (token.Kind is not JsonTokenKind.ObjectStart) return ValueTask.FromResult(false);
                return colion.SelectStruct<ValueTask<bool>, AsyncJsonDeserializer<U>.SelectSeraColctor>(ref c, null,
                    null);
            case SeraKinds.Union:
                return colion.SelectUnion<ValueTask<bool>, AsyncJsonDeserializer<U>.SelectSeraColctor>(ref c, null);
            default:
                throw new ArgumentOutOfRangeException(nameof(kind), kind, null);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public async ValueTask<T> CSelectPrimitive<C, M, U>(C colion, M mapper, Type<U> u)
        where C : ISelectPrimitiveSeraColion<U>
        where M : ISeraMapper<U, T>
    {
        var token = reader.CurrentToken;
        Unsafe.SkipInit(out CSelectPrimitiveCtx ctx);
        ctx.mark = SeraPrimitiveKinds.None;
        ctx.ex = null;
        var pos = await reader.Save();
        try
        {
            bool b;
            if (colion.Priorities.HasValue)
            {
                (b, ctx) = await CSelectPrimitive<C, M, U>(colion, mapper, token, ctx, pos,
                    colion.Priorities.Value);
                if (b)
                    return ctx.r;
            }
            (b, ctx) = await CSelectPrimitive<C, M, U>(colion, mapper, token, ctx, pos,
                AJsonDeserializer.SelectPrimitivePriorities);
            if (b)
                return ctx.r;
            var err_msg = $"Select Primitive failed at {token.Pos}";
            if (ctx.ex is null or { Count: 0 }) throw new JsonParseException(err_msg, token.Pos);
            if (ctx.ex is { Count: 1 } and [var ex0]) throw new JsonParseException(err_msg, token.Pos, ex0);
            var inner = new AggregateException(ctx.ex);
            throw new JsonParseException(err_msg, token.Pos, inner);
        }
        finally
        {
            await reader.UnSave(pos);
        }
    }

    private record struct CSelectPrimitiveCtx(SeraPrimitiveKinds mark, T r, List<Exception>? ex)
    {
        public SeraPrimitiveKinds mark = mark;
        public T r = r;
        public List<Exception>? ex = ex;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private async ValueTask<(bool, CSelectPrimitiveCtx)> CSelectPrimitive<C, M, U>(C colion, M mapper, JsonToken token,
        CSelectPrimitiveCtx ctx, long? pos,
        ReadOnlyMemory<SeraPrimitiveTypes> types)
        where C : ISelectPrimitiveSeraColion<U> where M : ISeraMapper<U, T>
    {
        var c = new AsyncJsonDeserializer<U>.SelectSeraColctor(impl);
        foreach (var type in types.Iter())
        {
            var kind = type.ToKinds();
            if (ctx.mark.Has(kind)) continue;
            if (pos.HasValue)
            {
                try
                {
                    if (await CSelectPrimitive(in colion, in token, type, ref c))
                    {
                        ctx.r = mapper.Map(c.Value);
                        return (true, ctx);
                    }
                    else await reader.Load(pos.Value);
                }
                catch (Exception e)
                {
                    ctx.ex ??= new();
                    ctx.ex.Add(e);
                    await reader.Load(pos.Value);
                }
            }
            else
            {
                if (await CSelectPrimitive(in colion, in token, type, ref c))
                {
                    ctx.r = mapper.Map(c.Value);
                    return (true, ctx);
                }
            }
            ctx.mark.Set(kind);
        }
        return (false, ctx);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private ValueTask<bool> CSelectPrimitive<C, U>(in C colion, in JsonToken token, SeraPrimitiveTypes type,
        ref AsyncJsonDeserializer<U>.SelectSeraColctor c)
        where C : ISelectPrimitiveSeraColion<U>
    {
        switch (type)
        {
            case SeraPrimitiveTypes.Boolean:
                if (token.Kind is not (JsonTokenKind.True or JsonTokenKind.False)) return ValueTask.FromResult(false);
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
                if (token.Kind is not (JsonTokenKind.Number or JsonTokenKind.String))
                    return ValueTask.FromResult(false);
                break;
            case SeraPrimitiveTypes.Complex:
                if (token.Kind is not (JsonTokenKind.String or JsonTokenKind.ArrayStart))
                    return ValueTask.FromResult(false);
                break;
            case SeraPrimitiveTypes.Guid:
            case SeraPrimitiveTypes.Range:
            case SeraPrimitiveTypes.Char:
            case SeraPrimitiveTypes.Rune:
            case SeraPrimitiveTypes.Uri:
            case SeraPrimitiveTypes.Version:
                if (token.Kind is not JsonTokenKind.String) return ValueTask.FromResult(false);
                break;
        }
        return colion.SelectPrimitiveDetail<ValueTask<bool>, AsyncJsonDeserializer<U>.SelectSeraColctor>(ref c, type);
    }

    private struct SelectSeraColctor(AsyncJsonDeserializer impl) : ISelectSeraColctor<T, ValueTask<bool>>
    {
        public T Value = default!;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public async ValueTask<bool> CSome<C, M, U>(C colion, M mapper, Type<U> u)
            where C : ISeraColion<U> where M : ISeraMapper<U, T>
        {
            var c = new AsyncJsonDeserializer<U>(impl);
            var r = await colion.Collect<ValueTask<U>, AsyncJsonDeserializer<U>>(ref c);
            Value = mapper.Map(r);
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ValueTask<bool> CNone() => ValueTask.FromResult(false);
    }

    #endregion

    #region Primitive

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public async ValueTask<T> CPrimitive<M>(M mapper, Type<bool> t, SeraFormats? formats = null)
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
        await reader.MoveNext();
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
    public ValueTask<T> CPrimitive<M>(M mapper, Type<sbyte> t, SeraFormats? formats = null)
        where M : ISeraMapper<sbyte, T>
        => CPrimitiveNumber<sbyte, M>(mapper, formats);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ValueTask<T> CPrimitive<M>(M mapper, Type<byte> t, SeraFormats? formats = null)
        where M : ISeraMapper<byte, T>
        => CPrimitiveNumber<byte, M>(mapper, formats);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ValueTask<T> CPrimitive<M>(M mapper, Type<short> t, SeraFormats? formats = null)
        where M : ISeraMapper<short, T>
        => CPrimitiveNumber<short, M>(mapper, formats);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ValueTask<T> CPrimitive<M>(M mapper, Type<ushort> t, SeraFormats? formats = null)
        where M : ISeraMapper<ushort, T>
        => CPrimitiveNumber<ushort, M>(mapper, formats);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ValueTask<T> CPrimitive<M>(M mapper, Type<int> t, SeraFormats? formats = null)
        where M : ISeraMapper<int, T>
        => CPrimitiveNumber<int, M>(mapper, formats);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ValueTask<T> CPrimitive<M>(M mapper, Type<uint> t, SeraFormats? formats = null)
        where M : ISeraMapper<uint, T>
        => CPrimitiveNumber<uint, M>(mapper, formats);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ValueTask<T> CPrimitive<M>(M mapper, Type<long> t, SeraFormats? formats = null)
        where M : ISeraMapper<long, T>
        => CPrimitiveNumber<long, M>(mapper, formats);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ValueTask<T> CPrimitive<M>(M mapper, Type<ulong> t, SeraFormats? formats = null)
        where M : ISeraMapper<ulong, T>
        => CPrimitiveNumber<ulong, M>(mapper, formats);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ValueTask<T> CPrimitive<M>(M mapper, Type<Int128> t, SeraFormats? formats = null)
        where M : ISeraMapper<Int128, T>
        => CPrimitiveNumber<Int128, M>(mapper, formats);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ValueTask<T> CPrimitive<M>(M mapper, Type<UInt128> t, SeraFormats? formats = null)
        where M : ISeraMapper<UInt128, T>
        => CPrimitiveNumber<UInt128, M>(mapper, formats);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ValueTask<T> CPrimitive<M>(M mapper, Type<IntPtr> t, SeraFormats? formats = null)
        where M : ISeraMapper<IntPtr, T>
        => CPrimitiveNumber<IntPtr, M>(mapper, formats);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ValueTask<T> CPrimitive<M>(M mapper, Type<UIntPtr> t, SeraFormats? formats = null)
        where M : ISeraMapper<UIntPtr, T>
        => CPrimitiveNumber<UIntPtr, M>(mapper, formats);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ValueTask<T> CPrimitive<M>(M mapper, Type<Half> t, SeraFormats? formats = null)
        where M : ISeraMapper<Half, T>
        => CPrimitiveNumber<Half, M>(mapper, formats);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ValueTask<T> CPrimitive<M>(M mapper, Type<float> t, SeraFormats? formats = null)
        where M : ISeraMapper<float, T>
        => CPrimitiveNumber<float, M>(mapper, formats);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ValueTask<T> CPrimitive<M>(M mapper, Type<double> t, SeraFormats? formats = null)
        where M : ISeraMapper<double, T>
        => CPrimitiveNumber<double, M>(mapper, formats);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ValueTask<T> CPrimitive<M>(M mapper, Type<decimal> t, SeraFormats? formats = null)
        where M : ISeraMapper<decimal, T>
        => CPrimitiveNumber<decimal, M>(mapper, formats);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ValueTask<T> CPrimitive<M>(M mapper, Type<NFloat> t, SeraFormats? formats = null)
        where M : ISeraMapper<NFloat, T>
        => CPrimitiveNumber<NFloat, M>(mapper, formats);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ValueTask<T> CPrimitive<M>(M mapper, Type<BigInteger> t, SeraFormats? formats = null)
        where M : ISeraMapper<BigInteger, T>
        => CPrimitiveNumber<BigInteger, M>(mapper, formats);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public async ValueTask<T> CPrimitive<M>(M mapper, Type<Complex> t, SeraFormats? formats = null)
        where M : ISeraMapper<Complex, T>
    {
        var token = reader.CurrentToken;
        Unsafe.SkipInit(out Complex r);
        if (token.Kind is JsonTokenKind.String)
        {
            if (token.AsSpan().TryParseComplex(formats.GetNumberStyles(), out r))
            {
                await reader.MoveNext();
                goto ok;
            }
            throw new JsonParseException($"Illegal Complex format at {token.Pos}", token.Pos);
        }
        await reader.ReadArrayStart();
        var c = new AsyncJsonDeserializer<double>(impl);
        var real = await c.CPrimitiveNumber<double, IdentityMapper<double>>(new(), formats);
        await reader.ReadComma();
        var imaginary = await c.CPrimitiveNumber<double, IdentityMapper<double>>(new(), formats);
        await reader.ReadArrayEnd();
        r = new Complex(real, imaginary);
        ok:
        return mapper.Map(r);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ValueTask<T> CPrimitive<M>(M mapper, Type<TimeSpan> t, SeraFormats? formats = null)
        where M : ISeraMapper<TimeSpan, T>
        => CPrimitiveDateTime<TimeSpan, M, DateOrTimeFromNumberMapper>(mapper, nameof(TimeSpan), new());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ValueTask<T> CPrimitive<M>(M mapper, Type<DateOnly> t, SeraFormats? formats = null)
        where M : ISeraMapper<DateOnly, T>
        => CPrimitiveDateTime<DateOnly, M, DateOrTimeFromNumberMapper>(mapper, nameof(DateOnly), new());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ValueTask<T> CPrimitive<M>(M mapper, Type<TimeOnly> t, SeraFormats? formats = null)
        where M : ISeraMapper<TimeOnly, T>
        => CPrimitiveDateTime<TimeOnly, M, DateOrTimeFromNumberMapper>(mapper, nameof(TimeOnly), new());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public async ValueTask<T> CPrimitive<M>(M mapper, Type<DateTime> t, SeraFormats? formats = null)
        where M : ISeraMapper<DateTime, T>
    {
        var token = reader.CurrentToken;
        var r = token.ParseDateOrTime<DateTime, DateOrTimeFromNumberWithOffsetMapper>(nameof(DateTime),
            new(Options.TimeZone.BaseUtcOffset));
        r = TimeZoneInfo.ConvertTime(r, Options.TimeZone);
        await reader.MoveNext();
        return mapper.Map(r);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public async ValueTask<T> CPrimitive<M>(M mapper, Type<DateTimeOffset> t, SeraFormats? formats = null)
        where M : ISeraMapper<DateTimeOffset, T>
    {
        var token = reader.CurrentToken;
        var r = token.ParseDateOrTime<DateTimeOffset, DateOrTimeFromNumberWithOffsetMapper>(nameof(DateTimeOffset),
            new(Options.TimeZone.BaseUtcOffset));
        if (formats?.DateTimeFormat.HasFlag(DateTimeFormatFlags.DateTimeOffsetUseTimeZone) ?? false)
        {
            r = TimeZoneInfo.ConvertTime(r, Options.TimeZone);
        }
        await reader.MoveNext();
        return mapper.Map(r);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public async ValueTask<T> CPrimitive<M>(M mapper, Type<Guid> t, SeraFormats? formats = null)
        where M : ISeraMapper<Guid, T>
    {
        var token = await reader.ReadStringToken();
        Guid v;
        try
        {
            var span = token.AsMemory();
            v = Guid.Parse(span.Span);
        }
        catch (Exception e)
        {
            throw new JsonParseException($"Illegal Guid format at {token.Pos}", token.Pos, e);
        }
        return mapper.Map(v);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public async ValueTask<T> CPrimitive<M>(M mapper, Type<Range> t, SeraFormats? formats = null)
        where M : ISeraMapper<Range, T>
    {
        var token = await reader.ReadStringToken();
        var span = token.AsMemory();
        if (!span.Span.TryParseRange(out var range))
            throw new JsonParseException($"Illegal Range format at {token.Pos}", token.Pos);
        return mapper.Map(range);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public async ValueTask<T> CPrimitive<M>(M mapper, Type<Index> t, SeraFormats? formats = null)
        where M : ISeraMapper<Index, T>
    {
        var token = reader.CurrentToken;
        if (token.Kind is not (JsonTokenKind.Number or JsonTokenKind.String))
        {
            reader.ThrowExpected(JsonTokenKind.Number, JsonTokenKind.String);
        }
        var span = token.AsMemory();
        if (!span.Span.TryParseIndex(out var index))
            throw new JsonParseException($"Illegal Index format at {token.Pos}", token.Pos);
        await reader.MoveNext();
        return mapper.Map(index);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public async ValueTask<T> CPrimitive<M>(M mapper, Type<char> t, SeraFormats? formats = null)
        where M : ISeraMapper<char, T>
    {
        var token = await reader.ReadStringToken();
        var span = token.AsMemory();
        if (span.Length != 1)
            throw new JsonParseException($"char must be a string of only one character at {token.Pos}", token.Pos);
        return mapper.Map(span.Span[0]);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public async ValueTask<T> CPrimitive<M>(M mapper, Type<Rune> t, SeraFormats? formats = null)
        where M : ISeraMapper<Rune, T>
    {
        var token = await reader.ReadStringToken();
        var span = token.AsMemory();
        var r = Rune.DecodeFromUtf16(span.Span, out var rune, out var len);
        if (r != OperationStatus.Done)
            throw new JsonParseException($"Decode Rune failed ({r}) at {token.Pos}", token.Pos);
        if (span.Length != len)
            throw new JsonParseException($"String too long for Rune at {token.Pos}", token.Pos);
        return mapper.Map(rune);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public async ValueTask<T> CPrimitive<M>(M mapper, Type<Uri> t, SeraFormats? formats = null)
        where M : ISeraMapper<Uri, T>
    {
        var token = await reader.ReadStringToken();
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
    public async ValueTask<T> CPrimitive<M>(M mapper, Type<Version> t, SeraFormats? formats = null)
        where M : ISeraMapper<Version, T>
    {
        var token = await reader.ReadStringToken();
        var span = token.AsMemory();
        Version v;
        try
        {
            v = Version.Parse(span.Span);
        }
        catch (Exception e)
        {
            throw new JsonParseException($"Illegal Version format at {token.Pos}", token.Pos, e);
        }
        return mapper.Map(v);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private async ValueTask<T> CPrimitiveNumber<N, M>(M mapper, SeraFormats? formats = null)
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
        await reader.MoveNext();
        return mapper.Map(r);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private async ValueTask<T> CPrimitiveDateTime<N, M, C>(M mapper, string TypeName, C FromTicks)
        where M : ISeraMapper<N, T> where N : ISpanParsable<N> where C : ISeraMapper<long, N>
    {
        var token = reader.CurrentToken;
        var r = token.ParseDateOrTime<N, C>(TypeName, FromTicks);
        await reader.MoveNext();
        return mapper.Map(r);
    }

    #endregion

    #region String

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public async ValueTask<T> CString<M>(M mapper, Type<string> t) where M : ISeraMapper<string, T>
    {
        var token = await reader.ReadStringToken();
        return mapper.Map(token.AsString());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public async ValueTask<T> CString<M>(M mapper, Type<char[]> t) where M : ISeraMapper<char[], T>
        => mapper.Map(await CStringArray());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public async ValueTask<T> CString<M>(M mapper, Type<Memory<char>> t) where M : ISeraMapper<Memory<char>, T>
        => mapper.Map(await CStringArray());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private async ValueTask<char[]> CStringArray()
    {
        var token = await reader.ReadStringToken();
        return token.AsSpan().ToArray();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public async ValueTask<T> CString<M>(M mapper, Type<ReadOnlyMemory<char>> t)
        where M : ISeraMapper<ReadOnlyMemory<char>, T>
    {
        var token = await reader.ReadStringToken();
        return mapper.Map(token.AsMemory());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public async ValueTask<T> CStringSpan<M>(M mapper) where M : ISeraSpanMapper<char, T>
    {
        var token = await reader.ReadStringToken();
        return mapper.SpanMap(token.AsSpan());
    }

    #endregion

    #region Bytes

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public async ValueTask<T> CBytes<M>(M mapper, Type<byte[]> t) where M : ISeraMapper<byte[], T>
        => mapper.Map((await CBytes()).ToArray());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public async ValueTask<T> CBytes<M>(M mapper, Type<Memory<byte>> t) where M : ISeraMapper<Memory<byte>, T>
        => mapper.Map(await CBytes());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public async ValueTask<T> CBytes<M>(M mapper, Type<ReadOnlyMemory<byte>> t)
        where M : ISeraMapper<ReadOnlyMemory<byte>, T>
        => mapper.Map(await CBytes());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public async ValueTask<T> CBytesSpan<M>(M mapper) where M : ISeraSpanMapper<byte, T>
        => mapper.SpanMap((await CBytes()).Span);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private async ValueTask<Memory<byte>> CBytes()
    {
        var token = reader.CurrentToken;
        if (token.Kind is JsonTokenKind.String)
        {
            var span = token.AsMemory();
            var size = Base64.GetMaxDecodedFromUtf8Length(span.Length);
            var arr = new byte[size];
            if (!Convert.TryFromBase64Chars(span.Span, arr, out var len))
                throw new JsonParseException($"Decoding base64 failed at {token.Pos}", token.Pos);
            await reader.MoveNext();
            return arr.AsMemory(0, len);
        }
        var vec = new Vec<byte>();
        await reader.ReadArrayStart();
        var first = true;
        var c = new AsyncJsonDeserializer<short>(impl);
        for (; reader.CurrentHas;)
        {
            token = reader.CurrentToken;
            if (token.Kind is JsonTokenKind.ArrayEnd) break;
            if (first) first = false;
            else await reader.ReadComma();
            var v = (byte)await c.CPrimitiveNumber<short, IdentityMapper<short>>(new());
            vec.Add(v);
        }
        await reader.ReadArrayEnd();
        return vec.AsMemory;
    }

    #endregion

    #region Array

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public async ValueTask<T> CArray<C, M, I>(C colion, M mapper, Type<I[]> t, Type<I> i)
        where C : ISeraColion<I> where M : ISeraMapper<I[], T>
        => mapper.Map((await CArray<C, I>(colion)).ToArray());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public async ValueTask<T> CArray<C, M, I>(C colion, M mapper, Type<Memory<I>> t, Type<I> i)
        where C : ISeraColion<I> where M : ISeraMapper<Memory<I>, T>
        => mapper.Map((await CArray<C, I>(colion)).AsMemory);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public async ValueTask<T> CArray<C, M, I>(C colion, M mapper, Type<ReadOnlyMemory<I>> t, Type<I> i)
        where C : ISeraColion<I> where M : ISeraMapper<ReadOnlyMemory<I>, T>
        => mapper.Map((await CArray<C, I>(colion)).AsMemory);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public async ValueTask<T> CArraySpan<C, M, I>(C colion, M mapper, Type<I> i)
        where C : ISeraColion<I> where M : ISeraSpanMapper<I, T>
    {
        var arr = await CArray<C, I>(colion);
        return mapper.SpanMap(arr.AsSpan);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private async ValueTask<Vec<I>> CArray<C, I>(C colion) where C : ISeraColion<I>
    {
        await reader.ReadArrayStart();
        var vec = new Vec<I>();
        var first = true;
        for (; reader.CurrentHas;)
        {
            var token = reader.CurrentToken;
            if (token.Kind is JsonTokenKind.ArrayEnd) break;
            if (first) first = false;
            else await reader.ReadComma();
            var c = new AsyncJsonDeserializer<I>(impl);
            var version = reader.Version;
            var i = await colion.Collect<ValueTask<I>, AsyncJsonDeserializer<I>>(ref c);
            reader.AssertMove(version);
            vec.Add(i);
        }
        await reader.ReadArrayEnd();
        return vec;
    }

    #endregion

    #region Unit

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public async ValueTask<T> CUnit<N>(N ctor) where N : ISeraCtor<T>
    {
        await reader.ReadNull();
        return ctor.Ctor();
    }

    #endregion

    #region Option

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public async ValueTask<T> COption<C>(C colion) where C : IOptionSeraColion<T>
    {
        var token = reader.CurrentToken;
        if (token.Kind is JsonTokenKind.Null)
        {
            await reader.MoveNext();
            return colion.CtorNone();
        }
        var colctor = new SomeSeraColctor(impl);
        return await colion.CollectSome<ValueTask<T>, SomeSeraColctor>(ref colctor);
    }

    private readonly struct SomeSeraColctor(AsyncJsonDeserializer impl) : ISomeSeraColctor<T, ValueTask<T>>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public async ValueTask<T> CSome<C, M, U>(C colion, M mapper, Type<U> u)
            where C : ISeraColion<U> where M : ISeraMapper<U, T>
        {
            var colctor = new AsyncJsonDeserializer<U>(impl);
            var r = await colion.Collect<ValueTask<U>, AsyncJsonDeserializer<U>>(ref colctor);
            return mapper.Map(r);
        }
    }

    #endregion

    #region Entry

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public async ValueTask<T> CEntry<C, B, M>(C colion, M mapper, Type<B> b)
        where C : IEntrySeraColion<B> where M : ISeraMapper<B, T>
    {
        await reader.ReadArrayStart();
        var colctor = new EntrySeraColctor<B>(colion.Builder(), impl);
        await colion.CollectKey<ValueTask, EntrySeraColctor<B>>(ref colctor);
        await reader.ReadComma();
        await colion.CollectValue<ValueTask, EntrySeraColctor<B>>(ref colctor);
        await reader.ReadArrayEnd();
        return mapper.Map(colctor.builder);
    }

    private struct EntrySeraColctor<B>(B builder, AsyncJsonDeserializer impl) : IEntrySeraColctor<B, ValueTask>
    {
        public B builder = builder;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public async ValueTask CItem<C, E, I>(C colion, E effector, Type<I> i)
            where C : ISeraColion<I> where E : ISeraEffector<B, I>
        {
            var c = new AsyncJsonDeserializer<I>(impl);
            var r = await colion.Collect<ValueTask<I>, AsyncJsonDeserializer<I>>(ref c);
            effector.Effect(ref builder, r);
        }
    }

    #endregion

    #region Tuple

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public async ValueTask<T> CTuple<C, B, M>(C colion, M mapper, Type<B> b)
        where C : ITupleSeraColion<B> where M : ISeraMapper<B, T>
    {
        await reader.ReadArrayStart();
        var size = colion.Size;
        var colctor = new TupleSeraColctor<B>(colion.Builder(), impl);
        var first = true;
        var i = 0;
        if (size.HasValue)
        {
            for (; i < size; i++)
            {
                if (first) first = false;
                else await reader.ReadComma();
                var version = reader.Version;
                var err = await colion.CollectItem<ValueTask<bool>, TupleSeraColctor<B>>(ref colctor, i);
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
                else await reader.ReadComma();
                var version = reader.Version;
                var err = await colion.CollectItem<ValueTask<bool>, TupleSeraColctor<B>>(ref colctor, i);
                if (err) throw new DeserializeException($"Unable to read item {i} of tuple {typeof(T)}");
                reader.AssertMove(version);
            }
        }
        await reader.ReadArrayEnd();
        if (!colion.FinishCollect(i)) throw new DeserializeException("Failed to collect tuple");
        return mapper.Map(colctor.builder);
    }

    private struct TupleSeraColctor<B>(B builder, AsyncJsonDeserializer impl) : ITupleSeraColctor<B, ValueTask<bool>>
    {
        public B builder = builder;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public async ValueTask<bool> CItem<C, E, I>(C colion, E effector, Type<I> i)
            where C : ISeraColion<I> where E : ISeraEffector<B, I>
        {
            var c = new AsyncJsonDeserializer<I>(impl);
            var r = await colion.Collect<ValueTask<I>, AsyncJsonDeserializer<I>>(ref c);
            effector.Effect(ref builder, r);
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ValueTask<bool> CNone() => ValueTask.FromResult(true);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public async ValueTask<bool> CRest<C, E, I>(C colion, E effector, Type<I> i) where C : ITupleRestSeraColion<I>
            where E : ISeraEffector<B, I>
        {
            var c = new AsyncJsonDeserializer<I>.TupleRestSeraColctor(impl);
            var r = await colion.CollectRest<ValueTask<I>, AsyncJsonDeserializer<I>.TupleRestSeraColctor>(ref c);
            effector.Effect(ref builder, r);
            return false;
        }
    }

    private readonly struct TupleRestSeraColctor(AsyncJsonDeserializer impl) : ITupleRestSeraColctor<T, ValueTask<T>>
    {
        private AAsyncJsonReader reader => impl.reader;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public async ValueTask<T> CTupleRest<C, B, M>(C colion, M mapper, Type<B> b)
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
                    else await reader.ReadComma();
                    var version = reader.Version;
                    var err = await colion.CollectItem<ValueTask<bool>, TupleSeraColctor<B>>(ref colctor, i);
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
                    else await reader.ReadComma();
                    var version = reader.Version;
                    var err = await colion.CollectItem<ValueTask<bool>, TupleSeraColctor<B>>(ref colctor, i);
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
    public async ValueTask<T> CSeq<C, B, M, I>(C colion, M mapper, Type<B> b, Type<I> _i)
        where C : ISeqSeraColion<B, I> where M : ISeraMapper<B, T>
    {
        await reader.ReadArrayStart();
        var colctor = new SeqSeraColctor<B, I>(colion.Builder(null), impl);
        var first = true;
        var i = 0;
        for (; reader.CurrentHas; i++)
        {
            var token = reader.CurrentToken;
            if (token.Kind is JsonTokenKind.ArrayEnd) break;
            if (first) first = false;
            else await reader.ReadComma();
            var version = reader.Version;
            await colion.CollectItem<ValueTask, SeqSeraColctor<B, I>>(ref colctor);
            reader.AssertMove(version);
        }
        await reader.ReadArrayEnd();
        if (!colion.FinishCollect(i)) throw new DeserializeException("Failed to collect seq");
        return mapper.Map(colctor.builder);
    }

    private struct SeqSeraColctor<B, I>(B builder, AsyncJsonDeserializer impl) : ISeqSeraColctor<B, I, ValueTask>
    {
        public B builder = builder;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public async ValueTask CItem<C, E>(C colion, E effector)
            where C : ISeraColion<I> where E : ISeraEffector<B, I>
        {
            var c = new AsyncJsonDeserializer<I>(impl);
            var r = await colion.Collect<ValueTask<I>, AsyncJsonDeserializer<I>>(ref c);
            effector.Effect(ref builder, r);
        }
    }

    #endregion

    #region Map

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public async ValueTask<T> CMap<C, B, M, IK, IV>(C colion, M mapper, Type<B> b, Type<IK> k, Type<IV> v,
        ASeraTypeAbility? keyAbility = null)
        where C : IMapSeraColion<B, IK, IV> where M : ISeraMapper<B, T>
        => typeof(IK) == typeof(string)
            ? await CObjectMapStringKey<C, B, M, IK, IV>(colion, mapper, keyAbility)
            : await CArrayMap<C, B, M, IK, IV>(colion, mapper, keyAbility);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private async ValueTask<T> CArrayMap<C, B, M, IK, IV>(C colion, M mapper, ASeraTypeAbility? keyAbility)
        where C : IMapSeraColion<B, IK, IV> where M : ISeraMapper<B, T>
    {
        var token = reader.CurrentToken;
        if (token.Kind is JsonTokenKind.ObjectStart)
        {
            if (keyAbility?.IsAccept(SeraKinds.String) ?? false)
                return await CObjectMapAsStringKey<C, B, M, IK, IV>(colion, mapper);
            return await CObjectMapSubJsonKey<C, B, M, IK, IV>(colion, mapper);
        }
        await reader.ReadArrayStart();
        var colctor = new ArrayMapSeraColctor<B, IK, IV>(colion.Builder(null), impl);
        var first = true;
        var i = 0;
        for (; reader.CurrentHas; i++)
        {
            token = reader.CurrentToken;
            if (token.Kind is JsonTokenKind.ArrayEnd) break;
            if (first) first = false;
            else await reader.ReadComma();
            var version = reader.Version;
            await colion.CollectItem<ValueTask, ArrayMapSeraColctor<B, IK, IV>>(ref colctor);
            reader.AssertMove(version);
        }
        await reader.ReadArrayEnd();
        if (!colion.FinishCollect(i)) throw new DeserializeException("Failed to collect map");
        return mapper.Map(colctor.builder);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private async ValueTask<T> CObjectMapStringKey<C, B, M, IK, IV>(C colion, M mapper, ASeraTypeAbility? keyAbility)
        where C : IMapSeraColion<B, IK, IV> where M : ISeraMapper<B, T>
    {
        var token = reader.CurrentToken;
        if (token.Kind is JsonTokenKind.ArrayStart) return await CArrayMap<C, B, M, IK, IV>(colion, mapper, keyAbility);
        if (keyAbility?.IsAccept(SeraKinds.String) ?? false)
            return await CObjectMapAsStringKey<C, B, M, IK, IV>(colion, mapper);
        await reader.ReadObjectStart();
        var colctor = new ObjectMapStringKeySeraColctor<B, IK, IV>(colion.Builder(null), impl);
        var first = true;
        var i = 0;
        for (; reader.CurrentHas; i++)
        {
            token = reader.CurrentToken;
            if (token.Kind is JsonTokenKind.ObjectEnd) break;
            if (first) first = false;
            else await reader.ReadComma();
            var version = reader.Version;
            await colion.CollectItem<ValueTask, ObjectMapStringKeySeraColctor<B, IK, IV>>(ref colctor);
            reader.AssertMove(version);
        }
        await reader.ReadObjectEnd();
        if (!colion.FinishCollect(i)) throw new DeserializeException("Failed to collect map");
        return mapper.Map(colctor.builder);
    }

    private async ValueTask<T> CObjectMapSubJsonKey<C, B, M, IK, IV>(C colion, M mapper)
        where C : IMapSeraColion<B, IK, IV> where M : ISeraMapper<B, T>
    {
        await reader.ReadObjectStart();
        var colctor = new ObjectMapSubJsonKeySeraColctor<B, IK, IV>(colion.Builder(null), impl);
        var first = true;
        var i = 0;
        for (; reader.CurrentHas; i++)
        {
            var token = reader.CurrentToken;
            if (token.Kind is JsonTokenKind.ObjectEnd) break;
            if (first) first = false;
            else await reader.ReadComma();
            var version = reader.Version;
            await colion.CollectItem<ValueTask, ObjectMapSubJsonKeySeraColctor<B, IK, IV>>(ref colctor);
            reader.AssertMove(version);
        }
        await reader.ReadObjectEnd();
        if (!colion.FinishCollect(i)) throw new DeserializeException("Failed to collect map");
        return mapper.Map(colctor.builder);
    }

    private async ValueTask<T> CObjectMapAsStringKey<C, B, M, IK, IV>(C colion, M mapper)
        where C : IMapSeraColion<B, IK, IV> where M : ISeraMapper<B, T>
    {
        await reader.ReadObjectStart();
        var colctor = new ObjectMapAsStringKeySeraColctor<B, IK, IV>(colion.Builder(null), impl);
        var first = true;
        var i = 0;
        for (; reader.CurrentHas; i++)
        {
            var token = reader.CurrentToken;
            if (token.Kind is JsonTokenKind.ObjectEnd) break;
            if (first) first = false;
            else await reader.ReadComma();
            var version = reader.Version;
            await colion.CollectItem<ValueTask, ObjectMapAsStringKeySeraColctor<B, IK, IV>>(ref colctor);
            reader.AssertMove(version);
        }
        await reader.ReadObjectEnd();
        if (!colion.FinishCollect(i)) throw new DeserializeException("Failed to collect map");
        return mapper.Map(colctor.builder);
    }

    private struct ArrayMapSeraColctor<B, IK, IV>(B builder, AsyncJsonDeserializer impl)
        : IMapSeraColctor<B, IK, IV, ValueTask>
    {
        public B builder = builder;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public async ValueTask CItem<CK, CV, E>(CK keyColion, CV valueColion, E effector)
            where CK : ISeraColion<IK> where CV : ISeraColion<IV> where E : ISeraEffector<B, KeyValuePair<IK, IV>>
        {
            await impl.reader.ReadArrayStart();
            var ck = new AsyncJsonDeserializer<IK>(impl);
            var vk = await keyColion.Collect<ValueTask<IK>, AsyncJsonDeserializer<IK>>(ref ck);
            await impl.reader.ReadComma();
            var cv = new AsyncJsonDeserializer<IV>(impl);
            var vv = await valueColion.Collect<ValueTask<IV>, AsyncJsonDeserializer<IV>>(ref cv);
            await impl.reader.ReadArrayEnd();
            effector.Effect(ref builder, new(vk, vv));
        }
    }

    private struct ObjectMapStringKeySeraColctor<B, IK, IV>(B builder, AsyncJsonDeserializer impl)
        : IMapSeraColctor<B, IK, IV, ValueTask>
    {
        public B builder = builder;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public async ValueTask CItem<CK, CV, E>(CK keyColion, CV valueColion, E effector)
            where CK : ISeraColion<IK> where CV : ISeraColion<IV> where E : ISeraEffector<B, KeyValuePair<IK, IV>>
        {
            var vk = impl.reader.ReadString();
            await impl.reader.ReadColon();
            var cv = new AsyncJsonDeserializer<IV>(impl);
            var vv = await valueColion.Collect<ValueTask<IV>, AsyncJsonDeserializer<IV>>(ref cv);
            effector.Effect(ref builder, new((IK)(object)vk, vv));
        }
    }

    private struct ObjectMapSubJsonKeySeraColctor<B, IK, IV>(B builder, AsyncJsonDeserializer impl)
        : IMapSeraColctor<B, IK, IV, ValueTask>
    {
        public B builder = builder;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public async ValueTask CItem<CK, CV, E>(CK keyColion, CV valueColion, E effector)
            where CK : ISeraColion<IK> where CV : ISeraColion<IV> where E : ISeraEffector<B, KeyValuePair<IK, IV>>
        {
            IK vk;
            var key_token = await impl.reader.ReadStringToken();
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
            await impl.reader.ReadColon();
            var cv = new AsyncJsonDeserializer<IV>(impl);
            var vv = await valueColion.Collect<ValueTask<IV>, AsyncJsonDeserializer<IV>>(ref cv);
            effector.Effect(ref builder, new(vk, vv));
        }
    }

    private struct ObjectMapAsStringKeySeraColctor<B, IK, IV>(B builder, AsyncJsonDeserializer impl)
        : IMapSeraColctor<B, IK, IV, ValueTask>
    {
        public B builder = builder;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public async ValueTask CItem<CK, CV, E>(CK keyColion, CV valueColion, E effector)
            where CK : ISeraColion<IK> where CV : ISeraColion<IV> where E : ISeraEffector<B, KeyValuePair<IK, IV>>
        {
            var key_token = impl.reader.CurrentToken;
            if (key_token.Kind is not JsonTokenKind.String) impl.reader.ThrowExpected(JsonTokenKind.String);
            var ck = new AsyncJsonDeserializer<IK>(impl);
            var vk = await keyColion.Collect<ValueTask<IK>, AsyncJsonDeserializer<IK>>(ref ck);
            await impl.reader.ReadColon();
            var cv = new AsyncJsonDeserializer<IV>(impl);
            var vv = await valueColion.Collect<ValueTask<IV>, AsyncJsonDeserializer<IV>>(ref cv);
            effector.Effect(ref builder, new(vk, vv));
        }
    }

    #endregion

    #region Struct

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public async ValueTask<T> CStruct<C, B, M>(C colion, M mapper, Type<B> b)
        where C : IStructSeraColion<B> where M : ISeraMapper<B, T>
    {
        await reader.ReadObjectStart();
        if (colion.Fields is { } fields)
            return await CStaticStruct<C, B, M>(colion, mapper, fields);
        else return await CDynamicStruct<C, B, M>(colion, mapper);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private async ValueTask<T> CStaticStruct<C, B, M>(C colion, M mapper, SeraFieldInfos fields)
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
            else await reader.ReadComma();
            var field_name = await reader.ReadString();
            await reader.ReadColon();
            if (!fields.TryGet(field_name, out var info))
            {
                await reader.SkipValue();
                continue;
            }
            var key = long.TryParse(field_name, out var key_) ? (long?)key_ : null;
            var version = reader.Version;
            var r = await colion.CollectField<ValueTask<StructRes>, StructSeraColctor<B>>(ref c, info.index, field_name,
                key);
            switch (r)
            {
                case StructRes.None:
                    throw new DeserializeException(
                        $"Unable to read field {field_name}({info.index}) of struct {typeof(T)}");
                case StructRes.Skip:
                    await reader.SkipValue();
                    break;
                case StructRes.Field:
                    reader.AssertMove(version);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        await reader.ReadObjectEnd();
        if (!colion.FinishCollect(i)) throw new DeserializeException("Failed to collect tuple");
        return mapper.Map(c.builder);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private async ValueTask<T> CDynamicStruct<C, B, M>(C colion, M mapper)
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
            else await reader.ReadComma();
            var field_name = await reader.ReadString();
            await reader.ReadColon();
            var key = long.TryParse(field_name, out var key_) ? (long?)key_ : null;
            var r = await colion.CollectField<ValueTask<StructRes>, StructSeraColctor<B>>(ref c, i, field_name, key);
            switch (r)
            {
                case StructRes.None:
                case StructRes.Skip:
                    await reader.SkipValue();
                    break;
                case StructRes.Field:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        await reader.ReadObjectEnd();
        if (!colion.FinishCollect(i)) throw new DeserializeException("Failed to collect tuple");
        return mapper.Map(c.builder);
    }

    private enum StructRes : byte
    {
        None,
        Skip,
        Field,
    }

    private struct StructSeraColctor<B>(B builder, AsyncJsonDeserializer impl)
        : IStructSeraColctor<B, ValueTask<StructRes>>
    {
        public B builder = builder;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public async ValueTask<StructRes> CField<C, E, I>(C colion, E effector, Type<I> i)
            where C : ISeraColion<I> where E : ISeraEffector<B, I>
        {
            var c = new AsyncJsonDeserializer<I>(impl);
            var r = await colion.Collect<ValueTask<I>, AsyncJsonDeserializer<I>>(ref c);
            effector.Effect(ref builder, r);
            return StructRes.Field;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ValueTask<StructRes> CSkip() => ValueTask.FromResult(StructRes.Skip);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ValueTask<StructRes> CNone() => ValueTask.FromResult(StructRes.None);
    }

    #endregion

    #region Union

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public async ValueTask<T> CUnionEmpty<N>(N ctor) where N : ISeraCtor<T>
    {
        var token = reader.CurrentToken;
        if (token.Kind is JsonTokenKind.Null) goto ok;
        if (token.Kind is JsonTokenKind.ObjectStart)
        {
            await reader.ReadObjectStart();
            await reader.ReadObjectEnd();
            goto ok;
        }
        if (token.Kind is JsonTokenKind.ArrayStart)
        {
            await reader.ReadArrayStart();
            await reader.ReadArrayEnd();
            // ReSharper disable once RedundantJumpStatement
            goto ok;
        }
        ok:
        return ctor.Ctor();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public async ValueTask<T> CUnionVariants<C, B, M>(C colion, M mapper, Type<B> b, UnionStyle? union_style = null)
        where C : IVariantsSeraColion<B> where M : ISeraMapper<B, T>
    {
        var variants = colion.Variants;
        var token = reader.CurrentToken;
        (Variant variant, int index) info;
        if (token.Kind is JsonTokenKind.String or JsonTokenKind.Number)
        {
            var span = token.AsMemory();
            if (span.Span.TryParseVariantTag(variants.TagKind, union_style?.VariantFormats, out var tag))
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
            UnionFormat.External => await CUnionVariantsExternal<C, B, M>(variants, colion, mapper, union_style),
            UnionFormat.Internal => await CUnionVariantsInternal<C, B, M>(variants, colion, mapper, union_style),
            UnionFormat.Adjacent => await CUnionVariantsAdjacent<C, B, M>(variants, colion, mapper, union_style),
            UnionFormat.Tuple => await CUnionVariantsTuple<C, B, M>(variants, colion, mapper, union_style),
            UnionFormat.Untagged => await CUnionVariantsUntagged<C, B, M>(colion, mapper),
            _ => await CUnionVariantsAny<C, B, M>(variants, in token, colion, mapper, union_style),
        };
        ok:
        await reader.MoveNext();
        var c = new AsyncJsonDeserializer<B>.JustVariantSeraColctor(impl);
        var r = colion.CollectVariant<B, AsyncJsonDeserializer<B>.JustVariantSeraColctor>(ref c, info.index);
        return mapper.Map(r);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ValueTask<T> CUnionVariantsAny<C, B, M>(SeraVariantInfos variants, in JsonToken token,
        C colion, M mapper, UnionStyle? union_style)
        where C : IVariantsSeraColion<B> where M : ISeraMapper<B, T>
    {
        if (token.Kind is JsonTokenKind.ArrayStart)
            return CUnionVariantsTuple<C, B, M>(variants, colion, mapper, union_style);
        return CUnionVariantsExternal<C, B, M>(variants, colion, mapper, union_style);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public async ValueTask<T> CUnionVariantsExternal<C, B, M>(SeraVariantInfos variants, C colion, M mapper,
        UnionStyle? union_style)
        where C : IVariantsSeraColion<B> where M : ISeraMapper<B, T>
    {
        var start = await reader.ReadObjectStart();
        var key = await reader.ReadStringToken();
        var span = key.AsMemory();
        (Variant variant, int index) info;
        if (span.Span.TryParseVariantTag(variants.TagKind, union_style?.VariantFormats, out var tag))
        {
            if (variants.TryGet(tag, out info)) goto ok;
        }
        {
            var str = key.AsString();
            if (variants.TryGet(str, out info)) goto ok;
        }
        throw new JsonParseException($"Unknown Union Variant at {key.Pos}", key.Pos);
        ok:
        await reader.ReadColon();
        var c = new AsyncJsonDeserializer<B>.ValueVariantSeraColctor(impl, start.Pos);
        var r = await colion.CollectVariant<ValueTask<B>, AsyncJsonDeserializer<B>.ValueVariantSeraColctor>(
            ref c, info.index);
        await reader.ReadObjectEnd();
        return mapper.Map(r);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public async ValueTask<T> CUnionVariantsTuple<C, B, M>(SeraVariantInfos variants, C colion, M mapper,
        UnionStyle? union_style)
        where C : IVariantsSeraColion<B> where M : ISeraMapper<B, T>
    {
        var start = await reader.ReadArrayStart();
        var key = reader.CurrentToken;
        if (key.Kind is not (JsonTokenKind.Number or JsonTokenKind.String))
            reader.ThrowExpected(JsonTokenKind.Number, JsonTokenKind.String);
        var span = key.AsMemory();
        (Variant variant, int index) info;
        if (span.Span.TryParseVariantTag(variants.TagKind, union_style?.VariantFormats, out var tag))
        {
            if (variants.TryGet(tag, out info)) goto ok;
        }
        {
            var str = key.AsString();
            if (variants.TryGet(str, out info)) goto ok;
        }
        throw new JsonParseException($"Unknown Union Variant at {key.Pos}", key.Pos);
        ok:
        await reader.MoveNext();
        await reader.ReadComma();
        var c = new AsyncJsonDeserializer<B>.ValueVariantSeraColctor(impl, start.Pos);
        var r = await colion.CollectVariant<ValueTask<B>, AsyncJsonDeserializer<B>.ValueVariantSeraColctor>(
            ref c, info.index);
        await reader.ReadArrayEnd();
        return mapper.Map(r);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public async ValueTask<T> CUnionVariantsUntagged<C, B, M>(C colion, M mapper)
        where C : IVariantsSeraColion<B> where M : ISeraMapper<B, T>
    {
        var c = new AsyncJsonDeserializer<B>.UntaggedUnionSeraColctor(impl);
        var r = await colion.CollectUntagged<ValueTask<B>, AsyncJsonDeserializer<B>.UntaggedUnionSeraColctor>(ref c);
        return mapper.Map(r);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public async ValueTask<T> CUnionVariantsAdjacent<C, B, M>(SeraVariantInfos variants, C colion, M mapper,
        UnionStyle? union_style)
        where C : IVariantsSeraColion<B> where M : ISeraMapper<B, T>
    {
        var ast = await reader.ReadObject();
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

            var span = token.AsMemory();
            if (span.Span.TryParseVariantTag(variants.TagKind, union_style?.VariantFormats, out var tag))
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
    public async ValueTask<T> CUnionVariantsInternal<C, B, M>(SeraVariantInfos variants, C colion, M mapper,
        UnionStyle? union_style)
        where C : IVariantsSeraColion<B> where M : ISeraMapper<B, T>
    {
        var ast = await reader.ReadObject();
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

            var span = token.AsMemory();
            if (span.Span.TryParseVariantTag(variants.TagKind, union_style?.VariantFormats, out var tag))
            {
                if (variants.TryGet(tag, out variant)) goto ok;
            }
            if (!variants.TryGet(span.ToString(), out variant))
                throw new JsonParseException($"Unknown Union Variant \"{span}\" at {key.Value.Pos}", key.Value.Pos);
            ok: ;
        }

        var removed_ast = ast with { Map = ast.Map.TryRemove(keys_last)! };
        Debug.Assert(removed_ast.Map != null);
        var c = new AsyncJsonDeserializer<B>.InternalVariantSeraColctor(impl, removed_ast, style);

        var r = colion.CollectVariant<B, AsyncJsonDeserializer<B>.InternalVariantSeraColctor>(ref c, variant.index);
        return mapper.Map(r);
    }

    private readonly struct JustVariantSeraColctor(AsyncJsonDeserializer impl) : IVariantSeraColctor<T, T>
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

    private readonly struct ValueVariantSeraColctor(AsyncJsonDeserializer impl, SourcePos pos)
        : IVariantSeraColctor<T, ValueTask<T>>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public async ValueTask<T> CVariant<N>(N ctor) where N : ISeraCtor<T>
        {
            await impl.reader.ReadNull();
            return ctor.Ctor();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public async ValueTask<T> CVariantValue<C, M, I>(C colion, M mapper, Type<I> i)
            where C : ISeraColion<I> where M : ISeraMapper<I, T>
        {
            var c = new AsyncJsonDeserializer<I>(impl);
            var r = await colion.Collect<ValueTask<I>, AsyncJsonDeserializer<I>>(ref c);
            return mapper.Map(r);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public async ValueTask<T> CVariantTuple<C, M, I>(C colion, M mapper, Type<I> i)
            where C : ITupleSeraColion<I> where M : ISeraMapper<I, T>
        {
            var c = new AsyncJsonDeserializer<I>(impl);
            var r = await c.CTuple(colion, new IdentityMapper<I>(), new Type<I>());
            return mapper.Map(r);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public async ValueTask<T> CVariantStruct<C, M, I>(C colion, M mapper, Type<I> i)
            where C : IStructSeraColion<I> where M : ISeraMapper<I, T>
        {
            var c = new AsyncJsonDeserializer<I>(impl);
            var r = await c.CStruct(colion, new IdentityMapper<I>(), new Type<I>());
            return mapper.Map(r);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ValueTask<T> CNone() => throw new JsonParseException($"Unable to read union {typeof(T)} at {pos}", pos);
    }

    private readonly struct UntaggedUnionSeraColctor(AsyncJsonDeserializer impl)
        : IUntaggedUnionSeraColctor<T, ValueTask<T>>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public async ValueTask<T> CSelect<C, M, U>(C colion, M mapper, Type<U> u)
            where C : ISelectSeraColion<U> where M : ISeraMapper<U, T>
        {
            var c = new AsyncJsonDeserializer<T>(impl);
            return await c.CSelect(colion, mapper, u);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public async ValueTask<T> CSelectPrimitive<C, M, U>(C colion, M mapper, Type<U> u)
            where C : ISelectPrimitiveSeraColion<U>
            where M : ISeraMapper<U, T>
        {
            var c = new AsyncJsonDeserializer<T>(impl);
            return await c.CSelectPrimitive(colion, mapper, u);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ValueTask<T> CNone() => throw new DeserializeException($"Unable to read union {typeof(T)}");
    }

    private readonly struct InternalVariantSeraColctor(AsyncJsonDeserializer impl, JsonAstObject ast, UnionStyle style)
        : IVariantSeraColctor<T, T>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T CVariant<N>(N ctor) where N : ISeraCtor<T> => ctor.Ctor();

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
