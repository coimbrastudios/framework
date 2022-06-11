using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.LowLevel;

namespace Coimbra
{
    /// <summary>
    /// Base class for listening to <see cref="PlayerLoop"/> callbacks.
    /// </summary>
    /// <seealso cref="FixedUpdateListener"/>
    /// <seealso cref="LateUpdateListener"/>
    /// <seealso cref="UpdateListener"/>
    public abstract class PlayerLoopListenerBase : MonoBehaviour
    {
        public delegate void EventHandler(float deltaTime);

        /// <summary>
        /// Invoked inside <see cref="Trigger"/>.
        /// </summary>
        public event EventHandler OnTrigger
        {
            add
            {
                _eventHandler += value;
                ValidateEnabledState();
            }
            remove
            {
                _eventHandler -= value;
                ValidateEnabledState();
            }
        }

        private EventHandler _eventHandler;

        /// <summary>
        /// OnValidate callback.
        /// </summary>
        protected virtual void OnValidate()
        {
            ValidateEnabledState();
        }

        /// <summary>
        /// Awake callback.
        /// </summary>
        protected virtual void Awake()
        {
            ValidateEnabledState();
        }

        /// <summary>
        /// Invokes the <see cref="OnTrigger"/> event.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void Trigger()
        {
            _eventHandler?.Invoke(Time.deltaTime);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ValidateEnabledState()
        {
            enabled = _eventHandler != null;
        }
    }
}
