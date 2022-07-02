using UnityEngine;

namespace Coimbra
{
    /// <summary>
    /// Add this to a <see cref="MonoBehaviour"/> to listen to PostProcessSceneAttribute callback from outside an editor script.
    /// </summary>
    public interface IScenePostProcessorComponent
    {
        /// <summary>
        /// Called after the scene of the implementor is processed.
        /// </summary>
        void OnPostProcessScene();
    }
}
