using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using Sera.Core;
using Sera.Core.Providers.Ser;
using Sera.Runtime.Emit;
using SerImpl = Sera.Core.Impls.Ser;

namespace Sera.Runtime.Utils.Internal;

public static class ReflectionUtils
{
    public const string Name__VariantTag_Create = nameof(VariantTag.Create);
    public const string Name__ISeraVision_Accept = nameof(ISeraVision<object>.Accept);
    public const string Name__IUnionSeraVision_Name = nameof(IUnionSeraVision<object>.Name);
    public const string Name__ITupleSeraVision_Size = nameof(ITupleSeraVision<object>.Size);
    public const string Name__IStructSeraVision_Count = nameof(IStructSeraVision<object>.Count);
    public const string Name__IUnionSeraVision_AcceptUnion = nameof(IUnionSeraVision<object>.AcceptUnion);
    public const string Name__ITupleSeraVision_AcceptItem = nameof(ITupleSeraVision<object>.AcceptItem);
    public static readonly Type TypeDel__ASeraVisitor = typeof(ASeraVisitor<>);
    public static readonly Type TypeDel__ISeraVision = typeof(ISeraVision<>);
    public static readonly Type TypeDel__IUnionSeraVision = typeof(IUnionSeraVision<>);
    public static readonly Type TypeDel__AUnionSeraVisitor = typeof(AUnionSeraVisitor<>);
    public static readonly Type TypeDel__ITupleSeraVision = typeof(ITupleSeraVision<>);
    public static readonly Type TypeDel__ATupleSeraVisitor = typeof(ATupleSeraVisitor<>);

    public static bool IsTypeBuilder(this Type type) =>
        type is TypeBuilder ||
        type is { IsGenericType: true } && type.GetGenericArguments().AsParallel().Any(IsTypeBuilder);

    public static ConstructorInfo NullReferenceException_ctor { get; } =
        typeof(NullReferenceException).GetConstructor(
            BindingFlags.Public | BindingFlags.Instance,
            Array.Empty<Type>()
        )!;

    public static ConstructorInfo Nullable_UInt64_ctor { get; } =
        typeof(long?).GetConstructor(new[] { typeof(long) })!;
    public static ConstructorInfo Nullable_UIntPtr_ctor { get; } =
        typeof(nuint?).GetConstructor(new[] { typeof(nuint) })!;

    private static readonly ConditionalWeakTable<Type, MethodInfo> _Memory_to_ReadOnlyMemory = new();

    public static MethodInfo Get_Memory_to_ReadOnlyMemory(Type itemType) =>
        _Memory_to_ReadOnlyMemory.GetValue(typeof(Memory<>).MakeGenericType(itemType), static t => t
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Single(a =>
                a is { Name: "op_Implicit" }
                && a.GetParameters() is [{ ParameterType : { IsGenericType: true } p1 }]
                && p1.GetGenericTypeDefinition() == typeof(Memory<>)
                && a.ReturnType is { IsGenericType: true } r
                && r.GetGenericTypeDefinition() == typeof(ReadOnlyMemory<>)
            ));

    public static MethodInfo StaticSerImpls_TryGet { get; } =
        typeof(StaticSerImpls).GetMethod(nameof(StaticSerImpls.TryGet))!;

    public static MethodInfo ASeraVisitor_VStruct { get; } =
        typeof(ASeraVisitor<>).GetMethod(nameof(ASeraVisitor<Unit>.VStruct),
            BindingFlags.Public | BindingFlags.Instance)!;

    public static MethodInfo ATupleSeraVisitor_VNone { get; } =
        typeof(ATupleSeraVisitor<>).GetMethod(nameof(ATupleSeraVisitor<Unit>.VNone),
            BindingFlags.Public | BindingFlags.Instance)!;
    
    public static MethodInfo ATupleSeraVisitor_VItem { get; } =
        typeof(ATupleSeraVisitor<>).GetMethod(nameof(ATupleSeraVisitor<Unit>.VItem),
            BindingFlags.Public | BindingFlags.Instance)!;
    public static MethodInfo AStructSeraVisitor_VField { get; } =
        typeof(AStructSeraVisitor<>).GetMethod(nameof(AStructSeraVisitor<Unit>.VField),
            BindingFlags.Public | BindingFlags.Instance)!;

    public static MethodInfo AStructSeraVisitor_VNone { get; } =
        typeof(AStructSeraVisitor<>).GetMethod(nameof(AStructSeraVisitor<Unit>.VNone),
            BindingFlags.Public | BindingFlags.Instance)!;

