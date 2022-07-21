using JetBrains.Annotations;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Coimbra
{
    /// <summary>
    /// Utility methods for <see cref="GameObject"/> type.
    /// </summary>
    public static class GameObjectUtility
    {
        private const string DontDestroyOnLoadScene = "DontDestroyOnLoad";

        /// <inheritdoc cref="GetActor"/>
        [NotNull]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Actor AsActor(this GameObject gameObject)
        {
            return gameObject.GetActor();
        }

        /// <inheritdoc cref="GetActor{T}"/>
        [CanBeNull]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T AsActor<T>(this GameObject gameObject)
            where T : Actor
        {
            return gameObject.GetActor<T>();
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
#pragma warning disable COIMBRA0008
                Object.Destroy(gameObject);
#pragma warning restore COIMBRA0008
            }
            else
            {
                Object.DestroyImmediate(gameObject);
            }

            return true;
        }

        /// <summary>
        /// Gets the <see cref="Actor"/> representing a <see cref="GameObject"/>, creating a default <see cref="Actor"/> for it if missing.
        /// </summary>
        [NotNull]
        public static Actor GetActor(this GameObject gameObject)
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
        /// Tries to get the specified type of <see cref="Actor"/> for a <see cref="GameObject"/>, initializing it as the specified type if no <see cref="Actor"/> component is found.
        /// </summary>
        [CanBeNull]
        public static T GetActor<T>(this GameObject gameObject)
            where T : Actor
        {
            if (Actor.HasCachedActor(gameObject, out Actor actor))
            {
                return actor as T;
            }

            if (!gameObject.TryGetComponent(out actor))
            {
                actor = gameObject.AddComponent<T>();
            }

            actor.Initialize();

            return actor as T;
        }

        /// <summary>
        /// Gets or adds the given component type.
        /// </summary>
        [NotNull]
        public static T GetOrAddComponent<T>(this GameObject gameObject)
            where T : Component
        {
            return gameObject.TryGetComponent(out T component) ? component : gameObject.AddComponent<T>();
        }

        /// <summary>
        /// Gets or adds the given component type.
        /// </summary>
        [NotNull]
        public static Component GetOrAddComponent(this GameObject gameObject, SerializableType<Component> type)
        {
            return gameObject.TryGetComponent(type.Value, out Component component) ? component : gameObject.AddComponent(type.Value);
        }

        /// <summary>
        /// Checks if the game object have the given component type.
        /// </summary>
        public static bool HasComponent<T>(this GameObject gameObject)
        {
#pragma warning disable UNT0014
            return gameObject.TryGetComponent<T>(out _);
#pragma warning restore UNT0014
        }

        /// <summary>
        /// Checks if the game object have the given component type.
        /// </summary>
        public static bool HasComponent(this GameObject gameObject, [NotNull] Type type)
        {
            return gameObject.TryGetComponent(type, out _);
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
