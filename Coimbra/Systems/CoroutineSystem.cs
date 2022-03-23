using UnityEngine;

namespace Coimbra
{
    [AddComponentMenu("")]
    [DisallowMultipleComponent]
    internal sealed class CoroutineSystem : MonoBehaviourServiceBase<ICoroutineService>, ICoroutineService
    {
        /// <inheritdoc/>
        protected override void OnDispose()
        {
            base.OnDispose();
            StopAllCoroutines();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Initialize()
        {
            ServiceLocator.SetDefaultCreateCallback(Create, false);
        }

        private static ICoroutineService Create()
        {
            GameObject gameObject = new GameObject(nameof(CoroutineSystem));
            DontDestroyOnLoad(gameObject);

            return gameObject.AddComponent<CoroutineSystem>();
        }
    }
}
