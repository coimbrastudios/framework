using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace Coimbra.Listeners
{
    /// <summary>
    /// Listen to <see cref="Collider"/>'s <see cref="ParticleCollisionListenerBase{T}.OnParticleCollision"/> callback.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Collider))]
    [AddComponentMenu(CoimbraListenersUtility.PhysicsMenuPath + "Particle Collision Listener (Collider)")]
    [HelpURL("https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnParticleCollision.html")]
    [MovedFrom(true, "Coimbra", "Coimbra")]
    public sealed class ColliderParticleCollisionListener : ParticleCollisionListenerBase<Collider> { }
}
