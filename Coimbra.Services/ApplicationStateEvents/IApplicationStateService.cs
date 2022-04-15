using UnityEngine.Scripting;

namespace Coimbra.Services.ApplicationStateEvents
{
    /// <summary>
    /// Responsible for the application lifetime cycle, meant to be used to fire Unity's callbacks events.
    /// </summary>
    [RequireImplementors]
    public interface IApplicationStateService : IService
    {
        /// <summary>
        /// True when application is focused.
        /// </summary>
        bool IsFocused { get; }

        /// <summary>
        /// True when application is paused.
        /// </summary>
        bool IsPaused { get; }

        /// <summary>
        /// True when application is quitting.
        /// </summary>
        bool IsQuitting { get; }
    }
}
