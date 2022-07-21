using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace Coimbra.Listeners
{
    /// <summary>
    /// Listen to <see cref="OnTransformParentChanged"/> callback.
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu(CoimbraListenersUtility.TransformOrHierarchyMenuPath + "Transform Parent Changed Listener")]
    [HelpURL("https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnTransformParentChanged.html")]
    [MovedFrom(true, "Coimbra", "Coimbra")]
    public sealed class TransformParentChangedListener : MonoBehaviour
    {
        public delegate void EventHandler(TransformParentChangedListener sender);

        /// <summary>
        /// Invoked inside <see cref="OnTransformParentChanged"/>.
        /// </summary>
        public event EventHandler OnTrigger;

        private void OnTransformParentChanged()
        {
            OnTrigger?.Invoke(this);
        }
    }
}
