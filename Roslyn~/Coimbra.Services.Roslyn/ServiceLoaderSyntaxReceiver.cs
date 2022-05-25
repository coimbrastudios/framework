using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace Coimbra.Services.Roslyn
{
    public sealed class ServiceLoaderSyntaxReceiver : ISyntaxReceiver
    {
        public readonly List<ClassDeclarationSyntax> Types = new();

        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            if (syntaxNode is ClassDeclarationSyntax classDeclarationSyntax
             && !classDeclarationSyntax.Modifiers.Any(SyntaxKind.AbstractKeyword))
            {
                Types.Add(classDeclarationSyntax);
            }
        }
    }
}
