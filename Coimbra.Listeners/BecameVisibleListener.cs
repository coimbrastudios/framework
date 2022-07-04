using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace Coimbra.Listeners
{
    /// <summary>
    /// Listen to <see cref="Renderer"/>'s <see cref="OnBecameVisible"/> callback.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Renderer))]
    [AddComponentMenu(CoimbraListenersUtility.RenderingMenuPath + "Became Visible Listener")]
    [HelpURL("https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnBecameVisible.html")]
    [MovedFrom(true, "Coimbra", "Coimbra")]
    public sealed class BecameVisibleListener : MonoBehaviour
    {
        public delegate void EventHandler(BecameVisibleListener sender);

        /// <summary>
        /// Invoked inside <see cref="OnBecameVisible"/>.
        /// </summary>
        public event EventHandler OnTrigger;

        private Renderer _renderer;

        /// <summary>
        /// Gets the renderer this component depends on.
        /// </summary>
        public Renderer Renderer => _renderer != null ? _renderer : _renderer = GetComponent<Renderer>();

        private void OnBecameVisible()
        {
            OnTrigger?.Invoke(this);
        }
    }
}
