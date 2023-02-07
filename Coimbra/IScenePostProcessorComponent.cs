using UnityEngine;

namespace Coimbra
{
    /// <summary>
    /// Implement this on a <see cref="MonoBehaviour"/> to listen to PostProcessSceneAttribute callback from outside an editor script.
    /// </summary>
    /// <seealso cref="ISceneProcessorComponent"/>
    public interface IScenePostProcessorComponent
    {
        /// <summary>
        /// Called after the scene of the implementor is processed.
        /// </summary>
        void OnPostProcessScene();
    }
}
