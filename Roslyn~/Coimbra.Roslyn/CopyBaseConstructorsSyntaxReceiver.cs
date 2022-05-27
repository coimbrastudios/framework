using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace Coimbra.Roslyn
{
    public sealed class CopyBaseConstructorsSyntaxReceiver : ISyntaxReceiver
    {
        public readonly List<ClassDeclarationSyntax> Types = new();

        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            if (syntaxNode is not ClassDeclarationSyntax { AttributeLists: { Count: > 0 } } classDeclaration
             || !classDeclaration.Modifiers.Any(SyntaxKind.PartialKeyword)
             || classDeclaration.Modifiers.Any(SyntaxKind.StaticKeyword))
            {
                return;
            }

            foreach (AttributeListSyntax attributeList in classDeclaration.AttributeLists)
            {
                foreach (AttributeSyntax attribute in attributeList.Attributes)
                {
                    string attributeName = attribute.Name.ToString();

                    if (attributeName != CoimbraTypes.CopyBaseConstructorsAttribute.Name
                     && attributeName != "CopyBaseConstructors")
                    {
                        continue;
                    }

                    Types.Add(classDeclaration);

                    return;
                }
            }
        }
    }
}
