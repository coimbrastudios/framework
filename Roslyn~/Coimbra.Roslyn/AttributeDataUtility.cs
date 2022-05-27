using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Coimbra.Roslyn
{
    public static class AttributeDataUtility
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsInherited(this AttributeData attributeData)
        {
            if (attributeData.AttributeClass == null)
            {
                return false;
            }

            foreach (AttributeData attribute in attributeData.AttributeClass.GetAttributes())
            {
                INamedTypeSymbol attributeClass = attribute.AttributeClass;

                if (attributeClass is not { Name: nameof(AttributeUsageAttribute), ContainingNamespace: { Name: "System" } })
                {
                    continue;
                }

                foreach (KeyValuePair<string, TypedConstant> item in attribute.NamedArguments)
                {
                    if (item.Key == nameof(AttributeUsageAttribute.Inherited))
                    {
                        return (bool)item.Value.Value!;
                    }
                }

                return true;
            }

            return false;
        }
    }
}
