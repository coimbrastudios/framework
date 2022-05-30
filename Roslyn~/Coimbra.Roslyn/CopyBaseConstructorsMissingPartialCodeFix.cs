using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Coimbra.Roslyn
{
    [ExportCodeFixProvider(LanguageNames.CSharp)]
    public sealed class CopyBaseConstructorsMissingPartialCodeFix : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(Diagnostics.CopyBaseConstructorsRequiresPartialKeyword.Id);

        public override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context
                                   .Document
                                   .GetSyntaxRootAsync(context.CancellationToken)
                                   .ConfigureAwait(false);

            ClassDeclarationSyntax classDeclaration = root?
                                                     .FindNode(context.Span)
                                                     .DescendantNodesAndSelf()
                                                     .OfType<ClassDeclarationSyntax>()
                                                     .FirstOrDefault();

            if (classDeclaration is null
             || !context.Diagnostics.Any())
            {
                return;
            }

            Task<Document> createChangedDocument(CancellationToken cancellationToken)
            {
                return AddMissingPartialKeywordAsync(context.Document, classDeclaration, cancellationToken);
            }

            context.RegisterCodeFix(CodeAction.Create("Add missing partial keyword.", createChangedDocument, classDeclaration.ToFullString()), context.Diagnostics);
        }

        private static async Task<Document> AddMissingPartialKeywordAsync(Document document, ClassDeclarationSyntax classDeclaration, CancellationToken cancellationToken)
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            if (root == null)
            {
                return document;
            }

            ClassDeclarationSyntax newClassDeclaration = classDeclaration.AddModifiers(SyntaxFactory.Token(SyntaxKind.PartialKeyword));
            SyntaxNode newRoot = root.ReplaceNode(classDeclaration, newClassDeclaration);

            return document.WithSyntaxRoot(newRoot);
        }
    }
}
