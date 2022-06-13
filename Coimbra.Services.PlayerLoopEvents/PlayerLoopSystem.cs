#nullable enable

using Coimbra.Services.Events;
using Cysharp.Threading.Tasks;
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
    public sealed class PlayerLoopSystem : ServiceActorBase<PlayerLoopSystem, IPlayerLoopService>, IPlayerLoopService
    {
        private const InjectPlayerLoopTimings FixedUpdateTimings = InjectPlayerLoopTimings.FixedUpdate | InjectPlayerLoopTimings.LastFixedUpdate;

        private const InjectPlayerLoopTimings MainUpdateTimings = InjectPlayerLoopTimings.All & ~FixedUpdateTimings;

        [SerializeField]
        [Disable]
        private InjectPlayerLoopTimings _currentTimings = InjectPlayerLoopTimings.All;

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
            OwningLocator?.Get<IEventService>()?.RemoveAllListeners<T>();
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
        }

        private async UniTask InvokeFixedUpdateEvents()
        {
            do
            {
                if (HasTiming(PlayerLoopTiming.FixedUpdate))
                {
                    await UniTask.Yield(PlayerLoopTiming.FixedUpdate);

                    Invoke(new FirstFixedUpdateEvent(Time.deltaTime));
                }

                if (HasTiming(PlayerLoopTiming.LastFixedUpdate))
                {
                    await UniTask.Yield(PlayerLoopTiming.LastFixedUpdate);

                    Invoke(new LastFixedUpdateEvent(Time.deltaTime));
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

                    Invoke(new FirstInitializationEvent(Time.deltaTime));
                }

                if (HasTiming(PlayerLoopTiming.LastInitialization))
                {
                    await UniTask.Yield(PlayerLoopTiming.LastInitialization);

                    Invoke(new LastInitializationEvent(Time.deltaTime));
                }

                if (HasTiming(PlayerLoopTiming.EarlyUpdate))
                {
                    await UniTask.Yield(PlayerLoopTiming.EarlyUpdate);

                    Invoke(new FirstEarlyUpdateEvent(Time.deltaTime));
                }

                if (HasTiming(PlayerLoopTiming.LastEarlyUpdate))
                {
                    await UniTask.Yield(PlayerLoopTiming.LastEarlyUpdate);

                    Invoke(new LastEarlyUpdateEvent(Time.deltaTime));
                }

                if (HasTiming(PlayerLoopTiming.PreUpdate))
                {
                    await UniTask.Yield(PlayerLoopTiming.PreUpdate);

                    Invoke(new FirstPreUpdateEvent(Time.deltaTime));
                }

                if (HasTiming(PlayerLoopTiming.LastPreUpdate))
                {
                    await UniTask.Yield(PlayerLoopTiming.LastPreUpdate);

                    Invoke(new LastPreUpdateEvent(Time.deltaTime));
                }

                if (HasTiming(PlayerLoopTiming.Update))
                {
                    await UniTask.Yield(PlayerLoopTiming.Update);

                    Invoke(new FirstUpdateEvent(Time.deltaTime));
                }

                if (HasTiming(PlayerLoopTiming.LastUpdate))
                {
                    await UniTask.Yield(PlayerLoopTiming.LastUpdate);

                    Invoke(new LastUpdateEvent(Time.deltaTime));
                }

                if (HasTiming(PlayerLoopTiming.PreLateUpdate))
                {
                    await UniTask.Yield(PlayerLoopTiming.PreLateUpdate);

                    Invoke(new PreLateUpdateEvent(Time.deltaTime));
                }

                if (HasTiming(PlayerLoopTiming.LastPreLateUpdate))
                {
                    await UniTask.Yield(PlayerLoopTiming.LastPreLateUpdate);

                    Invoke(new FirstPostLateUpdateEvent(Time.deltaTime));
                }

                if (HasTiming(PlayerLoopTiming.PostLateUpdate))
                {
                    await UniTask.Yield(PlayerLoopTiming.PostLateUpdate);

                    Invoke(new PostLateUpdateEvent(Time.deltaTime));
                }

                if (HasTiming(PlayerLoopTiming.LastPostLateUpdate))
                {
                    await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate);

                    Invoke(new LastPostLateUpdateEvent(Time.deltaTime));
                }

                if (HasTiming(PlayerLoopTiming.TimeUpdate))
                {
                    await UniTask.Yield(PlayerLoopTiming.TimeUpdate);

                    Invoke(new PreTimeUpdateEvent(Time.deltaTime));
                }

                if (HasTiming(PlayerLoopTiming.LastTimeUpdate))
                {
                    await UniTask.Yield(PlayerLoopTiming.LastTimeUpdate);

                    Invoke(new PostTimeUpdateEvent(Time.deltaTime));
                }
            }
            while (HasAnyMainUpdateTiming());
        }

        private void FixedUpdate()
        {
            Invoke(new FixedUpdateEvent(Time.deltaTime));
        }

        private void Update()
        {
            Invoke(new UpdateEvent(Time.deltaTime));
        }

        private void LateUpdate()
        {
            Invoke(new LateUpdateEvent(Time.deltaTime));
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Invoke<T>(in T e)
            where T : struct, IPlayerLoopEvent
        {
            OwningLocator?.Get<IEventService>()?.Invoke(this, in e);
        }
    }
}
