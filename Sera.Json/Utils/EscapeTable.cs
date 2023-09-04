namespace Sera.Json.Utils;

internal static class EscapeTable
{
    private const char BB = 'b'; // \x08
    private const char TT = 't'; // \x09
    private const char NN = 'n'; // \x0A
    private const char FF = 'f'; // \x0C
    private const char RR = 'r'; // \x0D
    private const char QU = '"'; // \x22
    private const char SB = '/'; // \x2F
    private const char BS = '\\'; // \x5C
    private const char __ = '\0';

    private static readonly char[] Table =
    {
        //   1   2   3   4   5   6   7   8   9   A   B   C   D   E   F
        __, __, __, __, __, __, __, __, BB, TT, NN, __, FF, RR, __, __, // 0
        __, __, __, __, __, __, __, __, __, __, __, __, __, __, __, __, // 1
        __, __, QU, __, __, __, __, __, __, __, __, __, __, __, __, SB, // 2
        __, __, __, __, __, __, __, __, __, __, __, __, __, __, __, __, // 3
        __, __, __, __, __, __, __, __, __, __, __, __, __, __, __, __, // 4
        __, __, __, __, __, __, __, __, __, __, __, __, BS, __, __, __, // 5
    };

    public static bool TryGet(char c, out char o)
    {
        if (c < Table.Length && Table[c] != '\0')
        {
            o = Table[c];
            return true;
        }
        else
        {
            o = default;
            return false;
        }
    }
}
