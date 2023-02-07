using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace Coimbra.Listeners
{
    /// <summary>
    /// Listen to <see cref="Collider"/>'s <see cref="OnTriggerEnter"/> callback.
    /// </summary>
    /// <seealso cref="ColliderParticleCollisionListener"/>
    /// <seealso cref="CollisionEnterListener"/>
    /// <seealso cref="CollisionExitListener"/>
    /// <seealso cref="CollisionStayListener"/>
    /// <seealso cref="ControllerColliderHitListener"/>
    /// <seealso cref="JointBreakListener"/>
    /// <seealso cref="TriggerExitListener"/>
    /// <seealso cref="TriggerStayListener"/>
    [DisallowMultipleComponent]
    [AddComponentMenu(CoimbraListenersUtility.PhysicsMenuPath + "Trigger Enter Listener")]
    [HelpURL("https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnTriggerEnter.html")]
    [MovedFrom(true, "Coimbra", "Coimbra")]
    public sealed class TriggerEnterListener : TriggerListenerBase
    {
        private void OnTriggerEnter(Collider other)
        {
            Trigger(other);
        }
    }
}
