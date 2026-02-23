using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;
using AotObjectMapper.Abstractions.Attributes;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace AotObjectMapper.Mapper.Tests.Utils;

public static class GeneratorTestHelper
{
    public static MetadataReference[] References { get; private set; }

    static GeneratorTestHelper()
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies()
                                  .Where(a => !a.IsDynamic && !string.IsNullOrWhiteSpace(a.Location))
                                  .ToList();

        assemblies.Add(typeof(GenerateMapperAttribute).Assembly);

        References = assemblies
           .Distinct()
           .Select(a => MetadataReference.CreateFromFile(a.Location))
           .ToArray<MetadataReference>();
    }

    public static GeneratorResults RunGenerator(string source)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(source);

        var compilation = CSharpCompilation.Create(
            assemblyName: "Tests",
            syntaxTrees:  [syntaxTree,],
            references:   References,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, nullableContextOptions: NullableContextOptions.Enable));

        var generator = new MapperGenerator();

        GeneratorDriver driver = CSharpGeneratorDriver.Create(generator.AsSourceGenerator());

        driver = driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out var diagnostics);

        var result = driver.GetRunResult();

        return new GeneratorResults(outputCompilation, result, diagnostics);
    }

    public static Assembly CompileToAssembly(Compilation compilation)
    {
        using (var ms = new MemoryStream())
        {
            var result = compilation.Emit(ms);

            if (!result.Success)
            {
                var ex = new AggregateException("Compilations failed", result.Diagnostics.Select(x => new Exception(x.ToString())));

                throw ex;
            }

            ms.Seek(0, SeekOrigin.Begin);
            return Assembly.Load(ms.ToArray());
        }
    }
}

public record GeneratorResults(Compilation Compilation, GeneratorDriverRunResult Result, ImmutableArray<Diagnostic> Diagnostics);