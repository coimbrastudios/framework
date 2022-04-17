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
    public sealed class EventDeclarationParameterlessCtorCodeFix : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(Diagnostics.ConcreteEventParameterlessCtorShouldBePublic.Id);

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
                return MakeParameterlessConstructorPublic(context.Document, typeDeclarationSyntax, cancellationToken);
            }

            context.RegisterCodeFix(CodeAction.Create("Make parameterless constructor public.", createChangedDocument, typeDeclarationSyntax.ToFullString()), context.Diagnostics);
        }

        public override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        private static async Task<Document> MakeParameterlessConstructorPublic(Document document, TypeDeclarationSyntax typeDeclarationSyntax, CancellationToken cancellationToken)
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            if (root == null)
            {
                return document;
            }

            foreach (MemberDeclarationSyntax memberDeclarationSyntax in typeDeclarationSyntax.Members)
            {
                if (memberDeclarationSyntax is ConstructorDeclarationSyntax constructorDeclarationSyntax && constructorDeclarationSyntax.ParameterList.Parameters.Count == 0)
                {
                    SyntaxTokenList newSyntaxTokenList = memberDeclarationSyntax.Modifiers;

                    for (int i = newSyntaxTokenList.Count - 1; i >= 0; i--)
                    {
                        if (newSyntaxTokenList[i].Kind() is SyntaxKind.ProtectedKeyword or SyntaxKind.InternalKeyword or SyntaxKind.PrivateKeyword)
                        {
                            newSyntaxTokenList = newSyntaxTokenList.RemoveAt(i);
                        }
                    }

                    newSyntaxTokenList = newSyntaxTokenList.Add(SyntaxFactory.Token(SyntaxKind.PublicKeyword));
                    MemberDeclarationSyntax newMemberDeclarationSyntax = memberDeclarationSyntax.WithModifiers(newSyntaxTokenList);
                    SyntaxNode newRoot = root.ReplaceNode(memberDeclarationSyntax, newMemberDeclarationSyntax);

                    return document.WithSyntaxRoot(newRoot);
                }
            }

            return document;
        }
    }
}
