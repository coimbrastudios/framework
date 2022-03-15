using System.Collections.Generic;
using UnityEngine;

namespace Coimbra.Services
{
    /// <summary>
    /// Default implementation for <see cref="IUpdateService"/>.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class UpdateService : UpdateServiceBase<IUpdateListener>, IUpdateService
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Initialize()
        {
            ServiceLocator.SetDefaultCreateCallback(Create, false);
        }

        private static IUpdateService Create()
        {
            GameObject gameObject = new GameObject(nameof(UpdateService));
            DontDestroyOnLoad(gameObject);

            return gameObject.AddComponent<UpdateService>();
        }

        private void Update()
        {
            float deltaTime = Time.deltaTime;
            IReadOnlyList<IUpdateListener> listeners = Listeners;
            int listenersCount = listeners.Count;

            for (int i = 0; i < listenersCount; i++)
            {
                listeners[i].OnUpdate(deltaTime);
            }
        }
    }
}
