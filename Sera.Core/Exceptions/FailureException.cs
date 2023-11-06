using System;

namespace Sera.Core;

public class SeraFailureException : Exception
{
    public SeraFailureException() { }
    public SeraFailureException(string message) : base(message) { }
    public SeraFailureException(string message, Exception inner) : base(message, inner) { }
}

public class SeraSerFailureException : SeraFailureException
{
    public SeraSerFailureException() { }
    public SeraSerFailureException(string message) : base(message) { }
    public SeraSerFailureException(string message, Exception inner) : base(message, inner) { }
}

public class SeraDeFailureException : SeraFailureException
{
    public SeraDeFailureException() { }
    public SeraDeFailureException(string message) : base(message) { }
    public SeraDeFailureException(string message, Exception inner) : base(message, inner) { }
}
