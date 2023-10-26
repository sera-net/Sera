using System;
using System.Buffers;
using System.IO;
using System.Runtime.InteropServices;

namespace Sera.Utils;

public class ReadOnlySequenceStream<T>(ReadOnlySequence<T> seq) : Stream
    where T : struct
{
    public override bool CanRead => true;
    public override bool CanSeek => false;
    public override bool CanWrite => false;

    public override long Length => seq.Length;

    public override long Position
    {
        get => _position;
        set
        {
            _seq_position = seq.GetPosition(value);
            _position = value;
        }
    }

    private long _position;
    private SequencePosition _seq_position = seq.Start;

    public override void Flush() { }

    public override long Seek(long offset, SeekOrigin origin)
        => throw new NotSupportedException();

    public override void SetLength(long value)
        => throw new NotSupportedException();

    public override void Write(byte[] buffer, int offset, int count)
        => throw new NotSupportedException();

    public override int Read(byte[] buffer, int offset, int count)
        => Read(buffer.AsSpan(offset, count));

    public override int ReadByte()
    {
        Span<byte> buffer = stackalloc byte[1];
        return Read(buffer) != 0 ? buffer[0] : -1;
    }

    public override int Read(Span<byte> buffer)
    {
        var _pos = _seq_position;
        if (!seq.TryGet(ref _pos, out var mem) || mem.Length == 0) return 0;
        var bytes = MemoryMarshal.AsBytes(mem.Span);
        if (bytes.Length <= buffer.Length)
        {
            bytes.CopyTo(buffer);
            Position += bytes.Length;
            return bytes.Length;
        }
        else
        {
            bytes[..buffer.Length].CopyTo(buffer);
            Position += buffer.Length;
            return buffer.Length;
        }
    }
}
