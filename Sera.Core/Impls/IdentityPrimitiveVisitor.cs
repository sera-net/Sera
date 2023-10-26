using System;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Sera.Core.De;

namespace Sera.Core.Impls;

public struct IdentityPrimitiveVisitor<R> : IPrimitiveDeserializerVisitor<R>
{
    public R VisitPrimitive<T>(T value) =>
        value is R r ? r : throw DeserializeInvalidTypeException.Unexpected(value);

    public R Visit(bool value) => value is R r ? r : throw DeserializeInvalidTypeException.Unexpected(value);

    public R Visit(sbyte value) => value is R r ? r : throw DeserializeInvalidTypeException.Unexpected(value);

    public R Visit(short value) => value is R r ? r : throw DeserializeInvalidTypeException.Unexpected(value);

    public R Visit(int value) => value is R r ? r : throw DeserializeInvalidTypeException.Unexpected(value);

    public R Visit(long value) => value is R r ? r : throw DeserializeInvalidTypeException.Unexpected(value);

    public R Visit(Int128 value) => value is R r ? r : throw DeserializeInvalidTypeException.Unexpected(value);

    public R Visit(byte value) => value is R r ? r : throw DeserializeInvalidTypeException.Unexpected(value);

    public R Visit(ushort value) => value is R r ? r : throw DeserializeInvalidTypeException.Unexpected(value);

    public R Visit(uint value) => value is R r ? r : throw DeserializeInvalidTypeException.Unexpected(value);

    public R Visit(ulong value) => value is R r ? r : throw DeserializeInvalidTypeException.Unexpected(value);

    public R Visit(UInt128 value) => value is R r ? r : throw DeserializeInvalidTypeException.Unexpected(value);

    public R Visit(IntPtr value) => value is R r ? r : throw DeserializeInvalidTypeException.Unexpected(value);

    public R Visit(UIntPtr value) => value is R r ? r : throw DeserializeInvalidTypeException.Unexpected(value);

    public R Visit(Half value) => value is R r ? r : throw DeserializeInvalidTypeException.Unexpected(value);

    public R Visit(float value) => value is R r ? r : throw DeserializeInvalidTypeException.Unexpected(value);

    public R Visit(double value) => value is R r ? r : throw DeserializeInvalidTypeException.Unexpected(value);

    public R Visit(decimal value) => value is R r ? r : throw DeserializeInvalidTypeException.Unexpected(value);

    public R Visit(BigInteger value) => value is R r ? r : throw DeserializeInvalidTypeException.Unexpected(value);

    public R Visit(Complex value) => value is R r ? r : throw DeserializeInvalidTypeException.Unexpected(value);
    
    public R Visit(TimeSpan value) => value is R r ? r : throw DeserializeInvalidTypeException.Unexpected(value);

    public R Visit(DateOnly value) => value is R r ? r : throw DeserializeInvalidTypeException.Unexpected(value);
    
    public R Visit(TimeOnly value) => value is R r ? r : throw DeserializeInvalidTypeException.Unexpected(value);

    public R Visit(DateTime value) => value is R r ? r : throw DeserializeInvalidTypeException.Unexpected(value);

    public R Visit(DateTimeOffset value) => value is R r ? r : throw DeserializeInvalidTypeException.Unexpected(value);

    public R Visit(Guid value) => value is R r ? r : throw DeserializeInvalidTypeException.Unexpected(value);

    public R Visit(Range value) => value is R r ? r : throw DeserializeInvalidTypeException.Unexpected(value);

    public R Visit(Index value) => value is R r ? r : throw DeserializeInvalidTypeException.Unexpected(value);

    public R Visit(char value) => value is R r ? r : throw DeserializeInvalidTypeException.Unexpected(value);

    public R Visit(Rune value) => value is R r ? r : throw DeserializeInvalidTypeException.Unexpected(value);
}

public struct AsyncIdentityPrimitiveVisitor<R> : IAsyncPrimitiveDeserializerVisitor<R>
{
    public ValueTask<R> VisitPrimitiveAsync<T>(T value)
        => value is R r ? ValueTask.FromResult(r) : throw DeserializeInvalidTypeException.Unexpected(value);

    public ValueTask<R> VisitAsync(bool value)
        => value is R r ? ValueTask.FromResult(r) : throw DeserializeInvalidTypeException.Unexpected(value);

    public ValueTask<R> VisitAsync(sbyte value)
        => value is R r ? ValueTask.FromResult(r) : throw DeserializeInvalidTypeException.Unexpected(value);

    public ValueTask<R> VisitAsync(short value)
        => value is R r ? ValueTask.FromResult(r) : throw DeserializeInvalidTypeException.Unexpected(value);

