using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace Coimbra.Listeners
{
    /// <summary>
    /// Base class to listen to <see cref="UnityEngine.Collider"/>'s trigger callbacks.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    [MovedFrom(true, "Coimbra", "Coimbra")]
    public abstract class TriggerListenerBase : MonoBehaviour
    {
        public delegate void EventHandler(TriggerListenerBase sender, Collider other);

        /// <summary>
        /// Invoked inside the trigger callback.
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
        protected void Trigger(Collider other)
        {
            OnTrigger?.Invoke(this, other);
        }
    }
}
