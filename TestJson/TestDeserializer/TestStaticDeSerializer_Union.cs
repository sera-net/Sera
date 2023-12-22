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
    [Union]
    public partial struct Union1
    {
        [UnionTemplate]
        private interface Template
        {
            int A();
            string B();
            void C();
        }
    }

    public readonly struct Union1Impl : ISeraColion<Union1>, IVariantsSeraColion<Union1>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public R Collect<R, C>(ref C colctor, InType<Union1>? t = null) where C : ISeraColctor<Union1, R>
            => colctor.CUnionVariants(this, new IdentityMapper<Union1>(), new Type<Union1>());

        public SeraVariantInfos Variants
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _variants;
        }

        private static readonly SeraVariantInfos _variants = new([
            new("A", VariantTag.Create(0)),
            new("B", VariantTag.Create(1)),
        ], VariantTagKind.Int32);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public R CollectVariant<R, C>(ref C colctor, int variant) where C : IVariantSeraColctor<Union1, R>
            => variant switch
            {
                0 => colctor.CVariantValue(new PrimitiveImpl(), new Union1_A_Mapper(), new Type<int>()),
                1 => colctor.CVariantValue(new StringImpl(), new Union1_B_Mapper(), new Type<string>()),
                2 => colctor.CVariant(new Union1_C_Ctor()),
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
}
