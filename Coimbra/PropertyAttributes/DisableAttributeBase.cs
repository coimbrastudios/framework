using System;
using UnityEngine;

namespace Coimbra
{
    /// <summary>
    /// Base class to disable the field on the inspector.
    /// </summary>
    /// <seealso cref="DisableAttribute"/>
    /// <seealso cref="DisableOnEditModeAttribute"/>
    /// <seealso cref="DisableOnPlayModeAttribute"/>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public abstract class DisableAttributeBase : PropertyAttribute
    {
        /// <summary>
        /// Implement this method to determine if the field should be disabled.
        /// </summary>
        /// <returns>True if the field should be disabled.</returns>
        public abstract bool ShouldDisableGUI();
    }
}
