using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace Coimbra.Listeners
{
    /// <summary>
    /// Listen to <see cref="OnCanvasGroupChanged"/> callback.
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu(CoimbraUtility.GeneralMenuPath + "Canvas Group Changed Listener")]
    [HelpURL("https://docs.unity3d.com/Packages/com.unity.ugui@1.0/api/UnityEngine.EventSystems.UIBehaviour.html#UnityEngine_EventSystems_UIBehaviour_OnCanvasGroupChanged")]
    [MovedFrom(true, "Coimbra", "Coimbra")]
    public sealed class CanvasGroupChangedListener : MonoBehaviour
    {
        public delegate void EventHandler(CanvasGroupChangedListener sender);

        /// <summary>
        /// Invoked inside <see cref="OnCanvasGroupChanged"/>.
        /// </summary>
        public event EventHandler OnTrigger;

        private void OnCanvasGroupChanged()
        {
            OnTrigger?.Invoke(this);
        }
    }
}
