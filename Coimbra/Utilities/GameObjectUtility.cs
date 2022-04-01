using System.Collections.Generic;
using UnityEngine;

namespace Coimbra
{
    /// <summary>
    /// Utility methods for <see cref="GameObject"/> type.
    /// </summary>
    public static class GameObjectUtility
    {
        private static readonly Dictionary<GameObjectID, GameObjectBehaviour> Behaviours = new Dictionary<GameObjectID, GameObjectBehaviour>();

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

            behaviour.Initialize();

            return behaviour;
        }

        internal static void AddCachedBehaviour(GameObjectBehaviour behaviour)
        {
            Behaviours.Add(behaviour.CachedGameObject, behaviour);
        }

        internal static void RemoveCachedBehaviour(GameObjectID id)
        {
            Behaviours.Remove(id);
        }
    }
}
