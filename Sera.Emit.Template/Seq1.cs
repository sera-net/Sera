using System.Collections;
using Sera.Core.Ser;

namespace Sera.Emit.Template;

public class Seq1 : IEnumerable<int>
{
    public IEnumerator<int> GetEnumerator()
    {
        throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public struct Seq2 : IEnumerable<int>
{
    public IEnumerator<int> GetEnumerator()
    {
        throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public struct Seq3 : IEnumerable<int>
{
    public Enumerator GetEnumerator() => new();

    IEnumerator<int> IEnumerable<int>.GetEnumerator() => GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public struct Enumerator : IEnumerator<int>
    {
        public void Dispose() { }

        public bool MoveNext() => throw new NotImplementedException();

        public void Reset() => throw new NotImplementedException();

        public int Current => throw new NotImplementedException();
        object IEnumerator.Current => Current;
    }
}

public struct Seq1Impl<ST>(ST s) : ISeqSerializerReceiver<Seq1> where ST : ISerialize<int>
{
    public void Receive<S>(Seq1 value, S serializer) where S : ISeqSerializer
    {
        foreach (var v in value)
        {
            serializer.WriteElement(v, s);
        }
    }
}

public struct Seq2Impl<ST>(ST s) : ISeqSerializerReceiver<Seq2> where ST : ISerialize<int>
{
    public void Receive<S>(Seq2 value, S serializer) where S : ISeqSerializer
    {
        foreach (var v in value)
        {
            serializer.WriteElement(v, s);
        }
    }
}

public struct SeqImpl<T, E, ST>(ST s) : ISeqSerializerReceiver<E>
    where E : IEnumerable<T>
    where ST : ISerialize<T>
{
    public void Receive<S>(E value, S serializer) where S : ISeqSerializer
    {
        foreach (var v in value)
        {
            serializer.WriteElement(v, s);
        }
    }
}

public struct SeqSpanImpl<T, ST>(ST s)
    where ST : ISerialize<T>
{
    public void Receive<S>(Span<T> value, S serializer) where S : ISeqSerializer
    {
        foreach (var v in value)
        {
            serializer.WriteElement(v, s);
        }
    }
}

public struct Seq3Impl<ST>(ST s) : ISeqSerializerReceiver<Seq3> where ST : ISerialize<int>
{
    public void Receive<S>(Seq3 value, S serializer) where S : ISeqSerializer
    {
        foreach (var v in value)
        {
            serializer.WriteElement(v, s);
        }
    }
}
