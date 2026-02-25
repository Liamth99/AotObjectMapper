using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AotObjectMapper.Abstractions.Attributes;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
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
}