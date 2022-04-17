using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Runtime.CompilerServices;

namespace Coimbra.Roslyn
{
    public static class TypeDeclarationSyntaxUtility
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetTypeName(this TypeDeclarationSyntax typeDeclarationSyntax)
        {
            return typeDeclarationSyntax.Identifier.Text;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetNamespace(this TypeDeclarationSyntax typeDeclarationSyntax)
        {
            return (typeDeclarationSyntax.Parent as NamespaceDeclarationSyntax)?.Name.ToString();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ImplementsInterface(this TypeDeclarationSyntax typeDeclarationSyntax, string interfaceName)
        {
            if (typeDeclarationSyntax.BaseList == null)
            {
                return false;
            }

            foreach (BaseTypeSyntax type in typeDeclarationSyntax.BaseList.Types)
            {
                if (type.ToString() == interfaceName)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
