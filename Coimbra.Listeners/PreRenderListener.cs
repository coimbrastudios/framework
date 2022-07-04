using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace Coimbra.Listeners
{
    /// <summary>
    /// Listen to <see cref="Camera"/>'s <see cref="OnPreRender"/> callback.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Camera))]
    [AddComponentMenu(CoimbraListenersUtility.RenderingMenuPath + "Pre Render Listener")]
    [HelpURL("https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnPreRender.html")]
    [MovedFrom(true, "Coimbra", "Coimbra")]
    public sealed class PreRenderListener : MonoBehaviour
    {
        public delegate void EventHandler(PreRenderListener sender);

        /// <summary>
        /// Invoked inside <see cref="OnPreRender"/>.
        /// </summary>
        public event EventHandler OnTrigger;

        private Camera _camera;

        /// <summary>
        /// Gets the animator this component depends on.
        /// </summary>
        public Camera Camera => _camera != null ? _camera : _camera = GetComponent<Camera>();

        private void OnPreRender()
        {
            OnTrigger?.Invoke(this);
        }
    }
}
