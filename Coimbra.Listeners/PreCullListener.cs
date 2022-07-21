using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace Coimbra.Listeners
{
    /// <summary>
    /// Listen to <see cref="Camera"/>'s <see cref="OnPreCull"/> callback.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Camera))]
    [AddComponentMenu(CoimbraListenersUtility.RenderingMenuPath + "Pre Cull Listener")]
    [HelpURL("https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnPreCull.html")]
    [MovedFrom(true, "Coimbra", "Coimbra")]
    public sealed class PreCullListener : MonoBehaviour
    {
        public delegate void EventHandler(PreCullListener sender);

        /// <summary>
        /// Invoked inside <see cref="OnPreCull"/>.
        /// </summary>
        public event EventHandler OnTrigger;

        private Camera _camera;

        /// <summary>
        /// Gets the animator this component depends on.
        /// </summary>
        public Camera Camera => _camera != null ? _camera : _camera = GetComponent<Camera>();

        private void OnPreCull()
        {
            OnTrigger?.Invoke(this);
        }
    }
}
