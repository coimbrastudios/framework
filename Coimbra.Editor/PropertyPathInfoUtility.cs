using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

namespace Coimbra.Editor
{
    /// <summary>
    /// Extensions to make easier to use the <see cref="PropertyPathInfo"/>.
    /// </summary>
    public static class PropertyPathInfoUtility
    {
        private const BindingFlags PropertyPathInfoFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy;

        private static readonly Dictionary<Type, Dictionary<string, PropertyPathInfo>> PropertyPathInfoMapFromType = new Dictionary<Type, Dictionary<string, PropertyPathInfo>>();

        /// <inheritdoc cref="PropertyPathInfo.GetFieldInfo"/>
        [NotNull]
        public static FieldInfo GetFieldInfo(this SerializedProperty property)
        {
            return property.GetPropertyPathInfo().GetFieldInfo(property.serializedObject.targetObject);
        }

        /// <inheritdoc cref="PropertyPathInfo.GetIndex"/>
        public static int? GetIndex(this SerializedProperty property)
        {
            return property.GetPropertyPathInfo().GetIndex(property.serializedObject.targetObject);
        }

        /// <summary>
        /// Creates or gets a cached <see cref="PropertyPathInfo"/>.
        /// </summary>
        public static PropertyPathInfo GetPropertyPathInfo(this SerializedProperty property)
        {
            Type targetType = property.serializedObject.targetObject.GetType();

            if (!PropertyPathInfoMapFromType.TryGetValue(targetType, out Dictionary<string, PropertyPathInfo> propertyPathInfoMap))
            {
                propertyPathInfoMap = new Dictionary<string, PropertyPathInfo>();
                PropertyPathInfoMapFromType.Add(targetType, propertyPathInfoMap);
            }

            string propertyPath = property.propertyPath;

            if (propertyPathInfoMap.TryGetValue(propertyPath, out PropertyPathInfo propertyPathInfo))
            {
                return propertyPathInfo;
            }

            using (ListPool.Pop(out List<string> splitPropertyPath))
            {
                splitPropertyPath.AddRange(propertyPath.Split('.'));

                propertyPathInfo = GetPropertyPathInfoRecursive(targetType, splitPropertyPath);
            }

            propertyPathInfoMap.Add(propertyPath, propertyPathInfo);

            return propertyPathInfo;
        }

        /// <inheritdoc cref="PropertyPathInfo.GetScope"/>
        [CanBeNull]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object GetScope(this SerializedProperty property)
        {
            return property.GetPropertyPathInfo().GetScope(property.serializedObject.targetObject);
        }

        /// <inheritdoc cref="PropertyPathInfo.GetScope"/>
        [CanBeNull]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T GetScope<T>(this SerializedProperty property)
        {
            return property.GetPropertyPathInfo().GetScope<T>(property.serializedObject.targetObject);
        }

        /// <inheritdoc cref="PropertyPathInfo.GetScopes(UnityEngine.Object[])"/>
        [NotNull]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object[] GetScopes(this SerializedProperty property)
        {
            return property.GetPropertyPathInfo().GetScopes(property.serializedObject.targetObjects);
        }

        /// <inheritdoc cref="PropertyPathInfo.GetScopes(UnityEngine.Object[])"/>
        [NotNull]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T[] GetScopes<T>(this SerializedProperty property)
        {
            return property.GetPropertyPathInfo().GetScopes<T>(property.serializedObject.targetObjects);
        }

        /// <inheritdoc cref="PropertyPathInfo.GetScopes(UnityEngine.Object[])"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void GetScopes(this SerializedProperty property, [NotNull] List<object> append)
        {
            property.GetPropertyPathInfo().GetScopes(property.serializedObject.targetObjects, append);
        }

        /// <inheritdoc cref="PropertyPathInfo.GetScopes(UnityEngine.Object[])"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void GetScopes<T>(this SerializedProperty property, [NotNull] List<T> append)
        {
            property.GetPropertyPathInfo().GetScopes(property.serializedObject.targetObjects, append);
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

        /// <inheritdoc cref="PropertyPathInfo.SetValue{T}(UnityEngine.Object,T)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetValue<T>(this SerializedProperty property, [CanBeNull] T value)
        {
            property.GetPropertyPathInfo().SetValue(property.serializedObject.targetObject, value);
        }

        /// <inheritdoc cref="PropertyPathInfo.SetValue{T}(UnityEngine.Object,T)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetValue<T>(this SerializedProperty property, [NotNull] PropertyPathInfo.SetValueHandler<T> onSetValue)
        {
            property.GetPropertyPathInfo().SetValue(property.serializedObject.targetObject, onSetValue);
        }

        /// <inheritdoc cref="PropertyPathInfo.SetValues{T}(UnityEngine.Object[],T)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetValues<T>(this SerializedProperty property, [CanBeNull] T value)
        {
            property.GetPropertyPathInfo().SetValues(property.serializedObject.targetObjects, value);
        }

        /// <inheritdoc cref="PropertyPathInfo.SetValues{T}(UnityEngine.Object[],T)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetValues<T>(this SerializedProperty property, [NotNull] PropertyPathInfo.SetValueHandler<T> onSetValue)
        {
            property.GetPropertyPathInfo().SetValues(property.serializedObject.targetObjects, onSetValue);
        }

        private static Type GetCollectionType(this Type type)
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

        [CanBeNull]
        private static PropertyPathInfo GetPropertyPathInfoRecursive(Type targetType, List<string> splitPropertyPath)
        {
            if (splitPropertyPath.Count == 0)
            {
                return null;
            }

            FieldInfo fieldInfo = GetField(targetType, splitPropertyPath[0]);

            if (fieldInfo == null)
            {
                return null;
            }

            if (splitPropertyPath.Count > 2 && splitPropertyPath[1] == "Array")
            {
                string index = new string(splitPropertyPath[2].Where(char.IsDigit).ToArray());

                if (splitPropertyPath[2].Replace(index, "") == "data[]")
                {
                    splitPropertyPath.RemoveRange(0, 3);

                    PropertyPathInfo nextInfo = GetPropertyPathInfoRecursive(fieldInfo.FieldType.GetCollectionType(), splitPropertyPath);

                    return new PropertyPathInfo(fieldInfo, nextInfo, Convert.ToInt32(index));
                }
            }

            splitPropertyPath.RemoveAt(0);

            return new PropertyPathInfo(fieldInfo, GetPropertyPathInfoRecursive(fieldInfo.FieldType, splitPropertyPath));
        }
    }
}
