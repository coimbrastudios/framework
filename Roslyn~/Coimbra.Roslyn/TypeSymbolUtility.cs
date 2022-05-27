using Microsoft.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Coimbra.Roslyn
{
    public static class TypeSymbolUtility
    {
        public static bool HasAttribute(this ITypeSymbol typeSymbol, in TypeString typeString, out AttributeData attributeData, bool checkInherited)
        {
            foreach (AttributeData attribute in typeSymbol.GetAttributes())
            {
                if (attribute.AttributeClass is null || !attribute.AttributeClass.Is(typeString))
                {
                    continue;
                }

                attributeData = attribute;

                return true;
            }

            if (checkInherited)
            {
                static bool hasInheritedAttribute(ITypeSymbol typeSymbol, in TypeString typeString, out AttributeData attributeData)
                {
                    foreach (AttributeData attribute in typeSymbol.GetAttributes())
                    {
                        if (attribute.AttributeClass is null || !attribute.AttributeClass.Is(typeString) || !attribute.IsInherited())
                        {
                            continue;
                        }

                        attributeData = attribute;

                        return true;
                    }

                    attributeData = null;

                    return false;
                }

                foreach (INamedTypeSymbol interfaceType in typeSymbol.Interfaces)
                {
                    if (hasInheritedAttribute(interfaceType, typeString, out attributeData))
                    {
                        return true;
                    }
                }

                while (typeSymbol.BaseType != null)
                {
                    typeSymbol = typeSymbol.BaseType;

                    if (hasInheritedAttribute(typeSymbol, typeString, out attributeData))
                    {
                        return true;
                    }

                    foreach (INamedTypeSymbol interfaceType in typeSymbol.Interfaces)
                    {
                        if (hasInheritedAttribute(interfaceType, typeString, out attributeData))
                        {
                            return true;
                        }
                    }
                }
            }

            attributeData = null;

            return false;
        }

        public static bool InheritsFrom(this ITypeSymbol typeSymbol, in TypeString typeString)
        {
            while (typeSymbol.BaseType != null)
            {
                if (typeSymbol.BaseType.Is(typeString))
                {
                    return true;
                }

                typeSymbol = typeSymbol.BaseType;
            }

            return false;
        }

        public static bool ImplementsInterface(this ITypeSymbol typeSymbol, in TypeString typeString)
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
        public static bool Is(this ITypeSymbol typeSymbol, in TypeString typeString)
        {
            return typeSymbol.Name == typeString.Name && typeSymbol.ContainingNamespace.ToString() == typeString.Namespace;
        }

        public static bool IsAssignableTo(this ITypeSymbol typeSymbol, in TypeString typeString)
        {
            do
            {
                if (typeSymbol.Is(typeString))
                {
                    return true;
                }

                foreach (INamedTypeSymbol interfaceSymbol in typeSymbol.Interfaces)
                {
                    if (interfaceSymbol.IsOrImplementsInterface(typeString))
                    {
                        return true;
                    }
                }

                typeSymbol = typeSymbol.BaseType;
            }
            while (typeSymbol != null);

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsOrImplementsInterface(this ITypeSymbol typeSymbol, in TypeString typeString)
        {
            return typeSymbol.Is(typeString) || typeSymbol.ImplementsInterface(typeString);
        }
    }
}
