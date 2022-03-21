using System.Runtime.CompilerServices;
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

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Initialize()
        {
            ServiceLocator.Shared.Set(Create());
        }

        private static IApplicationService Create()
        {
            GameObject gameObject = new GameObject(nameof(ApplicationSystem))
            {
                hideFlags = HideFlags.NotEditable,
            };

            DontDestroyOnLoad(gameObject);

            return gameObject.AddComponent<ApplicationSystem>();
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
        {
            _eventService?.Invoke(this, ref e, _eventKey);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ResetEventKey<T>()
        {
            _eventService.ResetEventKey<T>(_eventKey);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SetEventKey<T>()
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
