using UnityEngine;

namespace Coimbra
{
    /// <summary>
    /// Listen to <see cref="Camera"/>'s <see cref="OnRenderImage"/> callback.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Camera))]
    [AddComponentMenu(CoimbraUtility.GeneralMenuPath + "Render Image Listener")]
    [HelpURL("https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnRenderImage.html")]
    public sealed class RenderImageListener : MonoBehaviour
    {
        public delegate void EventHandler(RenderImageListener sender, RenderTexture source, RenderTexture destination);

        /// <summary>
        /// Invoked inside <see cref="OnRenderImage"/>.
        /// </summary>
        public event EventHandler OnTrigger;

        private Camera _camera;

        /// <summary>
        /// The animator this component depends on.
        /// </summary>
        public Camera Camera => _camera != null ? _camera : _camera = GetComponent<Camera>();

        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            OnTrigger?.Invoke(this, source, destination);
        }
    }
}
