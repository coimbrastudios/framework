using JetBrains.Annotations;
using System;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEngine.Scripting;
using Object = UnityEngine.Object;

namespace Coimbra
{
    /// <summary>
    /// Unified way to expose a managed field in the inspector. For it to be drawn it needs to either be a <see cref="Object"/> or satisfy the following conditions:
    /// <list type="bullet">
    /// <item>Not be abstract</item>
    /// <item>Not be generic</item>
    /// <item>Have the <see cref="SerializableAttribute"/></item>
    /// <item>Have a parameterless constructor</item>
    /// </list>
    /// </summary>
    /// <typeparam name="T">The managed type.</typeparam>
    [Preserve]
    [Serializable]
    public struct ManagedField<T> : IEquatable<ManagedField<T>>, IEquatable<T>
        where T : class
    {
        [SerializeReference]
        private T _systemObject;

        [SerializeField]
        private Object _unityObject;

        public ManagedField([CanBeNull] T value)
            : this()
        {
            Value = value;
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="Value"/> is not null.
        /// </summary>
        public bool HasValue => _systemObject != null || _unityObject != null;

        /// <summary>
        /// Gets a value indicating whether the <see cref="Value"/> is a non-<a href="https://docs.unity3d.com/ScriptReference/Object.html">Unity Object</a>.
        /// </summary>
        public bool IsSystemObject => _systemObject != null;

        /// <summary>
        /// Gets a value indicating whether the <see cref="Value"/> is an <a href="https://docs.unity3d.com/ScriptReference/Object.html">Unity Object</a>.
        /// </summary>
        public bool IsUnityObject => _unityObject != null;

        [CanBeNull]
        public T Value
        {
            get => _systemObject ?? (_unityObject as T).GetValid();
            set
            {
                if (value is Object o)
                {
                    _unityObject = o;
                    _systemObject = null;
                }
                else
                {
                    _systemObject = value;
                    _unityObject = null;
                }
            }
        }

        [Pure]
        [CanBeNull]
        public static implicit operator T(ManagedField<T> target)
        {
            return target.Value;
        }

        [Pure]
        public static implicit operator ManagedField<T>([CanBeNull] T target)
        {
            return new ManagedField<T>(target);
        }

        [Pure]
        public static bool operator ==(ManagedField<T> x, ManagedField<T> y)
        {
            return x.Equals(y);
        }

        [Pure]
        public static bool operator !=(ManagedField<T> x, ManagedField<T> y)
        {
            return !x.Equals(y);
        }

        [Pure]
        public override int GetHashCode()
        {
            if (_systemObject != null)
            {
                return _systemObject.GetHashCode();
            }

            if (_unityObject != null)
            {
                return _unityObject.GetHashCode();
            }

            return 0;
        }

        [Pure]
        public override bool Equals(object obj)
        {
            if (obj is T t)
            {
                return Value == t;
            }

            if (obj is ManagedField<T> field)
            {
                return Value == field.Value;
            }

            return false;
        }

        [Pure]
        [CanBeNull]
        public override string ToString()
        {
            if (_systemObject != null)
            {
                return _systemObject.ToString();
            }

            if (_unityObject != null)
            {
                return _unityObject.ToString();
            }

            return null;
        }

        [Pure]
        public bool Equals(T other)
        {
            return Value == other;
        }

        [Pure]
        public bool Equals(ManagedField<T> other)
        {
            return Value == other.Value;
        }

        public bool TryGetValue([NotNullWhen(true)] out T value)
        {
            value = _systemObject ?? (_unityObject as T).GetValid();

            return value != null;
        }
    }
}
