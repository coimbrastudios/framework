using JetBrains.Annotations;
using System;
using UnityEngine;
using UnityEngine.Scripting;

namespace Coimbra
{
    /// <summary>
    /// Create a reference for any type.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    [Preserve]
    [Serializable]
    public sealed class Reference<T>
    {
        [SerializeField]
        private T _value;

        public Reference()
        {
            _value = default;
        }

        public Reference([CanBeNull] T value)
        {
            _value = value;
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="Value"/> is not null.
        /// </summary>
        public bool HasValue => _value.IsValid();

        /// <summary>
        /// Gets or sets the current value.
        /// </summary>
        [CanBeNull]
        public T Value
        {
            get => _value;
            set => _value = value;
        }

        [Pure]
        [CanBeNull]
        public static implicit operator T([CanBeNull] Reference<T> target)
        {
            return target != default ? target.Value : default;
        }

        [Pure]
        [NotNull]
        public override string ToString()
        {
            return $"ref({_value})";
        }

        public bool TryGetValue([CanBeNull] out T value)
        {
            return _value.TryGetValid(out value);
        }
    }
}
