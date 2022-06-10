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
        private PlayerLoopSystem() { }

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

            GetComponent<StartListener>().OnTrigger += delegate
            {
                InvokeFixedUpdateEvents().AttachExternalCancellation(DestroyCancellationToken);
                InvokeMainUpdateEvents().AttachExternalCancellation(DestroyCancellationToken);
            };
        }

        private async UniTask InvokeFixedUpdateEvents()
        {
            while (!DestroyCancellationToken.IsCancellationRequested)
            {
                await UniTask.Yield(PlayerLoopTiming.FixedUpdate);

                float deltaTime = Time.deltaTime;
                Invoke(new FirstFixedUpdateEvent(deltaTime));

                await UniTask.Yield(PlayerLoopTiming.LastFixedUpdate);

                Invoke(new LastFixedUpdateEvent(deltaTime));
            }
        }

        private async UniTask InvokeMainUpdateEvents()
        {
            while (!DestroyCancellationToken.IsCancellationRequested)
            {
                await UniTask.Yield(PlayerLoopTiming.Initialization);

                float deltaTime = Time.deltaTime;

                Invoke(new FirstInitializationEvent(deltaTime));

                await UniTask.Yield(PlayerLoopTiming.LastInitialization);

                Invoke(new LastInitializationEvent(deltaTime));

                await UniTask.Yield(PlayerLoopTiming.EarlyUpdate);

                deltaTime = Time.deltaTime;

                Invoke(new FirstEarlyUpdateEvent(deltaTime));

                await UniTask.Yield(PlayerLoopTiming.LastEarlyUpdate);

                Invoke(new LastEarlyUpdateEvent(deltaTime));

                await UniTask.Yield(PlayerLoopTiming.PreUpdate);

                deltaTime = Time.deltaTime;

                Invoke(new FirstPreUpdateEvent(deltaTime));

                await UniTask.Yield(PlayerLoopTiming.LastPreUpdate);

                Invoke(new LastPreUpdateEvent(deltaTime));

                await UniTask.Yield(PlayerLoopTiming.Update);

                deltaTime = Time.deltaTime;

                Invoke(new FirstUpdateEvent(deltaTime));

                await UniTask.Yield(PlayerLoopTiming.LastUpdate);

                Invoke(new LastUpdateEvent(deltaTime));

                await UniTask.Yield(PlayerLoopTiming.PreLateUpdate);

                deltaTime = Time.deltaTime;

                Invoke(new PreLateUpdateEvent(deltaTime));

                await UniTask.Yield(PlayerLoopTiming.LastPreLateUpdate);

                Invoke(new FirstPostLateUpdateEvent(deltaTime));

                await UniTask.Yield(PlayerLoopTiming.PostLateUpdate);

                deltaTime = Time.deltaTime;

                Invoke(new PostLateUpdateEvent(deltaTime));

                await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate);

                Invoke(new LastPostLateUpdateEvent(deltaTime));

                await UniTask.Yield(PlayerLoopTiming.TimeUpdate);

                Invoke(new PreTimeUpdateEvent(Time.deltaTime));

                await UniTask.Yield(PlayerLoopTiming.LastTimeUpdate);

                Invoke(new PostTimeUpdateEvent(Time.deltaTime));
            }
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
        private void Invoke<T>(in T e)
            where T : struct, IPlayerLoopEvent
        {
            OwningLocator?.Get<IEventService>()?.Invoke(this, in e);
        }
    }
}
