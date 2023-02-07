using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace Coimbra.Listeners
{
    /// <summary>
    /// Listen to <see cref="Collider2D"/>'s overlaps with a more refined control.
    /// </summary>
    /// <seealso cref="Collider2DParticleCollisionListener"/>
    /// <seealso cref="CollisionEnter2DListener"/>
    /// <seealso cref="CollisionExit2DListener"/>
    /// <seealso cref="CollisionStay2DListener"/>
    /// <seealso cref="JointBreak2DListener"/>
    /// <seealso cref="RigidbodyOverlap2DListener"/>
    /// <seealso cref="TriggerEnter2DListener"/>
    /// <seealso cref="TriggerExit2DListener"/>
    /// <seealso cref="TriggerStay2DListener"/>
    [RequireComponent(typeof(Collider2D))]
    [AddComponentMenu(CoimbraListenersUtility.Physics2DMenuPath + "Overlap 2D Listener (Collider 2D)")]
    [HelpURL("https://docs.unity3d.com/ScriptReference/Collider2D.OverlapCollider.html")]
    [MovedFrom(true, "Coimbra", "Coimbra")]
    public sealed class ColliderOverlap2DListener : Overlap2DListenerBase<Collider2D>
    {
        /// <inheritdoc/>
        protected override int Overlap(ref ContactFilter2D contactFilter, List<Collider2D> results)
        {
            return Component.OverlapCollider(contactFilter, results);
        }
    }
}
