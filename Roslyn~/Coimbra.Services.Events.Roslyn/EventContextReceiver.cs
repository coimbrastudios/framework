using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Coimbra.Roslyn
{
    public sealed class EventContextReceiver : ISyntaxContextReceiver
    {
        public readonly List<TypeDeclarationSyntax> Types = new();

        private readonly Func<INamedTypeSymbol, bool> _interfacePredicate;

        public EventContextReceiver()
        {
            static bool interfacePredicate(INamedTypeSymbol x)
            {
                return x.Name == "IEvent" && x.ContainingNamespace.ToString() == "Coimbra.Services.Events";
            }

            _interfacePredicate = interfacePredicate;
        }

        public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
        {
            try
            {
                if (context.Node is not TypeDeclarationSyntax typeDeclarationSyntax
                 || typeDeclarationSyntax is not (StructDeclarationSyntax or ClassDeclarationSyntax)
                 || typeDeclarationSyntax.Parent is TypeDeclarationSyntax
                 || !typeDeclarationSyntax.Modifiers.Any(SyntaxKind.PartialKeyword)
                 || typeDeclarationSyntax.Modifiers.Any(SyntaxKind.AbstractKeyword)
                 || context.SemanticModel.GetDeclaredSymbol(context.Node) is not INamedTypeSymbol typeSymbol
                 || !typeSymbol.AllInterfaces.Any(_interfacePredicate))
                {
                    return;
                }

                Types.Add(typeDeclarationSyntax);
            }
            catch (Exception e)
            {
                Logger.Write(e);
            }
        }
    }
}
