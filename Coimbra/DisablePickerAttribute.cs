using System;

namespace Coimbra
{
    /// <summary>
    /// Apply to a <see cref="ManagedField{T}"/> to disable selecting another instance in the inspector.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class DisablePickerAttribute : Attribute { }
}
