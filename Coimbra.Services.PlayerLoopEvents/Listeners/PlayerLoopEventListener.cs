using Coimbra.Services.Events;
using UnityEngine;

namespace Coimbra.Services.PlayerLoopEvents
{
    /// <summary>
    /// Listen to any <see cref="IPlayerLoopEvent"/> invocation.
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu(CoimbraUtility.GeneralMenuPath + "Player Loop Event Listener")]
    [HelpURL("https://github.com/Cysharp/UniTask#playerloop")]
    public sealed class PlayerLoopEventListener : PlayerLoopListenerBase
    {
        /// <inheritdoc />
        public override event EventHandler OnTrigger
        {
            add
            {
                base.OnTrigger += value;

                if (!HasListener || _eventHandle.HasValue)
                {
                    return;
                }

                _eventHandle = ServiceLocator.GetChecked<IPlayerLoopService>().AddListener(_playerLoopEventType, HandlePlayerLoopEvent);
            }
            remove
            {
                base.OnTrigger -= value;

                if (HasListener || !_eventHandle.HasValue)
                {
                    return;
                }

                ServiceLocator.GetChecked<IEventService>().RemoveListener(_eventHandle.Value);

                _eventHandle = null;
            }
        }

        [SerializeField]
        [Tooltip("The player loop event type to use.")]
        private SerializableType<IPlayerLoopEvent> _playerLoopEventType;

        private EventHandle? _eventHandle;

        /// <summary>
        /// The <see cref="IPlayerLoopEvent"/> type to use.
        /// </summary>
        public SerializableType<IPlayerLoopEvent> PlayerLoopEventType
        {
            get => _playerLoopEventType;
            set => _playerLoopEventType = value;
        }

        private void HandlePlayerLoopEvent(ref EventContext context, float deltaTime)
        {
            Trigger(deltaTime);
        }
    }
}
