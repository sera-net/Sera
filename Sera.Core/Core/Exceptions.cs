using System;

namespace Sera.Core;

public class SerializeException : Exception
{
    public SerializeException() { }
    public SerializeException(string message) : base(message) { }
    public SerializeException(string message, Exception inner) : base(message, inner) { }
}

public class DeserializeException : Exception
{
    public DeserializeException() { }
    public DeserializeException(string message) : base(message) { }
    public DeserializeException(string message, Exception inner) : base(message, inner) { }
}
