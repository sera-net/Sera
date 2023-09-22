using System;
using Sera.Runtime.Utils;

namespace Sera.Runtime.Emit;

internal partial class EmitSerializeProvider
{
    private void GenArray(TypeMeta target, CacheStub stub)
    {
        if (target.IsSZArray) { }
        throw new NotImplementedException("todo");
    }
}
