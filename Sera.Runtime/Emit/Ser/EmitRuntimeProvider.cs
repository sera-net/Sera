using Sera.Core;

namespace Sera.Runtime.Emit.Ser;

public class EmitRuntimeProvider : IRuntimeProvider<EmitRuntimeProvider.Impl>
{
    public static EmitRuntimeProvider Instance { get; } = new();

    public Impl Get() => Impl.Instance;

    public class Impl : ISeraVision<object?>
    {
        // ReSharper disable once MemberHidesStaticFromOuterClass
        public static Impl Instance { get; } = new();

        public R Accept<R, V>(V visitor, object? value) where V : ASeraVisitor<R>
        {
            if (value == null) return visitor.VNone();
            var impl = EmitSerImpls.RuntimeGet(value.GetType());
            return impl.Accept<R, V>(visitor, value);
        }
    }
}
