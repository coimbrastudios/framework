using UnityEngine.Scripting;

namespace Coimbra
{
    /// <summary>
    /// Provides easy access to Unity's LateUpdate callback without requiring the object to be an <see cref="UnityEngine.MonoBehaviour"/>.
    /// </summary>
    [RequireImplementors]
    public interface ILateUpdateService
    {
        /// <summary>
        /// Add a listener to the LateUpdate.
        /// </summary>
        void AddListener(ILateUpdateListener listener);

        /// <summary>
        /// Remove all listeners from the LateUpdate;
        /// </summary>
        void RemoveAllListeners();

        /// <summary>
        /// Remove a listener from the LateUpdate.
        /// </summary>
        void RemoveListener(ILateUpdateListener listener);
    }
}
