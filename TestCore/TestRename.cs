using Sera.Utils;

namespace TestCore;

public class TestRename
{
    #region Split

    [Test]
    public void TestSplit1()
    {
        var name = "asdAsd123Z_qwe iop-bnm _- 阿斯顿😂😂阿斯顿_";
        var words = new List<SeraWord>();
        foreach (var word in SeraRename.SplitWord(name))
        {
            words.Add(word);
            Console.WriteLine(word);
        }
        var expected = new SeraWord[]
        {
            new("asd".AsMemory(), SeraWordKind.Word),
            new("Asd".AsMemory(), SeraWordKind.Word),
            new("123".AsMemory(), SeraWordKind.Number),
            new("Z".AsMemory(), SeraWordKind.Word),
            new("_".AsMemory(), SeraWordKind.Split),
            new("qwe".AsMemory(), SeraWordKind.Word),
            new(" ".AsMemory(), SeraWordKind.Space),
            new("iop".AsMemory(), SeraWordKind.Word),
            new("-".AsMemory(), SeraWordKind.Split),
            new("bnm".AsMemory(), SeraWordKind.Word),
            new(" ".AsMemory(), SeraWordKind.Space),
            new("_".AsMemory(), SeraWordKind.Split),
            new("-".AsMemory(), SeraWordKind.Split),
            new(" ".AsMemory(), SeraWordKind.Space),
            new("阿斯顿😂😂阿斯顿".AsMemory(), SeraWordKind.Word),
            new("_".AsMemory(), SeraWordKind.Split),
        };
        Console.WriteLine();
        Assert.That(words.Count, Is.EqualTo(expected.Length));
        for (var i = 0; i < expected.Length; i++)
        {
            Console.WriteLine(words[i]);
            Assert.Multiple(() =>
            {
                Assert.That(words[i].Kind, Is.EqualTo(expected[i].Kind));
                Assert.That(words[i].Word.ToString(), Is.EqualTo(expected[i].Word.ToString()));
            });
        }
    }

    #endregion

    #region PascalCase

    [Test]
    public void TestToPascalCase1()
    {
        var name = "asdAsd123Z_qwe iop-bnm vector3d";
        var r = SeraRename.ToPascalCase(name);
        Console.WriteLine(r);
        Assert.That(r, Is.EqualTo("AsdAsd123ZQweIopBnmVector3D"));
    }

    [Test]
    public void TestToPascalCase2()
    {
        var name = "123asd";
        var r = SeraRename.ToPascalCase(name);
        Console.WriteLine(r);
        Assert.That(r, Is.EqualTo("123Asd"));
    }

    [Test]
    public void TestToPascalCase3()
    {
        var name = "阿斯顿😂😂阿斯顿";
        var r = SeraRename.ToPascalCase(name);
        Console.WriteLine(r);
        Assert.That(r, Is.EqualTo("阿斯顿😂😂阿斯顿"));
    }

    [Test]
    public void TestToPascalCase4()
    {
        var name = "asd_qwe";
        var r = SeraRename.ToPascalCase(name);
        Console.WriteLine(r);
        Assert.That(r, Is.EqualTo("AsdQwe"));
    }

    [Test]
    public void TestToPascalCase5()
    {
        var name = "asd__qwe";
        var r = SeraRename.ToPascalCase(name);
        Console.WriteLine(r);
        Assert.That(r, Is.EqualTo("Asd__Qwe"));
    }

    [Test]
    public void TestToPascalCase6()
    {
        var name = "_asd_";
        var r = SeraRename.ToPascalCase(name);
        Console.WriteLine(r);
        Assert.That(r, Is.EqualTo("_Asd_"));
    }

    #endregion

    #region CamelCase

    [Test]
    public void TestToCamelCase1()
    {
        var name = "asdAsd123Z_qwe iop-bnm vector3d";
        var r = SeraRename.ToCamelCase(name);
        Console.WriteLine(r);
        Assert.That(r, Is.EqualTo("asdAsd123ZQweIopBnmVector3D"));
    }

    [Test]
    public void TestToCamelCase2()
    {
        var name = "123asd";
        var r = SeraRename.ToCamelCase(name);
        Console.WriteLine(r);
        Assert.That(r, Is.EqualTo("123Asd"));
    }

