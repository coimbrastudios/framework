using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace Coimbra.Listeners
{
    /// <summary>
    /// Listen to <see cref="OnTransformParentChanged"/> callback.
    /// </summary>
    /// <seealso cref="TransformChangedListener"/>
    /// <seealso cref="TransformChildrenChangedListener"/>
    [DisallowMultipleComponent]
    [AddComponentMenu(CoimbraListenersUtility.TransformMenuPath + "Transform Parent Changed Listener")]
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
