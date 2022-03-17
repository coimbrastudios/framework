using UnityEngine.Scripting;

namespace Coimbra
{
    /// <summary>
    /// Provides easy access to Unity's FixedUpdate callback without requiring the object to be an <see cref="UnityEngine.MonoBehaviour"/>.
    /// </summary>
    [RequireImplementors]
    public interface IFixedUpdateService : IService
    {
        /// <summary>
        /// Add a listener to the FixedUpdate.
        /// </summary>
        void AddListener(IFixedUpdateListener listener);

        /// <summary>
        /// Remove all listeners from the FixedUpdate;
        /// </summary>
        void RemoveAllListeners();

        /// <summary>
        /// Remove a listener from the FixedUpdate.
        /// </summary>
        void RemoveListener(IFixedUpdateListener listener);
    }
}
