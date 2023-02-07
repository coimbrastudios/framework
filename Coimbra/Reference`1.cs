using JetBrains.Annotations;
using System;
using UnityEngine;
using UnityEngine.Scripting;

namespace Coimbra
{
    /// <summary>
    /// Create a reference for any type.
    /// </summary>
    /// <remarks>
    /// It is compatible with <see cref="FilterTypesAttributeBase"/> attributes.
    /// </remarks>
    /// <typeparam name="T">The value type.</typeparam>
    /// <seealso cref="FilterTypesAttributeBase"/>
    /// <seealso cref="FilterTypesByAccessibilityAttribute"/>
    /// <seealso cref="FilterTypesByMethodAttribute"/>
    /// <seealso cref="FilterTypesByAssignableFromAttribute"/>
    /// <seealso cref="FilterTypesBySpecificTypeAttribute"/>
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
            return target != null ? target.Value : default;
        }

        [Pure]
        [NotNull]
        public override string ToString()
        {
            return $"ref({_value})";
        }
    }
}
