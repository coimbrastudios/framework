using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Coimbra.Roslyn
{
    public struct CopyBaseConstructorsTypeInfo
    {
        public bool IgnoreProtected;

        public ITypeSymbol ClassType;

        public ClassDeclarationSyntax ClassDeclaration;
    }
}
