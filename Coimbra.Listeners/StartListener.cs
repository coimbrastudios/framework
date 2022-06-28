using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace Coimbra.Listeners
{
    /// <summary>
    /// Listen to <see cref="Start"/> callback.
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu(CoimbraUtility.GeneralMenuPath + "Start Listener")]
    [HelpURL("https://docs.unity3d.com/ScriptReference/MonoBehaviour.Start.html")]
    [MovedFrom(true, "Coimbra", "Coimbra")]
    public sealed class StartListener : MonoBehaviour
    {
        public delegate void EventHandler(StartListener sender);

        /// <summary>
        /// Invoked inside <see cref="Start"/>. If <see cref="WasTriggered"/> is true, it will invoke the listener immediately.
        /// </summary>
        public event EventHandler OnTrigger
        {
            add
            {
                if (WasTriggered)
                {
                    value?.Invoke(this);
                }
                else
                {
                    _eventHandler += value;
                }
            }

            remove
            {
                if (!WasTriggered)
                {
                    _eventHandler -= value;
                }
            }
        }

        private EventHandler _eventHandler;

        /// <summary>
        /// Gets a value indicating whether <see cref="Start"/> was already called. This is set after invoking <see cref="OnTrigger"/> for the first time.
        /// </summary>
        public bool WasTriggered { get; private set; }

        private void Start()
        {
            _eventHandler?.Invoke(this);

            _eventHandler = null;
            WasTriggered = true;
        }
    }
}
