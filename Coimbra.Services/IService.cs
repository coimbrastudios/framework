using System;
using UnityEngine.Scripting;

namespace Coimbra.Services
{
    /// <summary>
    /// Base interface for any service to be used with <see cref="ServiceLocator"/>.
    /// </summary>
    [RequireImplementors]
    public interface IService : IDisposable
    {
        /// <summary>
        /// The <see cref="ServiceLocator"/> that owns this service. <see cref="set_OwningLocator"/> is for internal use only.
        /// </summary>
        ServiceLocator OwningLocator { get; set; }

        /// <summary>
        /// Will be called after the <see cref="OwningLocator"/> has been set and only if the value actually changed.
        /// </summary>
        /// <param name="previous">The value before.</param>
        /// <param name="current">The value after. Is the same as the current <see cref="OwningLocator"/>.</param>
        protected virtual void OnOwningLocatorChanged(ServiceLocator previous, ServiceLocator current) { }

        internal void SetOwningLocator(ServiceLocator locator)
        {
            ServiceLocator previous = OwningLocator;

            if (previous == locator)
            {
                return;
            }

            OwningLocator = locator;
            OnOwningLocatorChanged(previous, locator);
        }
    }
}
