using System;
using Sera.Core.Formats;

namespace Sera.Core.Formats
{

    public enum ToUpperOrLower
    {
        /// <summary>
        /// Format in uppercase if possible
        /// </summary>
        Upper,
        /// <summary>
        /// Format in lowercase if possible
        /// </summary>
        Lower,
    }

    public enum NumberTextFormat
    {
        /// <summary>
        /// Format numbers in decimal, Use "N"
        /// <code>1234</code>
        /// </summary>
        Decimal,
        /// <summary>
        /// Format numbers in hexadecimal, Use "X"
        /// <code>FF</code>
        /// </summary>
        Hex,
        /// <summary>
        /// Format numbers in binary, Use "B"
        /// <code>101010</code>
        /// </summary>
        Binary,
    }

    public record DateTimeFormat
    {
        public static DateTimeFormat Default { get; } = new();

        /// <summary>
        /// Format date/time as number
        /// </summary>
        public bool DateAsNumber { get; set; }
        /// <summary>
        /// Convert <see cref="DateOnly"/> to <see cref="DateTime"/> before formatting
        /// </summary>
        public bool DateOnlyToDateTime { get; set; }
        /// <summary>
        /// Convert <see cref="DateOnly"/> to <see cref="DateTimeOffset"/> before formatting
        /// </summary>
        public bool DateOnlyToDateTimeOffset { get; set; }
        /// <summary>
        /// Convert <see cref="DateTime"/> to <see cref="DateTimeOffset"/> before formatting
        /// </summary>
        public bool DateTimeToDateTimeOffset { get; set; }
        /// <summary>
        /// Convert <see cref="DateTimeOffset"/>'s timezone using <see cref="ISeraOptions.TimeZone"/>
        /// </summary>
        public bool DateTimeOffsetUseTimeZone { get; set; }
    }

    public enum GuidTextFormat
    {
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
    }

    public enum GuidBinaryFormat
    {
        /// <summary>
        /// Use the UUID standard for <see cref="Guid"/> in binary serialization
        /// </summary>
        GuidBinaryUuid,
        /// <summary>
        /// Use the GUID standard for &lt;see cref="Guid"/&gt; in binary serialization
        /// </summary>
        GuidBinaryGuid,
    }

}


namespace Sera.Core
{

    public record SeraFormats
    {
        public static SeraFormats Default { get; } = new();

        public bool BooleanAsNumber { get; set; }

        public ToUpperOrLower? ToUpperOrLower { get; set; }

        public NumberTextFormat? NumberTextFormat { get; set; }
        public string? CustomNumberTextFormat { get; set; }

        public DateTimeFormat? DateTimeFormat { get; set; }

        public GuidTextFormat? GuidTextFormat { get; set; }
        public GuidBinaryFormat? GuidBinaryFormat { get; set; }
        public string? CustomGuidTextFormat { get; set; }
    }

}
