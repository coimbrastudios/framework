using Microsoft.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Coimbra.Roslyn
{
    public static class TypeSymbolUtility
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasAttribute(this ITypeSymbol typeSymbol, string name, string containingNamespace, out AttributeData attributeData)
        {
            foreach (AttributeData attribute in typeSymbol.GetAttributes())
            {
                if (attribute.AttributeClass is not null && attribute.AttributeClass.Is(name, containingNamespace))
                {
                    attributeData = attribute;

                    return true;
                }
            }

            attributeData = null;

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ImplementsInterface(this ITypeSymbol typeSymbol, string name, string containingNamespace)
        {
            while (typeSymbol != null)
            {
                foreach (INamedTypeSymbol interfaceSymbol in typeSymbol.Interfaces)
                {
                    if (interfaceSymbol.IsOrImplementsInterface(name, containingNamespace))
                    {
                        return true;
                    }
                }

                typeSymbol = typeSymbol.BaseType;
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Is(this ITypeSymbol typeSymbol, string name, string containingNamespace)
        {
            return typeSymbol.Name == name && typeSymbol.ContainingNamespace.ToString() == containingNamespace;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsOrImplementsInterface(this ITypeSymbol typeSymbol, string name, string containingNamespace)
        {
            return typeSymbol.Is(name, containingNamespace) || typeSymbol.ImplementsInterface(name, containingNamespace);
        }
    }
}
