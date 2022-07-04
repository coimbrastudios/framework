using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace Coimbra.Listeners
{
    /// <summary>
    /// Listen to <see cref="ParticleSystem"/>'s <see cref="ParticleCollisionListenerBase{T}.OnParticleCollision"/> callback.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(ParticleSystem))]
    [AddComponentMenu(CoimbraListenersUtility.ParticleSystemMenuPath + "Particle Collision Listener (Particle System)")]
    [HelpURL("https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnParticleCollision.html")]
    [MovedFrom(true, "Coimbra", "Coimbra")]
    public sealed class ParticleSystemParticleCollisionListener : ParticleCollisionListenerBase<ParticleSystem> { }
}
