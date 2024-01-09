using System;
using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using Sera.Core;
using Sera.Json.De;

namespace Sera.Json.Utils;

internal static class JsonParseUtils
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static N ParseNumber<N>(this JsonToken token, NumberStyles style) where N : INumberBase<N>
    {
        var span = token.AsSpan();
        try
        {
            return style is NumberStyles.None ? N.Parse(span, null) : N.Parse(span, style, null);
        }
        catch (Exception e)
        {
            throw new JsonParseException($"Illegal number format at {token.Pos}", token.Pos, e);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static N ParseDateOrTime<N, C>(this JsonToken token, string TypeName, C FromTicks)
        where N : ISpanParsable<N> where C : ISeraMapper<long, N>
    {
        if (token.Kind is JsonTokenKind.String)
        {
            var span = token.AsSpan();
            Exception? ex;
            try
            {
                return N.Parse(span, null);
            }
            catch (Exception e)
            {
                ex = e;
            }
            if (long.TryParse(span, NumberStyles.Any, null, out var ticks))
            {
                return FromTicks.Map(ticks);
            }
            else throw new JsonParseException($"Illegal {TypeName} format at {token.Pos}", token.Pos, ex);
        }
        else if (token.Kind is JsonTokenKind.Number)
        {
            var span = token.AsSpan();
            if (long.TryParse(span, NumberStyles.Any, null, out var ticks))
            {
                return FromTicks.Map(ticks);
            }
        }
        throw new JsonParseException($"Expected {TypeName} but found {token.Kind} at {token.Pos}", token.Pos);
    }
}
