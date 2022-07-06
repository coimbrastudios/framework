using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Coimbra
{
    /// <summary>
    /// Utility methods for <see cref="PropertyPathInfo"/> type.
    /// </summary>
    public static class PropertyPathInfoUtility
    {
        private static readonly Dictionary<Type, Dictionary<string, PropertyPathInfo>> PropertyPathInfoMapFromRootType = new();

        /// <summary>
        /// Creates or gets a cached <see cref="PropertyPathInfo"/>.
        /// </summary>
        public static PropertyPathInfo GetPropertyPathInfo(this Object o, in string propertyPath)
        {
            Type type = o.GetType();

            if (!PropertyPathInfoMapFromRootType.TryGetValue(type, out Dictionary<string, PropertyPathInfo> propertyPathInfoMap))
            {
                propertyPathInfoMap = new Dictionary<string, PropertyPathInfo>();
                PropertyPathInfoMapFromRootType.Add(type, propertyPathInfoMap);
            }

            if (!propertyPathInfoMap.TryGetValue(propertyPath, out PropertyPathInfo propertyPathInfo))
            {
                propertyPathInfo = GetPropertyPathInfo(type, propertyPath.Split('.'), propertyPathInfoMap, null);
                propertyPathInfoMap[propertyPath] = propertyPathInfo;
            }

            return propertyPathInfo ?? GetPropertyPathInfo(type, propertyPath.Split('.'), propertyPathInfoMap, o);
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

            return type.FindFieldByName(field);
        }

        private static PropertyPathInfo GetPropertyPathInfo(Type rootType, IEnumerable<string> splitPropertyPathArray, IDictionary<string, PropertyPathInfo> cache, Object target)
        {
            bool isDynamic = target != null;

            using (StringBuilderPool.Pop(out StringBuilder propertyPathBuilder))
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

                    string propertyPath = propertyPathBuilder.ToString();

                    if (fieldInfo == null)
                    {
                        currentPropertyPathInfo = null;

                        break;
                    }

                    currentPropertyPathInfo = GetPropertyPathInfoFromCacheOrCreate(cache, fieldInfo.FieldType, rootType, fieldInfo, currentPropertyPathInfo, currentDepth, null, propertyPath, isDynamic);
                    currentType = currentPropertyPathInfo.GetType(target);
                    currentDepth++;

                    if (!TryGetIndex(splitPropertyPath, out int index))
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
                    currentPropertyPathInfo = GetPropertyPathInfoFromCacheOrCreate(cache, propertyType, rootType, fieldInfo, currentPropertyPathInfo, currentDepth, index, propertyPath, isDynamic);
                    currentType = currentPropertyPathInfo.GetType(target);
                    currentDepth++;
                }

                return currentPropertyPathInfo;
            }
        }

        private static PropertyPathInfo GetPropertyPathInfoFromCacheOrCreate(
            IDictionary<string, PropertyPathInfo> cache,
            [NotNull] Type propertyType,
            [NotNull] Type rootType,
            [NotNull] FieldInfo fieldInfo,
            [CanBeNull] PropertyPathInfo scopeInfo,
            int depth,
            int? index,
            string propertyPath,
            bool isDynamic)
        {
            if (isDynamic)
            {
                return new PropertyPathInfo(propertyType, rootType, fieldInfo, scopeInfo, depth, index, propertyPath, true);
            }

            if (cache.TryGetValue(propertyPath, out PropertyPathInfo cachedPropertyPathInfo))
            {
                return cachedPropertyPathInfo ?? new PropertyPathInfo(propertyType, rootType, fieldInfo, scopeInfo, depth, index, propertyPath, true);
            }

            cachedPropertyPathInfo = new PropertyPathInfo(propertyType, rootType, fieldInfo, scopeInfo, depth, index, propertyPath, false);
            cache[propertyPath] = cachedPropertyPathInfo;

            return cachedPropertyPathInfo;
        }

        private static bool TryGetIndex(IReadOnlyList<string> splitPropertyPath, out int index)
        {
            if (splitPropertyPath.Count > 1
             && splitPropertyPath[0] == "Array"
             && splitPropertyPath[1].Length > 6
             && splitPropertyPath[1].StartsWith("data[")
             && splitPropertyPath[1].EndsWith("]")
             && int.TryParse(splitPropertyPath[1].Substring(5, splitPropertyPath[1].Length - 6), out index))
            {
                return true;
            }

            index = -1;

            return false;
        }
    }
}
