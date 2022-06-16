﻿using UnityEngine;

namespace Coimbra
{
    /// <summary>
    /// Listen to <see cref="ParticleSystem"/>'s <see cref="ParticleCollisionListenerBase{T}.OnParticleCollision"/> callback.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(ParticleSystem))]
    [AddComponentMenu(CoimbraUtility.GeneralMenuPath + "Particle Collision Listener (Particle System)")]
    [HelpURL("https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnParticleCollision.html")]
    public sealed class ParticleSystemParticleCollisionListener : ParticleCollisionListenerBase<ParticleSystem> { }
}
