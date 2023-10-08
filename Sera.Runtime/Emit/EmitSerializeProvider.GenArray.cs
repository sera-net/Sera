using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Sera.Core.Impls;
using Sera.Runtime.Utils;

namespace Sera.Runtime.Emit;

internal partial class EmitSerializeProvider
{
    private void GenArray(TypeMeta target, CacheStub stub)
    {
        if (target.IsSZArray) GenSZArray(target, stub);
        else throw new NotSupportedException("Multidimensional and non-zero lower bound arrays are not supported");
    }

    private void GenSZArray(TypeMeta target, CacheStub stub)
    {
        #region ready base type

        var element_type = target.Type.GetElementType()!;
        var base_type = typeof(ArraySerializeImplBase<>).MakeGenericType(element_type);
        stub.ProvideType(base_type);

        #endregion

        #region ready dep

        var generics = GenericMeta.Create(element_type);
        var dep = MakeDepContainer(generics, stub.CreateThread);
        stub.ProvideDeps(dep);

        var warp_type = dep.GetSerWarp(0);

        #endregion

        #region ready type

        var type = typeof(ArraySerializeImpl<,>).MakeGenericType(element_type, warp_type);
        stub.ProvideType(type);

        #endregion

        #region create inst
        
        stub.ProvideLateInst(() =>
        {
            // todo refactor deps
            
            // var base_ctor = typeof(ArraySerializeImpl<,>).GetConstructors(BindingFlags.Public | BindingFlags.Instance)
            //     .First(a => a.GetParameters() is { Length: 1 });
            //
            //
            // // ReSharper disable once PossibleMistakenCallToGetType.2
            // if (type.GetType() != 
            //     // ReSharper disable once PossibleMistakenCallToGetType.2
            //     typeof(Type).GetType())
            // {
            //     var ctor1 = TypeBuilder.GetConstructor(type, base_ctor);
            // }
            
            var ctor = type.GetConstructor(
                BindingFlags.Public | BindingFlags.Instance,
                new[] { warp_type }
            )!;
            var inst = ctor.Invoke(new object?[] { null });
            return inst;
        });

        #endregion
    }
}
