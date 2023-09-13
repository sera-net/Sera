using System;

namespace Sera.Runtime.Emit;

internal partial class EmitSerializeProvider
{
    private void GenEnumFlagsPublic(
        Type target, Type underlying_type, EnumInfo[] items, EnumJumpTables? jump_table, CacheStub stub
    )
    {
        throw new NotImplementedException("todo");
    }
}
