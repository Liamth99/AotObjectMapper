using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace AotObjectMapper.Mapper.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class PreferNameOfAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = [AOMDiagnostics.AOM202_PreferNameOf, ];

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterOperationAction(AnalyzeArgumentOperation, OperationKind.Argument);
    }

    private static void AnalyzeArgumentOperation(OperationAnalysisContext context)
    {
        var argumentOperation = (IArgumentOperation)context.Operation;
        var parameter = argumentOperation.Parameter;

        if (parameter is null)
            return;

        var preferNameOfAttr = parameter.GetAttributes()
                                        .FirstOrDefault(a => a.AttributeClass?.Name is "PreferNameOfAttribute");

        if (preferNameOfAttr is null)
            return;

        var value = argumentOperation.Value;

        if (value is ILiteralOperation literal && literal.ConstantValue.HasValue && literal.ConstantValue.Value is string memberName)
        {
            var targetType = ResolveTargetType(preferNameOfAttr, parameter);

            if (targetType is null)
                return;

            if (targetType.GetMembers(memberName).Count(x => context.Compilation.IsSymbolAccessibleWithin(x, context.ContainingSymbol.ContainingAssembly)) > 0)
            {
                var properties = ImmutableDictionary<string, string?>.Empty
                    .Add("Type", targetType.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat))
                    .Add("MemberName", memberName);

                context.ReportDiagnostic(Diagnostic.Create(AOMDiagnostics.AOM202_PreferNameOf, value.Syntax.GetLocation(), properties, targetType.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat), memberName));
            }
        }
    }

    private static ITypeSymbol? ResolveTargetType(AttributeData preferNameOfAttr, IParameterSymbol parameter)
    {
        var arg = preferNameOfAttr.ConstructorArguments[0];

        if (arg.Value is not string targetTypeName)
            return null;

        var containingSymbol = parameter.ContainingSymbol;

        if (containingSymbol is IMethodSymbol methodSymbol)
        {
            for (int i = 0; i < methodSymbol.TypeParameters.Length; i++)
            {
                if (methodSymbol.TypeParameters[i].Name == targetTypeName)
                {
                    return methodSymbol.TypeArguments[i];
                }
            }

            var containingType = methodSymbol.ContainingType;
            if (containingType is not null)
            {
                return ResolveFromContainingType(containingType, targetTypeName);
            }
        }

        if (containingSymbol is INamedTypeSymbol typeSymbol)
        {
            return ResolveFromContainingType(typeSymbol, targetTypeName);
        }

        return null;
    }

    private static ITypeSymbol? ResolveFromContainingType(INamedTypeSymbol typeSymbol, string targetTypeName)
    {
        while (typeSymbol is not null)
        {
            for (int i = 0; i < typeSymbol.TypeParameters.Length; i++)
            {
                if (typeSymbol.TypeParameters[i].Name == targetTypeName)
                {
                    return typeSymbol.TypeArguments[i];
                }
            }

            typeSymbol = typeSymbol.ContainingType;
        }

        return null;
    }
}