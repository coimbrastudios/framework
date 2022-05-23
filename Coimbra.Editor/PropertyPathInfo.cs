using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using UnityEditor;
using Object = UnityEngine.Object;

namespace Coimbra.Editor
{
    /// <summary>
    /// Reflection information for a given <a href="https://docs.unity3d.com/ScriptReference/SerializedProperty-propertyPath.html">SerializedProperty</a>.
    /// To build one use <see cref="PropertyPathInfoUtility.GetPropertyPathInfo(UnityEditor.SerializedProperty)"/>.
    /// </summary>
    public sealed class PropertyPathInfo
    {
        /// <summary>
        /// Receives the current value and returns the new value.
        /// </summary>
        public delegate T SetValueHandler<T>(Object context, T current);

        /// <inheritdoc cref="SerializedProperty.depth"/>
        public readonly int Depth;

        /// <summary>
        /// Null if not an array element.
        /// </summary>
        public readonly int? Index;

        /// <summary>
        /// The field info for this property.
        /// </summary>
        [NotNull]
        public readonly FieldInfo FieldInfo;

        /// <summary>
        /// The root type of this property.
        /// </summary>
        [NotNull]
        public readonly Type RootType;

        /// <summary>
        /// The <see cref="PropertyPathInfo"/> that contains this property. Will always be null if <see cref="Depth"/> is 0, but never null otherwise.
        /// </summary>
        [CanBeNull]
        public readonly PropertyPathInfo Scope;

        private readonly string _propertyPath;

        private PropertyPathInfo[] _chainBackingField;

        internal PropertyPathInfo([NotNull] Type rootType, [NotNull] FieldInfo fieldInfo, [CanBeNull] PropertyPathInfo scope, int depth, int? index, string propertyPath)
        {
            _propertyPath = propertyPath;
            Depth = depth;
            Index = index;
            FieldInfo = fieldInfo;
            RootType = rootType;
            Scope = scope;
        }

        /// <summary>
        /// The chain of <see cref="PropertyPathInfo"/> to reach this one.
        /// </summary>
        public IReadOnlyList<PropertyPathInfo> Chain
        {
            get
            {
                if (_chainBackingField != null)
                {
                    return _chainBackingField;
                }

                PropertyPathInfo current = this;
                _chainBackingField = new PropertyPathInfo[Depth + 1];

                do
                {
                    _chainBackingField[current.Depth] = current;
                    current = current.Scope;
                }
                while (current != null);

                return _chainBackingField;
            }
        }

        public override string ToString()
        {
            return $"{RootType.FullName}.{_propertyPath}";
        }

        /// <summary>
        /// Get the field value.
        /// </summary>
        [CanBeNull]
        [Pure]
        public object GetValue([NotNull] Object target)
        {
            object current = target;

            for (int i = 0; i < Chain.Count; i++)
            {
                current = Chain[i].Index.HasValue ? ((IList)current)[Chain[i].Index.Value] : Chain[i].FieldInfo.GetValue(current);
            }

            return current;
        }

        /// <inheritdoc cref="GetValue"/>
        [CanBeNull]
        [Pure]
        public T GetValue<T>([NotNull] Object target)
        {
            object value = GetValue(target);

            return value != null ? (T)value : default;
        }

        /// <summary>
        /// Get the field value for each context.
        /// </summary>
        [NotNull]
        [Pure]
        public object[] GetValues([NotNull] Object[] targets)
        {
            object[] values = new object[targets.Length];

            Parallel.For(0, targets.Length, delegate(int i)
            {
                values[i] = GetValue(targets[i]);
            });

            return values;
        }

        /// <inheritdoc cref="GetValues(Object[])"/>
        [NotNull]
        [Pure]
        public T[] GetValues<T>([NotNull] Object[] targets)
        {
            T[] values = new T[targets.Length];

            Parallel.For(0, targets.Length, delegate(int i)
            {
                values[i] = GetValue<T>(targets[i]);
            });

            return values;
        }

        /// <inheritdoc cref="GetValues(Object[])"/>
        public void GetValues([NotNull] Object[] targets, [NotNull] List<object> append)
        {
            append.EnsureCapacity(append.Count + targets.Length);

            Parallel.For(append.Count, append.Count + targets.Length, delegate(int i)
            {
                append.Add(GetValue(targets[i]));
            });
        }

        /// <inheritdoc cref="GetValues(Object[])"/>
        public void GetValues<T>([NotNull] Object[] targets, [NotNull] List<T> append)
        {
            append.EnsureCapacity(append.Count + targets.Length);

            Parallel.For(append.Count, append.Count + targets.Length, delegate(int i)
            {
                append.Add(GetValue<T>(targets[i]));
            });
        }

        /// <see cref="SerializedProperty.hasMultipleDifferentValues"/>
        public bool HasMultipleDifferentValues([NotNull] Object[] targets)
        {
            using (ListPool.Pop(out List<object> list))
            {
                GetValues(targets, list);

                using (HashSetPool.Pop(out HashSet<object> hashSet))
                {
                    hashSet.Add(list[0]);

                    for (int i = 1; i < list.Count; i++)
                    {
                        if (hashSet.Add(list[i]))
                        {
                            return true;
                        }
                    }

                    return false;
                }
            }
        }

        /// <summary>
        /// Set the field value.
        /// </summary>
        public void SetValue<T>([NotNull] Object target, [CanBeNull] T value)
        {
            PropertyPathInfo propertyPathInfo = this;
            object currentValue = value;

            do
            {
                object currentScope = propertyPathInfo.Scope?.GetValue(target) ?? target;

                if (propertyPathInfo.Index.HasValue)
                {
                    ((IList)currentScope)[propertyPathInfo.Index.Value] = currentValue;

                    propertyPathInfo = propertyPathInfo.Scope!;
                    currentScope = propertyPathInfo.GetValue(target);
                }
                else
                {
                    FieldInfo.SetValue(currentScope, currentValue);
                }

                currentValue = currentScope;
                propertyPathInfo = propertyPathInfo.Scope;
            }
            while (propertyPathInfo != null);
        }

        /// <inheritdoc cref="SetValue{T}(Object,T)"/>
        public void SetValue<T>([NotNull] Object target, [NotNull] SetValueHandler<T> callback)
        {
            T current = GetValue<T>(target);
            current = callback.Invoke(target, current);
            SetValue(target, current);
        }

        /// <summary>
        /// Set the field value for each context.
        /// </summary>
        public void SetValues<T>([NotNull] Object[] targets, [CanBeNull] T value)
        {
            Parallel.ForEach(targets, delegate(Object target)
            {
                SetValue(target, value);
            });
        }

        /// <inheritdoc cref="SetValues{T}(Object[],T)"/>
        public void SetValues<T>([NotNull] Object[] targets, [NotNull] SetValueHandler<T> callback, bool isThreadSafe)
        {
            if (isThreadSafe)
            {
                Parallel.ForEach(targets, delegate(Object target)
                {
                    SetValue(target, callback);
                });
            }
            else
            {
                foreach (Object target in targets)
                {
                    SetValue(target, callback);
                }
            }
        }
    }
}
