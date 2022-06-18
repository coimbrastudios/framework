using System;
using System.Collections.Generic;
using System.Text;

namespace Coimbra
{
    internal static class TypeString
    {
        private static readonly Dictionary<Type, string> Cache = new Dictionary<Type, string>();

        public static string Get(Type type)
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
                    stringBuilder.Append($" ({type.Namespace})");
                }

                value = stringBuilder.ToString();
            }

            Cache.Add(type, value);

            return value;
        }
    }
}
