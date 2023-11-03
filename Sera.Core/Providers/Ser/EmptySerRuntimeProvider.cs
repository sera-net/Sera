using Sera.Core.Impls.Ser;

namespace Sera.Core.Providers.Ser;

public sealed class EmptySerRuntimeProvider : IRuntimeProvider<EmptySerRuntimeProvider.Impl>
{
    public static EmptySerRuntimeProvider Instance { get; } = new();

    public Impl Get() => Impl.Instance;

    public sealed class Impl : ISeraVision<object?>
    {
        // ReSharper disable once MemberHidesStaticFromOuterClass
        public static Impl Instance { get; } = new();

        public R Accept<R, V>(V visitor, object? value) where V : ASeraVisitor<R>
        {
            if (value == null) return visitor.VNone();
            return new ReferenceImpl<object, EmptyStructImpl<object>>().Accept<R, V>(visitor, value);
        }
    }
}
