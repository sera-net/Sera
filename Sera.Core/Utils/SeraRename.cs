using System;
using System.Buffers;
using System.Text;

namespace Sera.Utils;

public enum SeraWordKind : byte
{
    Word,
    Number,
    Split,
    Space,
}

public readonly record struct SeraWord(ReadOnlyMemory<char> Word, SeraWordKind Kind);

public static class SeraRename
{
    public static SeraRenameMode Or(this SeraRenameMode? self, SeraRenameMode? other) =>
        self switch
        {
            null => other ?? SeraRenameMode.None,
            SeraRenameMode.None => other ?? SeraRenameMode.None,
            { } r => r
        };

    public static string Rename(string name, SeraRenameMode mode) => mode switch
    {
        SeraRenameMode.None or SeraRenameMode.Dont => name,
        SeraRenameMode.PascalCase => ToPascalCase(name),
        SeraRenameMode.camelCase => ToCamelCase(name),
        SeraRenameMode.lowercase => ToLowerCase(name),
        SeraRenameMode.UPPERCASE => ToUpperCase(name),
        SeraRenameMode.snake_case => ToSnakeCase(name),
        SeraRenameMode.UPPER_SNAKE_CASE => ToUpperSnakeCase(name),
        SeraRenameMode.kebab_case => ToKebabCase(name),
        SeraRenameMode.UPPER_KEBAB_CASE => ToUpperKebabCase(name),
        _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
    };

    public static SplitWordEnumerable SplitWord(string name) => new(name);

    public readonly struct SplitWordEnumerable(string name)
    {
        public SplitWordEnumerator GetEnumerator() => new(name);
    }

    public struct SplitWordEnumerator(string name)
    {
        private static readonly Rune Underscore = new('_');
        private static readonly Rune MinusSign = new('-');

        private static bool IsUnderscore(Rune r) => r == Underscore;
        private static bool IsMinusSign(Rune r) => r == MinusSign;

        private static bool IsOther(Rune r)
            => Rune.IsLetter(r) ||
               (!Rune.IsDigit(r) && !Rune.IsWhiteSpace(r) && !IsUnderscore(r) &&
                !IsMinusSign(r));

        public SeraWord Current { get; private set; }

        private ReadOnlyMemory<char> last = name.AsMemory();
        private Rune pre_rune = default;

        public bool MoveNext()
        {
            if (last.IsEmpty) return false;
            var off = 0;
            for (;;)
            {
                var span = last.Span[off..];
                if (span.IsEmpty)
                {
                    Current = new SeraWord(last,
                        Rune.IsDigit(pre_rune) ? SeraWordKind.Number :
                        Rune.IsWhiteSpace(pre_rune) ? SeraWordKind.Space :
                        IsUnderscore(pre_rune) || IsMinusSign(pre_rune) ? SeraWordKind.Split :
                        SeraWordKind.Word);
                    last = default;
                    return true;
                }
                if (Rune.DecodeFromUtf16(span, out var rune, out var count) != OperationStatus.Done)
                    throw new ArgumentException("Illegal string", nameof(name));
                if (pre_rune != default)
                {
                    if (Rune.IsLetter(pre_rune) && Rune.IsLetter(rune))
                    {
                        if (Rune.IsLower(pre_rune) && Rune.IsUpper(rune))
                        {
                            Yield(off);
                            return true;
                        }
                    }
                    if (IsOther(pre_rune) && !IsOther(rune))
                    {
                        Yield(off);
                        return true;
                    }
                    if (Rune.IsDigit(pre_rune) && !Rune.IsDigit(rune))
                    {
                        Yield(off, SeraWordKind.Number);
                        return true;
                    }
                    if (Rune.IsWhiteSpace(pre_rune) && !Rune.IsWhiteSpace(rune))
                    {
                        Yield(off, SeraWordKind.Space);
                        return true;
                    }
                    if (IsUnderscore(pre_rune) && !IsUnderscore(rune))
                    {
                        Yield(off, SeraWordKind.Split);
                        return true;
                    }
                    if (IsMinusSign(pre_rune) && !IsMinusSign(rune))
                    {
                        Yield(off, SeraWordKind.Split);
                        return true;
                    }
                }

                off += count;
                pre_rune = rune;
            }
        }

        private void Yield(int off, SeraWordKind kind = SeraWordKind.Word)
        {
            Current = new SeraWord(last[..off], kind);
            last = last[off..];
            pre_rune = default;
        }
    }

    public static string ToPascalCase(string name)
    {
        if (string.IsNullOrEmpty(name)) return name;
        var sb = new StringBuilder();
        var first_word = true;
        SeraWord? pre_word = null;
        Span<char> chars = stackalloc char[2];
        foreach (var word in SplitWord(name))
        {
            switch (word.Kind)
            {
                case SeraWordKind.Word:
                    var first = true;
                    foreach (var rune in word.Word.Runes())
                    {
                        Rune r;
                        if (first)
                        {
                            r = Rune.ToUpperInvariant(rune);
                            first = false;
                        }
                        else
                        {
                            r = Rune.ToLowerInvariant(rune);
                        }
                        var len = r.EncodeToUtf16(chars);
                        sb.Append(chars[..len]);
                    }
                    break;
                case SeraWordKind.Number:
                    sb.Append(word.Word);
                    break;
                case SeraWordKind.Split:
                    if (first_word || word.Word.Length > 1) sb.Append(word.Word);
                    break;
                case SeraWordKind.Space:
                default:
                    break;
            }

            if (first_word) first_word = false;
            pre_word = word;
        }

        if (pre_word != null)
        {
            var word = pre_word.Value;
            if (word.Kind is SeraWordKind.Split)
            {
                sb.Append(word.Word);
            }
        }

        return sb.ToString();
    }

