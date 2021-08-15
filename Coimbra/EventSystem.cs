using System;

namespace Coimbra
{
    /// <summary>
    /// Simple event system that allows encapsulation of the <see cref="EventSystem{T}.Clear(object)"/> and <see cref="EventSystem{T}.Invoke(object, T, object)"/> methods.
    /// </summary>
    /// <typeparam name="T">The event type.</typeparam>
    public static class EventSystem<T>
        where T : IEvent
    {
        /// <summary>
        /// Use this subscribe and unsubscribe to the event.
        /// </summary>
        public static event EventHandler<T> OnInvoked
        {
            add
            {
                lock (Lock)
                {
                    _value += value;
                }
            }
            remove
            {
                lock (Lock)
                {
                    _value -= value;
                }
            }
        }

        private static readonly object Lock = new object();
        private static object _key;
        private static EventHandler<T> _value;

        /// <summary>
        /// Clear all subscribers.
        /// </summary>
        /// <param name="key">The key previously set with <see cref="SetKey(object)"/>, if any.</param>
        /// <exception cref="ArgumentOutOfRangeException">If the key doesn't match the one previously set.</exception>
        public static void Clear(object key = null)
        {
            if (key != _key)
            {
                throw new ArgumentOutOfRangeException(nameof(key));
            }

            lock (Lock)
            {
                _value = null;
            }
        }

        public static void Invoke(object sender, T @event, object key = null)
        {
            if (key != _key)
            {
                throw new ArgumentOutOfRangeException(nameof(key));
            }

            lock (Lock)
            {
                _value?.Invoke(sender, @event);
            }
        }

        public static void SetKey(object key)
        {
            if (_key != null)
            {
                throw new ArgumentOutOfRangeException(nameof(key));
            }

            _key = key;
        }

        public static void UnsetKey(object key)
        {
            if (_key != key)
            {
                throw new ArgumentOutOfRangeException(nameof(key));
            }

            _key = null;
        }
    }
}
