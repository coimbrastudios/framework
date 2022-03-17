using UnityEngine;

namespace Coimbra.Systems
{
    /// <summary>
    /// Default implementation for <see cref="ICoroutineService"/>.
    /// </summary>
    [AddComponentMenu("")]
    [DisallowMultipleComponent]
    public class CoroutineSystem : MonoBehaviourServiceBase<ICoroutineService>, ICoroutineService
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
            GameObject gameObject = new GameObject(nameof(CoroutineSystem))
            {
                hideFlags = HideFlags.NotEditable,
            };

            DontDestroyOnLoad(gameObject);

            return gameObject.AddComponent<CoroutineSystem>();
        }
    }
}
