using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using Microsoft.CodeAnalysis.Formatting;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Coimbra.Roslyn
{
    [ExportCodeFixProvider(LanguageNames.CSharp)]
    public sealed class CopyBaseConstructorsNestedTypeCodeFix : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(Diagnostics.CopyBaseConstructorsDoesntSupportNestedTypes.Id);

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
                return MoveTypeToOuterScopeAsync(context.Document, classDeclaration, cancellationToken);
            }

            context.RegisterCodeFix(CodeAction.Create("Move type to outer scope.", createChangedDocument, classDeclaration.ToFullString()), context.Diagnostics);
        }

        private static async Task<Document> MoveTypeToOuterScopeAsync(Document document, ClassDeclarationSyntax classDeclaration, CancellationToken cancellationToken)
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            DocumentEditor editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);

            if (root == null || editor == null)
            {
                return document;
            }

            SyntaxNode sibling = classDeclaration.Parent!;

            while (sibling.Parent is TypeDeclarationSyntax)
            {
                sibling = sibling.Parent;
            }

            editor.InsertBefore(sibling, classDeclaration.WithAdditionalAnnotations(Formatter.Annotation)
                                                              .WithLeadingTrivia(SyntaxFactory.ElasticMarker)
                                                              .WithTrailingTrivia(SyntaxFactory.ElasticMarker));

            editor.RemoveNode(classDeclaration, SyntaxRemoveOptions.AddElasticMarker);

            return editor.GetChangedDocument();
        }
    }
}
