using System.Collections.Generic;
using UnityEngine;

namespace Coimbra.Services
{
    /// <summary>
    /// Default implementation for <see cref="ILateUpdateService"/>.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class LateUpdateService : UpdateServiceBase<ILateUpdateListener>, ILateUpdateService
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Initialize()
        {
            ServiceLocator.SetDefaultCreateCallback(Create, false);
        }

        private static ILateUpdateService Create()
        {
            GameObject gameObject = new GameObject(nameof(LateUpdateService));
            DontDestroyOnLoad(gameObject);

            return gameObject.AddComponent<LateUpdateService>();
        }

        private void LateUpdate()
        {
            float deltaTime = Time.deltaTime;
            IReadOnlyList<ILateUpdateListener> listeners = Listeners;
            int listenersCount = listeners.Count;

            for (int i = 0; i < listenersCount; i++)
            {
                listeners[i].OnLateUpdate(deltaTime);
            }
        }
    }
}
