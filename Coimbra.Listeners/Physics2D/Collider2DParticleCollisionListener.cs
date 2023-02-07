using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace Coimbra.Listeners
{
    /// <summary>
    /// Listen to <see cref="Collider2D"/>'s <see cref="ParticleCollisionListenerBase{T}.OnParticleCollision"/> callback.
    /// </summary>
    /// <seealso cref="ParticleSystemParticleCollisionListener"/>
    /// <seealso cref="ColliderOverlap2DListener"/>
    /// <seealso cref="CollisionEnter2DListener"/>
    /// <seealso cref="CollisionExit2DListener"/>
    /// <seealso cref="CollisionStay2DListener"/>
    /// <seealso cref="JointBreak2DListener"/>
    /// <seealso cref="RigidbodyOverlap2DListener"/>
    /// <seealso cref="TriggerEnter2DListener"/>
    /// <seealso cref="TriggerExit2DListener"/>
    /// <seealso cref="TriggerStay2DListener"/>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Collider2D))]
    [AddComponentMenu(CoimbraListenersUtility.Physics2DMenuPath + "Particle Collision Listener (Collider 2D)")]
    [HelpURL("https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnParticleCollision.html")]
    [MovedFrom(true, "Coimbra", "Coimbra")]
    public sealed class Collider2DParticleCollisionListener : ParticleCollisionListenerBase<Collider2D> { }
}
