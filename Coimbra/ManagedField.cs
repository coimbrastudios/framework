using JetBrains.Annotations;
using System;
using UnityEngine;
using UnityEngine.Scripting;

namespace Coimbra
{
    /// <summary>
    /// Unified way to expose a managed field in the inspector.
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
        private UnityEngine.Object _unityObject;

        public ManagedField([CanBeNull] T value)
            : this()
        {
            Value = value;
        }

        /// <summary>
        /// True if the <see cref="Value"/> is not null.
        /// </summary>
        public bool HasValue => _systemObject != null || _unityObject != null;

        /// <summary>
        /// Is the <see cref="Value"/> a non-<a href="https://docs.unity3d.com/ScriptReference/Object.html">Unity Object</a>?
        /// </summary>
        public bool IsSystemObject => _systemObject != null;

        /// <summary>
        /// Is the <see cref="Value"/> an <a href="https://docs.unity3d.com/ScriptReference/Object.html">Unity Object</a>?
        /// </summary>
        public bool IsUnityObject => _unityObject != null;

        [CanBeNull]
        public T Value
        {
            get => _systemObject ?? (_unityObject as T).GetValid();
            set
            {
                if (value is UnityEngine.Object o)
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
        public override bool Equals(object other)
        {
            if (other is T t)
            {
                return Value == t;
            }

            if (other is ManagedField<T> field)
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

        public bool TryGetValue(out T value)
        {
            value = _systemObject ?? (_unityObject as T).GetValid();

            return value != null;
        }
    }
}
