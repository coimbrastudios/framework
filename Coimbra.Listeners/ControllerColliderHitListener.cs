using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace Coimbra.Listeners
{
    /// <summary>
    /// Listen to <see cref="UnityEngine.CharacterController"/>'s <see cref="OnControllerColliderHit"/> callback.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CharacterController))]
    [AddComponentMenu(CoimbraListenersUtility.PhysicsMenuPath + "Controller Collider Hit Listener")]
    [HelpURL("https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnControllerColliderHit.html")]
    [MovedFrom(true, "Coimbra", "Coimbra")]
    public sealed class ControllerColliderHitListener : MonoBehaviour
    {
        public delegate void EventHandler(ControllerColliderHitListener sender, ControllerColliderHit hit);

        /// <summary>
        /// Invoked inside <see cref="OnControllerColliderHit"/>.
        /// </summary>
        public event EventHandler OnTrigger;

        private CharacterController _characterController;

        /// <summary>
        /// Gets the character controller this component depends on.
        /// </summary>
        public CharacterController CharacterController => _characterController != null ? _characterController : _characterController = GetComponent<CharacterController>();

        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            OnTrigger?.Invoke(this, hit);
        }
    }
}
