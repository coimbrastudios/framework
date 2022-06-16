using System.Collections.Generic;
using UnityEngine;

namespace Coimbra
{
    /// <summary>
    /// Listen to <see cref="Collider2D"/>'s overlaps.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    [AddComponentMenu(CoimbraUtility.GeneralMenuPath + "Overlap 2D Listener (Collider 2D)")]
    [HelpURL("https://docs.unity3d.com/ScriptReference/Collider2D.OverlapCollider.html")]
    public sealed class ColliderOverlap2DListener : Overlap2DListenerBase<Collider2D>
    {
        /// <inheritdoc/>
        protected override int Overlap(ref ContactFilter2D contactFilter, List<Collider2D> results)
        {
            return Component.OverlapCollider(contactFilter, results);
        }
    }
}
