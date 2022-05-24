using JetBrains.Annotations;
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

        /// <summary>
        /// Creates or gets a cached <see cref="PropertyPathInfo"/>.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PropertyPathInfo GetPropertyPathInfo(this SerializedObject serializedObject, in string propertyPath)
        {
            return serializedObject.targetObject.GetType().GetPropertyPathInfo(in propertyPath);
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
    }
}
