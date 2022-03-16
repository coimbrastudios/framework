using UnityEngine.Scripting;

namespace Coimbra
{
    /// <summary>
    /// Provides easy access to Unity's Update callback without requiring the object to be an <see cref="UnityEngine.MonoBehaviour"/>.
    /// </summary>
    [RequireImplementors]
    public interface IUpdateService
    {
        /// <summary>
        /// Add a listener to the Update.
        /// </summary>
        void AddListener(IUpdateListener listener);

        /// <summary>
        /// Remove all listeners from the Update;
        /// </summary>
        void RemoveAllListeners();

        /// <summary>
        /// Remove a listener from the Update.
        /// </summary>
        void RemoveListener(IUpdateListener listener);
    }
}
