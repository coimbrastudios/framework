using Coimbra.Roslyn;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Coimbra.Services.Events.Roslyn
{
    [ExportCodeFixProvider(LanguageNames.CSharp)]
    public sealed class EventDeclarationMissingPartialCodeFix : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(Diagnostics.ConcreteEventShouldBePartial.Id);

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

            if (!(typeDeclarationSyntax is ClassDeclarationSyntax || typeDeclarationSyntax is StructDeclarationSyntax)
             || context.Diagnostics.FirstOrDefault() == null)
            {
                return;
            }

            Task<Document> createChangedDocument(CancellationToken cancellationToken)
            {
                return AddMissingPartialKeyword(context.Document, typeDeclarationSyntax, cancellationToken);
            }

            context.RegisterCodeFix(CodeAction.Create("Add missing partial keyword.", createChangedDocument, typeDeclarationSyntax.ToFullString()), context.Diagnostics);
        }

        public override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        private static async Task<Document> AddMissingPartialKeyword(Document document, TypeDeclarationSyntax typeDeclarationSyntax, CancellationToken cancellationToken)
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            if (root == null)
            {
                return document;
            }

            TypeDeclarationSyntax newTypeDeclarationSyntax = typeDeclarationSyntax.AddModifiers(SyntaxFactory.Identifier("partial"));
            SyntaxNode newRoot = root.ReplaceNode(typeDeclarationSyntax, newTypeDeclarationSyntax);

            return document.WithSyntaxRoot(newRoot);
        }
    }
}
