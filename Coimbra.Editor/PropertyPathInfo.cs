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
        /// Delegate for <see cref="PropertyPathInfo.SetValues{T}(UnityEngine.Object[],bool,SetValuesHandler{T})"/>.
        /// </summary>
        public delegate T SetValuesHandler<out T>(PropertyPathInfo sender, Object target);

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
                InitializeChain();

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
            TryGetValue(target, out T value);

            return value;
        }

        /// <summary>
        /// Get the field value for each target.
        /// </summary>
        [NotNull]
        [Pure]
        public object[] GetValues([NotNull] Object[] targets)
        {
            using (ListPool.Pop(out List<object> list))
            {
                GetValues(targets, list);

                return list.ToArray();
            }
        }

        /// <inheritdoc cref="GetValues(Object[])"/>
        [NotNull]
        [Pure]
        public T[] GetValues<T>([NotNull] Object[] targets)
        {
            using (ListPool.Pop(out List<T> list))
            {
                GetValues(targets, list);

                return list.ToArray();
            }
        }

        /// <inheritdoc cref="GetValues(Object[])"/>
        public void GetValues([NotNull] Object[] targets, [NotNull] List<object> append)
        {
            InitializeChain();
            append.EnsureCapacity(append.Count + targets.Length);

            Parallel.For(append.Count, append.Count + targets.Length, delegate(int i)
            {
                append.Add(GetValue(targets[i]));
            });
        }

        /// <inheritdoc cref="GetValues(Object[])"/>
        public void GetValues<T>([NotNull] Object[] targets, [NotNull] List<T> append)
        {
            InitializeChain();
            append.EnsureCapacity(append.Count + targets.Length);

            Parallel.For(append.Count, append.Count + targets.Length, delegate(int i)
            {
                TryGetValue(targets[i], out T value);
                append.Add(value);
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
        public void SetValue([NotNull] Object target, [CanBeNull] object value)
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
                    propertyPathInfo.FieldInfo.SetValue(currentScope, currentValue);
                }

                currentValue = currentScope;
                propertyPathInfo = propertyPathInfo.Scope;
            }
            while (propertyPathInfo != null);
        }

        /// <summary>
        /// Set the field value for each target.
        /// </summary>
        public void SetValues([NotNull] Object[] targets, [CanBeNull] object value)
        {
            InitializeChain();

            Parallel.ForEach(targets, delegate(Object target)
            {
                SetValue(target, value);
            });
        }

        /// <inheritdoc cref="SetValues(UnityEngine.Object[],object)"/>
        public void SetValues([NotNull] Object[] targets, bool isThreadSafe, [NotNull] SetValuesHandler<object> setter)
        {
            if (isThreadSafe)
            {
                InitializeChain();

                Parallel.ForEach(targets, delegate(Object target)
                {
                    SetValue(target, setter(this, target));
                });
            }
            else
            {
                foreach (Object target in targets)
                {
                    SetValue(target, setter(this, target));
                }
            }
        }

        /// <inheritdoc cref="SetValues(UnityEngine.Object[],object)"/>
        public void SetValues<T>([NotNull] Object[] targets, bool isThreadSafe, [NotNull] SetValuesHandler<T> setter)
        {
            if (isThreadSafe)
            {
                InitializeChain();

                Parallel.ForEach(targets, delegate(Object target)
                {
                    SetValue(target, setter(this, target));
                });
            }
            else
            {
                foreach (Object target in targets)
                {
                    SetValue(target, setter(this, target));
                }
            }
        }

        /// <inheritdoc cref="GetValue"/>
        [Pure]
        public bool TryGetValue<T>([NotNull] Object target, [CanBeNull] out T value)
        {
            try
            {
                value = (T)GetValue(target);

                return true;
            }
            catch
            {
                value = default;

                return false;
            }
        }

        private void InitializeChain()
        {
            if (_chainBackingField != null)
            {
                return;
            }

            PropertyPathInfo current = this;
            _chainBackingField = new PropertyPathInfo[Depth + 1];

            do
            {
                _chainBackingField[current.Depth] = current;
                current = current.Scope;
            }
            while (current != null);
        }
    }
}
