using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Coimbra.Editor
{
    /// <summary>
    /// Utility methods for <see cref="PropertyPathInfo"/> type.
    /// </summary>
    public static class PropertyPathInfoUtility
    {
        private const BindingFlags PropertyPathInfoFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy;

        private static readonly Dictionary<Type, Dictionary<string, PropertyPathInfo>> PropertyPathInfoMapFromRootType = new Dictionary<Type, Dictionary<string, PropertyPathInfo>>();

        /// <inheritdoc cref="PropertyPathInfo.FieldInfo"/>
        [NotNull]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FieldInfo GetFieldInfo(this SerializedProperty property)
        {
            return property.GetPropertyPathInfo().FieldInfo;
        }

        /// <inheritdoc cref="PropertyPathInfo.Index"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int? GetIndex(this SerializedProperty property)
        {
            return property.GetPropertyPathInfo().Index;
        }

        /// <summary>
        /// Creates or gets a cached <see cref="PropertyPathInfo"/>.
        /// </summary>
        public static PropertyPathInfo GetPropertyPathInfo(this SerializedObject serializedObject, in string propertyPath)
        {
            Type rootType = serializedObject.targetObject.GetType();

            if (!PropertyPathInfoMapFromRootType.TryGetValue(rootType, out Dictionary<string, PropertyPathInfo> propertyPathInfoMap))
            {
                propertyPathInfoMap = new Dictionary<string, PropertyPathInfo>();
                PropertyPathInfoMapFromRootType.Add(rootType, propertyPathInfoMap);
            }

            if (propertyPathInfoMap.TryGetValue(propertyPath, out PropertyPathInfo propertyPathInfo))
            {
                return propertyPathInfo;
            }

            propertyPathInfo = GetPropertyPathInfo(rootType, propertyPath.Split('.'), propertyPathInfoMap);
            propertyPathInfoMap[propertyPath] = propertyPathInfo;

            return propertyPathInfo;
        }

        /// <summary>
        /// Creates or gets a cached <see cref="PropertyPathInfo"/>.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PropertyPathInfo GetPropertyPathInfo(this SerializedProperty property)
        {
            return property.serializedObject.GetPropertyPathInfo(property.propertyPath);
        }

        /// <inheritdoc cref="PropertyPathInfo.Scope"/>
        [CanBeNull]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PropertyPathInfo GetScope(this SerializedProperty property)
        {
            return property.GetPropertyPathInfo().Scope;
        }

        /// <inheritdoc cref="PropertyPathInfo.GetValue"/>
        [CanBeNull]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object GetValue(this SerializedProperty property)
        {
            return property.GetPropertyPathInfo().GetValue(property.serializedObject.targetObject);
        }

        /// <inheritdoc cref="PropertyPathInfo.GetValue"/>
        [CanBeNull]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T GetValue<T>(this SerializedProperty property)
        {
            return property.GetPropertyPathInfo().GetValue<T>(property.serializedObject.targetObject);
        }

        /// <inheritdoc cref="PropertyPathInfo.GetValues(UnityEngine.Object[])"/>
        [NotNull]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object[] GetValues(this SerializedProperty property)
        {
            return property.GetPropertyPathInfo().GetValues(property.serializedObject.targetObjects);
        }

        /// <inheritdoc cref="PropertyPathInfo.GetValues(UnityEngine.Object[])"/>
        [NotNull]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T[] GetValues<T>(this SerializedProperty property)
        {
            return property.GetPropertyPathInfo().GetValues<T>(property.serializedObject.targetObjects);
        }

        /// <inheritdoc cref="PropertyPathInfo.GetValues(UnityEngine.Object[])"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void GetValues(this SerializedProperty property, [NotNull] List<object> append)
        {
            property.GetPropertyPathInfo().GetValues(property.serializedObject.targetObjects, append);
        }

        /// <inheritdoc cref="PropertyPathInfo.GetValues(UnityEngine.Object[])"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void GetValues<T>(this SerializedProperty property, [NotNull] List<T> append)
        {
            property.GetPropertyPathInfo().GetValues(property.serializedObject.targetObjects, append);
        }

        /// <inheritdoc cref="PropertyPathInfo.SetValue(UnityEngine.Object,object)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetValue(this SerializedProperty property, [CanBeNull] object value)
        {
            property.GetPropertyPathInfo().SetValue(property.serializedObject.targetObject, value);
        }

        /// <inheritdoc cref="PropertyPathInfo.SetValues(UnityEngine.Object[],object)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetValues(this SerializedProperty property, [CanBeNull] object value)
        {
            property.GetPropertyPathInfo().SetValues(property.serializedObject.targetObjects, value);
        }

        /// <inheritdoc cref="PropertyPathInfo.SetValues(UnityEngine.Object[],object)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetValues(this SerializedProperty property, bool isThreadSafe, [NotNull] PropertyPathInfo.SetValuesHandler<object> setter)
        {
            property.GetPropertyPathInfo().SetValues(property.serializedObject.targetObjects, isThreadSafe, setter);
        }


        /// <inheritdoc cref="PropertyPathInfo.SetValues(UnityEngine.Object[],object)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetValues<T>(this SerializedProperty property, bool isThreadSafe, [NotNull] PropertyPathInfo.SetValuesHandler<T> setter)
        {
            property.GetPropertyPathInfo().SetValues(property.serializedObject.targetObjects, isThreadSafe, setter);
        }

        /// <inheritdoc cref="PropertyPathInfo.GetValue"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetValue<T>(this SerializedProperty property, [CanBeNull] out T value)
        {
            return property.GetPropertyPathInfo().TryGetValue(property.serializedObject.targetObject, out value);
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
                            cachedPropertyPathInfo = new PropertyPathInfo(rootType, fieldInfo, currentPropertyPathInfo, currentDepth, null, propertyPath);
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
                        propertyPath = propertyPathBuilder.ToString();

                        if (!cache.TryGetValue(propertyPath, out cachedPropertyPathInfo))
                        {
                            cachedPropertyPathInfo = new PropertyPathInfo(rootType, fieldInfo, currentPropertyPathInfo, currentDepth, index, propertyPath);
                            cache.Add(propertyPath, cachedPropertyPathInfo);
                        }

                        currentPropertyPathInfo = cachedPropertyPathInfo;
                        currentType = GetCollectionType(fieldInfo.FieldType);
                        currentDepth++;
                    }

                    return currentPropertyPathInfo;
                }
            }
        }
    }
}
