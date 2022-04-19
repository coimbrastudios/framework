using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Runtime.CompilerServices;

namespace Coimbra.Roslyn
{
    public static class TypeDeclarationSyntaxUtility
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetNamespace(this TypeDeclarationSyntax typeDeclarationSyntax)
        {
            return (typeDeclarationSyntax.Parent as NamespaceDeclarationSyntax)?.Name.ToString();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetTypeName(this TypeDeclarationSyntax typeDeclarationSyntax)
        {
            return typeDeclarationSyntax.Identifier.Text;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasParameterlessConstructor(this TypeDeclarationSyntax typeDeclarationSyntax, out bool isPublic)
        {
            switch (typeDeclarationSyntax)
            {
                case StructDeclarationSyntax _:
                case ClassDeclarationSyntax _ when !typeDeclarationSyntax.Members.Any(SyntaxKind.ConstructorDeclaration):
                {
                    isPublic = true;

                    return true;
                }
            }

            foreach (MemberDeclarationSyntax memberDeclarationSyntax in typeDeclarationSyntax.Members)
            {
                if (memberDeclarationSyntax is ConstructorDeclarationSyntax constructorDeclarationSyntax && constructorDeclarationSyntax.ParameterList.Parameters.Count == 0)
                {
                    isPublic = constructorDeclarationSyntax.Modifiers.Any(SyntaxKind.PublicKeyword);

                    return true;
                }
            }

            isPublic = false;

            return false;
        }
    }
}
