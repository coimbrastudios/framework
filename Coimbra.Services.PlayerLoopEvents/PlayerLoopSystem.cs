#nullable enable

using Coimbra.Listeners;
using Coimbra.Services.Events;
using Cysharp.Threading.Tasks;
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Coimbra.Services.PlayerLoopEvents
{
    /// <summary>
    /// Default implementation for <see cref="IPlayerLoopService"/>.
    /// </summary>
    /// <remarks>
    /// The events are invoked using <see cref="IEventService"/> and this implementation makes use of <see cref="UnityEngine.Object.DontDestroyOnLoad"/> to persist between scenes.
    /// </remarks>
    /// <seealso cref="PlayerLoopEventListener"/>
    /// <seealso cref="IPlayerLoopService"/>
    /// <seealso cref="IPlayerLoopEvent"/>
    /// <seealso cref="FixedUpdateEvent"/>
    /// <seealso cref="UpdateEvent"/>
    /// <seealso cref="LateUpdateEvent"/>
    /// <seealso cref="FirstInitializationEvent"/>
    /// <seealso cref="LastInitializationEvent"/>
    /// <seealso cref="FirstEarlyUpdateEvent"/>
    /// <seealso cref="LastEarlyUpdateEvent"/>
    /// <seealso cref="FirstFixedUpdateEvent"/>
    /// <seealso cref="LastFixedUpdateEvent"/>
    /// <seealso cref="FirstPreUpdateEvent"/>
    /// <seealso cref="LastPreUpdateEvent"/>
    /// <seealso cref="FirstUpdateEvent"/>
    /// <seealso cref="LastUpdateEvent"/>
    /// <seealso cref="PreLateUpdateEvent"/>
    /// <seealso cref="FirstPostLateUpdateEvent"/>
    /// <seealso cref="PostLateUpdateEvent"/>
    /// <seealso cref="LastPostLateUpdateEvent"/>
    /// <seealso cref="PreTimeUpdateEvent"/>
    /// <seealso cref="PostTimeUpdateEvent"/>
    [AddComponentMenu("")]
    [PreloadService]
    [RequireComponent(typeof(FixedUpdateListener))]
    [RequireComponent(typeof(LateUpdateListener))]
    [RequireComponent(typeof(UpdateListener))]
    public sealed class PlayerLoopSystem : Actor, IPlayerLoopService
    {
        private const PlayerLoopInjectedTimings FixedUpdateTimings = PlayerLoopInjectedTimings.FirstFixedUpdate | PlayerLoopInjectedTimings.LastFixedUpdate;

        private const PlayerLoopInjectedTimings MainUpdateTimings = PlayerLoopInjectedTimings.All & ~FixedUpdateTimings;

        [SerializeField]
        [Disable]
        private PlayerLoopInjectedTimings _currentInjectedTimings = PlayerLoopInjectedTimings.All;

        private FixedUpdateListener _fixedUpdateListener = null!;

        private LateUpdateListener _lateUpdateListener = null!;

        private UpdateListener _updateListener = null!;

        private PlayerLoopSystem() { }

        /// <inheritdoc/>
        public PlayerLoopInjectedTimings CurrentInjectedTimings
        {
            [DebuggerStepThrough]
            get => _currentInjectedTimings;
            set
            {
                if (IsDestroyed)
                {
                    return;
                }

                bool hadAnyFixedUpdateTiming = HasAnyFixedUpdateTiming();
                bool hadAnyMainUpdateTiming = HasAnyMainUpdateTiming();
                _currentInjectedTimings = value;

                if (!hadAnyFixedUpdateTiming && HasAnyFixedUpdateTiming())
                {
                    InvokeFixedUpdateEventsAsync().Forget();
                }

                if (!hadAnyMainUpdateTiming && HasAnyMainUpdateTiming())
                {
                    InvokeMainUpdateEventsAsync().Forget();
                }
            }
        }

        /// <inheritdoc/>
        public EventHandle AddListener(SerializableType<IPlayerLoopEvent> eventType, PlayerLoopEventHandler eventHandler)
        {
            void handlePlayerLoopEvent<T>(ref EventContext context, in T e)
                where T : struct, IPlayerLoopEvent
            {
                eventHandler(ref context, e.DeltaTime);
            }

            switch (eventType)
            {
                case { } type when type == typeof(FixedUpdateEvent):
                {
                    return ServiceLocator.GetChecked<IEventService>().AddListener<FixedUpdateEvent>(handlePlayerLoopEvent);
                }

                case { } type when type == typeof(LateUpdateEvent):
                {
                    return ServiceLocator.GetChecked<IEventService>().AddListener<LateUpdateEvent>(handlePlayerLoopEvent);
                }

                case { } type when type == typeof(UpdateEvent):
                {
                    return ServiceLocator.GetChecked<IEventService>().AddListener<UpdateEvent>(handlePlayerLoopEvent);
                }

                case { } type when type == typeof(FirstFixedUpdateEvent):
                {
                    return ServiceLocator.GetChecked<IEventService>().AddListener<FirstFixedUpdateEvent>(handlePlayerLoopEvent);
                }

                case { } type when type == typeof(LastFixedUpdateEvent):
                {
                    return ServiceLocator.GetChecked<IEventService>().AddListener<LastFixedUpdateEvent>(handlePlayerLoopEvent);
                }

                case { } type when type == typeof(FirstInitializationEvent):
                {
                    return ServiceLocator.GetChecked<IEventService>().AddListener<FirstInitializationEvent>(handlePlayerLoopEvent);
                }

                case { } type when type == typeof(LastInitializationEvent):
                {
                    return ServiceLocator.GetChecked<IEventService>().AddListener<LastInitializationEvent>(handlePlayerLoopEvent);
                }

                case { } type when type == typeof(FirstEarlyUpdateEvent):
                {
                    return ServiceLocator.GetChecked<IEventService>().AddListener<FirstEarlyUpdateEvent>(handlePlayerLoopEvent);
                }

                case { } type when type == typeof(LastEarlyUpdateEvent):
                {
                    return ServiceLocator.GetChecked<IEventService>().AddListener<LastEarlyUpdateEvent>(handlePlayerLoopEvent);
                }

                case { } type when type == typeof(FirstPreUpdateEvent):
                {
                    return ServiceLocator.GetChecked<IEventService>().AddListener<FirstPreUpdateEvent>(handlePlayerLoopEvent);
                }

                case { } type when type == typeof(LastPreUpdateEvent):
                {
                    return ServiceLocator.GetChecked<IEventService>().AddListener<LastPreUpdateEvent>(handlePlayerLoopEvent);
                }

                case { } type when type == typeof(FirstUpdateEvent):
                {
                    return ServiceLocator.GetChecked<IEventService>().AddListener<FirstUpdateEvent>(handlePlayerLoopEvent);
                }

                case { } type when type == typeof(LastUpdateEvent):
                {
                    return ServiceLocator.GetChecked<IEventService>().AddListener<LastUpdateEvent>(handlePlayerLoopEvent);
                }

                case { } type when type == typeof(PreLateUpdateEvent):
                {
                    return ServiceLocator.GetChecked<IEventService>().AddListener<PreLateUpdateEvent>(handlePlayerLoopEvent);
                }

                case { } type when type == typeof(FirstPostLateUpdateEvent):
                {
                    return ServiceLocator.GetChecked<IEventService>().AddListener<FirstPostLateUpdateEvent>(handlePlayerLoopEvent);
                }

                case { } type when type == typeof(PostLateUpdateEvent):
                {
                    return ServiceLocator.GetChecked<IEventService>().AddListener<PostLateUpdateEvent>(handlePlayerLoopEvent);
                }

                case { } type when type == typeof(LastPostLateUpdateEvent):
                {
                    return ServiceLocator.GetChecked<IEventService>().AddListener<LastPostLateUpdateEvent>(handlePlayerLoopEvent);
                }

                case { } type when type == typeof(PreTimeUpdateEvent):
                {
                    return ServiceLocator.GetChecked<IEventService>().AddListener<PreTimeUpdateEvent>(handlePlayerLoopEvent);
                }

                case { } type when type == typeof(PostTimeUpdateEvent):
                {
                    return ServiceLocator.GetChecked<IEventService>().AddListener<PostTimeUpdateEvent>(handlePlayerLoopEvent);
                }

                default:
                {
                    return default;
                }
            }
        }

        /// <inheritdoc/>
        public bool RemoveAllListeners<T>()
            where T : IPlayerLoopEvent
        {
            return ServiceLocator.GetChecked<IEventService>().RemoveAllListeners<T>();
        }

        /// <inheritdoc/>
        protected override void OnInitialize()
        {
            base.OnInitialize();
            DontDestroyOnLoad(GameObject);

            PlayerLoopSettings settings = ScriptableSettings.Get<PlayerLoopSettings>();
            _currentInjectedTimings = settings.DefaultInjectedTimings;

            OnStarting += HandleStart;
            TryGetComponent(out _fixedUpdateListener);
            TryGetComponent(out _lateUpdateListener);
            TryGetComponent(out _updateListener);

            IEventService eventService = ServiceLocator.GetChecked<IEventService>();
            eventService.AddRelevancyListener<FixedUpdateEvent>(HandleFixedUpdateRelevancyChanged);
            eventService.AddRelevancyListener<LateUpdateEvent>(HandleLateUpdateRelevancyChanged);
            eventService.AddRelevancyListener<UpdateEvent>(HandleUpdateRelevancyChanged);

            if (eventService.GetListenerCount<FixedUpdateEvent>() > 0)
            {
                _fixedUpdateListener.OnTrigger += HandleFixedUpdate;
            }

            if (eventService.GetListenerCount<LateUpdateEvent>() > 0)
            {
                _lateUpdateListener.OnTrigger += HandleLateUpdate;
            }

            if (eventService.GetListenerCount<UpdateEvent>() > 0)
            {
                _updateListener.OnTrigger += HandleUpdate;
            }
        }

        private async UniTask InvokeFixedUpdateEventsAsync()
        {
            do
            {
                if (HasTiming(PlayerLoopTiming.FixedUpdate))
                {
                    await UniTask.Yield(PlayerLoopTiming.FixedUpdate, DestroyCancellationToken);

                    new FirstFixedUpdateEvent(Time.deltaTime).Invoke(this);
                }

                if (HasTiming(PlayerLoopTiming.LastFixedUpdate))
                {
                    await UniTask.Yield(PlayerLoopTiming.LastFixedUpdate, DestroyCancellationToken);

                    new LastFixedUpdateEvent(Time.deltaTime).Invoke(this);
                }
            }
            while (!DestroyCancellationToken.IsCancellationRequested && HasAnyFixedUpdateTiming());
        }

        [SuppressMessage("ReSharper", "CognitiveComplexity", Justification = "Can't simplify more without hurting performance or readability.")]
        private async UniTask InvokeMainUpdateEventsAsync()
        {
            do
            {
                if (HasTiming(PlayerLoopTiming.Initialization))
                {
                    await UniTask.Yield(PlayerLoopTiming.Initialization, DestroyCancellationToken);

                    new FirstInitializationEvent(Time.deltaTime).Invoke(this);
                }

                if (HasTiming(PlayerLoopTiming.LastInitialization))
                {
                    await UniTask.Yield(PlayerLoopTiming.LastInitialization, DestroyCancellationToken);

                    new LastInitializationEvent(Time.deltaTime).Invoke(this);
                }

                if (HasTiming(PlayerLoopTiming.EarlyUpdate))
                {
                    await UniTask.Yield(PlayerLoopTiming.EarlyUpdate, DestroyCancellationToken);

                    new FirstEarlyUpdateEvent(Time.deltaTime).Invoke(this);
                }

                if (HasTiming(PlayerLoopTiming.LastEarlyUpdate))
                {
                    await UniTask.Yield(PlayerLoopTiming.LastEarlyUpdate, DestroyCancellationToken);

                    new LastEarlyUpdateEvent(Time.deltaTime).Invoke(this);
                }

                if (HasTiming(PlayerLoopTiming.PreUpdate))
                {
                    await UniTask.Yield(PlayerLoopTiming.PreUpdate, DestroyCancellationToken);

                    new FirstPreUpdateEvent(Time.deltaTime).Invoke(this);
                }

                if (HasTiming(PlayerLoopTiming.LastPreUpdate))
                {
                    await UniTask.Yield(PlayerLoopTiming.LastPreUpdate, DestroyCancellationToken);

                    new LastPreUpdateEvent(Time.deltaTime).Invoke(this);
                }

                if (HasTiming(PlayerLoopTiming.Update))
                {
                    await UniTask.Yield(PlayerLoopTiming.Update, DestroyCancellationToken);

                    new FirstUpdateEvent(Time.deltaTime).Invoke(this);
                }

                if (HasTiming(PlayerLoopTiming.LastUpdate))
                {
                    await UniTask.Yield(PlayerLoopTiming.LastUpdate, DestroyCancellationToken);

                    new LastUpdateEvent(Time.deltaTime).Invoke(this);
                }

                if (HasTiming(PlayerLoopTiming.PreLateUpdate))
                {
                    await UniTask.Yield(PlayerLoopTiming.PreLateUpdate, DestroyCancellationToken);

                    new PreLateUpdateEvent(Time.deltaTime).Invoke(this);
                }

                if (HasTiming(PlayerLoopTiming.LastPreLateUpdate))
                {
                    await UniTask.Yield(PlayerLoopTiming.LastPreLateUpdate, DestroyCancellationToken);

                    new FirstPostLateUpdateEvent(Time.deltaTime).Invoke(this);
                }

                if (HasTiming(PlayerLoopTiming.PostLateUpdate))
                {
                    await UniTask.Yield(PlayerLoopTiming.PostLateUpdate, DestroyCancellationToken);

                    new PostLateUpdateEvent(Time.deltaTime).Invoke(this);
                }

                if (HasTiming(PlayerLoopTiming.LastPostLateUpdate))
                {
                    await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate, DestroyCancellationToken);

                    new LastPostLateUpdateEvent(Time.deltaTime).Invoke(this);
                }

                if (HasTiming(PlayerLoopTiming.TimeUpdate))
                {
                    await UniTask.Yield(PlayerLoopTiming.TimeUpdate, DestroyCancellationToken);

                    new PreTimeUpdateEvent(Time.deltaTime).Invoke(this);
                }

                if (HasTiming(PlayerLoopTiming.LastTimeUpdate))
                {
                    await UniTask.Yield(PlayerLoopTiming.LastTimeUpdate, DestroyCancellationToken);

                    new PostTimeUpdateEvent(Time.deltaTime).Invoke(this);
                }
            }
            while (!DestroyCancellationToken.IsCancellationRequested && HasAnyMainUpdateTiming());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool HasAnyFixedUpdateTiming()
        {
            return (_currentInjectedTimings & FixedUpdateTimings) != 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool HasAnyMainUpdateTiming()
        {
            return (_currentInjectedTimings & MainUpdateTimings) != 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool HasTiming(PlayerLoopTiming timing)
        {
            return ((int)_currentInjectedTimings & 1 << (int)timing) != 0;
        }

        private void HandleStart(Actor sender)
        {
            if (HasAnyFixedUpdateTiming())
            {
                InvokeFixedUpdateEventsAsync().Forget();
            }

            if (HasAnyMainUpdateTiming())
            {
                InvokeMainUpdateEventsAsync().Forget();
            }
        }

        private void HandleUpdateRelevancyChanged(IEventService service, Type type, bool relevant)
        {
            if (relevant)
            {
                _updateListener.OnTrigger += HandleUpdate;
            }
            else
            {
                _updateListener.OnTrigger -= HandleUpdate;
            }
        }

        private void HandleLateUpdateRelevancyChanged(IEventService service, Type type, bool relevant)
        {
            if (relevant)
            {
                _lateUpdateListener.OnTrigger += HandleLateUpdate;
            }
            else
            {
                _lateUpdateListener.OnTrigger -= HandleLateUpdate;
            }
        }

        private void HandleFixedUpdateRelevancyChanged(IEventService service, Type type, bool relevant)
        {
            if (relevant)
            {
                _fixedUpdateListener.OnTrigger += HandleFixedUpdate;
            }
            else
            {
                _fixedUpdateListener.OnTrigger -= HandleFixedUpdate;
            }
        }

        private void HandleFixedUpdate(PlayerLoopListenerBase sender, float deltaTime)
        {
            new FixedUpdateEvent(deltaTime).Invoke(this);
        }

        private void HandleLateUpdate(PlayerLoopListenerBase sender, float deltaTime)
        {
            new LateUpdateEvent(deltaTime).Invoke(this);
        }

        private void HandleUpdate(PlayerLoopListenerBase sender, float deltaTime)
        {
            new UpdateEvent(deltaTime).Invoke(this);
        }
    }
}
