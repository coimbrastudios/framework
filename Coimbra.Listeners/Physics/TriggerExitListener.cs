using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace Coimbra.Listeners
{
    /// <summary>
    /// Listen to <see cref="Collider"/>'s <see cref="OnTriggerExit"/> callback.
    /// </summary>
    /// <seealso cref="ColliderParticleCollisionListener"/>
    /// <seealso cref="CollisionEnterListener"/>
    /// <seealso cref="CollisionExitListener"/>
    /// <seealso cref="CollisionStayListener"/>
    /// <seealso cref="ControllerColliderHitListener"/>
    /// <seealso cref="JointBreakListener"/>
    /// <seealso cref="TriggerEnterListener"/>
    /// <seealso cref="TriggerStayListener"/>
    [DisallowMultipleComponent]
    [AddComponentMenu(CoimbraListenersUtility.PhysicsMenuPath + "Trigger Exit Listener")]
    [HelpURL("https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnTriggerExit.html")]
    [MovedFrom(true, "Coimbra", "Coimbra")]
    public sealed class TriggerExitListener : TriggerListenerBase
    {
        private void OnTriggerExit(Collider other)
        {
            Trigger(other);
        }
    }
}
