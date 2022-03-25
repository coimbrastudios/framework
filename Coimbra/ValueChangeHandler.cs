using JetBrains.Annotations;

namespace Coimbra
{
    /// <summary>
    /// Generic delegate for value changes.
    /// </summary>
    public delegate void ValueChangeHandler<in T>([CanBeNull] T oldValue, [CanBeNull] T newValue);
}
