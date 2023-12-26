using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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

public record JsonAstObjectKeyValue(string StringKey, JsonToken Key, JsonAst Value, JsonToken Colon, JsonToken? Comma)
{
    internal LinkedListNode<JsonAstObjectKeyValue> MainNode { get; set; } = null!;
    internal LinkedListNode<JsonAstObjectKeyValue> SubNode { get; set; } = null!;
}

public record JsonAstObject(
    JsonAstObject.LinkedMultiMap Map,
    JsonToken ObjectStart,
    JsonToken ObjectEnd)
{
    /// <summary>
    /// A multi-valued hash map sorted in insertion order
    /// </summary>
    public sealed class LinkedMultiMap
    {
        // No concurrent requirement, ConcurrentDictionary is used because its internal implementation is more suitable
        private readonly ConcurrentDictionary<string, LinkedList<JsonAstObjectKeyValue>> Dictionary = new();
        private readonly LinkedList<JsonAstObjectKeyValue> MainList = new();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(JsonAstObjectKeyValue value)
        {
            Dictionary.AddOrUpdate(value.StringKey, static (_, value) =>
            {
                var list = new LinkedList<JsonAstObjectKeyValue>();
                value.SubNode = list.AddLast(value);
                return list;
            }, static (_, list, value) =>
            {
                value.SubNode = list.AddLast(value);
                return list;
            }, value);
            value.MainNode = MainList.AddLast(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryGetValue(string key, [MaybeNullWhen(false)] out LinkedList<JsonAstObjectKeyValue> values)
            => Dictionary.TryGetValue(key, out values);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Remove(JsonAstObjectKeyValue value)
        {
            if (!Dictionary.TryGetValue(value.StringKey, out var subList)) return false;
            subList.Remove(value.SubNode);
            MainList.Remove(value.MainNode);
            if (subList.Count == 0)
                Dictionary.Remove(value.StringKey, out _);
            return true;
        }
        
        public LinkedListNode<JsonAstObjectKeyValue>? First
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => MainList.First;
        }

        public LinkedListNode<JsonAstObjectKeyValue>? Last
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => MainList.Last;
        }
    }
}
