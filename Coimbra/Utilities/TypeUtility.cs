using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace Coimbra
{
    /// <summary>
    /// Utility methods for <see cref="Type"/> type.
    /// </summary>
    public static class TypeUtility
    {
        private static readonly Dictionary<Type, string> Cache = new();

        /// <summary>
        /// Gets the <see cref="string"/> for the specified <see cref="Type"/>..
        /// </summary>
        public static string GetDisplayString(this Type type)
        {
            if (Cache.TryGetValue(type, out string value))
            {
                return value;
            }

            static void appendGenericParameters(StringBuilder stringBuilder, Type type)
            {
                Type[] types = type.GenericTypeArguments;

                if (!type.IsGenericType || types.Length == 0)
                {
                    return;
                }

                stringBuilder.Append("<");
                stringBuilder.Append(types[0].Name);
                appendGenericParameters(stringBuilder, types[0]);

                for (int i = 1; i < type.GenericTypeArguments.Length; i++)
                {
                    stringBuilder.Append(", ");
                    stringBuilder.Append(types[i].Name);
                    appendGenericParameters(stringBuilder, types[i]);
                }

                stringBuilder.Append(">");
            }

            using (StringBuilderPool.Pop(out StringBuilder stringBuilder))
            {
                stringBuilder.Append(type.Name);
                appendGenericParameters(stringBuilder, type);

                if (!string.IsNullOrWhiteSpace(type.Namespace))
                {
                    stringBuilder.Append(" (").Append(type.Namespace).Append(")");
                }

                value = stringBuilder.ToString();
            }

            Cache.Add(type, value);

            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static Type GetTypeFromString(in string fullTypeName)
        {
            int index = fullTypeName.IndexOf(' ');
            string assemblyName = fullTypeName.Substring(0, index);
            string typeName = fullTypeName.Substring(index + 1);

            return Assembly.Load(assemblyName).GetType(typeName);
        }
    }
}
