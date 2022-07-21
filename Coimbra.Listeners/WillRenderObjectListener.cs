using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace Coimbra.Listeners
{
    /// <summary>
    /// Listen to <see cref="Renderer"/>'s <see cref="OnWillRenderObject"/> callback.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Renderer))]
    [AddComponentMenu(CoimbraListenersUtility.RenderingMenuPath + "Will Render Object Listener")]
    [HelpURL("https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnWillRenderObject.html")]
    [MovedFrom(true, "Coimbra", "Coimbra")]
    public sealed class WillRenderObjectListener : MonoBehaviour
    {
        public delegate void EventHandler(WillRenderObjectListener sender);

        /// <summary>
        /// Invoked inside <see cref="OnWillRenderObject"/>.
        /// </summary>
        public event EventHandler OnTrigger
        {
            add
            {
                _eventHandler += value;

                if (this.IsValid())
                {
                    enabled = _eventHandler != null;
                }
            }

            remove
            {
                _eventHandler -= value;

                if (this.IsValid())
                {
                    enabled = _eventHandler != null;
                }
            }
        }

        private EventHandler _eventHandler;

        private Renderer _renderer;

        /// <summary>
        /// Gets the renderer this component depends on.
        /// </summary>
        public Renderer Renderer => _renderer != null ? _renderer : _renderer = GetComponent<Renderer>();

        private void Awake()
        {
            enabled = _eventHandler != null;
        }

        private void OnWillRenderObject()
        {
            _eventHandler?.Invoke(this);
        }
    }
}
