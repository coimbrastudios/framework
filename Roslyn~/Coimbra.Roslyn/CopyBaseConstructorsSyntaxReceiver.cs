using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace Coimbra.Roslyn
{
    public sealed class CopyBaseConstructorsSyntaxReceiver : ISyntaxContextReceiver
    {
        public readonly List<CopyBaseConstructorsTypeInfo> Types = new();

        public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
        {
            if (context.Node is not ClassDeclarationSyntax { AttributeLists: { Count: > 0 } } classDeclaration
             || !classDeclaration.Modifiers.Any(SyntaxKind.PartialKeyword)
             || classDeclaration.Modifiers.Any(SyntaxKind.StaticKeyword)
             || context.SemanticModel.GetDeclaredSymbol(classDeclaration) is not ITypeSymbol { BaseType: not null } classType
             || classType.BaseType.InstanceConstructors.Length == 0
             || !classType.HasAttribute(CoimbraTypes.CopyBaseConstructorsAttribute, out AttributeData attribute, false))
            {
                return;
            }

            CopyBaseConstructorsTypeInfo type = new()
            {
                ClassDeclaration = classDeclaration,
                ClassType = classType,
                IgnoreProtected = attribute.ConstructorArguments.Length > 0 && attribute.ConstructorArguments[0].Value is bool and true,
            };

            Types.Add(type);
        }
    }
}
