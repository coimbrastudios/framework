using Coimbra.SourceGenerators;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;

namespace Coimbra.Services.SourceGenerators
{
    public sealed class EventsSyntaxReceiver : ISyntaxReceiver
    {
        public readonly List<ClassDeclarationSyntax> Classes = new List<ClassDeclarationSyntax>();

        public readonly List<StructDeclarationSyntax> Structs = new List<StructDeclarationSyntax>();

        private const string EventInterface = "IEvent";

        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            switch (syntaxNode)
            {
                case TypeDeclarationSyntax typeNode when !typeNode.ImplementsInterface(EventInterface):
                    return;

                case TypeDeclarationSyntax typeNode when typeNode.Parent is TypeDeclarationSyntax:
                {
                    Console.WriteLine($"Skipping {typeNode.GetTypeName()} due being nested into another type.");

                    return;
                }

                case TypeDeclarationSyntax typeNode when !typeNode.IsPartial():
                {
                    Console.WriteLine($"Skipping {typeNode.GetTypeName()} due not being partial.");

                    return;
                }

                case ClassDeclarationSyntax classNode:
                {
                    if (!classNode.IsAbstract())
                    {
                        Classes.Add(classNode);
                    }

                    break;
                }

                case StructDeclarationSyntax structNode:
                {
                    Structs.Add(structNode);

                    break;
                }
            }
        }
    }
}
