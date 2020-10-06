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
#if !UNITY_2020_1_OR_NEWER
    public class InterfaceField<T> : IEquatable<InterfaceField<T>>, IEquatable<T>
#else
    public struct InterfaceField<T> : IEquatable<InterfaceField<T>>, IEquatable<T>
#endif
        where T : class
    {
        [SerializeReference] private T _systemObject;
        [SerializeField] private UnityEngine.Object _unityObject;

        private T _value;

#if !UNITY_2020_1_OR_NEWER
        public InterfaceField()
        {
            _systemObject = null;
            _unityObject = null;
        }
#endif

        public InterfaceField(T value)
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

        public T Value
        {
            get => _value;
            set
            {
                _value = value;

                if (_value is UnityEngine.Object o)
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
        public static implicit operator T(InterfaceField<T> target)
        {
            return target.Value;
        }

        [Pure]
        public static implicit operator InterfaceField<T>(T target)
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
                return _value == t;
            }

            if (other is InterfaceField<T> field)
            {
                return _value == field.Value;
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
            return _value == other;
        }

        [Pure]
        public bool Equals(InterfaceField<T> other)
        {
            return _value == other.Value;
        }
    }
}