    public static MethodInfo ASeraVisitor_VUnion { get; } =
        typeof(ASeraVisitor<>).GetMethod(nameof(ASeraVisitor<Unit>.VUnion),
            BindingFlags.Public | BindingFlags.Instance)!;

    public static MethodInfo AUnionSeraVisitor_VVariant { get; } =
        typeof(AUnionSeraVisitor<>).GetMethod(nameof(AUnionSeraVisitor<Unit>.VVariant),
            BindingFlags.Public | BindingFlags.Instance)!;

    public static MethodInfo AUnionSeraVisitor_VVariantValue { get; } =
        typeof(AUnionSeraVisitor<>).GetMethod(nameof(AUnionSeraVisitor<Unit>.VVariantValue),
            BindingFlags.Public | BindingFlags.Instance)!;
    
    public static MethodInfo AUnionSeraVisitor_VVariantTuple { get; } =
        typeof(AUnionSeraVisitor<>).GetMethod(nameof(AUnionSeraVisitor<Unit>.VVariantTuple),
            BindingFlags.Public | BindingFlags.Instance)!;

    public static MethodInfo AUnionSeraVisitor_VNone { get; } =
        typeof(AUnionSeraVisitor<>).GetMethod(nameof(AUnionSeraVisitor<Unit>.VNone),
            BindingFlags.Public | BindingFlags.Instance)!;

    public static MethodInfo AUnionSeraVisitor_VEmpty { get; } =
        typeof(AUnionSeraVisitor<>).GetMethod(nameof(AUnionSeraVisitor<Unit>.VEmpty),
            BindingFlags.Public | BindingFlags.Instance)!;


    public static readonly ConstructorInfo IsReadOnlyAttributeCtor =
        typeof(IsReadOnlyAttribute).GetConstructor(BindingFlags.Instance | BindingFlags.Public, Array.Empty<Type>())!;

    public static void MarkReadonly(this MethodBuilder builder) =>
        builder.SetCustomAttribute(new CustomAttributeBuilder(IsReadOnlyAttributeCtor, Array.Empty<object>()));

    public static void MarkReadonly(this TypeBuilder builder) =>
        builder.SetCustomAttribute(new CustomAttributeBuilder(IsReadOnlyAttributeCtor, Array.Empty<object>()));

    public static readonly MethodInfo ToFrozenDictionary_2generic_2arg__IEnumerable_KeyValuePair__IEqualityComparer =
        typeof(FrozenDictionary)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Single(static a =>
                a is { Name: nameof(FrozenDictionary.ToFrozenDictionary), IsGenericMethod: true } &&
                a.GetGenericArguments() is { Length: 2 } && a.GetParameters() is [var arg1, var arg2]
                && arg1.ParameterType.GetGenericTypeDefinition() == typeof(IEnumerable<>)
                && arg1.ParameterType.GetGenericArguments() is [var arg1_g1]
                && arg1_g1.GetGenericTypeDefinition() == typeof(KeyValuePair<,>)
                && arg2.ParameterType.GetGenericTypeDefinition() == typeof(IEqualityComparer<>)
            );

    public static string GetAsmName(string name) => $"{nameof(Sera)}.{nameof(Runtime)}.{nameof(Emit)}.Runtime.{name}";

    public static ModuleBuilder CreateAssembly(string name)
    {
        var asm_name = new AssemblyName(GetAsmName(name));
        AssemblyBuilder asm;
        if (Debugger.IsAttached)
        {
            var con = typeof(DebuggableAttribute).GetConstructor(new[] { typeof(bool), typeof(bool) })!;
            var debug_attr = new CustomAttributeBuilder(con, new object[] { true, true });
            asm = AssemblyBuilder.DefineDynamicAssembly(asm_name, AssemblyBuilderAccess.RunAndCollect,
                new[] { debug_attr });
        }
        else
        {
            asm = AssemblyBuilder.DefineDynamicAssembly(asm_name, AssemblyBuilderAccess.RunAndCollect);
        }
        return asm.DefineDynamicModule(asm_name.Name!);
    }

