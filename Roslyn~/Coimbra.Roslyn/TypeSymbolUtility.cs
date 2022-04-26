using Microsoft.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Coimbra.Roslyn
{
    public static class TypeSymbolUtility
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasAttribute(this ITypeSymbol namedTypeSymbol, string name, string containingNamespace)
        {
            foreach (AttributeData attributeData in namedTypeSymbol.GetAttributes())
            {
                if (attributeData.AttributeClass is not null && attributeData.AttributeClass.Is(name, containingNamespace))
                {
                    return true;
                }
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ImplementsInterface(this ITypeSymbol namedTypeSymbol, string name, string containingNamespace)
        {
            while (namedTypeSymbol != null)
            {
                foreach (INamedTypeSymbol interfaceSymbol in namedTypeSymbol.Interfaces)
                {
                    if (interfaceSymbol.IsOrImplementsInterface(name, containingNamespace))
                    {
                        return true;
                    }
                }

                namedTypeSymbol = namedTypeSymbol.BaseType;
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Is(this ITypeSymbol namedTypeSymbol, string name, string containingNamespace)
        {
            return namedTypeSymbol.Name == name && namedTypeSymbol.ContainingNamespace.ToString() == containingNamespace;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsOrImplementsInterface(this ITypeSymbol namedTypeSymbol, string name, string containingNamespace)
        {
            return namedTypeSymbol.Is(name, containingNamespace) || namedTypeSymbol.ImplementsInterface(name, containingNamespace);
        }
    }
}
