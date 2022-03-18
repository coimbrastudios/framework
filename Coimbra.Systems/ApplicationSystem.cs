using System.Runtime.CompilerServices;
using UnityEngine;

namespace Coimbra.Systems
{
    /// <summary>
    /// Default implementation for <see cref="IApplicationService"/>.
    /// </summary>
    [AddComponentMenu("")]
    [DisallowMultipleComponent]
    public class ApplicationSystem : MonoBehaviourServiceBase<IApplicationService>, IApplicationService
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

        protected void FixedUpdate()
        {
            Invoke(new FixedUpdateEvent(Time.deltaTime));
        }

        protected void Update()
        {
            Invoke(new UpdateEvent(Time.deltaTime));
        }

        protected void LateUpdate()
        {
            Invoke(new LateUpdateEvent(Time.deltaTime));
        }

        protected void OnApplicationFocus(bool hasFocus)
        {
            Invoke(new ApplicationFocusEvent(hasFocus));
        }

        protected void OnApplicationPause(bool pauseStatus)
        {
            Invoke(new ApplicationPauseEvent(pauseStatus));
        }

        protected void OnApplicationQuit()
        {
            Invoke(new ApplicationQuitEvent());
#if UNITY_EDITOR
            OwningLocator?.Dispose();
#endif
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Initialize()
        {
            ServiceLocator.Shared.Set(Create());
            ServiceLocator.SetDefaultCreateCallback(Create, false);
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
