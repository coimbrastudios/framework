using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace Coimbra.Listeners
{
    /// <summary>
    /// Listen to <see cref="Rigidbody2D"/>'s overlaps.
    /// </summary>
    /// <seealso cref="Collider2DParticleCollisionListener"/>
    /// <seealso cref="ColliderOverlap2DListener"/>
    /// <seealso cref="CollisionEnter2DListener"/>
    /// <seealso cref="CollisionExit2DListener"/>
    /// <seealso cref="CollisionStay2DListener"/>
    /// <seealso cref="JointBreak2DListener"/>
    /// <seealso cref="TriggerEnter2DListener"/>
    /// <seealso cref="TriggerExit2DListener"/>
    /// <seealso cref="TriggerStay2DListener"/>
    [RequireComponent(typeof(Rigidbody2D))]
    [AddComponentMenu(CoimbraListenersUtility.Physics2DMenuPath + "Overlap 2D Listener (Rigidbody 2D)")]
    [HelpURL("https://docs.unity3d.com/ScriptReference/Rigidbody2D.OverlapCollider.html")]
    [MovedFrom(true, "Coimbra", "Coimbra")]
    public sealed class RigidbodyOverlap2DListener : Overlap2DListenerBase<Rigidbody2D>
    {
        /// <inheritdoc/>
        protected override int Overlap(ref ContactFilter2D contactFilter, List<Collider2D> results)
        {
            return Component.OverlapCollider(contactFilter, results);
        }
    }
}
