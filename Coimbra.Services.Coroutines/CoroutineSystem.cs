#nullable enable

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

        /// <inheritdoc/>
        protected override void OnDispose()
        {
            base.OnDispose();
            StopAllCoroutines();
        }

        /// <inheritdoc/>
        protected override void OnInitialize()
        {
            base.OnInitialize();
            DontDestroyOnLoad(GameObject);
        }
    }
}
