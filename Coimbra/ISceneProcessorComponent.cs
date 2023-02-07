using UnityEngine;

namespace Coimbra
{
    /// <summary>
    /// Implement this on a <see cref="MonoBehaviour"/> to listen to IProcessSceneWithReport.OnProcessScene callback from outside an editor script.
    /// </summary>
    /// <seealso cref="IScenePostProcessorComponent"/>
    /// <seealso cref="DebugOnlyComponent"/>
    /// <seealso cref="HierarchyFolder"/>
    public interface ISceneProcessorComponent
    {
        /// <summary>
        /// Called when the scene of the implementor is being processed.
        /// </summary>
        void OnProcessScene();
    }
}