    public static readonly FrozenSet<Type> SingleGenericTypes = new[]
    {
        typeof(IEnumerable<>),
        typeof(ICollection<>),
        typeof(IList<>),
        typeof(ISet<>),

        typeof(IReadOnlyCollection<>),
        typeof(IReadOnlyList<>),
        typeof(IReadOnlySet<>),

        typeof(ConcurrentBag<>),
        typeof(ConcurrentQueue<>),
        typeof(ConcurrentStack<>),

        typeof(FrozenSet<>),
        typeof(HashSet<>),

        typeof(List<>),
        typeof(LinkedList<>),
        typeof(Queue<>),
        typeof(Stack<>),
        typeof(SortedSet<>),

        typeof(IImmutableList<>),
        typeof(IImmutableQueue<>),
        typeof(IImmutableStack<>),
        typeof(IImmutableSet<>),

        typeof(ImmutableList<>),
        typeof(ImmutableQueue<>),
        typeof(ImmutableStack<>),
        typeof(ImmutableSortedSet<>),
        typeof(ImmutableHashSet<>),
        typeof(ImmutableArray<>),
    }.ToFrozenSet();

    public static readonly FrozenSet<Type> DoubleGenericTypes = new[]
    {
        typeof(IDictionary<,>),
        typeof(IReadOnlyDictionary<,>),

        typeof(FrozenDictionary<,>),
        typeof(Dictionary<,>),
        typeof(SortedDictionary<,>),
        typeof(SortedList<,>),
        typeof(ReadOnlyDictionary<,>),

        typeof(ImmutableDictionary<,>),
        typeof(ImmutableSortedDictionary<,>),

        typeof(ConditionalWeakTable<,>),
    }.ToFrozenSet();

    public static FrozenSet<Type> ValueTuples { get; } = new HashSet<Type>
    {
        typeof(ValueTuple<>),
        typeof(ValueTuple<,>),
        typeof(ValueTuple<,,>),
        typeof(ValueTuple<,,,>),
        typeof(ValueTuple<,,,,>),
        typeof(ValueTuple<,,,,,>),
        typeof(ValueTuple<,,,,,,>),
        typeof(ValueTuple<,,,,,,,>),
    }.ToFrozenSet();

    public static FrozenSet<Type> ClassTuples { get; } = new HashSet<Type>
    {
        typeof(Tuple<>),
        typeof(Tuple<,>),
        typeof(Tuple<,,>),
        typeof(Tuple<,,,>),
        typeof(Tuple<,,,,>),
        typeof(Tuple<,,,,,>),
        typeof(Tuple<,,,,,,>),
        typeof(Tuple<,,,,,,,>),
    }.ToFrozenSet();

    public static bool IsTuple(this EmitMeta target)
        => IsTuple(target.Type, out _);

    public static bool IsTuple(this EmitMeta target, out bool is_value_tuple)
        => IsTuple(target.Type, out is_value_tuple);

    public static bool IsTuple(this TypeMeta target)
        => IsTuple(target.Type, out _);

    public static bool IsTuple(this TypeMeta target, out bool is_value_tuple)
        => IsTuple(target.Type, out is_value_tuple);

    public static bool IsTuple(Type target) => IsTuple(target, out _);

    public static bool IsTuple(Type target, out bool is_value_tuple)
    {
        if (!target.IsGenericType)
        {
            is_value_tuple = false;
            return false;
        }
        var t = target.GetGenericTypeDefinition();
        if (ValueTuples.Contains(t))
        {
            is_value_tuple = true;
            return true;
        }
        else if (ClassTuples.Contains(t))
        {
            is_value_tuple = false;
            return true;
        }
        else
        {
            is_value_tuple = false;
            return false;
        }
    }

    public static bool IsValueTuple(this EmitMeta target)
        => IsValueTuple(target.Type);

    public static bool IsValueTuple(this TypeMeta target)
        => IsValueTuple(target.Type);

    public static bool IsValueTuple(Type target)
    {
        if (!target.IsGenericType) return false;
        var t = target.GetGenericTypeDefinition();
        return ValueTuples.Contains(t);
    }

    public static bool IsClassTuple(this EmitMeta target)
        => IsClassTuple(target.Type);

    public static bool IsClassTuple(this TypeMeta target)
        => IsClassTuple(target.Type);

    public static bool IsClassTuple(Type target)
    {
        if (!target.IsGenericType) return false;
        var t = target.GetGenericTypeDefinition();
        return ClassTuples.Contains(t);
    }

    public static FrozenDictionary<int, Type> TupleSerImpls { get; } = new Dictionary<int, Type>
    {
        { 1, typeof(SerImpl.TupleImpl<,>) },
        { 2, typeof(SerImpl.TupleImpl<,,,>) },
        { 3, typeof(SerImpl.TupleImpl<,,,,,>) },
        { 4, typeof(SerImpl.TupleImpl<,,,,,,,>) },
        { 5, typeof(SerImpl.TupleImpl<,,,,,,,,,>) },
        { 6, typeof(SerImpl.TupleImpl<,,,,,,,,,,,>) },
        { 7, typeof(SerImpl.TupleImpl<,,,,,,,,,,,,,>) },
    }.ToFrozenDictionary();

