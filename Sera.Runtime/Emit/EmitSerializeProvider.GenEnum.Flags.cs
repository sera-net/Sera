using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Numerics;
using System.Reflection;
using Sera.Core;
using Sera.Core.Impls;
using Sera.Core.Ser;
using Sera.Runtime.Utils;

namespace Sera.Runtime.Emit;

internal partial class EmitSerializeProvider
{
    private void GenEnumFlags(Type target, Type underlying_type, CacheStub stub)
    {
        #region ready

        var flags_attr = target.GetCustomAttribute<SeraFlagsAttribute>();
        var mode = flags_attr?.Mode ?? SeraFlagsMode.Array;

        #endregion

        if (mode == SeraFlagsMode.Array)
        {
            var items = GetEnumInfo(target, underlying_type, distinct: false);
            GenEnumFlagsArray(target, underlying_type, items, stub);
        }
        else if (mode == SeraFlagsMode.Number)
        {
            #region ready type

            var number_impl_type = typeof(PrimitiveImpl<>).MakeGenericType(underlying_type);
            var type = typeof(FlagsAsUnderlyingImpl<,,>).MakeGenericType(target, underlying_type, number_impl_type);
            stub.ProvideType(type);

            #endregion

            #region cast

            var cast_arg_a = Expression.Parameter(target);
            var cast_expr = Expression.Convert(cast_arg_a, underlying_type);
            var cast = Expression.Lambda(cast_expr, cast_arg_a).Compile();

            #endregion

            #region create inst

            var number_impl_inst_ctor = number_impl_type.GetConstructor(
                BindingFlags.Public | BindingFlags.Instance,
                new[] { typeof(SerializerPrimitiveHint?) }
            )!;
            var number_impl_inst = number_impl_inst_ctor.Invoke(new object?[] { ((SerializerPrimitiveHint?)null) });

            var inst_ctor = type.GetConstructor(
                BindingFlags.Public | BindingFlags.Instance,
                new[] { typeof(Func<,>).MakeGenericType(target, underlying_type), number_impl_type }
            )!;
            var inst = inst_ctor.Invoke(new[] { cast, number_impl_inst });
            stub.ProvideInst(inst);

            #endregion
        }
        else if (mode == SeraFlagsMode.String)
        {
            #region ready type

            var type = typeof(ToStringSerializeImpl<>).MakeGenericType(target);
            stub.ProvideType(type);

            #endregion

            #region create inst

            var inst_field = type.GetProperty(nameof(ToStringSerializeImpl<Enum>.Instance),
                BindingFlags.Public | BindingFlags.Static)!;
            var inst = inst_field.GetValue(null)!;
            stub.ProvideInst(inst);

            #endregion
        }
        else if (mode == SeraFlagsMode.StringSplit)
        {
            #region ready type

            var type = typeof(FlagsToStringSplitSerializeImpl<>).MakeGenericType(target);
            stub.ProvideType(type);

            #endregion

            #region create inst

            var inst_field = type.GetProperty(nameof(FlagsToStringSplitSerializeImpl<Enum>.Instance),
                BindingFlags.Public | BindingFlags.Static)!;
            var inst = inst_field.GetValue(null)!;
            stub.ProvideInst(inst);

            #endregion
        }
        else throw new ArgumentOutOfRangeException();
    }

    internal record FlagsAsUnderlyingImpl<T, V, SV>(Func<T, V> Cast, SV Serialize) : ISerialize<T>
        where T : Enum where V : unmanaged where SV : ISerialize<V>
    {
        public void Write<S>(S serializer, T value, ISeraOptions options) where S : ISerializer
            => Serialize.Write(serializer, Cast(value), options);
    }

    private void GenEnumFlagsArray(Type target, Type underlying_type, EnumInfo[] items, CacheStub stub)
    {
        #region ready type

        var type = typeof(FlagsArrayImpl<>).MakeGenericType(target);
        stub.ProvideType(type);

        #endregion

        #region infos

        var info_type = typeof(FlagInfo<>).MakeGenericType(target);
        var info_ctor = info_type.GetConstructor(
            BindingFlags.Public | BindingFlags.Instance,
            new[] { typeof(string), target }
        )!;

        var infos = Array.CreateInstance(info_type, items.Length);
        for (var i = 0; i < items.Length; i++)
        {
            var item = items[i];
            var info = info_ctor.Invoke(new[] { item.Name, item.Tag.ToObject() });
            infos.SetValue(info, i);
        }

        #endregion

        #region create inst

        var inst_ctor = type.GetConstructor(
            BindingFlags.Public | BindingFlags.Instance,
            new[] { info_type.MakeArrayType() }
        )!;
        var inst = inst_ctor.Invoke(new object[] { infos });
        stub.ProvideInst(inst);

        #endregion
    }

    internal record struct FlagInfo<T>(string name, T value);

    internal record FlagsArrayImpl<T>(FlagInfo<T>[] Items) : ISerialize<T>, ISeqSerializerReceiver<T>
        where T : Enum
    {
        public void Write<S>(S serializer, T value, ISeraOptions options) where S : ISerializer
        {
            serializer.StartSeq<string, T, FlagsArrayImpl<T>>(null, value, this);
        }

        public void Receive<S>(T value, S serializer) where S : ISeqSerializer
        {
            foreach (var item in Items)
            {
                if (value.HasFlag(item.value))
                {
                    serializer.WriteElement(item.name, StringImpl.Instance);
                }
            }
        }
    }
}
