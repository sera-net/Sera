using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using Sera.Core.Impls;
using Sera.Runtime.Utils;

namespace Sera.Runtime.Emit;

internal partial class EmitSerializeProvider
{
    public readonly ConditionalWeakTable<Type, ConcurrentDictionary<TypeMeta, CacheStub>> cache = new();

    public ISerialize<T> GetSerialize<T>()
    {
        var stub = GetSerializeStub(typeof(T), Thread.CurrentThread);
        stub.EnsureInstProvided();
        stub.EnsureDepsReady();
        return (ISerialize<T>)stub.SerInst;
    }

    internal class CacheStub
    {
        private enum CreateState
        {
            Idle,
            Creating,
            Created,
        }

        private volatile CreateState state = CreateState.Idle;
        public Thread CreateThread { get; }

        // todo ManualResetEvent => Task
        private readonly ManualResetEvent WaitType = new(false);
        private readonly ManualResetEvent WaitReady = new(false);
        private readonly ManualResetEvent WaitCreate = new(false);

        private Type? ser_type;
        private object? ser_inst;
        private Func<object>? late_inst;

        public Type SerType => ser_type!;
        public object SerInst => ser_inst!;

        public CacheStub(Thread createThread)
        {
            CreateThread = createThread;
        }

        private Type? dep_container_type;
        private IEnumerable<CacheStubDeps>? deps;
        private volatile bool deps_ready;
        private readonly object deps_ready_lock = new();

#pragma warning disable CS0420
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryEnterCreating() =>
            Interlocked.CompareExchange(ref Unsafe.As<CreateState, int>(ref state),
                (int)CreateState.Creating, (int)CreateState.Idle) == (int)CreateState.Idle;
#pragma warning restore CS0420

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WaitTypeProvided()
        {
            WaitType.WaitOne();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WaitInstReady()
        {
            WaitReady.WaitOne();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WaitInstProvided()
        {
            WaitInstReady();
            if (state != CreateState.Created)
            {
                // todo lock
                if (late_inst == null) throw new Exception("internal state error");
                ser_inst = late_inst.Invoke();
                MarkInstProvided();
                return;
            }
            WaitCreate.WaitOne();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void EnsureInstProvided()
        {
            if (state != CreateState.Created)
            {
                WaitInstProvided();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void MarkTypeProvided()
        {
            WaitType.Set();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void MarkInstProvided()
        {
            state = CreateState.Created;
            WaitReady.Set();
            WaitCreate.Set();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void MarkInstReady()
        {
            WaitCreate.Set();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ProvideType(Type type)
        {
            ser_type = type;
            MarkTypeProvided();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ProvideInst(object inst)
        {
            ser_inst = inst;
            MarkInstProvided();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ProvideLateInst(Func<object> init)
        {
            late_inst = init;
            WaitReady.Set();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ProvideDeps(Type dep_container_type, IEnumerable<CacheStubDeps> deps)
        {
            this.dep_container_type = dep_container_type;
            this.deps = deps;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ProvideDeps(TheDep dep) => ProvideDeps(dep.dep_container_type, dep.deps);

        public void EnsureDepsReady()
        {
            if (deps == null || dep_container_type == null) return;
            if (deps_ready) return;
            lock (deps_ready_lock)
            {
                if (deps_ready) return;
                try
                {
                    foreach (var dep in deps!)
                    {
                        dep.Ready(dep_container_type);
                    }
                }
                finally
                {
                    deps_ready = true;
                }
            }
            foreach (var dep in deps)
            {
                dep.WaitInstReady();
            }
        }
    }

    internal enum DepPlaceKind : byte
    {
        Field,
        Property,
        LateField,
        LateProperty,
    }

    internal record struct DepPlace
    {
        private _union_ _union;
        public DepPlaceKind Kind { get; }

        public FieldInfo Field => _union._field;
        public PropertyInfo Property => _union._property;
        public string Name => _union._name;

        [StructLayout(LayoutKind.Explicit)]
        private struct _union_
        {
            [FieldOffset(0)]
            public FieldInfo _field;
            [FieldOffset(0)]
            public PropertyInfo _property;
            [FieldOffset(0)]
            public string _name;
        }

        private DepPlace(DepPlaceKind kind, _union_ union)
        {
            Kind = kind;
            _union = union;
        }

        public static DepPlace MakeField(FieldInfo field) =>
            new(DepPlaceKind.Field, new _union_ { _field = field });

        public static DepPlace MakeProperty(PropertyInfo property) =>
            new(DepPlaceKind.Property, new _union_ { _property = property });

        public static DepPlace MakeLateField(string FieldName) => new(DepPlaceKind.LateField,
            new _union_ { _name = FieldName });

        public static DepPlace MakeLateProperty(string PropertyName) => new(DepPlaceKind.LateProperty,
            new _union_ { _name = PropertyName });
    }

    /// <summary>
    /// Nesting order
    /// <list type="number">
    ///     <item><term>ImplType</term></item>
    ///     <item><term><see cref="NullableReferenceTypeImpl{T,ST}"/></term></item>
    ///     <item><term><see cref="Box{T}"/></term></item>
    /// </list>
    /// </summary>
    /// <param name="RefNullable">Whether the type is nested within a <see cref="NullableReferenceTypeImpl{T,ST}"/></param>
    /// <param name="Boxed">Whether the type is nested within a <see cref="Box{T}"/></param>
    internal record CacheStubDeps(
        int index, int[] rawIndexes,
        DepPlace Place, Type ImplType, Type RawImplType, Type ValueType,
        CacheStub? ImplStub, object? ImplInst, bool RefNullable, Type? BoxedType, bool Boxed
    )
    {
        public DepPlace Place { get; set; } = Place;
        public Type ImplType { get; set; } = ImplType;
        public Type? BoxedType { get; set; } = BoxedType;
        public Type RawImplType { get; set; } = RawImplType;
        public Type ValueType { get; set; } = ValueType;

        private CacheStub? ImplStub { get; set; } = ImplStub;
        private object? ImplInst { get; set; } = ImplInst;

        public Type RuntimeImplType => ImplStub?.SerType ?? ImplType;

        /// <summary>
        /// If <c>true</c>
        /// <code>
        /// ImplType == typeof(NullableReferenceTypeImpl&lt;typeof(ImplInst)&gt;)
        /// </code>
        /// </summary>
        private bool RefNullable { get; set; } = RefNullable;

        internal void Ready(Type dep_container_type)
        {
            var inst = Ready();

            if (Place.Kind == DepPlaceKind.Field)
            {
                var name = Place.Field.Name;
                var field = dep_container_type.GetField(name)!;

                field.SetValue(null, inst);
            }
            else if (Place.Kind == DepPlaceKind.Property)
            {
                var name = Place!.Property.Name;
                var property = dep_container_type.GetProperty(name)!;

                property.SetValue(null, inst);
            }
            else if (Place.Kind == DepPlaceKind.LateField)
            {
                var name = Place!.Name;
                var field = dep_container_type.GetField(name)!;

                field.SetValue(null, inst);
            }
            else if (Place.Kind == DepPlaceKind.LateProperty)
            {
                var name = Place!.Name;
                var property = dep_container_type.GetProperty(name)!;

                property.SetValue(null, inst);
            }
            else throw new ArgumentOutOfRangeException();
        }

        private object? Ready()
        {
            var inst = ImplInst;
            var current_impl_type = RawImplType;
            if (inst == null)
            {
                ImplStub!.WaitInstProvided();
                inst = ImplStub.SerInst;
                current_impl_type = ImplStub.SerType;
            }
            if (RefNullable)
            {
                var ref_type = typeof(NullableReferenceTypeImpl<,>).MakeGenericType(ValueType, current_impl_type);
                var ctor = ref_type.GetConstructor(
                    BindingFlags.Public | BindingFlags.Instance,
                    new[] { current_impl_type }
                )!;
                inst = ctor.Invoke(new[] { inst });
                current_impl_type = ref_type;
            }
            if (Boxed)
            {
                var boxed_type = typeof(Box<>).MakeGenericType(current_impl_type);
                var ctor = boxed_type.GetConstructor(
                    BindingFlags.Public | BindingFlags.Instance,
                    new[] { current_impl_type }
                )!;
                inst = ctor.Invoke(new[] { inst });
                current_impl_type = boxed_type;
            }
            return inst;
        }

        internal void WaitInstReady()
        {
            if (ImplInst == null)
            {
                ImplStub!.EnsureDepsReady();
            }
        }
    }

    private CacheStub GetSerializeStub(Type type, Thread current_thread)
        => GetSerializeStub(type, null, current_thread);

    private CacheStub GetSerializeStub(Type type, NullabilityMeta? nullabilityMeta, Thread current_thread)
        => GetSerializeStub(TypeMetas.GetTypeMeta(type, nullabilityMeta), current_thread);

    private CacheStub GetSerializeStub(TypeMeta target, Thread current_thread)
    {
        var cache_group = cache.GetValue(target.Type, static _ => new());
        var stub = cache_group.GetOrAdd(target, static (_, current_thread) => new(current_thread), current_thread);
        if (stub.CreateThread == current_thread)
        {
            if (stub.TryEnterCreating())
            {
                CreateSerialize(target, stub);
            }
        }
        return stub;
    }

    private void CreateSerialize(TypeMeta target, CacheStub stub)
    {
        try
        {
            // todo other type
            if (target.IsArray)
            {
                GenArray(target, stub);
            }
            else if (target.IsEnum)
            {
                GenEnum(target, stub);
            }
            else if (target.IsTuple(out var is_value_tuple))
            {
                GenTuple(target, is_value_tuple, stub);
            }
            else
            {
                GenStruct(target, stub);
            }
        }
        finally
        {
            stub.MarkInstReady();
        }
    }
}
