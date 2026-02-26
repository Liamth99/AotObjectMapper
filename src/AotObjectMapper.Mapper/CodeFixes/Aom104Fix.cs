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

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(Aom104Fix)), Shared]
public sealed class Aom104Fix : CodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds => [AOMDiagnostics.MethodHasIncorrectSignatureNotStaticId];

    public override FixAllProvider GetFixAllProvider()
    {
        return WellKnownFixAllProviders.BatchFixer;
    }

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var diagnostic = context.Diagnostics.First();
        var root       = await context.Document.GetSyntaxRootAsync(context.CancellationToken);

        var token = root!.FindToken(diagnostic.Location.SourceSpan.Start);

        var method = token.Parent?.AncestorsAndSelf()
                             .OfType<MethodDeclarationSyntax>()
                             .FirstOrDefault();

        if(method is null)
            return;

        if (method.Modifiers.Any(SyntaxKind.StaticKeyword))
            return;

        context.RegisterCodeFix(
            CodeAction.Create(
                "Make method static",
                ct => Fix(context.Document, root, method, ct),
                equivalenceKey: "MakeMethodStatic"),
            diagnostic);
    }

    private static Task<Document> Fix(Document document, SyntaxNode root, MethodDeclarationSyntax methodDeclarationSyntax, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var staticToken = SyntaxFactory.Token(SyntaxKind.StaticKeyword).WithTrailingTrivia(SyntaxFactory.Space);

        var newMethod = methodDeclarationSyntax.WithModifiers(methodDeclarationSyntax.Modifiers.Add(staticToken));

        var newRoot = root.ReplaceNode(methodDeclarationSyntax, newMethod);

        return Task.FromResult(document.WithSyntaxRoot(newRoot));
    }
}