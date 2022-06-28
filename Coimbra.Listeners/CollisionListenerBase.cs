using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace Coimbra.Listeners
{
    /// <summary>
    /// Base class to listen to <see cref="Collider"/>'s <see cref="Collision"/> callbacks.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    [MovedFrom(true, "Coimbra", "Coimbra")]
    public abstract class CollisionListenerBase : MonoBehaviour
    {
        public delegate void EventHandler(CollisionListenerBase sender, Collision collision);

        /// <summary>
        /// Invoked inside the <see cref="Collision"/> callback.
        /// </summary>
        public event EventHandler OnTrigger;

        private Collider _collider;

        /// <summary>
        /// Gets the collider this component depends on.
        /// </summary>
        public Collider Collider => _collider != null ? _collider : _collider = GetComponent<Collider>();

        /// <summary>
        /// Invokes the <see cref="OnTrigger"/> event.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void Trigger(Collision collision)
        {
            OnTrigger?.Invoke(this, collision);
        }
    }
}
