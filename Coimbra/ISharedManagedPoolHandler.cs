using System;

namespace Coimbra
{
    /// <summary>
    /// Listen for pop and push events from <see cref="ManagedPool"/>.
    /// </summary>
    public interface ISharedManagedPoolHandler : IDisposable
    {
        /// <summary>
        /// Called when picking this instance from the pool.
        /// </summary>
        void OnPop();

        /// <summary>
        /// Called when returning this instance to the pool.
        /// </summary>
        void OnPush();
    }
}
