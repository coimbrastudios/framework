using Cysharp.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;

namespace Coimbra
{
    [AddComponentMenu("")]
    [DisallowMultipleComponent]
    internal sealed class ApplicationSystem : MonoBehaviourServiceBase<IApplicationService>, IApplicationService
    {
        private readonly object _eventKey = new object();
        private IEventService _eventService;

        /// <inheritdoc/>
        protected override void OnDispose()
        {
            base.OnDispose();
            SetEventService(null);
            OwningLocator = null;
        }

        /// <inheritdoc/>
        protected override void OnOwningLocatorChanged(ServiceLocator oldValue, ServiceLocator newValue)
        {
            base.OnOwningLocatorChanged(oldValue, newValue);
            oldValue?.RemoveValueChangedListener<IEventService>(HandleEventServiceChanged);
            newValue?.AddValueChangedListener<IEventService>(HandleEventServiceChanged);
            SetEventService(newValue?.Get<IEventService>());
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            ServiceLocator.Shared.SetCreateCallback(Create, false);
            ServiceLocator.Shared.Set(Create());
        }

        private static IApplicationService Create()
        {
            GameObject gameObject = new GameObject(nameof(ApplicationSystem));
            DontDestroyOnLoad(gameObject);

            return gameObject.AddComponent<ApplicationSystem>();
        }

        private void Start()
        {
            CancellationToken token = gameObject.GetCancellationTokenOnDestroy();
            InvokeFixedUpdateEvents().AttachExternalCancellation(token);
            InvokeMainUpdateEvents().AttachExternalCancellation(token);
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

        private void OnApplicationFocus(bool hasFocus)
        {
            Invoke(new ApplicationFocusEvent(hasFocus));
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            Invoke(new ApplicationPauseEvent(pauseStatus));
        }

        private void OnApplicationQuit()
        {
            Invoke(new ApplicationQuitEvent());
#if UNITY_EDITOR
            OwningLocator?.Dispose();
#endif
        }

        private void HandleEventServiceChanged(IService oldValue, IService newValue)
        {
            SetEventService(newValue as IEventService);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Invoke<T>(T e)
            where T : IEvent
        {
            _eventService?.Invoke(this, ref e, _eventKey);
        }

        private async UniTask InvokeFixedUpdateEvents()
        {
            while (this != null)
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
            while (this != null)
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

                Invoke(new FistPostLateUpdateEvent(deltaTime));

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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ResetEventKey<T>()
            where T : IEvent
        {
            _eventService.ResetEventKey<T>(_eventKey);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SetEventKey<T>()
            where T : IEvent
        {
            _eventService.SetEventKey<T>(_eventKey);
        }

        private void SetEventService(IEventService service)
        {
            if (_eventService != null)
            {
                ResetEventKey<ApplicationFocusEvent>();
                ResetEventKey<ApplicationPauseEvent>();
                ResetEventKey<ApplicationQuitEvent>();
                ResetEventKey<FixedUpdateEvent>();
                ResetEventKey<LateUpdateEvent>();
                ResetEventKey<UpdateEvent>();
            }

            _eventService = service;

            if (_eventService == null)
            {
                return;
            }

            SetEventKey<ApplicationFocusEvent>();
            SetEventKey<ApplicationPauseEvent>();
            SetEventKey<ApplicationQuitEvent>();
            SetEventKey<FixedUpdateEvent>();
            SetEventKey<LateUpdateEvent>();
            SetEventKey<UpdateEvent>();
        }
    }
}
