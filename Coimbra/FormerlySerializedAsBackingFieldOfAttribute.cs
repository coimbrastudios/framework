using System;
using UnityEngine.Serialization;

namespace Coimbra
{
    /// <summary>
    ///   <para>Use this attribute to rename a field without losing its serialized value.</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public sealed class FormerlySerializedAsBackingFieldOfAttribute : FormerlySerializedAsAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FormerlySerializedAsBackingFieldOfAttribute"/> class.
        /// </summary>
        public FormerlySerializedAsBackingFieldOfAttribute(string propertyName)
            : base($"<{propertyName}>k__BackingField") { }
    }
}
