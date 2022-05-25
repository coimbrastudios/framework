using Microsoft.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Coimbra.Roslyn
{
    public static class TypeSymbolUtility
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasAttribute(this ITypeSymbol typeSymbol, TypeString typeString, out AttributeData attributeData)
        {
            foreach (AttributeData attribute in typeSymbol.GetAttributes())
            {
                if (attribute.AttributeClass is not null && attribute.AttributeClass.Is(typeString))
                {
                    attributeData = attribute;

                    return true;
                }
            }

            attributeData = null;

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool InheritsFrom(this ITypeSymbol typeSymbol, TypeString typeString)
        {
            while (typeSymbol.BaseType != null)
            {
                if (typeSymbol.BaseType.Name == typeString.Name && typeSymbol.BaseType.ContainingNamespace.ToString() == typeString.Namespace)
                {
                    return true;
                }

                typeSymbol = typeSymbol.BaseType;
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ImplementsInterface(this ITypeSymbol typeSymbol, TypeString typeString)
        {
            while (typeSymbol != null)
            {
                foreach (INamedTypeSymbol interfaceSymbol in typeSymbol.Interfaces)
                {
                    if (interfaceSymbol.IsOrImplementsInterface(typeString))
                    {
                        return true;
                    }
                }

                typeSymbol = typeSymbol.BaseType;
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Is(this ITypeSymbol typeSymbol, TypeString typeString)
        {
            return typeSymbol.Name == typeString.Name && typeSymbol.ContainingNamespace.ToString() == typeString.Namespace;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsOrImplementsInterface(this ITypeSymbol typeSymbol, TypeString typeString)
        {
            return typeSymbol.Is(typeString) || typeSymbol.ImplementsInterface(typeString);
        }
    }
}
