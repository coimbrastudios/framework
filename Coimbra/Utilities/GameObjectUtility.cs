#nullable enable

using System;
using System.Diagnostics.CodeAnalysis;
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
        private const string PersistentSceneName = "DontDestroyOnLoad";

        /// <summary>
        /// Destroys the <see cref="GameObject"/> correctly by checking if it isn't already an <see cref="Actor"/> first.
        /// </summary>
        /// <seealso cref="Actor.Dispose"/>
        public static ObjectDisposeResult Dispose(this GameObject? gameObject, bool forceDestroy)
        {
            if (!gameObject.TryGetValid(out gameObject))
            {
                return ObjectDisposeResult.None;
            }

            if (Actor.HasCachedActor(gameObject, out Actor actor))
            {
                return ObjectUtility.Dispose(actor, forceDestroy);
            }

            if (ApplicationUtility.IsPlayMode)
            {
#pragma warning disable COIMBRA0008
                Object.Destroy(gameObject);
#pragma warning restore COIMBRA0008
            }
            else
            {
                Object.DestroyImmediate(gameObject);
            }

            return ObjectDisposeResult.Destroyed;
        }

        /// <summary>
        /// Gets or adds the given component type.
        /// </summary>
        public static T GetOrAddComponent<T>(this GameObject gameObject)
            where T : Component
        {
            return gameObject.TryGetComponent(out T component) ? component : gameObject.AddComponent<T>();
        }

        /// <summary>
        /// Gets or adds the given component type.
        /// </summary>
        public static Component GetOrAddComponent(this GameObject gameObject, SerializableType<Component> type)
        {
            return gameObject.TryGetComponent(type.Value, out Component component) ? component : gameObject.AddComponent(type.Value);
        }

        /// <summary>
        /// Gets the <see cref="Actor"/> representing a <see cref="GameObject"/>, creating a default <see cref="Actor"/> for it if missing.
        /// </summary>
        public static Actor GetOrInitializeActor(this GameObject gameObject)
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
        public static T? GetOrInitializeActor<T>(this GameObject gameObject)
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
        public static bool HasComponent(this GameObject gameObject, Type type)
        {
            return gameObject.TryGetComponent(type, out _);
        }

        /// <summary>
        /// Checks if the <see cref="Actor"/> representing a <see cref="GameObject"/> is of the specified type.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsActor<T>(this GameObject gameObject)
            where T : class
        {
            return Actor.HasCachedActor(gameObject, out Actor? raw) && raw is T;
        }

        /// <summary>
        /// Checks if the <see cref="Actor"/> representing a <see cref="GameObject"/> if of the specified type, returning it if true.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsActor<T>(this GameObject gameObject, [NotNullWhen(true)] out T? actor)
            where T : class
        {
            if (Actor.HasCachedActor(gameObject, out Actor? raw) && raw is T result)
            {
                actor = result;

                return true;
            }

            actor = null;

            return false;
        }

        /// <summary>
        /// Check if object is currently persistent.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsPersistent(this GameObject gameObject)
        {
            Scene scene = gameObject.scene;

            return scene is { buildIndex: -1, name: PersistentSceneName };
        }
    }
}
