using System;
using UnityEngine.Serialization;

namespace Coimbra
{
    /// <summary>
    /// Use this attribute to change from a property with a backing field to a explicit field without losing its serialized value.
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
