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

                if (HasListener)
                {
                    TryListenPlayerLoopService();
                }
            }
            remove
            {
                base.OnTrigger -= value;

                if (!HasListener)
                {
                    ForgetPlayerLoopEvent();
                }
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
            set
            {
                _playerLoopEventType = value;

                if (!HasListener)
                {
                    return;
                }

                ForgetPlayerLoopEvent();
                TryListenPlayerLoopService();
            }
        }

        private void Reset()
        {
            ForgetPlayerLoopEvent();
        }

        private void OnValidate()
        {
            if (Application.isPlaying)
            {
                PlayerLoopEventType = _playerLoopEventType;
            }
        }

        private void OnDestroy()
        {
            ForgetPlayerLoopEvent();
        }

        private void ForgetPlayerLoopEvent()
        {
            if (!_eventHandle.HasValue)
            {
                return;
            }

            ServiceLocator.GetChecked<IEventService>().RemoveListener(_eventHandle.Value);

            _eventHandle = null;
        }

        private void TryListenPlayerLoopService()
        {
            if (this.IsValid() && !_eventHandle.HasValue)
            {
                _eventHandle = ServiceLocator.GetChecked<IPlayerLoopService>().AddListener(_playerLoopEventType, HandlePlayerLoopEvent);
            }
        }

        private void HandlePlayerLoopEvent(ref EventContext context, float deltaTime)
        {
            Trigger(deltaTime);
        }
    }
}
