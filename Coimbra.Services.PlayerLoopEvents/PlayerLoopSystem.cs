#nullable enable

using Coimbra.Services.Events;
using Cysharp.Threading.Tasks;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Coimbra.Services.PlayerLoopEvents
{
    /// <summary>
    /// Default implementation for <see cref="IPlayerLoopService"/>.
    /// </summary>
    [AddComponentMenu("")]
    [PreloadService]
    [RequireComponent(typeof(StartListener))]
    [RequireComponent(typeof(FixedUpdateListener))]
    [RequireComponent(typeof(LateUpdateListener))]
    [RequireComponent(typeof(UpdateListener))]
    public sealed class PlayerLoopSystem : ServiceActorBase, IPlayerLoopService
    {
        private const InjectPlayerLoopTimings FixedUpdateTimings = InjectPlayerLoopTimings.FixedUpdate | InjectPlayerLoopTimings.LastFixedUpdate;

        private const InjectPlayerLoopTimings MainUpdateTimings = InjectPlayerLoopTimings.All & ~FixedUpdateTimings;

        [SerializeField]
        [Disable]
        private InjectPlayerLoopTimings _currentTimings = InjectPlayerLoopTimings.All;

        private FixedUpdateListener _fixedUpdateListener = null!;

        private LateUpdateListener _lateUpdateListener = null!;

        private UpdateListener _updateListener = null!;

        private PlayerLoopSystem() { }

        /// <inheritdoc/>
        public InjectPlayerLoopTimings CurrentTimings
        {
            get => _currentTimings;
            set
            {
                if (IsDestroyed)
                {
                    return;
                }

                bool hadAnyFixedUpdateTiming = HasAnyFixedUpdateTiming();
                bool hadAnyMainUpdateTiming = HasAnyMainUpdateTiming();
                _currentTimings = value;

                if (!hadAnyFixedUpdateTiming && HasAnyFixedUpdateTiming())
                {
                    InvokeFixedUpdateEvents().AttachExternalCancellation(DestroyCancellationToken);
                }

                if (!hadAnyMainUpdateTiming && HasAnyMainUpdateTiming())
                {
                    InvokeMainUpdateEvents().AttachExternalCancellation(DestroyCancellationToken);
                }
            }
        }

        /// <inheritdoc/>
        public void RemoveAllListeners<T>()
            where T : IPlayerLoopEvent
        {
            ServiceLocator.Get<IEventService>()?.RemoveAllListeners<T>();
        }

        /// <inheritdoc/>
        protected override void OnDestroyed()
        {
            base.OnDestroyed();
            ServiceLocator.RemoveSetListener<IEventService>(HandleEventServiceSet);
        }

        /// <inheritdoc/>
        protected override void OnInitialize()
        {
            base.OnInitialize();
            DontDestroyOnLoad(GameObject);

            if (ScriptableSettings.TryGetOrFind(out PlayerLoopSettings settings))
            {
                _currentTimings = settings.DefaultTimings;
            }

            TryGetComponent(out _fixedUpdateListener);
            TryGetComponent(out _lateUpdateListener);
            TryGetComponent(out _updateListener);

            GetComponent<StartListener>().OnTrigger += delegate
            {
                if (HasAnyFixedUpdateTiming())
                {
                    InvokeFixedUpdateEvents().AttachExternalCancellation(DestroyCancellationToken);
                }

                if (HasAnyMainUpdateTiming())
                {
                    InvokeMainUpdateEvents().AttachExternalCancellation(DestroyCancellationToken);
                }
            };

            ServiceLocator.AddSetListener<IEventService>(HandleEventServiceSet);
            InitializeEventService();
        }

        private async UniTask InvokeFixedUpdateEvents()
        {
            do
            {
                if (HasTiming(PlayerLoopTiming.FixedUpdate))
                {
                    await UniTask.Yield(PlayerLoopTiming.FixedUpdate);

                    new FirstFixedUpdateEvent(Time.deltaTime).Invoke(this);
                }

                if (HasTiming(PlayerLoopTiming.LastFixedUpdate))
                {
                    await UniTask.Yield(PlayerLoopTiming.LastFixedUpdate);

                    new LastFixedUpdateEvent(Time.deltaTime).Invoke(this);
                }
            }

            while (HasAnyFixedUpdateTiming());
        }

        private async UniTask InvokeMainUpdateEvents()
        {
            do
            {
                if (HasTiming(PlayerLoopTiming.Initialization))
                {
                    await UniTask.Yield(PlayerLoopTiming.Initialization);

                    new FirstInitializationEvent(Time.deltaTime).Invoke(this);
                }

                if (HasTiming(PlayerLoopTiming.LastInitialization))
                {
                    await UniTask.Yield(PlayerLoopTiming.LastInitialization);

                    new LastInitializationEvent(Time.deltaTime).Invoke(this);
                }

                if (HasTiming(PlayerLoopTiming.EarlyUpdate))
                {
                    await UniTask.Yield(PlayerLoopTiming.EarlyUpdate);

                    new FirstEarlyUpdateEvent(Time.deltaTime).Invoke(this);
                }

                if (HasTiming(PlayerLoopTiming.LastEarlyUpdate))
                {
                    await UniTask.Yield(PlayerLoopTiming.LastEarlyUpdate);

                    new LastEarlyUpdateEvent(Time.deltaTime).Invoke(this);
                }

                if (HasTiming(PlayerLoopTiming.PreUpdate))
                {
                    await UniTask.Yield(PlayerLoopTiming.PreUpdate);

                    new FirstPreUpdateEvent(Time.deltaTime).Invoke(this);
                }

                if (HasTiming(PlayerLoopTiming.LastPreUpdate))
                {
                    await UniTask.Yield(PlayerLoopTiming.LastPreUpdate);

                    new LastPreUpdateEvent(Time.deltaTime).Invoke(this);
                }

                if (HasTiming(PlayerLoopTiming.Update))
                {
                    await UniTask.Yield(PlayerLoopTiming.Update);

                    new FirstUpdateEvent(Time.deltaTime).Invoke(this);
                }

                if (HasTiming(PlayerLoopTiming.LastUpdate))
                {
                    await UniTask.Yield(PlayerLoopTiming.LastUpdate);

                    new LastUpdateEvent(Time.deltaTime).Invoke(this);
                }

                if (HasTiming(PlayerLoopTiming.PreLateUpdate))
                {
                    await UniTask.Yield(PlayerLoopTiming.PreLateUpdate);

                    new PreLateUpdateEvent(Time.deltaTime).Invoke(this);
                }

                if (HasTiming(PlayerLoopTiming.LastPreLateUpdate))
                {
                    await UniTask.Yield(PlayerLoopTiming.LastPreLateUpdate);

                    new FirstPostLateUpdateEvent(Time.deltaTime).Invoke(this);
                }

                if (HasTiming(PlayerLoopTiming.PostLateUpdate))
                {
                    await UniTask.Yield(PlayerLoopTiming.PostLateUpdate);

                    new PostLateUpdateEvent(Time.deltaTime).Invoke(this);
                }

                if (HasTiming(PlayerLoopTiming.LastPostLateUpdate))
                {
                    await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate);

                    new LastPostLateUpdateEvent(Time.deltaTime).Invoke(this);
                }

                if (HasTiming(PlayerLoopTiming.TimeUpdate))
                {
                    await UniTask.Yield(PlayerLoopTiming.TimeUpdate);

                    new PreTimeUpdateEvent(Time.deltaTime).Invoke(this);
                }

                if (HasTiming(PlayerLoopTiming.LastTimeUpdate))
                {
                    await UniTask.Yield(PlayerLoopTiming.LastTimeUpdate);

                    new PostTimeUpdateEvent(Time.deltaTime).Invoke(this);
                }
            }
            while (HasAnyMainUpdateTiming());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool HasAnyFixedUpdateTiming()
        {
            return (_currentTimings & FixedUpdateTimings) != 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool HasAnyMainUpdateTiming()
        {
            return (_currentTimings & MainUpdateTimings) != 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool HasTiming(PlayerLoopTiming timing)
        {
            return ((int)_currentTimings & 1 << (int)timing) != 0;
        }

        private void InitializeEventService()
        {
            if (!ServiceLocator.TryGet(out IEventService? eventService))
            {
                return;
            }

            eventService!.AddRelevancyListener<FixedUpdateEvent>(delegate(IEventService service, Type type, bool relevant)
            {
                if (relevant)
                {
                    _fixedUpdateListener.OnTrigger += HandleFixedUpdate;
                }
                else
                {
                    _fixedUpdateListener.OnTrigger -= HandleFixedUpdate;
                }
            });

            eventService.AddRelevancyListener<LateUpdateEvent>(delegate(IEventService service, Type type, bool relevant)
            {
                if (relevant)
                {
                    _lateUpdateListener.OnTrigger += HandleLateUpdate;
                }
                else
                {
                    _lateUpdateListener.OnTrigger -= HandleLateUpdate;
                }
            });

            eventService.AddRelevancyListener<UpdateEvent>(delegate(IEventService service, Type type, bool relevant)
            {
                if (relevant)
                {
                    _updateListener.OnTrigger += HandleUpdate;
                }
                else
                {
                    _updateListener.OnTrigger -= HandleUpdate;
                }
            });
        }

        private void HandleEventServiceSet(Type service)
        {
            InitializeEventService();
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
