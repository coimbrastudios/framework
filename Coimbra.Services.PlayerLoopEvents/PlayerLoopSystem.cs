#nullable enable

using Coimbra.Services.Events;
using Cysharp.Threading.Tasks;
using System.Collections;
using UnityEngine;

namespace Coimbra.Services.PlayerLoopEvents
{
    /// <summary>
    /// Default implementation for <see cref="IPlayerLoopService"/>.
    /// </summary>
    [AddComponentMenu("")]
    [PreloadService]
    public sealed class PlayerLoopSystem : ServiceActorBase<PlayerLoopSystem, IPlayerLoopService>, IPlayerLoopService
    {
        private PlayerLoopSystem() { }

        private IEventService? EventService => OwningLocator?.Get<IEventService>();

        /// <inheritdoc/>
        public void RemoveAllListeners<T>()
            where T : IPlayerLoopEvent
        {
            EventService?.RemoveAllListeners<T>();
        }

        /// <inheritdoc/>
        protected override void OnInitialize()
        {
            base.OnInitialize();
            DontDestroyOnLoad(GameObject);

            IEnumerator coroutine()
            {
                yield return null;

                InvokeFixedUpdateEvents().AttachExternalCancellation(DestroyCancellationToken);
                InvokeMainUpdateEvents().AttachExternalCancellation(DestroyCancellationToken);
            }

            StartCoroutine(coroutine());
        }

        private async UniTask InvokeFixedUpdateEvents()
        {
            while (!DestroyCancellationToken.IsCancellationRequested)
            {
                await UniTask.Yield(PlayerLoopTiming.FixedUpdate);

                float deltaTime = Time.deltaTime;
                new FirstFixedUpdateEvent(deltaTime).TryInvokeAt(EventService, this);

                await UniTask.Yield(PlayerLoopTiming.LastFixedUpdate);

                new LastFixedUpdateEvent(deltaTime).TryInvokeAt(EventService, this);
            }
        }

        private async UniTask InvokeMainUpdateEvents()
        {
            while (!DestroyCancellationToken.IsCancellationRequested)
            {
                await UniTask.Yield(PlayerLoopTiming.Initialization);

                float deltaTime = Time.deltaTime;

                new FirstInitializationEvent(deltaTime).TryInvokeAt(EventService, this);

                await UniTask.Yield(PlayerLoopTiming.LastInitialization);

                new LastInitializationEvent(deltaTime).TryInvokeAt(EventService, this);

                await UniTask.Yield(PlayerLoopTiming.EarlyUpdate);

                deltaTime = Time.deltaTime;

                new FirstEarlyUpdateEvent(deltaTime).TryInvokeAt(EventService, this);

                await UniTask.Yield(PlayerLoopTiming.LastEarlyUpdate);

                new LastEarlyUpdateEvent(deltaTime).TryInvokeAt(EventService, this);

                await UniTask.Yield(PlayerLoopTiming.PreUpdate);

                deltaTime = Time.deltaTime;

                new FirstPreUpdateEvent(deltaTime).TryInvokeAt(EventService, this);

                await UniTask.Yield(PlayerLoopTiming.LastPreUpdate);

                new LastPreUpdateEvent(deltaTime).TryInvokeAt(EventService, this);

                await UniTask.Yield(PlayerLoopTiming.Update);

                deltaTime = Time.deltaTime;

                new FirstUpdateEvent(deltaTime).TryInvokeAt(EventService, this);

                await UniTask.Yield(PlayerLoopTiming.LastUpdate);

                new LastUpdateEvent(deltaTime).TryInvokeAt(EventService, this);

                await UniTask.Yield(PlayerLoopTiming.PreLateUpdate);

                deltaTime = Time.deltaTime;

                new PreLateUpdateEvent(deltaTime).TryInvokeAt(EventService, this);

                await UniTask.Yield(PlayerLoopTiming.LastPreLateUpdate);

                new FirstPostLateUpdateEvent(deltaTime).TryInvokeAt(EventService, this);

                await UniTask.Yield(PlayerLoopTiming.PostLateUpdate);

                deltaTime = Time.deltaTime;

                new PostLateUpdateEvent(deltaTime).TryInvokeAt(EventService, this);

                await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate);

                new LastPostLateUpdateEvent(deltaTime).TryInvokeAt(EventService, this);

                await UniTask.Yield(PlayerLoopTiming.TimeUpdate);

                new PreTimeUpdateEvent(Time.deltaTime).TryInvokeAt(EventService, this);

                await UniTask.Yield(PlayerLoopTiming.LastTimeUpdate);

                new PostTimeUpdateEvent(Time.deltaTime).TryInvokeAt(EventService, this);
            }
        }

        private void FixedUpdate()
        {
            new FixedUpdateEvent(Time.deltaTime).TryInvokeAt(EventService, this);
        }

        private void Update()
        {
            new UpdateEvent(Time.deltaTime).TryInvokeAt(EventService, this);
        }

        private void LateUpdate()
        {
            new LateUpdateEvent(Time.deltaTime).TryInvokeAt(EventService, this);
        }
    }
}
