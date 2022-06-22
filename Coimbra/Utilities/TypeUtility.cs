using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Coimbra
{
    internal static class TypeUtility
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static Type GetType(in string fullTypeName)
        {
            int index = fullTypeName.IndexOf(' ');
            string assemblyName = fullTypeName.Substring(0, index);
            string typeName = fullTypeName.Substring(index + 1);

            return Assembly.Load(assemblyName).GetType(typeName);
        }
    }
}
