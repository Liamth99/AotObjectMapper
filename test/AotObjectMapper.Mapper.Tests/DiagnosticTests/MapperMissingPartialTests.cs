using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using Xunit;

namespace AotObjectMapper.Mapper.Tests.DiagnosticTests;

public class MapperMissingPartialTests : AOMVerifierBase
{
    [Fact]
    public async Task GenerateMapperAttribute_NoPartialKeyWord_AOM105()
    {
        const string code =
        """
        using System;
        using AotObjectMapper.Abstractions.Attributes;
        using AotObjectMapper.Abstractions.Enums;
        using AotObjectMapper.Abstractions.Models;
        
        public partial class Top
        {
            [GenerateMapper]
            public class {|#0:Mapper|};
        }
        """;

        await VerifyGeneratorDiagnosticsAsync(code, [DiagnosticResult.CompilerError(AOMDiagnostics.ClassRequiresPartialKeywordId).WithLocation(0).WithArguments("Mapper")], cancellationToken: TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task GenerateMapperAttribute_ParentClassNoPartialKeyWord_AOM105()
    {
        const string code =
        """
        using System;
        using AotObjectMapper.Abstractions.Attributes;
        using AotObjectMapper.Abstractions.Enums;
        using AotObjectMapper.Abstractions.Models;
        
        public class {|#0:Top|}
        {
            [GenerateMapper]
            public partial class Mapper;
        }
        """;


        List<DiagnosticResult> expectedDiagnostics =
        [
            DiagnosticResult.CompilerError(AOMDiagnostics.ClassRequiresPartialKeywordId).WithLocation(0).WithArguments("Top")
        ];

        await VerifyGeneratorDiagnosticsAsync(code, expectedDiagnostics, cancellationToken: TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task GenerateMapperAttribute_MultipleClassesNoPartialKeyWord_AOM105()
    {
        const string code =
        """
        using System;
        using AotObjectMapper.Abstractions.Attributes;
        using AotObjectMapper.Abstractions.Enums;
        using AotObjectMapper.Abstractions.Models;
        
        public class {|#0:Top|}
        {
            [GenerateMapper]
            public class {|#1:Mapper|};
        }
        """;

        List<DiagnosticResult> expectedDiagnostics =
            [
                DiagnosticResult.CompilerError(AOMDiagnostics.ClassRequiresPartialKeywordId).WithLocation(0).WithArguments("Top"),
                DiagnosticResult.CompilerError(AOMDiagnostics.ClassRequiresPartialKeywordId).WithLocation(1).WithArguments("Mapper")
            ];

        await VerifyGeneratorDiagnosticsAsync(code, expectedDiagnostics, cancellationToken: TestContext.Current.CancellationToken);

    }
}