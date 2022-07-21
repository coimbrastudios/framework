using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace Coimbra.Listeners
{
    /// <summary>
    /// Listen to <see cref="Collider2D"/>'s overlaps.
    /// </summary>
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
