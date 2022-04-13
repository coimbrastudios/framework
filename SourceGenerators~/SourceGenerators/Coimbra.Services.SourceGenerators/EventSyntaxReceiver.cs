using Coimbra.SourceGenerators;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace Coimbra.Services.SourceGenerators
{
    public sealed class EventSyntaxReceiver : ISyntaxReceiver
    {
        public readonly List<ClassDeclarationSyntax> Classes = new List<ClassDeclarationSyntax>();

        public readonly List<StructDeclarationSyntax> Structs = new List<StructDeclarationSyntax>();

        private const string EventInterface = "IEvent";

        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            if (syntaxNode.Parent is TypeDeclarationSyntax)
            {
                return;
            }

            if (syntaxNode is ClassDeclarationSyntax classNode)
            {
                if (classNode.IsPartial() && !classNode.IsAbstract() && classNode.ImplementsInterface(EventInterface))
                {
                    Classes.Add(classNode);
                }
            }
            else if (syntaxNode is StructDeclarationSyntax structNode)
            {
                if (structNode.IsPartial() && structNode.ImplementsInterface(EventInterface))
                {
                    Structs.Add(structNode);
                }
            }
        }
    }
}
