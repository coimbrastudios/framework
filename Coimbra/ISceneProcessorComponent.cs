using UnityEngine;

namespace Coimbra
{
    /// <summary>
    /// Add this to a <see cref="MonoBehaviour"/> to listen to IProcessSceneWithReport.OnProcessScene callback from outside an editor script.
    /// </summary>
    public interface ISceneProcessorComponent
    {
        /// <summary>
        /// Called when the scene of the implementor is being processed.
        /// </summary>
        void OnProcessScene();
    }
}
