using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Runtime.CompilerServices;

namespace Coimbra.SourceGenerators
{
    public static class TypeDeclarationSyntaxUtility
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetTypeName(this TypeDeclarationSyntax node)
        {
            return node.Identifier.Text;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetNamespace(this TypeDeclarationSyntax node)
        {
            return (node.Parent as NamespaceDeclarationSyntax)?.Name.ToString();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ImplementsInterface(this TypeDeclarationSyntax node, string interfaceName)
        {
            if (node.BaseList == null)
            {
                return false;
            }

            foreach (BaseTypeSyntax type in node.BaseList.Types)
            {
                if (type.ToString() == interfaceName)
                {
                    return true;
                }
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsAbstract(this TypeDeclarationSyntax node)
        {
            return node.Modifiers.Any(SyntaxKind.AbstractKeyword);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsPartial(this TypeDeclarationSyntax node)
        {
            return node.Modifiers.Any(SyntaxKind.PartialKeyword);
        }
    }
}
