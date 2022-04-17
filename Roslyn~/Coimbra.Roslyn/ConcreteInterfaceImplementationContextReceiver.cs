using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Coimbra.Roslyn
{
    public sealed class ConcreteInterfaceImplementationContextReceiver : ISyntaxContextReceiver
    {
        public readonly List<TypeDeclarationSyntax> Types = new List<TypeDeclarationSyntax>();

        private readonly Func<INamedTypeSymbol, bool> _interfacePredicate;

        public ConcreteInterfaceImplementationContextReceiver(string interfaceTypeName, string interfaceTypeNamespace)
        {
            bool predicate(INamedTypeSymbol x)
            {
                return x.Name == interfaceTypeName && x.ContainingNamespace.ToString() == interfaceTypeNamespace;
            }

            _interfacePredicate = predicate;
        }

        public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
        {
            try
            {
                if (!(context.Node is TypeDeclarationSyntax typeDeclarationSyntax)
                 || !(typeDeclarationSyntax is ClassDeclarationSyntax || typeDeclarationSyntax is StructDeclarationSyntax)
                 || typeDeclarationSyntax.Parent is TypeDeclarationSyntax
                 || !typeDeclarationSyntax.Modifiers.Any(SyntaxKind.PartialKeyword)
                 || typeDeclarationSyntax.Modifiers.Any(SyntaxKind.AbstractKeyword)
                 || !(context.SemanticModel.GetDeclaredSymbol(context.Node) is INamedTypeSymbol typeSymbol)
                 || typeSymbol.AllInterfaces.FirstOrDefault(_interfacePredicate) == null)
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
