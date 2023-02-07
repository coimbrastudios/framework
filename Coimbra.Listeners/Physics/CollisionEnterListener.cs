using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace Coimbra.Listeners
{
    /// <summary>
    /// Listen to <see cref="Collider"/>'s <see cref="OnCollisionEnter"/> callback.
    /// </summary>
    /// <seealso cref="ColliderParticleCollisionListener"/>
    /// <seealso cref="CollisionExitListener"/>
    /// <seealso cref="CollisionStayListener"/>
    /// <seealso cref="ControllerColliderHitListener"/>
    /// <seealso cref="JointBreakListener"/>
    /// <seealso cref="TriggerEnterListener"/>
    /// <seealso cref="TriggerExitListener"/>
    /// <seealso cref="TriggerStayListener"/>
    [DisallowMultipleComponent]
    [AddComponentMenu(CoimbraListenersUtility.PhysicsMenuPath + "Collision Enter Listener")]
    [HelpURL("https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnCollisionEnter.html")]
    [MovedFrom(true, "Coimbra", "Coimbra")]
    public sealed class CollisionEnterListener : CollisionListenerBase
    {
        private void OnCollisionEnter(Collision collision)
        {
            Trigger(collision);
        }
    }
}
