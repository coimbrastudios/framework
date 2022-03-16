using System.Collections.Generic;
using UnityEngine;

namespace Coimbra.Systems
{
    /// <summary>
    /// Default implementation for <see cref="IUpdateService"/>.
    /// </summary>
    [DisallowMultipleComponent]
    public class UpdateSystem : UpdateSystemBase<IUpdateListener>, IUpdateService
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Initialize()
        {
            ServiceLocator.SetDefaultCreateCallback(Create, false);
        }

        private static IUpdateService Create()
        {
            GameObject gameObject = new GameObject(nameof(UpdateSystem))
            {
                hideFlags = HideFlags.NotEditable,
            };

            DontDestroyOnLoad(gameObject);

            return gameObject.AddComponent<UpdateSystem>();
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
