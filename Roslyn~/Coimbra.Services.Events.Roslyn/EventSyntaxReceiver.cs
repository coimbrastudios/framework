using Coimbra.Roslyn;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace Coimbra.Services.Events.Roslyn
{
    public sealed class EventSyntaxReceiver : ISyntaxContextReceiver
    {
        public readonly List<TypeDeclarationSyntax> Types = new();

        public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
        {
            if (context.Node is TypeDeclarationSyntax { Parent: not TypeDeclarationSyntax } typeDeclarationSyntax and (StructDeclarationSyntax or ClassDeclarationSyntax)
             && !typeDeclarationSyntax.Modifiers.Any(SyntaxKind.AbstractKeyword)
             && typeDeclarationSyntax.Modifiers.Any(SyntaxKind.PartialKeyword)
             && context.SemanticModel.GetDeclaredSymbol(context.Node) is ITypeSymbol typeSymbol
             && typeSymbol.ImplementsInterface(CoimbraServicesEventsTypes.EventInterface))
            {
                Types.Add(typeDeclarationSyntax);
            }
        }
    }
}
