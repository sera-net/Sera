using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Sera.Core.De;
using Sera.Core.Ser;

namespace Sera.Core.Impls;

public record AnyImpl : ISerialize<SeraAny>, IDeserialize<SeraAny>
{
    public static AnyImpl Instance { get; } = new();

    public void Write<S>(S serializer, in SeraAny value, SeraOptions options) where S : ISerializer
    {
        switch (value.Kind)
        {
            case SeraAnyKind.Primitive:
                switch (value.PrimitiveType)
                {
                    case SeraPrimitiveTypes.Boolean:
                        serializer.WritePrimitive(value.PrimitiveBoolean, SeraPrimitiveTypes.Boolean);
                        break;
                    case SeraPrimitiveTypes.SByte:
                        serializer.WritePrimitive(value.PrimitiveSByte, SeraPrimitiveTypes.SByte);
                        break;
                    case SeraPrimitiveTypes.Int16:
                        serializer.WritePrimitive(value.PrimitiveInt16, SeraPrimitiveTypes.Int16);
                        break;
                    case SeraPrimitiveTypes.Int32:
                        serializer.WritePrimitive(value.PrimitiveInt32, SeraPrimitiveTypes.Int32);
                        break;
                    case SeraPrimitiveTypes.Int64:
                        serializer.WritePrimitive(value.PrimitiveInt64, SeraPrimitiveTypes.Int64);
                        break;
                    case SeraPrimitiveTypes.Int128:
                        serializer.WritePrimitive(value.PrimitiveInt128, SeraPrimitiveTypes.Int128);
                        break;
                    case SeraPrimitiveTypes.Byte:
                        serializer.WritePrimitive(value.PrimitiveByte, SeraPrimitiveTypes.Byte);
                        break;
                    case SeraPrimitiveTypes.UInt16:
                        serializer.WritePrimitive(value.PrimitiveUInt16, SeraPrimitiveTypes.UInt16);
                        break;
                    case SeraPrimitiveTypes.UInt32:
                        serializer.WritePrimitive(value.PrimitiveUInt32, SeraPrimitiveTypes.UInt32);
                        break;
                    case SeraPrimitiveTypes.UInt64:
                        serializer.WritePrimitive(value.PrimitiveUInt64, SeraPrimitiveTypes.UInt64);
                        break;
                    case SeraPrimitiveTypes.UInt128:
                        serializer.WritePrimitive(value.PrimitiveUInt128, SeraPrimitiveTypes.UInt128);
                        break;
                    case SeraPrimitiveTypes.IntPtr:
                        serializer.WritePrimitive(value.PrimitiveIntPtr, SeraPrimitiveTypes.IntPtr);
                        break;
                    case SeraPrimitiveTypes.UIntPtr:
                        serializer.WritePrimitive(value.PrimitiveUIntPtr, SeraPrimitiveTypes.UIntPtr);
                        break;
                    case SeraPrimitiveTypes.Half:
                        serializer.WritePrimitive(value.PrimitiveHalf, SeraPrimitiveTypes.Half);
                        break;
                    case SeraPrimitiveTypes.Single:
                        serializer.WritePrimitive(value.PrimitiveSingle, SeraPrimitiveTypes.Single);
                        break;
                    case SeraPrimitiveTypes.Double:
                        serializer.WritePrimitive(value.PrimitiveDouble, SeraPrimitiveTypes.Double);
                        break;
                    case SeraPrimitiveTypes.Decimal:
                        serializer.WritePrimitive(value.PrimitiveDecimal, SeraPrimitiveTypes.Decimal);
                        break;
                    case SeraPrimitiveTypes.BigInteger:
                        serializer.WritePrimitive(value.PrimitiveBigInteger, SeraPrimitiveTypes.BigInteger);
                        break;
                    case SeraPrimitiveTypes.Complex:
                        serializer.WritePrimitive(value.PrimitiveComplex, SeraPrimitiveTypes.Complex);
                        break;
                    case SeraPrimitiveTypes.DateOnly:
                        serializer.WritePrimitive(value.PrimitiveDateOnly, SeraPrimitiveTypes.DateOnly);
                        break;
                    case SeraPrimitiveTypes.DateTime:
                        serializer.WritePrimitive(value.PrimitiveDateTime, SeraPrimitiveTypes.DateTime);
                        break;
                    case SeraPrimitiveTypes.DateTimeOffset:
                        serializer.WritePrimitive(value.PrimitiveDateTimeOffset, SeraPrimitiveTypes.DateTimeOffset);
                        break;
                    case SeraPrimitiveTypes.Guid:
                        serializer.WritePrimitive(value.PrimitiveGuid, SeraPrimitiveTypes.Guid);
                        break;
                    case SeraPrimitiveTypes.Range:
                        serializer.WritePrimitive(value.PrimitiveRange, SeraPrimitiveTypes.Range);
                        break;
                    case SeraPrimitiveTypes.Index:
                        serializer.WritePrimitive(value.PrimitiveIndex, SeraPrimitiveTypes.Index);
                        break;
                    case SeraPrimitiveTypes.Char:
                        serializer.WritePrimitive(value.PrimitiveChar, SeraPrimitiveTypes.Char);
                        break;
                    case SeraPrimitiveTypes.Rune:
                        serializer.WritePrimitive(value.PrimitiveRune, SeraPrimitiveTypes.Rune);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                break;
            case SeraAnyKind.String:
                serializer.WriteString(value.String);
                break;
            case SeraAnyKind.Bytes:
                serializer.WriteBytes(value.Bytes);
                break;
            case SeraAnyKind.Unit:
                serializer.WriteUnit();
                break;
            case SeraAnyKind.Null:
                serializer.WriteNull();
                break;
            case SeraAnyKind.NullableNotNull:
            {
                var v = value.NullableNotNull;
                serializer.WriteNullableNotNull(in v.Value, Instance);
                break;
            }
            case SeraAnyKind.Enum:
            {
                var e = value.Enum;
                serializer.WriteEnum(e.Name, e.Number, Instance);
                break;
            }
            case SeraAnyKind.Tuple:
            {
                var t = value.Tuple;
                var len = (nuint)t.LongLength;
                serializer.WriteTupleStart(len);
                for (nuint i = 0; i < len; i++)
                {
                    serializer.WriteTupleElement(in t[i], Instance);
                }
                serializer.WriteTupleEnd();
                break;
            }
            case SeraAnyKind.Seq:
            {
                var s = value.Seq;
                serializer.WriteSeqStart<SeraAny>((nuint)s.Count);
                foreach (var i in s)
                {
                    serializer.WriteSeqElement(i, Instance);
                }
                serializer.WriteSeqEnd();
                break;
            }
            case SeraAnyKind.Map:
            {
                var m = value.Map;
                serializer.WriteMapStart<SeraAny, SeraAny>((nuint)m.Count);
                foreach (var (k, v) in m)
                {
                    serializer.WriteMapEntry(k, v, Instance, Instance);
                }
                serializer.WriteMapEnd();
                break;
            }
            case SeraAnyKind.Struct:
            {
                var s = value.Struct;
                serializer.WriteStructStart(s.StructName, (nuint)s.Fields.Count);
                foreach (var (k, v) in s.Fields)
                {
                    serializer.WriteStructField(k, v, Instance);
                }
                serializer.WriteStructEnd();
                break;
            }
            case SeraAnyKind.Variant:
            {
                var v = value.Variant;
                serializer.WriteVariantStart(v.UnionName, v.VariantName, v.VariantTag);
                if (v.Value.Kind is SeraAnyKind.Unit)
                {
                    serializer.WriteVariantValueUnit();
                }
                else
                {
                    serializer.WriteVariantValue(in v.Value, Instance);
                }
                serializer.WriteVariantEnd();
                break;
            }
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public async ValueTask WriteAsync<S>(S serializer, SeraAny value, SeraOptions options) where S : IAsyncSerializer
    {
        switch (value.Kind)
        {
            case SeraAnyKind.Primitive:
                switch (value.PrimitiveType)
                {
                    case SeraPrimitiveTypes.Boolean:
                        await serializer.WritePrimitiveAsync(value.PrimitiveBoolean, SeraPrimitiveTypes.Boolean);
                        break;
                    case SeraPrimitiveTypes.SByte:
                        await serializer.WritePrimitiveAsync(value.PrimitiveSByte, SeraPrimitiveTypes.SByte);
                        break;
                    case SeraPrimitiveTypes.Int16:
                        await serializer.WritePrimitiveAsync(value.PrimitiveInt16, SeraPrimitiveTypes.Int16);
                        break;
                    case SeraPrimitiveTypes.Int32:
                        await serializer.WritePrimitiveAsync(value.PrimitiveInt32, SeraPrimitiveTypes.Int32);
                        break;
                    case SeraPrimitiveTypes.Int64:
                        await serializer.WritePrimitiveAsync(value.PrimitiveInt64, SeraPrimitiveTypes.Int64);
                        break;
                    case SeraPrimitiveTypes.Int128:
                        await serializer.WritePrimitiveAsync(value.PrimitiveInt128, SeraPrimitiveTypes.Int128);
                        break;
                    case SeraPrimitiveTypes.Byte:
                        await serializer.WritePrimitiveAsync(value.PrimitiveByte, SeraPrimitiveTypes.Byte);
                        break;
                    case SeraPrimitiveTypes.UInt16:
                        await serializer.WritePrimitiveAsync(value.PrimitiveUInt16, SeraPrimitiveTypes.UInt16);
                        break;
                    case SeraPrimitiveTypes.UInt32:
                        await serializer.WritePrimitiveAsync(value.PrimitiveUInt32, SeraPrimitiveTypes.UInt32);
                        break;
                    case SeraPrimitiveTypes.UInt64:
                        await serializer.WritePrimitiveAsync(value.PrimitiveUInt64, SeraPrimitiveTypes.UInt64);
                        break;
                    case SeraPrimitiveTypes.UInt128:
                        await serializer.WritePrimitiveAsync(value.PrimitiveUInt128, SeraPrimitiveTypes.UInt128);
                        break;
                    case SeraPrimitiveTypes.IntPtr:
                        await serializer.WritePrimitiveAsync(value.PrimitiveIntPtr, SeraPrimitiveTypes.IntPtr);
                        break;
                    case SeraPrimitiveTypes.UIntPtr:
                        await serializer.WritePrimitiveAsync(value.PrimitiveUIntPtr, SeraPrimitiveTypes.UIntPtr);
                        break;
                    case SeraPrimitiveTypes.Half:
                        await serializer.WritePrimitiveAsync(value.PrimitiveHalf, SeraPrimitiveTypes.Half);
                        break;
                    case SeraPrimitiveTypes.Single:
                        await serializer.WritePrimitiveAsync(value.PrimitiveSingle, SeraPrimitiveTypes.Single);
                        break;
                    case SeraPrimitiveTypes.Double:
                        await serializer.WritePrimitiveAsync(value.PrimitiveDouble, SeraPrimitiveTypes.Double);
                        break;
                    case SeraPrimitiveTypes.Decimal:
                        await serializer.WritePrimitiveAsync(value.PrimitiveDecimal, SeraPrimitiveTypes.Decimal);
                        break;
                    case SeraPrimitiveTypes.BigInteger:
                        await serializer.WritePrimitiveAsync(value.PrimitiveBigInteger, SeraPrimitiveTypes.BigInteger);
                        break;
                    case SeraPrimitiveTypes.Complex:
                        await serializer.WritePrimitiveAsync(value.PrimitiveComplex, SeraPrimitiveTypes.Complex);
                        break;
                    case SeraPrimitiveTypes.DateOnly:
                        await serializer.WritePrimitiveAsync(value.PrimitiveDateOnly, SeraPrimitiveTypes.DateOnly);
                        break;
                    case SeraPrimitiveTypes.DateTime:
                        await serializer.WritePrimitiveAsync(value.PrimitiveDateTime, SeraPrimitiveTypes.DateTime);
                        break;
                    case SeraPrimitiveTypes.DateTimeOffset:
                        await serializer.WritePrimitiveAsync(value.PrimitiveDateTimeOffset,
                            SeraPrimitiveTypes.DateTimeOffset);
                        break;
                    case SeraPrimitiveTypes.Guid:
                        await serializer.WritePrimitiveAsync(value.PrimitiveGuid, SeraPrimitiveTypes.Guid);
                        break;
                    case SeraPrimitiveTypes.Range:
                        await serializer.WritePrimitiveAsync(value.PrimitiveRange, SeraPrimitiveTypes.Range);
                        break;
                    case SeraPrimitiveTypes.Index:
                        await serializer.WritePrimitiveAsync(value.PrimitiveIndex, SeraPrimitiveTypes.Index);
                        break;
                    case SeraPrimitiveTypes.Char:
                        await serializer.WritePrimitiveAsync(value.PrimitiveChar, SeraPrimitiveTypes.Char);
                        break;
                    case SeraPrimitiveTypes.Rune:
                        await serializer.WritePrimitiveAsync(value.PrimitiveRune, SeraPrimitiveTypes.Rune);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                break;
            case SeraAnyKind.String:
                await serializer.WriteStringAsync(value.String);
                break;
            case SeraAnyKind.Bytes:
                await serializer.WriteBytesAsync(value.Bytes);
                break;
            case SeraAnyKind.Unit:
                await serializer.WriteUnitAsync();
                break;
            case SeraAnyKind.Null:
                await serializer.WriteNullAsync();
                break;
            case SeraAnyKind.NullableNotNull:
            {
                var v = value.NullableNotNull;
                await serializer.WriteNullableNotNullAsync(v.Value, Instance);
                break;
            }
            case SeraAnyKind.Enum:
            {
                var e = value.Enum;
                await serializer.WriteEnumAsync(e.Name, e.Number, Instance);
                break;
            }
            case SeraAnyKind.Tuple:
            {
                var t = value.Tuple;
                var len = (nuint)t.LongLength;
                await serializer.WriteTupleStartAsync(len);
                for (nuint i = 0; i < len; i++)
                {
                    await serializer.WriteTupleElementAsync(t[i], Instance);
                }
                await serializer.WriteTupleEndAsync();
                break;
            }
            case SeraAnyKind.Seq:
            {
                var s = value.Seq;
                await serializer.WriteSeqStartAsync<SeraAny>((nuint)s.Count);
                foreach (var i in s)
                {
                    await serializer.WriteSeqElementAsync(i, Instance);
                }
                await serializer.WriteSeqEndAsync();
                break;
            }
            case SeraAnyKind.Map:
            {
                var m = value.Map;
                await serializer.WriteMapStartAsync<SeraAny, SeraAny>((nuint)m.Count);
                foreach (var (k, v) in m)
                {
                    await serializer.WriteMapEntryAsync(k, v, Instance, Instance);
                }
                await serializer.WriteMapEndAsync();
                break;
            }
            case SeraAnyKind.Struct:
            {
                var s = value.Struct;
                await serializer.WriteStructStartAsync(s.StructName, (nuint)s.Fields.Count);
                foreach (var (k, v) in s.Fields)
                {
                    await serializer.WriteStructFieldAsync(k, v, Instance);
                }
                await serializer.WriteStructEndAsync();
                break;
            }
            case SeraAnyKind.Variant:
            {
                var v = value.Variant;
                await serializer.WriteVariantStartAsync(v.UnionName, v.VariantName, v.VariantTag);
                if (v.Value.Kind is SeraAnyKind.Unit)
                {
                    await serializer.WriteVariantValueUnitAsync();
                }
                else
                {
                    await serializer.WriteVariantValueAsync(v.Value, Instance);
                }
                await serializer.WriteVariantEndAsync();
                break;
            }
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void Read<D>(D deserializer, out SeraAny value, SeraOptions options) where D : IDeserializer
    {
        var peek = deserializer.PeekNext();
        if ((peek & DeserializerHint.Null) != 0)
        {
            deserializer.ReadNull();
            value = SeraAny.MakeNull();
        }
        else if ((peek & DeserializerHint.Unit) != 0)
        {
            deserializer.ReadUnit();
            value = SeraAny.MakeUnit();
        }
        else if ((peek & DeserializerHint.Primitive) != 0)
        {
            var pri = deserializer.PeekPrimitive();

            if ((pri & DeserializerPrimitiveHint.Decimal) != 0)
            {
                var v = deserializer.ReadPrimitive<decimal>(SeraPrimitiveTypes.Decimal);
                value = SeraAny.MakePrimitive(v);
            }
            else if ((pri & DeserializerPrimitiveHint.UInt128) != 0)
            {
                var v = deserializer.ReadPrimitive<UInt128>(SeraPrimitiveTypes.UInt128);
                value = SeraAny.MakePrimitive(v);
            }
            else if ((pri & DeserializerPrimitiveHint.Int128) != 0)
            {
                var v = deserializer.ReadPrimitive<Int128>(SeraPrimitiveTypes.Int128);
                value = SeraAny.MakePrimitive(v);
            }

            else if ((pri & DeserializerPrimitiveHint.Double) != 0)
            {
                var v = deserializer.ReadPrimitive<double>(SeraPrimitiveTypes.Double);
                value = SeraAny.MakePrimitive(v);
            }
            else if ((pri & DeserializerPrimitiveHint.UInt64) != 0)
            {
                var v = deserializer.ReadPrimitive<ulong>(SeraPrimitiveTypes.UInt64);
                value = SeraAny.MakePrimitive(v);
            }
            else if ((pri & DeserializerPrimitiveHint.Int64) != 0)
            {
                var v = deserializer.ReadPrimitive<long>(SeraPrimitiveTypes.Int64);
                value = SeraAny.MakePrimitive(v);
            }

            else if ((pri & DeserializerPrimitiveHint.Single) != 0)
            {
                var v = deserializer.ReadPrimitive<float>(SeraPrimitiveTypes.Single);
                value = SeraAny.MakePrimitive(v);
            }
            else if ((pri & DeserializerPrimitiveHint.UInt32) != 0)
            {
                var v = deserializer.ReadPrimitive<uint>(SeraPrimitiveTypes.UInt32);
                value = SeraAny.MakePrimitive(v);
            }
            else if ((pri & DeserializerPrimitiveHint.Int32) != 0)
            {
                var v = deserializer.ReadPrimitive<int>(SeraPrimitiveTypes.Int32);
                value = SeraAny.MakePrimitive(v);
            }
            else if ((pri & DeserializerPrimitiveHint.Rune) != 0)
            {
                var v = deserializer.ReadPrimitive<Guid>(SeraPrimitiveTypes.Guid);
                value = SeraAny.MakePrimitive(v);
            }

            else if ((pri & DeserializerPrimitiveHint.Half) != 0)
            {
                var v = deserializer.ReadPrimitive<float>(SeraPrimitiveTypes.Single);
                value = SeraAny.MakePrimitive(v);
            }
            else if ((pri & DeserializerPrimitiveHint.UInt16) != 0)
            {
                var v = deserializer.ReadPrimitive<ushort>(SeraPrimitiveTypes.UInt16);
                value = SeraAny.MakePrimitive(v);
            }
            else if ((pri & DeserializerPrimitiveHint.Int16) != 0)
            {
                var v = deserializer.ReadPrimitive<short>(SeraPrimitiveTypes.Int16);
                value = SeraAny.MakePrimitive(v);
            }
            else if ((pri & DeserializerPrimitiveHint.Char) != 0)
            {
                var v = deserializer.ReadPrimitive<char>(SeraPrimitiveTypes.Char);
                value = SeraAny.MakePrimitive(v);
            }

            else if ((pri & DeserializerPrimitiveHint.Byte) != 0)
            {
                var v = deserializer.ReadPrimitive<byte>(SeraPrimitiveTypes.Byte);
                value = SeraAny.MakePrimitive(v);
            }
            else if ((pri & DeserializerPrimitiveHint.SByte) != 0)
            {
                var v = deserializer.ReadPrimitive<sbyte>(SeraPrimitiveTypes.SByte);
                value = SeraAny.MakePrimitive(v);
            }

            else if ((pri & DeserializerPrimitiveHint.Boolean) != 0)
            {
                var v = deserializer.ReadPrimitive<bool>(SeraPrimitiveTypes.Boolean);
                value = SeraAny.MakePrimitive(v);
            }

            else if ((pri & DeserializerPrimitiveHint.BigInteger) != 0)
            {
                var v = deserializer.ReadPrimitive<BigInteger>(SeraPrimitiveTypes.BigInteger);
                value = SeraAny.MakePrimitive(new SeraBoxed<BigInteger>(v));
            }
            else if ((pri & DeserializerPrimitiveHint.Complex) != 0)
            {
                var v = deserializer.ReadPrimitive<Complex>(SeraPrimitiveTypes.Complex);
                value = SeraAny.MakePrimitive(v);
            }

            else if ((pri & DeserializerPrimitiveHint.UIntPtr) != 0)
            {
                var v = deserializer.ReadPrimitive<UIntPtr>(SeraPrimitiveTypes.UIntPtr);
                value = SeraAny.MakePrimitive(v);
            }
            else if ((pri & DeserializerPrimitiveHint.IntPtr) != 0)
            {
                var v = deserializer.ReadPrimitive<IntPtr>(SeraPrimitiveTypes.IntPtr);
                value = SeraAny.MakePrimitive(v);
            }

            else if ((pri & DeserializerPrimitiveHint.DateTimeOffset) != 0)
            {
                var v = deserializer.ReadPrimitive<DateTimeOffset>(SeraPrimitiveTypes.DateTimeOffset);
                value = SeraAny.MakePrimitive(v);
            }
            else if ((pri & DeserializerPrimitiveHint.DateTime) != 0)
            {
                var v = deserializer.ReadPrimitive<DateTime>(SeraPrimitiveTypes.DateTime);
                value = SeraAny.MakePrimitive(v);
            }
            else if ((pri & DeserializerPrimitiveHint.DateOnly) != 0)
            {
                var v = deserializer.ReadPrimitive<DateOnly>(SeraPrimitiveTypes.DateOnly);
                value = SeraAny.MakePrimitive(v);
            }

            else if ((pri & DeserializerPrimitiveHint.Guid) != 0)
            {
                var v = deserializer.ReadPrimitive<Guid>(SeraPrimitiveTypes.Guid);
                value = SeraAny.MakePrimitive(v);
            }

            else if ((pri & DeserializerPrimitiveHint.Range) != 0)
            {
                var v = deserializer.ReadPrimitive<Guid>(SeraPrimitiveTypes.Guid);
                value = SeraAny.MakePrimitive(v);
            }
            else if ((pri & DeserializerPrimitiveHint.Index) != 0)
            {
                var v = deserializer.ReadPrimitive<Guid>(SeraPrimitiveTypes.Guid);
                value = SeraAny.MakePrimitive(v);
            }

            else
            {
                value = SeraAny.MakePrimitive();
            }
        }
        else if ((peek & DeserializerHint.String) != 0)
        {
            var v = deserializer.ReadString();
            value = SeraAny.MakeString(v);
        }
        else if ((peek & DeserializerHint.Bytes) != 0)
        {
            var v = deserializer.ReadBytes();
            value = SeraAny.MakeBytes(v);
        }
        else if ((peek & DeserializerHint.Enum) != 0)
        {
            deserializer.ReadEnum(out var name, out SeraAny a, Instance);
            value = SeraAny.MakeEnum(new SeraAnyEnum(name, a));
        }
        else if ((peek & DeserializerHint.Struct) != 0)
        {
            var len = deserializer.ReadStructStart(null, null);
            var name = deserializer.ViewStructName();
            Dictionary<string, SeraAny> fields;
            if (len.HasValue)
            {
                fields = new((int)len.Value);
                for (nuint i = 0; i < len.Value; i++)
                {
                    var key = deserializer.PeekStructNextKey() ?? $"{i}";
                    deserializer.ReadStructField(out SeraAny a, Instance);
                    fields[key] = a;
                }
            }
            else
            {
                fields = new();
                for (nuint i = 0; deserializer.PeekStructHasNext(); i++)
                {
                    var key = deserializer.PeekStructNextKey() ?? $"{i}";
                    deserializer.ReadStructField(out SeraAny a, Instance);
                    fields[key] = a;
                }
            }
            deserializer.ReadStructEnd();
            value = SeraAny.MakeStruct(new SeraAnyStruct(fields) { StructName = name });
        }
        else if ((peek & DeserializerHint.Variant) != 0)
        {
            deserializer.ReadVariantStart(null);
            var union_name = deserializer.ViewUnionName();
            var name = deserializer.ViewVariantName();
            var tag = deserializer.ViewVariantTag();
            SeraAny a;
            if (deserializer.PeekVariantIsUnit())
            {
                deserializer.ReadVariantValueUnit();
                a = SeraAny.MakeUnit();
            }
            else
            {
                deserializer.ReadVariantValue(out a, Instance);
            }
            deserializer.ReadVariantEnd();
            value = SeraAny.MakeVariant(new SeraAnyVariant(name, tag ?? 0, a) { UnionName = union_name });
        }
        else if ((peek & DeserializerHint.Tuple) != 0)
        {
            var len = deserializer.ReadTupleStart(null);
            SeraAny[] arr;
            if (len.HasValue)
            {
                arr = new SeraAny[len.Value];
                for (nuint i = 0; i < len.Value; i++)
                {
                    deserializer.ReadTupleElement(out arr[i], Instance);
                }
            }
            else
            {
                var list = new List<SeraAny>();
                while (deserializer.PeekTupleHasNext())
                {
                    deserializer.ReadTupleElement(out SeraAny a, Instance);
                    list.Add(a);
                }
                arr = list.ToArray();
            }
            deserializer.ReadTupleEnd();
            value = SeraAny.MakeTuple(arr);
        }
        else if ((peek & DeserializerHint.Seq) != 0)
        {
            var len = deserializer.ReadSeqStart(null);
            List<SeraAny> list;
            if (len.HasValue)
            {
                list = new((int)len.Value);
                foreach (ref var a in CollectionsMarshal.AsSpan(list))
                {
                    deserializer.ReadSeqElement(out a, Instance);
                }
            }
            else
            {
                list = new();
                while (deserializer.PeekTupleHasNext())
                {
                    deserializer.ReadSeqElement(out SeraAny a, Instance);
                    list.Add(a);
                }
            }
            deserializer.ReadSeqEnd();
            value = SeraAny.MakeSeq(list);
        }
        else if ((peek & DeserializerHint.Map) != 0)
        {
            var len = deserializer.ReadMapStart(null);
            Dictionary<SeraAny, SeraAny> map;
            if (len.HasValue)
            {
                map = new((int)len.Value);
                for (nuint i = 0; i < len.Value; i++)
                {
                    deserializer.ReadMapEntry(out SeraAny k, out SeraAny v, Instance, Instance);
                    map[k] = v;
                }
            }
            else
            {
                map = new();
                while (deserializer.PeekMapHasNext())
                {
                    deserializer.ReadMapEntry(out SeraAny k, out SeraAny v, Instance, Instance);
                    map[k] = v;
                }
            }
            deserializer.ReadMapEnd();
            value = SeraAny.MakeMap(map);
        }
        else if ((peek & DeserializerHint.NullableNotNull) != 0)
        {
            deserializer.ReadNullableNotNull(out SeraAny a, Instance);
            value = SeraAny.MakeNullableNotNull(new SeraBoxed<SeraAny>(a));
        }
        else
        {
            deserializer.Skip();
            value = SeraAny.MakeUnknown();
        }
    }

    public async ValueTask<SeraAny> ReadAsync<D>(D deserializer, SeraOptions options) where D : IAsyncDeserializer
    {
        var peek = await deserializer.PeekNextAsync();
        if ((peek & DeserializerHint.Null) != 0)
        {
            await deserializer.ReadNullAsync();
            return SeraAny.MakeNull();
        }
        else if ((peek & DeserializerHint.Unit) != 0)
        {
            await deserializer.ReadUnitAsync();
            return SeraAny.MakeUnit();
        }
        else if ((peek & DeserializerHint.Primitive) != 0)
        {
            var pri = await deserializer.PeekPrimitiveAsync();

            if ((pri & DeserializerPrimitiveHint.Decimal) != 0)
            {
                var v = await deserializer.ReadPrimitiveAsync<decimal>(SeraPrimitiveTypes.Decimal);
                return SeraAny.MakePrimitive(v);
            }
            else if ((pri & DeserializerPrimitiveHint.UInt128) != 0)
            {
                var v = await deserializer.ReadPrimitiveAsync<UInt128>(SeraPrimitiveTypes.UInt128);
                return SeraAny.MakePrimitive(v);
            }
            else if ((pri & DeserializerPrimitiveHint.Int128) != 0)
            {
                var v = await deserializer.ReadPrimitiveAsync<Int128>(SeraPrimitiveTypes.Int128);
                return SeraAny.MakePrimitive(v);
            }

            else if ((pri & DeserializerPrimitiveHint.Double) != 0)
            {
                var v = await deserializer.ReadPrimitiveAsync<double>(SeraPrimitiveTypes.Double);
                return SeraAny.MakePrimitive(v);
            }
            else if ((pri & DeserializerPrimitiveHint.UInt64) != 0)
            {
                var v = await deserializer.ReadPrimitiveAsync<ulong>(SeraPrimitiveTypes.UInt64);
                return SeraAny.MakePrimitive(v);
            }
            else if ((pri & DeserializerPrimitiveHint.Int64) != 0)
            {
                var v = await deserializer.ReadPrimitiveAsync<long>(SeraPrimitiveTypes.Int64);
                return SeraAny.MakePrimitive(v);
            }

            else if ((pri & DeserializerPrimitiveHint.Single) != 0)
            {
                var v = await deserializer.ReadPrimitiveAsync<float>(SeraPrimitiveTypes.Single);
                return SeraAny.MakePrimitive(v);
            }
            else if ((pri & DeserializerPrimitiveHint.UInt32) != 0)
            {
                var v = await deserializer.ReadPrimitiveAsync<uint>(SeraPrimitiveTypes.UInt32);
                return SeraAny.MakePrimitive(v);
            }
            else if ((pri & DeserializerPrimitiveHint.Int32) != 0)
            {
                var v = await deserializer.ReadPrimitiveAsync<int>(SeraPrimitiveTypes.Int32);
                return SeraAny.MakePrimitive(v);
            }
            else if ((pri & DeserializerPrimitiveHint.Rune) != 0)
            {
                var v = await deserializer.ReadPrimitiveAsync<Guid>(SeraPrimitiveTypes.Guid);
                return SeraAny.MakePrimitive(v);
            }

            else if ((pri & DeserializerPrimitiveHint.Half) != 0)
            {
                var v = await deserializer.ReadPrimitiveAsync<float>(SeraPrimitiveTypes.Single);
                return SeraAny.MakePrimitive(v);
            }
            else if ((pri & DeserializerPrimitiveHint.UInt16) != 0)
            {
                var v = await deserializer.ReadPrimitiveAsync<ushort>(SeraPrimitiveTypes.UInt16);
                return SeraAny.MakePrimitive(v);
            }
            else if ((pri & DeserializerPrimitiveHint.Int16) != 0)
            {
                var v = await deserializer.ReadPrimitiveAsync<short>(SeraPrimitiveTypes.Int16);
                return SeraAny.MakePrimitive(v);
            }
            else if ((pri & DeserializerPrimitiveHint.Char) != 0)
            {
                var v = await deserializer.ReadPrimitiveAsync<char>(SeraPrimitiveTypes.Char);
                return SeraAny.MakePrimitive(v);
            }

            else if ((pri & DeserializerPrimitiveHint.Byte) != 0)
            {
                var v = await deserializer.ReadPrimitiveAsync<byte>(SeraPrimitiveTypes.Byte);
                return SeraAny.MakePrimitive(v);
            }
            else if ((pri & DeserializerPrimitiveHint.SByte) != 0)
            {
                var v = await deserializer.ReadPrimitiveAsync<sbyte>(SeraPrimitiveTypes.SByte);
                return SeraAny.MakePrimitive(v);
            }

            else if ((pri & DeserializerPrimitiveHint.Boolean) != 0)
            {
                var v = await deserializer.ReadPrimitiveAsync<bool>(SeraPrimitiveTypes.Boolean);
                return SeraAny.MakePrimitive(v);
            }

            else if ((pri & DeserializerPrimitiveHint.BigInteger) != 0)
            {
                var v = await deserializer.ReadPrimitiveAsync<BigInteger>(SeraPrimitiveTypes.BigInteger);
                return SeraAny.MakePrimitive(new SeraBoxed<BigInteger>(v));
            }
            else if ((pri & DeserializerPrimitiveHint.Complex) != 0)
            {
                var v = await deserializer.ReadPrimitiveAsync<Complex>(SeraPrimitiveTypes.Complex);
                return SeraAny.MakePrimitive(v);
            }

            else if ((pri & DeserializerPrimitiveHint.UIntPtr) != 0)
            {
                var v = await deserializer.ReadPrimitiveAsync<UIntPtr>(SeraPrimitiveTypes.UIntPtr);
                return SeraAny.MakePrimitive(v);
            }
            else if ((pri & DeserializerPrimitiveHint.IntPtr) != 0)
            {
                var v = await deserializer.ReadPrimitiveAsync<IntPtr>(SeraPrimitiveTypes.IntPtr);
                return SeraAny.MakePrimitive(v);
            }

            else if ((pri & DeserializerPrimitiveHint.DateTimeOffset) != 0)
            {
                var v = await deserializer.ReadPrimitiveAsync<DateTimeOffset>(SeraPrimitiveTypes.DateTimeOffset);
                return SeraAny.MakePrimitive(v);
            }
            else if ((pri & DeserializerPrimitiveHint.DateTime) != 0)
            {
                var v = await deserializer.ReadPrimitiveAsync<DateTime>(SeraPrimitiveTypes.DateTime);
                return SeraAny.MakePrimitive(v);
            }
            else if ((pri & DeserializerPrimitiveHint.DateOnly) != 0)
            {
                var v = await deserializer.ReadPrimitiveAsync<DateOnly>(SeraPrimitiveTypes.DateOnly);
                return SeraAny.MakePrimitive(v);
            }

            else if ((pri & DeserializerPrimitiveHint.Guid) != 0)
            {
                var v = await deserializer.ReadPrimitiveAsync<Guid>(SeraPrimitiveTypes.Guid);
                return SeraAny.MakePrimitive(v);
            }

            else if ((pri & DeserializerPrimitiveHint.Range) != 0)
            {
                var v = await deserializer.ReadPrimitiveAsync<Guid>(SeraPrimitiveTypes.Guid);
                return SeraAny.MakePrimitive(v);
            }
            else if ((pri & DeserializerPrimitiveHint.Index) != 0)
            {
                var v = await deserializer.ReadPrimitiveAsync<Guid>(SeraPrimitiveTypes.Guid);
                return SeraAny.MakePrimitive(v);
            }

            else
            {
                await deserializer.SkipAsync();
                return SeraAny.MakePrimitive();
            }
        }
        else if ((peek & DeserializerHint.String) != 0)
        {
            var v = await deserializer.ReadStringAsync();
            return SeraAny.MakeString(v);
        }
        else if ((peek & DeserializerHint.Bytes) != 0)
        {
            var v = await deserializer.ReadBytesAsync();
            return SeraAny.MakeBytes(v);
        }
        else if ((peek & DeserializerHint.Enum) != 0)
        {
            var (name, a) = await deserializer.ReadEnumAsync<SeraAny, AnyImpl>(Instance);
            return SeraAny.MakeEnum(new SeraAnyEnum(name, a));
        }
        else if ((peek & DeserializerHint.Struct) != 0)
        {
            var len = await deserializer.ReadStructStartAsync(null, null);
            var name = await deserializer.ViewStructNameAsync();
            Dictionary<string, SeraAny> fields;
            if (len.HasValue)
            {
                fields = new((int)len.Value);
                for (nuint i = 0; i < len.Value; i++)
                {
                    var key = await deserializer.PeekStructNextKeyAsync() ?? $"{i}";
                    var a = await deserializer.ReadStructFieldAsync<SeraAny, AnyImpl>(Instance);
                    fields[key] = a;
                }
            }
            else
            {
                fields = new();
                for (nuint i = 0; await deserializer.PeekStructHasNextAsync(); i++)
                {
                    var key = await deserializer.PeekStructNextKeyAsync() ?? $"{i}";
                    var a = await deserializer.ReadStructFieldAsync<SeraAny, AnyImpl>(Instance);
                    fields[key] = a;
                }
            }
            await deserializer.ReadStructEndAsync();
            return SeraAny.MakeStruct(new SeraAnyStruct(fields) { StructName = name });
        }
        else if ((peek & DeserializerHint.Variant) != 0)
        {
            await deserializer.ReadVariantStartAsync(null);
            var union_name = await deserializer.ViewUnionNameAsync();
            var name = await deserializer.ViewVariantNameAsync();
            var tag = await deserializer.ViewVariantTagAsync();
            SeraAny a;
            if (await deserializer.PeekVariantIsUnitAsync())
            {
                await deserializer.ReadVariantValueUnitAsync();
                a = SeraAny.MakeUnit();
            }
            else
            {
                a = await deserializer.ReadVariantValueAsync<SeraAny, AnyImpl>(Instance);
            }
            await deserializer.ReadVariantEndAsync();
            return SeraAny.MakeVariant(new SeraAnyVariant(name, tag ?? 0, a) { UnionName = union_name });
        }
        else if ((peek & DeserializerHint.Tuple) != 0)
        {
            var len = await deserializer.ReadTupleStartAsync(null);
            SeraAny[] arr;
            if (len.HasValue)
            {
                arr = new SeraAny[len.Value];
                for (nuint i = 0; i < len.Value; i++)
                {
                    arr[i] = await deserializer.ReadTupleElementAsync<SeraAny, AnyImpl>(Instance);
                }
            }
            else
            {
                var list = new List<SeraAny>();
                while (await deserializer.PeekTupleHasNextAsync())
                {
                    var a = await deserializer.ReadTupleElementAsync<SeraAny, AnyImpl>(Instance);
                    list.Add(a);
                }
                arr = list.ToArray();
            }
            await deserializer.ReadTupleEndAsync();
            return SeraAny.MakeTuple(arr);
        }
        else if ((peek & DeserializerHint.Seq) != 0)
        {
            var len = await deserializer.ReadSeqStartAsync(null);
            List<SeraAny> list;
            if (len.HasValue)
            {
                list = new((int)len.Value);
                for (nuint i = 0; i < len.Value; i++)
                {
                    var a = await deserializer.ReadSeqElementAsync<SeraAny, AnyImpl>(Instance);
                    list.Add(a);
                }
            }
            else
            {
                list = new();
                while (await deserializer.PeekTupleHasNextAsync())
                {
                    var a = await deserializer.ReadSeqElementAsync<SeraAny, AnyImpl>(Instance);
                    list.Add(a);
                }
            }
            await deserializer.ReadSeqEndAsync();
            return SeraAny.MakeSeq(list);
        }
        else if ((peek & DeserializerHint.Map) != 0)
        {
            var len = await deserializer.ReadMapStartAsync(null);
            Dictionary<SeraAny, SeraAny> map;
            if (len.HasValue)
            {
                map = new((int)len.Value);
                for (nuint i = 0; i < len.Value; i++)
                {
                    var (k, v) =
                        await deserializer.ReadMapEntryAsync<SeraAny, SeraAny, AnyImpl, AnyImpl>(Instance, Instance);
                    map[k] = v;
                }
            }
            else
            {
                map = new();
                while (await deserializer.PeekMapHasNextAsync())
                {
                    var (k, v) =
                        await deserializer.ReadMapEntryAsync<SeraAny, SeraAny, AnyImpl, AnyImpl>(Instance, Instance);
                    map[k] = v;
                }
            }
            await deserializer.ReadMapEndAsync();
            return SeraAny.MakeMap(map);
        }
        else if ((peek & DeserializerHint.NullableNotNull) != 0)
        {
            var a = await deserializer.ReadNullableNotNullAsync<SeraAny, AnyImpl>(Instance);
            return SeraAny.MakeNullableNotNull(new SeraBoxed<SeraAny>(a));
        }
        else
        {
            await deserializer.SkipAsync();
            return SeraAny.MakeUnknown();
        }
    }
}
