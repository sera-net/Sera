using Sera.Core.SerDe;

namespace Sera.Core;

public abstract class SeraBase<RT> : ISeraAbility<RT>
{
    public abstract string FormatName { get; }
    public abstract string FormatMIME { get; }
    public abstract SeraFormatType FormatType { get; }
    public abstract ISeraOptions Options { get; }
    public abstract IRuntimeProvider<RT> RuntimeProvider { get; }
    public virtual RT RuntimeImpl => RuntimeProvider.Get();
}

public abstract class SeraBaseForward<RT>(SeraBase<RT> Base) : SeraBase<RT>
{
    public override string FormatName => Base.FormatName;
    public override string FormatMIME => Base.FormatMIME;
    public override SeraFormatType FormatType => Base.FormatType;
    public override ISeraOptions Options => Base.Options;
    public override IRuntimeProvider<RT> RuntimeProvider => Base.RuntimeProvider;
    public override RT RuntimeImpl => Base.RuntimeImpl;
}
