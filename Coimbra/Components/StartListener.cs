using UnityEngine;

namespace Coimbra
{
    /// <summary>
    /// Listen to <see cref="Start"/> callback.
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu(CoimbraUtility.GeneralMenuPath + "Start Listener")]
    public sealed class StartListener : MonoBehaviour
    {
        public delegate void EventHandler();

        /// <summary>
        /// Invoked inside <see cref="Start"/>. If <see cref="WasTriggered"/> is true, it will invoke the listener immediately.
        /// </summary>
        public event EventHandler OnTrigger
        {
            add
            {
                if (WasTriggered)
                {
                    value?.Invoke();
                }
                else
                {
                    _triggerCallback += value;
                }
            }
            remove
            {
                if (!WasTriggered)
                {
                    _triggerCallback -= value;
                }
            }
        }

        private EventHandler _triggerCallback;

        /// <summary>
        /// True if <see cref="Start"/> was already called. This is set after invoking <see cref="OnTrigger"/> for the first time.
        /// </summary>
        public bool WasTriggered { get; private set; }

        private void Start()
        {
            _triggerCallback?.Invoke();

            _triggerCallback = null;
            WasTriggered = true;
        }
    }
}
