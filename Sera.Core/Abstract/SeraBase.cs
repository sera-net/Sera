using Sera.Core.SerDe;

namespace Sera.Core;

public abstract class SeraBase : ISeraAbility
{
    public abstract string FormatName { get; }
    public abstract string FormatMIME { get; }
    public abstract SeraFormatType FormatType { get; }
    public abstract ISeraOptions Options { get; }
    public abstract IRuntimeProvider RuntimeProvider { get; }
}

public abstract class SeraBaseForward(SeraBase Base) : SeraBase
{
    public override string FormatName => Base.FormatName;
    public override string FormatMIME => Base.FormatMIME;
    public override SeraFormatType FormatType => Base.FormatType;
    public override ISeraOptions Options => Base.Options;
    public override IRuntimeProvider RuntimeProvider => Base.RuntimeProvider;
}
