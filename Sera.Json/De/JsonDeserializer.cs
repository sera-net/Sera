using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Sera.Core;
using Sera.Utils;

namespace Sera.Json.De;

public class JsonDeserializer(SeraJsonOptions options, AJsonReader reader) : SeraBase<ISeraColion<object?>>
{
    public override string FormatName => "json";
    public override string FormatMIME => "application/json";
    public override SeraFormatType FormatType => SeraFormatType.HumanReadableText;
    public override ISeraOptions Options => options;
    internal readonly AJsonReader reader = reader;

    public override IRuntimeProvider<ISeraColion<object?>> RuntimeProvider =>
        RuntimeProviderOverride ?? throw new NotImplementedException("todo"); // EmptyDeRuntimeProvider.Instance

    public IRuntimeProvider<ISeraColion<object?>>? RuntimeProviderOverride { get; set; }

    public T Collect<C, T>(C colion) where C : ISeraColion<T>
    {
        var c = new JsonDeserializer<T>(this);
        return colion.Collect<T, JsonDeserializer<T>>(ref c);
    }
}

public readonly struct JsonDeserializer<T>(JsonDeserializer impl) : ISeraColctor<T, T>
{
    #region Ability

    public string FormatName => impl.FormatName;
    public string FormatMIME => impl.FormatMIME;
    public SeraFormatType FormatType => impl.FormatType;
    public ISeraOptions Options => impl.Options;
    public ISeraColion<object?> RuntimeImpl => impl.RuntimeImpl;
    private AJsonReader reader => impl.reader;

    #endregion

    #region Primitive

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T CPrimitive<M>(M mapper, Type<bool> t, SeraFormats? formats = null)
        where M : ISeraMapper<bool, T>
    {
        var token = reader.CurrentToken();
        if (token.Kind is JsonTokenKind.True)
        {
            return mapper.Map(true);
        }
        else if (token.Kind is JsonTokenKind.False)
        {
            return mapper.Map(false);
        }
        else throw new NotImplementedException(); // todo
    }

    #endregion

    #region String

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T CString<M>(M mapper, Type<string> t) where M : ISeraMapper<string, T>
        => mapper.Map(CString().ToString());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T CString<M>(M mapper, Type<char[]> t) where M : ISeraMapper<char[], T>
        => mapper.Map(CString().ToArray());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T CString<M>(M mapper, Type<Memory<char>> t) where M : ISeraMapper<Memory<char>, T>
        => mapper.Map(CString().ToArray());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T CString<M>(M mapper, Type<ReadOnlyMemory<char>> t) where M : ISeraMapper<ReadOnlyMemory<char>, T>
        => mapper.Map(CString());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T CString<M>(M mapper, Type<byte[]> t, Encoding encoding) where M : ISeraMapper<byte[], T>
        => mapper.Map(CString(encoding));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T CString<M>(M mapper, Type<Memory<byte>> t, Encoding encoding) where M : ISeraMapper<Memory<byte>, T>
        => mapper.Map(CString(encoding));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T CString<M>(M mapper, Type<ReadOnlyMemory<byte>> t, Encoding encoding)
        where M : ISeraMapper<ReadOnlyMemory<byte>, T>
        => mapper.Map(CString(encoding));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private ReadOnlyMemory<char> CString()
    {
        var token = reader.CurrentToken();
        if (token.Kind is not JsonTokenKind.String) throw new NotImplementedException(); // todo throw;
        return token.AsString;
    }

    private byte[] CString(Encoding encoding)
    {
        var str = CString();
        if (Equals(encoding, Encoding.Unicode))
            return MemoryMarshal.AsBytes(str.Span).ToArray();
        var len = encoding.GetMaxByteCount(str.Length);
        var arr = ArrayPool<byte>.Shared.Rent(len);
        try
        {
            var n = encoding.GetBytes(str.Span, arr.AsSpan());
            return arr.AsMemory(0, n).ToArray();
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(arr);
        }
    }

    #endregion

    #region Array

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T CArray<C, M, I>(C colion, M mapper, Type<I[]> t, Type<I> i)
        where C : ISeraColion<I> where M : ISeraMapper<I[], T>
        => mapper.Map(CArray<C, I>(colion).ToArray());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T CArray<C, M, I>(C colion, M mapper, Type<Memory<I>> t, Type<I> i)
        where C : ISeraColion<I> where M : ISeraMapper<Memory<I>, T>
        => mapper.Map(CArray<C, I>(colion).AsMemory);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T CArray<C, M, I>(C colion, M mapper, Type<ReadOnlyMemory<I>> t, Type<I> i)
        where C : ISeraColion<I> where M : ISeraMapper<ReadOnlyMemory<I>, T>
        => mapper.Map(CArray<C, I>(colion).AsMemory);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T CArray<C, M, I>(C colion, M mapper, Type<ReadOnlySequence<I>> t, Type<I> i)
        where C : ISeraColion<I> where M : ISeraMapper<ReadOnlySequence<I>, T>
        => mapper.Map(CArraySequence<C, I>(colion));

    private Vec<I> CArray<C, I>(C colion) where C : ISeraColion<I>
    {
        reader.ReadArrayStart();
        var vec = new Vec<I>();
        var first = true;
        for (; reader.Has(); reader.MoveNext())
        {
            var token = reader.CurrentToken();
            if (token.Kind is JsonTokenKind.ArrayEnd) break;
            if (first) first = false;
            else if (token.Kind is not JsonTokenKind.Comma) throw new NotImplementedException(); // todo error
            var c = new JsonDeserializer<I>(impl);
            var i = colion.Collect<I, JsonDeserializer<I>>(ref c);
            vec.Add(i);
        }
        reader.ReadArrayEnd();
        return vec;
    }

    #region ArraySequence

    private const int seq_size_limit = 1073741824;

    private int VecBytes<I>(Vec<I> vec) => Unsafe.SizeOf<T>() * vec.Count;

    private ReadOnlySequence<I> CArraySequence<C, I>(C colion) where C : ISeraColion<I>
    {
        reader.ReadArrayStart();
        SequenceSegment<I>? first_segment = null;
        SequenceSegment<I>? last_segment = null;
        long index = 0;
        var vec = new Vec<I>();
        var first = true;
        for (; reader.Has(); reader.MoveNext())
        {
            var token = reader.CurrentToken();
            if (token.Kind is JsonTokenKind.ArrayEnd) break;
            if (first) first = false;
            else if (token.Kind is not JsonTokenKind.Comma) throw new NotImplementedException(); // todo error
            var c = new JsonDeserializer<I>(impl);
            var i = colion.Collect<I, JsonDeserializer<I>>(ref c);
            vec.Add(i);
            if (VecBytes(vec) >= seq_size_limit)
            {
                var new_segment = new SequenceSegment<I>();
                new_segment.SetMemory(vec.AsMemory);
                new_segment.SetRunningIndex(index);
                if (first_segment == null)
                {
                    last_segment = first_segment = new_segment;
                }
                else
                {
                    last_segment!.SetNext(new_segment);
                    last_segment = new_segment;
                }
                index += vec.Count;
                vec = new();
            }
        }
        reader.ReadArrayEnd();
        if (first_segment != null)
        {
            var new_segment = new SequenceSegment<I>();
            new_segment.SetMemory(vec.AsMemory);
            new_segment.SetRunningIndex(index);
            last_segment!.SetNext(new_segment);
            return new(first_segment, 0, new_segment, vec.Count - 1);
        }
        return new(vec.AsMemory);
    }

    private sealed class SequenceSegment<I> : ReadOnlySequenceSegment<I>
    {
        public void SetMemory(ReadOnlyMemory<I> m) => Memory = m;
        public void SetNext(ReadOnlySequenceSegment<I>? n) => Next = n;
        public void SetRunningIndex(long i) => RunningIndex = i;
    }

    #endregion

    #endregion

    #region Unit

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T CUnit<N>(N ctor) where N : ISeraCtor<T>
    {
        reader.ReadNull();
        return ctor.Ctor();
    }

    #endregion

    #region Option

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T COption<C>(C colion) where C : IOptionSeraColion<T>
    {
        var token = reader.CurrentToken();
        if (token.Kind is JsonTokenKind.Null) return colion.CtorNone();
        var colctor = new SomeSeraColctor(impl);
        return colion.CollectSome<T, SomeSeraColctor>(ref colctor);
    }

    private readonly struct SomeSeraColctor(JsonDeserializer impl) : ISomeSeraColctor<T, T>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T CSome<C, M, U>(C colion, M mapper, Type<U> u) where C : ISeraColion<U> where M : ISeraMapper<U, T>
        {
            var colctor = new JsonDeserializer<U>(impl);
            var r = colion.Collect<U, JsonDeserializer<U>>(ref colctor);
            return mapper.Map(r);
        }
    }

    #endregion

    #region Tuple

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T CTuple<C, B, M>(C colion, M mapper, Type<B> b)
        where C : ITupleSeraColion<B> where M : ISeraMapper<B, T>
    {
        reader.ReadArrayStart();
        var size = colion.Size;
        var colctor = new TupleSeraColctor<B>(colion.Builder(), impl);
        var first = true;
        for (var i = 0; i < size; i++)
        {
            if (first) first = false;
            else reader.ReadComma();
            var err = colion.CollectItem<bool, TupleSeraColctor<B>>(ref colctor, i);
            if (err) throw new SerializeException($"Unable to write item {i} of tuple {typeof(T)}");
        }
        reader.ReadArrayEnd();
        return mapper.Map(colctor.builder);
    }

    private struct TupleSeraColctor<B>(B builder, JsonDeserializer impl) : ITupleSeraColctor<B, bool>
    {
        public B builder = builder;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool CItem<C, E, I>(C colion, E effector, Type<I> i)
            where C : ISeraColion<I> where E : ISeraEffector<B, I>
        {
            var c = new JsonDeserializer<I>(impl);
            var r = colion.Collect<I, JsonDeserializer<I>>(ref c);
            effector.Effect(ref builder, r);
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool CNone() => false;
    }

    #endregion

    #region Seq

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T CSeq<C, B, M>(C colion, M mapper, Type<B> b) where C : ISeqSeraColion<B> where M : ISeraMapper<B, T>
    {
        reader.ReadArrayStart();
        var colctor = new SeqSeraColctor<B>(colion.Builder(null), impl);
        var first = true;
        for (; reader.Has(); reader.MoveNext())
        {
            var token = reader.CurrentToken();
            if (token.Kind is JsonTokenKind.ArrayEnd) break;
            if (first) first = false;
            else if (token.Kind is not JsonTokenKind.Comma) throw new NotImplementedException(); // todo error
            colion.CollectItem<Unit, SeqSeraColctor<B>>(ref colctor);
        }
        reader.ReadArrayEnd();
        return mapper.Map(colctor.builder);
    }

    private struct SeqSeraColctor<B>(B builder, JsonDeserializer impl) : ISeqSeraColctor<B, Unit>
    {
        public B builder = builder;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Unit CItem<C, E, I>(C colion, E effector, Type<I> i)
            where C : ISeraColion<I> where E : ISeraEffector<B, I>
        {
            var c = new JsonDeserializer<I>(impl);
            var r = colion.Collect<I, JsonDeserializer<I>>(ref c);
            effector.Effect(ref builder, r);
            return default;
        }
    }

    #endregion
}
