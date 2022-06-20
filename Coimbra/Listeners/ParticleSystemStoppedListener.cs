﻿using UnityEngine;

namespace Coimbra
{
    /// <summary>
    /// Listen to <see cref="ParticleSystem"/>'s <see cref="OnParticleSystemStopped"/> callback.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(ParticleSystem))]
    [AddComponentMenu(CoimbraUtility.GeneralMenuPath + "Particle System Stopped Listener")]
    [HelpURL("https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnParticleSystemStopped.html")]
    public sealed class ParticleSystemStoppedListener : MonoBehaviour
    {
        public delegate void EventHandler(ParticleSystemStoppedListener sender);

        /// <summary>
        /// Invoked inside <see cref="OnParticleSystemStopped"/>.
        /// </summary>
        public event EventHandler OnTrigger;

        private ParticleSystem _particleSystem;

        /// <summary>
        /// The particle system this component depends on.
        /// </summary>
        public ParticleSystem ParticleSystem => _particleSystem != null ? _particleSystem : _particleSystem = GetComponent<ParticleSystem>();

        private void OnParticleSystemStopped()
        {
            OnTrigger?.Invoke(this);
        }
    }
}