using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace Coimbra.Listeners
{
    /// <summary>
    /// Listen to <see cref="ParticleSystem"/>'s <see cref="OnParticleTrigger"/> callback.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(ParticleSystem))]
    [AddComponentMenu(CoimbraListenersUtility.ParticleSystemMenuPath + "Particle Trigger Listener")]
    [HelpURL("https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnParticleTrigger.html")]
    [MovedFrom(true, "Coimbra", "Coimbra")]
    public sealed class ParticleTriggerListener : MonoBehaviour
    {
        public delegate void EventHandler(ParticleTriggerListener sender);

        /// <summary>
        /// Invoked inside <see cref="OnParticleTrigger"/>.
        /// </summary>
        public event EventHandler OnTrigger;

        private ParticleSystem _particleSystem;

        /// <summary>
        /// Gets the particle system this component depends on.
        /// </summary>
        public ParticleSystem ParticleSystem => _particleSystem != null ? _particleSystem : _particleSystem = GetComponent<ParticleSystem>();

        private void OnParticleTrigger()
        {
            OnTrigger?.Invoke(this);
        }
    }
}
