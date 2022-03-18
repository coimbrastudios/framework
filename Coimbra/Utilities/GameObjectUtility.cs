using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Coimbra
{
    /// <summary>
    /// Utility methods for <see cref="GameObject"/> type.
    /// </summary>
    public static class GameObjectUtility
    {
        private static readonly Dictionary<GameObject, GameObjectEventListener> EventListenerFromGameObject = new Dictionary<GameObject, GameObjectEventListener>();

        /// <summary>
        /// Add listener to the <see cref="GameObjectEventListener.OnActiveStateChanged"/> event.
        /// </summary>
        public static void AddActiveStateChangedListener(this GameObject gameObject, UnityAction<GameObject, bool> callback)
        {
            gameObject.GetOrCreateCachedEventListener().OnActiveStateChanged += callback;
        }

        /// <summary>
        /// Add listener to the <see cref="GameObjectEventListener.OnDestroyEvent"/> event.
        /// </summary>
        public static void AddDestroyEventListener(this GameObject gameObject, UnityAction<GameObject, DestroyEventType> callback)
        {
            gameObject.GetOrCreateCachedEventListener().OnDestroyEvent += callback;
        }

        /// <summary>
        /// Remove listener to the <see cref="GameObjectEventListener.OnDestroyEvent"/> event.
        /// </summary>
        public static void RemoveActiveStateChangedListener(this GameObject gameObject, UnityAction<GameObject, bool> callback)
        {
            gameObject.GetOrCreateCachedEventListener().OnActiveStateChanged -= callback;
        }

        /// <summary>
        /// Remove listener to the <see cref="GameObjectEventListener.OnDestroyEvent"/> event.
        /// </summary>
        public static void RemoveDestroyEventListener(this GameObject gameObject, UnityAction<GameObject, DestroyEventType> callback)
        {
            gameObject.GetOrCreateCachedEventListener().OnDestroyEvent -= callback;
        }

        internal static void RemoveCachedEventListener(this GameObject gameObject)
        {
            EventListenerFromGameObject.Remove(gameObject);
        }

        private static GameObjectEventListener GetOrCreateCachedEventListener(this GameObject gameObject)
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

        private static void HandleGameObjectDestroyEvent(GameObject sender, DestroyEventType eventType)
        {
            EventListenerFromGameObject.Remove(sender);
        }
    }
}
