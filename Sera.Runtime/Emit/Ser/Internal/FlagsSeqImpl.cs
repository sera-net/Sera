using System;
using System.Runtime.CompilerServices;
using Sera.Core;
using Sera.Core.Impls.Ser;

namespace Sera.Runtime.Emit.Ser.Internal;

public readonly record struct FlagInfo<T>(string name, T value);

public readonly struct FlagsSeqImpl<T>(FlagInfo<T>[] Items) : ISeraVision<T>
    where T : Enum
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Accept<R, V>(V visitor, T value) where V : ASeraVisitor<R>
        => visitor.VSeq(new Wrapper(new Impl(Items, value)));

    private readonly struct Wrapper(Impl impl) : ISeqSeraVision
    {
        public int? Count => null;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext() => impl.MoveNext();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public R AcceptNext<R, V>(V visitor) where V : ASeqSeraVisitor<R>
            => impl.AcceptNext<R, V>(visitor);
    }

    private sealed class Impl(FlagInfo<T>[] Items, T value) : ISeqSeraVision
    {
        public int? Count => null;
        private int index;
        private FlagInfo<T> item;
        private bool HasNext;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext()
        {
            loop:
            if (index >= Items.Length) return HasNext = false;
            if (value.HasFlag((item = Items[index]).value))
            {
                index += 1;
                return HasNext = true;
            }
            else
            {
                index += 1;
                goto loop;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public R AcceptNext<R, V>(V visitor) where V : ASeqSeraVisitor<R>
        {
            if (!HasNext) return visitor.VEnd();
            return visitor.VItem(new StringImpl(), item.name);
        }
    }
}
