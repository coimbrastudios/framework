using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace Coimbra.Editor
{
    /// <summary>
    /// Reflection information for a given <a href="https://docs.unity3d.com/ScriptReference/SerializedProperty-propertyPath.html">SerializedProperty</a>.
    /// To build one use <see cref="PropertyPathInfoExtensions.GetPropertyPathInfo(UnityEditor.SerializedProperty)"/>.
    /// </summary>
    public sealed class PropertyPathInfo
    {
        /// <summary>
        /// Receives the old value and expects the new value.
        /// </summary>
        public delegate T SetValueHandler<T>(T oldValue);

        /// <summary>
        /// The field info for this property.
        /// </summary>
        public readonly FieldInfo FieldInfo;

        /// <summary>
        /// Null if not an array element.
        /// </summary>
        public readonly int? Index;

        private string _propertyPath;

        internal PropertyPathInfo(FieldInfo fieldInfo, PropertyPathInfo next, int? index = null)
        {
            _propertyPath = null;
            FieldInfo = fieldInfo;
            Next = next;
            Index = index;
        }

        private PropertyPathInfo Next { get; }

        public override string ToString()
        {
            if (_propertyPath != null)
            {
                return _propertyPath;
            }

            StringBuilder builder = new StringBuilder();
            PropertyPathInfo current = this;

            do
            {
                builder.Append($"{current.FieldInfo.Name}");

                if (current.Index.HasValue)
                {
                    builder.Append($"[{current.Index}]");
                }

                current = current.Next;

                if (current == null)
                {
                    break;
                }

                builder.Append(".");
            }
            while (true);

            _propertyPath = builder.ToString();

            return _propertyPath;
        }

        /// <summary>
        /// Get the object that contains that field.
        /// </summary>
        [CanBeNull] [Pure]
        public object GetScope([NotNull] Object context)
        {
            PropertyPathInfo propertyPathInfo = this;

            return GetScopeInternal(ref propertyPathInfo, context);
        }

        /// <inheritdoc cref="GetScope"/>
        [CanBeNull]
        public T GetScope<T>([NotNull] Object context)
        {
            object value = GetScope(context);

            return value != null ? (T)value : default;
        }

        /// <summary>
        /// Get the object that contains that field for each context.
        /// </summary>
        [NotNull] [Pure]
        public object[] GetScopes([NotNull] Object[] context)
        {
            object[] values = new object[context.Length];

            for (int i = 0; i < context.Length; i++)
            {
                values[i] = GetScope(context[i]);
            }

            return values;
        }

        /// <inheritdoc cref="GetScopes(Object[])"/>
        [NotNull] [Pure]
        public T[] GetScopes<T>([NotNull] Object[] context)
        {
            T[] values = new T[context.Length];

            for (int i = 0; i < context.Length; i++)
            {
                values[i] = GetScope<T>(context[i]);
            }

            return values;
        }

        /// <inheritdoc cref="GetScopes(Object[])"/>
        public void GetScopes([NotNull] Object[] context, [NotNull] List<object> append)
        {
            int capacity = append.Count + context.Length;

            if (append.Capacity < capacity)
            {
                append.Capacity = capacity;
            }

            foreach (Object target in context)
            {
                object result = GetScope(target);
                append.Add(result);
            }
        }

        /// <inheritdoc cref="GetScopes(Object[])"/>
        public void GetScopes<T>([NotNull] Object[] context, [NotNull] List<T> append)
        {
            int capacity = append.Count + context.Length;

            if (append.Capacity < capacity)
            {
                append.Capacity = capacity;
            }

            foreach (Object target in context)
            {
                T result = GetScope<T>(target);
                append.Add(result);
            }
        }

        /// <summary>
        /// Get the field value.
        /// </summary>
        [CanBeNull] [Pure]
        public object GetValue([NotNull] Object context)
        {
            PropertyPathInfo propertyPathInfo = this;
            object scope = GetScopeInternal(ref propertyPathInfo, context);

            return GetValueInternal(propertyPathInfo, scope);
        }

        /// <inheritdoc cref="GetValue"/>
        [CanBeNull] [Pure]
        public T GetValue<T>([NotNull] Object context)
        {
            object value = GetValue(context);

            return value != null ? (T)value : default;
        }

        /// <summary>
        /// Get the field value for each context.
        /// </summary>
        [NotNull] [Pure]
        public object[] GetValues([NotNull] Object[] context)
        {
            object[] values = new object[context.Length];

            for (int i = 0; i < context.Length; i++)
            {
                values[i] = GetValue(context[i]);
            }

            return values;
        }

        /// <inheritdoc cref="GetValues(Object[])"/>
        [NotNull] [Pure]
        public T[] GetValues<T>([NotNull] Object[] context)
        {
            T[] values = new T[context.Length];

            for (int i = 0; i < context.Length; i++)
            {
                values[i] = GetValue<T>(context[i]);
            }

            return values;
        }

        /// <inheritdoc cref="GetValues(Object[])"/>
        public void GetValues([NotNull] Object[] context, [NotNull] List<object> append)
        {
            int capacity = append.Count + context.Length;

            if (append.Capacity < capacity)
            {
                append.Capacity = capacity;
            }

            for (int i = 0; i < context.Length; i++)
            {
                append.Add(GetValue(context[i]));
            }
        }

        /// <inheritdoc cref="GetValues(Object[])"/>
        public void GetValues<T>([NotNull] Object[] context, [NotNull] List<T> append)
        {
            int capacity = append.Count + context.Length;

            if (append.Capacity < capacity)
            {
                append.Capacity = capacity;
            }

            for (int i = 0; i < context.Length; i++)
            {
                append.Add(GetValue<T>(context[i]));
            }
        }

        /// <summary>
        /// Set the field value.
        /// </summary>
        public void SetValue<T>([NotNull] Object context, [CanBeNull] T value)
        {
            object target = context;
            PropertyPathInfo current = this;

            while (current.Next != null)
            {
                if (current.Index.HasValue == false)
                {
                    target = current.FieldInfo.GetValue(target);
                }
                else
                {
                    IEnumerator enumerator = ((IEnumerable)current.FieldInfo.GetValue(target)).GetEnumerator();

                    for (int i = 0; enumerator.MoveNext(); i++)
                    {
                        if (current.Index == i)
                        {
                            target = enumerator.Current;

                            break;
                        }
                    }
                }

                current = current.Next;
            }

            if (current.Index.HasValue == false)
            {
                current.FieldInfo.SetValue(target, value);
            }
            else if (current.FieldInfo.GetValue(target) is T[] array)
            {
                if (array.Length > current.Index)
                {
                    array[current.Index.Value] = value;
                }
                else
                {
                    T[] temp = new T[array.Length + 1];
                    array.CopyTo(temp, 0);
                    temp[array.Length] = value;
                    array = temp;
                }

                current.FieldInfo.SetValue(target, array);
            }
        }

        /// <inheritdoc cref="SetValue{T}(Object,T)"/>
        public void SetValue<T>([NotNull] Object target, [NotNull] SetValueHandler<T> callback)
        {
            T oldValue = GetValue<T>(target);
            T newValue = callback.Invoke(oldValue);
            SetValue(target, newValue);
        }

        /// <summary>
        /// Set the field value for each context.
        /// </summary>
        public void SetValues<T>([NotNull] Object[] targets, [CanBeNull] T value)
        {
            foreach (Object target in targets)
            {
                SetValue(target, value);
            }
        }

        /// <inheritdoc cref="SetValues{T}(Object[],T)"/>
        public void SetValues<T>([NotNull] Object[] targets, [NotNull] SetValueHandler<T> callback)
        {
            foreach (Object target in targets)
            {
                SetValue(target, callback);
            }
        }

        private static object GetScopeInternal(ref PropertyPathInfo propertyPathInfo, object context)
        {
            for (; propertyPathInfo.Next != null; propertyPathInfo = propertyPathInfo.Next)
            {
                context = GetValueInternal(propertyPathInfo, context);
            }

            return context;
        }

        private static object GetValueInternal(PropertyPathInfo propertyPathInfo, object context)
        {
            if (propertyPathInfo.Index.HasValue == false)
            {
                return propertyPathInfo.FieldInfo.GetValue(context);
            }

            IEnumerator enumerator = ((IEnumerable)propertyPathInfo.FieldInfo.GetValue(context)).GetEnumerator();

            for (int i = 0; enumerator.MoveNext(); i++)
            {
                if (propertyPathInfo.Index == i)
                {
                    return enumerator.Current;
                }
            }

            return null;
        }
    }
}
