using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace Coimbra.Listeners
{
    /// <summary>
    /// Listen to <see cref="Joint2D"/>'s <see cref="OnJointBreak2D"/> callback.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Joint2D))]
    [AddComponentMenu(CoimbraListenersUtility.Physics2DMenuPath + "Joint Break 2D Listener")]
    [HelpURL("https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnJointBreak2D.html")]
    [MovedFrom(true, "Coimbra", "Coimbra")]
    public sealed class JointBreak2DListener : MonoBehaviour
    {
        public delegate void EventHandler(JointBreak2DListener sender, Joint2D brokenJoint);

        /// <summary>
        /// Invoked inside <see cref="OnJointBreak2D"/>.
        /// </summary>
        public event EventHandler OnTrigger;

        private Joint2D _joint;

        /// <summary>
        /// Gets the joint this component depends on.
        /// </summary>
        public Joint2D Joint => _joint != null ? _joint : _joint = GetComponent<Joint2D>();

        private void OnJointBreak2D(Joint2D brokenJoint)
        {
            OnTrigger?.Invoke(this, brokenJoint);
        }
    }
}