    [Test]
    public void TestToCamelCase3()
    {
        var name = "阿斯顿😂😂阿斯顿";
        var r = SeraRename.ToCamelCase(name);
        Console.WriteLine(r);
        Assert.That(r, Is.EqualTo("阿斯顿😂😂阿斯顿"));
    }

    [Test]
    public void TestToCamelCase4()
    {
        var name = "asd_qwe";
        var r = SeraRename.ToCamelCase(name);
        Console.WriteLine(r);
        Assert.That(r, Is.EqualTo("asdQwe"));
    }

    [Test]
    public void TestToCamelCase5()
    {
        var name = "asd__qwe";
        var r = SeraRename.ToCamelCase(name);
        Console.WriteLine(r);
        Assert.That(r, Is.EqualTo("asd__Qwe"));
    }

    [Test]
    public void TestToCamelCase6()
    {
        var name = "_asd_";
        var r = SeraRename.ToCamelCase(name);
        Console.WriteLine(r);
        Assert.That(r, Is.EqualTo("_asd_"));
    }

    #endregion

    #region LowerCase

    [Test]
    public void TestToLowerCase1()
    {
        var name = "asdAsd123Z_qwe iop-bnm vector3d";
        var r = SeraRename.ToLowerCase(name);
        Console.WriteLine(r);
        Assert.That(r, Is.EqualTo("asdasd123zqweiopbnmvector3d"));
    }

    [Test]
    public void TestToLowerCase2()
    {
        var name = "123asd";
        var r = SeraRename.ToLowerCase(name);
        Console.WriteLine(r);
        Assert.That(r, Is.EqualTo("123asd"));
    }

    [Test]
    public void TestToLowerCase3()
    {
        var name = "阿斯顿😂😂阿斯顿";
        var r = SeraRename.ToLowerCase(name);
        Console.WriteLine(r);
        Assert.That(r, Is.EqualTo("阿斯顿😂😂阿斯顿"));
    }

    [Test]
    public void TestToLowerCase4()
    {
        var name = "asd_qwe";
        var r = SeraRename.ToLowerCase(name);
        Console.WriteLine(r);
        Assert.That(r, Is.EqualTo("asdqwe"));
    }

    [Test]
    public void TestToLowerCase5()
    {
        var name = "asd__qwe";
        var r = SeraRename.ToLowerCase(name);
        Console.WriteLine(r);
        Assert.That(r, Is.EqualTo("asd__qwe"));
    }

    [Test]
    public void TestToLowerCase6()
    {
        var name = "_asd_";
        var r = SeraRename.ToLowerCase(name);
        Console.WriteLine(r);
        Assert.That(r, Is.EqualTo("_asd_"));
    }

    #endregion

    #region UpperCase

    [Test]
    public void TestToUpperCase1()
    {
        var name = "asdAsd123Z_qwe iop-bnm vector3d";
        var r = SeraRename.ToUpperCase(name);
        Console.WriteLine(r);
        Assert.That(r, Is.EqualTo("ASDASD123ZQWEIOPBNMVECTOR3D"));
    }

    [Test]
    public void TestToUpperCase2()
    {
        var name = "123asd";
        var r = SeraRename.ToUpperCase(name);
        Console.WriteLine(r);
        Assert.That(r, Is.EqualTo("123ASD"));
    }

    [Test]
    public void TestToUpperCase3()
    {
        var name = "阿斯顿😂😂阿斯顿";
        var r = SeraRename.ToUpperCase(name);
        Console.WriteLine(r);
        Assert.That(r, Is.EqualTo("阿斯顿😂😂阿斯顿"));
    }

    [Test]
    public void TestToUpperCase4()
    {
        var name = "asd_qwe";
        var r = SeraRename.ToUpperCase(name);
        Console.WriteLine(r);
        Assert.That(r, Is.EqualTo("ASDQWE"));
    }

    [Test]
    public void TestToUpperCase5()
    {
        var name = "asd__qwe";
        var r = SeraRename.ToUpperCase(name);
        Console.WriteLine(r);
        Assert.That(r, Is.EqualTo("ASD__QWE"));
    }

