using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AotObjectMapper.Abstractions.Attributes;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;

namespace AotObjectMapper.Mapper.Tests;

public abstract class AOMVerifierBase
{
    public static async Task VerifyGeneratorDiagnosticsAsync(string code, IEnumerable<DiagnosticResult>? expectedDiagnostics = null, CancellationToken cancellationToken = default)
    {
        var test = new CSharpSourceGeneratorTest<MapperGenerator, DefaultVerifier>()
        {
            TestState =
            {
                Sources = { code, },
            },
            TestBehaviors = TestBehaviors.SkipGeneratedSourcesCheck,
        };

        test.TestState.AdditionalReferences.Add(MetadataReference.CreateFromFile(typeof(GenerateMapperAttribute).Assembly.Location));

        if(expectedDiagnostics is not null)
            test.ExpectedDiagnostics.AddRange(expectedDiagnostics);

        await test.RunAsync(cancellationToken);
    }

    public static async Task VerifyAnalyzerAsync<TAnalyzer>(string source, IEnumerable<DiagnosticResult>? expected = null, CancellationToken cancellationToken = default) where TAnalyzer : DiagnosticAnalyzer, new()
    {
        var test = new CSharpAnalyzerTest<TAnalyzer, DefaultVerifier>
        {
            TestCode = source,
        };

        test.TestState.AdditionalReferences.Add(MetadataReference.CreateFromFile(typeof(GenerateMapperAttribute).Assembly.Location));

        if(expected is not null)
            test.ExpectedDiagnostics.AddRange(expected);

        await test.RunAsync(cancellationToken);
    }

    public static async Task VerifyCodeFixAsync<TAnalyzer, TCodeFixProvider>(string source, string fix, IEnumerable<DiagnosticResult>? expected = null, CancellationToken cancellationToken = default)
            where TAnalyzer         : DiagnosticAnalyzer, new()
            where TCodeFixProvider  : CodeFixProvider,    new()
    {
        var test = new CSharpCodeFixTest<TAnalyzer, TCodeFixProvider, DefaultVerifier>
        {
            TestCode  = source,
            FixedCode = fix,
        };

        test.TestState.AdditionalReferences.Add(MetadataReference.CreateFromFile(typeof(GenerateMapperAttribute).Assembly.Location));

        if(expected is not null)
            test.ExpectedDiagnostics.AddRange(expected);

        await test.RunAsync(cancellationToken);
    }
}