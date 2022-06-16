using UnityEngine;

namespace Coimbra
{
    /// <summary>
    /// Listen to <see cref="OnRenderObject"/> callback.
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu(CoimbraUtility.GeneralMenuPath + "Render Object Listener")]
    [HelpURL("https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnRenderObject.html")]
    public sealed class RenderObjectListener : MonoBehaviour
    {
        public delegate void EventHandler(RenderObjectListener sender);

        /// <summary>
        /// Invoked inside <see cref="OnRenderObject"/>.
        /// </summary>
        public event EventHandler OnTrigger;

        private void OnRenderObject()
        {
            OnTrigger?.Invoke(this);
        }
    }
}
