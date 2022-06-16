using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace Coimbra
{
    /// <summary>
    /// Utility methods for <see cref="PropertyPathInfo"/> type.
    /// </summary>
    public static class PropertyPathInfoUtility
    {
        private const BindingFlags PropertyPathInfoFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy;

        private static readonly Dictionary<Type, Dictionary<string, PropertyPathInfo>> PropertyPathInfoMapFromRootType = new Dictionary<Type, Dictionary<string, PropertyPathInfo>>();

        /// <summary>
        /// Creates or gets a cached <see cref="PropertyPathInfo"/>.
        /// </summary>
        public static PropertyPathInfo GetPropertyPathInfo(this Type type, in string propertyPath)
        {
            if (!PropertyPathInfoMapFromRootType.TryGetValue(type, out Dictionary<string, PropertyPathInfo> propertyPathInfoMap))
            {
                propertyPathInfoMap = new Dictionary<string, PropertyPathInfo>();
                PropertyPathInfoMapFromRootType.Add(type, propertyPathInfoMap);
            }

            if (propertyPathInfoMap.TryGetValue(propertyPath, out PropertyPathInfo propertyPathInfo))
            {
                return propertyPathInfo;
            }

            propertyPathInfo = GetPropertyPathInfo(type, propertyPath.Split('.'), propertyPathInfoMap);
            propertyPathInfoMap[propertyPath] = propertyPathInfo;

            return propertyPathInfo;
        }

        internal static void ClearCaches()
        {
            PropertyPathInfoMapFromRootType.Clear();
        }

        private static Type GetCollectionType(Type type)
        {
            Type value = type.GetElementType();

            if (value != null)
            {
                return value;
            }

            Type[] arguments = type.GetGenericArguments();

            return arguments.Length > 0 ? arguments[0] : type;
        }

        private static FieldInfo GetField(Type type, string field)
        {
            if (type == typeof(Vector2Int) || type == typeof(Vector3Int))
            {
                if (field.Length == 1)
                {
                    field = $"m_{field.ToUpperInvariant()}";
                }
            }

            FieldInfo result = null;

            for (; result == null && type != null; type = type.BaseType)
            {
                result = type.GetField(field, PropertyPathInfoFlags);
            }

            return result;
        }

        private static PropertyPathInfo GetPropertyPathInfo(Type rootType, IEnumerable<string> splitPropertyPathArray, IDictionary<string, PropertyPathInfo> cache)
        {
            using (StringBuilderPool.Pop(out StringBuilder propertyPathBuilder))
            {
                using (ListPool.Pop(out List<string> splitPropertyPath))
                {
                    splitPropertyPath.AddRange(splitPropertyPathArray);

                    const char separator = '.';
                    PropertyPathInfo currentPropertyPathInfo = null;
                    Type currentType = rootType;
                    int currentDepth = 0;

                    while (splitPropertyPath.Count > 0)
                    {
                        if (propertyPathBuilder.Length > 0)
                        {
                            propertyPathBuilder.Append(".");
                        }

                        FieldInfo fieldInfo = GetField(currentType, splitPropertyPath[0]);
                        propertyPathBuilder.Append(splitPropertyPath[0]);
                        splitPropertyPath.RemoveAt(0);

                        if (fieldInfo == null)
                        {
                            currentPropertyPathInfo = null;

                            break;
                        }

                        string propertyPath = propertyPathBuilder.ToString();

                        if (!cache.TryGetValue(propertyPath, out PropertyPathInfo cachedPropertyPathInfo))
                        {
                            cachedPropertyPathInfo = new PropertyPathInfo(fieldInfo.FieldType, rootType, fieldInfo, currentPropertyPathInfo, currentDepth, null, propertyPath);
                            cache.Add(propertyPath, cachedPropertyPathInfo);
                        }

                        currentPropertyPathInfo = cachedPropertyPathInfo;
                        currentType = fieldInfo.FieldType;
                        currentDepth++;

                        if (splitPropertyPath.Count <= 1
                         || splitPropertyPath[0] != "Array"
                         || splitPropertyPath[1].Length <= 6
                         || !splitPropertyPath[1].StartsWith("data[")
                         || !splitPropertyPath[1].EndsWith("]")
                         || !int.TryParse(splitPropertyPath[1].Substring(5, splitPropertyPath[1].Length - 6), out int index))
                        {
                            continue;
                        }

                        propertyPathBuilder.Append(separator);
                        propertyPathBuilder.Append(splitPropertyPath[0]);
                        propertyPathBuilder.Append(separator);
                        propertyPathBuilder.Append(splitPropertyPath[1]);
                        splitPropertyPath.RemoveRange(0, 2);

                        Type propertyType = GetCollectionType(fieldInfo.FieldType);
                        propertyPath = propertyPathBuilder.ToString();

                        if (!cache.TryGetValue(propertyPath, out cachedPropertyPathInfo))
                        {
                            cachedPropertyPathInfo = new PropertyPathInfo(propertyType, rootType, fieldInfo, currentPropertyPathInfo, currentDepth, index, propertyPath);
                            cache.Add(propertyPath, cachedPropertyPathInfo);
                        }

                        currentPropertyPathInfo = cachedPropertyPathInfo;
                        currentType = propertyType;
                        currentDepth++;
                    }

                    return currentPropertyPathInfo;
                }
            }
        }
    }
}
