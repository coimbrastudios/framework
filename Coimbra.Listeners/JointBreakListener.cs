using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace Coimbra.Listeners
{
    /// <summary>
    /// Listen to <see cref="UnityEngine.Joint"/>'s <see cref="OnJointBreak"/> callback.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Joint))]
    [AddComponentMenu(CoimbraListenersUtility.PhysicsMenuPath + "Joint Break Listener")]
    [HelpURL("https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnJointBreak.html")]
    [MovedFrom(true, "Coimbra", "Coimbra")]
    public sealed class JointBreakListener : MonoBehaviour
    {
        public delegate void EventHandler(JointBreakListener sender, float breakForce);

        /// <summary>
        /// Invoked inside <see cref="OnJointBreak"/>.
        /// </summary>
        public event EventHandler OnTrigger;

        private Joint _joint;

        /// <summary>
        /// Gets the joint this component depends on.
        /// </summary>
        public Joint Joint => _joint != null ? _joint : _joint = GetComponent<Joint>();

        private void OnJointBreak(float breakForce)
        {
            OnTrigger?.Invoke(this, breakForce);
        }
    }
}
