using UnityEngine;

namespace Coimbra
{
    /// <summary>
    /// Listen to <see cref="OnParticleCollision"/> callback.
    /// </summary>
    public abstract class ParticleCollisionListenerBase<T> : MonoBehaviour
    {
        public delegate void EventHandler(ParticleCollisionListenerBase<T> sender, GameObject other);

        /// <summary>
        /// Invoked inside <see cref="OnParticleCollision"/>.
        /// </summary>
        public event EventHandler OnTrigger;

        private T _component;

        /// <summary>
        /// The component this component depends on.
        /// </summary>
        public T Component => _component != null ? _component : _component = GetComponent<T>();

        private void OnParticleCollision(GameObject other)
        {
            OnTrigger?.Invoke(this, other);
        }
    }
}
