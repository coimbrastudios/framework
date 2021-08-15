using UnityEngine;

namespace Coimbra
{
    /// <summary>
    /// Defines the reason why the OnDestroy callback is being called.
    /// </summary>
    public enum DestroyEventType
    {
        /// <summary>
        /// Instigated by a <see cref="Object.Destroy(Object)"/> or <see cref="Object.DestroyImmediate(Object)"/> call (or one of its overloads).
        /// </summary>
        DestroyCall,
        /// <summary>
        /// Instigated by a scene change when the object was not flagged to don't destroy on load.
        /// </summary>
        SceneChange,
        /// <summary>
        /// Being called due the application being shutdown.
        /// </summary>
        ApplicationQuit,
    }
}