    public ValueTask<R> VisitAsync(int value)
        => value is R r ? ValueTask.FromResult(r) : throw DeserializeInvalidTypeException.Unexpected(value);

    public ValueTask<R> VisitAsync(long value)
        => value is R r ? ValueTask.FromResult(r) : throw DeserializeInvalidTypeException.Unexpected(value);

    public ValueTask<R> VisitAsync(Int128 value)
        => value is R r ? ValueTask.FromResult(r) : throw DeserializeInvalidTypeException.Unexpected(value);

    public ValueTask<R> VisitAsync(byte value)
        => value is R r ? ValueTask.FromResult(r) : throw DeserializeInvalidTypeException.Unexpected(value);

    public ValueTask<R> VisitAsync(ushort value)
        => value is R r ? ValueTask.FromResult(r) : throw DeserializeInvalidTypeException.Unexpected(value);

    public ValueTask<R> VisitAsync(uint value)
        => value is R r ? ValueTask.FromResult(r) : throw DeserializeInvalidTypeException.Unexpected(value);

    public ValueTask<R> VisitAsync(ulong value)
        => value is R r ? ValueTask.FromResult(r) : throw DeserializeInvalidTypeException.Unexpected(value);

    public ValueTask<R> VisitAsync(UInt128 value)
        => value is R r ? ValueTask.FromResult(r) : throw DeserializeInvalidTypeException.Unexpected(value);

    public ValueTask<R> VisitAsync(IntPtr value)
        => value is R r ? ValueTask.FromResult(r) : throw DeserializeInvalidTypeException.Unexpected(value);

    public ValueTask<R> VisitAsync(UIntPtr value)
        => value is R r ? ValueTask.FromResult(r) : throw DeserializeInvalidTypeException.Unexpected(value);

    public ValueTask<R> VisitAsync(Half value)
        => value is R r ? ValueTask.FromResult(r) : throw DeserializeInvalidTypeException.Unexpected(value);

    public ValueTask<R> VisitAsync(float value)
        => value is R r ? ValueTask.FromResult(r) : throw DeserializeInvalidTypeException.Unexpected(value);

    public ValueTask<R> VisitAsync(double value)
        => value is R r ? ValueTask.FromResult(r) : throw DeserializeInvalidTypeException.Unexpected(value);

    public ValueTask<R> VisitAsync(decimal value)
        => value is R r ? ValueTask.FromResult(r) : throw DeserializeInvalidTypeException.Unexpected(value);

    public ValueTask<R> VisitAsync(BigInteger value)
        => value is R r ? ValueTask.FromResult(r) : throw DeserializeInvalidTypeException.Unexpected(value);

    public ValueTask<R> VisitAsync(Complex value)
        => value is R r ? ValueTask.FromResult(r) : throw DeserializeInvalidTypeException.Unexpected(value);

    public ValueTask<R> VisitAsync(TimeSpan value)
        => value is R r ? ValueTask.FromResult(r) : throw DeserializeInvalidTypeException.Unexpected(value);
    
    public ValueTask<R> VisitAsync(DateOnly value)
        => value is R r ? ValueTask.FromResult(r) : throw DeserializeInvalidTypeException.Unexpected(value);
    
    public ValueTask<R> VisitAsync(TimeOnly value)
        => value is R r ? ValueTask.FromResult(r) : throw DeserializeInvalidTypeException.Unexpected(value);

    public ValueTask<R> VisitAsync(DateTime value)
        => value is R r ? ValueTask.FromResult(r) : throw DeserializeInvalidTypeException.Unexpected(value);

    public ValueTask<R> VisitAsync(DateTimeOffset value)
        => value is R r ? ValueTask.FromResult(r) : throw DeserializeInvalidTypeException.Unexpected(value);

    public ValueTask<R> VisitAsync(Guid value)
        => value is R r ? ValueTask.FromResult(r) : throw DeserializeInvalidTypeException.Unexpected(value);

    public ValueTask<R> VisitAsync(Range value)
        => value is R r ? ValueTask.FromResult(r) : throw DeserializeInvalidTypeException.Unexpected(value);

    public ValueTask<R> VisitAsync(Index value)
        => value is R r ? ValueTask.FromResult(r) : throw DeserializeInvalidTypeException.Unexpected(value);

    public ValueTask<R> VisitAsync(char value)
        => value is R r ? ValueTask.FromResult(r) : throw DeserializeInvalidTypeException.Unexpected(value);

    public ValueTask<R> VisitAsync(Rune value)
        => value is R r ? ValueTask.FromResult(r) : throw DeserializeInvalidTypeException.Unexpected(value);
}
