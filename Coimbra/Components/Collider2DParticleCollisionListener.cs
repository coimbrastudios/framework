﻿using UnityEngine;

namespace Coimbra
{
    /// <summary>
    /// Listen to <see cref="Collider2D"/>'s <see cref="ParticleCollisionListenerBase{T}.OnParticleCollision"/> callback.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Collider2D))]
    [AddComponentMenu(CoimbraUtility.GeneralMenuPath + "Particle Collision Listener (Collider 2D)")]
    [HelpURL("https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnParticleCollision.html")]
    public sealed class Collider2DParticleCollisionListener : ParticleCollisionListenerBase<Collider2D> { }
}
