using System;
using System.Runtime.CompilerServices;

namespace Sera.Core.Impls.Ser;

public readonly struct SeraPrimitiveImpl(SeraFormats? formats = null) : ISeraVision<SeraPrimitive>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, SeraPrimitive value) where V : ASeraVisitor<R>
        => value.Tag switch
        {
            SeraPrimitiveTypes.Boolean => new PrimitiveImpl(formats).Accept<R, V>(visitor, value.Boolean),
            SeraPrimitiveTypes.SByte => new PrimitiveImpl(formats).Accept<R, V>(visitor, value.SByte),
            SeraPrimitiveTypes.Byte => new PrimitiveImpl(formats).Accept<R, V>(visitor, value.Byte),
            SeraPrimitiveTypes.Int16 => new PrimitiveImpl(formats).Accept<R, V>(visitor, value.Int16),
            SeraPrimitiveTypes.UInt16 => new PrimitiveImpl(formats).Accept<R, V>(visitor, value.UInt16),
            SeraPrimitiveTypes.Int32 => new PrimitiveImpl(formats).Accept<R, V>(visitor, value.Int32),
            SeraPrimitiveTypes.UInt32 => new PrimitiveImpl(formats).Accept<R, V>(visitor, value.UInt32),
            SeraPrimitiveTypes.Int64 => new PrimitiveImpl(formats).Accept<R, V>(visitor, value.Int64),
            SeraPrimitiveTypes.UInt64 => new PrimitiveImpl(formats).Accept<R, V>(visitor, value.UInt64),
            SeraPrimitiveTypes.Int128 => new PrimitiveImpl(formats).Accept<R, V>(visitor, value.Int128),
            SeraPrimitiveTypes.UInt128 => new PrimitiveImpl(formats).Accept<R, V>(visitor, value.UInt128),
            SeraPrimitiveTypes.IntPtr => new PrimitiveImpl(formats).Accept<R, V>(visitor, value.IntPtr),
            SeraPrimitiveTypes.UIntPtr => new PrimitiveImpl(formats).Accept<R, V>(visitor, value.UIntPtr),
            SeraPrimitiveTypes.Half => new PrimitiveImpl(formats).Accept<R, V>(visitor, value.Half),
            SeraPrimitiveTypes.Single => new PrimitiveImpl(formats).Accept<R, V>(visitor, value.Single),
            SeraPrimitiveTypes.Double => new PrimitiveImpl(formats).Accept<R, V>(visitor, value.Double),
            SeraPrimitiveTypes.Decimal => new PrimitiveImpl(formats).Accept<R, V>(visitor, value.Decimal),
            SeraPrimitiveTypes.NFloat => new PrimitiveImpl(formats).Accept<R, V>(visitor, value.NFloat),
            SeraPrimitiveTypes.BigInteger => new PrimitiveImpl(formats).Accept<R, V>(visitor, value.BigInteger.Value),
            SeraPrimitiveTypes.Complex => new PrimitiveImpl(formats).Accept<R, V>(visitor, value.Complex),
            SeraPrimitiveTypes.TimeSpan => new PrimitiveImpl(formats).Accept<R, V>(visitor, value.TimeSpan),
            SeraPrimitiveTypes.DateOnly => new PrimitiveImpl(formats).Accept<R, V>(visitor, value.DateOnly),
            SeraPrimitiveTypes.TimeOnly => new PrimitiveImpl(formats).Accept<R, V>(visitor, value.TimeOnly),
            SeraPrimitiveTypes.DateTime => new PrimitiveImpl(formats).Accept<R, V>(visitor, value.DateTime),
            SeraPrimitiveTypes.DateTimeOffset => new PrimitiveImpl(formats).Accept<R, V>(visitor, value.DateTimeOffset),
            SeraPrimitiveTypes.Guid => new PrimitiveImpl(formats).Accept<R, V>(visitor, value.Guid),
            SeraPrimitiveTypes.Range => new PrimitiveImpl(formats).Accept<R, V>(visitor, value.Range),
            SeraPrimitiveTypes.Index => new PrimitiveImpl(formats).Accept<R, V>(visitor, value.Index),
            SeraPrimitiveTypes.Char => new PrimitiveImpl(formats).Accept<R, V>(visitor, value.Char),
            SeraPrimitiveTypes.Rune => new PrimitiveImpl(formats).Accept<R, V>(visitor, value.Rune),
            SeraPrimitiveTypes.Uri => new PrimitiveImpl(formats).Accept<R, V>(visitor, value.Uri),
            SeraPrimitiveTypes.Version => new PrimitiveImpl(formats).Accept<R, V>(visitor, value.Version),
            _ => throw new ArgumentOutOfRangeException()
        };
}
