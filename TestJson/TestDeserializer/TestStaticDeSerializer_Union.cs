using System.Runtime.CompilerServices;
using Sera;
using Sera.Core;
using Sera.Core.Impls.De;
using Sera.Json;
using Sera.TaggedUnion;
using Sera.Utils;

namespace TestJson.TestDeserializer;

public partial class TestStaticDeSerializer_Union
{
    public struct Struct1
    {
        public int A { get; set; }
    }

    public readonly struct Struct1Impl : ISeraColion<Struct1>, IStructSeraColion<Struct1>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public R Collect<R, C>(ref C colctor, InType<Struct1>? t = null) where C : ISeraColctor<Struct1, R>
            => colctor.CStruct(this, new IdentityMapper<Struct1>(), new Type<Struct1>());

        public SeraFieldInfos? Fields
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => fields;
        }
        private static readonly SeraFieldInfos fields = new([new(nameof(Struct1.A), 0)]);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Struct1 Builder(string? name, Type<Struct1> b = default) => default;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public R CollectField<R, C>(ref C colctor, int field, string? name, long? key, Type<Struct1> b = default)
            where C : IStructSeraColctor<Struct1, R>
            => field switch
            {
                0 => colctor.CField(new PrimitiveImpl(), new FieldAEffector(), new Type<int>()),
                _ => colctor.CNone(),
            };

        private readonly struct FieldAEffector : ISeraEffector<Struct1, int>
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Effect(ref Struct1 target, int value)
                => target.A = value;
        }
    }

    [Union]
    public partial struct Union1
    {
        [UnionTemplate]
        private interface Template
        {
            int A();
            string B();
            void C();
            Struct1 Struct();
        }
    }

    public readonly struct Union1Impl(UnionStyle? style = null) : ISeraColion<Union1>, IVariantsSeraColion<Union1>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public R Collect<R, C>(ref C colctor, InType<Union1>? t = null) where C : ISeraColctor<Union1, R>
            => colctor.CUnionVariants(this, new IdentityMapper<Union1>(), new Type<Union1>(), style);

        public SeraVariantInfos Variants
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _variants;
        }

        private static readonly SeraVariantInfos _variants = new([
            new("A", VariantTag.Create(0)),
            new("B", VariantTag.Create(1)),
            new("C", VariantTag.Create(2)),
            new("Struct", VariantTag.Create(3)),
        ], VariantTagKind.Int32);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public R CollectVariant<R, C>(ref C colctor, int variant) where C : IVariantSeraColctor<Union1, R>
            => variant switch
            {
                0 => colctor.CVariantValue(new PrimitiveImpl(), new Union1_A_Mapper(), new Type<int>()),
                1 => colctor.CVariantValue(new StringImpl(), new Union1_B_Mapper(), new Type<string>()),
                2 => colctor.CVariant(new Union1_C_Ctor()),
                3 => colctor.CVariantStruct(new Struct1Impl(), new Union1_Struct_Mapper(), new Type<Struct1>()),
                _ => colctor.CNone(),
            };

        private readonly struct Union1_A_Mapper : ISeraMapper<int, Union1>
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Union1 Map(int value, InType<Union1>? u = null)
                => Union1.MakeA(value);
        }

        private readonly struct Union1_B_Mapper : ISeraMapper<string, Union1>
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Union1 Map(string value, InType<Union1>? u = null)
                => Union1.MakeB(value);
        }

        private readonly struct Union1_C_Ctor : ISeraCtor<Union1>
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Union1 Ctor(InType<Union1>? t = null)
                => Union1.MakeC();
        }

        private readonly struct Union1_Struct_Mapper : ISeraMapper<Struct1, Union1>
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Union1 Map(Struct1 value, InType<Union1>? u = null)
                => Union1.MakeStruct(value);
        }
    }

    [Test]
    public void Test1()
    {
        var json = "{\"A\":123}";

        var val = SeraJson.Deserializer
            .Deserialize<Union1>()
            .Use(new Union1Impl())
            .Static.From.String(json);

        Console.WriteLine(val);

        Assert.That(val, Is.EqualTo(Union1.MakeA(123)));
    }

    [Test]
    public void TestAdjacent1()
    {
        var json = "{\"t\":\"A\",\"c\":123}";

        var val = SeraJson.Deserializer
            .Deserialize<Union1>()
            .Use(new Union1Impl(UnionStyle.Default with { Format = UnionFormat.Adjacent }))
            .Static.From.String(json);

        Console.WriteLine(val);

        Assert.That(val, Is.EqualTo(Union1.MakeA(123)));
    }

    [Test]
    public void TestAdjacent2()
    {
        var json = "{\"c\":\"asd\",\"t\":1}";

        var val = SeraJson.Deserializer
            .Deserialize<Union1>()
            .Use(new Union1Impl(UnionStyle.Default with { Format = UnionFormat.Adjacent }))
            .Static.From.String(json);

        Console.WriteLine(val);

        Assert.That(val, Is.EqualTo(Union1.MakeB("asd")));
    }

    [Test]
    public void TestInternal1()
    {
        var json = "{\"type\":\"A\",\"value\":123}";

        var val = SeraJson.Deserializer
            .Deserialize<Union1>()
            .Use(new Union1Impl(UnionStyle.Default with { Format = UnionFormat.Internal }))
            .Static.From.String(json);

        Console.WriteLine(val);

        Assert.That(val, Is.EqualTo(Union1.MakeA(123)));
    }

    [Test]
    public void TestInternal2()
    {
        var json = "{\"type\":\"Struct\",\"A\":123}";

        var val = SeraJson.Deserializer
            .Deserialize<Union1>()
            .Use(new Union1Impl(UnionStyle.Default with { Format = UnionFormat.Internal }))
            .Static.From.String(json);

        Console.WriteLine(val);

        Assert.That(val, Is.EqualTo(Union1.MakeStruct(new() { A = 123 })));
    }

    [Test]
    public void TestInternal3()
    {
        var json = "{\"A\":123,\"type\":\"Struct\"}";

        var val = SeraJson.Deserializer
            .Deserialize<Union1>()
            .Use(new Union1Impl(UnionStyle.Default with { Format = UnionFormat.Internal }))
            .Static.From.String(json);

        Console.WriteLine(val);

        Assert.That(val, Is.EqualTo(Union1.MakeStruct(new() { A = 123 })));
    }

    [Test]
    public void TestInternal4()
    {
        var json = "{\"value\":123,\"type\":\"A\"}";

        var val = SeraJson.Deserializer
            .Deserialize<Union1>()
            .Use(new Union1Impl(UnionStyle.Default with { Format = UnionFormat.Internal }))
            .Static.From.String(json);

        Console.WriteLine(val);

        Assert.That(val, Is.EqualTo(Union1.MakeA(123)));
    }
}
