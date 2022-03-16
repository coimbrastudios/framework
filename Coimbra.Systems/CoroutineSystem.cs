using System;
using UnityEngine;

namespace Coimbra.Systems
{
    /// <summary>
    /// Default implementation for <see cref="ICoroutineService"/>.
    /// </summary>
    [AddComponentMenu("")]
    [DisallowMultipleComponent]
    public class CoroutineSystem : MonoBehaviour, ICoroutineService
    {
        /// <inheritdoc cref="IDisposable.Dispose"/>
        public void Dispose()
        {
            StopAllCoroutines();

            if (gameObject != null)
            {
                Destroy(gameObject);
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Initialize()
        {
            ServiceLocator.SetDefaultCreateCallback(Create, false);
        }

        private static ICoroutineService Create()
        {
            GameObject gameObject = new GameObject(nameof(CoroutineSystem))
            {
                hideFlags = HideFlags.NotEditable,
            };

            DontDestroyOnLoad(gameObject);

            return gameObject.AddComponent<CoroutineSystem>();
        }
    }
}
