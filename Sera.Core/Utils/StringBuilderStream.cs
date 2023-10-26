using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Sera.Utils;

public class StringBuilderStream(StringBuilder Builder) : Stream
{
    public override bool CanRead => false;
    public override bool CanSeek => false;
    public override bool CanWrite => true;
    public override long Length => Builder.Length;
    public override long Position { get; set; }

    public override void Flush() { }

    public override int Read(byte[] buffer, int offset, int count)
        => throw new NotSupportedException();

    public override long Seek(long offset, SeekOrigin origin)
        => throw new NotSupportedException();

    public override void SetLength(long value)
        => throw new NotSupportedException();

    public override void Write(byte[] buffer, int offset, int count)
        => Write(buffer.AsSpan(offset, count));

    public override void WriteByte(byte value)
        => Write(new ReadOnlySpan<byte>(ref value));

    public override void Write(ReadOnlySpan<byte> buffer)
    {
        if (Position == Length)
        {
            Builder.Append(MemoryMarshal.Cast<byte, char>(buffer));
        }
        else
        {
            Builder.Insert((int)Position, MemoryMarshal.Cast<byte, char>(buffer));
        }
        Position += buffer.Length;
    }
}
