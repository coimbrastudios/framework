using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace Coimbra.Listeners
{
    /// <summary>
    /// Listen to <see cref="Camera"/>'s <see cref="OnPostRender"/> callback.
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu(CoimbraListenersUtility.RenderingMenuPath + "Post Render Listener")]
    [HelpURL("https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnPostRender.html")]
    [MovedFrom(true, "Coimbra", "Coimbra")]
    public sealed class PostRenderListener : MonoBehaviour
    {
        public delegate void EventHandler(PostRenderListener sender);

        /// <summary>
        /// Invoked inside <see cref="OnPostRender"/>.
        /// </summary>
        public event EventHandler OnTrigger;

        private Camera _camera;

        /// <summary>
        /// Gets the animator this component depends on.
        /// </summary>
        public Camera Camera => _camera != null ? _camera : _camera = GetComponent<Camera>();

        private void OnPostRender()
        {
            OnTrigger?.Invoke(this);
        }
    }
}
