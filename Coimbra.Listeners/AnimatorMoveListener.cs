using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace Coimbra.Listeners
{
    /// <summary>
    /// Listen to <see cref="Animator"/>'s <see cref="OnAnimatorMove"/> callback.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Animator))]
    [AddComponentMenu(CoimbraListenersUtility.RenderingMenuPath + "Animator Move Listener")]
    [HelpURL("https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnAnimatorMove.html")]
    [MovedFrom(true, "Coimbra", "Coimbra")]
    public sealed class AnimatorMoveListener : MonoBehaviour
    {
        public delegate void EventHandler(AnimatorMoveListener sender);

        /// <summary>
        /// Invoked inside <see cref="OnAnimatorMove"/>.
        /// </summary>
        public event EventHandler OnTrigger;

        private Animator _animator;

        /// <summary>
        /// Gets the animator this component depends on.
        /// </summary>
        public Animator Animator => _animator != null ? _animator : _animator = GetComponent<Animator>();

        private void OnAnimatorMove()
        {
            OnTrigger?.Invoke(this);
        }
    }
}
