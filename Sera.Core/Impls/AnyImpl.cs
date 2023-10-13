using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Sera.Core.De;
using Sera.Core.Ser;

namespace Sera.Core.Impls;

public class AnyImpl :
    ISerialize<SeraAny>, IAsyncSerialize<SeraAny>,
    IDeserialize<SeraAny>, IAsyncDeserialize<SeraAny>,
    ISeqSerializerReceiver<List<SeraAny>>,
    IMapSerializerReceiver<Dictionary<SeraAny, SeraAny>>,
    IStructSerializerReceiver<SeraAnyStruct>,
    IAsyncSeqSerializerReceiver<List<SeraAny>>,
    IAsyncMapSerializerReceiver<Dictionary<SeraAny, SeraAny>>,
    IAsyncStructSerializerReceiver<SeraAnyStruct>,
    IAnyDeserializerVisitor<SeraAny>, IAsyncAnyDeserializerVisitor<SeraAny>
{
    public static AnyImpl Instance { get; } = new();

    #region ISerialize

    public void Write<S>(S serializer, SeraAny value, ISeraOptions options) where S : ISerializer
    {
        switch (value.Kind)
        {
            case SeraAnyKind.Primitive:
                switch (value.PrimitiveType)
                {
                    case SeraPrimitiveTypes.Boolean:
                        serializer.WritePrimitive(value.PrimitiveBoolean, null);
                        break;
                    case SeraPrimitiveTypes.SByte:
                        serializer.WritePrimitive(value.PrimitiveSByte, null);
                        break;
                    case SeraPrimitiveTypes.Int16:
                        serializer.WritePrimitive(value.PrimitiveInt16, null);
                        break;
                    case SeraPrimitiveTypes.Int32:
                        serializer.WritePrimitive(value.PrimitiveInt32, null);
                        break;
                    case SeraPrimitiveTypes.Int64:
                        serializer.WritePrimitive(value.PrimitiveInt64, null);
                        break;
                    case SeraPrimitiveTypes.Int128:
                        serializer.WritePrimitive(value.PrimitiveInt128, null);
                        break;
                    case SeraPrimitiveTypes.Byte:
                        serializer.WritePrimitive(value.PrimitiveByte, null);
                        break;
                    case SeraPrimitiveTypes.UInt16:
                        serializer.WritePrimitive(value.PrimitiveUInt16, null);
                        break;
                    case SeraPrimitiveTypes.UInt32:
                        serializer.WritePrimitive(value.PrimitiveUInt32, null);
                        break;
                    case SeraPrimitiveTypes.UInt64:
                        serializer.WritePrimitive(value.PrimitiveUInt64, null);
                        break;
                    case SeraPrimitiveTypes.UInt128:
                        serializer.WritePrimitive(value.PrimitiveUInt128, null);
                        break;
                    case SeraPrimitiveTypes.IntPtr:
                        serializer.WritePrimitive(value.PrimitiveIntPtr, null);
                        break;
                    case SeraPrimitiveTypes.UIntPtr:
                        serializer.WritePrimitive(value.PrimitiveUIntPtr, null);
                        break;
                    case SeraPrimitiveTypes.Half:
                        serializer.WritePrimitive(value.PrimitiveHalf, null);
                        break;
                    case SeraPrimitiveTypes.Single:
                        serializer.WritePrimitive(value.PrimitiveSingle, null);
                        break;
                    case SeraPrimitiveTypes.Double:
                        serializer.WritePrimitive(value.PrimitiveDouble, null);
                        break;
                    case SeraPrimitiveTypes.Decimal:
                        serializer.WritePrimitive(value.PrimitiveDecimal, null);
                        break;
                    case SeraPrimitiveTypes.BigInteger:
                        serializer.WritePrimitive(value.PrimitiveBigInteger, null);
                        break;
                    case SeraPrimitiveTypes.Complex:
                        serializer.WritePrimitive(value.PrimitiveComplex, null);
                        break;
                    case SeraPrimitiveTypes.TimeSpan:
                        serializer.WritePrimitive(value.PrimitiveTimeSpan, null);
                        break;
                    case SeraPrimitiveTypes.DateOnly:
                        serializer.WritePrimitive(value.PrimitiveDateOnly, null);
                        break;
                    case SeraPrimitiveTypes.TimeOnly:
                        serializer.WritePrimitive(value.PrimitiveTimeOnly, null);
                        break;
                    case SeraPrimitiveTypes.DateTime:
                        serializer.WritePrimitive(value.PrimitiveDateTime, null);
                        break;
                    case SeraPrimitiveTypes.DateTimeOffset:
                        serializer.WritePrimitive(value.PrimitiveDateTimeOffset, null);
                        break;
                    case SeraPrimitiveTypes.Guid:
                        serializer.WritePrimitive(value.PrimitiveGuid, null);
                        break;
                    case SeraPrimitiveTypes.Range:
                        serializer.WritePrimitive(value.PrimitiveRange, null);
                        break;
                    case SeraPrimitiveTypes.Index:
                        serializer.WritePrimitive(value.PrimitiveIndex, null);
                        break;
                    case SeraPrimitiveTypes.Char:
                        serializer.WritePrimitive(value.PrimitiveChar, null);
                        break;
                    case SeraPrimitiveTypes.Rune:
                        serializer.WritePrimitive(value.PrimitiveRune, null);
                        break;
                    default:
                        serializer.WriteUnit();
                        break;
                }
                break;
            case SeraAnyKind.String:
                serializer.WriteString(value.String);
                break;
            case SeraAnyKind.Bytes:
                serializer.WriteBytes(value.Bytes.AsMemory());
                break;
            case SeraAnyKind.Unit:
                serializer.WriteUnit();
                break;
            case SeraAnyKind.Option:
            {
                var o = value.Option;
                if (o == null) serializer.WriteNone();
                else serializer.WriteSome(o.Value, this);
                break;
            }
            case SeraAnyKind.Seq:
            {
                var seq = value.Seq;
                serializer.StartSeq((nuint)seq.Count, seq, this);
                break;
            }
            case SeraAnyKind.Map:
            {
                var map = value.Map;
                serializer.StartMap((nuint)map.Count, map, this);
                break;
            }
            case SeraAnyKind.Struct:
            {
                var s = value.Struct;
                serializer.StartStruct(s.StructName, (nuint)s.Fields.Count, s, this);
                break;
            }
            case SeraAnyKind.Variant:
            {
                var v = value.Variant;
                if (v.Value.HasValue) serializer.WriteVariant(v.UnionName, v.Variant, v.Value.Value, this, default);
                else serializer.WriteVariantUnit(v.UnionName, v.Variant, default);
                break;
            }
            default:
                serializer.WriteUnit();
                break;
        }
    }

    public void Receive<S>(List<SeraAny> value, S serializer) where S : ISeqSerializer
    {
        foreach (var item in value)
        {
            serializer.WriteElement(item, this);
        }
    }

    public void Receive<S>(Dictionary<SeraAny, SeraAny> value, S serializer) where S : IMapSerializer
    {
        foreach (var (k, v) in value)
        {
            serializer.WriteEntry(k, v, this, this);
        }
    }

    public void Receive<S>(SeraAnyStruct value, S serialize) where S : IStructSerializer
    {
        foreach (var (k, ik, v) in value.Fields)
        {
            serialize.WriteField(k, ik, v, this);
        }
    }

    #endregion

    #region IAsyncSerialize

    public ValueTask WriteAsync<S>(S serializer, SeraAny value, ISeraOptions options) where S : IAsyncSerializer
        => value.Kind switch
        {
            SeraAnyKind.Primitive => value.PrimitiveType switch
            {
                SeraPrimitiveTypes.Boolean => serializer.WritePrimitiveAsync(value.PrimitiveBoolean, null),
                SeraPrimitiveTypes.SByte => serializer.WritePrimitiveAsync(value.PrimitiveSByte, null),
                SeraPrimitiveTypes.Int16 => serializer.WritePrimitiveAsync(value.PrimitiveInt16, null),
                SeraPrimitiveTypes.Int32 => serializer.WritePrimitiveAsync(value.PrimitiveInt32, null),
                SeraPrimitiveTypes.Int64 => serializer.WritePrimitiveAsync(value.PrimitiveInt64, null),
                SeraPrimitiveTypes.Int128 => serializer.WritePrimitiveAsync(value.PrimitiveInt128, null),
                SeraPrimitiveTypes.Byte => serializer.WritePrimitiveAsync(value.PrimitiveByte, null),
                SeraPrimitiveTypes.UInt16 => serializer.WritePrimitiveAsync(value.PrimitiveUInt16, null),
                SeraPrimitiveTypes.UInt32 => serializer.WritePrimitiveAsync(value.PrimitiveUInt32, null),
                SeraPrimitiveTypes.UInt64 => serializer.WritePrimitiveAsync(value.PrimitiveUInt64, null),
                SeraPrimitiveTypes.UInt128 => serializer.WritePrimitiveAsync(value.PrimitiveUInt128, null),
                SeraPrimitiveTypes.IntPtr => serializer.WritePrimitiveAsync(value.PrimitiveIntPtr, null),
                SeraPrimitiveTypes.UIntPtr => serializer.WritePrimitiveAsync(value.PrimitiveUIntPtr, null),
                SeraPrimitiveTypes.Half => serializer.WritePrimitiveAsync(value.PrimitiveHalf, null),
                SeraPrimitiveTypes.Single => serializer.WritePrimitiveAsync(value.PrimitiveSingle, null),
                SeraPrimitiveTypes.Double => serializer.WritePrimitiveAsync(value.PrimitiveDouble, null),
                SeraPrimitiveTypes.Decimal => serializer.WritePrimitiveAsync(value.PrimitiveDecimal, null),
                SeraPrimitiveTypes.BigInteger => serializer.WritePrimitiveAsync(value.PrimitiveBigInteger, null),
                SeraPrimitiveTypes.Complex => serializer.WritePrimitiveAsync(value.PrimitiveComplex, null),
                SeraPrimitiveTypes.TimeSpan => serializer.WritePrimitiveAsync(value.PrimitiveTimeSpan, null),
                SeraPrimitiveTypes.DateOnly => serializer.WritePrimitiveAsync(value.PrimitiveDateOnly, null),
                SeraPrimitiveTypes.TimeOnly => serializer.WritePrimitiveAsync(value.PrimitiveTimeOnly, null),
                SeraPrimitiveTypes.DateTime => serializer.WritePrimitiveAsync(value.PrimitiveDateTime, null),
                SeraPrimitiveTypes.DateTimeOffset =>
                    serializer.WritePrimitiveAsync(value.PrimitiveDateTimeOffset, null),
                SeraPrimitiveTypes.Guid => serializer.WritePrimitiveAsync(value.PrimitiveGuid, null),
                SeraPrimitiveTypes.Range => serializer.WritePrimitiveAsync(value.PrimitiveRange, null),
                SeraPrimitiveTypes.Index => serializer.WritePrimitiveAsync(value.PrimitiveIndex, null),
                SeraPrimitiveTypes.Char => serializer.WritePrimitiveAsync(value.PrimitiveChar, null),
                SeraPrimitiveTypes.Rune => serializer.WritePrimitiveAsync(value.PrimitiveRune, null),
                _ => serializer.WriteUnitAsync()
            },
            SeraAnyKind.String => serializer.WriteStringAsync(value.String),
            SeraAnyKind.Bytes => serializer.WriteBytesAsync(value.Bytes.AsMemory()),
            SeraAnyKind.Unit => serializer.WriteUnitAsync(),
            SeraAnyKind.Option => value.Option is not null and var o
                ? serializer.WriteSomeAsync(o.Value, this)
                : serializer.WriteNoneAsync(),
            SeraAnyKind.Seq => serializer.StartSeqAsync((nuint)value.Seq.Count, value.Seq, this),
            SeraAnyKind.Map => serializer.StartMapAsync((nuint)value.Map.Count, value.Map, this),
            SeraAnyKind.Struct => serializer.StartStructAsync(value.Struct.StructName,
                (nuint)value.Struct.Fields.Count, value.Struct, this),
            SeraAnyKind.Variant => value.Variant is { Value: not null and var v }
                ? serializer.WriteVariantAsync(value.Variant.UnionName, value.Variant.Variant, v.Value, this, default)
                : serializer.WriteVariantUnitAsync(value.Variant.UnionName, value.Variant.Variant, default),
            _ => serializer.WriteUnitAsync()
        };

    public async ValueTask ReceiveAsync<S>(List<SeraAny> value, S serializer) where S : IAsyncSeqSerializer
    {
        foreach (var item in value)
        {
            await serializer.WriteElementAsync(item, this);
        }
    }

    public async ValueTask ReceiveAsync<S>(Dictionary<SeraAny, SeraAny> value, S serializer)
        where S : IAsyncMapSerializer
    {
        foreach (var (k, v) in value)
        {
            await serializer.WriteEntryAsync(k, v, this, this);
        }
    }

    public async ValueTask ReceiveAsync<S>(SeraAnyStruct value, S serialize) where S : IAsyncStructSerializer
    {
        foreach (var (k, ik, v) in value.Fields)
        {
            await serialize.WriteFieldAsync(k, ik, v, this);
        }
    }

    #endregion

    #region IDeserialize

    public SeraAny Read<D>(D deserializer, ISeraOptions options) where D : IDeserializer
        => deserializer.ReadAny<SeraAny, AnyImpl>(null, null, this);

    public SeraAny Visit(bool value)
        => SeraAny.MakePrimitive(value);

    public SeraAny Visit(sbyte value)
        => SeraAny.MakePrimitive(value);

    public SeraAny Visit(short value)
        => SeraAny.MakePrimitive(value);

    public SeraAny Visit(int value)
        => SeraAny.MakePrimitive(value);

    public SeraAny Visit(long value)
        => SeraAny.MakePrimitive(value);

    public SeraAny Visit(Int128 value)
        => SeraAny.MakePrimitive(value);

    public SeraAny Visit(byte value)
        => SeraAny.MakePrimitive(value);

    public SeraAny Visit(ushort value)
        => SeraAny.MakePrimitive(value);

    public SeraAny Visit(uint value)
        => SeraAny.MakePrimitive(value);

    public SeraAny Visit(ulong value)
        => SeraAny.MakePrimitive(value);

    public SeraAny Visit(UInt128 value)
        => SeraAny.MakePrimitive(value);

    public SeraAny Visit(IntPtr value)
        => SeraAny.MakePrimitive(value);

    public SeraAny Visit(UIntPtr value)
        => SeraAny.MakePrimitive(value);

    public SeraAny Visit(Half value)
        => SeraAny.MakePrimitive(value);

    public SeraAny Visit(float value)
        => SeraAny.MakePrimitive(value);

    public SeraAny Visit(double value)
        => SeraAny.MakePrimitive(value);

    public SeraAny Visit(decimal value)
        => SeraAny.MakePrimitive(value);

    public SeraAny Visit(BigInteger value)
        => SeraAny.MakePrimitive(new SeraBoxed<BigInteger>(value));

    public SeraAny Visit(Complex value)
        => SeraAny.MakePrimitive(value);

    public SeraAny Visit(TimeSpan value)
        => SeraAny.MakePrimitive(value);

    public SeraAny Visit(DateOnly value)
        => SeraAny.MakePrimitive(value);

    public SeraAny Visit(TimeOnly value)
        => SeraAny.MakePrimitive(value);

    public SeraAny Visit(DateTime value)
        => SeraAny.MakePrimitive(value);

    public SeraAny Visit(DateTimeOffset value)
        => SeraAny.MakePrimitive(value);

    public SeraAny Visit(Guid value)
        => SeraAny.MakePrimitive(value);

    public SeraAny Visit(Range value)
        => SeraAny.MakePrimitive(value);

    public SeraAny Visit(Index value)
        => SeraAny.MakePrimitive(value);

    public SeraAny Visit(char value)
        => SeraAny.MakePrimitive(value);

    public SeraAny Visit(Rune value)
        => SeraAny.MakePrimitive(value);

    public SeraAny VisitString<A>(A access) where A : IStringAccess
        => SeraAny.MakeString(access.ReadString());

    public SeraAny VisitBytes<A>(A access) where A : IBytesAccess
        => SeraAny.MakeBytes(access.ReadBytes());

    public SeraAny VisitUnit()
        => SeraAny.MakeUnit();

    public SeraAny VisitNone()
        => SeraAny.MakeOption(null);

    public SeraAny VisitSome<D>(D deserializer, ISeraOptions options) where D : IDeserializer
        => SeraAny.MakeOption(new SeraBoxed<SeraAny>(Read(deserializer, options)));

    public SeraAny VisitSeq<A>(A access) where A : ISeqAccess
    {
        var cap = access.GetLength();
        List<SeraAny> list;
        if (cap.HasValue)
        {
            list = new((int)cap.Value);
            for (nuint i = 0; i < cap.Value; i++)
            {
                access.ReadElement(out SeraAny item, this);
                list.Add(item);
            }
        }
        else
        {
            list = new();
            while (access.HasNext())
            {
                access.ReadElement(out SeraAny item, this);
                list.Add(item);
            }
        }
        return SeraAny.MakeSeq(list);
    }

    public SeraAny VisitMap<A>(A access) where A : IMapAccess
    {
        var cap = access.GetLength();
        Dictionary<SeraAny, SeraAny> map;
        if (cap.HasValue)
        {
            map = new((int)cap.Value);
            for (nuint i = 0; i < cap.Value; i++)
            {
                access.ReadEntry(out SeraAny k, out SeraAny v, this, this);
                map[k] = v;
            }
        }
        else
        {
            map = new();
            while (access.HasNext())
            {
                access.ReadEntry(out SeraAny k, out SeraAny v, this, this);
                map[k] = v;
            }
        }
        return SeraAny.MakeMap(map);
    }

    public SeraAny VisitStructSeq<A>(A access) where A : IStructSeqAccess
    {
        var name = access.ViewStructName();
        var cap = access.GetLength();
        List<(string, long?, SeraAny)> fields;
        if (cap.HasValue)
        {
            fields = new((int)cap.Value);
            for (nuint i = 0; i < cap.Value; i++)
            {
                access.ReadField(out SeraAny v, this);
                fields.Add(($"{i}", (long)i, v));
            }
        }
        else
        {
            fields = new();
            for (nuint i = 0; access.HasNext(); i++)
            {
                access.ReadField(out SeraAny v, this);
                fields.Add(($"{i}", (long)i, v));
            }
        }
        return SeraAny.MakeStruct(new SeraAnyStruct(fields) { StructName = name });
    }

    public SeraAny VisitStructMap<A>(A access) where A : IStructMapAccess
    {
        var name = access.ViewStructName();
        var cap = access.GetLength();
        List<(string, long?, SeraAny)> fields;
        if (cap.HasValue)
        {
            fields = new((int)cap.Value);
            for (nuint i = 0; i < cap.Value; i++)
            {
                access.ReadField(out var k, out var ik, out SeraAny v, this);
                if (k == null && ik == null) throw new DeserializeException("Deserializer implementation error");
                fields.Add((k ?? $"{ik}", ik, v));
            }
        }
        else
        {
            fields = new();
            while (access.HasNext())
            {
                access.ReadField(out var k, out var ik, out SeraAny v, this);
                if (k == null && ik == null) throw new DeserializeException("Deserializer implementation error");
                fields.Add((k ?? $"{ik}", ik, v));
            }
        }
        return SeraAny.MakeStruct(new SeraAnyStruct(fields) { StructName = name });
    }

    public SeraAny VisitEmptyUnion() => SeraAny.MakeEmptyUnion();

    public SeraAny VisitVariantUnit<A>(Variant variant, A access) where A : IVariantAccess
    {
        var name = access.ViewUnionName();
        return SeraAny.MakeVariant(new SeraAnyVariant(variant, null) { UnionName = name });
    }

    public SeraAny VisitVariant<A, D>(Variant variant, A access, D deserializer, ISeraOptions options)
        where A : IVariantAccess where D : IDeserializer
    {
        var name = access.ViewUnionName();
        return SeraAny.MakeVariant(new SeraAnyVariant(variant, Read(deserializer, options)) { UnionName = name });
    }

    #endregion

    #region IAsyncDeserialize

    public ValueTask<SeraAny> ReadAsync<D>(D deserializer, ISeraOptions options) where D : IAsyncDeserializer
        => deserializer.ReadAnyAsync<SeraAny, AnyImpl>(null, null, this);

    public ValueTask<SeraAny> VisitAsync(bool value)
        => ValueTask.FromResult(SeraAny.MakePrimitive(value));

    public ValueTask<SeraAny> VisitAsync(sbyte value)
        => ValueTask.FromResult(SeraAny.MakePrimitive(value));

    public ValueTask<SeraAny> VisitAsync(short value)
        => ValueTask.FromResult(SeraAny.MakePrimitive(value));

    public ValueTask<SeraAny> VisitAsync(int value)
        => ValueTask.FromResult(SeraAny.MakePrimitive(value));

    public ValueTask<SeraAny> VisitAsync(long value)
        => ValueTask.FromResult(SeraAny.MakePrimitive(value));

    public ValueTask<SeraAny> VisitAsync(Int128 value)
        => ValueTask.FromResult(SeraAny.MakePrimitive(value));

    public ValueTask<SeraAny> VisitAsync(byte value)
        => ValueTask.FromResult(SeraAny.MakePrimitive(value));

    public ValueTask<SeraAny> VisitAsync(ushort value)
        => ValueTask.FromResult(SeraAny.MakePrimitive(value));

    public ValueTask<SeraAny> VisitAsync(uint value)
        => ValueTask.FromResult(SeraAny.MakePrimitive(value));

    public ValueTask<SeraAny> VisitAsync(ulong value)
        => ValueTask.FromResult(SeraAny.MakePrimitive(value));

    public ValueTask<SeraAny> VisitAsync(UInt128 value)
        => ValueTask.FromResult(SeraAny.MakePrimitive(value));

    public ValueTask<SeraAny> VisitAsync(IntPtr value)
        => ValueTask.FromResult(SeraAny.MakePrimitive(value));

    public ValueTask<SeraAny> VisitAsync(UIntPtr value)
        => ValueTask.FromResult(SeraAny.MakePrimitive(value));

    public ValueTask<SeraAny> VisitAsync(Half value)
        => ValueTask.FromResult(SeraAny.MakePrimitive(value));

    public ValueTask<SeraAny> VisitAsync(float value)
        => ValueTask.FromResult(SeraAny.MakePrimitive(value));

    public ValueTask<SeraAny> VisitAsync(double value)
        => ValueTask.FromResult(SeraAny.MakePrimitive(value));

    public ValueTask<SeraAny> VisitAsync(decimal value)
        => ValueTask.FromResult(SeraAny.MakePrimitive(value));

    public ValueTask<SeraAny> VisitAsync(BigInteger value)
        => ValueTask.FromResult(SeraAny.MakePrimitive(new SeraBoxed<BigInteger>(value)));

    public ValueTask<SeraAny> VisitAsync(Complex value)
        => ValueTask.FromResult(SeraAny.MakePrimitive(value));

    public ValueTask<SeraAny> VisitAsync(TimeSpan value)
        => ValueTask.FromResult(SeraAny.MakePrimitive(value));

    public ValueTask<SeraAny> VisitAsync(DateOnly value)
        => ValueTask.FromResult(SeraAny.MakePrimitive(value));

    public ValueTask<SeraAny> VisitAsync(TimeOnly value)
        => ValueTask.FromResult(SeraAny.MakePrimitive(value));

    public ValueTask<SeraAny> VisitAsync(DateTime value)
        => ValueTask.FromResult(SeraAny.MakePrimitive(value));

    public ValueTask<SeraAny> VisitAsync(DateTimeOffset value)
        => ValueTask.FromResult(SeraAny.MakePrimitive(value));

    public ValueTask<SeraAny> VisitAsync(Guid value)
        => ValueTask.FromResult(SeraAny.MakePrimitive(value));

    public ValueTask<SeraAny> VisitAsync(Range value)
        => ValueTask.FromResult(SeraAny.MakePrimitive(value));

    public ValueTask<SeraAny> VisitAsync(Index value)
        => ValueTask.FromResult(SeraAny.MakePrimitive(value));

    public ValueTask<SeraAny> VisitAsync(char value)
        => ValueTask.FromResult(SeraAny.MakePrimitive(value));

    public ValueTask<SeraAny> VisitAsync(Rune value)
        => ValueTask.FromResult(SeraAny.MakePrimitive(value));

    public async ValueTask<SeraAny> VisitStringAsync<A>(A access) where A : IAsyncStringAccess
        => SeraAny.MakeString(await access.ReadStringAsync());

    public async ValueTask<SeraAny> VisitBytesAsync<A>(A access) where A : IAsyncBytesAccess
        => SeraAny.MakeBytes(await access.ReadBytesAsync());

    public ValueTask<SeraAny> VisitUnitAsync()
        => ValueTask.FromResult(SeraAny.MakeUnit());

    public ValueTask<SeraAny> VisitNoneAsync()
        => ValueTask.FromResult(SeraAny.MakeOption(null));

    public async ValueTask<SeraAny> VisitSomeAsync<D>(D deserializer, ISeraOptions options) where D : IAsyncDeserializer
        => SeraAny.MakeOption(new SeraBoxed<SeraAny>(await ReadAsync(deserializer, options)));

    public async ValueTask<SeraAny> VisitSeqAsync<A>(A access) where A : IAsyncSeqAccess
    {
        var cap = await access.GetLengthAsync();
        List<SeraAny> list;
        if (cap.HasValue)
        {
            list = new((int)cap.Value);
            for (nuint i = 0; i < cap.Value; i++)
            {
                var item = await access.ReadElementAsync<SeraAny, AnyImpl>(this);
                list.Add(item);
            }
        }
        else
        {
            list = new();
            while (await access.HasNextAsync())
            {
                var item = await access.ReadElementAsync<SeraAny, AnyImpl>(this);
                list.Add(item);
            }
        }
        return SeraAny.MakeSeq(list);
    }

    public async ValueTask<SeraAny> VisitMapAsync<A>(A access) where A : IAsyncMapAccess
    {
        var cap = await access.GetLengthAsync();
        Dictionary<SeraAny, SeraAny> map;
        if (cap.HasValue)
        {
            map = new((int)cap.Value);
            for (nuint i = 0; i < cap.Value; i++)
            {
                var (k, v) = await access.ReadEntryAsync<SeraAny, SeraAny, AnyImpl, AnyImpl>(this, this);
                map[k] = v;
            }
        }
        else
        {
            map = new();
            while (await access.HasNextAsync())
            {
                var (k, v) = await access.ReadEntryAsync<SeraAny, SeraAny, AnyImpl, AnyImpl>(this, this);
                map[k] = v;
            }
        }
        return SeraAny.MakeMap(map);
    }

    public async ValueTask<SeraAny> VisitStructSeqAsync<A>(A access) where A : IAsyncStructSeqAccess
    {
        var name = await access.ViewStructNameAsync();
        var cap = await access.GetLengthAsync();
        List<(string, long?, SeraAny)> fields;
        if (cap.HasValue)
        {
            fields = new((int)cap.Value);
            for (nuint i = 0; i < cap.Value; i++)
            {
                var v = await access.ReadFieldAsync<SeraAny, AnyImpl>(this);
                fields.Add(($"{i}", (long)i, v));
            }
        }
        else
        {
            fields = new();
            for (nuint i = 0; await access.HasNextAsync(); i++)
            {
                var v = await access.ReadFieldAsync<SeraAny, AnyImpl>(this);
                fields.Add(($"{i}", (long)i, v));
            }
        }
        return SeraAny.MakeStruct(new SeraAnyStruct(fields) { StructName = name });
    }

    public async ValueTask<SeraAny> VisitStructMapAsync<A>(A access) where A : IAsyncStructMapAccess
    {
        var name = await access.ViewStructNameAsync();
        var cap = await access.GetLengthAsync();
        List<(string, long?, SeraAny)> fields;
        if (cap.HasValue)
        {
            fields = new((int)cap.Value);
            for (nuint i = 0; i < cap.Value; i++)
            {
                var (k, ik, v) = await access.ReadFieldAsync<SeraAny, AnyImpl>(this);
                if (k == null && ik == null) throw new DeserializeException("Deserializer implementation error");
                fields.Add((k ?? $"{ik}", ik, v));
            }
        }
        else
        {
            fields = new();
            while (await access.HasNextAsync())
            {
                var (k, ik, v) = await access.ReadFieldAsync<SeraAny, AnyImpl>(this);
                if (k == null && ik == null) throw new DeserializeException("Deserializer implementation error");
                fields.Add((k ?? $"{ik}", ik, v));
            }
        }
        return SeraAny.MakeStruct(new SeraAnyStruct(fields) { StructName = name });
    }

    public ValueTask<SeraAny> VisitEmptyUnionAsync() => ValueTask.FromResult(SeraAny.MakeEmptyUnion());

    public async ValueTask<SeraAny> VisitVariantUnitAsync<A>(Variant variant, A access) where A : IAsyncVariantAccess
    {
        var name = await access.ViewUnionNameAsync();
        return SeraAny.MakeVariant(new SeraAnyVariant(variant, null) { UnionName = name });
    }

    public async ValueTask<SeraAny> VisitVariantAsync<A, D>(Variant variant, A access, D deserializer,
        ISeraOptions options)
        where A : IAsyncVariantAccess where D : IAsyncDeserializer
    {
        var name = await access.ViewUnionNameAsync();
        return SeraAny.MakeVariant(new SeraAnyVariant(variant, await ReadAsync(deserializer, options))
            { UnionName = name });
    }

    #endregion
}
