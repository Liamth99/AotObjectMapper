using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.Simplification;

namespace AotObjectMapper.Mapper.CodeFixes;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(Aom101Fix)), Shared]
public sealed class Aom101Fix : CodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds => [AOMDiagnostics.MethodHasIncorrectSignatureParameterTypeId];

    public override FixAllProvider GetFixAllProvider()
    {
        return WellKnownFixAllProviders.BatchFixer;
    }

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var diagnostic = context.Diagnostics.First();
        var root       = await context.Document.GetSyntaxRootAsync(context.CancellationToken);

        if (root is null || !diagnostic.Properties.TryGetValue("ExpectedType", out var expectedTypeName))
            return ;

        var expectedType = SyntaxFactory.ParseTypeName(expectedTypeName!).WithAdditionalAnnotations(Formatter.Annotation, Simplifier.Annotation);

        var token = root.FindToken(diagnostic.Location.SourceSpan.Start);

        var parameter = token.Parent?.AncestorsAndSelf()
                             .OfType<ParameterSyntax>()
                             .FirstOrDefault();

        if (parameter is null)
            return;

        context.RegisterCodeFix(
            CodeAction.Create(
                "Fix parameter type",
                ct => Fix(context.Document, root, expectedType, parameter, ct),
                equivalenceKey: "FixParameterType"),
            diagnostic);
    }

    private static Task<Document> Fix(Document document, SyntaxNode root, TypeSyntax expectedType, ParameterSyntax parameter, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var newParameter = parameter.WithType(expectedType.WithTriviaFrom(parameter.Type));

        var newRoot = root.ReplaceNode(parameter, newParameter);
        return Task.FromResult(document.WithSyntaxRoot(newRoot));
    }
}