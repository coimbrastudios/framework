using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace Coimbra.Listeners
{
    /// <summary>
    /// Listen to <see cref="Camera"/>'s <see cref="OnRenderImage"/> callback.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Camera))]
    [AddComponentMenu(CoimbraListenersUtility.RenderingMenuPath + "Render Image Listener")]
    [HelpURL("https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnRenderImage.html")]
    [MovedFrom(true, "Coimbra", "Coimbra")]
    public sealed class RenderImageListener : MonoBehaviour
    {
        public delegate void EventHandler(RenderImageListener sender, RenderTexture source, RenderTexture destination);

        /// <summary>
        /// Invoked inside <see cref="OnRenderImage"/>.
        /// </summary>
        public event EventHandler OnTrigger;

        private Camera _camera;

        /// <summary>
        /// Gets the animator this component depends on.
        /// </summary>
        public Camera Camera => _camera != null ? _camera : _camera = GetComponent<Camera>();

        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            OnTrigger?.Invoke(this, source, destination);
        }
    }
}
