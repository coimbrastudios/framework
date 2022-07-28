using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Object = UnityEngine.Object;

namespace Coimbra
{
    /// <summary>
    /// Reflection information for a given <a href="https://docs.unity3d.com/ScriptReference/SerializedProperty-propertyPath.html">SerializedProperty</a>.
    /// </summary>
    public sealed class PropertyPathInfo
    {
        /// <summary>
        /// Delegate for <see cref="PropertyPathInfo.SetValues{T}(UnityEngine.Object[],bool,SetValuesHandler{T})"/>.
        /// </summary>
        public delegate T SetValuesHandler<out T>(PropertyPathInfo sender, Object target);

        /// <summary>
        /// If true, this <see cref="PropertyPathInfo"/> is not cached due being dependant on its scope value type.
        /// </summary>
        public readonly bool IsDynamic;

        /// <summary>
        /// Nesting depth of the property.
        /// </summary>
        public readonly int Depth;

        /// <summary>
        /// Null if not an array element.
        /// </summary>
        public readonly int? Index;

        /// <summary>
        /// The field info for this property as it is declared in the object. If you want the actual element type you can use <see cref="PropertyType"/>.
        /// </summary>
        [NotNull]
        public readonly FieldInfo FieldInfo;

        /// <summary>
        /// The property type. Returns the actual type to be drawn so if it is an <see cref="Array"/> or <see cref="List{T}"/>, it will return the item type.
        /// </summary>
        [NotNull]
        public readonly Type PropertyType;

        /// <summary>
        /// The root type of this property. Always a class that inherits from <see cref="Object"/>.
        /// </summary>
        [NotNull]
        public readonly Type RootType;

        /// <summary>
        /// The <see cref="PropertyPathInfo"/> that contains this property. Will always be null if <see cref="Depth"/> is 0, but never null otherwise.
        /// </summary>
        [MaybeNull]
        public readonly PropertyPathInfo ScopeInfo;

        private readonly string _propertyPath;

        private PropertyPathInfo[] _chainBackingField;

        internal PropertyPathInfo([NotNull] Type propertyType, [NotNull] Type rootType, [NotNull] FieldInfo fieldInfo, [MaybeNull] PropertyPathInfo scopeInfo, int depth, int? index, string propertyPath, bool isDynamic)
        {
            _propertyPath = propertyPath;
            Depth = depth;
            Index = index;
            FieldInfo = fieldInfo;
            PropertyType = propertyType;
            RootType = rootType;
            ScopeInfo = scopeInfo;
            IsDynamic = isDynamic;
        }

        /// <summary>
        /// Gets the chain of <see cref="PropertyPathInfo"/> to reach this one.
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
        /// Get the scope value.
        /// </summary>
        [Pure]
        [return: NotNull]
        public object GetScope([NotNull] Object target)
        {
            if (ScopeInfo == null)
            {
                return target;
            }

            if (ScopeInfo.Index.HasValue)
            {
                return ScopeInfo.GetScope(target);
            }

            return ScopeInfo.GetValue(target) ?? target;
        }

        /// <inheritdoc cref="GetScope"/>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NotNull]
        public T GetScope<T>([NotNull] Object target)
        {
            return (T)GetScope(target);
        }

        /// <summary>
        /// Get the scope value for each target.
        /// </summary>
        [Pure]
        [return: NotNull]
        public object[] GetScopes([NotNull] Object[] targets)
        {
            using (ListPool.Pop(out List<object> list))
            {
                GetScopes(targets, list);

                return list.ToArray();
            }
        }

        /// <inheritdoc cref="GetScopes(UnityEngine.Object[])"/>
        [Pure]
        [return: NotNull]
        public T[] GetScopes<T>([NotNull] Object[] targets)
        {
            using (ListPool.Pop(out List<T> list))
            {
                GetScopes(targets, list);

                return list.ToArray();
            }
        }

