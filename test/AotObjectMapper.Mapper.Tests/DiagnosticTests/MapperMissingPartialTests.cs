using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using Xunit;

namespace AotObjectMapper.Mapper.Tests.DiagnosticTests;

public class MapperMissingPartialTests
{
    [Fact]
    public void GenerateMapperAttribute_NoPartialKeyWord_AOM105()
    {
        const string code =
        """
        public partial class Top
        {
            [GenerateMapper]
            public class {|#0:Mapper|};
        }
        """;

        var test = new CSharpSourceGeneratorTest<MapperGenerator, DefaultVerifier>()
        {
            TestState = { Sources = { code, }, },
        };

        test.ExpectedDiagnostics.Add(DiagnosticResult.CompilerError(AOMDiagnostics.ClassRequiresPartialKeywordId).WithLocation(0).WithArguments("Mapper"));
    }

    [Fact]
    public void GenerateMapperAttribute_ParentClassNoPartialKeyWord_AOM105()
    {
        const string code =
        """
        public class {|#0:Top|}
        {
            [GenerateMapper]
            public partial class Mapper;
        }
        """;

        var test = new CSharpSourceGeneratorTest<MapperGenerator, DefaultVerifier>()
        {
            TestState = { Sources = { code, }, },
        };

        test.ExpectedDiagnostics.Add(DiagnosticResult.CompilerError(AOMDiagnostics.ClassRequiresPartialKeywordId).WithLocation(0).WithArguments("Top"));
    }

    [Fact]
    public void GenerateMapperAttribute_MultipleClassesNoPartialKeyWord_AOM105()
    {
        const string code =
        """
        public class {|#0:Top|}
        {
            [GenerateMapper]
            public class {|#1:Mapper|};
        }
        """;

        var test = new CSharpSourceGeneratorTest<MapperGenerator, DefaultVerifier>()
        {
            TestState = { Sources = { code, }, },
        };

        test.ExpectedDiagnostics.Add(DiagnosticResult.CompilerError(AOMDiagnostics.ClassRequiresPartialKeywordId).WithLocation(0).WithArguments("Top"));
        test.ExpectedDiagnostics.Add(DiagnosticResult.CompilerError(AOMDiagnostics.ClassRequiresPartialKeywordId).WithLocation(1).WithArguments("Mapper"));
    }
}