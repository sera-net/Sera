using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Sera.Core;
using Sera.Core.Impls;
using Sera.Core.Ser;
using Sera.Runtime.Utils;

namespace Sera.Runtime.Emit;

internal partial class EmitSerializeProvider
{
    private void GenEnumVariantPublic(
        Type target, Type underlying_type, EnumInfo[] items, EnumJumpTables? jump_table, CacheStub stub
    )
    {
        #region create type builder

        var guid = Guid.NewGuid();
        var module = ReflectionUtils.CreateAssembly($"Ser.{target.Name}._{guid:N}_");
        var type_builder = module.DefineType(
            $"{module.Assembly.GetName().Name}.SerializeImpl_{target.Name}",
            TypeAttributes.Public | TypeAttributes.Sealed
        );

        stub.ProvideType(type_builder);

        #endregion

        #region ready

        var write_variant_unit = ReflectionUtils.ISerializer_WriteVariantUnit_1generic
            .MakeGenericMethod(target);

        var create_variant_tag = typeof(VariantTag)
            .GetMethod(
                nameof(VariantTag.Create), BindingFlags.Static | BindingFlags.Public,
                new[] { underlying_type }
            )!;

        var new_variant_by_tag = typeof(Variant)
            .GetConstructor(
                BindingFlags.Public | BindingFlags.Instance,
                new[] { typeof(VariantTag) }
            )!;
        
        var new_variant_by_name_tag = typeof(Variant)
            .GetConstructor(
                BindingFlags.Public | BindingFlags.Instance,
                new[] { typeof(string), typeof(VariantTag) }
            )!;

        #endregion

        #region ready deps

        TryGetStaticImpl(underlying_type, out var underlying_dep_inst);
        var underlying_dep_type = underlying_dep_inst!.GetType();
        var underlying_dep_field = type_builder.DefineField(
            "_underlying_ser_impl", underlying_dep_type,
            FieldAttributes.Public | FieldAttributes.Static
        );

        #endregion

        #region public void WriteS>(S serializer, T value, ISeraOptions options) where S : ISerializer

        {
            var write_method = type_builder.DefineMethod("Write", MethodAttributes.Public | MethodAttributes.Virtual);
            var generic_parameters = write_method.DefineGenericParameters("S");
            var TS = generic_parameters[0];
            TS.SetInterfaceConstraints(typeof(ISerializer));
            write_method.SetParameters(TS, target, typeof(ISeraOptions));
            write_method.DefineParameter(1, ParameterAttributes.None, "serializer");
            write_method.DefineParameter(2, ParameterAttributes.None, "value");
            write_method.DefineParameter(3, ParameterAttributes.None, "options");

            var ilg = write_method.GetILGenerator();

            var default_label = ilg.DefineLabel();

            #region def local_int_key_null

            var local_hint_null = ilg.DeclareLocal(typeof(SerializerVariantHint?));
            ilg.Emit(OpCodes.Ldloca_S, local_hint_null);
            ilg.Emit(OpCodes.Initobj, typeof(SerializerVariantHint?));

            #endregion

            if (items.Length > 0)
            {
                if (jump_table != null)
                {
                    var labels = items
                        .Select(_ => ilg.DefineLabel())
                        .ToArray();

                    #region load value

                    ilg.Emit(OpCodes.Ldarg_2);

                    #endregion

                    #region apply offset
                    
                    if (jump_table.Offset == 0)
                    {
                        if (underlying_type != typeof(int) && underlying_type != typeof(uint))
                        {
                            ilg.Emit(OpCodes.Conv_I4);
                        }
                    }
                    else
                    {
                        if (underlying_type == typeof(long) || underlying_type == typeof(ulong))
                        {
                            ilg.Emit(OpCodes.Ldc_I8, (long)jump_table.Offset);
                            ilg.Emit(OpCodes.Add);
                            ilg.Emit(OpCodes.Conv_I4);
                        }
                        else
                        {
                            ilg.Emit(OpCodes.Conv_I4);
                            ilg.Emit(OpCodes.Ldc_I4, jump_table.Offset);
                            ilg.Emit(OpCodes.Add);
                        }
                    }
                    
                    #endregion

                    #region switch
                    
                    var table = jump_table.Table
                        .Select(a => labels[a.Index])
                        .ToArray();
                    ilg.Emit(OpCodes.Switch, table);

                    #endregion
                    
                    #region goto default

                    ilg.Emit(OpCodes.Br, default_label);

                    #endregion

                    #region cases

                    for (var i = 0; i < items.Length; i++)
                    {
                        var item = items[i];
                        var label = labels[i];
                        
                        ilg.MarkLabel(label);
                        
                        #region load serializer

                        ilg.Emit(OpCodes.Ldarga, 1);

                        #endregion

                        #region load target.Name

                        ilg.Emit(OpCodes.Ldstr, target.Name);

                        #endregion
                        
                        #region new Variant

                        ilg.Emit(OpCodes.Ldstr, item.Name);
                        ilg.Emit(OpCodes.Ldarg_2);
                        ilg.Emit(OpCodes.Call, create_variant_tag);
                        ilg.Emit(OpCodes.Newobj, new_variant_by_name_tag);

                        #endregion
                        
                        #region load hint

                        ilg.Emit(OpCodes.Ldloc, local_hint_null);

                        #endregion

                        #region serializer.WriteVariantUnit<T>(target.Name, variant, null);

                        ilg.Emit(OpCodes.Constrained, TS);
                        ilg.Emit(OpCodes.Callvirt, write_variant_unit);

                        #endregion

                        #region return;

                        ilg.Emit(OpCodes.Ret);

                        #endregion
                    }

                    #endregion
                }
                else
                {
                    // todo
                }
            }

            #region default

            ilg.MarkLabel(default_label);

            #region load serializer

            ilg.Emit(OpCodes.Ldarga, 1);

            #endregion

            #region load target.Name

            ilg.Emit(OpCodes.Ldstr, target.Name);

            #endregion

            #region new Variant

            ilg.Emit(OpCodes.Ldarg_2);
            ilg.Emit(OpCodes.Call, create_variant_tag);
            ilg.Emit(OpCodes.Newobj, new_variant_by_tag);

            #endregion

            #region load hint

            ilg.Emit(OpCodes.Ldloc, local_hint_null);

            #endregion

            #region serializer.WriteVariantUnit<T>(target.Name, variant, null);

            ilg.Emit(OpCodes.Constrained, TS);
            ilg.Emit(OpCodes.Callvirt, write_variant_unit);

            #endregion

            #region return;

            ilg.Emit(OpCodes.Ret);

            #endregion

            #endregion

            var interface_type = typeof(ISerialize<>).MakeGenericType(target);
            type_builder.AddInterfaceImplementation(interface_type);
            type_builder.DefineMethodOverride(write_method, interface_type.GetMethod("Write")!);
        }

        #endregion

        #region create type

        var type = type_builder.CreateType();
        stub.ProvideType(type);

        var dep_field = type.GetField(underlying_dep_field.Name, BindingFlags.Public | BindingFlags.Static)!;
        dep_field.SetValue(null, underlying_dep_inst);

        #endregion

        #region create inst

        var inst = Activator.CreateInstance(type)!;
        stub.ProvideInst(inst);

        #endregion
    }
}
