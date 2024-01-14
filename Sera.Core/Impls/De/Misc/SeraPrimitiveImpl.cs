using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Sera.Utils;

namespace Sera.Core.Impls.De;

using Impl = PrimitiveImpl;
using Mapper = PrimitiveToSeraPrimitiveMapper;

public readonly struct SeraPrimitiveImpl(SeraFormats? formats = null)
    : ISeraColion<SeraPrimitive>, ISelectPrimitiveSeraColion<SeraPrimitive>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Collect<R, C>(ref C colctor, InType<SeraPrimitive>? t = null) where C : ISeraColctor<SeraPrimitive, R>
        => colctor.CSelectPrimitive(this, new IdentityMapper<SeraPrimitive>(), new Type<SeraPrimitive>());

    public ReadOnlyMemory<SeraPrimitiveTypes>? Priorities
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => null;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R SelectPrimitiveDetail<R, C>(ref C colctor, SeraPrimitiveTypes type, InType<SeraPrimitive>? t = null)
        where C : ISelectSeraColctor<SeraPrimitive, R>
        => type switch
        {
            SeraPrimitiveTypes.Boolean => colctor.CSome(new Impl(), new Mapper(), new Type<bool>()),
            SeraPrimitiveTypes.SByte => colctor.CSome(new Impl(), new Mapper(), new Type<sbyte>()),
            SeraPrimitiveTypes.Byte => colctor.CSome(new Impl(), new Mapper(), new Type<byte>()),
            SeraPrimitiveTypes.Int16 => colctor.CSome(new Impl(), new Mapper(), new Type<short>()),
            SeraPrimitiveTypes.UInt16 => colctor.CSome(new Impl(), new Mapper(), new Type<ushort>()),
            SeraPrimitiveTypes.Int32 => colctor.CSome(new Impl(), new Mapper(), new Type<int>()),
            SeraPrimitiveTypes.UInt32 => colctor.CSome(new Impl(), new Mapper(), new Type<uint>()),
            SeraPrimitiveTypes.Int64 => colctor.CSome(new Impl(), new Mapper(), new Type<long>()),
            SeraPrimitiveTypes.UInt64 => colctor.CSome(new Impl(), new Mapper(), new Type<ulong>()),
            SeraPrimitiveTypes.Int128 => colctor.CSome(new Impl(), new Mapper(), new Type<Int128>()),
            SeraPrimitiveTypes.UInt128 => colctor.CSome(new Impl(), new Mapper(), new Type<UInt128>()),
            SeraPrimitiveTypes.IntPtr => colctor.CSome(new Impl(), new Mapper(), new Type<nint>()),
            SeraPrimitiveTypes.UIntPtr => colctor.CSome(new Impl(), new Mapper(), new Type<nuint>()),
            SeraPrimitiveTypes.Half => colctor.CSome(new Impl(), new Mapper(), new Type<Half>()),
            SeraPrimitiveTypes.Single => colctor.CSome(new Impl(), new Mapper(), new Type<float>()),
            SeraPrimitiveTypes.Double => colctor.CSome(new Impl(), new Mapper(), new Type<double>()),
            SeraPrimitiveTypes.Decimal => colctor.CSome(new Impl(), new Mapper(), new Type<decimal>()),
            SeraPrimitiveTypes.NFloat => colctor.CSome(new Impl(), new Mapper(), new Type<NFloat>()),
            SeraPrimitiveTypes.BigInteger => colctor.CSome(new Impl(), new Mapper(), new Type<BigInteger>()),
            SeraPrimitiveTypes.Complex => colctor.CSome(new Impl(), new Mapper(), new Type<Complex>()),
            SeraPrimitiveTypes.TimeSpan => colctor.CSome(new Impl(), new Mapper(), new Type<TimeSpan>()),
            SeraPrimitiveTypes.DateOnly => colctor.CSome(new Impl(), new Mapper(), new Type<DateOnly>()),
            SeraPrimitiveTypes.TimeOnly => colctor.CSome(new Impl(), new Mapper(), new Type<TimeOnly>()),
            SeraPrimitiveTypes.DateTime => colctor.CSome(new Impl(), new Mapper(), new Type<DateTime>()),
            SeraPrimitiveTypes.DateTimeOffset => colctor.CSome(new Impl(), new Mapper(), new Type<DateTimeOffset>()),
            SeraPrimitiveTypes.Guid => colctor.CSome(new Impl(), new Mapper(), new Type<Guid>()),
            SeraPrimitiveTypes.Range => colctor.CSome(new Impl(), new Mapper(), new Type<Range>()),
            SeraPrimitiveTypes.Index => colctor.CSome(new Impl(), new Mapper(), new Type<Index>()),
            SeraPrimitiveTypes.Char => colctor.CSome(new Impl(), new Mapper(), new Type<char>()),
            SeraPrimitiveTypes.Rune => colctor.CSome(new Impl(), new Mapper(), new Type<Rune>()),
            SeraPrimitiveTypes.Uri => colctor.CSome(new Impl(), new Mapper(), new Type<Uri>()),
            SeraPrimitiveTypes.Version => colctor.CSome(new Impl(), new Mapper(), new Type<Version>()),
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
}
