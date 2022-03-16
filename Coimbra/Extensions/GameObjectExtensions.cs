using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Coimbra
{
    public static class GameObjectExtensions
    {
        private static readonly Dictionary<GameObject, GameObjectEventListener> EventListenerFromGameObject = new Dictionary<GameObject, GameObjectEventListener>();

        public static void AddActiveStateChangedListener(this GameObject gameObject, UnityAction<GameObjectEventListener, bool> callback)
        {
            gameObject.GetOrCreateCachedEventListener().OnActiveStateChanged += callback;
        }

        public static void AddDestroyEventListener(this GameObject gameObject, UnityAction<GameObjectEventListener, DestroyEventType> callback)
        {
            gameObject.GetOrCreateCachedEventListener().OnDestroyEvent += callback;
        }

        public static GameObjectEventListener GetOrCreateCachedEventListener(this GameObject gameObject)
        {
            if (EventListenerFromGameObject.TryGetValue(gameObject, out GameObjectEventListener eventListener))
            {
                return eventListener;
            }

            if (!gameObject.TryGetComponent(out eventListener))
            {
                eventListener = gameObject.AddComponent<GameObjectEventListener>();
            }

            eventListener.OnDestroyEvent += HandleGameObjectDestroyEvent;
            EventListenerFromGameObject.Add(gameObject, eventListener);

            return eventListener;
        }

        public static void RemoveActiveStateChangedListener(this GameObject gameObject, UnityAction<GameObjectEventListener, bool> callback)
        {
            gameObject.GetOrCreateCachedEventListener().OnActiveStateChanged -= callback;
        }

        public static void RemoveDestroyEventListener(this GameObject gameObject, UnityAction<GameObjectEventListener, DestroyEventType> callback)
        {
            gameObject.GetOrCreateCachedEventListener().OnDestroyEvent -= callback;
        }

        private static void HandleGameObjectDestroyEvent(GameObjectEventListener sender, DestroyEventType eventType)
        {
            EventListenerFromGameObject.Remove(sender.gameObject);
        }
    }
}