    [Test]
    public void TestToUpperCase6()
    {
        var name = "_asd_";
        var r = SeraRename.ToUpperCase(name);
        Console.WriteLine(r);
        Assert.That(r, Is.EqualTo("_ASD_"));
    }

    #endregion

    #region SnakeCase

    [Test]
    public void TestToSnakeCase1()
    {
        var name = "asdAsd123Z_qwe iop-bnm vector3d";
        var r = SeraRename.ToSnakeCase(name);
        Console.WriteLine(r);
        Assert.That(r, Is.EqualTo("asd_asd123_z_qwe_iop_bnm_vector3d"));
    }

    [Test]
    public void TestToSnakeCase2()
    {
        var name = "123asd456";
        var r = SeraRename.ToSnakeCase(name);
        Console.WriteLine(r);
        Assert.That(r, Is.EqualTo("123asd456"));
    }

    [Test]
    public void TestToSnakeCase2b()
    {
        var name = "123Asd456";
        var r = SeraRename.ToSnakeCase(name);
        Console.WriteLine(r);
        Assert.That(r, Is.EqualTo("123_asd456"));
    }

    [Test]
    public void TestToSnakeCase3()
    {
        var name = "阿斯顿😂😂阿斯顿";
        var r = SeraRename.ToSnakeCase(name);
        Console.WriteLine(r);
        Assert.That(r, Is.EqualTo("阿斯顿😂😂阿斯顿"));
    }

    [Test]
    public void TestToSnakeCase4()
    {
        var name = "asd_qwe";
        var r = SeraRename.ToSnakeCase(name);
        Console.WriteLine(r);
        Assert.That(r, Is.EqualTo("asd_qwe"));
    }

    [Test]
    public void TestToSnakeCase5()
    {
        var name = "asd__qwe";
        var r = SeraRename.ToSnakeCase(name);
        Console.WriteLine(r);
        Assert.That(r, Is.EqualTo("asd__qwe"));
    }

    [Test]
    public void TestToSnakeCase6()
    {
        var name = "_asd_";
        var r = SeraRename.ToSnakeCase(name);
        Console.WriteLine(r);
        Assert.That(r, Is.EqualTo("_asd_"));
    }

    #endregion

    #region UpperSnakeCase

    [Test]
    public void TestToUpperSnakeCase1()
    {
        var name = "asdAsd123Z_qwe iop-bnm vector3d";
        var r = SeraRename.ToUpperSnakeCase(name);
        Console.WriteLine(r);
        Assert.That(r, Is.EqualTo("ASD_ASD123_Z_QWE_IOP_BNM_VECTOR3D"));
    }

    [Test]
    public void TestToUpperSnakeCase2()
    {
        var name = "123asd456";
        var r = SeraRename.ToUpperSnakeCase(name);
        Console.WriteLine(r);
        Assert.That(r, Is.EqualTo("123ASD456"));
    }

    [Test]
    public void TestToUpperSnakeCase2b()
    {
        var name = "123Asd456";
        var r = SeraRename.ToUpperSnakeCase(name);
        Console.WriteLine(r);
        Assert.That(r, Is.EqualTo("123_ASD456"));
    }

    [Test]
    public void TestToUpperSnakeCase3()
    {
        var name = "阿斯顿😂😂阿斯顿";
        var r = SeraRename.ToUpperSnakeCase(name);
        Console.WriteLine(r);
        Assert.That(r, Is.EqualTo("阿斯顿😂😂阿斯顿"));
    }

    [Test]
    public void TestToUpperSnakeCase4()
    {
        var name = "asd_qwe";
        var r = SeraRename.ToUpperSnakeCase(name);
        Console.WriteLine(r);
        Assert.That(r, Is.EqualTo("ASD_QWE"));
    }

    [Test]
    public void TestToUpperSnakeCase5()
    {
        var name = "asd__qwe";
        var r = SeraRename.ToUpperSnakeCase(name);
        Console.WriteLine(r);
        Assert.That(r, Is.EqualTo("ASD__QWE"));
    }

