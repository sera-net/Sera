using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using Sera.Json.Utils;

namespace Sera.Json.De;

public sealed class AstJsonReader : AJsonReader
{
    private JsonAst? ast;

    #region Ctor

    public AstJsonReader(SeraJsonOptions options, JsonAst ast) : base(options)
    {
        this.ast = ast;
        MoveNext();
    }

    #endregion

    #region Seek

    public override bool CanSeek
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => true;
    }

    private Dictionary<long, SavePoint>? savePoints;
    private long saves;

    private struct SavePoint
    {
        public bool Has;
        public JsonToken CurrentToken;
        public SourcePos pos;
        public int cursor;
        public SubState subState;
        public ImmutableStack<SubState> subStates;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override long Save()
    {
        savePoints ??= new();
        var save = saves++;
        savePoints[save] = new()
        {
            Has = Has,
            CurrentToken = CurrentToken,
            pos = pos,
            cursor = cursor,
            subState = subState,
            subStates = subStates,
        };
        return save;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override void Load(long pos)
    {
        var savePoint = savePoints![pos];
        Has = savePoint.Has;
        CurrentToken = savePoint.CurrentToken;
        this.pos = savePoint.pos;
        cursor = savePoint.cursor;
        subState = savePoint.subState;
        subStates = savePoint.subStates;
        Has = true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override void UnSave(long pos)
    {
        savePoints!.Remove(pos);
    }

    #endregion

    #region Cursor

    public override ulong Cursor
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => (ulong)cursor;
    }
    private int cursor;

    private SubState subState;
    private ImmutableStack<SubState> subStates = ImmutableStack<SubState>.Empty;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void PushState(SubState newState)
    {
        subStates = subStates.Push(subState);
        subState = newState;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void PopState()
    {
        subStates = subStates.Pop(out subState);
    }

    private enum State
    {
        None,
        ArrayComma = 10,
        ArrayValue,
        ObjectComma = 20,
        ObjectKey,
        ObjectColon,
        ObjectValue,
    }

    private struct SubState()
    {
        public State state = State.None;
        public int index = 0;
        public JsonAstArray? arr;
        public JsonAstObject? obj;
        public JsonAstObjectNode? node;
    }

    #endregion

    #region MoveNext

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override bool MoveNextImpl() => subState.state switch
    {
        State.ArrayComma or State.ArrayValue => MoveNextArray(),
        State.ObjectComma or State.ObjectKey or State.ObjectColon or State.ObjectValue => MoveNextObject(),
        _ => MoveNextValue()
    };

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool MoveNextEnd()
    {
        pos = CurrentToken.Pos;
        cursor++;
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool MoveNextValue()
    {
        if (!this.ast.HasValue) return false;
        var ast = this.ast.Value;
        switch (ast.Tag)
        {
            case JsonAst.Tags.Null:
                CurrentToken = ast.Null;
                break;
            case JsonAst.Tags.Bool:
                CurrentToken = ast.Bool;
                break;
            case JsonAst.Tags.Number:
                CurrentToken = ast.Number;
                break;
            case JsonAst.Tags.String:
                CurrentToken = ast.String;
                break;
            case JsonAst.Tags.Array:
                PushState(new SubState { state = State.ArrayValue, arr = ast.Array });
                CurrentToken = ast.Array.ArrayStart;
                break;
            case JsonAst.Tags.Object:
                PushState(new SubState { state = State.ObjectValue, obj = ast.Object, node = ast.Object.Map.First });
                CurrentToken = ast.Object.ObjectStart;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        this.ast = null;
        return MoveNextEnd();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool MoveNextArray()
    {
        var arr = subState.arr!;
        var list = arr.List;
        JsonAstListValue val;
        switch (subState.state)
        {
            case State.ArrayComma:
            {
                val = list[subState.index];
                goto on_value;
            }
            case State.ArrayValue:
            {
                if (subState.index >= list.Count) goto on_end;
                val = list[subState.index];
                if (val.Comma.HasValue && subState.index != 0)
                {
                    subState.state = State.ArrayComma;
                    CurrentToken = val.Comma.Value;
                    return MoveNextEnd();
                }
                else goto on_value;
            }
        }
        throw new ArgumentOutOfRangeException();

        on_end:
        CurrentToken = arr.ArrayEnd;
        PopState();
        ast = null;
        return MoveNextEnd();

        on_value:
        subState.state = State.ArrayValue;
        ast = val.Value;
        subState.index++;
        return MoveNextValue();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool MoveNextObject()
    {
        var obj = subState.obj!;
        JsonAstObjectKeyValue val;
        var node = subState.node;
        switch (subState.state)
        {
            case State.ObjectComma:
            {
                val = node!.Value.Value;
                goto on_key;
            }
            case State.ObjectKey:
                val = node!.Value.Value;
                subState.state = State.ObjectColon;
                CurrentToken = val.Colon;
                return MoveNextEnd();
            case State.ObjectColon:
                val = node!.Value.Value;
                subState.state = State.ObjectValue;
                ast = val.Value;
                subState.node = node.Value.Next;
                subState.index++;
                return MoveNextValue();
            case State.ObjectValue:
            {
                if (node is null) goto on_end;
                val = node.Value.Value;
                if (val.Comma.HasValue && subState.index != 0)
                {
                    subState.state = State.ObjectComma;
                    CurrentToken = val.Comma.Value;
                    return MoveNextEnd();
                }
                else goto on_key;
            }
        }
        throw new ArgumentOutOfRangeException();

        on_end:
        CurrentToken = obj.ObjectEnd;
        PopState();
        ast = null;
        return MoveNextEnd();

        on_key:
        subState.state = State.ObjectKey;
        CurrentToken = val.Key;
        return MoveNextEnd();
    }

    #endregion
}
