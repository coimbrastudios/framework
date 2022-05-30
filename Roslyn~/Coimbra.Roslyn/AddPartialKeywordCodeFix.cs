using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Coimbra.Roslyn
{
    public abstract class AddPartialKeywordCodeFix : CodeFixProvider
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
                return AddMissingPartialKeywordAsync(context.Document, typeDeclarationSyntax, cancellationToken);
            }

            context.RegisterCodeFix(CodeAction.Create("Add missing partial keyword.", createChangedDocument, typeDeclarationSyntax.ToFullString()), context.Diagnostics);
        }

        private static async Task<Document> AddMissingPartialKeywordAsync(Document document, TypeDeclarationSyntax typeDeclarationSyntax, CancellationToken cancellationToken)
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            if (root == null)
            {
                return document;
            }

            TypeDeclarationSyntax newTypeDeclarationSyntax = typeDeclarationSyntax.AddModifiers(SyntaxFactory.Token(SyntaxKind.PartialKeyword));
            SyntaxNode newRoot = root.ReplaceNode(typeDeclarationSyntax, newTypeDeclarationSyntax);

            return document.WithSyntaxRoot(newRoot);
        }
    }
}
