using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using Microsoft.CodeAnalysis.Formatting;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Coimbra.Roslyn
{
    public abstract class MoveToOuterScopeCodeFix : CodeFixProvider
    {
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

            TypeDeclarationSyntax typeDeclarationSyntax = root?
                                                         .FindNode(context.Span)
                                                         .DescendantNodesAndSelf()
                                                         .OfType<TypeDeclarationSyntax>()
                                                         .FirstOrDefault();

            if (typeDeclarationSyntax is not (ClassDeclarationSyntax or StructDeclarationSyntax)
             || !context.Diagnostics.Any())
            {
                return;
            }

            Task<Document> createChangedDocument(CancellationToken cancellationToken)
            {
                return MoveTypeToOuterScopeAsync(context.Document, typeDeclarationSyntax, cancellationToken);
            }

            context.RegisterCodeFix(CodeAction.Create("Move type to outer scope.", createChangedDocument, typeDeclarationSyntax.ToFullString()), context.Diagnostics);
        }

        private static async Task<Document> MoveTypeToOuterScopeAsync(Document document, TypeDeclarationSyntax typeDeclarationSyntax, CancellationToken cancellationToken)
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            DocumentEditor editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);

            if (root == null || editor == null)
            {
                return document;
            }

            SyntaxNode sibling = typeDeclarationSyntax.Parent!;

            while (sibling.Parent is TypeDeclarationSyntax)
            {
                sibling = sibling.Parent;
            }

            editor.InsertBefore(sibling, typeDeclarationSyntax.WithAdditionalAnnotations(Formatter.Annotation)
                                                              .WithLeadingTrivia(SyntaxFactory.ElasticMarker)
                                                              .WithTrailingTrivia(SyntaxFactory.ElasticMarker));

            editor.RemoveNode(typeDeclarationSyntax, SyntaxRemoveOptions.AddElasticMarker);

            return editor.GetChangedDocument();
        }
    }
}
