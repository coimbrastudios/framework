using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEditor;

namespace Coimbra.Editor
{
    /// <summary>
    /// Utility methods for <see cref="SerializedProperty"/> type.
    /// </summary>
    public static class SerializedPropertyUtility
    {
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

        /// <inheritdoc cref="PropertyPathInfo.PropertyType"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Type GetPropertyType(this SerializedProperty property)
        {
            return property.GetPropertyPathInfo().PropertyType;
        }

        /// <summary>
        /// Creates or gets a cached <see cref="PropertyPathInfo"/>.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PropertyPathInfo GetPropertyPathInfo(this SerializedObject serializedObject, in string propertyPath)
        {
            return serializedObject.targetObject.GetPropertyPathInfo(in propertyPath);
        }

        /// <summary>
        /// Creates or gets a cached <see cref="PropertyPathInfo"/>.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PropertyPathInfo GetPropertyPathInfo(this SerializedProperty property)
        {
            return property.serializedObject.GetPropertyPathInfo(property.propertyPath);
        }

        /// <inheritdoc cref="PropertyPathInfo.GetScope"/>
        [NotNull]
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object GetScope(this SerializedProperty property)
        {
            return property.GetPropertyPathInfo().GetScope(property.serializedObject.targetObject);
        }

        /// <inheritdoc cref="PropertyPathInfo.GetScope"/>
        [NotNull]
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T GetScope<T>(this SerializedProperty property)
        {
            return property.GetPropertyPathInfo().GetScope<T>(property.serializedObject.targetObject);
        }

        /// <inheritdoc cref="PropertyPathInfo.GetScopes(UnityEngine.Object[])"/>
        [NotNull]
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object[] GetScopes(this SerializedProperty property)
        {
            return property.GetPropertyPathInfo().GetScopes(property.serializedObject.targetObjects);
        }

        /// <inheritdoc cref="PropertyPathInfo.GetScopes(UnityEngine.Object[])"/>
        [NotNull]
        [Pure]
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

        /// <inheritdoc cref="PropertyPathInfo.ScopeInfo"/>
        [CanBeNull]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PropertyPathInfo GetScopeInfo(this SerializedProperty property)
        {
            return property.GetPropertyPathInfo().ScopeInfo;
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

        /// <summary>
        /// Checks if a given property has multiple different <see cref="SerializedProperty.arraySize"/>.
        /// </summary>
        public static bool HasMultipleDifferentArraySizes(this SerializedProperty property)
        {
            if (!property.hasMultipleDifferentValues)
            {
                return false;
            }

            using (ListPool.Pop(out List<ICollection> collections))
            {
                property.GetValues(collections);

                int size = property.arraySize;

                foreach (ICollection collection in collections)
                {
                    if (collection.Count != size)
                    {
                        return true;
                    }
                }

                return false;
            }
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
    }
}
