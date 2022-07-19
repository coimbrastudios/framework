using UnityEngine.Scripting;

namespace Coimbra.Services.ApplicationStateEvents
{
    /// <summary>
    /// Responsible for the application lifetime cycle, meant to be used to fire Unity's callbacks events.
    /// </summary>
    [RequiredService]
    [RequireImplementors]
    public interface IApplicationStateService : IService
    {
        /// <summary>
        /// Gets a value indicating whether application is focused.
        /// </summary>
        bool IsFocused { get; }

        /// <summary>
        /// Gets a value indicating whether application is paused.
        /// </summary>
        bool IsPaused { get; }

        /// <summary>
        /// Gets a value indicating whether application is quitting.
        /// </summary>
        bool IsQuitting { get; }

        /// <summary>
        /// Removes all listeners from the specified type.
        /// </summary>
        void RemoveAllListeners<T>()
            where T : IApplicationStateEvent;
    }
}
