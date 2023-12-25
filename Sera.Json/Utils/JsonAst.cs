using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Sera.Json.De;
using Sera.TaggedUnion;

namespace Sera.Json.Utils;

/// <summary>
/// Temporary Json Ast represents
/// <para><br/><b>Warning</b>: No escape safety, moving out of the current call stack may result in wild pointers</para>
/// </summary>
[Union]
public readonly partial struct JsonAst
{
    [UnionTemplate]
    private interface Template
    {
        JsonToken Null();
        JsonToken Bool();
        JsonToken Number();
        JsonToken String();
        JsonAstArray Array();
        JsonAstObject Object();
    }

    public SourcePos Pos
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Tag switch
        {
            Tags.Null => Null.Pos,
            Tags.Bool => Bool.Pos,
            Tags.Number => Number.Pos,
            Tags.String => String.Pos,
            Tags.Array => Array.ArrayStart.Pos,
            Tags.Object => Object.ObjectStart.Pos,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}

public record struct JsonAstListValue(JsonAst Value, JsonToken? Comma);

public record JsonAstArray(
    List<JsonAstListValue> List,
    JsonToken ArrayStart,
    JsonToken ArrayEnd);

public record JsonAstObjectKeyValue(JsonToken Key, JsonAst Value, JsonToken Colon, JsonToken? Comma);

public record JsonAstObject(
    Dictionary<string, JsonAstObjectKeyValue> Dictionary,
    List<JsonAstObjectKeyValue> List,
    JsonToken ObjectStart,
    JsonToken ObjectEnd);
