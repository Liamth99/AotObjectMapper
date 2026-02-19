using System.Linq;
using System.Threading.Tasks;
using AotObjectMapper.Mapper.Tests.Utils;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Shouldly;
using Xunit;

namespace AotObjectMapper.Mapper.Tests.GenerationTests;

public class MapAttributeTests
{
    private const string SourceCode =
        """
        using System;
        using AotObjectMapper.Abstractions.Attributes;
        using AotObjectMapper.Abstractions.Enums;
        using AotObjectMapper.Abstractions.Models;
        using TestNamespace.Models;
        
        namespace TestNamespace.Models
        {
            public class Source
            {
                public int Id { get; set; }
            }
            
            public class Destination
            {
                public int Id { get; set; }
            }
        }

        namespace TestNamespace.Generators
        {
            [GenerateMapper]
            [Map<Source, Destination>]
            public partial class TestGen;
        }
        """;

    [Fact]
    public void Map_NoDiagnostics()
    {
        var results = GeneratorTestHelper.RunGenerator(SourceCode);

        results.Diagnostics.ShouldBeEmpty();
    }

    [Fact]
    public void Map_CreatesMapMethod()
    {
        var results = GeneratorTestHelper.RunGenerator(SourceCode);

        var assm = GeneratorTestHelper.CompileToAssembly(results.Compilation);

        var genClass =  assm.GetType("TestNamespace.Generators.TestGen", throwOnError: true);
        genClass.ShouldNotBeNull();

        var methods = genClass.GetMethods();

        var mapMethod = methods.SingleOrDefault(x => x.Name == "Map");
        mapMethod.ShouldNotBeNull();

        mapMethod.IsStatic.ShouldBeTrue();
        mapMethod.IsPublic.ShouldBeTrue();
        mapMethod.ReturnType.FullName.ShouldBe("TestNamespace.Models.Destination");

        var parameters = mapMethod.GetParameters();
        parameters.Length.ShouldBe(2);

        parameters[0].ParameterType.FullName.ShouldBe("TestNamespace.Models.Source");
        parameters[1].ParameterType.FullName.ShouldBe("AotObjectMapper.Abstractions.Models.MapperContextBase");
    }

    [Fact]
    public async Task Map_MethodIncrementsAndDecrementsDepth()
    {
        var results = GeneratorTestHelper.RunGenerator(SourceCode);

        var rootNode         = await results.Result.GeneratedTrees[0].GetRootAsync(TestContext.Current.CancellationToken);
        var methodSyntaxNode = rootNode
                              .DescendantNodes()
                              .OfType<MethodDeclarationSyntax>()
                              .SingleOrDefault(x => x.Identifier.Text == "Map");

        methodSyntaxNode.ShouldNotBeNull();

        var statements = methodSyntaxNode.Body!.Statements;

        var incrementIndex = statements
                            .Select((s, i) => (Statement: s, Index: i))
                            .First(x => x.Statement.ToString().Contains("IncrementDepth"))
                            .Index;

        var decrementIndex = statements
                            .Select((s, i) => (Statement: s, Index: i))
                            .First(x => x.Statement.ToString().Contains("DecrementDepth"))
                            .Index;

        incrementIndex.ShouldBeLessThan(decrementIndex);
    }

    [Fact]
    public async Task Map_MethodAssignsId()
    {
        var results = GeneratorTestHelper.RunGenerator(SourceCode);

        var rootNode         = await results.Result.GeneratedTrees[0].GetRootAsync(TestContext.Current.CancellationToken);
        var methodSyntaxNode = rootNode
                              .DescendantNodes()
                              .OfType<MethodDeclarationSyntax>()
                              .SingleOrDefault(x => x.Identifier.Text == "Map");

        methodSyntaxNode.ShouldNotBeNull();

        var assignments = methodSyntaxNode.Body!
                                          .DescendantNodes()
                                          .OfType<AssignmentExpressionSyntax>()
                                          .ToArray();

        var assignment = assignments[1];

        assignment.Left.ToString().ShouldBe("dest.Id");
        assignment.Right.ToString().ShouldBe("src.Id");
    }
}