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

namespace AotObjectMapper.Mapper.CodeFixes;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(Aom100Fix)), Shared]
public sealed class Aom100Fix : CodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds => [AOMDiagnostics.MethodHasIncorrectSignatureReturnTypeId];

    public override FixAllProvider GetFixAllProvider()
    {
        return WellKnownFixAllProviders.BatchFixer;
    }

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var diagnostic = context.Diagnostics.First();

        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken);

        var returnTypeNode = root?.FindNode(diagnostic.Location.SourceSpan) as TypeSyntax;

        if (returnTypeNode == null)
            return;

        context.RegisterCodeFix(
            CodeAction.Create(
                title: $"Change return type to {diagnostic.Properties["ExpectedReturnType"]}",
                createChangedDocument: ct => ChangeReturnTypeAsync(context.Document, returnTypeNode, diagnostic, ct),
                equivalenceKey: "ChangeReturnType"),
            diagnostic);
    }

    private async Task<Document> ChangeReturnTypeAsync(Document document, TypeSyntax returnTypeNode, Diagnostic diagnostic, CancellationToken cancellationToken)
    {
        var root = await document.GetSyntaxRootAsync(cancellationToken);

        var expectedTypeName = diagnostic.Properties["ExpectedReturnType"]!;

        var newTypeSyntax = SyntaxFactory.ParseTypeName(expectedTypeName)
                                         .WithTriviaFrom(returnTypeNode);

        var newRoot = root!.ReplaceNode(returnTypeNode, newTypeSyntax);

        return document.WithSyntaxRoot(newRoot);
    }
}