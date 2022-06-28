using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace Coimbra.Listeners
{
    /// <summary>
    /// Base class to listen to <see cref="Collider2D"/>'s trigger callbacks.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    [MovedFrom(true, "Coimbra", "Coimbra")]
    public abstract class Trigger2DListenerBase : MonoBehaviour
    {
        public delegate void EventHandler(Trigger2DListenerBase sender, Collider2D other);

        /// <summary>
        /// Invoked inside the trigger callback.
        /// </summary>
        public event EventHandler OnTrigger;

        private Collider2D _collider;

        /// <summary>
        /// Gets the collider this component depends on.
        /// </summary>
        public Collider2D Collider => _collider != null ? _collider : _collider = GetComponent<Collider2D>();

        /// <summary>
        /// Invokes the <see cref="OnTrigger"/> event.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void Trigger(Collider2D other)
        {
            OnTrigger?.Invoke(this, other);
        }
    }
}
