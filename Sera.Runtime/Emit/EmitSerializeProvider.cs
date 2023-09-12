using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using Sera.Core.Impls;
using Sera.Runtime.Utils;

namespace Sera.Runtime.Emit;

internal partial class EmitSerializeProvider
{
    public readonly ConditionalWeakTable<Type, CacheStub> cache = new();

    public ISerialize<T> GetSerialize<T>()
    {
        var stub = GetSerializeStub(typeof(T), Thread.CurrentThread);
        if (stub.state != CacheStub.CreateState.Created)
        {
            stub.WaitCreate.WaitOne();
        }
        stub.EnsureDepsReady();
        return (ISerialize<T>)stub.ser_inst!;
    }

    internal class CacheStub
    {
        public enum CreateState
        {
            Idle,
            Creating,
            Created,
        }

        public volatile CreateState state = CreateState.Idle;
        public readonly Thread CreateThread;

        public readonly ManualResetEvent WaitType = new(false);
        public readonly ManualResetEvent WaitCreate = new(false);

        public Type? ser_type;
        public object? ser_inst;

        public CacheStub(Thread createThread)
        {
            CreateThread = createThread;
        }

        public Type? dep_container_type;
        public Dictionary<Type, CacheCellDeps>? deps;
        private volatile bool deps_ready;
        private readonly object deps_ready_lock = new();

        public void EnsureDepsReady()
        {
            if (deps == null || dep_container_type == null) return;
            if (deps_ready) return;
            lock (deps_ready_lock)
            {
                if (deps_ready) return;
                try
                {
                    foreach (var (_, dep) in deps!)
                    {
                        dep.Ready(dep_container_type);
                    }
                }
                finally
                {
                    deps_ready = true;
                }
            }
            foreach (var (_, dep) in deps)
            {
                dep.WaitInstReady();
            }
        }
    }

    internal record CacheCellDeps(
        FieldInfo Field, Type ImplType, Type RawImplType, Type ValueType, CacheStub? ImplCell, object? ImplInst,
        bool RefNullable)
    {
        public FieldInfo Field { get; set; } = Field;
        public Type ImplType { get; set; } = ImplType;
        public Type RawImplType { get; set; } = RawImplType;
        public Type ValueType { get; set; } = ValueType;

        private CacheStub? ImplCell { get; set; } = ImplCell;
        private object? ImplInst { get; set; } = ImplInst;

        /// <summary>
        /// If <c>true</c>
        /// <code>
        /// ImplType == typeof(NullableReferenceTypeImpl&lt;typeof(ImplInst)&gt;)
        /// </code>
        /// </summary>
        private bool RefNullable { get; set; } = RefNullable;

        internal void Ready(Type dep_container_type)
        {
            var field_name = Field.Name;
            var field = dep_container_type.GetField(field_name)!;

            if (ImplInst != null)
            {
                var inst = ImplInst;
                if (RefNullable)
                {
                    var ctor = ImplType.GetConstructor(BindingFlags.Public | BindingFlags.Instance,
                        new[] { RawImplType })!;
                    inst = ctor.Invoke(new[] { inst });
                }
                field.SetValue(null, inst);
            }
            else
            {
                ImplCell!.WaitCreate.WaitOne();
                var inst = ImplCell.ser_inst;
                if (RefNullable)
                {
                    var impl_type = field.FieldType;
                    var ctor = impl_type.GetConstructor(BindingFlags.Public | BindingFlags.Instance,
                        new[] { ImplCell.ser_type! })!;
                    inst = ctor.Invoke(new[] { inst });
                }
                field.SetValue(null, inst);
            }
        }

        internal void WaitInstReady()
        {
            if (ImplInst == null)
            {
                ImplCell!.EnsureDepsReady();
            }
        }
    }

    private CacheStub GetSerializeStub(Type type, Thread current_thread)
    {
        var stub = cache.GetValue(type, _ => new(current_thread));
        if (stub.CreateThread == current_thread)
        {
#pragma warning disable CS0420
            if (Interlocked.CompareExchange(ref Unsafe.As<CacheStub.CreateState, int>(ref stub.state),
                    (int)CacheStub.CreateState.Creating, (int)CacheStub.CreateState.Idle) ==
                (int)CacheStub.CreateState.Idle)
            {
                CreateSerialize(type, stub);
            }
#pragma warning restore CS0420
        }
        return stub;
    }

    private void CreateSerialize(Type type, CacheStub stub)
    {
        try
        {
            // todo other type
            GenStruct(type, stub);
        }
        finally
        {
            stub.state = CacheStub.CreateState.Created;
            stub.WaitCreate.Set();
        }
    }

    private void GenStruct(Type target, CacheStub stub)
    {
        var members = StructReflectionUtils.GetStructMembers(target, SerOrDe.Ser);
        if (target.IsVisible && members.All(m => m.Type.IsVisible))
        {
            GenPublicStruct(target, members, stub);
        }
        else
        {
            GenPrivateStruct(target, members, stub);
        }
    }
}
