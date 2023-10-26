using System;

namespace Sera.Core;

public enum Formats : byte
{
    /// <summary>
    /// No format specified
    /// </summary>
    Any,

    /// <summary>
    /// Format <see cref="bool"/> as number
    /// </summary>
    BooleanAsNumber,

    /// <summary>
    /// Format in uppercase if possible
    /// </summary>
    TextToUpper,
    /// <summary>
    /// Format in lowercase if possible
    /// </summary>
    TextToLower,

    /// <summary>
    /// Format numbers in decimal, Use "N"
    /// <code>1234</code>
    /// </summary>
    NumberTextDecimal,
    /// <summary>
    /// Format numbers in hexadecimal, Use "X"
    /// <code>FF</code>
    /// </summary>
    NumberTextHex,
    /// <summary>
    /// Format numbers in binary, Use "B"
    /// <code>101010</code>
    /// </summary>
    NumberTextBinary,

    /// <summary>
    /// Format date/time as number
    /// </summary>
    DateAsNumber,
    /// <summary>
    /// Convert <see cref="DateOnly"/> to <see cref="DateTime"/> before formatting
    /// </summary>
    DateOnlyToDateTime,
    /// <summary>
    /// Convert <see cref="DateOnly"/> to <see cref="DateTimeOffset"/> before formatting
    /// </summary>
    DateOnlyToDateTimeOffset,
    /// <summary>
    /// Convert <see cref="DateTime"/> to <see cref="DateTimeOffset"/> before formatting
    /// </summary>
    DateTimeToDateTimeOffset,
    /// <summary>
    /// Convert <see cref="DateTimeOffset"/>'s timezone using <see cref="ISeraOptions.TimeZone"/>
    /// </summary>
    DateTimeOffsetUseTimeZone,

    /// <summary>
    /// Use "N" to format <see cref="Guid"/>
    /// <code>00000000000000000000000000000000</code>
    /// </summary>
    GuidTextShort,
    /// <summary>
    /// Use "D" to format <see cref="Guid"/>
    /// <code>00000000-0000-0000-0000-000000000000</code>
    /// </summary>
    GuidTextGuid,
    /// <summary>
    /// Use "B" to format <see cref="Guid"/>
    /// <code>{00000000-0000-0000-0000-000000000000}</code>
    /// </summary>
    GuidTextBraces,
    /// <summary>
    /// Use "P" to format <see cref="Guid"/>
    /// <code>(00000000-0000-0000-0000-000000000000)</code>
    /// </summary>
    GuidTextParentheses,
    /// <summary>
    /// Use "X" to format <see cref="Guid"/>
    /// <code>{0x00000000，0x0000，0x0000，{0x00，0x00，0x00，0x00，0x00，0x00，0x00，0x00}}</code>
    /// </summary>
    GuidTextHex,
    /// <summary>
    /// Use the UUID standard for <see cref="Guid"/> in binary serialization
    /// </summary>
    GuidBinaryUuid,
    /// <summary>
    /// Use the GUID standard for &lt;see cref="Guid"/&gt; in binary serialization
    /// </summary>
    GuidBinaryGuid,
}
