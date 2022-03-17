using System;
using System.Collections.Generic;
using UnityEngine;

namespace Coimbra.Systems
{
    /// <summary>
    /// Default implementation for <see cref="IFixedUpdateService"/>.
    /// </summary>
    [DisallowMultipleComponent]
    [Obsolete(nameof(FixedUpdateSystem) + " has been deprecated. Use " + nameof(FixedUpdateEvent) + " through the " + nameof(IEventService) + " instead.")]
    public class FixedUpdateSystem : UpdateSystemBase<IFixedUpdateService, IFixedUpdateListener>, IFixedUpdateService
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Initialize()
        {
            ServiceLocator.SetDefaultCreateCallback(Create, false);
        }

        private static IFixedUpdateService Create()
        {
            GameObject gameObject = new GameObject(nameof(FixedUpdateSystem))
            {
                hideFlags = HideFlags.NotEditable,
            };

            DontDestroyOnLoad(gameObject);

            return gameObject.AddComponent<FixedUpdateSystem>();
        }

        private void FixedUpdate()
        {
            float deltaTime = Time.deltaTime;
            IReadOnlyList<IFixedUpdateListener> listeners = Listeners;
            int listenersCount = listeners.Count;

            for (int i = 0; i < listenersCount; i++)
            {
                listeners[i].OnFixedUpdate(deltaTime);
            }
        }
    }
}
