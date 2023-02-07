#nullable enable

using System.Collections;
using UnityEngine;

namespace Coimbra.Services.Coroutines
{
    /// <summary>
    /// Default implementation for <see cref="ICoroutineService"/>.
    /// </summary>
    /// <remarks>
    /// Forwards the implementation implicitly to <see cref="MonoBehaviour.StartCoroutine(IEnumerator)"/>, <see cref="MonoBehaviour.StopCoroutine(Coroutine)"/>, and <see cref="MonoBehaviour.StopAllCoroutines"/>.
    /// </remarks>
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
