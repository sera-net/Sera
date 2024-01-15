using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Sera.Core;
using Sera.Core.Builders;
using Sera.Core.Builders.De;
using Sera.Core.Builders.Inputs;
using Sera.Utils;

namespace Sera;

public static class SeraBuilderForDe
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DeBuilder<B> Make<B>(this B builder)
        where B : IDeBuilder => new(builder);
}

public static class SeraBuilderForStaticDe
{
    #region Accept

    public readonly struct StaticAccept<T, C>(C Colion) : AcceptASeraColctor<T, T, T>
        where C : ISeraColion<T>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Accept<CI>(CI colctor) where CI : ISeraColctor<T, T>
            => Colion.Collect<T, CI>(ref colctor);
    }

    public readonly struct AsyncStaticAccept<T, C>(C Colion) : AcceptASeraColctor<T, ValueTask<T>, ValueTask<T>>
        where C : ISeraColion<T>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ValueTask<T> Accept<CI>(CI colctor) where CI : ISeraColctor<T, ValueTask<T>>
            => Colion.Collect<ValueTask<T>, CI>(ref colctor);
    }

    #endregion

    #region String

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T String<B, T, C>(this From<Static<DeBuilder<B, T, C>>> self, string str)
        where B : IDeBuilder, IStringInput where C : ISeraColion<T>
    {
        var (Target, Colion) = self.Target.Target;
        var source = CompoundString.MakeString(str);
        return Target.BuildStringInput<T, T, StaticAccept<T, C>>(new StaticAccept<T, C>(Colion), default, source);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T String<B, T, C>(this From<Static<DeBuilder<B, T, C>>> self, ReadOnlyMemory<char> str)
        where B : IDeBuilder, IStringInput where C : ISeraColion<T>
    {
        var (Target, Colion) = self.Target.Target;
        var source = CompoundString.MakeMemory(str);
        return Target.BuildStringInput<T, T, StaticAccept<T, C>>(new StaticAccept<T, C>(Colion), default, source);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe T String<B, T, C>(this From<Static<DeBuilder<B, T, C>>> self, ReadOnlySpan<char> str)
        where B : IDeBuilder, IStringInput where C : ISeraColion<T>
    {
        var (Target, Colion) = self.Target.Target;
        fixed (char* ptr = str)
        {
            var source = CompoundString.MakeSpan(new ReadOnlyFixedSpan<char>(ptr, (nuint)str.Length));
            return Target.BuildStringInput<T, T, StaticAccept<T, C>>(new StaticAccept<T, C>(Colion), default, source);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T String<B, T, C>(this From<Static<DeBuilder<B, T, C>>> self, ReadOnlyFixedSpan<char> str)
        where B : IDeBuilder, IStringInput where C : ISeraColion<T>
    {
        var (Target, Colion) = self.Target.Target;
        var source = CompoundString.MakeSpan(str);
        return Target.BuildStringInput<T, T, StaticAccept<T, C>>(new StaticAccept<T, C>(Colion), default, source);
    }

    #endregion

    #region Stream

    /// <summary>
    /// Does not require ownership of stream, stream's dispose will not be called
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Stream<B, T, C>(this From<Static<DeBuilder<B, T, C>>> self, Stream stream)
        where B : IDeBuilder, IStreamInput where C : ISeraColion<T>
    {
        var (Target, Colion) = self.Target.Target;
        return Target.BuildStreamInput<T, T, StaticAccept<T, C>>(new StaticAccept<T, C>(Colion), default, stream);
    }

    #endregion

    #region StreamAsync

    /// <summary>
    /// Does not require ownership of stream, stream's dispose will not be called
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueTask<T> StreamAsync<B, T, C>(this From<Static<DeBuilder<B, T, C>>> self, Stream stream)
        where B : IDeBuilder, IAsyncStreamInput where C : ISeraColion<T>
    {
        var (Target, Colion) = self.Target.Target;
        return Target.BuildAsyncStreamInput<T, T, AsyncStaticAccept<T, C>>(
            new AsyncStaticAccept<T, C>(Colion), default, stream);
    }

    #endregion
}
