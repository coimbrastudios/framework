#nullable enable

using JetBrains.Annotations;
using UnityEngine;

namespace Coimbra
{
    /// <summary>
    /// Register a method or property to be called when the field is changed.
    /// </summary>
    [PublicAPI]
    public class ValidateAttribute : PropertyAttribute
    {
        /// <summary>
        /// If false, the <see cref="Callback"/> will be called for each user input on it.
        /// </summary>
        public readonly bool Delayed;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidateAttribute"/> class.
        /// </summary>
        /// <param name="callback">The method or property to be called when settings the value.</param>
        /// <param name="delayed">If false, the callback will be called for each user input on it.</param>
        public ValidateAttribute(string callback, bool delayed = true)
        {
            Callback = callback;
            Delayed = delayed;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidateAttribute"/> class.
        /// </summary>
        protected ValidateAttribute() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidateAttribute"/> class.
        /// </summary>
        /// <param name="delayed">If false, the callback will be called for each user input on it.</param>
        protected ValidateAttribute(bool delayed)
        {
            Delayed = delayed;
        }

        /// <summary>
        /// Gets or sets the method or property to be called when setting the value.
        /// <para></para>
        /// If a method is used it will be called after the value is set and it should one of the following signatures: void() or void(T), where T is the previous value.
        /// <para></para>
        /// If a property is used, it will be called before the value is set, passing the new value as the value input.
        /// </summary>
        public string? Callback { get; set; }
    }
}
