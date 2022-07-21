using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace Coimbra.Roslyn
{
    public sealed class CopyBaseConstructorsSyntaxReceiver : ISyntaxReceiver
    {
        public readonly List<CopyBaseConstructorsTypeInfo> Types = new();

        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            if (syntaxNode is not ClassDeclarationSyntax { AttributeLists: { Count: > 0 } } classDeclaration
             || !classDeclaration.Modifiers.Any(SyntaxKind.PartialKeyword)
             || classDeclaration.Modifiers.Any(SyntaxKind.StaticKeyword))
            {
                return;
            }

            CopyBaseConstructorsTypeInfo type = new()
            {
                ClassDeclaration = classDeclaration,
            };

            Types.Add(type);
        }
    }
}
