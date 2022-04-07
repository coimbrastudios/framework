using Cysharp.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Coimbra
{
    /// <summary>
    /// Default implementation for <see cref="IApplicationService"/>.
    /// </summary>
    [AddComponentMenu("")]
    public sealed class ApplicationSystem : ServiceActorBase<IApplicationService>, IApplicationService
    {
        private readonly EventKey _eventKey = new EventKey(EventKey.RestrictionOptions.DisallowInvoke);

        private IEventService _eventService;

        private ApplicationSystem() { }

        /// <summary>
        /// Create a new <see cref="IApplicationService"/>.
        /// </summary>
        public static IApplicationService Create()
        {
            return new GameObject(nameof(ApplicationSystem)).AddComponent<ApplicationSystem>();
        }

        /// <inheritdoc/>
        protected override void OnDestroying()
        {
            base.OnDestroying();
            SetEventService(null);
            SceneManager.sceneLoaded -= HandleSceneLoaded;
        }

        /// <inheritdoc/>
        protected override void OnInitialize()
        {
            base.OnInitialize();
            DontDestroyOnLoad(CachedGameObject);
            SceneManager.sceneLoaded += HandleSceneLoaded;
            OnDestroyed += HandleDestroyed;
        }

        /// <inheritdoc/>
        protected override void OnOwningLocatorChanged(ServiceLocator oldValue, ServiceLocator newValue)
        {
            base.OnOwningLocatorChanged(oldValue, newValue);
            oldValue?.RemoveValueChangedListener<IEventService>(HandleEventServiceChanged);
            newValue?.AddValueChangedListener<IEventService>(HandleEventServiceChanged);
            SetEventService(newValue?.Get<IEventService>());
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void HandleSubsystemRegistration()
        {
            ServiceLocator.Shared.SetCreateCallback(Create, false);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void HandleBeforeSceneLoad()
        {
            ServiceLocator.Shared.Get<IApplicationService>();
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

        private void Start()
        {
            CancellationToken token = CachedGameObject.GetCancellationTokenOnDestroy();
            InvokeFixedUpdateEvents().AttachExternalCancellation(token);
            InvokeMainUpdateEvents().AttachExternalCancellation(token);
            InitializeAllActors();
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

        private void HandleDestroyed(Actor sender, DestroyReason reason)
        {
            if (reason != DestroyReason.ApplicationQuit)
            {
                return;
            }

            Invoke(new ApplicationQuitEvent());
#if UNITY_EDITOR
            OwningLocator?.Dispose();
#endif
        }

        private void HandleEventServiceChanged(IService oldValue, IService newValue)
        {
            SetEventService(newValue as IEventService);
        }

        private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            InitializeAllActors();
            Invoke(new SceneLoadedEvent(scene, mode));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Invoke<T>(T e)
            where T : IEvent
        {
            _eventService?.Invoke(this, ref e, _eventKey);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SetEventKey<T>()
            where T : IEvent
        {
            _eventService.SetEventKey<T>(_eventKey);
        }

        private void SetEventService(IEventService service)
        {
            _eventService?.ResetAllEventKeys(_eventKey);

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
            SetEventKey<FirstEarlyUpdateEvent>();
            SetEventKey<FirstFixedUpdateEvent>();
            SetEventKey<FirstInitializationEvent>();
            SetEventKey<FirstPostLateUpdateEvent>();
            SetEventKey<FirstPreUpdateEvent>();
            SetEventKey<FirstUpdateEvent>();
            SetEventKey<LastEarlyUpdateEvent>();
            SetEventKey<LastFixedUpdateEvent>();
            SetEventKey<LastInitializationEvent>();
            SetEventKey<LastPostLateUpdateEvent>();
            SetEventKey<LastPreUpdateEvent>();
            SetEventKey<LastUpdateEvent>();
            SetEventKey<PostLateUpdateEvent>();
            SetEventKey<PostTimeUpdateEvent>();
            SetEventKey<PreLateUpdateEvent>();
            SetEventKey<PreTimeUpdateEvent>();
            SetEventKey<SceneLoadedEvent>();
        }
    }
}
