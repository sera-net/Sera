using System;

namespace Sera.Core;

/// <summary>Non-exhaustive match failures will raise the MatchFailureException exception</summary>
public class SeraMatchFailureException : Exception
{
    public SeraMatchFailureException() { }
    public SeraMatchFailureException(string message) : base(message) { }
    public SeraMatchFailureException(string message, Exception inner) : base(message, inner) { }
}
