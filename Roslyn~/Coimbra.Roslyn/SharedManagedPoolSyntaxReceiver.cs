using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace Coimbra.Roslyn
{
    public sealed class SharedManagedPoolSyntaxReceiver : ISyntaxReceiver
    {
        public readonly List<ClassDeclarationSyntax> Types = new();

        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            if (syntaxNode is ClassDeclarationSyntax { TypeParameterList: null } classDeclarationSyntax
             && classDeclarationSyntax.Modifiers.Any(SyntaxKind.StaticKeyword)
             && classDeclarationSyntax.Modifiers.Any(SyntaxKind.PartialKeyword))
            {
                Types.Add(classDeclarationSyntax);
            }
        }
    }
}
