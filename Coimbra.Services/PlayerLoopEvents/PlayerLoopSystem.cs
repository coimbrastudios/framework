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
    public sealed class PlayerLoopSystem : EventServiceActorBase<PlayerLoopSystem, IPlayerLoopService>, IPlayerLoopService
    {
        private readonly EventKey _eventKey = new EventKey(EventKey.RestrictionOptions.DisallowInvoke);

        private PlayerLoopSystem() { }

        /// <summary>
        /// Create a new <see cref="IPlayerLoopService"/>.
        /// </summary>
        public static IPlayerLoopService Create()
        {
            return new GameObject(nameof(PlayerLoopSystem)).AddComponent<PlayerLoopSystem>();
        }

        /// <inheritdoc/>
        protected override void OnEventServiceChanged(IEventService previous, IEventService current)
        {
            previous?.ResetAllEventKeys(_eventKey);

            if (current == null)
            {
                return;
            }

            current.SetEventKey<FixedUpdateEvent>(_eventKey);
            current.SetEventKey<LateUpdateEvent>(_eventKey);
            current.SetEventKey<UpdateEvent>(_eventKey);
            current.SetEventKey<FirstEarlyUpdateEvent>(_eventKey);
            current.SetEventKey<FirstFixedUpdateEvent>(_eventKey);
            current.SetEventKey<FirstInitializationEvent>(_eventKey);
            current.SetEventKey<FirstPostLateUpdateEvent>(_eventKey);
            current.SetEventKey<FirstPreUpdateEvent>(_eventKey);
            current.SetEventKey<FirstUpdateEvent>(_eventKey);
            current.SetEventKey<LastEarlyUpdateEvent>(_eventKey);
            current.SetEventKey<LastFixedUpdateEvent>(_eventKey);
            current.SetEventKey<LastInitializationEvent>(_eventKey);
            current.SetEventKey<LastPostLateUpdateEvent>(_eventKey);
            current.SetEventKey<LastPreUpdateEvent>(_eventKey);
            current.SetEventKey<LastUpdateEvent>(_eventKey);
            current.SetEventKey<PostLateUpdateEvent>(_eventKey);
            current.SetEventKey<PostTimeUpdateEvent>(_eventKey);
            current.SetEventKey<PreLateUpdateEvent>(_eventKey);
            current.SetEventKey<PreTimeUpdateEvent>(_eventKey);
        }

        /// <inheritdoc/>
        protected override void OnInitialize()
        {
            base.OnInitialize();
            DontDestroyOnLoad(CachedGameObject);

            IEnumerator coroutine()
            {
                yield return null;

                InvokeFixedUpdateEvents().AttachExternalCancellation(DestroyCancellationToken);
                InvokeMainUpdateEvents().AttachExternalCancellation(DestroyCancellationToken);
            }

            StartCoroutine(coroutine());
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void HandleSubsystemRegistration()
        {
            ServiceLocator.Shared.SetCreateCallback(Create, false);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void HandleBeforeSceneLoad()
        {
            ServiceLocator.Shared.Get<IPlayerLoopService>();
        }

        private async UniTask InvokeFixedUpdateEvents()
        {
            while (this != null)
            {
                await UniTask.Yield(PlayerLoopTiming.FixedUpdate);

                float deltaTime = Time.deltaTime;
                new FirstFixedUpdateEvent(deltaTime).InvokeAt(EventService, this, _eventKey);

                await UniTask.Yield(PlayerLoopTiming.LastFixedUpdate);

                new LastFixedUpdateEvent(deltaTime).InvokeAt(EventService, this, _eventKey);
            }
        }

        private async UniTask InvokeMainUpdateEvents()
        {
            while (this != null)
            {
                await UniTask.Yield(PlayerLoopTiming.Initialization);

                float deltaTime = Time.deltaTime;

                new FirstInitializationEvent(deltaTime).InvokeAt(EventService, this, _eventKey);

                await UniTask.Yield(PlayerLoopTiming.LastInitialization);

                new LastInitializationEvent(deltaTime).InvokeAt(EventService, this, _eventKey);

                await UniTask.Yield(PlayerLoopTiming.EarlyUpdate);

                deltaTime = Time.deltaTime;

                new FirstEarlyUpdateEvent(deltaTime).InvokeAt(EventService, this, _eventKey);

                await UniTask.Yield(PlayerLoopTiming.LastEarlyUpdate);

                new LastEarlyUpdateEvent(deltaTime).InvokeAt(EventService, this, _eventKey);

                await UniTask.Yield(PlayerLoopTiming.PreUpdate);

                deltaTime = Time.deltaTime;

                new FirstPreUpdateEvent(deltaTime).InvokeAt(EventService, this, _eventKey);

                await UniTask.Yield(PlayerLoopTiming.LastPreUpdate);

                new LastPreUpdateEvent(deltaTime).InvokeAt(EventService, this, _eventKey);

                await UniTask.Yield(PlayerLoopTiming.Update);

                deltaTime = Time.deltaTime;

                new FirstUpdateEvent(deltaTime).InvokeAt(EventService, this, _eventKey);

                await UniTask.Yield(PlayerLoopTiming.LastUpdate);

                new LastUpdateEvent(deltaTime).InvokeAt(EventService, this, _eventKey);

                await UniTask.Yield(PlayerLoopTiming.PreLateUpdate);

                deltaTime = Time.deltaTime;

                new PreLateUpdateEvent(deltaTime).InvokeAt(EventService, this, _eventKey);

                await UniTask.Yield(PlayerLoopTiming.LastPreLateUpdate);

                new FirstPostLateUpdateEvent(deltaTime).InvokeAt(EventService, this, _eventKey);

                await UniTask.Yield(PlayerLoopTiming.PostLateUpdate);

                deltaTime = Time.deltaTime;

                new PostLateUpdateEvent(deltaTime).InvokeAt(EventService, this, _eventKey);

                await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate);

                new LastPostLateUpdateEvent(deltaTime).InvokeAt(EventService, this, _eventKey);

                await UniTask.Yield(PlayerLoopTiming.TimeUpdate);

                new PreTimeUpdateEvent(Time.deltaTime).InvokeAt(EventService, this, _eventKey);

                await UniTask.Yield(PlayerLoopTiming.LastTimeUpdate);

                new PostTimeUpdateEvent(Time.deltaTime).InvokeAt(EventService, this, _eventKey);
            }
        }

        private void FixedUpdate()
        {
            EventService?.Invoke(this, new FixedUpdateEvent(Time.deltaTime), _eventKey);
        }

        private void Update()
        {
            EventService?.Invoke(this, new UpdateEvent(Time.deltaTime), _eventKey);
        }

        private void LateUpdate()
        {
            EventService?.Invoke(this, new LateUpdateEvent(Time.deltaTime), _eventKey);
        }
    }
}
