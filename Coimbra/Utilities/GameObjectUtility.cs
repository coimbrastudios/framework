using JetBrains.Annotations;
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

        /// <summary>
        /// Tries to get the specified type of <see cref="Actor"/> for a <see cref="GameObject"/>, initializing it as the specified type if no <see cref="Actor"/> component is found.
        /// </summary>
        [CanBeNull]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TActor AsActor<TActor>(this GameObject gameObject)
            where TActor : Actor
        {
            if (Actor.HasCachedActor(gameObject, out Actor actor))
            {
                return actor as TActor;
            }

            if (!gameObject.TryGetComponent(out actor))
            {
                actor = gameObject.AddComponent<TActor>();
            }

            actor.Initialize();

            return actor as TActor;
        }

        /// <summary>
        /// Gets the <see cref="Actor"/> representing a <see cref="GameObject"/>, creating a default <see cref="Actor"/> for it if missing.
        /// </summary>
        [NotNull]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Actor AsActor(this GameObject gameObject)
        {
            if (Actor.HasCachedActor(gameObject, out Actor actor))
            {
                return actor;
            }

            if (!gameObject.TryGetComponent(out actor))
            {
                actor = gameObject.AddComponent<Actor>();
            }

            actor.Initialize();

            return actor;
        }

        /// <summary>
        /// Destroys the <see cref="GameObject"/> correctly by checking if it isn't already an <see cref="Actor"/> first.
        /// </summary>
        public static bool Destroy(this GameObject gameObject)
        {
            if (!gameObject.TryGetValid(out gameObject))
            {
                return false;
            }

            if (Actor.HasCachedActor(gameObject, out Actor actor))
            {
                actor.Destroy();
            }
            else if (CoimbraUtility.IsPlayMode)
            {
#pragma warning disable COIMBRA0019
                Object.Destroy(gameObject);
#pragma warning restore COIMBRA0019
            }
            else
            {
                Object.DestroyImmediate(gameObject);
            }

            return true;
        }

        /// <summary>
        /// Checks if the <see cref="Actor"/> representing a <see cref="GameObject"/> if of the specified type.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsActor<TActor>(this GameObject gameObject)
            where TActor : Actor
        {
            return AsActor(gameObject) is TActor;
        }

        /// <summary>
        /// Checks if the <see cref="Actor"/> representing a <see cref="GameObject"/> if of the specified type, returning it if true.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsActor<TActor>(this GameObject gameObject, [CanBeNull] out TActor result)
            where TActor : Actor
        {
            if (AsActor(gameObject) is TActor actor)
            {
                result = actor;

                return true;
            }

            result = null;

            return false;
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
    }
}
