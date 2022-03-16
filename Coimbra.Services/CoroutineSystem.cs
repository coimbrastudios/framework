using UnityEngine;

namespace Coimbra.Services
{
    /// <summary>
    /// Default implementation for <see cref="ICoroutineService"/>.
    /// </summary>
    [AddComponentMenu("")]
    [DisallowMultipleComponent]
    public class CoroutineSystem : MonoBehaviour, ICoroutineService
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Initialize()
        {
            ServiceLocator.SetDefaultCreateCallback(Create, false);
        }

        private static ICoroutineService Create()
        {
            GameObject gameObject = new GameObject(nameof(CoroutineSystem))
            {
                hideFlags = HideFlags.HideAndDontSave,
            };

            DontDestroyOnLoad(gameObject);

            return gameObject.AddComponent<CoroutineSystem>();
        }
    }
}
