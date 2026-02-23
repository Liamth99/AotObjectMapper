using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Testing;
using Xunit;

namespace AotObjectMapper.Mapper.Tests.GenerationTests;

public class NoConstructorTests : AOMVerifierBase
{
    [Fact]
    public async Task MapAttribute_NoPublicCtor_AOM207()
    {
        const string code =
        """
        using System;
        using AotObjectMapper.Abstractions.Attributes;
        using AotObjectMapper.Abstractions.Enums;
        using AotObjectMapper.Abstractions.Models;
        
        namespace TestGen
        {
            public class T1;
            
            public class T2
            {
                private T2()
                {
            
                }
            }
            
            [GenerateMapper]
            [Map<T1, {|#0:T2|}>]
            public partial class NoCtorMapper;
        }
        """;

        await VerifyGeneratorDiagnosticsAsync(code, [DiagnosticResult.CompilerError(AOMDiagnostics.NoConstructorId).WithLocation(0).WithArguments("T2")], TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task MapAttribute_NoPublicCtorWithFactory_NoDiagnostic()
    {
        const string code =
        """
        using System;
        using AotObjectMapper.Abstractions.Attributes;
        using AotObjectMapper.Abstractions.Enums;
        using AotObjectMapper.Abstractions.Models;
        
        namespace TestGen
        {
            public class T1;
            
            public class T2
            {
                private T2()
                {
            
                }
            }
            
            [GenerateMapper]
            [Map<T1, T2>]
            public partial class NoCtorMapper
            {
                [UseFactory<T2>]
                private static T2 GetT2() => null!;
            }
        }
        """;

        await VerifyGeneratorDiagnosticsAsync(code, cancellationToken: TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task MapAttribute_DefaultCtor_NoDiagnostic()
    {
        const string code =
        """
        using System;
        using AotObjectMapper.Abstractions.Attributes;
        using AotObjectMapper.Abstractions.Enums;
        using AotObjectMapper.Abstractions.Models;
        
        namespace TestGen
        {
            public class T1;
            
            public class T2;
            
            [GenerateMapper]
            [Map<T1, T2>]
            public partial class NoCtorMapper;
        }
        """;

        await VerifyGeneratorDiagnosticsAsync(code, cancellationToken: TestContext.Current.CancellationToken);
    }
}