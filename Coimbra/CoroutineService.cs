using System.Collections;
using UnityEngine;

namespace Coimbra
{
    /// <summary>
    /// Provides easy access to Unity's <see cref="Coroutine"/> system without requiring the object to be an <see cref="MonoBehaviour"/>.
    /// </summary>
    public interface ICoroutineService
    {
        /// <inheritdoc cref="MonoBehaviour.StartCoroutine(IEnumerator)"/>
        Coroutine StartCoroutine(IEnumerator routine);

        /// <inheritdoc cref="MonoBehaviour.StopCoroutine(Coroutine)"/>
        void StopCoroutine(Coroutine routine);

        /// <inheritdoc cref="MonoBehaviour.StopAllCoroutines()"/>
        void StopAllCoroutines();
    }

    [AddComponentMenu("")]
    [DisallowMultipleComponent]
    internal sealed class CoroutineService : MonoBehaviour, ICoroutineService
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Initialize()
        {
            if (ServiceLocator.Global.IsCreated<ICoroutineService>() || ServiceLocator.Global.HasCreateCallback<ICoroutineService>())
            {
                return;
            }

            ServiceLocator.Global.SetCreateCallback(Create, true);
        }

        private static ICoroutineService Create()
        {
            GameObject gameObject = new GameObject(nameof(CoroutineService));
            DontDestroyOnLoad(gameObject);

            return gameObject.AddComponent<CoroutineService>();
        }
    }
}
