using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace Coimbra.Roslyn
{
    public sealed class SharedManagedPoolSyntaxReceiver : ISyntaxReceiver
    {
        public readonly List<TypeDeclarationSyntax> Types = new();

        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            if (syntaxNode is TypeDeclarationSyntax { TypeParameterList: null } typeDeclaration
             && typeDeclaration.Modifiers.Any(SyntaxKind.PartialKeyword))
            {
                Types.Add(typeDeclaration);
            }
        }
    }
}
