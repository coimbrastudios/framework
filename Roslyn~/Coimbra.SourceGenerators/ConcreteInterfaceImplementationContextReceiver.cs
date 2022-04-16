using Coimbra.SourceGenerators;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Coimbra.Services.Events.SourceGenerators
{
    public sealed class ConcreteInterfaceImplementationContextReceiver : ISyntaxContextReceiver
    {
        public readonly List<TypeDeclarationSyntax> Types = new List<TypeDeclarationSyntax>();

        private readonly Func<INamedTypeSymbol, bool> _predicate;

        public ConcreteInterfaceImplementationContextReceiver(string interfaceTypeName, string interfaceTypeNamespace)
        {
            bool predicate(INamedTypeSymbol x)
            {
                return x.Name == interfaceTypeName && x.ContainingNamespace.ToString() == interfaceTypeNamespace;
            }

            _predicate = predicate;
        }

        public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
        {
            try
            {
                if (!(context.Node is TypeDeclarationSyntax typeNode)
                 || !(typeNode is ClassDeclarationSyntax || typeNode is StructDeclarationSyntax)
                 || typeNode.Parent is TypeDeclarationSyntax
                 || !typeNode.Modifiers.Any(SyntaxKind.PartialKeyword)
                 || typeNode.Modifiers.Any(SyntaxKind.AbstractKeyword)
                 || !(context.SemanticModel.GetDeclaredSymbol(context.Node) is INamedTypeSymbol typeSymbol)
                 || typeSymbol.AllInterfaces.FirstOrDefault(_predicate) == null)
                {
                    return;
                }

                Types.Add(typeNode);
            }
            catch (Exception e)
            {
                Logger.Write(e);
            }
        }
    }
}
