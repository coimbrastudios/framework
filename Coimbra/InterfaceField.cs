using JetBrains.Annotations;
using System;
using UnityEngine;

namespace Coimbra
{
    /// <summary>
    ///     Unified way to expose an interface in the inspector.
    /// </summary>
    /// <typeparam name="T">The interface type.</typeparam>
    [Serializable]
    public struct InterfaceField<T> : IEquatable<InterfaceField<T>>, IEquatable<T>
        where T : class
    {
        [SerializeReference] private T _systemObject;
        [SerializeField] private UnityEngine.Object _unityObject;

        public InterfaceField([CanBeNull] T value)
            : this()
        {
            Value = value;
        }

        /// <summary>
        ///     True if the <see cref="Value"/> is not null.
        /// </summary>
        public bool HasValue => _systemObject != null || _unityObject != null;

        /// <summary>
        ///     Is the <see cref="Value"/> a non-<a href="https://docs.unity3d.com/ScriptReference/Object.html">Unity Object</a>?
        /// </summary>
        public bool IsSystemObject => _systemObject != null;

        /// <summary>
        ///     Is the <see cref="Value"/> an <a href="https://docs.unity3d.com/ScriptReference/Object.html">Unity Object</a>?
        /// </summary>
        public bool IsUnityObject => _unityObject != null;

        [CanBeNull]
        public T Value
        {
            get => _systemObject ?? _unityObject as T;
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

        [CanBeNull] [Pure]
        public static implicit operator T(InterfaceField<T> target)
        {
            return target.Value;
        }

        [Pure]
        public static implicit operator InterfaceField<T>([CanBeNull] T target)
        {
            return new InterfaceField<T>(target);
        }

        [Pure]
        public static bool operator ==(InterfaceField<T> x, InterfaceField<T> y)
        {
            return x.Equals(y);
        }

        [Pure]
        public static bool operator !=(InterfaceField<T> x, InterfaceField<T> y)
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

            if (other is InterfaceField<T> field)
            {
                return Value == field.Value;
            }

            return false;
        }

        [CanBeNull] [Pure]
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
        public bool Equals(InterfaceField<T> other)
        {
            return Value == other.Value;
        }
    }
}
