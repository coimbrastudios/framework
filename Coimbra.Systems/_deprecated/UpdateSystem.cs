using System;
using System.Collections.Generic;
using UnityEngine;

namespace Coimbra.Systems
{
    /// <summary>
    /// Default implementation for <see cref="IUpdateService"/>.
    /// </summary>
    [DisallowMultipleComponent]
    [Obsolete(nameof(UpdateSystem) + " has been deprecated. Use " + nameof(UpdateEvent) + " through the " + nameof(IEventService) + " instead.")]
    public class UpdateSystem : UpdateSystemBase<IUpdateService, IUpdateListener>, IUpdateService
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
