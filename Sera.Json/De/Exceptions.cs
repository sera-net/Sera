using System;

namespace Sera.Json.De;

public class JsonParseException : Exception
{
    public SourcePos pos;

    public JsonParseException(SourcePos pos)
    {
        this.pos = pos;
    }

    public JsonParseException(string message, SourcePos pos) : base(message)
    {
        this.pos = pos;
    }

    public JsonParseException(string message, SourcePos pos, Exception inner) : base(message, inner)
    {
        this.pos = pos;
    }
}
