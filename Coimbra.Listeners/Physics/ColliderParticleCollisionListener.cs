using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace Coimbra.Listeners
{
    /// <summary>
    /// Listen to <see cref="Collider"/>'s <see cref="ParticleCollisionListenerBase{T}.OnParticleCollision"/> callback.
    /// </summary>
    /// <seealso cref="ParticleSystemParticleCollisionListener"/>
    /// <seealso cref="CollisionEnterListener"/>
    /// <seealso cref="CollisionExitListener"/>
    /// <seealso cref="CollisionStayListener"/>
    /// <seealso cref="ControllerColliderHitListener"/>
    /// <seealso cref="JointBreakListener"/>
    /// <seealso cref="TriggerEnterListener"/>
    /// <seealso cref="TriggerExitListener"/>
    /// <seealso cref="TriggerStayListener"/>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Collider))]
    [AddComponentMenu(CoimbraListenersUtility.PhysicsMenuPath + "Particle Collision Listener (Collider)")]
    [HelpURL("https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnParticleCollision.html")]
    [MovedFrom(true, "Coimbra", "Coimbra")]
    public sealed class ColliderParticleCollisionListener : ParticleCollisionListenerBase<Collider> { }
}
