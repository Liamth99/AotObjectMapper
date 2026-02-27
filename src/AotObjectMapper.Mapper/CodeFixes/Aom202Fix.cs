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

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(Aom202Fix)), Shared]
public sealed class Aom202Fix : CodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds => [AOMDiagnostics.PreferNameOfId];

    public override FixAllProvider GetFixAllProvider()
    {
        return WellKnownFixAllProviders.BatchFixer;
    }

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var diagnostic = context.Diagnostics[0];

        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken);

        if (root is null)
            return;

        var token = root.FindToken(diagnostic.Location.SourceSpan.Start);

        var literalNode = token.Parent?
                               .AncestorsAndSelf()
                               .OfType<LiteralExpressionSyntax>()
                               .FirstOrDefault();

        if (literalNode is null)
            return;

        context.RegisterCodeFix(
            CodeAction.Create(
                title: "Replace with nameof(...)",
                createChangedDocument: ct => ReplaceWithNameOfAsync(root, context.Document, literalNode, diagnostic, ct),
                equivalenceKey: "ReplaceWithNameOf"),
            diagnostic);
    }

    private static Task<Document> ReplaceWithNameOfAsync(SyntaxNode root, Document document, ExpressionSyntax literalNode, Diagnostic diagnostic, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
            return Task.FromResult(document);

        var targetTypeName = diagnostic.Properties["Type"];
        var memberName     = diagnostic.Properties["MemberName"];

        if (targetTypeName is null || memberName is null)
            return Task.FromResult(document);

        var nameofExpression = SyntaxFactory.ParseExpression($"nameof({targetTypeName}.{memberName})")
                                            .WithTriviaFrom(literalNode);

        var newRoot = root.ReplaceNode(literalNode, nameofExpression);

        return Task.FromResult(document.WithSyntaxRoot(newRoot));
    }
}