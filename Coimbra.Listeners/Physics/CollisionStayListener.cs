using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace Coimbra.Listeners
{
    /// <summary>
    /// Listen to <see cref="Collider"/>'s <see cref="OnCollisionStay"/> callback.
    /// </summary>
    /// <seealso cref="ColliderParticleCollisionListener"/>
    /// <seealso cref="CollisionEnterListener"/>
    /// <seealso cref="CollisionExitListener"/>
    /// <seealso cref="ControllerColliderHitListener"/>
    /// <seealso cref="JointBreakListener"/>
    /// <seealso cref="TriggerEnterListener"/>
    /// <seealso cref="TriggerExitListener"/>
    /// <seealso cref="TriggerStayListener"/>
    [DisallowMultipleComponent]
    [AddComponentMenu(CoimbraListenersUtility.PhysicsMenuPath + "Collision Stay Listener")]
    [HelpURL("https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnCollisionStay.html")]
    [MovedFrom(true, "Coimbra", "Coimbra")]
    public sealed class CollisionStayListener : CollisionListenerBase
    {
        private void OnCollisionStay(Collision collision)
        {
            Trigger(collision);
        }
    }
}
