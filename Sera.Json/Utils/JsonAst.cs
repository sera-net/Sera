using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
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

public record JsonAstObjectKeyValue(string StringKey, JsonToken Key, JsonAst Value, JsonToken Colon, JsonToken? Comma);

public readonly struct JsonAstObjectNode
{
    internal readonly JsonAstObject.ALinkedMultiMap.Node node;

    internal JsonAstObjectNode(JsonAstObject.ALinkedMultiMap.Node node)
    {
        this.node = node;
    }

    #region Getters

    public JsonAstObjectKeyValue Value
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => node.Value;
    }

    public JsonAstObjectNode? Next
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => node.Next;
    }

    public JsonAstObjectNode? Previous
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => node.Previous;
    }

    public JsonAstObjectNode First
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => node.First;
    }

    public JsonAstObjectNode Last
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => node.Last;
    }

    public JsonAstObjectNode? SubNext
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => node.SubNext;
    }

    public JsonAstObjectNode? SubPrevious
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => node.SubPrevious;
    }

    public JsonAstObjectNode SubFirst
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => node.SubFirst;
    }

    public JsonAstObjectNode SubLast
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => node.SubLast;
    }

    #endregion
}

public record JsonAstObject(
    JsonAstObject.ALinkedMultiMap Map,
    JsonToken ObjectStart,
    JsonToken ObjectEnd)
{
    /// <summary>
    /// A multi-valued hash map sorted in insertion order
    /// </summary>
    public abstract class ALinkedMultiMap
    {
        protected internal sealed class Node
        {
            public JsonAstObjectKeyValue Value { get; }
            public ALinkedMultiMap Map { get; }
            internal LinkedListNode<Node> MainNode { get; set; } = null!;
            internal LinkedListNode<Node> SubNode { get; set; } = null!;

            internal Node(JsonAstObjectKeyValue Value, ALinkedMultiMap map)
            {
                this.Value = Value;
                Map = map;
            }

            #region Getters

            public JsonAstObjectNode? Next
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => Map.Next(this);
            }

            public JsonAstObjectNode? Previous
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => Map.Previous(this);
            }

            public JsonAstObjectNode First
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => Map.First!.Value;
            }

            public JsonAstObjectNode Last
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => Map.Last!.Value;
            }

            public JsonAstObjectNode? SubNext
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => Map.SubNext(this);
            }

            public JsonAstObjectNode? SubPrevious
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => Map.SubPrevious(this);
            }

            public JsonAstObjectNode SubFirst
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => Map.SubFirst(this);
            }

            public JsonAstObjectNode SubLast
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => Map.SubLast(this);
            }

            #endregion
        }

        public abstract int Count
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract bool TryGetValue(string key, out JsonAstObjectNode values);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract ALinkedMultiMap? TryRemove(JsonAstObjectNode value);

        #region Getters

        public abstract JsonAstObjectNode? First
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }

        public abstract JsonAstObjectNode? Last
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected internal abstract JsonAstObjectNode SubFirst(Node node);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected internal abstract JsonAstObjectNode SubLast(Node node);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected internal abstract JsonAstObjectNode? Next(Node node);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected internal abstract JsonAstObjectNode? SubNext(Node node);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected internal abstract JsonAstObjectNode? Previous(Node node);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected internal abstract JsonAstObjectNode? SubPrevious(Node node);

        #endregion
    }

    /// <summary>
    /// A multi-valued hash map sorted in insertion order
    /// </summary>
    public sealed class LinkedMultiMap : ALinkedMultiMap
    {
        // No concurrent requirement, ConcurrentDictionary is used because its internal implementation is more suitable
        private readonly ConcurrentDictionary<string, LinkedList<Node>> Dictionary = new();
        private readonly LinkedList<Node> MainList = new();

        public override int Count
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => MainList.Count;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(JsonAstObjectKeyValue value)
        {
            var node = new Node(value, this);
            Dictionary.AddOrUpdate(value.StringKey, static (_, node) =>
            {
                var list = new LinkedList<Node>();
                node.SubNode = list.AddLast(node);
                return list;
            }, static (_, list, node) =>
            {
                node.SubNode = list.AddLast(node);
                return list;
            }, node);
            node.MainNode = MainList.AddLast(node);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool TryGetValue(string key, out JsonAstObjectNode values)
        {
            if (Dictionary.TryGetValue(key, out var nodes))
            {
                values = new(nodes.First?.Value!);
                return true;
            }
            else
            {
                values = default;
                return false;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override ALinkedMultiMap? TryRemove(JsonAstObjectNode node)
        {
            if (!Dictionary.TryGetValue(node.Value.StringKey, out var list)) return null;
            if (!list.Contains(node.node)) return null;
            if (Count == 1) return EmptyLinkedMultiMap.Instance;
            return new RemovedLinkedMultiMap(this, node.Value);
        }

        #region Getters

        public override JsonAstObjectNode? First
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => MainList.First?.Value is { } a ? new(a) : null;
        }

        public override JsonAstObjectNode? Last
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => MainList.Last?.Value is { } a ? new(a) : null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected internal override JsonAstObjectNode SubFirst(Node node) =>
            new(node.SubNode.List?.First?.Value!);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected internal override JsonAstObjectNode SubLast(Node node) =>
            new(node.SubNode.List?.Last?.Value!);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected internal override JsonAstObjectNode? Next(Node node) =>
            node.MainNode.Next?.Value is { } a ? new(a) : null;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected internal override JsonAstObjectNode? SubNext(Node node) =>
            node.SubNode.Next?.Value is { } a ? new(a) : null;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected internal override JsonAstObjectNode? Previous(Node node) =>
            node.MainNode.Previous?.Value is { } a ? new(a) : null;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected internal override JsonAstObjectNode? SubPrevious(Node node) =>
            node.SubNode.Previous?.Value is { } a ? new(a) : null;

        #endregion
    }

    public sealed class EmptyLinkedMultiMap : ALinkedMultiMap
    {
        public static EmptyLinkedMultiMap Instance { get; } = new();

        public override int Count
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool TryGetValue(string key, out JsonAstObjectNode values)
        {
            values = default;
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override ALinkedMultiMap? TryRemove(JsonAstObjectNode node) => null;

        #region Getters

        public override JsonAstObjectNode? First
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => null;
        }
        public override JsonAstObjectNode? Last
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected internal override JsonAstObjectNode SubFirst(Node node) => default;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected internal override JsonAstObjectNode SubLast(Node node) => default;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected internal override JsonAstObjectNode? Next(Node node) => null;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected internal override JsonAstObjectNode? SubNext(Node node) => null;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected internal override JsonAstObjectNode? Previous(Node node) => null;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected internal override JsonAstObjectNode? SubPrevious(Node node) => null;

        #endregion
    }

    public sealed class RemovedLinkedMultiMap : ALinkedMultiMap
    {
        private readonly LinkedMultiMap BaseMap;
        private readonly JsonAstObjectKeyValue RemovedItem;

        public RemovedLinkedMultiMap(LinkedMultiMap BaseMap, JsonAstObjectKeyValue RemovedItem)
        {
            this.BaseMap = BaseMap;
            this.RemovedItem = RemovedItem;
            if (BaseMap.Count < 2)
                throw new ArgumentOutOfRangeException(nameof(BaseMap), "BaseMap must have at least 2 elements");
        }

        public override int Count
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => BaseMap.Count - 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool TryGetValue(string key, out JsonAstObjectNode values)
        {
            if (!BaseMap.TryGetValue(key, out values)) return false;
            values = values.SubFirst;
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override ALinkedMultiMap? TryRemove(JsonAstObjectNode value)
        {
            if (value.Value == RemovedItem) return null;
            if (!BaseMap.TryGetValue(value.Value.StringKey, out var values)) return null;
            if (!values.node.SubNode.List!.Contains(value.node)) return null;
            return new MultiRemovedLinkedMultiMap(BaseMap, ImmutableHashSet.Create([RemovedItem, value.Value]));
        }

        #region Getters

        public override JsonAstObjectNode? First
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                var first = BaseMap.First;
                if (first != null && first.Value.Value == RemovedItem) return first.Value.Next;
                return first;
            }
        }
        public override JsonAstObjectNode? Last
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                var last = BaseMap.Last;
                if (last != null && last.Value.Value == RemovedItem) return last.Value.Previous;
                return last;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected internal override JsonAstObjectNode SubFirst(Node node)
        {
            if (node.Value == RemovedItem) throw new ArgumentOutOfRangeException(nameof(node));
            var result = BaseMap.SubFirst(node);
            if (result.Value == RemovedItem) return result.SubNext!.Value;
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected internal override JsonAstObjectNode SubLast(Node node)
        {
            if (node.Value == RemovedItem) throw new ArgumentOutOfRangeException(nameof(node));
            var result = BaseMap.SubLast(node);
            if (result.Value == RemovedItem) return result.SubPrevious!.Value;
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected internal override JsonAstObjectNode? Next(Node node)
        {
            if (node.Value == RemovedItem) throw new ArgumentOutOfRangeException(nameof(node));
            var result = BaseMap.Next(node);
            if (result.HasValue && result.Value.Value == RemovedItem) return result.Value.Next!.Value;
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected internal override JsonAstObjectNode? SubNext(Node node)
        {
            if (node.Value == RemovedItem) throw new ArgumentOutOfRangeException(nameof(node));
            var result = BaseMap.SubNext(node);
            if (result.HasValue && result.Value.Value == RemovedItem) return result.Value.SubNext!.Value;
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected internal override JsonAstObjectNode? Previous(Node node)
        {
            if (node.Value == RemovedItem) throw new ArgumentOutOfRangeException(nameof(node));
            var result = BaseMap.Previous(node);
            if (result.HasValue && result.Value.Value == RemovedItem) return result.Value.Previous!.Value;
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected internal override JsonAstObjectNode? SubPrevious(Node node)
        {
            if (node.Value == RemovedItem) throw new ArgumentOutOfRangeException(nameof(node));
            var result = BaseMap.SubPrevious(node);
            if (result.HasValue && result.Value.Value == RemovedItem) return result.Value.SubPrevious!.Value;
            return result;
        }

        #endregion
    }

    public sealed class MultiRemovedLinkedMultiMap : ALinkedMultiMap
    {
        private readonly LinkedMultiMap BaseMap;
        private readonly ImmutableHashSet<JsonAstObjectKeyValue> RemovedItems;

        public MultiRemovedLinkedMultiMap(LinkedMultiMap BaseMap, ImmutableHashSet<JsonAstObjectKeyValue> RemovedItems)
        {
            this.BaseMap = BaseMap;
            this.RemovedItems = RemovedItems;
            var count = RemovedItems.Count;
            if (BaseMap.Count <= count)
                throw new ArgumentOutOfRangeException(nameof(BaseMap),
                    $"BaseMap must have at least {count + 1} elements");
        }

        public override int Count
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => BaseMap.Count - RemovedItems.Count;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool TryGetValue(string key, out JsonAstObjectNode values)
        {
            if (!BaseMap.TryGetValue(key, out values)) return false;
            values = values.SubFirst;
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override ALinkedMultiMap? TryRemove(JsonAstObjectNode value)
        {
            if (RemovedItems.Contains(value.Value)) return null;
            if (!BaseMap.TryGetValue(value.Value.StringKey, out var values)) return null;
            if (!values.node.SubNode.List!.Contains(value.node)) return null;
            return new MultiRemovedLinkedMultiMap(BaseMap, RemovedItems.Add(value.Value));
        }


        #region Getters

        public override JsonAstObjectNode? First
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                var first = BaseMap.First;
                while (first != null && RemovedItems.Contains(first.Value.Value)) first = first.Value.Next;
                return first;
            }
        }
        public override JsonAstObjectNode? Last
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                var last = BaseMap.Last;
                while (last != null && RemovedItems.Contains(last.Value.Value)) last = last.Value.Previous;
                return last;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected internal override JsonAstObjectNode SubFirst(Node node)
        {
            if (RemovedItems.Contains(node.Value)) throw new ArgumentOutOfRangeException(nameof(node));
            var result = BaseMap.SubFirst(node);
            while (RemovedItems.Contains(result.Value)) result = result.SubNext!.Value;
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected internal override JsonAstObjectNode SubLast(Node node)
        {
            if (RemovedItems.Contains(node.Value)) throw new ArgumentOutOfRangeException(nameof(node));
            var result = BaseMap.SubLast(node);
            while (RemovedItems.Contains(result.Value)) result = result.SubPrevious!.Value;
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected internal override JsonAstObjectNode? Next(Node node)
        {
            if (RemovedItems.Contains(node.Value)) throw new ArgumentOutOfRangeException(nameof(node));
            var result = BaseMap.Next(node);
            while (result.HasValue && RemovedItems.Contains(result.Value.Value)) result = result.Value.Next!.Value;
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected internal override JsonAstObjectNode? SubNext(Node node)
        {
            if (RemovedItems.Contains(node.Value)) throw new ArgumentOutOfRangeException(nameof(node));
            var result = BaseMap.SubNext(node);
            while (result.HasValue && RemovedItems.Contains(result.Value.Value)) result = result.Value.SubNext!.Value;
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected internal override JsonAstObjectNode? Previous(Node node)
        {
            if (RemovedItems.Contains(node.Value)) throw new ArgumentOutOfRangeException(nameof(node));
            var result = BaseMap.Previous(node);
            while (result.HasValue && RemovedItems.Contains(result.Value.Value)) result = result.Value.Previous!.Value;
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected internal override JsonAstObjectNode? SubPrevious(Node node)
        {
            if (RemovedItems.Contains(node.Value)) throw new ArgumentOutOfRangeException(nameof(node));
            var result = BaseMap.SubPrevious(node);
            while (result.HasValue && RemovedItems.Contains(result.Value.Value))
                result = result.Value.SubPrevious!.Value;
            return result;
        }

        #endregion
    }
}
