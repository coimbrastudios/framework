using Coimbra.Listeners;
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

        [SerializeField]
        private EventHandle _eventHandle;

        /// <summary>
        /// Gets or sets the <see cref="IPlayerLoopEvent"/> type to use.
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
            if (!_eventHandle.IsValid)
            {
                return;
            }

            _eventHandle.Service.RemoveListener(in _eventHandle);

            _eventHandle = default;
        }

        private void TryListenPlayerLoopService()
        {
            if (this.IsValid() && !_eventHandle.IsValid)
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