    public static bool IsAssignableTo2(this Type type, Type target)
    {
        if (type == target) return true;
        if (!type.IsGenericTypeDefinition && !target.IsGenericTypeDefinition) return type.IsAssignableTo(target);
        for (;;)
        {
            var l = type.IsGenericType ? type.GetGenericTypeDefinition() : type;
            var r = target.IsGenericType ? target.GetGenericTypeDefinition() : target;

            if (l == r) return true;

            var base_type = type.BaseType;
            if (base_type == null) return false;
            if (base_type == target) return true;

            type = base_type;
        }
    }

    public static bool IsOpenTypeEq(this Type type, Type target)
        => type.IsGenericType && type.GetGenericTypeDefinition() == target;

    public static bool IsOpenTypeEqAny(this Type type, params Type[] targets)
    {
        if (!type.IsGenericType) return false;
        var t = type.GetGenericTypeDefinition();
        return targets.Any(a => a == t);
    }

    public static bool IsListBase(this Type type, [NotNullWhen(true)] out Type? itemType)
    {
        for (;;)
        {
            if (type.IsOpenTypeEq(typeof(List<>)))
            {
                itemType = type.GetGenericArguments()[0];
                return true;
            }

            var base_type = type.BaseType;
            if (base_type == null)
            {
                itemType = null;
                return false;
            }

            type = base_type;
        }
    }

    private static bool IsCollectionLikeInternal(
        this Type self,
        Type? target,
        out CollectionLikeKind Kind,
        [NotNullWhen(true)] out Type? itemType,
        out Type? keyType,
        out InterfaceMapping? mapping
    )
    {
        if (self.IsOpenTypeEq(typeof(IDictionary<,>)))
        {
            Kind = CollectionLikeKind.IDictionary;
            goto Dictionary_Like;
        }
        if (self.IsOpenTypeEq(typeof(IReadOnlyDictionary<,>)))
        {
            Kind = CollectionLikeKind.IReadOnlyDictionary;
            goto Dictionary_Like;
        }
        if (self.IsOpenTypeEq(typeof(ICollection<>)))
        {
            Kind = CollectionLikeKind.ICollection;
            goto Enumerable_Like;
        }
        if (self.IsOpenTypeEq(typeof(IReadOnlyCollection<>)))
        {
            Kind = CollectionLikeKind.IReadOnlyCollection;
            goto Enumerable_Like;
        }
        if (self.IsOpenTypeEq(typeof(IEnumerable<>)))
        {
            Kind = CollectionLikeKind.IEnumerable;
            goto Enumerable_Like;
        }

        Kind = CollectionLikeKind.None;
        keyType = null;
        itemType = null;
        mapping = null;
        return false;

        ok:
        mapping = target?.GetInterfaceMap(self);
        return true;

        Enumerable_Like:
        keyType = null;
        itemType = self.GetGenericArguments()[0];
        goto ok;

        Dictionary_Like:
        var generic = self.GetGenericArguments();
        keyType = generic[0];
        itemType = generic[1];
        goto ok;
    }

    public static bool IsCollectionLike(
        this Type type,
        out CollectionLikeKind Kind,
        [NotNullWhen(true)] out Type? itemType,
        out Type? keyType,
        out InterfaceMapping? mapping
    )
    {
        if (IsCollectionLikeInternal(type, null, out Kind, out itemType, out keyType, out mapping)) return true;

        var interfaces = type.GetInterfaces();

        foreach (var it in interfaces)
        {
            if (IsCollectionLikeInternal(it, type, out Kind, out itemType, out keyType, out mapping)) return true;
        }

        Kind = CollectionLikeKind.None;
        keyType = null;
        itemType = null;
        mapping = null;
        return false;
    }

    public static bool IsIEnumerable(this Type type)
    {
        if (type == typeof(IEnumerable)) return true;

        var interfaces =
            type.FindInterfaces((it, _) => it == typeof(IEnumerable), null);

        return interfaces.Length > 0;
    }

    public static bool IsICollection(this Type type)
    {
        if (type == typeof(ICollection)) return true;

        var interfaces =
            type.FindInterfaces((it, _) => it == typeof(ICollection), null);

        return interfaces.Length > 0;
    }

    public static bool IsIDictionary(this Type type)
    {
        if (type == typeof(IDictionary)) return true;

        var interfaces =
            type.FindInterfaces((it, _) => it == typeof(IDictionary), null);

        return interfaces.Length > 0;
    }
}