        /// <inheritdoc cref="GetScopes(UnityEngine.Object[])"/>
        public void GetScopes([NotNull] Object[] targets, [NotNull] List<object> append)
        {
            InitializeChain();
            append.EnsureCapacity(append.Count + targets.Length);

            Parallel.For(append.Count, append.Count + targets.Length, delegate(int i)
            {
                append.Add(GetScope(targets[i]));
            });
        }

        /// <inheritdoc cref="GetScopes(UnityEngine.Object[])"/>
        public void GetScopes<T>([NotNull] Object[] targets, [NotNull] List<T> append)
        {
            InitializeChain();
            append.EnsureCapacity(append.Count + targets.Length);

            Parallel.For(append.Count, append.Count + targets.Length, delegate(int i)
            {
                append.Add(GetScope<T>(targets[i]));
            });
        }

        /// <summary>
        /// Get the current assigned type for the given <paramref name="target"/>. If not <see cref="IsDynamic"/>, will always return the <see cref="PropertyType"/>.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Type GetType([NotNull] Object target)
        {
            return IsDynamic ? GetValue(target)?.GetType() ?? PropertyType : PropertyType;
        }

        /// <summary>
        /// Get the current assigned type for each target. If not <see cref="IsDynamic"/>, will always return the <see cref="PropertyType"/>.
        /// </summary>
        public void GetTypes([NotNull] Object[] targets, [NotNull] List<Type> append)
        {
            if (IsDynamic)
            {
                InitializeChain();
            }

            append.EnsureCapacity(append.Count + targets.Length);

            Parallel.For(append.Count, append.Count + targets.Length, delegate(int i)
            {
                append.Add(GetType(targets[i]));
            });
        }

        /// <inheritdoc cref="GetTypes(UnityEngine.Object[],System.Collections.Generic.List{System.Type})"/>
        public Type[] GetTypes([NotNull] Object[] targets)
        {
            using (ListPool.Pop(out List<Type> list))
            {
                GetTypes(targets, list);

                return list.ToArray();
            }
        }

        /// <summary>
        /// Get the field value.
        /// </summary>
        [Pure]
        [return: MaybeNull]
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
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: MaybeNull]
        public T GetValue<T>([NotNull] Object target)
        {
            TryGetValue(target, out T value);

            return value;
        }

        /// <summary>
        /// Get the field value for each target.
        /// </summary>
        [Pure]
        [return: NotNull]
        public object[] GetValues([NotNull] Object[] targets)
        {
            using (ListPool.Pop(out List<object> list))
            {
                GetValues(targets, list);

                return list.ToArray();
            }
        }

        /// <inheritdoc cref="GetValues(Object[])"/>
        [Pure]
        [return: NotNull]
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
                append.Add(GetValue<T>(targets[i]));
            });
        }

        /// <summary>
        /// Checks if this property represent multiple different values due to multi-object editing.
        /// </summary>
        public bool HasMultipleDifferentValues([NotNull] Object[] targets)
        {
            if (targets.Length <= 1)
            {
                return false;
            }

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
        public void SetValue([NotNull] Object target, [MaybeNull] object value)
        {
            PropertyPathInfo propertyPathInfo = this;
            object currentValue = value;

            do
            {
                object currentScope = propertyPathInfo.ScopeInfo?.GetValue(target) ?? target;

                if (propertyPathInfo.Index.HasValue)
                {
                    ((IList)currentScope)[propertyPathInfo.Index.Value] = currentValue;
                    propertyPathInfo = propertyPathInfo.ScopeInfo!;
                }
                else
                {
                    propertyPathInfo.FieldInfo.SetValue(currentScope, currentValue);
                }

                currentValue = currentScope;
                propertyPathInfo = propertyPathInfo.ScopeInfo;
            }
            while (propertyPathInfo != null);
        }

        /// <summary>
        /// Set the field value for each target.
        /// </summary>
        public void SetValues([NotNull] Object[] targets, [MaybeNull] object value)
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
        public bool TryGetValue<T>([NotNull] Object target, [MaybeNull] out T value)
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
                current = current.ScopeInfo;
            }
            while (current != null);
        }
    }
}