    public static string ToCamelCase(string name)
    {
        if (string.IsNullOrEmpty(name)) return name;
        var sb = new StringBuilder();
        var first_word = true;
        var first_word_word = true;
        SeraWord? pre_word = null;
        Span<char> chars = stackalloc char[2];
        foreach (var word in SplitWord(name))
        {
            switch (word.Kind)
            {
                case SeraWordKind.Word:
                    var first = true;
                    foreach (var rune in word.Word.Runes())
                    {
                        Rune r;
                        if (first && !first_word && !first_word_word)
                        {
                            r = Rune.ToUpperInvariant(rune);
                            first = false;
                        }
                        else
                        {
                            r = Rune.ToLowerInvariant(rune);
                        }
                        var len = r.EncodeToUtf16(chars);
                        sb.Append(chars[..len]);
                    }
                    first_word_word = false;
                    break;
                case SeraWordKind.Number:
                    sb.Append(word.Word);
                    first_word_word = false;
                    break;
                case SeraWordKind.Split:
                    if (first_word || word.Word.Length > 1) sb.Append(word.Word);
                    break;
                case SeraWordKind.Space:
                default:
                    break;
            }

            if (first_word) first_word = false;
            pre_word = word;
        }

        if (pre_word != null)
        {
            var word = pre_word.Value;
            if (word.Kind is SeraWordKind.Split)
            {
                sb.Append(word.Word);
            }
        }

        return sb.ToString();
    }

    private static string ToFlatLike(string name, bool upper = false)
    {
        if (string.IsNullOrEmpty(name)) return name;
        var sb = new StringBuilder();
        var first_word = true;
        SeraWord? pre_word = null;
        Span<char> chars = stackalloc char[2];
        foreach (var word in SplitWord(name))
        {
            switch (word.Kind)
            {
                case SeraWordKind.Word:
                    foreach (var rune in word.Word.Runes())
                    {
                        var r = upper ? Rune.ToUpperInvariant(rune) : Rune.ToLowerInvariant(rune);
                        var len = r.EncodeToUtf16(chars);
                        sb.Append(chars[..len]);
                    }
                    break;
                case SeraWordKind.Number:
                    sb.Append(word.Word);
                    break;
                case SeraWordKind.Split:
                    if (first_word || word.Word.Length > 1) sb.Append(word.Word);
                    break;
                case SeraWordKind.Space:
                default:
                    break;
            }

            if (first_word) first_word = false;
            pre_word = word;
        }

        if (pre_word != null)
        {
            var word = pre_word.Value;
            if (word.Kind is SeraWordKind.Split)
            {
                sb.Append(word.Word);
            }
        }

        return sb.ToString();
    }

    public static string ToLowerCase(string name)
        => ToFlatLike(name);

    public static string ToUpperCase(string name)
        => ToFlatLike(name, upper: true);

    private static string ToSnakeCaseLike(string name, bool upper = false, char split = '_')
    {
        if (string.IsNullOrEmpty(name)) return name;
        var sb = new StringBuilder();
        var first_word = true;
        SeraWord? pre_word = null;
        Span<char> chars = stackalloc char[2];
        foreach (var word in SplitWord(name))
        {
            switch (word.Kind)
            {
                case SeraWordKind.Word:
                    if (!first_word && pre_word is { Kind: SeraWordKind.Word })
                    {
                        sb.Append(split);
                    }
                    var first = true;
                    foreach (var rune in word.Word.Runes())
                    {
                        if (first)
                        {
                            first = false;
                            if (!first_word && pre_word is { Kind: SeraWordKind.Number })
                            {
                                if (Rune.IsUpper(rune))
                                {
                                    sb.Append(split);
                                }
                            }
                        }
                        var r = upper ? Rune.ToUpperInvariant(rune) : Rune.ToLowerInvariant(rune);
                        var len = r.EncodeToUtf16(chars);
                        sb.Append(chars[..len]);
                    }
                    break;
                case SeraWordKind.Number:
                    sb.Append(word.Word);
                    break;
                case SeraWordKind.Split:
                    foreach (var _ in word.Word)
                    {
                        sb.Append(split);
                    }
                    break;
                case SeraWordKind.Space:
                    sb.Append(split);
                    break;
                default:
                    break;
            }

            if (first_word) first_word = false;
            pre_word = word;
        }

        return sb.ToString();
    }

    public static string ToSnakeCase(string name)
        => ToSnakeCaseLike(name);

    public static string ToUpperSnakeCase(string name)
        => ToSnakeCaseLike(name, upper: true);

    public static string ToKebabCase(string name)
        => ToSnakeCaseLike(name, split: '-');

    public static string ToUpperKebabCase(string name)
        => ToSnakeCaseLike(name, upper: true, split: '-');
}
