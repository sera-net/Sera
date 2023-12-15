using System;
using System.Runtime.CompilerServices;
using Sera.Utils;

namespace Sera.Core.Impls.De;

public readonly struct DateOrTimeFromNumberMapper :
    ISeraMapper<long, TimeSpan>,
    ISeraMapper<long, DateOnly>, ISeraMapper<int, DateOnly>,
    ISeraMapper<long, TimeOnly>,
    ISeraMapper<long, DateTime>, ISeraMapper<long, DateTimeOffset>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TimeSpan Map(long value, InType<TimeSpan>? u = null)
        => new(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public DateOnly Map(long value, InType<DateOnly>? u = null)
        => DateOnly.FromDayNumber((int)value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public DateOnly Map(int value, InType<DateOnly>? u = null)
        => DateOnly.FromDayNumber(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TimeOnly Map(long value, InType<TimeOnly>? u = null)
        => new(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public DateTime Map(long value, InType<DateTime>? u = null)
        => new(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public DateTimeOffset Map(long value, InType<DateTimeOffset>? u = null)
        => new(value, TimeSpan.Zero);
}

public readonly struct DateOrTimeFromNumberWithOffsetMapper(TimeSpan offset) :
    ISeraMapper<long, DateTime>, ISeraMapper<long, DateTimeOffset>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public DateTime Map(long value, InType<DateTime>? u = null)
        => new DateTimeOffset(value, offset).LocalDateTime;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public DateTimeOffset Map(long value, InType<DateTimeOffset>? u = null)
        => new(value, offset);
}
