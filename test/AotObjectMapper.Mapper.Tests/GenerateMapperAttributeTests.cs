using AotObjectMapper.Mapper.Tests.Utils;
using Shouldly;
using Xunit;

namespace AotObjectMapper.Mapper.Tests;

public class GenerateMapperAttributeTests
{
    private const string SourceCode =
    """
    using System;
    using AotObjectMapper.Abstractions.Attributes;
    using AotObjectMapper.Abstractions.Enums;
    using AotObjectMapper.Abstractions.Models;
    
    namespace TestNamespace.Generators
    {
        public partial class TestGenTop
        {
            [GenerateMapper]
            public partial class TestGen;
        }
    }
    """;

    [Fact]
    public void GenerateMapper_NoDiagnostics()
    {
        var results = GeneratorTestHelper.RunGenerator(SourceCode);

        results.Diagnostics.ShouldBeEmpty();
    }

    [Fact]
    public void GenerateMapper_CreatesOneTree()
    {
        var results = GeneratorTestHelper.RunGenerator(SourceCode);

        results.Result.GeneratedTrees.ShouldHaveSingleItem();
    }

    [Fact]
    public void GenerateMapper_CorrectFileName()
    {
        var results = GeneratorTestHelper.RunGenerator(SourceCode);

        results.Result.GeneratedTrees[0].FilePath.ShouldEndWith("TestNamespace.Generators.TestGenTop.TestGen.g.cs");
    }

    [Fact]
    public void GenerateMapper_CorrectNamespace()
    {
        var results = GeneratorTestHelper.RunGenerator(SourceCode);

        var assm = GeneratorTestHelper.CompileToAssembly(results.Compilation);

        var genClass = assm.GetType("TestNamespace.Generators.TestGenTop+TestGen", throwOnError: true);
        genClass.ShouldNotBeNull();

        genClass.Namespace.ShouldBe("TestNamespace.Generators");
    }

    [Fact]
    public void GenerateMapper_UtilsClassCreated()
    {
        var results = GeneratorTestHelper.RunGenerator(SourceCode);

        var assm = GeneratorTestHelper.CompileToAssembly(results.Compilation);

        var genClass = assm.GetType("TestNamespace.Generators.TestGenTop+TestGen", throwOnError: true);
        genClass.ShouldNotBeNull();

        var nestedTypes = genClass.GetNestedTypes();

        nestedTypes.ShouldHaveSingleItem();
        nestedTypes[0].Name.ShouldBe("Utils");
    }

    [Fact]
    public void GeneratedMapper_NoNamespace()
    {
        var results = GeneratorTestHelper.RunGenerator(
            """
            using System;
            using AotObjectMapper.Abstractions.Attributes;
            using AotObjectMapper.Abstractions.Enums;
            using AotObjectMapper.Abstractions.Models;

            public partial class TestGenTop
            {
                [GenerateMapper]
                public partial class TestGen;
            }
            """);

        results.Diagnostics.ShouldBeEmpty();

        Should.NotThrow(() => _ = GeneratorTestHelper.CompileToAssembly(results.Compilation));
    }
}