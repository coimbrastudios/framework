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

            return GetType(assemblyName, typeName);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static Type GetType(in string assemblyName, in string typeName)
        {
            Assembly assembly = Assembly.Load(assemblyName);

            return assembly.GetType(typeName);
        }
    }
}
