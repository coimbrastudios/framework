using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace Coimbra.Listeners
{
    /// <summary>
    /// Listen to <see cref="OnParticleCollision"/> callback.
    /// </summary>
    [MovedFrom(true, "Coimbra", "Coimbra")]
    public abstract class ParticleCollisionListenerBase<T> : MonoBehaviour
        where T : Component
    {
        public delegate void EventHandler(ParticleCollisionListenerBase<T> sender, GameObject other);

        /// <summary>
        /// Invoked inside <see cref="OnParticleCollision"/>.
        /// </summary>
        public event EventHandler OnTrigger;

        private T _component;

        /// <summary>
        /// Gets the component this component depends on.
        /// </summary>
        public T Component => _component != null ? _component : _component = GetComponent<T>();

        /// <summary>
        /// Unity callback.
        /// </summary>
        protected void OnParticleCollision(GameObject other)
        {
            OnTrigger?.Invoke(this, other);
        }
    }
}
