using UnityEngine;

namespace Coimbra.Services.Coroutines
{
    /// <summary>
    /// Default implementation for <see cref="ICoroutineService"/>.
    /// </summary>
    [AddComponentMenu("")]
    public sealed class CoroutineSystem : ServiceActorBase<CoroutineSystem, ICoroutineService>, ICoroutineService
    {
        private CoroutineSystem() { }

        /// <summary>
        /// Create a new <see cref="ICoroutineService"/>.
        /// </summary>
        public static ICoroutineService Create()
        {
            return new GameObject(nameof(CoroutineSystem)).AddComponent<CoroutineSystem>();
        }

        /// <inheritdoc/>
        protected override void OnDestroyed()
        {
            base.OnDestroyed();
            StopAllCoroutines();
        }

        /// <inheritdoc/>
        protected override void OnInitialize()
        {
            base.OnInitialize();
            DontDestroyOnLoad(CachedGameObject);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void HandleSubsystemRegistration()
        {
            ServiceLocator.Shared.SetCreateCallback(Create, false);
        }
    }
}
