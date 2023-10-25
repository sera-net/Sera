using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Sera.Core.De;
using Sera.Core.Ser;

namespace Sera.Core.Impls;

public class AnyImpl :
    ISerialize<SeraAny>,
    IDeserialize<SeraAny>,
    ISeqSerializerReceiver<List<SeraAny>>,
    IMapSerializerReceiver<Dictionary<SeraAny, SeraAny>>,
    IStructSerializerReceiver<SeraAnyStruct>,
    IAnyDeserializerVisitor<SeraAny>
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
}
