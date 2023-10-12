using System;
using System.Reflection;
using Sera.Runtime.Emit.Deps;
using Sera.Runtime.Emit.Ser.Internal;
using Sera.Runtime.Utils;

namespace Sera.Runtime.Emit.Ser.Jobs;

internal class _Enum_Flags_Array(Type UnderlyingType, EnumInfo[] Items) : _Enum_Flags(UnderlyingType)
{
    private Type InfoType = null!;
    private Array Infos = null!;

    public override void Init(EmitStub stub, EmitMeta target)
    {
        ImplType = typeof(FlagsArrayImpl<>).MakeGenericType(target.Type);

        InfoType = typeof(FlagInfo<>).MakeGenericType(target.Type);
        var info_ctor = InfoType.GetConstructor(
            BindingFlags.Public | BindingFlags.Instance,
            new[] { typeof(string), target.Type }
        )!;

        Infos = Array.CreateInstance(InfoType, Items.Length);
        for (var i = 0; i < Items.Length; i++)
        {
            var item = Items[i];
            var info = info_ctor.Invoke(new[] { item.Name, item.Tag.ToObject() });
            Infos.SetValue(info, i);
        }
    }

    public override object CreateInst(EmitStub stub, EmitMeta target, RuntimeDeps deps)
    {
        var inst_ctor = ImplType.GetConstructor(
            BindingFlags.Public | BindingFlags.Instance,
            new[] { InfoType.MakeArrayType() }
        )!;
        return inst_ctor.Invoke(new object[] { Infos });
    }
}