    [Test]
    public void TestToUpperSnakeCase6()
    {
        var name = "_asd_";
        var r = SeraRename.ToUpperSnakeCase(name);
        Console.WriteLine(r);
        Assert.That(r, Is.EqualTo("_ASD_"));
    }

    #endregion
    
    #region KebabCase

    [Test]
    public void TestToKebabCase1()
    {
        var name = "asdAsd123Z_qwe iop-bnm vector3d";
        var r = SeraRename.ToKebabCase(name);
        Console.WriteLine(r);
        Assert.That(r, Is.EqualTo("asd-asd123-z-qwe-iop-bnm-vector3d"));
    }

    [Test]
    public void TestToKebabCase2()
    {
        var name = "123asd456";
        var r = SeraRename.ToKebabCase(name);
        Console.WriteLine(r);
        Assert.That(r, Is.EqualTo("123asd456"));
    }

    [Test]
    public void TestToKebabCase2b()
    {
        var name = "123Asd456";
        var r = SeraRename.ToKebabCase(name);
        Console.WriteLine(r);
        Assert.That(r, Is.EqualTo("123-asd456"));
    }

    [Test]
    public void TestToKebabCase3()
    {
        var name = "阿斯顿😂😂阿斯顿";
        var r = SeraRename.ToKebabCase(name);
        Console.WriteLine(r);
        Assert.That(r, Is.EqualTo("阿斯顿😂😂阿斯顿"));
    }

    [Test]
    public void TestToKebabCase4()
    {
        var name = "asd_qwe";
        var r = SeraRename.ToKebabCase(name);
        Console.WriteLine(r);
        Assert.That(r, Is.EqualTo("asd-qwe"));
    }

    [Test]
    public void TestToKebabCase5()
    {
        var name = "asd__qwe";
        var r = SeraRename.ToKebabCase(name);
        Console.WriteLine(r);
        Assert.That(r, Is.EqualTo("asd--qwe"));
    }

    [Test]
    public void TestToKebabCase6()
    {
        var name = "_asd_";
        var r = SeraRename.ToKebabCase(name);
        Console.WriteLine(r);
        Assert.That(r, Is.EqualTo("-asd-"));
    }

    #endregion
    
    #region UpperKebabCase

    [Test]
    public void TestToUpperKebabCase1()
    {
        var name = "asdAsd123Z_qwe iop-bnm vector3d";
        var r = SeraRename.ToUpperKebabCase(name);
        Console.WriteLine(r);
        Assert.That(r, Is.EqualTo("ASD-ASD123-Z-QWE-IOP-BNM-VECTOR3D"));
    }

    [Test]
    public void TestToUpperKebabCase2()
    {
        var name = "123asd456";
        var r = SeraRename.ToUpperKebabCase(name);
        Console.WriteLine(r);
        Assert.That(r, Is.EqualTo("123ASD456"));
    }

    [Test]
    public void TestToUpperKebabCase2b()
    {
        var name = "123Asd456";
        var r = SeraRename.ToUpperKebabCase(name);
        Console.WriteLine(r);
        Assert.That(r, Is.EqualTo("123-ASD456"));
    }

    [Test]
    public void TestToUpperKebabCaseCase3()
    {
        var name = "阿斯顿😂😂阿斯顿";
        var r = SeraRename.ToUpperKebabCase(name);
        Console.WriteLine(r);
        Assert.That(r, Is.EqualTo("阿斯顿😂😂阿斯顿"));
    }

    [Test]
    public void TestToUpperKebabCaseCase4()
    {
        var name = "asd_qwe";
        var r = SeraRename.ToUpperKebabCase(name);
        Console.WriteLine(r);
        Assert.That(r, Is.EqualTo("ASD-QWE"));
    }

    [Test]
    public void TestToUpperKebabCaseCase5()
    {
        var name = "asd__qwe";
        var r = SeraRename.ToUpperKebabCase(name);
        Console.WriteLine(r);
        Assert.That(r, Is.EqualTo("ASD--QWE"));
    }

    [Test]
    public void TestToUpperKebabCaseCase6()
    {
        var name = "_asd_";
        var r = SeraRename.ToUpperKebabCase(name);
        Console.WriteLine(r);
        Assert.That(r, Is.EqualTo("-ASD-"));
    }

    #endregion
}
