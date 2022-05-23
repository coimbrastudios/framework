using System;
using System.Collections.Generic;
using System.Text;

namespace Coimbra.Editor
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

            using (StringBuilderPool.Pop(out StringBuilder stringBuilder))
            {
                stringBuilder.Append(CoimbraEditorGUIUtility.ToDisplayName(type.Name));

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
