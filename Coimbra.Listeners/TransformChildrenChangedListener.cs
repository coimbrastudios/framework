using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace Coimbra.Listeners
{
    /// <summary>
    /// Listen to <see cref="OnTransformChildrenChanged"/> callback.
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu(CoimbraListenersUtility.TransformOrHierarchyMenuPath + "Transform Children Changed Listener")]
    [HelpURL("https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnTransformChildrenChanged.html")]
    [MovedFrom(true, "Coimbra", "Coimbra")]
    public sealed class TransformChildrenChangedListener : MonoBehaviour
    {
        public delegate void EventHandler(TransformChildrenChangedListener sender);

        /// <summary>
        /// Invoked inside <see cref="OnTransformChildrenChanged"/>.
        /// </summary>
        public event EventHandler OnTrigger;

        private void OnTransformChildrenChanged()
        {
            OnTrigger?.Invoke(this);
        }
    }
}
