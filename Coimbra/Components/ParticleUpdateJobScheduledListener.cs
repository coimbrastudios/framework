using UnityEngine;

namespace Coimbra
{
    /// <summary>
    /// Listen to <see cref="ParticleSystem"/>'s <see cref="OnParticleUpdateJobScheduled"/> callback.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(ParticleSystem))]
    [AddComponentMenu(CoimbraUtility.GeneralMenuPath + "Particle Update Job Scheduled Listener")]
    [HelpURL("https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnParticleUpdateJobScheduled.html")]
    public sealed class ParticleUpdateJobScheduledListener : MonoBehaviour
    {
        public delegate void EventHandler(ParticleUpdateJobScheduledListener sender);

        /// <summary>
        /// Invoked inside <see cref="OnParticleUpdateJobScheduled"/>.
        /// </summary>
        public event EventHandler OnTrigger;

        private ParticleSystem _particleSystem;

        /// <summary>
        /// The particle system this component depends on.
        /// </summary>
        public ParticleSystem ParticleSystem => _particleSystem != null ? _particleSystem : _particleSystem = GetComponent<ParticleSystem>();

        private void OnParticleUpdateJobScheduled()
        {
            OnTrigger?.Invoke(this);
        }
    }
}
