using System.Collections.Generic;
using UnityEngine;

namespace Coimbra
{
    /// <summary>
    /// Utility methods for <see cref="GameObject"/> type.
    /// </summary>
    public static class GameObjectUtility
    {
        private static readonly Dictionary<GameObject, GameObjectBehaviour> Behaviours = new Dictionary<GameObject, GameObjectBehaviour>();

        /// <summary>
        /// Gets the <see cref="GameObjectBehaviour"/> or creates a new default one if missing.
        /// </summary>
        public static GameObjectBehaviour GetOrCreateBehaviour(this Component component)
        {
            return GetOrCreateBehaviour(component.gameObject);
        }

        /// <summary>
        /// Gets the <see cref="GameObjectBehaviour"/> or creates a new default one if missing.
        /// </summary>
        public static GameObjectBehaviour GetOrCreateBehaviour(this GameObject gameObject)
        {
            if (Behaviours.TryGetValue(gameObject, out GameObjectBehaviour behaviour))
            {
                return behaviour;
            }

            if (!gameObject.TryGetComponent(out behaviour))
            {
                behaviour = gameObject.AddComponent<GameObjectBehaviour>();
            }

            behaviour.Instantiate();

            return behaviour;
        }

        internal static void AddCachedBehaviour(this GameObject gameObject, GameObjectBehaviour behaviour)
        {
            Behaviours.Add(gameObject, behaviour);
        }

        internal static void RemoveCachedBehaviour(this GameObject gameObject)
        {
            Behaviours.Remove(gameObject);
        }

        private static void HandleGameObjectDestroyed(GameObject sender, DestroyReason reason)
        {
            Behaviours.Remove(sender);
        }
    }
}
