using System;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using Sera.Json.Utils;

namespace Sera.Json.De;

public readonly struct AstJsonReader : IJsonReaderState<AstJsonReader>
{
    #region Create

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static JsonReader<AstJsonReader> Create(SeraJsonOptions options, JsonAst source)
        => new(options, new(source));

    #endregion

    #region Seek

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public AstJsonReader Save() => this;

    #endregion

    #region Fields

    internal readonly SubState state;
    internal readonly ImmutableStack<SubState> stack = ImmutableStack<SubState>.Empty;

    internal readonly struct SubState
    {
        private readonly JsonAst ast;
        private readonly JsonAstObjectNode? node;
        private readonly int index;
        private readonly byte state;
        private readonly Tags tag;

        public static readonly SubState None = MakeNone();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private SubState(Tags tag, JsonAst ast, byte state, int index, JsonAstObjectNode? node)
        {
            this.tag = tag;
            this.ast = ast;
            this.state = state;
            this.index = index;
            this.node = node;
        }

        public enum Tags : byte
        {
            None,
            Ast,
            Array,
            Object,
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SubState MakeNone()
            => new(Tags.None, default, 0, 0, null);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SubState MakeAst(JsonAst value)
            => new(Tags.Ast, value, 0, 0, null);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SubState MakeArray(SubStateArray value)
            => new(Tags.Array, value.ast, (byte)value.state, value.index, null);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SubState MakeObject(SubStateObject value)
            => new(Tags.Object, value.ast, (byte)value.state, value.index, value.node);

        public Tags Tag
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => tag;
        }

        public JsonAst Ast
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ast;
        }
        public SubStateArray Array
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new(ast, ast.Array, index, (ArrState)state);
        }
        public SubStateObject Object
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new(ast, ast.Object, index, node, (ObjState)state);
        }
    }

    internal readonly record struct SubStateArray(JsonAst ast, JsonAstArray arr, int index, ArrState state);

    internal enum ArrState : byte
    {
        Comma,
        Value,
    }

    internal readonly record struct SubStateObject(
        JsonAst ast,
        JsonAstObject obj,
        int index,
        JsonAstObjectNode? node,
        ObjState state);

    internal enum ObjState : byte
    {
        Comma,
        Key,
        Colon,
        Value,
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private ImmutableStack<SubState> PushState(SubState state) => stack.Push(state);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private ImmutableStack<SubState> PopState(out SubState state) => stack.Pop(out state);

    #endregion

    #region Ctor

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public AstJsonReader(JsonAst source)
    {
        state = SubState.MakeAst(source);
        this = MoveNext();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private AstJsonReader(SubState state, ImmutableStack<SubState> stack, bool currentHas,
        JsonToken currentToken, SourcePos sourcePos)
    {
        this.state = state;
        this.stack = stack;
        CurrentHas = currentHas;
        CurrentToken = currentToken;
        SourcePos = sourcePos;
    }

    #endregion

    #region Iter

    #region Getters

    public bool CurrentHas
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get;
    }
    public JsonToken CurrentToken
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get;
    }
    public SourcePos SourcePos
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get;
    }

    #endregion

    #region MoveNext

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public AstJsonReader MoveNext()
    {
        switch (state.Tag)
        {
            case SubState.Tags.None:
                return EofMoveNext();
            case SubState.Tags.Ast:
                return ValueMoveNext(SubState.None, state.Ast);
            case SubState.Tags.Array:
                return ArrayMoveNext(state.Array);
            case SubState.Tags.Object:
                return ObjectMoveNext(state.Object);
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private AstJsonReader EofMoveNext()
        => new(state, stack, false, CurrentToken, SourcePos);

    #region Value

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private AstJsonReader ValueMoveNext(SubState state, JsonAst ast)
    {
        switch (ast.Tag)
        {
            case JsonAst.Tags.Null:
                return ValueMoveNextFoundValue(state, ast.Null);
            case JsonAst.Tags.Bool:
                return ValueMoveNextFoundValue(state, ast.Bool);
            case JsonAst.Tags.Number:
                return ValueMoveNextFoundValue(state, ast.Number);
            case JsonAst.Tags.String:
                return ValueMoveNextFoundValue(state, ast.String);
            case JsonAst.Tags.Array:
                return ValueMoveNextFoundArray(state, ast, ast.Array);
            case JsonAst.Tags.Object:
                return ValueMoveNextFoundObject(state, ast, ast.Object);
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private AstJsonReader ValueMoveNextFoundValue(SubState state, JsonToken token)
    {
        return new(state, stack, true, token, token.Pos);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private AstJsonReader ValueMoveNextFoundArray(SubState lastState, JsonAst ast, JsonAstArray arr)
    {
        var state = SubState.MakeArray(new(ast, arr, 0, ArrState.Value));
        var stack = PushState(lastState);
        var token = arr.ArrayStart;
        return new(state, stack, true, token, token.Pos);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private AstJsonReader ValueMoveNextFoundObject(SubState lastState, JsonAst ast, JsonAstObject obj)
    {
        var state = SubState.MakeObject(new(ast, obj, 0, obj.Map.First, ObjState.Value));
        var stack = PushState(lastState);
        var token = obj.ObjectStart;
        return new(state, stack, true, token, token.Pos);
    }

    #endregion

    #region Array

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private AstJsonReader ArrayMoveNext(SubStateArray ssa)
    {
        var arr = ssa.arr;
        var list = arr.List;
        switch (ssa.state)
        {
            case ArrState.Comma:
                return ArrayMoveNextFoundValue(ssa, list[ssa.index]);
            case ArrState.Value:
                if (ssa.index >= list.Count) return ArrayMoveNextFoundEnd(arr);
                var val = list[ssa.index];
                if (val.Comma.HasValue && ssa.index != 0)
                    return ArrayMoveNextFoundComma(ssa, val);
                return ArrayMoveNextFoundValue(ssa, val);
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private AstJsonReader ArrayMoveNextFoundValue(SubStateArray ssa, JsonAstListValue val)
    {
        var state = SubState.MakeArray(ssa with { index = ssa.index + 1, state = ArrState.Value });
        return ValueMoveNext(state, val.Value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private AstJsonReader ArrayMoveNextFoundComma(SubStateArray ssa, JsonAstListValue val)
    {
        var state = SubState.MakeArray(ssa with { state = ArrState.Comma });
        var token = val.Comma!.Value;
        return new(state, stack, true, token, token.Pos);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private AstJsonReader ArrayMoveNextFoundEnd(JsonAstArray arr)
    {
        var token = arr.ArrayEnd;
        var stack = PopState(out var state);
        return new(state, stack, true, token, token.Pos);
    }

    #endregion

    #region Object

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private AstJsonReader ObjectMoveNext(SubStateObject sso)
    {
        var obj = sso.obj;
        var node = sso.node;
        switch (sso.state)
        {
            case ObjState.Comma:
                return ObjectMoveNextFoundKey(sso, node!.Value.Value);
            case ObjState.Key:
                return ObjectMoveNextFoundColon(sso, node!.Value.Value);
            case ObjState.Colon:
                return ObjectMoveNextFoundValue(sso, node!.Value, node.Value.Value);
            case ObjState.Value:
                if (node is null) return ObjectMoveNextFoundEnd(obj);
                var val = node.Value.Value;
                if (val.Comma.HasValue && sso.index != 0)
                    return ObjectMoveNextFoundComma(sso, val);
                return ObjectMoveNextFoundKey(sso, node.Value.Value);
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private AstJsonReader ObjectMoveNextFoundComma(SubStateObject sso, JsonAstObjectKeyValue val)
    {
        var state = SubState.MakeObject(sso with { state = ObjState.Comma });
        var token = val.Comma!.Value;
        return new(state, stack, true, token, token.Pos);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private AstJsonReader ObjectMoveNextFoundKey(SubStateObject sso, JsonAstObjectKeyValue val)
    {
        var state = SubState.MakeObject(sso with { state = ObjState.Key });
        var token = val.Key;
        return new(state, stack, true, token, token.Pos);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private AstJsonReader ObjectMoveNextFoundColon(SubStateObject sso, JsonAstObjectKeyValue val)
    {
        var state = SubState.MakeObject(sso with { state = ObjState.Colon });
        var token = val.Colon;
        return new(state, stack, true, token, token.Pos);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private AstJsonReader ObjectMoveNextFoundValue(SubStateObject sso, JsonAstObjectNode node,
        JsonAstObjectKeyValue val)
    {
        var state = SubState.MakeObject(sso with { state = ObjState.Value, index = sso.index + 1, node = node.Next });
        return ValueMoveNext(state, val.Value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private AstJsonReader ObjectMoveNextFoundEnd(JsonAstObject obj)
    {
        var token = obj.ObjectEnd;
        var stack = PopState(out var state);
        return new(state, stack, true, token, token.Pos);
    }

    #endregion

    #endregion

    #endregion
}
