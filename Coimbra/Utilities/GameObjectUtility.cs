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
        /// Creates and initializes a new default <see cref="GameObjectBehaviour"/>.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static GameObjectBehaviour CreateBehaviour(this GameObject gameObject)
        {
            GameObjectBehaviour behaviour = gameObject.AddComponent<GameObjectBehaviour>();
            behaviour.Initialize();

            return behaviour;
        }

        /// <summary>
        /// Creates and initializes a new <see cref="GameObjectBehaviour"/> of the specified type.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T CreateBehaviour<T>(this GameObject gameObject)
            where T : GameObjectBehaviour
        {
            T behaviour = gameObject.AddComponent<T>();
            behaviour.Initialize();

            return behaviour;
        }

        /// <summary>
        /// Gets the <see cref="GameObjectBehaviour"/> or creates a new default one if missing.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static GameObjectBehaviour GetOrCreateBehaviour(this GameObject gameObject)
        {
            return TryGetBehaviour(gameObject, out GameObjectBehaviour behaviour) ? behaviour : CreateBehaviour(gameObject);
        }

        /// <summary>
        /// Gets the <see cref="GameObjectBehaviour"/> or creates a new one of the specified type if missing.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T GetOrCreateBehaviour<T>(this GameObject gameObject)
            where T : GameObjectBehaviour
        {
            return TryGetBehaviour(gameObject, out GameObjectBehaviour behaviour) ? behaviour as T : CreateBehaviour<T>(gameObject);
        }

        /// <summary>
        /// Tries to get an existing <see cref="GameObjectBehaviour"/>.
        /// </summary>
        /// <returns>True if found a <see cref="GameObjectBehaviour"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetBehaviour(this GameObject gameObject, out GameObjectBehaviour behaviour)
        {
            if (Behaviours.TryGetValue(gameObject, out behaviour))
            {
                return true;
            }

            if (!gameObject.TryGetComponent(out behaviour))
            {
                return false;
            }

            behaviour.Initialize();

            return true;
        }

        /// <summary>
        /// Tries to get an existing <see cref="GameObjectBehaviour"/>.
        /// </summary>
        /// <returns>True if found a <see cref="GameObjectBehaviour"/>, event if of the wrong type.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetBehaviour<T>(this GameObject gameObject, out T behaviour)
            where T : GameObjectBehaviour
        {
            if (TryGetBehaviour(gameObject, out GameObjectBehaviour value))
            {
                behaviour = value as T;

                return true;
            }

            behaviour = null;

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void AddCachedBehaviour(GameObjectBehaviour behaviour)
        {
            Behaviours.Add(behaviour.CachedGameObject, behaviour);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void RemoveCachedBehaviour(GameObjectID id)
        {
            Behaviours.Remove(id);
        }
    }
}
