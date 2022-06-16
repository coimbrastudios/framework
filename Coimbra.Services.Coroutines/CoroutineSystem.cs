#nullable enable

using UnityEngine;

namespace Coimbra.Services.Coroutines
{
    /// <summary>
    /// Default implementation for <see cref="ICoroutineService"/>.
    /// </summary>
    [AddComponentMenu("")]
    public sealed class CoroutineSystem : Actor, ICoroutineService
    {
        private CoroutineSystem() { }

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
            DontDestroyOnLoad(GameObject);
        }
    }
}
