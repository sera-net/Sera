using System;
using System.Globalization;
using Sera.Core.Formats;

namespace Sera.Core.Formats
{

    public enum ToUpperOrLower
    {
        None,
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
        /// Any is not All, Any means to let the (de)serializer handle it
        /// </summary>
        Any,
        /// <summary>
        /// Format numbers in generic, Use "G"
        /// <code>123.456</code>
        /// </summary>
        Generic,
        /// <summary>
        /// Format numbers in decimal, Use "D" for integers and "N" for floating points
        /// <code>1234</code>
        /// </summary>
        Decimal,
        /// <summary>
        /// Format numbers in hexadecimal, Use "X", Integers only
        /// <code>FF</code>
        /// </summary>
        Hex,
        /// <summary>
        /// Format numbers in binary, Use "B", Integers only
        /// <code>101010</code>
        /// </summary>
        Binary,
        /// <summary>
        /// Format numbers in exponent, Use "E", Floating points only
        /// <code>1.052033E+003</code> 
        /// </summary>
        Exponent,
    }

    [Flags]
    public enum DateTimeFormatFlags
    {
        None,
        /// <summary>
        /// Format date/time as number
        /// </summary>
        DateAsNumber = 1 << 0,
        /// <summary>
        /// Convert <see cref="DateOnly"/> to <see cref="DateTime"/> before formatting
        /// </summary>
        DateOnlyToDateTime = 1 << 1,
        /// <summary>
        /// Convert <see cref="DateOnly"/> to <see cref="DateTimeOffset"/> before formatting
        /// </summary>
        DateOnlyToDateTimeOffset = 1 << 2,
        /// <summary>
        /// Convert <see cref="DateTime"/> to <see cref="DateTimeOffset"/> before formatting
        /// </summary>
        DateTimeToDateTimeOffset = 1 << 3,
        /// <summary>
        /// Convert <see cref="DateTimeOffset"/>'s timezone using <see cref="ISeraOptions.TimeZone"/>
        /// </summary>
        DateTimeOffsetUseTimeZone = 1 << 4,
    }

    public enum GuidTextFormat
    {
        /// <summary>
        /// Any is not All, Any means to let the (de)serializer handle it
        /// </summary>
        Any,
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
        /// Any is not All, Any means to let the (de)serializer handle it
        /// </summary>
        Any,
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

        public ToUpperOrLower ToUpperOrLower { get; set; } = ToUpperOrLower.None;

        public NumberTextFormat NumberTextFormat { get; set; } = NumberTextFormat.Any;
        public NumberStyles NumberStyles { get; set; } = NumberStyles.None;
        public string? CustomNumberTextFormat { get; set; }

        public bool ComplexAsString { get; set; }

        public DateTimeFormatFlags DateTimeFormat { get; set; } = DateTimeFormatFlags.None;

        public GuidTextFormat GuidTextFormat { get; set; } = GuidTextFormat.Any;
        public GuidBinaryFormat GuidBinaryFormat { get; set; } = GuidBinaryFormat.Any;
        public string? CustomGuidTextFormat { get; set; }

        public static SeraFormats FromAttr(SeraFormatsAttribute? attr) => attr == null
            ? Default
            : new()
            {
                BooleanAsNumber = attr.BooleanAsNumber,
                ToUpperOrLower = attr.ToUpperOrLower,
                NumberTextFormat = attr.NumberTextFormat,
                NumberStyles = attr.NumberStyles,
                CustomNumberTextFormat = attr.CustomNumberTextFormat,
                ComplexAsString = attr.ComplexAsString,
                DateTimeFormat = attr.DateTimeFormat,
                GuidTextFormat = attr.GuidTextFormat,
                GuidBinaryFormat = attr.GuidBinaryFormat,
                CustomGuidTextFormat = attr.CustomGuidTextFormat,
            };
    }

}
