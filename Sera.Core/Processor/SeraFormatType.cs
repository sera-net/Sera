using System;

namespace Sera.Core;

/// <summary>
/// Indicates the format type, <see cref="SeraFormatType.Text"/> and <see cref="SeraFormatType.Binary"/> are mutually exclusive
/// </summary>
[Flags]
public enum SeraFormatType
{
    HumanReadableText = 1 << 0,
    HumanUnreadableText = 1 << 1,
    Binary = 1 << 2,

    Text = HumanReadableText | HumanUnreadableText,
}
