using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Coimbra
{
    /// <summary>
    /// Utility methods for <see cref="GameObject"/> type.
    /// </summary>
    public static class GameObjectUtility
    {
        private const string DontDestroyOnLoadScene = "DontDestroyOnLoad";
        private static readonly Dictionary<GameObjectID, GameObjectBehaviour> Behaviours = new Dictionary<GameObjectID, GameObjectBehaviour>();

        /// <summary>
        /// Check if object is currently persistent.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsPersistent(this GameObjectBehaviour behaviour)
        {
            return IsPersistent(behaviour.CachedGameObject);
        }

        /// <summary>
        /// Check if object is currently persistent.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsPersistent(this GameObject gameObject)
        {
            Scene scene = gameObject.scene;

            return scene.buildIndex == -1 && scene.name == DontDestroyOnLoadScene;
        }

        /// <summary>
        /// Gets the <see cref="GameObjectBehaviour"/> or creates a new default one if missing.
        /// </summary>
        public static GameObjectBehaviour GetOrCreateBehaviour(this Component component)
        {
            return GetOrCreateBehaviour<GameObjectBehaviour>(component.gameObject);
        }

        /// <summary>
        /// Gets the <see cref="GameObjectBehaviour"/> or creates a new default one if missing.
        /// </summary>
        public static T GetOrCreateBehaviour<T>(this Component component)
            where T : GameObjectBehaviour
        {
            return GetOrCreateBehaviour<T>(component.gameObject);
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

        /// <summary>
        /// Gets the <see cref="GameObjectBehaviour"/> or creates a new default one if missing.
        /// </summary>
        public static T GetOrCreateBehaviour<T>(this GameObject gameObject)
            where T : GameObjectBehaviour
        {
            if (Behaviours.TryGetValue(gameObject, out GameObjectBehaviour behaviour))
            {
                return (T)behaviour;
            }

            if (!gameObject.TryGetComponent(out behaviour))
            {
                behaviour = gameObject.AddComponent<T>();
            }

            behaviour.Initialize();

            return (T)behaviour;
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
